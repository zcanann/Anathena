namespace Squalr.Source.ProjectExplorer.ProjectItems
{
    using Squalr.Engine.Common.DataTypes;
    using Squalr.Engine.Projects.Items;
    using Squalr.Source.Controls;
    using Squalr.Source.Utils.TypeConverters;
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Decorates the base project item class with annotations for use in the view.
    /// </summary>
    internal class InstructionItemView : ProjectItemView
    {
        private InstructionItem instructionItem;

        public InstructionItemView(InstructionItem instructionItem)
        {
            this.InstructionItem = instructionItem;
        }

        [Browsable(false)]
        private InstructionItem InstructionItem
        {
            get
            {
                return this.instructionItem;
            }

            set
            {
                this.instructionItem = value;
                this.ProjectItem = value;
                this.RaisePropertyChanged(nameof(this.InstructionItem));
            }
        }

        /// <summary>
        /// Gets or sets the value at this address.
        /// </summary>
        [Browsable(true)]
        [TypeConverter(typeof(DynamicConverter))]
        [SortedCategory(SortedCategory.CategoryType.Common), DisplayName("Value"), Description("Value at the calculated address")]
        public Object AddressValue
        {
            get
            {
                return this.InstructionItem.AddressValue;
            }

            set
            {
                this.InstructionItem.AddressValue = value;
            }
        }

        [Browsable(true)]
        [RefreshProperties(RefreshProperties.All)]
        [SortedCategory(SortedCategory.CategoryType.Advanced), DisplayName("Instruction Bytes"), Description("The bytes of the instruction")]
        public Byte[] InstructionBytes
        {
            get
            {
                return this.InstructionItem.InstructionBytes;
            }

            set
            {
                this.InstructionItem.InstructionBytes = value;
            }
        }

        /// <summary>
        /// Gets or sets the disassembled instruction.
        /// </summary>
        [Browsable(true)]
        [RefreshProperties(RefreshProperties.All)]
        [SortedCategory(SortedCategory.CategoryType.Advanced), DisplayName("Instruction"), Description("The disassembled instruction")]
        public String Instruction
        {
            get
            {
                return this.InstructionItem.Instruction;
            }

            set
            {
                this.InstructionItem.Instruction = value;
            }
        }

        /// <summary>
        /// Gets or sets the identifier for the base address of this object.
        /// </summary>
        [Browsable(true)]
        [RefreshProperties(RefreshProperties.All)]
        [SortedCategory(SortedCategory.CategoryType.Advanced), DisplayName("Module Name"), Description("The module to use as a base address")]
        public String ModuleName
        {
            get
            {
                return this.InstructionItem.ModuleName;
            }

            set
            {
                this.InstructionItem.ModuleName = value;
            }
        }

        /// <summary>
        /// Gets or sets the base address of this object. This will be added as an offset from the resolved base identifier.
        /// </summary>
        [Browsable(true)]
        [RefreshProperties(RefreshProperties.All)]
        [TypeConverter(typeof(AddressConverter))]
        [SortedCategory(SortedCategory.CategoryType.Advanced), DisplayName("Module Offset"), Description("The offset from the module address. If no module address, then this is the base address.")]
        public UInt64 ModuleOffset
        {
            get
            {
                return this.InstructionItem.ModuleOffset;
            }

            set
            {
                this.InstructionItem.ModuleOffset = value;
            }
        }

        /// <summary>
        /// Gets or sets the data type of the value at this address.
        /// </summary>
        [Browsable(true)]
        [RefreshProperties(RefreshProperties.All)]
        [TypeConverter(typeof(DataTypeConverter))]
        [SortedCategory(SortedCategory.CategoryType.Advanced), DisplayName("Data Type"), Description("Data type of the calculated address")]
        public DataTypeBase DataType
        {
            get
            {
                return this.InstructionItem.DataType;
            }

            set
            {
                this.InstructionItem.DataType = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the value at this address should be displayed as hex.
        /// </summary>
        [Browsable(true)]
        [RefreshProperties(RefreshProperties.All)]
        [SortedCategory(SortedCategory.CategoryType.Advanced), DisplayName("Value as Hex"), Description("Whether the value is displayed as hexedecimal")]
        public Boolean IsValueHex
        {
            get
            {
                return this.InstructionItem.IsValueHex;
            }

            set
            {
                this.InstructionItem.IsValueHex = value;
            }
        }

        /// <summary>
        /// Gets the effective address after tracing all pointer offsets.
        /// </summary>
        [ReadOnly(true)]
        [TypeConverter(typeof(AddressConverter))]
        [SortedCategory(SortedCategory.CategoryType.Common), DisplayName("Calculated Address"), Description("The final computed address of this variable")]
        public UInt64 CalculatedAddress
        {
            get
            {
                return this.InstructionItem.CalculatedAddress;
            }
        }
    }
    //// End class
}
//// End namespace