namespace Squalr.Source.ProjectExplorer.ProjectItems
{
    using Squalr.Engine.Common.DataStructures;
    using Squalr.Engine.Projects.Items;
    using System;
    using System.ComponentModel;

    internal class ProjectItemView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ProjectItem projectItem;

        private Boolean isSelected;

        /// <summary>
        /// Indicates that a given property in this project item has changed.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        protected void RaisePropertyChanged(String propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Browsable(false)]
        public virtual FullyObservableCollection<ProjectItem> ChildItems
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the value at this address.
        /// </summary>
        [Browsable(false)]
        public Boolean IsActivated
        {
            get
            {
                return this.ProjectItem.IsActivated;
            }

            set
            {
                this.ProjectItem.IsActivated = value;
                this.RaisePropertyChanged(nameof(this.IsActivated));
            }
        }

        [Browsable(false)]
        public Boolean IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                this.isSelected = value;
                this.RaisePropertyChanged(nameof(this.IsSelected));
            }
        }

        [Browsable(false)]
        public virtual Boolean IsExpanded
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        [Browsable(false)]
        public ProjectItem ProjectItem
        {
            get
            {
                return this.projectItem;
            }

            set
            {
                this.projectItem = value;
                this.RaisePropertyChanged(nameof(this.ProjectItem));
            }
        }

        public virtual Object DisplayValue
        {
            get
            {
                return String.Empty;
            }

            set
            {
            }
        }
    }
    //// End class
}
//// End namespace