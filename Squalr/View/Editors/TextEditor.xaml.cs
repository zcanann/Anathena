namespace Squalr.View.Editors
{
    using Squalr.Source.Editors.TextEditor;
    using System;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for TextEditor.xaml.
    /// </summary>
    public partial class TextEditor : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextEditor" /> class.
        /// </summary>
        /// <param name="text">The initial text being edited.</param>
        public TextEditor(String text = null)
        {
            this.InitializeComponent();
            //this.TextEditorTextEditor.FontFamily = new FontFamily("Consolas");
           // this.TextEditorTextEditor.TextChanged += this.TextEditorTextEditorTextChanged;
           // this.TextEditorTextEditor.Text = text ?? String.Empty;
        }

        /// <summary>
        /// Gets the view model associated with this view.
        /// </summary>
        public TextEditorViewModel TextEditorViewModel
        {
            get
            {
                return this.DataContext as TextEditorViewModel;
            }
        }

        /// <summary>
        /// Invoked when the text editor text is changed.
        /// </summary>
        /// <param name="sender">Sending object.</param>
        /// <param name="e">Event args.</param>
        private void TextEditorTextEditorTextChanged(Object sender, EventArgs e)
        {
          //  this.TextEditorViewModel.UpdateTextCommand.Execute(this.TextEditorTextEditor.Text);
        }

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

        /// <summary>
        /// Invoked when the exit file menu event executes. Closes the view.
        /// </summary>
        /// <param name="sender">Sending object.</param>
        /// <param name="e">Event args.</param>
        private void ExitFileMenuItemClick(Object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
    //// End class
}
//// End namespace