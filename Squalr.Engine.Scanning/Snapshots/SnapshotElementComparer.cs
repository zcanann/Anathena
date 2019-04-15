namespace Squalr.Engine.Scanning.Snapshots
{
    using Squalr.Engine.Common.Extensions;
    using Squalr.Engine.Scanning.Scanners.Constraints;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Defines a reference to an element within a snapshot region.
    /// </summary>
    public class SnapshotElementComparer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnapshotElementComparer" /> class.
        /// </summary>
        /// <param name="region">The parent region that contains this element.</param>
        /// <param name="pointerIncrementMode">The method by which to increment element pointers.</param>
        /// <param name="constraints">The constraints to use for the element comparisons.</param>
        public unsafe SnapshotElementComparer(
            SnapshotRegion region,
            PointerIncrementMode pointerIncrementMode)
        {
            this.Region = region;

            // The garbage collector can relocate variables at runtime. Since we use unsafe pointers, we need to keep these pinned
            this.CurrentValuesHandle = GCHandle.Alloc(this.Region.ReadGroup.CurrentValues, GCHandleType.Pinned);
            this.PreviousValuesHandle = GCHandle.Alloc(this.Region.ReadGroup.PreviousValues, GCHandleType.Pinned);

            this.InitializePointers();
            this.SetConstraintFunctions();
            this.SetPointerFunction(pointerIncrementMode);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnapshotElementComparer" /> class.
        /// </summary>
        /// <param name="region">The parent region that contains this element.</param>
        /// <param name="pointerIncrementMode">The method by which to increment element pointers.</param>
        /// <param name="constraints">The constraints to use for the element comparisons.</param>
        public unsafe SnapshotElementComparer(
            SnapshotRegion region,
            PointerIncrementMode pointerIncrementMode,
            ConstraintNode constraints) : this(region, pointerIncrementMode)
        {
            this.ElementCompare = this.BuildCompareActions(constraints);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SnapshotElementComparer" /> class.
        /// </summary>
        ~SnapshotElementComparer()
        {
            // Let the GC do what it wants now
            this.CurrentValuesHandle.Free();
            this.PreviousValuesHandle.Free();
        }

        /// <summary>
        /// Gets an action to increment only the needed pointers.
        /// </summary>
        public Action IncrementPointers { get; private set; }

        /// <summary>
        /// Gets an action based on the element iterator scan constraint.
        /// </summary>
        public Func<Boolean> ElementCompare { get; private set; }

        /// <summary>
        /// Gets a function which determines if this element has changed.
        /// </summary>
        private Func<Boolean> Changed { get; set; }

        /// <summary>
        /// Gets a function which determines if this element has not changed.
        /// </summary>
        private Func<Boolean> Unchanged { get; set; }

        /// <summary>
        /// Gets a function which determines if this element has increased.
        /// </summary>
        private Func<Boolean> Increased { get; set; }

        /// <summary>
        /// Gets a function which determines if this element has decreased.
        /// </summary>
        private Func<Boolean> Decreased { get; set; }

        /// <summary>
        /// Gets a function which determines if this element has a value equal to the given value.
        /// </summary>
        private Func<Object, Boolean> EqualToValue { get; set; }

        /// <summary>
        /// Gets a function which determines if this element has a value not equal to the given value.
        /// </summary>
        private Func<Object, Boolean> NotEqualToValue { get; set; }

        /// <summary>
        /// Gets a function which determines if this element has a value greater than to the given value.
        /// </summary>
        private Func<Object, Boolean> GreaterThanValue { get; set; }

        /// <summary>
        /// Gets a function which determines if this element has a value greater than or equal to the given value.
        /// </summary>
        private Func<Object, Boolean> GreaterThanOrEqualToValue { get; set; }

        /// <summary>
        /// Gets a function which determines if this element has a value less than to the given value.
        /// </summary>
        private Func<Object, Boolean> LessThanValue { get; set; }

        /// <summary>
        /// Gets a function which determines if this element has a value less than to the given value.
        /// </summary>
        private Func<Object, Boolean> LessThanOrEqualToValue { get; set; }

        /// <summary>
        /// Gets a function which determines if the element has increased it's value by the given value.
        /// </summary>
        private Func<Object, Boolean> IncreasedByValue { get; set; }

        /// <summary>
        /// Gets a function which determines if the element has decreased it's value by the given value.
        /// </summary>
        private Func<Object, Boolean> DecreasedByValue { get; set; }

        /// <summary>
        /// Enums determining which pointers need to be updated every iteration.
        /// </summary>
        public enum PointerIncrementMode
        {
            /// <summary>
            /// Increment all pointers.
            /// </summary>
            AllPointers,

            /// <summary>
            /// Only increment current and previous value pointers.
            /// </summary>
            ValuesOnly,

            /// <summary>
            /// Only increment label pointers.
            /// </summary>
            LabelsOnly,

            /// <summary>
            /// Only increment current value pointer.
            /// </summary>
            CurrentOnly,

            /// <summary>
            /// Increment all pointers except the previous value pointer.
            /// </summary>
            NoPrevious,
        }

        /// <summary>
        /// Gets the base address of this element.
        /// </summary>
        public UInt64 BaseAddress
        {
            get
            {
                return this.Region.BaseAddress.Add(this.ElementIndex);
            }
        }

        /// <summary>
        /// Gets or sets the label associated with this element.
        /// </summary>
        public Object ElementLabel
        {
            get
            {
                return this.Region.ReadGroup.ElementLabels[this.CurrentLabelIndex];
            }

            set
            {
                this.Region.ReadGroup.ElementLabels[this.CurrentLabelIndex] = value;
            }
        }

        /// <summary>
        /// Gets or sets a garbage collector handle to the current value array.
        /// </summary>
        private GCHandle CurrentValuesHandle { get; set; }

        /// <summary>
        /// Gets or sets a garbage collector handle to the previous value array.
        /// </summary>
        private GCHandle PreviousValuesHandle { get; set; }

        /// <summary>
        /// Gets or sets the parent snapshot region.
        /// </summary>
        private SnapshotRegion Region { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the current value.
        /// </summary>
        private unsafe Byte* CurrentValuePointer { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the previous value.
        /// </summary>
        private unsafe Byte* PreviousValuePointer { get; set; }

        /// <summary>
        /// Gets or sets the index of this element, used for setting and getting the label.
        /// Note that we cannot have a pointer to the label, as it is a non-blittable type.
        /// </summary>
        private Int32 CurrentLabelIndex { get; set; }

        /// <summary>
        /// Gets the index of this element.
        /// </summary>
        private unsafe Int32 ElementIndex
        {
            get
            {
                // Use the incremented current value pointer or label index to figure out the index of this element
                if (this.CurrentLabelIndex != 0)
                {
                    return this.CurrentLabelIndex;
                }
                else if (this.CurrentValuePointer != null)
                {
                    fixed (Byte* pointerBase = &this.Region.ReadGroup.CurrentValues[this.Region.ReadGroupOffset])
                    {
                        return (Int32)(this.CurrentValuePointer - pointerBase);
                    }
                }
                else if (this.PreviousValuePointer != null)
                {
                    fixed (Byte* pointerBase = &this.Region.ReadGroup.PreviousValues[this.Region.ReadGroupOffset])
                    {
                        return (Int32)(this.PreviousValuePointer - pointerBase);
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets or sets the type code associated with the data type of this element.
        /// </summary>
        private TypeCode CurrentTypeCode { get; set; }

        /// <summary>
        /// Sets a custom comparison function to use in scanning.
        /// </summary>
        /// <param name="customCompare"></param>
        public void SetCustomCompareAction(Func<Boolean> customCompare)
        {
            this.ElementCompare = customCompare;
        }

        /// <summary>
        /// Gets the current value of this element.
        /// </summary>
        /// <returns>The current value of this element.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Object GetCurrentValue()
        {
            return this.LoadValue(this.CurrentValuePointer);
        }

        /// <summary>
        /// Gets the previous value of this element.
        /// </summary>
        /// <returns>The previous value of this element.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Object GetPreviousValue()
        {
            return this.LoadValue(this.PreviousValuePointer);
        }

        /// <summary>
        /// Gets the label of this element.
        /// </summary>
        /// <returns>The label of this element.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Object GetElementLabel()
        {
            return this.Region.ReadGroup.ElementLabels == null ? null : this.Region.ReadGroup.ElementLabels[this.CurrentLabelIndex];
        }

        /// <summary>
        /// Sets the label of this element.
        /// </summary>
        /// <param name="newLabel">The new element label.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SetElementLabel(Object newLabel)
        {
            this.Region.ReadGroup.ElementLabels[this.CurrentLabelIndex] = newLabel;
        }

        /// <summary>
        /// Determines if this element has a current value associated with it.
        /// </summary>
        /// <returns>True if a current value is present.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Boolean HasCurrentValue()
        {
            if (this.CurrentValuePointer == (Byte*)0)
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
            if (this.PreviousValuePointer == (Byte*)0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Initializes snapshot value reference pointers
        /// </summary>
        private unsafe void InitializePointers()
        {
            this.CurrentLabelIndex = 0;
            this.CurrentTypeCode = Type.GetTypeCode(this.Region.ReadGroup.ElementDataType);

            if (this.Region.ReadGroup.CurrentValues != null && this.Region.ReadGroup.CurrentValues.Length > 0)
            {
                fixed (Byte* pointerBase = &this.Region.ReadGroup.CurrentValues[this.Region.ReadGroupOffset])
                {
                    this.CurrentValuePointer = pointerBase;
                }
            }
            else
            {
                this.CurrentValuePointer = null;
            }

            if (this.Region.ReadGroup.PreviousValues != null && this.Region.ReadGroup.PreviousValues.Length > 0)
            {
                fixed (Byte* pointerBase = &this.Region.ReadGroup.PreviousValues[this.Region.ReadGroupOffset])
                {
                    this.PreviousValuePointer = pointerBase;
                }
            }
            else
            {
                this.PreviousValuePointer = null;
            }
        }

        /// <summary>
        /// Initializes all constraint functions for value comparisons.
        /// </summary>
        private unsafe void SetConstraintFunctions()
        {
            switch (this.CurrentTypeCode)
            {
                case TypeCode.Byte:
                    this.Changed = () => { return *this.CurrentValuePointer != *this.PreviousValuePointer; };
                    this.Unchanged = () => { return *this.CurrentValuePointer == *this.PreviousValuePointer; };
                    this.Increased = () => { return *this.CurrentValuePointer > *this.PreviousValuePointer; };
                    this.Decreased = () => { return *this.CurrentValuePointer < *this.PreviousValuePointer; };
                    this.EqualToValue = (value) => { return *this.CurrentValuePointer == (Byte)value; };
                    this.NotEqualToValue = (value) => { return *this.CurrentValuePointer != (Byte)value; };
                    this.GreaterThanValue = (value) => { return *this.CurrentValuePointer > (Byte)value; };
                    this.GreaterThanOrEqualToValue = (value) => { return *this.CurrentValuePointer >= (Byte)value; };
                    this.LessThanValue = (value) => { return *this.CurrentValuePointer < (Byte)value; };
                    this.LessThanOrEqualToValue = (value) => { return *this.CurrentValuePointer <= (Byte)value; };
                    this.IncreasedByValue = (value) => { return *this.CurrentValuePointer == unchecked(*this.PreviousValuePointer + (Byte)value); };
                    this.DecreasedByValue = (value) => { return *this.CurrentValuePointer == unchecked(*this.PreviousValuePointer - (Byte)value); };
                    break;
                case TypeCode.SByte:
                    this.Changed = () => { return *(SByte*)this.CurrentValuePointer != *(SByte*)this.PreviousValuePointer; };
                    this.Unchanged = () => { return *(SByte*)this.CurrentValuePointer == *(SByte*)this.PreviousValuePointer; };
                    this.Increased = () => { return *(SByte*)this.CurrentValuePointer > *(SByte*)this.PreviousValuePointer; };
                    this.Decreased = () => { return *(SByte*)this.CurrentValuePointer < *(SByte*)this.PreviousValuePointer; };
                    this.EqualToValue = (value) => { return *(SByte*)this.CurrentValuePointer == (SByte)value; };
                    this.NotEqualToValue = (value) => { return *(SByte*)this.CurrentValuePointer != (SByte)value; };
                    this.GreaterThanValue = (value) => { return *(SByte*)this.CurrentValuePointer > (SByte)value; };
                    this.GreaterThanOrEqualToValue = (value) => { return *(SByte*)this.CurrentValuePointer >= (SByte)value; };
                    this.LessThanValue = (value) => { return *(SByte*)this.CurrentValuePointer < (SByte)value; };
                    this.LessThanOrEqualToValue = (value) => { return *(SByte*)this.CurrentValuePointer <= (SByte)value; };
                    this.IncreasedByValue = (value) => { return *(SByte*)this.CurrentValuePointer == unchecked(*(SByte*)this.PreviousValuePointer + (SByte)value); };
                    this.DecreasedByValue = (value) => { return *(SByte*)this.CurrentValuePointer == unchecked(*(SByte*)this.PreviousValuePointer - (SByte)value); };
                    break;
                case TypeCode.Int16:
                    this.Changed = () => { return *(Int16*)this.CurrentValuePointer != *(Int16*)this.PreviousValuePointer; };
                    this.Unchanged = () => { return *(Int16*)this.CurrentValuePointer == *(Int16*)this.PreviousValuePointer; };
                    this.Increased = () => { return *(Int16*)this.CurrentValuePointer > *(Int16*)this.PreviousValuePointer; };
                    this.Decreased = () => { return *(Int16*)this.CurrentValuePointer < *(Int16*)this.PreviousValuePointer; };
                    this.EqualToValue = (value) => { return *(Int16*)this.CurrentValuePointer == (Int16)value; };
                    this.NotEqualToValue = (value) => { return *(Int16*)this.CurrentValuePointer != (Int16)value; };
                    this.GreaterThanValue = (value) => { return *(Int16*)this.CurrentValuePointer > (Int16)value; };
                    this.GreaterThanOrEqualToValue = (value) => { return *(Int16*)this.CurrentValuePointer >= (Int16)value; };
                    this.LessThanValue = (value) => { return *(Int16*)this.CurrentValuePointer < (Int16)value; };
                    this.LessThanOrEqualToValue = (value) => { return *(Int16*)this.CurrentValuePointer <= (Int16)value; };
                    this.IncreasedByValue = (value) => { return *(Int16*)this.CurrentValuePointer == unchecked(*(Int16*)this.PreviousValuePointer + (Int16)value); };
                    this.DecreasedByValue = (value) => { return *(Int16*)this.CurrentValuePointer == unchecked(*(Int16*)this.PreviousValuePointer - (Int16)value); };
                    break;
                case TypeCode.Int32:
                    this.Changed = () => { return *(Int32*)this.CurrentValuePointer != *(Int32*)this.PreviousValuePointer; };
                    this.Unchanged = () => { return *(Int32*)this.CurrentValuePointer == *(Int32*)this.PreviousValuePointer; };
                    this.Increased = () => { return *(Int32*)this.CurrentValuePointer > *(Int32*)this.PreviousValuePointer; };
                    this.Decreased = () => { return *(Int32*)this.CurrentValuePointer < *(Int32*)this.PreviousValuePointer; };
                    this.EqualToValue = (value) => { return *(Int32*)this.CurrentValuePointer == (Int32)value; };
                    this.NotEqualToValue = (value) => { return *(Int32*)this.CurrentValuePointer != (Int32)value; };
                    this.GreaterThanValue = (value) => { return *(Int32*)this.CurrentValuePointer > (Int32)value; };
                    this.GreaterThanOrEqualToValue = (value) => { return *(Int32*)this.CurrentValuePointer >= (Int32)value; };
                    this.LessThanValue = (value) => { return *(Int32*)this.CurrentValuePointer < (Int32)value; };
                    this.LessThanOrEqualToValue = (value) => { return *(Int32*)this.CurrentValuePointer <= (Int32)value; };
                    this.IncreasedByValue = (value) => { return *(Int32*)this.CurrentValuePointer == unchecked(*(Int32*)this.PreviousValuePointer + (Int32)value); };
                    this.DecreasedByValue = (value) => { return *(Int32*)this.CurrentValuePointer == unchecked(*(Int32*)this.PreviousValuePointer - (Int32)value); };
                    break;
                case TypeCode.Int64:
                    this.Changed = () => { return *(Int64*)this.CurrentValuePointer != *(Int64*)this.PreviousValuePointer; };
                    this.Unchanged = () => { return *(Int64*)this.CurrentValuePointer == *(Int64*)this.PreviousValuePointer; };
                    this.Increased = () => { return *(Int64*)this.CurrentValuePointer > *(Int64*)this.PreviousValuePointer; };
                    this.Decreased = () => { return *(Int64*)this.CurrentValuePointer < *(Int64*)this.PreviousValuePointer; };
                    this.EqualToValue = (value) => { return *(Int64*)this.CurrentValuePointer == (Int64)value; };
                    this.NotEqualToValue = (value) => { return *(Int64*)this.CurrentValuePointer != (Int64)value; };
                    this.GreaterThanValue = (value) => { return *(Int64*)this.CurrentValuePointer > (Int64)value; };
                    this.GreaterThanOrEqualToValue = (value) => { return *(Int64*)this.CurrentValuePointer >= (Int64)value; };
                    this.LessThanValue = (value) => { return *(Int64*)this.CurrentValuePointer < (Int64)value; };
                    this.LessThanOrEqualToValue = (value) => { return *(Int64*)this.CurrentValuePointer <= (Int64)value; };
                    this.IncreasedByValue = (value) => { return *(Int64*)this.CurrentValuePointer == unchecked(*(Int64*)this.PreviousValuePointer + (Int64)value); };
                    this.DecreasedByValue = (value) => { return *(Int64*)this.CurrentValuePointer == unchecked(*(Int64*)this.PreviousValuePointer - (Int64)value); };
                    break;
                case TypeCode.UInt16:
                    this.Changed = () => { return *(UInt16*)this.CurrentValuePointer != *(UInt16*)this.PreviousValuePointer; };
                    this.Unchanged = () => { return *(UInt16*)this.CurrentValuePointer == *(UInt16*)this.PreviousValuePointer; };
                    this.Increased = () => { return *(UInt16*)this.CurrentValuePointer > *(UInt16*)this.PreviousValuePointer; };
                    this.Decreased = () => { return *(UInt16*)this.CurrentValuePointer < *(UInt16*)this.PreviousValuePointer; };
                    this.EqualToValue = (value) => { return *(UInt16*)this.CurrentValuePointer == (UInt16)value; };
                    this.NotEqualToValue = (value) => { return *(UInt16*)this.CurrentValuePointer != (UInt16)value; };
                    this.GreaterThanValue = (value) => { return *(UInt16*)this.CurrentValuePointer > (UInt16)value; };
                    this.GreaterThanOrEqualToValue = (value) => { return *(UInt16*)this.CurrentValuePointer >= (UInt16)value; };
                    this.LessThanValue = (value) => { return *(UInt16*)this.CurrentValuePointer < (UInt16)value; };
                    this.LessThanOrEqualToValue = (value) => { return *(UInt16*)this.CurrentValuePointer <= (UInt16)value; };
                    this.IncreasedByValue = (value) => { return *(UInt16*)this.CurrentValuePointer == unchecked(*(UInt16*)this.PreviousValuePointer + (UInt16)value); };
                    this.DecreasedByValue = (value) => { return *(UInt16*)this.CurrentValuePointer == unchecked(*(UInt16*)this.PreviousValuePointer - (UInt16)value); };
                    break;
                case TypeCode.UInt32:
                    this.Changed = () => { return *(UInt32*)this.CurrentValuePointer != *(UInt32*)this.PreviousValuePointer; };
                    this.Unchanged = () => { return *(UInt32*)this.CurrentValuePointer == *(UInt32*)this.PreviousValuePointer; };
                    this.Increased = () => { return *(UInt32*)this.CurrentValuePointer > *(UInt32*)this.PreviousValuePointer; };
                    this.Decreased = () => { return *(UInt32*)this.CurrentValuePointer < *(UInt32*)this.PreviousValuePointer; };
                    this.EqualToValue = (value) => { return *(UInt32*)this.CurrentValuePointer == (UInt32)value; };
                    this.NotEqualToValue = (value) => { return *(UInt32*)this.CurrentValuePointer != (UInt32)value; };
                    this.GreaterThanValue = (value) => { return *(UInt32*)this.CurrentValuePointer > (UInt32)value; };
                    this.GreaterThanOrEqualToValue = (value) => { return *(UInt32*)this.CurrentValuePointer >= (UInt32)value; };
                    this.LessThanValue = (value) => { return *(UInt32*)this.CurrentValuePointer < (UInt32)value; };
                    this.LessThanOrEqualToValue = (value) => { return *(UInt32*)this.CurrentValuePointer <= (UInt32)value; };
                    this.IncreasedByValue = (value) => { return *(UInt32*)this.CurrentValuePointer == unchecked(*(UInt32*)this.PreviousValuePointer + (UInt32)value); };
                    this.DecreasedByValue = (value) => { return *(UInt32*)this.CurrentValuePointer == unchecked(*(UInt32*)this.PreviousValuePointer - (UInt32)value); };
                    break;
                case TypeCode.UInt64:
                    this.Changed = () => { return *(UInt64*)this.CurrentValuePointer != *(UInt64*)this.PreviousValuePointer; };
                    this.Unchanged = () => { return *(UInt64*)this.CurrentValuePointer == *(UInt64*)this.PreviousValuePointer; };
                    this.Increased = () => { return *(UInt64*)this.CurrentValuePointer > *(UInt64*)this.PreviousValuePointer; };
                    this.Decreased = () => { return *(UInt64*)this.CurrentValuePointer < *(UInt64*)this.PreviousValuePointer; };
                    this.EqualToValue = (value) => { return *(UInt64*)this.CurrentValuePointer == (UInt64)value; };
                    this.NotEqualToValue = (value) => { return *(UInt64*)this.CurrentValuePointer != (UInt64)value; };
                    this.GreaterThanValue = (value) => { return *(UInt64*)this.CurrentValuePointer > (UInt64)value; };
                    this.GreaterThanOrEqualToValue = (value) => { return *(UInt64*)this.CurrentValuePointer >= (UInt64)value; };
                    this.LessThanValue = (value) => { return *(UInt64*)this.CurrentValuePointer < (UInt64)value; };
                    this.LessThanOrEqualToValue = (value) => { return *(UInt64*)this.CurrentValuePointer <= (UInt64)value; };
                    this.IncreasedByValue = (value) => { return *(UInt64*)this.CurrentValuePointer == unchecked(*(UInt64*)this.PreviousValuePointer + (UInt64)value); };
                    this.DecreasedByValue = (value) => { return *(UInt64*)this.CurrentValuePointer == unchecked(*(UInt64*)this.PreviousValuePointer - (UInt64)value); };
                    break;
                case TypeCode.Single:
                    this.Changed = () => { return !(*(Single*)this.CurrentValuePointer).AlmostEquals(*(Single*)this.PreviousValuePointer); };
                    this.Unchanged = () => { return (*(Single*)this.CurrentValuePointer).AlmostEquals(*(Single*)this.PreviousValuePointer); };
                    this.Increased = () => { return *(Single*)this.CurrentValuePointer > *(Single*)this.PreviousValuePointer; };
                    this.Decreased = () => { return *(Single*)this.CurrentValuePointer < *(Single*)this.PreviousValuePointer; };
                    this.EqualToValue = (value) => { return (*(Single*)this.CurrentValuePointer).AlmostEquals((Single)value); };
                    this.NotEqualToValue = (value) => { return !(*(Single*)this.CurrentValuePointer).AlmostEquals((Single)value); };
                    this.GreaterThanValue = (value) => { return *(Single*)this.CurrentValuePointer > (Single)value; };
                    this.GreaterThanOrEqualToValue = (value) => { return *(Single*)this.CurrentValuePointer >= (Single)value; };
                    this.LessThanValue = (value) => { return *(Single*)this.CurrentValuePointer < (Single)value; };
                    this.LessThanOrEqualToValue = (value) => { return *(Single*)this.CurrentValuePointer <= (Single)value; };
                    this.IncreasedByValue = (value) => { return (*(Single*)this.CurrentValuePointer).AlmostEquals(unchecked(*(Single*)this.PreviousValuePointer + (Single)value)); };
                    this.DecreasedByValue = (value) => { return (*(Single*)this.CurrentValuePointer).AlmostEquals(unchecked(*(Single*)this.PreviousValuePointer - (Single)value)); };
                    break;
                case TypeCode.Double:
                    this.Changed = () => { return !(*(Double*)this.CurrentValuePointer).AlmostEquals(*(Double*)this.PreviousValuePointer); };
                    this.Unchanged = () => { return (*(Double*)this.CurrentValuePointer).AlmostEquals(*(Double*)this.PreviousValuePointer); };
                    this.Increased = () => { return *(Double*)this.CurrentValuePointer > *(Double*)this.PreviousValuePointer; };
                    this.Decreased = () => { return *(Double*)this.CurrentValuePointer < *(Double*)this.PreviousValuePointer; };
                    this.EqualToValue = (value) => { return (*(Double*)this.CurrentValuePointer).AlmostEquals((Double)value); };
                    this.NotEqualToValue = (value) => { return !(*(Double*)this.CurrentValuePointer).AlmostEquals((Double)value); };
                    this.GreaterThanValue = (value) => { return *(Double*)this.CurrentValuePointer > (Double)value; };
                    this.GreaterThanOrEqualToValue = (value) => { return *(Double*)this.CurrentValuePointer >= (Double)value; };
                    this.LessThanValue = (value) => { return *(Double*)this.CurrentValuePointer < (Double)value; };
                    this.LessThanOrEqualToValue = (value) => { return *(Double*)this.CurrentValuePointer <= (Double)value; };
                    this.IncreasedByValue = (value) => { return (*(Double*)this.CurrentValuePointer).AlmostEquals(unchecked(*(Double*)this.PreviousValuePointer + (Double)value)); };
                    this.DecreasedByValue = (value) => { return (*(Double*)this.CurrentValuePointer).AlmostEquals(unchecked(*(Double*)this.PreviousValuePointer - (Double)value)); };
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        /// Initializes the pointer incrementing function based on the provided parameters.
        /// </summary>
        /// <param name="pointerIncrementMode">The method by which to increment pointers.</param>
        private unsafe void SetPointerFunction(PointerIncrementMode pointerIncrementMode)
        {
            Int32 alignment = this.Region.ReadGroup.Alignment;

            if (this.Region.ReadGroup.Alignment == 1)
            {
                switch (pointerIncrementMode)
                {
                    case PointerIncrementMode.AllPointers:
                        this.IncrementPointers = () =>
                        {
                            this.CurrentLabelIndex++;
                            this.CurrentValuePointer++;
                            this.PreviousValuePointer++;
                        };
                        break;
                    case PointerIncrementMode.CurrentOnly:
                        this.IncrementPointers = () =>
                        {
                            this.CurrentValuePointer++;
                        };
                        break;
                    case PointerIncrementMode.LabelsOnly:
                        this.IncrementPointers = () =>
                        {
                            this.CurrentLabelIndex++;
                        };
                        break;
                    case PointerIncrementMode.NoPrevious:
                        this.IncrementPointers = () =>
                        {
                            this.CurrentLabelIndex++;
                            this.CurrentValuePointer++;
                        };
                        break;
                    case PointerIncrementMode.ValuesOnly:
                        this.IncrementPointers = () =>
                        {
                            this.CurrentValuePointer++;
                            this.PreviousValuePointer++;
                        };
                        break;
                }
            }
            else
            {
                switch (pointerIncrementMode)
                {
                    case PointerIncrementMode.AllPointers:
                        this.IncrementPointers = () =>
                        {
                            this.CurrentLabelIndex += alignment;
                            this.CurrentValuePointer += alignment;
                            this.PreviousValuePointer += alignment;
                        };
                        break;
                    case PointerIncrementMode.CurrentOnly:
                        this.IncrementPointers = () =>
                        {
                            this.CurrentValuePointer += alignment;
                        };
                        break;
                    case PointerIncrementMode.LabelsOnly:
                        this.IncrementPointers = () =>
                        {
                            this.CurrentLabelIndex += alignment;
                        };
                        break;
                    case PointerIncrementMode.NoPrevious:
                        this.IncrementPointers = () =>
                        {
                            this.CurrentLabelIndex += alignment;
                            this.CurrentValuePointer += alignment;
                        };
                        break;
                    case PointerIncrementMode.ValuesOnly:
                        this.IncrementPointers = () =>
                        {
                            this.CurrentValuePointer += alignment;
                            this.PreviousValuePointer += alignment;
                        };
                        break;
                }
            }
        }

        /// <summary>
        /// Sets the default compare action to use for this element.
        /// </summary>
        /// <param name="constraints">The constraints to use for the element quick action.</param>
        private Func<Boolean> BuildCompareActions(ConstraintNode constraints)
        {
            if (constraints is Operation)
            {
                if (constraints.Left == null || constraints.Right == null)
                {
                    throw new ArgumentException("An operation constraint must have both a left and right child");
                }

                switch ((constraints as Operation).BinaryOperation)
                {
                    case Operation.OperationType.AND:
                        return () => this.BuildCompareActions(constraints.Left).Invoke() && this.BuildCompareActions(constraints.Right).Invoke();
                    case Operation.OperationType.OR:
                        return () => this.BuildCompareActions(constraints.Left).Invoke() || this.BuildCompareActions(constraints.Right).Invoke();
                    default:
                        throw new ArgumentException("Unkown operation type");
                }
            }
            else if (constraints is ScanConstraint)
            {
                ScanConstraint scanConstraint = (constraints as ScanConstraint);

                switch (scanConstraint.Constraint)
                {
                    case ScanConstraint.ConstraintType.Unchanged:
                        return this.Unchanged;
                    case ScanConstraint.ConstraintType.Changed:
                        return this.Changed;
                    case ScanConstraint.ConstraintType.Increased:
                        return this.Increased;
                    case ScanConstraint.ConstraintType.Decreased:
                        return this.Decreased;
                    case ScanConstraint.ConstraintType.IncreasedByX:
                        return () => this.IncreasedByValue(scanConstraint.ConstraintValue);
                    case ScanConstraint.ConstraintType.DecreasedByX:
                        return () => this.DecreasedByValue(scanConstraint.ConstraintValue);
                    case ScanConstraint.ConstraintType.Equal:
                        return () => this.EqualToValue(scanConstraint.ConstraintValue);
                    case ScanConstraint.ConstraintType.NotEqual:
                        return () => this.NotEqualToValue(scanConstraint.ConstraintValue);
                    case ScanConstraint.ConstraintType.GreaterThan:
                        return () => this.GreaterThanValue(scanConstraint.ConstraintValue);
                    case ScanConstraint.ConstraintType.GreaterThanOrEqual:
                        return () => this.GreaterThanOrEqualToValue(scanConstraint.ConstraintValue);
                    case ScanConstraint.ConstraintType.LessThan:
                        return () => this.LessThanValue(scanConstraint.ConstraintValue);
                    case ScanConstraint.ConstraintType.LessThanOrEqual:
                        return () => this.LessThanOrEqualToValue(scanConstraint.ConstraintValue);
                    default:
                        throw new Exception("Unknown constraint type");
                }
            }

            throw new ArgumentException("Invalid constraint node");
        }

        /// <summary>
        /// Loads the value of this snapshot element from the given array.
        /// </summary>
        /// <param name="array">The byte array from which to read a value.</param>
        /// <returns>The value at the start of this array casted as the proper data type.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe Object LoadValue(Byte* array)
        {
            switch (this.CurrentTypeCode)
            {
                case TypeCode.Byte:
                    return *array;
                case TypeCode.SByte:
                    return *(SByte*)array;
                case TypeCode.Int16:
                    return *(Int16*)array;
                case TypeCode.Int32:
                    return *(Int32*)array;
                case TypeCode.Int64:
                    return *(Int64*)array;
                case TypeCode.UInt16:
                    return *(UInt16*)array;
                case TypeCode.UInt32:
                    return *(UInt32*)array;
                case TypeCode.UInt64:
                    return *(UInt64*)array;
                case TypeCode.Single:
                    return *(Single*)array;
                case TypeCode.Double:
                    return *(Double*)array;
                default:
                    throw new ArgumentException();
            }
        }
    }
    //// End class
}
//// End namespace