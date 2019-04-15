namespace Squalr.Engine.Scanning.Snapshots
{
    using Squalr.Engine.Common.Extensions;
    using Squalr.Engine.Scanning.Scanners.Constraints;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a region of memory in an external process.
    /// </summary>
    public class SnapshotRegion
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnapshotRegion" /> class.
        /// </summary>
        /// <param name="readGroup">The read group of this snapshot region.</param>
        /// <param name="baseAddress">The base address of this snapshot region.</param>
        /// <param name="regionSize">The size of this snapshot region.</param>
        public SnapshotRegion(ReadGroup readGroup, Int32 readGroupOffset, Int32 regionSize)
        {
            this.ReadGroup = readGroup;
            this.ReadGroupOffset = readGroupOffset;
            this.RegionSize = regionSize;
        }

        /// <summary>
        /// Gets or sets the readgroup to which this snapshot region reads it's values.
        /// </summary>
        public ReadGroup ReadGroup { get; set; }

        /// <summary>
        /// Gets or sets the offset from the base of this snapshot's read group.
        /// </summary>
        public Int32 ReadGroupOffset { get; set; }

        /// <summary>
        /// Gets the size of this snapshot in bytes.
        /// </summary>
        public Int32 RegionSize { get; set; }

        /// <summary>
        /// Gets the base address of the region.
        /// </summary>
        public UInt64 BaseAddress
        {
            get
            {
                return this.ReadGroup.BaseAddress.Add(this.ReadGroupOffset);
            }
        }

        /// <summary>
        /// Gets the end address of the region.
        /// </summary>
        public UInt64 EndAddress
        {
            get
            {
                return this.ReadGroup.BaseAddress.Add(this.ReadGroupOffset + this.RegionSize);
            }
        }

        /// <summary>
        /// Gets or sets the base index of this snapshot. In other words, the index of the first element of this region in the scan results.
        /// </summary>
        public UInt64 BaseElementIndex { get; set; }

        /// <summary>
        /// Gets the number of elements contained in this snapshot.
        /// </summary>
        public Int32 ElementCount
        {
            get
            {
                return this.RegionSize / this.ReadGroup.Alignment;
            }
        }

        /// <summary>
        /// Indexer to allow the retrieval of the element at the specified index.
        /// </summary>
        /// <param name="index">The index of the snapshot element.</param>
        /// <returns>Returns the snapshot element at the specified index.</returns>
        public SnapshotElementIndexer this[Int32 index]
        {
            get
            {
                return new SnapshotElementIndexer(region: this, elementIndex: index);
            }
        }

        /// <summary>
        /// Gets the enumerator for an element reference within this snapshot region.
        /// </summary>
        /// <param name="pointerIncrementMode">The method for incrementing pointers.</param>
        /// <param name="compareActionConstraint">The constraint to use for the element quick action.</param>
        /// <param name="compareActionValue">The value to use for the element quick action.</param>
        /// <returns>The enumerator for an element reference within this snapshot region.</returns>
        public IEnumerator<SnapshotElementIndexer> IterateElements()
        {
            Int32 elementCount = this.ElementCount;
            SnapshotElementIndexer snapshotElement = new SnapshotElementIndexer(region: this);

            for (snapshotElement.ElementIndex = 0; snapshotElement.ElementIndex < elementCount; snapshotElement.ElementIndex++)
            {
                yield return snapshotElement;
            }
        }

        /// <summary>
        /// Gets the enumerator for an element reference within this snapshot region.
        /// </summary>
        /// <param name="pointerIncrementMode">The method for incrementing pointers.</param>
        /// <param name="compareActionConstraint">The constraint to use for the element quick action.</param>
        /// <param name="compareActionValue">The value to use for the element quick action.</param>
        /// <returns>The enumerator for an element reference within this snapshot region.</returns>
        public IEnumerator<SnapshotElementComparer> IterateComparer(
            SnapshotElementComparer.PointerIncrementMode pointerIncrementMode,
            ConstraintNode constraints)
        {
            Int32 elementCount = this.ElementCount;
            SnapshotElementComparer snapshotElement = new SnapshotElementComparer(region: this, pointerIncrementMode: pointerIncrementMode, constraints: constraints);

            for (Int32 elementIndex = 0; elementIndex < elementCount; elementIndex++)
            {
                yield return snapshotElement;
                snapshotElement.IncrementPointers();
            }
        }
    }
    //// End class
}
//// End namespace