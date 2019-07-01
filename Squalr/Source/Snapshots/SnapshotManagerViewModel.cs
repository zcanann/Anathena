namespace Squalr.Source.Snapshots
{
    using GalaSoft.MvvmLight.Command;
    using Squalr.Engine.Scanning.Snapshots;
    using Squalr.Source.Docking;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Windows.Input;

    /// <summary>
    /// View model for the Snapshot Manager.
    /// </summary>
    internal class SnapshotManagerViewModel : ToolViewModel
    {
        /// <summary>
        /// Singleton instance of the <see cref="SnapshotManagerViewModel"/> class.
        /// </summary>
        private static Lazy<SnapshotManagerViewModel> snapshotManagerViewModelInstance = new Lazy<SnapshotManagerViewModel>(
                () => { return new SnapshotManagerViewModel(); },
                LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Prevents a default instance of the <see cref="SnapshotManagerViewModel"/> class from being created.
        /// </summary>
        private SnapshotManagerViewModel() : base("Snapshot Manager")
        {
            // Note: Not async to avoid updates slower than the perception threshold
            this.ClearSnapshotsCommand = new RelayCommand(() => throw new NotImplementedException() /*SnapshotManager.ClearSnapshots()*/, () => true);
            this.UndoSnapshotCommand = new RelayCommand(() => throw new NotImplementedException() /*SnapshotManager.UndoSnapshot()*/, () => true);
            this.RedoSnapshotCommand = new RelayCommand(() => throw new NotImplementedException() /*SnapshotManager.RedoSnapshot()*/, () => true);

            DockingViewModel.GetInstance().RegisterViewModel(this);
        }

        /// <summary>
        /// Gets a command to start a new scan.
        /// </summary>
        public ICommand ClearSnapshotsCommand { get; private set; }

        /// <summary>
        /// Gets a command to undo the last scan.
        /// </summary>
        public ICommand UndoSnapshotCommand { get; private set; }

        /// <summary>
        /// Gets a command to redo the last scan.
        /// </summary>
        public ICommand RedoSnapshotCommand { get; private set; }

        /// <summary>
        /// Gets the enumeration of snapshots in the snapshot manager.
        /// </summary>
        public IEnumerable<Snapshot> Snapshots
        {
            get
            {
                throw new NotImplementedException();
                //return SnapshotManager.Snapshots;
            }
        }

        /// <summary>
        /// Gets the enumeration of snapshots in the snapshot manager.
        /// </summary>
        public IEnumerable<Snapshot> DeletedSnapshots
        {
            get
            {
                throw new NotImplementedException();
                //// return SnapshotManager.DeletedSnapshots;
            }
        }

        /// <summary>
        /// Gets a singleton instance of the <see cref="SnapshotManagerViewModel"/> class.
        /// </summary>
        /// <returns>A singleton instance of the class.</returns>
        public static SnapshotManagerViewModel GetInstance()
        {
            return snapshotManagerViewModelInstance.Value;
        }

        /// <summary>
        /// Recieves an update of the active snapshot.
        /// </summary>
        /// <param name="snapshot">The active snapshot.</param>
        public void Update(Snapshot snapshot)
        {
            this.RaisePropertyChanged(nameof(this.Snapshots));
            this.RaisePropertyChanged(nameof(this.DeletedSnapshots));
        }
    }
    //// End class
}
//// End namespace