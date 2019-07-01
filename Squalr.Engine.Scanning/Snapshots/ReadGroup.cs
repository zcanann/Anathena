namespace Squalr.Engine.Scanning.Snapshots
{
    using Squalr.Engine.Common.DataTypes;
    using Squalr.Engine.Common.OS;
    using Squalr.Engine.Memory;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Defines a segment of process memory, which many snapshot regions may read from. This serves as a shared pool of memory, such as to
    /// minimize the number of calls to the OS to read the memory of a process.
    /// </summary>
    public class ReadGroup : NormalizedRegion
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadGroup" /> class.
        /// </summary>
        /// <param name="baseAddress">The base address of this memory region.</param>
        /// <param name="regionSize">The size of this memory region.</param>
        public ReadGroup(UInt64 baseAddress, Int32 regionSize, DataTypeBase dataType, Int32 alignment) : base(baseAddress, regionSize)
        {
            this.Alignment = alignment;
            this.ElementDataType = dataType;

            this.SnapshotRegions = new List<SnapshotRegion>() { new SnapshotRegion(this, 0, Math.Max(Vectors.VectorSize, regionSize)) };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadGroup" /> class.
        /// </summary>
        /// <param name="baseAddress">The base address of this memory region.</param>
        /// <param name="regionSize">The size of this memory region.</param>
        public ReadGroup(UInt64 baseAddress, Byte[] initialBytes, DataTypeBase dataType, Int32 alignment) : base(baseAddress, initialBytes.Length)
        {
            this.Alignment = alignment;
            this.ElementDataType = dataType;
            this.CurrentValues = initialBytes;

            this.SnapshotRegions = new List<SnapshotRegion>() { new SnapshotRegion(this, 0, initialBytes.Length) };
        }

        /// <summary>
        /// Gets the most recently read values.
        /// </summary>
        public unsafe Byte[] CurrentValues { get; private set; }

        /// <summary>
        /// Gets the previously read values.
        /// </summary>
        public unsafe Byte[] PreviousValues { get; private set; }

        /// <summary>
        /// Gets or sets the data type of the elements of this region.
        /// </summary>
        public DataTypeBase ElementDataType { get; set; }

        /// <summary>
        /// Gets the element labels.
        /// </summary>
        public unsafe Object[] ElementLabels { get; private set; }

        /// <summary>
        /// Gets or sets the data type of the labels of this region.
        /// </summary>
        public DataTypeBase LabelDataType { get; set; }

        /// <summary>
        /// Gets or sets the collection of snapshot regions within this read group.
        /// </summary>
        public IEnumerable<SnapshotRegion> SnapshotRegions { get; set; }

        /// <summary>
        /// Reads all memory for this memory region.
        /// </summary>
        /// <returns>The bytes read from memory.</returns>
        public Boolean ReadAllMemory(Process process)
        {
            Boolean readSuccess;
            Byte[] newCurrentValues = MemoryReader.Instance.ReadBytes(process, this.BaseAddress, this.RegionSize, out readSuccess);

            if (readSuccess)
            {
                this.SetPreviousValues(this.CurrentValues);
                this.SetCurrentValues(newCurrentValues);
            }
            else
            {
                this.SetPreviousValues(null);
                this.SetCurrentValues(null);
            }

            return readSuccess;
        }

        /// <summary>
        /// Determines if a relative comparison can be done for this region, ie current and previous values are loaded.
        /// </summary>
        /// <returns>True if a relative comparison can be done for this region.</returns>
        public Boolean CanCompare(Boolean hasRelativeConstraint)
        {
            if (this?.CurrentValues == null || (hasRelativeConstraint && this?.PreviousValues == null))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sets the current values of this region.
        /// </summary>
        /// <param name="newValues">The raw bytes of the values.</param>
        public void SetCurrentValues(Byte[] newValues)
        {
            this.CurrentValues = newValues;
        }

        /// <summary>
        /// Sets the previous values of this region.
        /// </summary>
        /// <param name="newValues">The raw bytes of the values.</param>
        public void SetPreviousValues(Byte[] newValues)
        {
            this.PreviousValues = newValues;
        }

        /// <summary>
        /// Sets the element labels for this snapshot region.
        /// </summary>
        /// <param name="newLabels">The new labels to be assigned.</param>
        public void SetElementLabels(Object[] newLabels)
        {
            this.ElementLabels = newLabels;
        }
    }
    //// End class
}
//// End namespace