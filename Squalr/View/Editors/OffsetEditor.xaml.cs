namespace Squalr.View.Editors
{
    using Squalr.Engine.Common.DataStructures;
    using Squalr.Source.Controls;
    using Squalr.Source.Editors.OffsetEditor;
    using Squalr.Source.Mvvm;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// Interaction logic for OffsetEditor.xaml.
    /// </summary>
    public partial class OffsetEditor : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetEditor" /> class.
        /// </summary>
        /// <param name="offsets">The initial offsets to edit.</param>
        public OffsetEditor(IList<Int32> offsets)
        {
            this.InitializeComponent();

            this.OffsetHexDecBoxViewModel = this.OffsetHexDecBox.DataContext as HexDecBoxViewModel;
            this.OffsetHexDecBoxViewModel.PropertyChanged += HexDecBoxViewModelPropertyChanged;

            this.OffsetEditorViewModel.Offsets = offsets == null ? null : new FullyObservableCollection<PrimitiveBinding<Int32>>(offsets.Select(offset => new PrimitiveBinding<Int32>(offset)));
        }

        /// <summary>
        /// Gets the view model associated with this view.
        /// </summary>
        public OffsetEditorViewModel OffsetEditorViewModel
        {
            get
            {
                return this.DataContext as OffsetEditorViewModel;
            }
        }

        /// <summary>
        /// Gets or sets the view model for the offset hex dec box.
        /// </summary>
        private HexDecBoxViewModel OffsetHexDecBoxViewModel { get; set; }

        /// <summary>
        /// Invoked when the added offsets are canceled. Closes the view.
        /// </summary>
        /// <param name="sender">Sending object.</param>
        /// <param name="e">Event args.</param>
        private void CancelButtonClick(Object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Invoked when the added offsets are accepted. Closes the view.
        /// </summary>
        /// <param name="sender">Sending object.</param>
        /// <param name="e">Event args.</param>
        private void AcceptButtonClick(Object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void HexDecBoxViewModelPropertyChanged(Object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OffsetHexDecBoxViewModel.Text))
            {
                this.OffsetEditorViewModel.UpdateActiveValueCommand.Execute(this.OffsetHexDecBoxViewModel.GetValue());
            }
        }

        /// <summary>
        /// Invoked when the selected offset is changed, and informs the viewmodel.
        /// </summary>
        /// <param name="sender">Sending object.</param>
        /// <param name="e">Selection event args.</param>
        private void DataGridSelectionChanged(Object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.OffsetEditorViewModel.SelectedOffsetIndex = this.offsetsDataGrid.SelectedIndex;
        }
    }
    //// End class
}
//// End namespace