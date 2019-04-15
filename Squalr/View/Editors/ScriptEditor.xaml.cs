namespace Squalr.View.Editors
{
    using Source.Editors.ScriptEditor;
    using Squalr.Engine.Logging;
    using Squalr.Engine.Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Xml;

    /// <summary>
    /// Interaction logic for ScriptEditor.xaml.
    /// </summary>
    public partial class ScriptEditor : Window
    {
        /// <summary>
        /// The resource file for C# script highlighting rules.
        /// </summary>
        private const String ScriptSyntaxHighlightingResource = "CSharp.xshd";

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptEditor" /> class.
        /// </summary>
        /// <param name="script">The initial script text being edited.</param>
        public ScriptEditor(String script = null)
        {
            this.InitializeComponent();
            this.LoadHightLightRules();
            this.InitializeCompleteionWindow();
            //this.ScriptEditorTextEditor.FontFamily = new FontFamily("Consolas");
            //// this.ScriptEditorTextEditor.TextArea.TextEntering += this.ScriptEditorTextEditorTextAreaTextEntering;
            //// this.ScriptEditorTextEditor.TextArea.TextEntered += this.ScriptEditorTextEditorTextAreaTextEntered;
            //this.ScriptEditorTextEditor.TextChanged += this.ScriptEditorTextEditorTextChanged;
            //this.ScriptEditorTextEditor.Text = script ?? String.Empty;
        }

        /// <summary>
        /// Gets the view model associated with this view.
        /// </summary>
        public ScriptEditorViewModel ScriptEditorViewModel
        {
            get
            {
                return this.DataContext as ScriptEditorViewModel;
            }
        }

        /// <summary>
        /// Gets or sets the completion fields.
        /// </summary>
       // private IList<ICompletionData> CompletionData { get; set; }

        /// <summary>
        /// Gets or sets the completion window.
        /// </summary>
       // private CompletionWindow CompletionWindow { get; set; }

        /// <summary>
        /// Initializes fields that can be shown in the completion window.
        /// </summary>
        private void InitializeCompleteionWindow()
        {
          //  this.CompletionData = new List<ICompletionData>();
           // this.CompletionData.Add(new AutoCompleteData("Memory"));
          //  this.CompletionData.Add(new AutoCompleteData("Engine"));
          //  this.CompletionData.Add(new AutoCompleteData("Graphics"));
        }

        /// <summary>
        /// Loads highlighting rules from the highlighting resource file.
        /// </summary>
        private void LoadHightLightRules()
        {
            String highlightingResource = Assembly.GetExecutingAssembly().GetManifestResourceNames()
                .FirstOrDefault(resourceName => resourceName.EndsWith(ScriptEditor.ScriptSyntaxHighlightingResource));

            if (highlightingResource.IsNullOrEmpty())
            {
                Logger.Log(LogLevel.Error, "Unable to load code highlighting rules. Scripts will be affected");
                return;
            }

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(highlightingResource))
            {
                if (stream != null)
                {
                    using (XmlTextReader reader = new XmlTextReader(stream))
                    {
                        //this.ScriptEditorTextEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when the script editor text is changed.
        /// </summary>
        /// <param name="sender">Sending object.</param>
        /// <param name="e">Event args.</param>
        private void ScriptEditorTextEditorTextChanged(Object sender, EventArgs e)
        {
           // this.ScriptEditorViewModel.UpdateScriptCommand.Execute(this.ScriptEditorTextEditor.Text);
        }

        /// <summary>
        /// Invoked when the script editor text area is entered.
        /// </summary>
        /// <param name="sender">Sending object.</param>
        /// <param name="e">Event args.</param>
        private void ScriptEditorTextEditorTextAreaTextEntered(Object sender, TextCompositionEventArgs e)
        {
            /*
            //// if (e.Text == ".")
            {
                //// Open code completion after the user has pressed dot:
                this.CompletionWindow = new CompletionWindow(this.ScriptEditorTextEditor.TextArea);
                this.CompletionWindow.Closed += delegate
                {
                    this.CompletionWindow = null;
                };

                foreach (ICompletionData data in this.CompletionData)
                {
                    this.CompletionWindow.CompletionList.CompletionData.Add(data);
                }

                this.CompletionWindow.Show();
            }
            */
        }

        /// <summary>
        /// Invoked when the script editor text is entered.
        /// </summary>
        /// <param name="sender">Sending object.</param>
        /// <param name="e">Event args.</param>
        private void ScriptEditorTextEditorTextAreaTextEntering(Object sender, TextCompositionEventArgs e)
        {
           // if (e.Text.Length > 0 && this.CompletionWindow != null)
            {
             //   if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open, insert the currently selected element
                   // this.CompletionWindow.CompletionList.RequestInsertion(e);
                }
            }
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

        /// <summary>
        /// Invoked when the code injection file menu event executes.
        /// </summary>
        /// <param name="sender">Sending object.</param>
        /// <param name="e">Event args.</param>
        private void CodeInjectionFileMenuItemClick(Object sender, RoutedEventArgs e)
        {
          //  this.ScriptEditorTextEditor.Text = this.ScriptEditorViewModel.GetCodeInjectionTemplate() + this.ScriptEditorTextEditor.Text;
        }

        /// <summary>
        /// Invoked when the graphics overlay file menu event executes.
        /// </summary>
        /// <param name="sender">Sending object.</param>
        /// <param name="e">Event args.</param>
        private void GraphicsOverlayFileMenuItemClick(Object sender, RoutedEventArgs e)
        {
           // this.ScriptEditorTextEditor.Text = this.ScriptEditorViewModel.GetGraphicsInjectionTemplate() + this.ScriptEditorTextEditor.Text;
        }

        /// <summary>
        /// Implements AvalonEdit ICompletionData interface to provide the entries in the completion drop down.
        /// </summary>
        /*
        internal class AutoCompleteData : ICompletionData
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AutoCompleteData" /> class.
            /// </summary>
            /// <param name="text">The auto completion text.</param>
            public AutoCompleteData(String text)
            {
                this.Text = text;
            }

            /// <summary>
            /// Gets the image associated with this auto complete data.
            /// </summary>
            public ImageSource Image
            {
                get { return null; }
            }

            /// <summary>
            /// Gets the auto complete text.
            /// </summary>
            public String Text { get; private set; }

            /// <summary>
            /// Gets an optional UIElement to show in the list.
            /// </summary>
            public Object Content
            {
                get
                {
                    return this.Text;
                }
            }

            /// <summary>
            /// Gets the description associated with this autocomplete element.
            /// </summary>
            public Object Description
            {
                get
                {
                    return String.Empty;
                }
            }

            /// <summary>
            /// Gets the priority of this autocomplete element.
            /// </summary>
            public Double Priority
            {
                get
                {
                    return 1.0;
                }
            }

            /// <summary>
            /// Invoked when the auto complete is accepted.
            /// </summary>
            /// <param name="textArea">Text area.</param>
            /// <param name="completionSegment">Completion segment.</param>
            /// <param name="insertionRequestEventArgs">Insertion event args.</param>
            public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
            {
                textArea.Document.Replace(completionSegment, this.Text);
            }
        }
        */
        //// End class
    }
    //// End class
}
//// End namespace