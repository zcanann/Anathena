namespace Squalr.View
{
    using Squalr.Engine.Common;
    using Squalr.Engine.Common.DataTypes;
    using Squalr.Engine.Common.Extensions;
    using Squalr.Source.Controls;
    using Squalr.Source.Results;
    using Squalr.Source.Scanning;
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for PointerScanner.xaml.
    /// </summary>
    internal partial class PointerScanner : UserControl, IResultDataTypeObserver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManualScanner"/> class.
        /// </summary>
        public PointerScanner()
        {
            this.InitializeComponent();

            this.PointerScanAddressHexDecBoxViewModel = this.PointerScanAddressHexDecBox.DataContext as HexDecBoxViewModel;
            this.PointerScanAddressHexDecBoxViewModel.PropertyChanged += PointerScanAddressHexDecBoxViewModelPropertyChanged;
            this.PointerScanAddressHexDecBoxViewModel.IsHex = true;
            this.PointerScanAddressHexDecBoxViewModel.DataType = DataTypeBase.UInt64;

            this.PointerRetargetAddressHexDecBoxViewModel = this.PointerRetargetAddressHexDecBox.DataContext as HexDecBoxViewModel;
            this.PointerRetargetAddressHexDecBoxViewModel.PropertyChanged += PointerRetargetAddressHexDecBoxViewModelPropertyChanged;
            this.PointerRetargetAddressHexDecBoxViewModel.IsHex = true;
            this.PointerRetargetAddressHexDecBoxViewModel.DataType = DataTypeBase.Int32;

            this.DepthHexDecBoxViewModel = this.DepthHexDecBox.DataContext as HexDecBoxViewModel;
            this.DepthHexDecBoxViewModel.PropertyChanged += DepthHexDecBoxViewModelPropertyChanged;
            this.DepthHexDecBoxViewModel.DataType = DataTypeBase.Int32;
            this.DepthHexDecBoxViewModel.SetValue(PointerScannerViewModel.DefaultPointerScanDepth);

            this.PointerRadiusHexDecBoxViewModel = this.PointerRadiusHexDecBox.DataContext as HexDecBoxViewModel;
            this.PointerRadiusHexDecBoxViewModel.PropertyChanged += PointerRadiusHexDecBoxViewModelPropertyChanged;
            this.PointerRadiusHexDecBoxViewModel.DataType = DataTypeBase.Int32;
            this.PointerRadiusHexDecBoxViewModel.SetValue(PointerScannerViewModel.DefaultPointerScanRadius);

            Task.Run(() => PointerScanResultsViewModel.GetInstance().Subscribe(this));
        }

        private void PointerScanAddressHexDecBoxViewModelPropertyChanged(Object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(HexDecBoxViewModel.Text))
            {
                Object value = this.PointerScanAddressHexDecBoxViewModel.GetValue();

                if (value == null)
                {
                    return;
                }

                this.PointerScannerViewModel.SetPointerScanAddressCommand.Execute(value);
            }
        }

        private void PointerRetargetAddressHexDecBoxViewModelPropertyChanged(Object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(HexDecBoxViewModel.Text))
            {
                Object value = this.PointerRetargetAddressHexDecBoxViewModel.GetValue();

                if (value == null)
                {
                    return;
                }

                this.PointerScannerViewModel.SetPointerRetargetScanAddressCommand.Execute(value);
            }
        }

        private void DepthHexDecBoxViewModelPropertyChanged(Object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(HexDecBoxViewModel.Text))
            {
                Object value = this.DepthHexDecBoxViewModel.GetValue();
                Int32 realValue = value == null ? 0 : (Int32)Conversions.ParsePrimitiveStringAsPrimitive(DataTypeBase.Int32, value.ToString());

                if (this.DepthHexDecBoxViewModel.IsValid)
                {
                    this.DepthHexDecBoxViewModel.SetValue(realValue.Clamp<Int32>(0, PointerScannerViewModel.MaximumPointerScanDepth));
                }

                this.PointerScannerViewModel.SetDepthCommand.Execute(realValue);
            }
        }

        private void PointerRadiusHexDecBoxViewModelPropertyChanged(Object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(HexDecBoxViewModel.Text))
            {
                Object value = this.PointerRadiusHexDecBoxViewModel.GetValue();
                this.PointerScannerViewModel.SetPointerRadiusCommand.Execute(value == null ? 0 : Conversions.ParsePrimitiveStringAsPrimitive(DataTypeBase.Int32, value.ToString()));
            }
        }

        /// <summary>
        /// Gets the view model associated with this view.
        /// </summary>
        public PointerScannerViewModel PointerScannerViewModel
        {
            get
            {
                return this.DataContext as PointerScannerViewModel;
            }
        }

        /// <summary>
        /// Gets or sets the hex dec box view model used to display the current pointer scan address being edited.
        /// </summary>
        private HexDecBoxViewModel PointerScanAddressHexDecBoxViewModel { get; set; }

        /// <summary>
        /// Gets or sets the value hex dec box view model used to display the current pointer rescan address being edited.
        /// </summary>
        private HexDecBoxViewModel PointerRetargetAddressHexDecBoxViewModel { get; set; }

        /// <summary>
        /// Gets or sets the value hex dec box view model used to display the current depth being edited.
        /// </summary>
        private HexDecBoxViewModel DepthHexDecBoxViewModel { get; set; }

        /// <summary>
        /// Gets or sets the value hex dec box view model used to display the current pointer radius being edited.
        /// </summary>
        private HexDecBoxViewModel PointerRadiusHexDecBoxViewModel { get; set; }

        /// <summary>
        /// Updates the active type.
        /// </summary>
        /// <param name="activeType">The new active type.</param>
        public void Update(DataTypeBase activeType)
        {
            this.PointerRetargetAddressHexDecBoxViewModel.DataType = activeType;
        }
    }
    //// End class
}
//// End namespace