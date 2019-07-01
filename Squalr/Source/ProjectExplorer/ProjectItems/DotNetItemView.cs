namespace Squalr.Source.ProjectExplorer.ProjectItems
{
    using Squalr.Engine.Common.DataTypes;
    using Squalr.Engine.Input.HotKeys;
    using Squalr.Engine.Projects.Items;
    using Squalr.Source.Controls;
    using Squalr.Source.Editors.HotkeyEditor;
    using Squalr.Source.Editors.TextEditor;
    using Squalr.Source.Utils.TypeConverters;
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;

    /// <summary>
    /// Decorates the base project item class with annotations for use in the view.
    /// </summary>
    internal class DotNetItemView : ProjectItemView
    {
        private DotNetItem dotNetItem;

        public DotNetItemView(DotNetItem dotNetItem)
        {
            this.DotNetItem = dotNetItem;
        }

        [Browsable(false)]
        private DotNetItem DotNetItem
        {
            get
            {
                return this.dotNetItem;
            }

            set
            {
                this.dotNetItem = value;
                this.ProjectItem = value;
                this.RaisePropertyChanged(nameof(this.DotNetItem));
            }
        }

        [Browsable(true)]
        [RefreshProperties(RefreshProperties.All)]
        [SortedCategory(SortedCategory.CategoryType.Common), DisplayName("Identifier"), Description("The full namespace identifier for this item")]
        public String Identifier
        {
            get
            {
                return this.DotNetItem.Identifier;
            }

            set
            {
                this.DotNetItem.Identifier = value;
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
                return this.DotNetItem.DataType;
            }

            set
            {
                this.DotNetItem.DataType = value;
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
                return this.DotNetItem.AddressValue;
            }

            set
            {
                this.DotNetItem.AddressValue = value;
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
                return this.DotNetItem.IsValueHex;
            }

            set
            {
                this.DotNetItem.IsValueHex = value;
            }
        }

        /// <summary>
        /// Gets the effective address after tracing all pointer offsets.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(AddressConverter))]
        [SortedCategory(SortedCategory.CategoryType.Common), DisplayName("Calculated Address"), Description("The final computed address of this variable")]
        public UInt64 CalculatedAddress
        {
            get
            {
                return this.DotNetItem.CalculatedAddress;
            }
        }

        /// <summary>
        /// Gets or sets the description for this object.
        /// </summary>
        [SortedCategory(SortedCategory.CategoryType.Common), DisplayName("Name"), Description("The name of this cheat")]
        public String Name
        {
            get
            {
                return this.DotNetItem.Name;
            }

            set
            {
                this.DotNetItem.Name = value;
            }
        }

        /// <summary>
        /// Gets or sets the description for this object.
        /// </summary>
        [Browsable(true)]
        [TypeConverter(typeof(TextConverter))]
        [Editor(typeof(TextEditorModel), typeof(UITypeEditor))]
        [SortedCategory(SortedCategory.CategoryType.Common), DisplayName("Description"), Description("The description of this cheat")]
        public String Description
        {
            get
            {
                return this.DotNetItem.Description;
            }

            set
            {
                this.DotNetItem.Description = value;
            }
        }

        /// <summary>
        /// Gets or sets the hotkey for this project item.
        /// </summary>
        [Browsable(true)]
        [TypeConverter(typeof(HotkeyConverter))]
        [Editor(typeof(HotkeyEditorModel), typeof(UITypeEditor))]
        [SortedCategory(SortedCategory.CategoryType.Common), DisplayName("Hotkey"), Description("The hotkey for this item")]
        public Hotkey HotKey
        {
            get
            {
                return this.DotNetItem.HotKey;
            }

            set
            {
                this.DotNetItem.HotKey = value;
            }
        }
    }
    //// End class
}
//// End namespace