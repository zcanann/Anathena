namespace Squalr.Source.Scanning
{
    using GalaSoft.MvvmLight.Command;
    using Squalr.Engine;
    using Squalr.Engine.DataTypes;
    using Squalr.Engine.Logging;
    using Squalr.Engine.Scanning.Scanners;
    using Squalr.Engine.Scanning.Scanners.Constraints;
    using Squalr.Engine.Scanning.Snapshots;
    using Squalr.Source.Docking;
    using Squalr.Source.Results;
    using Squalr.Source.Tasks;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// View model for the Manual Scanner.
    /// </summary>
    internal class ManualScannerViewModel : ToolViewModel, IResultDataTypeObserver
    {
        /// <summary>
        /// Singleton instance of the <see cref="ManualScannerViewModel" /> class.
        /// </summary>
        private static Lazy<ManualScannerViewModel> manualScannerViewModelInstance = new Lazy<ManualScannerViewModel>(
                () => { return new ManualScannerViewModel(); },
                LazyThreadSafetyMode.ExecutionAndPublication);

        private ScanConstraint activeConstraint;

        /// <summary>
        /// Prevents a default instance of the <see cref="ManualScannerViewModel" /> class from being created.
        /// </summary>
        private ManualScannerViewModel() : base("Manual Scanner")
        {
            this.StartScanCommand = new RelayCommand(() => this.StartScan(), () => true);

            // Note: Not async to avoid updates slower than the perception threshold
            this.UpdateActiveValueCommand = new RelayCommand<Object>((newValue) => this.UpdateActiveValue(newValue), (newValue) => true);
            this.SelectChangedCommand = new RelayCommand(() => this.ChangeScanConstraintSelection(ScanConstraint.ConstraintType.Changed), () => true);
            this.SelectDecreasedCommand = new RelayCommand(() => this.ChangeScanConstraintSelection(ScanConstraint.ConstraintType.Decreased), () => true);
            this.SelectDecreasedByXCommand = new RelayCommand(() => this.ChangeScanConstraintSelection(ScanConstraint.ConstraintType.DecreasedByX), () => true);
            this.SelectEqualCommand = new RelayCommand(() => this.ChangeScanConstraintSelection(ScanConstraint.ConstraintType.Equal), () => true);
            this.SelectGreaterThanCommand = new RelayCommand(() => this.ChangeScanConstraintSelection(ScanConstraint.ConstraintType.GreaterThan), () => true);
            this.SelectGreaterThanOrEqualCommand = new RelayCommand(() => this.ChangeScanConstraintSelection(ScanConstraint.ConstraintType.GreaterThanOrEqual), () => true);
            this.SelectIncreasedCommand = new RelayCommand(() => this.ChangeScanConstraintSelection(ScanConstraint.ConstraintType.Increased), () => true);
            this.SelectIncreasedByXCommand = new RelayCommand(() => this.ChangeScanConstraintSelection(ScanConstraint.ConstraintType.IncreasedByX), () => true);
            this.SelectLessThanCommand = new RelayCommand(() => this.ChangeScanConstraintSelection(ScanConstraint.ConstraintType.LessThan), () => true);
            this.SelectLessThanOrEqualCommand = new RelayCommand(() => this.ChangeScanConstraintSelection(ScanConstraint.ConstraintType.LessThanOrEqual), () => true);
            this.SelectNotEqualCommand = new RelayCommand(() => this.ChangeScanConstraintSelection(ScanConstraint.ConstraintType.NotEqual), () => true);
            this.SelectUnchangedCommand = new RelayCommand(() => this.ChangeScanConstraintSelection(ScanConstraint.ConstraintType.Unchanged), () => true);

            // Note: Constraint modifying commands cannot be async since they modify the observable collection, which must be done on the same thread as the GUI
            this.AddCurrentConstraintCommand = new RelayCommand(() => this.AddCurrentConstraint(), () => true);
            this.RemoveConstraintCommand = new RelayCommand<ScanConstraint>((ScanConstraint) => this.RemoveConstraint(ScanConstraint), (ScanConstraint) => true);
            this.EditConstraintCommand = new RelayCommand<ScanConstraint>((ScanConstraint) => this.EditConstraint(ScanConstraint), (ScanConstraint) => true);
            this.ClearConstraintsCommand = new RelayCommand(() => this.ClearConstraints(), () => true);
            this.ActiveConstraint = new ScanConstraint(ScanConstraint.ConstraintType.Equal);

            Task.Run(() => ScanResultsViewModel.GetInstance().Subscribe(this));
            DockingViewModel.GetInstance().RegisterViewModel(this);
        }

        /// <summary>
        /// Gets the command begin the scan.
        /// </summary>
        public ICommand StartScanCommand { get; private set; }

        /// <summary>
        /// Gets the command to update the value of the active scan constraint.
        /// </summary>
        public ICommand UpdateActiveValueCommand { get; private set; }

        /// <summary>
        /// Gets the command to add the current constraint to the list of scan constraints.
        /// </summary>
        public ICommand AddCurrentConstraintCommand { get; private set; }

        /// <summary>
        /// Gets the command to remove the target constraint to the list of scan constraints.
        /// </summary>
        public ICommand RemoveConstraintCommand { get; private set; }

        /// <summary>
        /// Gets the command to edit the target constraint.
        /// </summary>
        public ICommand EditConstraintCommand { get; private set; }

        /// <summary>
        /// Gets the command to clear all added constraints.
        /// </summary>
        public ICommand ClearConstraintsCommand { get; private set; }

        /// <summary>
        /// Gets the command to select the <see cref="ScanConstraint.ConstraintType.Changed"/> constraint.
        /// </summary>
        public ICommand SelectChangedCommand { get; private set; }

        /// <summary>
        /// Gets the command to select the <see cref="ScanConstraint.ConstraintType.Decreased"/> constraint.
        /// </summary>
        public ICommand SelectDecreasedCommand { get; private set; }

        /// <summary>
        /// Gets the command to select the <see cref="ScanConstraint.ConstraintType.DecreasedByX"/> constraint.
        /// </summary>
        public ICommand SelectDecreasedByXCommand { get; private set; }

        /// <summary>
        /// Gets the command to select the <see cref="ScanConstraint.ConstraintType.Equal"/> constraint.
        /// </summary>
        public ICommand SelectEqualCommand { get; private set; }

        /// <summary>
        /// Gets the command to select the <see cref="ScanConstraint.ConstraintType.GreaterThan"/> constraint.
        /// </summary>
        public ICommand SelectGreaterThanCommand { get; private set; }

        /// <summary>
        /// Gets the command to select the <see cref="ScanConstraint.ConstraintType.GreaterThanOrEqual"/> constraint.
        /// </summary>
        public ICommand SelectGreaterThanOrEqualCommand { get; private set; }

        /// <summary>
        /// Gets the command to select the <see cref="ScanConstraint.ConstraintType.Increased"/> constraint.
        /// </summary>
        public ICommand SelectIncreasedCommand { get; private set; }

        /// <summary>
        /// Gets the command to select the <see cref="ScanConstraint.ConstraintType.IncreasedByX"/> constraint.
        /// </summary>
        public ICommand SelectIncreasedByXCommand { get; private set; }

        /// <summary>
        /// Gets the command to select the <see cref="ScanConstraint.ConstraintType.LessThan"/> constraint.
        /// </summary>
        public ICommand SelectLessThanCommand { get; private set; }

        /// <summary>
        /// Gets the command to select the <see cref="ScanConstraint.ConstraintType.LessThanOrEqual"/> constraint.
        /// </summary>
        public ICommand SelectLessThanOrEqualCommand { get; private set; }

        /// <summary>
        /// Gets the command to select the <see cref="ScanConstraint.ConstraintType.NotEqual"/> constraint.
        /// </summary>
        public ICommand SelectNotEqualCommand { get; private set; }

        /// <summary>
        /// Gets the command to select the <see cref="ScanConstraint.ConstraintType.Unchanged"/> constraint.
        /// </summary>
        public ICommand SelectUnchangedCommand { get; private set; }

        /// <summary>
        /// Gets the current set of scan constraints added to the manager.
        /// </summary>
        public ScanConstraint ActiveConstraint
        {
            get
            {
                return this.activeConstraint;
            }

            set
            {
                this.activeConstraint = value;
                this.UpdateAllProperties();
            }
        }

        /// <summary>
        /// Gets a value indicating if the current scan constraint requires a value.
        /// </summary>
        public Boolean IsActiveScanConstraintValued
        {
            get
            {
                return ActiveConstraint == null ? true : ScanConstraint.IsValuedConstraint(this.ActiveConstraint.Constraint);
            }
        }

        /// <summary>
        /// Gets a singleton instance of the <see cref="ManualScannerViewModel"/> class.
        /// </summary>
        /// <returns>A singleton instance of the class.</returns>
        public static ManualScannerViewModel GetInstance()
        {
            return ManualScannerViewModel.manualScannerViewModelInstance.Value;
        }

        /// <summary>
        /// Updates the active type.
        /// </summary>
        /// <param name="activeType">The new active type.</param>
        public void Update(DataType activeType)
        {
            // Create a temporary manager to update our current constraint
            ConstraintNode scanConstraintCollection = new ConstraintNode();
            //   scanConstraintCollection.AddConstraint(this.CurrentScanConstraint);
            scanConstraintCollection.SetElementType(activeType);

            this.ActiveConstraint.SetElementType(activeType);
            this.UpdateAllProperties();
        }

        /// <summary>
        /// Starts the scan using the current constraints.
        /// </summary>
        private void StartScan()
        {
            // Create a constraint manager that includes the current active constraint
            ConstraintNode scanConstraints = this.ActiveConstraint.Clone();

            if (!scanConstraints.IsValid())
            {
                Logger.Log(LogLevel.Warn, "Unable to start scan with given constraints");
                return;
            }

            DataType dataType = ScanResultsViewModel.GetInstance().ActiveType;

            try
            {
                // Collect values
                TrackableTask<Snapshot> valueCollectorTask = ValueCollector.CollectValues(
                    SnapshotManager.GetSnapshot(Snapshot.SnapshotRetrievalMode.FromActiveSnapshotOrPrefilter, dataType),
                    TrackableTask.UniversalIdentifier);

                TaskTrackerViewModel.GetInstance().TrackTask(valueCollectorTask);

                // Perform manual scan on value collection complete
                valueCollectorTask.OnCompletedEvent += ((completedValueCollectionTask) =>
                {
                    Snapshot snapshot = valueCollectorTask.Result;
                    TrackableTask<Snapshot> scanTask = ManualScanner.Scan(
                        snapshot,
                        scanConstraints,
                        TrackableTask.UniversalIdentifier);

                    TaskTrackerViewModel.GetInstance().TrackTask(scanTask);
                    SnapshotManager.SaveSnapshot(scanTask.Result);
                });
            }
            catch (TaskConflictException)
            {
            }
        }

        /// <summary>
        /// Adds the current constraint to the list of scan constraints.
        /// </summary>
        private void AddCurrentConstraint()
        {
            this.ActiveConstraint = new ScanConstraint(this.ActiveConstraint.Constraint, this.ActiveConstraint.ConstraintValue);
            this.UpdateAllProperties();
        }

        /// <summary>
        /// Updates the value of the current scan constraint.
        /// </summary>
        /// <param name="newValue">The new value of the scan constraint.</param>
        private void UpdateActiveValue(Object newValue)
        {
            this.ActiveConstraint.ConstraintValue = newValue;
            this.UpdateAllProperties();
        }

        /// <summary>
        /// Edits the target constraint by removing it and making it the current constraint.
        /// </summary>
        /// <param name="scanConstraint">The constraint to edit.</param>
        private void EditConstraint(ScanConstraint scanConstraint)
        {
            this.ActiveConstraint = scanConstraint;
            this.UpdateAllProperties();
        }

        /// <summary>
        /// Removes the target constraint from the list of scan constraints.
        /// </summary>
        /// <param name="scanConstraint">The constraint to remove.</param>
        private void RemoveConstraint(ScanConstraint scanConstraint)
        {
            this.UpdateAllProperties();
        }

        /// <summary>
        /// Clears all scan constraints.
        /// </summary>
        private void ClearConstraints()
        {
            this.UpdateAllProperties();
        }

        /// <summary>
        /// Changes the current scan constraint.
        /// </summary>
        /// <param name="constraint">The new scan constraint.</param>
        private void ChangeScanConstraintSelection(ScanConstraint.ConstraintType constraint)
        {
            this.ActiveConstraint.Constraint = constraint;
            this.UpdateAllProperties();
        }

        /// <summary>
        /// Raises property changed events for all used properties. This is convenient since there are several interdependencies between these.
        /// </summary>
        private void UpdateAllProperties()
        {
            this.RaisePropertyChanged(nameof(this.ActiveConstraint));
            this.RaisePropertyChanged(nameof(this.IsActiveScanConstraintValued));
        }
    }
    //// End class
}
//// End namespace