namespace Squalr.Source.ChangeLog
{
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// View model for the Change Log.
    /// </summary>
    public class ChangeLogViewModel : ViewModelBase
    {
        /// <summary>
        /// Singleton instance of the <see cref="ChangeLogViewModel"/> class.
        /// </summary>
        private static Lazy<ChangeLogViewModel> changeLogViewModelInstance = new Lazy<ChangeLogViewModel>(
                () => { return new ChangeLogViewModel(); },
                LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// The changelog text.
        /// </summary>
        private String changeLog;

        /// <summary>
        /// Prevents a default instance of the <see cref="ChangeLogViewModel" /> class from being created.
        /// </summary>
        private ChangeLogViewModel()
        {
            this.CloseWindowCommand = new RelayCommand<Window>(this.CloseWindow);
        }

        /// <summary>
        /// Gets a command to close the changelog window.
        /// </summary>
        public ICommand CloseWindowCommand { get; private set; }

        /// <summary>
        /// Gets the changelog text.
        /// </summary>
        public String ChangeLog
        {
            get
            {
                return this.changeLog;
            }

            private set
            {
                this.changeLog = value;
                this.RaisePropertyChanged(nameof(this.ChangeLog));
            }
        }

        /// <summary>
        /// Gets the title, including version, of the changelog.
        /// </summary>
        public String Title
        {
            get
            {
                return "Change Log - " + Assembly.GetEntryAssembly().GetName().Version.ToString();
            }
        }

        /// <summary>
        /// Gets a singleton instance of the <see cref="ChangeLogViewModel"/> class.
        /// </summary>
        /// <returns>A singleton instance of the class.</returns>
        public static ChangeLogViewModel GetInstance()
        {
            return ChangeLogViewModel.changeLogViewModelInstance.Value;
        }

        /// <summary>
        /// Displays the change log to the user if there has been a recent update.
        /// </summary>
        public void DisplayChangeLog(String changeLogText)
        {
            /*
            this.ChangeLog = changeLogText;
            View.ChangeLog changeLog = new View.ChangeLog();
            changeLog.Owner = Application.Current.MainWindow;
            changeLog.ShowDialog();
            */
        }

        /// <summary>
        /// Closes the changelog window.
        /// </summary>
        /// <param name="window">The changelog window.</param>
        private void CloseWindow(Window window)
        {
            window?.Close();
        }
    }
    //// End class
}
//// End namespace