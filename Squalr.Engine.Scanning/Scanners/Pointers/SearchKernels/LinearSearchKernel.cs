namespace Squalr.Engine.Scanning.Scanners.Pointers.SearchKernels
{
    using Squalr.Engine.Common.Extensions;
    using Squalr.Engine.Scanning.Scanners.Pointers.Structures;
    using Squalr.Engine.Scanning.Snapshots;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;

    internal class LinearSearchKernel : IVectorSearchKernel
    {
        public LinearSearchKernel(Snapshot boundsSnapshot, UInt32 maxOffset, PointerSize pointerSize)
        {
            this.BoundsSnapshot = boundsSnapshot;
            this.MaxOffset = maxOffset;

            this.LowerBounds = this.GetLowerBounds();
            this.UpperBounds = this.GetUpperBounds();
        }

        private Snapshot BoundsSnapshot { get; set; }

        private UInt32 MaxOffset { get; set; }

        private UInt32[] LowerBounds { get; set; }

        private UInt32[] UpperBounds { get; set; }

        private const Int32 UnrollSize = 8;

        public Func<Vector<Byte>> GetSearchKernel(SnapshotElementVectorComparer snapshotElementVectorComparer)
        {
            return new Func<Vector<Byte>>(() =>
            {
                Vector<UInt32> result = Vector<UInt32>.Zero;
                Vector<UInt32> currentValues = Vector.AsVectorUInt32(snapshotElementVectorComparer.CurrentValues);

                for (Int32 boundsIndex = 0; boundsIndex < this.LowerBounds.Length; boundsIndex += LinearSearchKernel.UnrollSize)
                {
                    Vector<UInt32> result0 = Vector.BitwiseAnd(Vector.GreaterThanOrEqual(currentValues, new Vector<UInt32>(this.LowerBounds[boundsIndex + 0])),
                        Vector.LessThanOrEqual(currentValues, new Vector<UInt32>(this.UpperBounds[boundsIndex + 0])));
                    Vector<UInt32> result1 = Vector.BitwiseAnd(Vector.GreaterThanOrEqual(currentValues, new Vector<UInt32>(this.LowerBounds[boundsIndex + 1])),
                        Vector.LessThanOrEqual(currentValues, new Vector<UInt32>(this.UpperBounds[boundsIndex + 1])));
                    Vector<UInt32> result2 = Vector.BitwiseAnd(Vector.GreaterThanOrEqual(currentValues, new Vector<UInt32>(this.LowerBounds[boundsIndex + 2])),
                        Vector.LessThanOrEqual(currentValues, new Vector<UInt32>(this.UpperBounds[boundsIndex + 2])));
                    Vector<UInt32> result3 = Vector.BitwiseAnd(Vector.GreaterThanOrEqual(currentValues, new Vector<UInt32>(this.LowerBounds[boundsIndex + 3])),
                        Vector.LessThanOrEqual(currentValues, new Vector<UInt32>(this.UpperBounds[boundsIndex + 3])));
                    Vector<UInt32> result4 = Vector.BitwiseAnd(Vector.GreaterThanOrEqual(currentValues, new Vector<UInt32>(this.LowerBounds[boundsIndex + 4])),
                        Vector.LessThanOrEqual(currentValues, new Vector<UInt32>(this.UpperBounds[boundsIndex + 4])));
                    Vector<UInt32> result5 = Vector.BitwiseAnd(Vector.GreaterThanOrEqual(currentValues, new Vector<UInt32>(this.LowerBounds[boundsIndex + 5])),
                        Vector.LessThanOrEqual(currentValues, new Vector<UInt32>(this.UpperBounds[boundsIndex + 5])));
                    Vector<UInt32> result6 = Vector.BitwiseAnd(Vector.GreaterThanOrEqual(currentValues, new Vector<UInt32>(this.LowerBounds[boundsIndex + 6])),
                        Vector.LessThanOrEqual(currentValues, new Vector<UInt32>(this.UpperBounds[boundsIndex + 6])));
                    Vector<UInt32> result7 = Vector.BitwiseAnd(Vector.GreaterThanOrEqual(currentValues, new Vector<UInt32>(this.LowerBounds[boundsIndex + 7])),
                        Vector.LessThanOrEqual(currentValues, new Vector<UInt32>(this.UpperBounds[boundsIndex + 7])));

                    // Where is your god now
                    result = Vector.BitwiseOr(result,
                        Vector.BitwiseOr(
                            Vector.BitwiseOr(
                                Vector.BitwiseOr(result0, result1),
                                Vector.BitwiseOr(result2, result3)),
                            Vector.BitwiseOr(
                                Vector.BitwiseOr(result4, result5),
                                Vector.BitwiseOr(result6, result7))));
                }

                return Vector.AsVectorByte(result);
            });
        }

        public UInt32[] GetLowerBounds()
        {
            IEnumerable<UInt32> lowerBounds = this.BoundsSnapshot.SnapshotRegions.Select(region => unchecked((UInt32)region.BaseAddress.Subtract(this.MaxOffset, wrapAround: false)));

            while (lowerBounds.Count() % LinearSearchKernel.UnrollSize != 0)
            {
                lowerBounds = lowerBounds.Append<UInt32>(UInt32.MaxValue);
            }

            return lowerBounds.ToArray();
        }

        public UInt32[] GetUpperBounds()
        {
            IEnumerable<UInt32> upperBounds = this.BoundsSnapshot.SnapshotRegions.Select(region => unchecked((UInt32)region.EndAddress.Add(this.MaxOffset, wrapAround: false)));

            while (upperBounds.Count() % LinearSearchKernel.UnrollSize != 0)
            {
                upperBounds = upperBounds.Append<UInt32>(UInt32.MinValue);
            }

            return upperBounds.ToArray();
        }
    }
    //// End class
}
//// End namespace