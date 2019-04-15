namespace Squalr.Engine.Scanning.Scanners.Pointers.SearchKernels
{
    using Squalr.Engine.Common.Extensions;
    using Squalr.Engine.Common.OS;
    using Squalr.Engine.Scanning.Scanners.Pointers.Structures;
    using Squalr.Engine.Scanning.Snapshots;
    using System;
    using System.Linq;
    using System.Numerics;

    internal class EytzingerSearchKernel : IVectorSearchKernel
    {
        public EytzingerSearchKernel(Snapshot boundsSnapshot, UInt32 maxOffset, PointerSize pointerSize)
        {
            this.BoundsSnapshot = boundsSnapshot;
            this.MaxOffset = maxOffset;

            this.L = 1 + this.Log2(2 + this.BoundsSnapshot.SnapshotRegions.Length + 1); // Final +1 due to inversion
            this.M = new Vector<UInt32>(unchecked((UInt32)(~(2 * this.L))));

            this.Length = 2 << (this.L + 2) - 1;

            this.LowerBounds = this.GetInverseLowerBounds();
            this.UpperBounds = this.GetInverseUpperBounds();

            this.YArray = new UInt32[Vectors.VectorSize / sizeof(UInt32)];
            this.LArray = new UInt32[Vectors.VectorSize / sizeof(UInt32)];
            this.UArray = new UInt32[Vectors.VectorSize / sizeof(UInt32)];
        }

        private Snapshot BoundsSnapshot { get; set; }

        private UInt32 MaxOffset { get; set; }

        private UInt32[] LowerBounds { get; set; }

        private UInt32[] UpperBounds { get; set; }

        private UInt32[] YArray { get; set; }

        private UInt32[] LArray { get; set; }

        private UInt32[] UArray { get; set; }

        private Int32 Length { get; set; }

        private Int32 L { get; set; }

        private Vector<UInt32> M { get; set; }

        private Vector<UInt32> Two = new Vector<UInt32>(2);

        public Func<Vector<Byte>> GetSearchKernel(SnapshotElementVectorComparer snapshotElementVectorComparer)
        {
            return new Func<Vector<Byte>>(() =>
            {
                Vector<UInt32> z = Vector.AsVectorUInt32(snapshotElementVectorComparer.CurrentValues);
                Vector<UInt32> heapRoot = new Vector<UInt32>(this.LowerBounds[0]);
                Vector<UInt32> P = Vector.ConditionalSelect(Vector.GreaterThanOrEqual(z, heapRoot), this.Two, Vector<UInt32>.One);
                Int32 l = this.L;

                while (l > 1)
                {
                    for (Int32 index = 0; index < Vectors.VectorSize / sizeof(UInt32); index++)
                    {
                        this.YArray[index] = this.LowerBounds[P[index]];
                    }

                    Vector<UInt32> YP = new Vector<UInt32>(this.YArray);
                    Vector<UInt32> Q = Vector.ConditionalSelect(Vector.GreaterThanOrEqual(z, YP), this.Two, Vector<UInt32>.One);

                    P = Vector.Add(Vector.Multiply(P, this.Two), Q);
                    l--;
                }

                Vector<UInt32> i = Vector.BitwiseAnd(P, M);

                for (Int32 index = 0; index < Vectors.VectorSize / sizeof(UInt32); index++)
                {
                    UInt32 newIndex = i[index];
                    this.LArray[index] = this.LowerBounds[newIndex];
                    this.UArray[index] = this.UpperBounds[newIndex];
                }

                return Vector.AsVectorByte(Vector.Negate(Vector.BitwiseAnd(Vector.GreaterThanOrEqual(z, new Vector<UInt32>(this.LArray)), Vector.LessThanOrEqual(z, new Vector<UInt32>(this.UArray)))));
            });
        }

        public UInt32[] GetInverseLowerBounds()
        {
            BinaryHeap<UInt32> lowerBoundsHeap = new BinaryHeap<UInt32>();

            foreach (UInt32 next in this.BoundsSnapshot.SnapshotRegions.Select(region => unchecked((UInt32)region.EndAddress.Subtract(this.MaxOffset, wrapAround: false))).Prepend(UInt32.MinValue))
            {
                lowerBoundsHeap.Insert(next);
            }

            // Force power of 2 alignment
            while (lowerBoundsHeap.Count < this.Length)
            {
                lowerBoundsHeap.Insert(UInt32.MinValue);
            }

            return lowerBoundsHeap.ToArray();
        }

        public UInt32[] GetInverseUpperBounds()
        {
            BinaryHeap<UInt32> upperBoundsHeap = new BinaryHeap<UInt32>();

            foreach (UInt32 next in this.BoundsSnapshot.SnapshotRegions.Select(region => unchecked((UInt32)region.BaseAddress.Add(this.MaxOffset, wrapAround: false))).Append(UInt32.MaxValue))
            {
                upperBoundsHeap.Insert(next);
            }

            // Force power of 2 alignment
            while (upperBoundsHeap.Count < this.Length)
            {
                upperBoundsHeap.Insert(UInt32.MaxValue);
            }

            return upperBoundsHeap.ToArray();
        }

        private Int32 Log2(Int32 x)
        {
            Int32 lowBitCount = 0;

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