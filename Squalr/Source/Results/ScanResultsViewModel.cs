namespace Squalr.Source.Results
{
    using GalaSoft.MvvmLight.Command;
    using Squalr.Engine.DataTypes;
    using Squalr.Engine.Memory;
    using Squalr.Engine.Projects.Items;
    using Squalr.Engine.Scanning.Snapshots;
    using Squalr.Engine.Utils;
    using Squalr.Engine.Utils.DataStructures;
    using Squalr.Engine.Utils.Extensions;
    using Squalr.Source.Docking;
    using Squalr.Source.Editors.ValueEditor;
    using Squalr.Source.ProjectExplorer;
    using Squalr.Source.ProjectExplorer.ProjectItems;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// View model for the scan results.
    /// </summary>
    internal class ScanResultsViewModel : ToolViewModel, ISnapshotObserver
    {
        /// <summary>
        /// The number of elements to display on each page.
        /// </summary>
        private const Int32 PageSize = 64;

        /// <summary>
        /// Singleton instance of the <see cref="ScanResultsViewModel" /> class.
        /// </summary>
        private static Lazy<ScanResultsViewModel> scanResultsViewModelInstance = new Lazy<ScanResultsViewModel>(
                () => { return new ScanResultsViewModel(); },
                LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// The active data type for the scan results.
        /// </summary>
        private Type activeType;

        /// <summary>
        /// The current page of scan results.
        /// </summary>
        private UInt64 currentPage;

        /// <summary>
        /// The total number of addresses.
        /// </summary>
        private UInt64 addressCount;

        /// <summary>
        /// The total number of bytes in memory.
        /// </summary>
        private UInt64 byteCount;

        /// <summary>
        /// The addresses on the current page.
        /// </summary>
        private FullyObservableCollection<ScanResult> addresses;

        /// <summary>
        /// The selected scan results.
        /// </summary>
        private IEnumerable<ScanResult> selectedScanResults;

        /// <summary>
        /// Prevents a default instance of the <see cref="ScanResultsViewModel" /> class from being created.
        /// </summary>
        private ScanResultsViewModel() : base("Scan Results")
        {
            this.ObserverLock = new Object();

            this.EditValueCommand = new RelayCommand<ScanResult>((scanResult) => this.EditValue(scanResult), (scanResult) => true);
            this.ChangeTypeCommand = new RelayCommand<DataType>((type) => this.ChangeType(type), (type) => true);
            this.SelectScanResultsCommand = new RelayCommand<Object>((selectedItems) => this.SelectedScanResults = (selectedItems as IList)?.Cast<ScanResult>(), (selectedItems) => true);
            this.FirstPageCommand = new RelayCommand(() => Task.Run(() => this.FirstPage()), () => true);
            this.LastPageCommand = new RelayCommand(() => Task.Run(() => this.LastPage()), () => true);
            this.PreviousPageCommand = new RelayCommand(() => Task.Run(() => this.PreviousPage()), () => true);
            this.NextPageCommand = new RelayCommand(() => Task.Run(() => this.NextPage()), () => true);
            this.AddScanResultCommand = new RelayCommand<ScanResult>((scanResult) => this.AddScanResult(scanResult), (scanResult) => true);
            this.AddScanResultsCommand = new RelayCommand<Object>((selectedItems) => this.AddScanResults(this.SelectedScanResults), (selectedItems) => true);

            this.ScanResultsObservers = new List<IResultDataTypeObserver>();
            this.ActiveType = DataType.Int32;
            this.addresses = new FullyObservableCollection<ScanResult>();

            SnapshotManager.Subscribe(this);
            DockingViewModel.GetInstance().RegisterViewModel(this);
            this.Update();
        }

        /// <summary>
        /// Gets the command to edit the specified address item.
        /// </summary>
        public ICommand EditValueCommand { get; private set; }

        /// <summary>
        /// Gets the command to change the active data type.
        /// </summary>
        public ICommand ChangeTypeCommand { get; private set; }

        /// <summary>
        /// Gets or sets the command to select scan results.
        /// </summary>
        public ICommand SelectScanResultsCommand { get; private set; }

        /// <summary>
        /// Gets the command to go to the first page.
        /// </summary>
        public ICommand FirstPageCommand { get; private set; }

        /// <summary>
        /// Gets the command to go to the last page.
        /// </summary>
        public ICommand LastPageCommand { get; private set; }

        /// <summary>
        /// Gets the command to go to the previous page.
        /// </summary>
        public ICommand PreviousPageCommand { get; private set; }

        /// <summary>
        /// Gets the command to go to the next page.
        /// </summary>
        public ICommand NextPageCommand { get; private set; }

        /// <summary>
        /// Gets the command to add a scan result to the project explorer.
        /// </summary>
        public ICommand AddScanResultCommand { get; private set; }

        /// <summary>
        /// Gets the command to add all selected scan results to the project explorer.
        /// </summary>
        public ICommand AddScanResultsCommand { get; private set; }

        /// <summary>
        /// Gets or sets the selected scan results.
        /// </summary>
        public IEnumerable<ScanResult> SelectedScanResults
        {
            get
            {
                return this.selectedScanResults;
            }

            set
            {
                this.selectedScanResults = value;
                this.RaisePropertyChanged(nameof(this.SelectedScanResults));
            }
        }

        /// <summary>
        /// Gets or sets the active scan results data type.
        /// </summary>
        public DataType ActiveType
        {
            get
            {
                return this.activeType;
            }

            set
            {
                this.activeType = value;

                // Update data type of addresses
                this.Addresses?.ToArray().ForEach(address => address.PointerItem.DataType = this.ActiveType);

                this.NotifyObservers();
                this.RaisePropertyChanged(nameof(this.ActiveType));
                this.RaisePropertyChanged(nameof(this.ActiveTypeName));
            }
        }

        /// <summary>
        /// Gets the name associated with the active data type.
        /// </summary>
        public String ActiveTypeName
        {
            get
            {
                return Conversions.DataTypeToName(this.ActiveType);
            }
        }

        /// <summary>
        /// Gets or sets the total number of addresses found.
        /// </summary>
        public UInt64 CurrentPage
        {
            get
            {
                return this.currentPage;
            }

            set
            {
                this.currentPage = value;
                this.LoadScanResults();
                this.RaisePropertyChanged(nameof(this.CurrentPage));
                this.RaisePropertyChanged(nameof(this.CanNavigateFirst));
                this.RaisePropertyChanged(nameof(this.CanNavigatePrevious));
                this.RaisePropertyChanged(nameof(this.CanNavigateNext));
                this.RaisePropertyChanged(nameof(this.CanNavigateLast));
            }
        }

        /// <summary>
        /// Gets a value indicating whether first page navigation is available.
        /// </summary>
        public Boolean CanNavigateFirst
        {
            get
            {
                return this.PageCount > 0 && this.CurrentPage > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether next page navigation is available.
        /// </summary>
        public Boolean CanNavigateNext
        {
            get
            {
                return this.CurrentPage < this.PageCount;
            }
        }

        /// <summary>
        /// Gets a value indicating whether previous page navigation is available.
        /// </summary>
        public Boolean CanNavigatePrevious
        {
            get
            {
                return this.CurrentPage > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether last page navigation is available.
        /// </summary>
        public Boolean CanNavigateLast
        {
            get
            {
                return this.PageCount > 0 && this.CurrentPage != this.PageCount;
            }
        }

        /// <summary>
        /// Gets the total number of addresses found.
        /// </summary>
        public UInt64 PageCount
        {
            get
            {
                return this.ResultCount == 0 ? 0 : (this.ResultCount - 1) / ScanResultsViewModel.PageSize;
            }
        }

        /// <summary>
        /// Gets or sets the total number of bytes found.
        /// </summary>
        public UInt64 ByteCount
        {
            get
            {
                return this.byteCount;
            }

            set
            {
                this.byteCount = value;
                this.RaisePropertyChanged(nameof(this.ByteCount));
            }
        }

        /// <summary>
        /// Gets or sets the total number of addresses found.
        /// </summary>
        public UInt64 ResultCount
        {
            get
            {
                return this.addressCount;
            }

            set
            {
                this.addressCount = value;
                this.RaisePropertyChanged(nameof(this.ResultCount));
                this.RaisePropertyChanged(nameof(this.PageCount));
            }
        }

        /// <summary>
        /// Gets the address elements.
        /// </summary>
        public FullyObservableCollection<ScanResult> Addresses
        {
            get
            {
                return this.addresses;
            }

            set
            {
                this.addresses = value;
                this.RaisePropertyChanged(nameof(this.Addresses));
            }
        }

        /// <summary>
        /// Gets or sets the lock for accessing observers.
        /// </summary>
        private Object ObserverLock { get; set; }

        /// <summary>
        /// Gets or sets objects observing changes in the scan results data type.
        /// </summary>
        private List<IResultDataTypeObserver> ScanResultsObservers { get; set; }

        /// <summary>
        /// Gets a singleton instance of the <see cref="ScanResultsViewModel"/> class.
        /// </summary>
        /// <returns>A singleton instance of the class.</returns>
        public static ScanResultsViewModel GetInstance()
        {
            return ScanResultsViewModel.scanResultsViewModelInstance.Value;
        }

        /// <summary>
        /// Subscribes the given object to changes in the scan results data type.
        /// </summary>
        /// <param name="snapshotObserver">The object to observe scan results data type changes.</param>
        public void Subscribe(IResultDataTypeObserver snapshotObserver)
        {
            lock (this.ObserverLock)
            {
                if (!this.ScanResultsObservers.Contains(snapshotObserver))
                {
                    this.ScanResultsObservers.Add(snapshotObserver);
                    snapshotObserver.Update(this.ActiveType);
                }
            }
        }

        /// <summary>
        /// Unsubscribes the given object from changes in the scan results data type.
        /// </summary>
        /// <param name="snapshotObserver">The object to observe scan results data type changes.</param>
        public void Unsubscribe(IResultDataTypeObserver snapshotObserver)
        {
            lock (this.ObserverLock)
            {
                if (this.ScanResultsObservers.Contains(snapshotObserver))
                {
                    this.ScanResultsObservers.Remove(snapshotObserver);
                }
            }
        }

        /// <summary>
        /// Recieves an update of the active snapshot.
        /// </summary>
        /// <param name="snapshot">The active snapshot.</param>
        public void Update(Snapshot snapshot)
        {
            this.ResultCount = snapshot == null ? 0 : snapshot.ElementCount;
            this.ByteCount = snapshot == null ? 0 : snapshot.ByteCount;
            this.CurrentPage = 0;
        }

        /// <summary>
        /// Runs the update loop, updating all scan results.
        /// </summary>
        public void Update()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    IList<ScanResult> scanResults = this.Addresses?.ToList();

                    if (scanResults != null)
                    {
                        foreach (ScanResult result in scanResults)
                        {
                            result?.PointerItem?.ProjectItem.Update();
                        }
                    }

                    Thread.Sleep(50);
                }
            });
        }

        /// <summary>
        /// Promts the user to edit the value of the specified result.
        /// </summary>
        private void EditValue(ScanResult scanResult)
        {
            ValueEditorViewModel.GetInstance().ShowDialog(scanResult?.PointerItem?.ProjectItem as PointerItem);
        }

        /// <summary>
        /// Loads the results for the current page.
        /// </summary>
        private void LoadScanResults()
        {
            Snapshot snapshot = SnapshotManager.GetSnapshot(Snapshot.SnapshotRetrievalMode.FromActiveSnapshot, this.ActiveType);
            IList<ScanResult> newAddresses = new List<ScanResult>();

            if (snapshot != null)
            {
                UInt64 startIndex = Math.Min(ScanResultsViewModel.PageSize * this.CurrentPage, snapshot.ElementCount);
                UInt64 endIndex = Math.Min((ScanResultsViewModel.PageSize * this.CurrentPage) + ScanResultsViewModel.PageSize, snapshot.ElementCount);

                for (UInt64 index = startIndex; index < endIndex; index++)
                {
                    SnapshotElementIndexer element = snapshot[index];

                    String label = element.GetElementLabel() != null ? element.GetElementLabel().ToString() : String.Empty;
                    Object currentValue = element.HasCurrentValue() ? element.LoadCurrentValue() : null;
                    Object previousValue = element.HasPreviousValue() ? element.LoadPreviousValue() : null;

                    String moduleName = String.Empty;
                    UInt64 address = Query.Default.AddressToModule(element.BaseAddress, out moduleName);

                    PointerItem pointerItem = new PointerItem(baseAddress: address, dataType: this.ActiveType, moduleName: moduleName, value: currentValue);
                    newAddresses.Add(new ScanResult(new PointerItemView(pointerItem), previousValue, label));
                }
            }

            this.Addresses = new FullyObservableCollection<ScanResult>(newAddresses);

            // Ensure results are visible
            this.IsVisible = true;
            this.IsSelected = true;
            this.IsActive = true;
        }

        /// <summary>
        /// Changes the active scan results type.
        /// </summary>
        /// <param name="newType">The new scan results type.</param>
        private void ChangeType(DataType newType)
        {
            this.ActiveType = newType;
        }

        /// <summary>
        /// Goes to the first page of results.
        /// </summary>
        private void FirstPage()
        {
            this.CurrentPage = 0;
        }

        /// <summary>
        /// Goes to the last page of results.
        /// </summary>
        private void LastPage()
        {
            this.CurrentPage = this.PageCount;
        }

        /// <summary>
        /// Goes to the previous page of results.
        /// </summary>
        private void PreviousPage()
        {
            this.CurrentPage = (this.CurrentPage - 1).Clamp(0UL, this.PageCount);
        }

        /// <summary>
        /// Goes to the next page of results.
        /// </summary>
        private void NextPage()
        {
            this.CurrentPage = (this.CurrentPage + 1).Clamp(0UL, this.PageCount);
        }

        /// <summary>
        /// Adds the given scan result to the project explorer.
        /// </summary>
        /// <param name="scanResult">The scan result to add to the project explorer.</param>
        private void AddScanResult(ScanResult scanResult)
        {
            ProjectExplorerViewModel.GetInstance().AddProjectItems(scanResult?.PointerItem?.ProjectItem);
        }

        /// <summary>
        /// Adds the given scan results to the project explorer.
        /// </summary>
        /// <param name="scanResults">The scan results to add to the project explorer.</param>
        private void AddScanResults(IEnumerable<ScanResult> scanResults)
        {
            if (scanResults == null)
            {
                return;
            }

            IEnumerable<PointerItem> projectItems = scanResults.Select(scanResult => scanResult.PointerItem?.ProjectItem as PointerItem);

            ProjectExplorerViewModel.GetInstance().AddProjectItems(projectItems.ToArray());
        }

        /// <summary>
        /// Notify all observing objects of an active type change.
        /// </summary>
        private void NotifyObservers()
        {
            lock (this.ObserverLock)
            {
                foreach (IResultDataTypeObserver observer in this.ScanResultsObservers)
                {
                    observer.Update(this.ActiveType);
                }
            }
        }
    }
    //// End class
}
//// End namespace