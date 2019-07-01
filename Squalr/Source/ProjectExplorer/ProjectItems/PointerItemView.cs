namespace Squalr.Source.ProjectExplorer.ProjectItems
{
    using Squalr.Engine.Common.DataTypes;
    using Squalr.Engine.Projects.Items;
    using Squalr.Source.Controls;
    using Squalr.Source.Editors.OffsetEditor;
    using Squalr.Source.Editors.ValueEditor;
    using Squalr.Source.Utils.TypeConverters;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing.Design;

    /// <summary>
    /// Decorates the base project item class with annotations for use in the view.
    /// </summary>
    internal class PointerItemView : ProjectItemView
    {
        private PointerItem pointerItem;

        public PointerItemView(PointerItem pointerItem)
        {
            this.PointerItem = pointerItem;
            this.PointerItem.PropertyChanged += PointerItemPropertyChanged;
        }

        ~PointerItemView()
        {
            this.PointerItem.PropertyChanged -= PointerItemPropertyChanged;
        }

        private void PointerItemPropertyChanged(Object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(PointerItem.AddressValue):
                    this.RaisePropertyChanged(nameof(this.DisplayValue));
                    break;
                case nameof(PointerItem.IsStatic):
                    this.RaisePropertyChanged(nameof(this.IsStatic));
                    break;
                case nameof(PointerItem.ModuleOffset):
                    this.RaisePropertyChanged(nameof(this.ModuleOffset));
                    break;
                case nameof(PointerItem.DataType):
                    this.RaisePropertyChanged(nameof(this.DataType));
                    break;
            }
        }

        [Browsable(false)]
        private PointerItem PointerItem
        {
            get
            {
                return this.pointerItem;
            }

            set
            {
                this.pointerItem = value;
                this.ProjectItem = value;
                this.RaisePropertyChanged(nameof(this.PointerItem));
            }
        }

        /// <summary>
        /// Gets or sets the description for this object.
        /// </summary>
        [Browsable(false)]
        public String AddressSpecifier
        {
            get
            {
                return this.PointerItem.AddressSpecifier;
            }
        }

        /// <summary>
        /// Gets or sets the description for this object.
        /// </summary>
        [Browsable(true)]
        [SortedCategory(SortedCategory.CategoryType.Common), DisplayName("Name"), Description("The name of this pointer")]
        public String Name
        {
            get
            {
                return this.PointerItem.Name;
            }

            set
            {
                this.PointerItem.Name = value;
                this.RaisePropertyChanged(nameof(this.Name));
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
                return this.PointerItem.ModuleName;
            }

            set
            {
                this.PointerItem.ModuleName = value;
                this.RaisePropertyChanged(nameof(this.ModuleName));
            }
        }

        /// <summary>
        /// Gets or sets the identifier for the base address of this object.
        /// </summary>
        [Browsable(true)]
        [RefreshProperties(RefreshProperties.All)]
        [SortedCategory(SortedCategory.CategoryType.Advanced), DisplayName("Data Type"), Description("The data type of this address")]
        public DataTypeBase DataType
        {
            get
            {
                return this.PointerItem.DataType;
            }

            set
            {
                this.PointerItem.DataType = value;
                this.RaisePropertyChanged(nameof(this.DataType));
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
                return this.PointerItem.ModuleOffset;
            }

            set
            {
                this.PointerItem.ModuleOffset = value;
                this.RaisePropertyChanged(nameof(this.ModuleOffset));
            }
        }

        /// <summary>
        /// Gets or sets the pointer offsets of this address item.
        /// </summary>
        [Browsable(true)]
        [RefreshProperties(RefreshProperties.All)]
        [TypeConverter(typeof(OffsetConverter))]
        [Editor(typeof(OffsetEditorModel), typeof(UITypeEditor))]
        [SortedCategory(SortedCategory.CategoryType.Advanced), DisplayName("Pointer Offsets"), Description("The pointer offsets used to calculate the final address")]
        public IEnumerable<Int32> PointerOffsets
        {
            get
            {
                return this.PointerItem.PointerOffsets;
            }

            set
            {
                this.PointerItem.PointerOffsets = value;
                this.RaisePropertyChanged(nameof(this.PointerOffsets));
            }
        }

        [Browsable(true)]
        [RefreshProperties(RefreshProperties.All)]
        [Editor(typeof(ValueEditorModel), typeof(UITypeEditor))]
        [SortedCategory(SortedCategory.CategoryType.Advanced), DisplayName("Address Value"), Description("The value at the resolved address")]
        public override Object DisplayValue
        {
            get
            {
                return this.PointerItem.AddressValue;
            }

            set
            {
                this.PointerItem.AddressValue = value;
                this.RaisePropertyChanged(nameof(this.DisplayValue));
            }
        }

        [Browsable(false)]
        public Boolean IsStatic
        {
            get
            {
                return this.PointerItem.IsStatic;
            }
        }
    }
    //// End class
}
//// End namespace