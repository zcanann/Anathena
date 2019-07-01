namespace Squalr.Source.Output
{
    using GalaSoft.MvvmLight.Command;
    using Squalr.Engine.Common.Extensions;
    using Squalr.Engine.Common.Logging;
    using Squalr.Source.Docking;
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Navigation;

    /// <summary>
    /// View model for the Output.
    /// </summary>
    public class OutputViewModel : ToolViewModel, ILoggerObserver
    {

        /// <summary>
        /// Singleton instance of the <see cref="OutputViewModel" /> class.
        /// </summary>
        private static Lazy<OutputViewModel> outputViewModelInstance = new Lazy<OutputViewModel>(
                () => { return new OutputViewModel(); },
                LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// The uri prefix for output inner message 'hyperlinks'.
        /// </summary>
        private const String UriPrefix = @"https://squalr.com/";

        /// <summary>
        /// The rough total capacity in bytes of our log.
        /// </summary>
        private const Int32 LogCapacity = Int16.MaxValue;

        /// <summary>
        /// The minimum number of bytes to clear when going over capacity.
        /// </summary>
        private const Int32 MinimumClearSize = 4096;

        /// <summary>
        /// A value indicating whether the current inner message is visible.
        /// </summary>
        private Boolean innerMessageVisible;

        /// <summary>
        /// The current inner message text.
        /// </summary>
        private String innerMessageText;

        /// <summary>
        /// The log text builder.
        /// </summary>
        private StringBuilder logText;

        /// <summary>
        /// Prevents a default instance of the <see cref="OutputViewModel" /> class from being created.
        /// </summary>
        private OutputViewModel() : base("Output")
        {
            this.AccessLock = new Object();
            this.logText = new StringBuilder(OutputViewModel.LogCapacity);
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            this.ClearOutputCommand = new RelayCommand(() => this.ClearOutput(), () => true);

            DockingViewModel.GetInstance().RegisterViewModel(this);
        }

        /// <summary>
        /// Gets the command to clear the output text.
        /// </summary>
        public ICommand ClearOutputCommand { get; private set; }

        /// <summary>
        /// Gets the log text and builds it from the current string builder.
        /// </summary>
        public String LogText
        {
            get
            {
                lock (this.AccessLock)
                {
                    return this.logText.ToString();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the inner message is visible.
        /// </summary>
        public Boolean InnerMessageVisible
        {
            get
            {
                return this.innerMessageVisible;
            }

            set
            {
                this.innerMessageVisible = value;
                this.RaisePropertyChanged(nameof(this.InnerMessageVisible));
            }
        }

        /// <summary>
        /// Gets or sets the current inner message text.
        /// </summary>
        public String InnerMessageText
        {
            get
            {
                return this.innerMessageText;
            }

            set
            {
                this.innerMessageText = value;
                this.RaisePropertyChanged(nameof(this.InnerMessageText));
            }
        }

        /// <summary>
        /// Gets or sets a lock for access to the output log.
        /// </summary>
        private Object AccessLock { get; set; }

        /// <summary>
        /// Gets a singleton instance of the <see cref="OutputViewModel"/> class.
        /// </summary>
        /// <returns>A singleton instance of the class.</returns>
        public static OutputViewModel GetInstance()
        {
            return OutputViewModel.outputViewModelInstance.Value;
        }

        /// <summary>
        /// Event fired when the user clicks on log text with an inner message.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="args">The navigation args.</param>
        public void LinkRequestNavigate(Object sender, RequestNavigateEventArgs args)
        {
            Hyperlink hyperlink = sender as Hyperlink;

            if (hyperlink != null)
            {
                this.InnerMessageText = HttpUtility.UrlDecode(hyperlink.NavigateUri?.AbsoluteUri.TrimStartString(OutputViewModel.UriPrefix));
                this.InnerMessageVisible = true;
            }
        }

        /// <summary>
        /// Logs a message to output.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The log message.</param>
        /// <param name="innerMessage">The log inner message.</param>
        public void OnLogEvent(LogLevel logLevel, String message, String innerMessage)
        {
            lock (this.AccessLock)
            {
                // Over capacity, remove some of the first lines based on the minimum clear size
                if (this.logText.Length > OutputViewModel.LogCapacity)
                {
                    String currentText = this.LogText.Substring(Math.Min(this.LogText.Length, OutputViewModel.MinimumClearSize));
                    this.logText = new StringBuilder(currentText.Substring(currentText.IndexOf(Environment.NewLine) + 1));
                }

                // Write log message. We do this after the capacity check to avoid any chance of accidentally clearing out this message.
                message = DateTime.Now.ToString("mm:ss.fff") + String.Concat(Enumerable.Repeat(" ", 4)) + "[" + logLevel.ToString() + "] - " + message;
                message = this.FormatAsRtf(message, innerMessage);

                this.logText.AppendLine(message);
            }

            this.RaisePropertyChanged(nameof(this.LogText));
        }

        /// <summary>
        /// Formats a message for display in a rich textbox.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="innerMessage">The tooltip, if applicable.</param>
        /// <returns>The rich textbox formatted message.</returns>
        private String FormatAsRtf(String message, String innerMessage)
        {
            String result = String.Empty;

            // Creating a dummy rich textbox requires doing so on an STA thread for some reason
            Thread thread = new Thread(() =>
            {
                RichTextBox textBox = new RichTextBox();
                textBox.IsDocumentEnabled = true;
                textBox.IsReadOnly = true;

                Boolean hasInnerMessage = !String.IsNullOrWhiteSpace(innerMessage);

                if (hasInnerMessage)
                {
                    Paragraph para = AddToolTip(message, innerMessage);
                    textBox.Document.Blocks.Add(para);
                    textBox.Document.Blocks.Remove(textBox.Document.Blocks.FirstBlock);
                    textBox.Document.Blocks.Add(para);
                }
                else
                {
                    textBox.AppendText(message);
                }

                TextRange textRange = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd);
                textRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.White);
                MemoryStream memoryStream = new MemoryStream();
                textRange.Save(memoryStream, DataFormats.Rtf);

                result = ASCIIEncoding.Default.GetString(memoryStream.ToArray());
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            return result;
        }

        /// <summary>
        /// Creates a tooltip for use in a rich textbox.
        /// </summary>
        /// <param name="message">The display message.</param>
        /// <param name="innerMessage">The tooltip message.</param>
        /// <returns>The tooltip object.</returns>
        private Paragraph AddToolTip(String message, String innerMessage)
        {
            Hyperlink link = new Hyperlink();
            link.IsEnabled = true;
            link.Inlines.Add(message);
            link.NavigateUri = new Uri(OutputViewModel.UriPrefix + HttpUtility.UrlEncode(innerMessage));

            Paragraph para = new Paragraph();
            para.Margin = new Thickness(0);
            para.Inlines.Add(link);

            return para;
        }

        /// <summary>
        /// Clears all output text.
        /// </summary>
        private void ClearOutput()
        {
            lock (this.AccessLock)
            {
                this.logText.Clear();
            }

            this.RaisePropertyChanged(nameof(this.LogText));
        }
    }
    //// End class
}
//// End namespace