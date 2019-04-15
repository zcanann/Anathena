namespace Squalr.Source.Docking
{
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using System;
    using System.Windows;
    using System.Windows.Input;
    using Squalr.Theme;

    /// <summary>
    /// The view model for a window that hosts docked windows.
    /// </summary>
    public abstract class WindowHostViewModel : ViewModelBase
    {
        /// <summary>
        /// Creates an instance of the <see cref="WindowHostViewModel" /> class.
        /// </summary>
        public WindowHostViewModel()
        {
            // Note: These cannot be async, as the logic to update the layout or window cannot be on a new thread
            this.CloseCommand = new RelayCommand<Window>((window) => this.Close(window), (window) => true);
            this.MaximizeRestoreCommand = new RelayCommand<Window>((window) => this.MaximizeRestore(window), (window) => true);
            this.MinimizeCommand = new RelayCommand<Window>((window) => this.Minimize(window), (window) => true);

            this.ResetLayoutCommand = new RelayCommand<DockingManager>((dockingManager)
                => DockingViewModel.GetInstance().LoadLayoutFromResource(dockingManager, this.DefaultLayoutResource), (dockingManager) => true);
            this.LoadLayoutCommand = new RelayCommand<DockingManager>((dockingManager)
                => DockingViewModel.GetInstance().LoadLayoutFromFile(dockingManager, this.LayoutSaveFile, this.DefaultLayoutResource), (dockingManager) => true);
            this.SaveLayoutCommand = new RelayCommand<DockingManager>((dockingManager)
                => DockingViewModel.GetInstance().SaveLayout(dockingManager, this.LayoutSaveFile), (dockingManager) => true);
        }

        /// <summary>
        /// Gets the command to close the main window.
        /// </summary>
        public ICommand CloseCommand { get; private set; }

        /// <summary>
        /// Gets the command to maximize the main window.
        /// </summary>
        public ICommand MaximizeRestoreCommand { get; private set; }

        /// <summary>
        /// Gets the command to minimize the main window.
        /// </summary>
        public ICommand MinimizeCommand { get; private set; }

        /// <summary>
        /// Gets the command to reset the current docking layout to the default.
        /// </summary>
        public ICommand ResetLayoutCommand { get; private set; }

        /// <summary>
        /// Gets the command to open the current docking layout.
        /// </summary>
        public ICommand LoadLayoutCommand { get; private set; }

        /// <summary>
        /// Gets the command to save the current docking layout.
        /// </summary>
        public ICommand SaveLayoutCommand { get; private set; }

        /// <summary>
        /// Gets or sets the fallback default layout resource to load when there is no save file.
        /// </summary>
        protected abstract String DefaultLayoutResource { get; }

        /// <summary>
        /// Gets or sets the layout save file.
        /// </summary>
        protected abstract String LayoutSaveFile { get; }

        /// <summary>
        /// Closes the main window.
        /// </summary>
        /// <param name="window">The window to close.</param>
        protected virtual void Close(Window window)
        {
            window?.Close();
        }

        /// <summary>
        /// Maximizes or Restores the main window.
        /// </summary>
        /// <param name="window">The window to maximize or restore.</param>
        private void MaximizeRestore(Window window)
        {
            if (window == null)
            {
                return;
            }

            if (window.WindowState != WindowState.Maximized)
            {
                window.WindowState = WindowState.Maximized;
            }
            else
            {
                window.WindowState = WindowState.Normal;
            }
        }

        /// <summary>
        /// Minimizes the main window.
        /// </summary>
        /// <param name="window">The window to minimize.</param>
        private void Minimize(Window window)
        {
            window.WindowState = WindowState.Minimized;
        }
    }
    //// End class
}
//// End namesapce