namespace Squalr.Source.DotNetExplorer
{
    using Engine.AddressResolver.DotNet;
    using GalaSoft.MvvmLight.Command;
    using Squalr.Engine.DataTypes;
    using Squalr.Engine.Memory.Clr;
    using Squalr.Engine.Projects.Items;
    using Squalr.Source.Docking;
    using Squalr.Source.ProjectExplorer;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Windows.Input;

    /// <summary>
    /// View model for the .Net Explorer.
    /// </summary>
    internal class DotNetExplorerViewModel : ToolViewModel
    {
        /// <summary>
        /// Singleton instance of the <see cref="DotNetExplorerViewModel" /> class.
        /// </summary>
        private static Lazy<DotNetExplorerViewModel> dotNetExplorerViewModelInstance = new Lazy<DotNetExplorerViewModel>(
                () => { return new DotNetExplorerViewModel(); },
                LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// The collection of dot net objects in the target process.
        /// </summary>
        private ReadOnlyCollection<DotNetObjectViewModel> dotNetObjects;

        /// <summary>
        /// Prevents a default instance of the <see cref="DotNetExplorerViewModel" /> class from being created.
        /// </summary>
        private DotNetExplorerViewModel() : base(".Net Explorer")
        {
            this.dotNetObjects = new ReadOnlyCollection<DotNetObjectViewModel>(new List<DotNetObjectViewModel>());
            this.RefreshObjectsCommand = new RelayCommand(() => this.RefreshObjects(), () => true);
            this.AddDotNetObjectCommand = new RelayCommand<DotNetObjectViewModel>((dotNetObjectViewModel) => this.AddDotNetObject(dotNetObjectViewModel), (dotNetObjectViewModel) => true);

            DockingViewModel.GetInstance().RegisterViewModel(this);
        }

        /// <summary>
        /// Gets a command to refresh the list of active .Net objects based on gathered managed heap data.
        /// </summary>
        public ICommand RefreshObjectsCommand { get; private set; }

        /// <summary>
        /// Gets a command to add a .Net object to the project explorer.
        /// </summary>
        public ICommand AddDotNetObjectCommand { get; private set; }

        /// <summary>
        /// Gets or sets the collection of dot net objects in the target process.
        /// </summary>
        public ReadOnlyCollection<DotNetObjectViewModel> DotNetObjects
        {
            get
            {
                return this.dotNetObjects;
            }

            set
            {
                this.dotNetObjects = value;
                this.RaisePropertyChanged(nameof(this.DotNetObjects));
            }
        }

        /// <summary>
        /// Gets a singleton instance of the <see cref="DotNetExplorerViewModel"/> class.
        /// </summary>
        /// <returns>A singleton instance of the class.</returns>
        public static DotNetExplorerViewModel GetInstance()
        {
            return dotNetExplorerViewModelInstance.Value;
        }

        /// <summary>
        /// Refreshes the list of active .Net objects based on gathered managed heap data.
        /// </summary>
        private void RefreshObjects()
        {
            IEnumerable<DotNetObject> dotNetObjects = DotNetObjectCollector.GetInstance().ObjectTrees;

            if (dotNetObjects == null)
            {
                dotNetObjects = new List<DotNetObject>();
            }

            this.DotNetObjects = new ReadOnlyCollection<DotNetObjectViewModel>(dotNetObjects.Select(x => new DotNetObjectViewModel(x)).ToList());
        }

        /// <summary>
        /// Adds a .Net object to the project explorer.
        /// </summary>
        /// <param name="dotNetObjectViewModel">The view model of the .Net object.</param>
        private void AddDotNetObject(DotNetObjectViewModel dotNetObjectViewModel)
        {
            DotNetObject dotNetObject = dotNetObjectViewModel.DotNetObject;
            DotNetItem dotnetItem = new DotNetItem(
                dotNetObject.Name,
                dotNetObject.ElementType == DataType.Boolean ? DataType.Byte : dotNetObject.ElementType,
                dotNetObject.GetFullName());

            ProjectExplorerViewModel.GetInstance().AddProjectItems(dotnetItem);
        }
    }
    //// End class
}
//// End namespace