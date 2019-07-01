namespace Squalr.Source.Controls
{
    using GalaSoft.MvvmLight;
    using Squalr.Engine.Common.DataStructures;
    using System;

    /// <summary>
    /// Base class for all ViewModel classes displayed by TreeViewItems.  
    /// This acts as an adapter between a raw data object and a TreeViewItem.
    /// </summary>
    public class TreeViewItemViewModel : ViewModelBase
    {
        /// <summary>
        /// Dummy child for nodes with dynamically loaded children.
        /// </summary>
        private static readonly TreeViewItemViewModel DummyChild = new TreeViewItemViewModel();

        /// <summary>
        /// Root items in the collection.
        /// </summary>
        private readonly FullyObservableCollection<TreeViewItemViewModel> children;

        /// <summary>
        /// The parent tree view of this node.
        /// </summary>
        private readonly TreeViewItemViewModel parent;

        /// <summary>
        /// Whether or not this node has its children expanded.
        /// </summary>
        private Boolean isExpanded;

        /// <summary>
        /// Whether or not this node is selected.
        /// </summary>
        private Boolean isSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewItemViewModel" /> class.
        /// </summary>
        /// <param name="parent">The parent of this node.</param>
        /// <param name="lazyLoadChildren">Whether children need to be lazy loaded.</param>
        protected TreeViewItemViewModel(TreeViewItemViewModel parent, Boolean lazyLoadChildren = false)
        {
            this.parent = parent;
            this.children = new FullyObservableCollection<TreeViewItemViewModel>();

            if (lazyLoadChildren)
            {
                this.children.Add(DummyChild);
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="TreeViewItemViewModel" /> class from being created. This is used to create the DummyChild instance.
        /// </summary>
        private TreeViewItemViewModel()
        {
        }

        /// <summary>
        /// Gets the parent tree view of this node.
        /// </summary>
        public virtual TreeViewItemViewModel Parent
        {
            get
            {
                return this.parent;
            }
        }

        /// <summary>
        /// Gets the logical child items of this object.
        /// </summary>
        public virtual FullyObservableCollection<TreeViewItemViewModel> Children
        {
            get
            {
                return this.children;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this object's children have not yet been populated.
        /// </summary>
        public Boolean HasDummyChild
        {
            get
            {
                return this.Children.Count == 1 && this.Children[0] == TreeViewItemViewModel.DummyChild;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the TreeViewItem associated with this object is expanded.
        /// </summary>
        public Boolean IsExpanded
        {
            get
            {
                return this.isExpanded;
            }

            set
            {
                if (value != this.isExpanded)
                {
                    this.isExpanded = value;
                    this.RaisePropertyChanged(nameof(this.IsExpanded));
                }

                // Expand all the way up to the root.
                if (this.isExpanded && this.parent != null)
                {
                    this.parent.IsExpanded = true;
                }

                // Lazy load the child items, if necessary.
                if (this.HasDummyChild)
                {
                    this.Children.Remove(TreeViewItemViewModel.DummyChild);
                    this.LoadChildren();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the TreeViewItem associated with this object is selected.
        /// </summary>
        public Boolean IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                if (value != this.isSelected)
                {
                    this.isSelected = value;
                    this.OnSelected();
                    this.RaisePropertyChanged(nameof(this.IsSelected));
                }
            }
        }

        /// <summary>
        /// Invoked when the tree view model is selected.
        /// </summary>
        protected virtual void OnSelected()
        {
        }

        /// <summary>
        /// Invoked when the child items need to be loaded on demand. Subclasses can override this to populate the Children collection.
        /// </summary>
        protected virtual void LoadChildren()
        {
        }
    }
    //// End class
}
//// End namespace