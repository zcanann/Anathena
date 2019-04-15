namespace Squalr.Engine.Scanning.Scanners.Pointers.SearchKernels
{
    using Squalr.Engine.Common.Extensions;
    using Squalr.Engine.Common.OS;
    using Squalr.Engine.Scanning.Scanners.Pointers.Structures;
    using Squalr.Engine.Scanning.Snapshots;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;

    internal class BinarySearchKernel : IVectorSearchKernel
    {
        public BinarySearchKernel(Snapshot boundsSnapshot, UInt32 maxOffset, PointerSize pointerSize)
        {
            this.BoundsSnapshot = boundsSnapshot;
            this.MaxOffset = maxOffset;

            this.PowerOf2Padding = this.Log2((UInt32)this.BoundsSnapshot.SnapshotRegions.Length) << 1;

            this.LowerBounds = this.GetLowerBounds();
            this.UpperBounds = this.GetUpperBounds();

            this.LArray = new UInt32[Vectors.VectorSize / sizeof(UInt32)];
            this.UArray = new UInt32[Vectors.VectorSize / sizeof(UInt32)];
        }

        private Snapshot BoundsSnapshot { get; set; }

        private UInt32 MaxOffset { get; set; }

        private UInt32[] LowerBounds { get; set; }

        private UInt32[] UpperBounds { get; set; }

        private UInt32[] LArray { get; set; }

        private UInt32[] UArray { get; set; }

        private UInt32 PowerOf2Padding { get; set; }

        public Func<Vector<Byte>> GetSearchKernel(SnapshotElementVectorComparer snapshotElementVectorComparer)
        {
            return new Func<Vector<Byte>>(() =>
            {
                UInt32 halfIndex = this.PowerOf2Padding >> 1;
                Vector<UInt32> currentValues = Vector.AsVectorUInt32(snapshotElementVectorComparer.CurrentValues);
                Vector<UInt32> discoveredIndicies = Vector.ConditionalSelect(Vector.GreaterThan(currentValues, new Vector<UInt32>(this.UpperBounds[halfIndex])), new Vector<UInt32>(halfIndex), Vector<UInt32>.Zero);

                while (halfIndex > 1)
                {
                    halfIndex >>= 1;

                    for (Int32 index = 0; index < Vectors.VectorSize / sizeof(UInt32); index++)
                    {
                        this.UArray[index] = this.UpperBounds[discoveredIndicies[index] + halfIndex];
                    }

                    discoveredIndicies = Vector.Add(discoveredIndicies, Vector.ConditionalSelect(Vector.GreaterThan(currentValues, new Vector<UInt32>(this.UArray)), new Vector<UInt32>(halfIndex), Vector<UInt32>.Zero));
                }

                for (Int32 index = 0; index < Vectors.VectorSize / sizeof(UInt32); index++)
                {
                    this.LArray[index] = this.LowerBounds[discoveredIndicies[index]];
                    this.UArray[index] = this.UpperBounds[discoveredIndicies[index]];
                }

                return Vector.AsVectorByte(Vector.BitwiseAnd(Vector.GreaterThanOrEqual(currentValues, new Vector<UInt32>(this.LArray)), Vector.LessThanOrEqual(currentValues, new Vector<UInt32>(this.UArray))));
            });
        }

        public UInt32[] GetLowerBounds()
        {
            IEnumerable<UInt32> lowerBounds = this.BoundsSnapshot.SnapshotRegions.Select(region => unchecked((UInt32)region.BaseAddress.Subtract(this.MaxOffset, wrapAround: false)));

            while (lowerBounds.Count() < PowerOf2Padding)
            {
                lowerBounds = lowerBounds.Append<UInt32>(UInt32.MinValue);
            }

            return lowerBounds.ToArray();
        }

        public UInt32[] GetUpperBounds()
        {
            IEnumerable<UInt32> upperBounds = this.BoundsSnapshot.SnapshotRegions.Select(region => unchecked((UInt32)region.EndAddress.Add(this.MaxOffset, wrapAround: false)));

            while (upperBounds.Count() < PowerOf2Padding)
            {
                upperBounds = upperBounds.Append<UInt32>(UInt32.MaxValue);
            }

            return upperBounds.ToArray();
        }

        private UInt32 Log2(UInt32 x)
        {
            UInt32 lowBitCount = 0;

            while (x > 0)
            {
                x >>= 1;
                lowBitCount++;
            }

            return lowBitCount;
        }
    }
    //// End class
}
//// End namespace