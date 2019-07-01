namespace Squalr.View
{
    using Squalr.Engine.Common.DataTypes;
    using Squalr.Source.Controls;
    using Squalr.Source.Results;
    using Squalr.Source.Scanning;
    using System;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    internal partial class MainWindow : Window, IResultDataTypeObserver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManualScanner"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            this.ValueHexDecBoxViewModel = this.ValueHexDecBox.DataContext as HexDecBoxViewModel;
            this.ValueHexDecBoxViewModel.PropertyChanged += HexDecBoxViewModelPropertyChanged;

            Task.Run(() => ScanResultsViewModel.GetInstance().Subscribe(this));
        }

        private HexDecBoxViewModel ValueHexDecBoxViewModel { get; set; }

        /// <summary>
        /// Updates the active type.
        /// </summary>
        /// <param name="activeType">The new active type.</param>
        public void Update(DataTypeBase activeType)
        {
            this.ValueHexDecBoxViewModel.DataType = activeType;
        }

        private void HexDecBoxViewModelPropertyChanged(Object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ValueHexDecBoxViewModel.Text))
            {
                ManualScannerViewModel.GetInstance().UpdateActiveValueCommand.Execute(this.ValueHexDecBoxViewModel.GetValue());
            }
        }
    }
    //// End class
}
//// End namespace