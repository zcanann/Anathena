namespace Squalr.Source.Docking
{
    using GalaSoft.MvvmLight.Command;
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Generic view model for all tool panes.
    /// </summary>
    public abstract class ToolViewModel : PaneViewModel
    {
        /// <summary>
        /// Value indicating if tool pane is visible.
        /// </summary>
        private Boolean isVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolViewModel" /> class.
        /// </summary>
        /// <param name="title">The title to display for the tool pane.</param>
        public ToolViewModel(String title)
        {
            this.Title = title;
            this.Show = new RelayCommand(() => this.ShowExecute(), () => true);
            this.Hide = new RelayCommand(() => this.HideExecute(), () => true);
            this.ToggleVisibility = new RelayCommand(() => this.ToggleVisibilityExecute(), () => true);
        }

        /// <summary>
        /// Gets a command that shows this tool.
        /// </summary>
        public ICommand Show { get; private set; }

        /// <summary>
        /// Gets a command that hides this tool.
        /// </summary>
        public ICommand Hide { get; private set; }

        /// <summary>
        /// Gets a command that toggles the visibility of this tool.
        /// </summary>
        public ICommand ToggleVisibility { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the tool pane is visible.
        /// </summary>
        public Boolean IsVisible
        {
            get
            {
                return this.isVisible;
            }

            set
            {
                if (this.isVisible != value)
                {
                    this.isVisible = value;
                    this.OnVisibilityChanged();
                    this.RaisePropertyChanged(nameof(this.IsVisible));
                }
            }
        }

        /// <summary>
        /// Shows this tool.
        /// </summary>
        public void ShowExecute()
        {
            this.IsVisible = true;
        }

        /// <summary>
        /// Hide this tool.
        /// </summary>
        public void HideExecute()
        {
            this.IsVisible = false;
        }

        /// <summary>
        /// Toggles the visibility of this tool.
        /// </summary>
        public void ToggleVisibilityExecute()
        {
            this.IsVisible = !this.IsVisible;
        }

        /// <summary>
        /// Called when the visibility of this tool is changed.
        /// </summary>
        protected virtual void OnVisibilityChanged()
        {
        }
    }
    //// End class
}
//// End namespace