namespace Squalr.Engine.Scanning.Scanners.Pointers
{
    using Squalr.Engine.Common;
    using Squalr.Engine.Common.Extensions;
    using Squalr.Engine.Common.Logging;
    using Squalr.Engine.Scanning.Scanners.Pointers.SearchKernels;
    using Squalr.Engine.Scanning.Snapshots;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using static Squalr.Engine.Common.TrackableTask;

    /// <summary>
    /// Validates a snapshot of pointers.
    /// </summary>
    internal static class PointerFilter
    {
        /// <summary>
        /// The name of this scan.
        /// </summary>
        private const String Name = "Pointer Filter";

        /// <summary>
        /// Filters the given snapshot to find all values that are valid pointers.
        /// </summary>
        /// <param name="snapshot">The snapshot on which to perfrom the scan.</param>
        /// <returns></returns>
        public static TrackableTask<Snapshot> Filter(TrackableTask parentTask, Snapshot snapshot, IVectorSearchKernel searchKernel, Snapshot DEBUG, UInt32 RADIUS_DEBUG)
        {
            return TrackableTask<Snapshot>
                .Create(PointerFilter.Name, out UpdateProgress updateProgress, out CancellationToken cancellationToken)
                .With(Task<Snapshot>.Run(() =>
                {
                    try
                    {
                        parentTask.CancellationToken.ThrowIfCancellationRequested();

                        ConcurrentBag<IList<SnapshotRegion>> regions = new ConcurrentBag<IList<SnapshotRegion>>();

                        ParallelOptions options = ParallelSettings.ParallelSettingsFastest.Clone();
                        options.CancellationToken = parentTask.CancellationToken;

                        // ISearchKernel DEBUG_KERNEL = new SpanSearchKernel(DEBUG, RADIUS_DEBUG);

                        Parallel.ForEach(
                            snapshot.OptimizedSnapshotRegions,
                            options,
                            (region) =>
                            {
                                // Check for canceled scan
                                parentTask.CancellationToken.ThrowIfCancellationRequested();

                                if (!region.ReadGroup.CanCompare(hasRelativeConstraint: false))
                                {
                                    return;
                                }

                                SnapshotElementVectorComparer vectorComparer = new SnapshotElementVectorComparer(region: region);
                                vectorComparer.SetCustomCompareAction(searchKernel.GetSearchKernel(vectorComparer));

                                // SnapshotElementVectorComparer DEBUG_COMPARER = new SnapshotElementVectorComparer(region: region);
                                // DEBUG_COMPARER.SetCustomCompareAction(DEBUG_KERNEL.GetSearchKernel(DEBUG_COMPARER));

                                IList<SnapshotRegion> results = vectorComparer.Compare();

                                // When debugging, these results should be the same as the results above
                                // IList<SnapshotRegion> DEBUG_RESULTS = vectorComparer.Compare();

                                if (!results.IsNullOrEmpty())
                                {
                                    regions.Add(results);
                                }
                            });

                        // Exit if canceled
                        parentTask.CancellationToken.ThrowIfCancellationRequested();

                        snapshot = new Snapshot(PointerFilter.Name, regions.SelectMany(region => region));
                    }
                    catch (OperationCanceledException ex)
                    {
                        Logger.Log(LogLevel.Warn, "Pointer filtering canceled", ex);
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogLevel.Error, "Error performing pointer filtering", ex);
                        return null;
                    }

                    return snapshot;
                }, parentTask.CancellationToken));
        }
    }
    //// End class
}
//// End namespace