namespace Squalr.Source.Engine.AddressResolver.DotNet
{
    using Squalr.Engine.Common.DataStructures;
    using Squalr.Engine.Memory.Clr;
    using Squalr.Source.Controls;
    using Squalr.Source.PropertyViewer;
    using System.Linq;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// View model for an individual .Net object.
    /// </summary>
    internal class DotNetObjectViewModel : TreeViewItemViewModel
    {
        /// <summary>
        /// The .Net object model.
        /// </summary>
        private readonly DotNetObject dotNetObject;

        /// <summary>
        /// The children view models under this object.
        /// </summary>
        private FullyObservableCollection<TreeViewItemViewModel> children;

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetObjectViewModel" /> class.
        /// </summary>
        /// <param name="dotNetObject">The .Net object model.</param>
        /// <param name="parent">The parent to this .Net object view model.</param>
        public DotNetObjectViewModel(DotNetObject dotNetObject, TreeViewItemViewModel parent = null) : base(parent)
        {
            this.dotNetObject = dotNetObject;
            this.children = new FullyObservableCollection<TreeViewItemViewModel>(this.DotNetObject.Children.Select(x => new DotNetObjectViewModel(x)));
            this.RaisePropertyChanged(nameof(this.Children));
        }

        /// <summary>
        /// Gets the children view models under this object.
        /// </summary>
        public override FullyObservableCollection<TreeViewItemViewModel> Children
        {
            get
            {
                return this.children;
            }
        }

        /// <summary>
        /// Gets the .Net object that this view model represents.
        /// </summary>
        public DotNetObject DotNetObject
        {
            get
            {
                return this.dotNetObject;
            }
        }

        /// <summary>
        /// Gets the icon associated with this .Net object.
        /// </summary>
        public BitmapImage Icon
        {
            get
            {
                // TODO: It would be cool to have an icon based on object type.
                return null;
            }
        }

        /// <summary>
        /// Invoked when this view model is selected.
        /// </summary>
        protected override void OnSelected()
        {
            PropertyViewerViewModel.GetInstance().SetTargetObjects(this.DotNetObject);
        }
    }
    //// End class
}
//// End namespace