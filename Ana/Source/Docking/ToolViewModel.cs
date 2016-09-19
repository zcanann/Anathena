﻿namespace Ana.Source.Docking
{
    using System;

    /// <summary>
    /// Generic view model for all tool panes
    /// </summary>
    internal class ToolViewModel : PaneViewModel
    {
        /// <summary>
        /// Value indicating if tool pane is visible
        /// </summary>
        private Boolean isVisible = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolViewModel" /> class
        /// </summary>
        /// <param name="title">The title to display for the tool pane</param>
        public ToolViewModel(String title)
        {
            this.Title = title;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the tool pane is visible
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
                    this.RaisePropertyChanged(nameof(this.IsVisible));
                }
            }
        }
    }
    //// End class
}
//// End namespace