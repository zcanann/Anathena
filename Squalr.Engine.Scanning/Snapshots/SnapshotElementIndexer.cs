namespace Squalr.Engine.Scanning.Snapshots
{
    using Squalr.Engine.Common.DataTypes;
    using Squalr.Engine.Common.Extensions;
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Defines a reference to an element within a snapshot region.
    /// </summary>
    public unsafe class SnapshotElementIndexer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnapshotElementIndexer" /> class.
        /// </summary>
        /// <param name="region">The parent region that contains this element.</param>
        /// <param name="elementIndex">The index of the element to begin pointing to.</param>
        public unsafe SnapshotElementIndexer(
            SnapshotRegion region,
            Int32 elementIndex = 0)
        {
            this.Region = region;
            this.ElementIndex = elementIndex;
        }

        /// <summary>
        /// Gets the base address of this element.
        /// </summary>
        public UInt64 BaseAddress
        {
            get
            {
                return this.Region.ReadGroup.BaseAddress.Add(this.Region.ReadGroupOffset).Add(this.ElementIndex * this.Region.ReadGroup.Alignment);
            }
        }

        /// <summary>
        /// Gets or sets the label associated with this element.
        /// </summary>
        public Object ElementLabel
        {
            get
            {
                return this.Region.ReadGroup.ElementLabels[this.ElementIndex];
            }

            set
            {
                this.Region.ReadGroup.ElementLabels[this.ElementIndex] = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent snapshot region.
        /// </summary>
        private SnapshotRegion Region { get; set; }

        /// <summary>
        /// Gets the index of this element.
        /// </summary>
        public Int32 ElementIndex { get; set; }

        public Object LoadCurrentValue()
        {
            fixed (Byte* pointerBase = &this.Region.ReadGroup.CurrentValues[this.Region.ReadGroupOffset + this.ElementIndex])
            {
                switch (this.Region.ReadGroup.ElementDataType)
                {
                    case DataType type when type == DataType.Byte:
                        return *pointerBase;
                    case DataType type when type == DataType.SByte:
                        return *(SByte*)pointerBase;
                    case DataType type when type == DataType.Int16:
                        return *(Int16*)pointerBase;
                    case DataType type when type == DataType.Int32:
                        return *(Int32*)pointerBase;
                    case DataType type when type == DataType.Int64:
                        return *(Int64*)pointerBase;
                    case DataType type when type == DataType.UInt16:
                        return *(UInt16*)pointerBase;
                    case DataType type when type == DataType.UInt32:
                        return *(UInt32*)pointerBase;
                    case DataType type when type == DataType.UInt64:
                        return *(UInt64*)pointerBase;
                    case DataType type when type == DataType.Single:
                        return *(Single*)pointerBase;
                    case DataType type when type == DataType.Double:
                        return *(Double*)pointerBase;
                    default:
                        throw new ArgumentException();
                }
            }
        }

        public Object LoadPreviousValue()
        {
            fixed (Byte* pointerBase = &this.Region.ReadGroup.PreviousValues[this.Region.ReadGroupOffset + this.ElementIndex])
            {
                switch (this.Region.ReadGroup.ElementDataType)
                {
                    case DataType type when type == DataType.Byte:
                        return *pointerBase;
                    case DataType type when type == DataType.SByte:
                        return *(SByte*)pointerBase;
                    case DataType type when type == DataType.Int16:
                        return *(Int16*)pointerBase;
                    case DataType type when type == DataType.Int32:
                        return *(Int32*)pointerBase;
                    case DataType type when type == DataType.Int64:
                        return *(Int64*)pointerBase;
                    case DataType type when type == DataType.UInt16:
                        return *(UInt16*)pointerBase;
                    case DataType type when type == DataType.UInt32:
                        return *(UInt32*)pointerBase;
                    case DataType type when type == DataType.UInt64:
                        return *(UInt64*)pointerBase;
                    case DataType type when type == DataType.Single:
                        return *(Single*)pointerBase;
                    case DataType type when type == DataType.Double:
                        return *(Double*)pointerBase;
                    default:
                        throw new ArgumentException();
                }
            }
        }

        /// <summary>
        /// Gets the label of this element.
        /// </summary>
        /// <returns>The label of this element.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Object GetElementLabel()
        {
            return this.Region.ReadGroup.ElementLabels == null ? null : this.Region.ReadGroup.ElementLabels[this.ElementIndex];
        }

        /// <summary>
        /// Sets the label of this element.
        /// </summary>
        /// <param name="newLabel">The new element label.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SetElementLabel(Object newLabel)
        {
            this.Region.ReadGroup.ElementLabels[this.ElementIndex] = newLabel;
        }

        /// <summary>
        /// Determines if this element has a current value associated with it.
        /// </summary>
        /// <returns>True if a current value is present.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Boolean HasCurrentValue()
        {
            if (this.Region.ReadGroup.CurrentValues.IsNullOrEmpty())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines if this element has a previous value associated with it.
        /// </summary>
        /// <returns>True if a previous value is present.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Boolean HasPreviousValue()
        {
            if (this.Region.ReadGroup.PreviousValues.IsNullOrEmpty())
            {
                return false;
            }

            return true;
        }
    }
    //// End class
}
//// End namespace