namespace Squalr.Engine.Scanning.Scanners
{
    using Squalr.Engine.Common;
    using Squalr.Engine.Common.DataTypes;
    using Squalr.Engine.Scanning.Snapshots;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class ChangeCounter
    {
        /// <summary>
        /// The number of scans completed.
        /// </summary>
        private Int32 scanCount;

        public ChangeCounter(Action updateScanCount)
        {
            this.UpdateScanCount = updateScanCount;
            this.ProgressLock = new Object();
        }

        /// <summary>
        /// Gets the number of scans that have been executed.
        /// </summary>
        public Int32 ScanCount
        {
            get
            {
                return this.scanCount;
            }

            private set
            {
                this.scanCount = value;
                //// this.RaisePropertyChanged(nameof(this.ScanCount));
            }
        }

        /// <summary>
        /// Gets or sets the snapshot being labeled with change counts.
        /// </summary>
        private Snapshot Snapshot { get; set; }

        private UInt16 MinChanges { get; set; }

        private UInt16 MaxChanges { get; set; }

        private Object ProgressLock { get; set; }

        private Action UpdateScanCount { get; set; }

        public void SetMinChanges(UInt16 minChanges)
        {
            this.MinChanges = minChanges;
        }

        public void SetMaxChanges(UInt16 maxChanges)
        {
            this.MaxChanges = maxChanges;
        }

        public void Stop()
        {
            //// this.EndUpdateLoop();
        }

        protected void OnBegin()
        {
            // Initialize labeled snapshot
            this.Snapshot = SnapshotManager.GetSnapshot(Snapshot.SnapshotRetrievalMode.FromActiveSnapshotOrPrefilter, DataType.Int32).Clone("Change Counter");
            this.Snapshot.LabelDataType = DataType.UInt16;

            if (this.Snapshot == null)
            {
                return;
            }

            // Initialize change counts to zero
            this.Snapshot.SetElementLabels<UInt16>(0);

            this.ScanCount = 0;
        }

        /// <summary>
        /// Called when the scan updates.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for handling canceled tasks.</param>
        protected void OnUpdate(CancellationToken cancellationToken)
        {
            Int32 processedPages = 0;

            Snapshot snapshot = ValueCollector.CollectValues(this.Snapshot).Result;

            Parallel.ForEach(
                snapshot.OptimizedSnapshotRegions,
                ParallelSettings.ParallelSettingsFastest,
                (region) =>
                {
                    if (!region.ReadGroup.CanCompare(hasRelativeConstraint: true))
                    {
                        return;
                    }

                    for (IEnumerator<SnapshotElementComparer> enumerator = region.IterateComparer(SnapshotElementComparer.PointerIncrementMode.ValuesOnly, null); enumerator.MoveNext();)
                    {
                        SnapshotElementComparer element = enumerator.Current;

                        // Perform the comparison based on the current scan constraint
                        if (element.ElementCompare())
                        {
                            element.ElementLabel = (UInt16)((UInt16)element.ElementLabel + 1);
                        }
                    }

                    // Update progress
                    lock (this.ProgressLock)
                    {
                        processedPages++;
                        //// this.UpdateProgress(processedPages, snapshot.RegionCount, canFinalize: false);
                    }
                });

            this.ScanCount++;
            this.UpdateScanCount?.Invoke();
        }

        /// <summary>
        /// Called when the repeated task completes.
        /// </summary>
        protected void OnEnd()
        {
            SnapshotManager.SaveSnapshot(this.Snapshot);
            this.Snapshot = null;
        }
    }
    //// End class
}
//// End namespace