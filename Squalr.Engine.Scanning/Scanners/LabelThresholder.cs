namespace Squalr.Engine.Scanning.Scanners
{
    using Squalr.Engine.Common;
    using Squalr.Engine.Scanning.Snapshots;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class LabelThresholder : ISnapshotObserver
    {
        private Double upperThreshold;

        private Double lowerThreshold;

        public LabelThresholder(Action onUpdateHistogram)
        {
            this.ItemLock = new Object();
            this.SnapshotLock = new Object();
            this.ProgressLock = new Object();
            this.OnUpdateHistogram = onUpdateHistogram;
            Task.Run(() => SnapshotManager.Subscribe(this));
        }

        public Double LowerThreshold
        {
            get
            {
                return this.lowerThreshold;
            }

            set
            {
                this.lowerThreshold = value;
                this.UpdateHistogram();
            }
        }

        public Double UpperThreshold
        {
            get
            {
                return this.upperThreshold;
            }

            set
            {
                this.upperThreshold = value;
                this.UpdateHistogram();
            }
        }

        public SortedList<Object, Int64> Histogram { get; set; }

        public SortedList<Object, Int64> HistogramFiltered { get; set; }

        public SortedList<Object, Int64> HistogramKept { get; set; }

        private Int32 LowerIndex { get; set; }

        private Int32 UpperIndex { get; set; }

        private Action OnUpdateHistogram { get; set; }

        private Snapshot Snapshot { get; set; }

        private Boolean Inverted { get; set; }

        private Object ItemLock { get; set; }

        private Object SnapshotLock { get; set; }

        private Object ProgressLock { get; set; }

        public void ToggleInverted()
        {
            this.Inverted = !this.Inverted;
            this.UpdateHistogram(forceUpdate: true);
        }

        /// <summary>
        /// Recieves an update of the active snapshot.
        /// </summary>
        /// <param name="snapshot">The active snapshot.</param>
        public void Update(Snapshot snapshot)
        {
            lock (this.SnapshotLock)
            {
                this.Snapshot = snapshot;

                if (this.Snapshot == null)
                {
                    return;
                }
            }

            //// this.Start();
        }

        public void ApplyThreshold()
        {
            lock (this.SnapshotLock)
            {
                if (this.Snapshot == null)
                {
                    return;
                }
            }

            dynamic lowerValue = this.Histogram.Keys[this.LowerIndex];
            dynamic upperValue = this.Histogram.Keys[this.UpperIndex];

            lock (this.SnapshotLock)
            {
                if (!this.Inverted)
                {
                    //// this.Snapshot.SetAllValidBits(false);

                    foreach (SnapshotRegion region in this.Snapshot.SnapshotRegions)
                    {
                        for (IEnumerator<SnapshotElementComparer> enumerator = region.IterateComparer(SnapshotElementComparer.PointerIncrementMode.ValuesOnly, null); enumerator.MoveNext();)
                        {
                            SnapshotElementComparer element = enumerator.Current;

                            dynamic label = element.GetElementLabel();

                            if (label >= lowerValue && label <= upperValue)
                            {
                                //// element.SetValid(true);
                            }
                        }
                    }
                }
                else
                {
                    //// this.Snapshot.SetAllValidBits(true);

                    foreach (SnapshotRegion region in this.Snapshot.SnapshotRegions)
                    {
                        for (IEnumerator<SnapshotElementComparer> enumerator = region.IterateComparer(SnapshotElementComparer.PointerIncrementMode.LabelsOnly, null); enumerator.MoveNext();)
                        {
                            SnapshotElementComparer element = enumerator.Current;

                            dynamic label = element.GetElementLabel();

                            if (label >= lowerValue && label <= upperValue)
                            {
                                //// element.SetValid(false);
                            }
                        }
                    }
                }

                //// this.Snapshot.DiscardInvalidRegions();
            }

            SnapshotManager.SaveSnapshot(this.Snapshot);
            this.UpdateHistogram(forceUpdate: true);
        }

        protected void OnBegin()
        {
        }

        /// <summary>
        /// Called when the scan updates.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for handling canceled tasks.</param>
        protected void OnUpdate(CancellationToken cancellationToken)
        {
            ConcurrentDictionary<Object, Int64> histogram = new ConcurrentDictionary<Object, Int64>();
            Int32 processedPages = 0;

            lock (this.SnapshotLock)
            {
                if (this.Snapshot == null)
                {
                    //// this.Cancel();
                    return;
                }

                Parallel.ForEach(
                    this.Snapshot.SnapshotRegions,
                    ParallelSettings.ParallelSettingsFast,
                    (region) =>
                {
                    if (region.ReadGroup.ElementLabels == null || region.ElementCount <= 0)
                    {
                        return;
                    }

                    for (IEnumerator<SnapshotElementComparer> enumerator = region.IterateComparer(SnapshotElementComparer.PointerIncrementMode.LabelsOnly, null); enumerator.MoveNext();)
                    {
                        SnapshotElementComparer element = enumerator.Current;

                        lock (this.ItemLock)
                        {

                            Object label = element.GetElementLabel();

                            if (histogram.ContainsKey(label))
                            {
                                histogram[label]++;
                            }
                            else
                            {
                                histogram.TryAdd(label, 1);
                            }
                        }
                    }

                    lock (this.ProgressLock)
                    {
                        processedPages++;
                        //// this.UpdateProgress(processedPages, this.Snapshot.RegionCount, canFinalize: false);
                    }
                    //// End foreach element
                });
                //// End foreach region
            }

            this.Histogram = new SortedList<Object, Int64>(histogram);
            this.UpdateHistogram();
        }

        protected void OnEnd()
        {
        }

        private void UpdateHistogram(Boolean forceUpdate = false)
        {
            if (this.Histogram == null || this.Histogram.Count <= 0)
            {
                return;
            }

            Int32 lowerIndex = (Int32)Math.Round(((Double)this.LowerThreshold / 100.0) * (Double)(this.Histogram.Count - 1));
            Int32 upperIndex = (Int32)Math.Round(((Double)this.UpperThreshold / 100.0) * (Double)(this.Histogram.Count - 1));

            // Prevent updating the histograms again if we have already computed them
            if (!forceUpdate && lowerIndex == this.LowerIndex && upperIndex == this.UpperIndex)
            {
                return;
            }

            this.LowerIndex = lowerIndex;
            this.UpperIndex = upperIndex;

            SortedList<Object, Int64> histogramKept = new SortedList<Object, Int64>();
            SortedList<Object, Int64> histogramFiltered = new SortedList<Object, Int64>();

            foreach (KeyValuePair<Object, Int64> bar in this.Histogram)
            {
                Int32 index = this.Histogram.IndexOfKey(bar.Key);

                if (this.Inverted ^ (index >= lowerIndex && index <= upperIndex))
                {
                    // Keep items within bounds (unless inverted)
                    histogramKept.Add(bar.Key, bar.Value);
                    histogramFiltered.Add(bar.Key, 0);
                }
                else
                {
                    // Filter items outside bounds (unless inverted)
                    histogramKept.Add(bar.Key, 0);
                    histogramFiltered.Add(bar.Key, bar.Value);
                }
            }

            this.HistogramKept = histogramKept;
            this.HistogramFiltered = histogramFiltered;
            this.OnUpdateHistogram();
        }
    }
    //// End class
}
//// End namespace