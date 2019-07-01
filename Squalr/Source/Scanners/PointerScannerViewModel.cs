namespace Squalr.Source.Scanning
{
    using GalaSoft.MvvmLight.Command;
    using Squalr.Engine;
    using Squalr.Engine.Common;
    using Squalr.Engine.Scanning.Scanners.Pointers;
    using Squalr.Engine.Scanning.Scanners.Pointers.Structures;
    using Squalr.Source.Docking;
    using Squalr.Source.Results;
    using Squalr.Source.Tasks;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// View model for the Input Correlator.
    /// </summary>
    internal class PointerScannerViewModel : ToolViewModel
    {
        /// <summary>
        /// An identifier to ensure only one pointer scan runs at a time.
        /// </summary>
        public static readonly String PointerScanTaskIdentifier = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets the default pointer scan depth.
        /// </summary>
        public const Int32 DefaultPointerScanDepth = 3;

        /// <summary>
        /// Gets the default pointer scan radius.
        /// </summary>
        public const Int32 DefaultPointerScanRadius = 2048;

        /// <summary>
        /// Gets the maximum pointer scan depth.
        /// </summary>
        public const Int32 MaximumPointerScanDepth = 25;

        private UInt64 retargetAddress;

        private UInt64 targetAddress;

        private Int32 pointerRadius;

        private Object rescanValue;

        private Int32 pointerDepth;

        /// <summary>
        /// Singleton instance of the <see cref="PointerScannerViewModel" /> class.
        /// </summary>
        private static Lazy<PointerScannerViewModel> pointerScannerViewModelInstance = new Lazy<PointerScannerViewModel>(
                () => { return new PointerScannerViewModel(); },
                LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Prevents a default instance of the <see cref="PointerScannerViewModel" /> class from being created.
        /// </summary>
        private PointerScannerViewModel() : base("Pointer Scanner")
        {
            this.StartPointerRetargetScanCommand = new RelayCommand<UInt64>((newValue) => this.StartPointerRetargetScan(), (newValue) => true);
            this.StartPointerRebaseCommand = new RelayCommand(() => Task.Run(() => this.StartPointerRebase()), () => true);
            this.StartScanCommand = new RelayCommand(() => Task.Run(() => this.StartScan()), () => true);

            this.SetPointerScanAddressCommand = new RelayCommand<UInt64>((newValue) => this.TargetAddress = newValue, (newValue) => true);
            this.SetPointerRetargetScanAddressCommand = new RelayCommand<UInt64>((newValue) => this.RetargetAddress = newValue, (newValue) => true);
            this.SetDepthCommand = new RelayCommand<Int32>((newValue) => this.PointerDepth = newValue, (newValue) => true);
            this.SetPointerRadiusCommand = new RelayCommand<Int32>((newValue) => this.PointerRadius = newValue, (newValue) => true);

            DockingViewModel.GetInstance().RegisterViewModel(this);
        }

        /// <summary>
        /// Gets a command to start the pointer scan on a specific project item.
        /// </summary>
        public ICommand PointerScanCommand { get; private set; }

        /// <summary>
        /// Gets a command to start the pointer scan.
        /// </summary>
        public ICommand StartScanCommand { get; private set; }

        /// <summary>
        /// Gets a command to start the pointer rescan.
        /// </summary>
        public ICommand StartPointerRebaseCommand { get; private set; }

        /// <summary>
        /// Gets a command to start the pointer rescan.
        /// </summary>
        public ICommand StartPointerRetargetScanCommand { get; private set; }

        /// <summary>
        /// Gets a command to set the pointer scan address.
        /// </summary>
        public ICommand SetPointerScanAddressCommand { get; private set; }

        /// <summary>
        /// Gets a command to set the pointer rescan address.
        /// </summary>
        public ICommand SetPointerRetargetScanAddressCommand { get; private set; }

        /// <summary>
        /// Gets a command to set the scan depth.
        /// </summary>
        public ICommand SetDepthCommand { get; private set; }

        /// <summary>
        /// Gets a command to set the pointer radius for the scan.
        /// </summary>
        public ICommand SetPointerRadiusCommand { get; private set; }

        /// <summary>
        /// Gets or sets the target scan address.
        /// </summary>
        public UInt64 TargetAddress
        {
            get
            {
                return this.targetAddress;
            }

            set
            {
                this.targetAddress = value;
                this.RaisePropertyChanged(nameof(this.TargetAddress));
            }
        }

        /// <summary>
        /// Gets or sets the retarget scan address.
        /// </summary>
        public UInt64 RetargetAddress
        {
            get
            {
                return this.retargetAddress;
            }

            set
            {
                this.retargetAddress = value;
                this.RaisePropertyChanged(nameof(this.RetargetAddress));
            }
        }

        /// <summary>
        /// Gets or sets the target rescan value.
        /// </summary>
        public Object RescanValue
        {
            get
            {
                return this.rescanValue;
            }

            set
            {
                this.rescanValue = value;
                this.RaisePropertyChanged(nameof(this.RescanValue));
            }
        }

        /// <summary>
        /// Gets or sets the pointer depth of the scan.
        /// </summary>
        public Int32 PointerDepth
        {
            get
            {
                return this.pointerDepth;
            }

            set
            {
                this.pointerDepth = value;
                this.RaisePropertyChanged(nameof(this.PointerDepth));
            }
        }

        /// <summary>
        /// Gets or sets the pointer radius of the scan.
        /// </summary>
        public Int32 PointerRadius
        {
            get
            {
                return this.pointerRadius;
            }

            set
            {
                this.pointerRadius = value;
                this.RaisePropertyChanged(nameof(this.PointerRadius));
            }
        }

        /// <summary>
        /// Gets a singleton instance of the <see cref="ChangeCounterViewModel"/> class.
        /// </summary>
        /// <returns>A singleton instance of the class.</returns>
        public static PointerScannerViewModel GetInstance()
        {
            return pointerScannerViewModelInstance.Value;
        }

        /// <summary>
        /// Starts the pointer scan.
        /// </summary>
        private void StartScan()
        {
            try
            {
                throw new NotImplementedException();
                TrackableTask<PointerBag> pointerScanTask = null;// PointerScan.Scan(this.TargetAddress, (UInt32)this.PointerRadius, this.PointerDepth, 4, PointerScannerViewModel.PointerScanTaskIdentifier);
                TaskTrackerViewModel.GetInstance().TrackTask(pointerScanTask);
                PointerScanResultsViewModel.GetInstance().DiscoveredPointers = pointerScanTask.Result;
            }
            catch (TaskConflictException)
            {
            }
        }

        /// <summary>
        /// Starts the pointer address rebase.
        /// </summary>
        private void StartPointerRebase()
        {
            try
            {
                TrackableTask<PointerBag> pointerRebaseTask = PointerRebase.Scan(PointerScanResultsViewModel.GetInstance().DiscoveredPointers, readMemory: true, performUnchangedScan: true, taskIdentifier: PointerScannerViewModel.PointerScanTaskIdentifier);
                TaskTrackerViewModel.GetInstance().TrackTask(pointerRebaseTask);
                PointerScanResultsViewModel.GetInstance().DiscoveredPointers = pointerRebaseTask.Result;
            }
            catch (TaskConflictException)
            {
            }
        }

        /// <summary>
        /// Starts the pointer value rescan.
        /// </summary>
        private void StartPointerRetargetScan()
        {
            try
            {
                throw new NotImplementedException();
                TrackableTask<PointerBag> pointerRetargetScanTask = null; // PointerRetargetScan.Scan(this.RetargetAddress, 4, PointerScanResultsViewModel.GetInstance().DiscoveredPointers, PointerScannerViewModel.PointerScanTaskIdentifier);
                TaskTrackerViewModel.GetInstance().TrackTask(pointerRetargetScanTask);
                PointerScanResultsViewModel.GetInstance().DiscoveredPointers = pointerRetargetScanTask.Result;
            }
            catch (TaskConflictException)
            {
            }
        }
    }
    //// End class
}
//// End namespace