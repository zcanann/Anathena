namespace Squalr.Engine.Scanning.Scanners
{
    using Squalr.Engine.Common;
    using Squalr.Engine.Common.Extensions;
    using Squalr.Engine.Common.Logging;
    using Squalr.Engine.Scanning.Scanners.Constraints;
    using Squalr.Engine.Scanning.Snapshots;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using static Squalr.Engine.Common.TrackableTask;

    /// <summary>
    /// A memory scanning class for classic manual memory scanning techniques.
    /// </summary>
    public static class ManualScanner
    {
        /// <summary>
        /// The name of this scan.
        /// </summary>
        private const String Name = "Manual Scanner";

        /// <summary>
        /// Begins the manual scan based on the provided snapshot and parameters.
        /// </summary>
        /// <param name="snapshot">The snapshot on which to perfrom the scan.</param>
        /// <param name="constraints">The collection of scan constraints to use in the manual scan.</param>
        /// <param name="taskIdentifier">The unique identifier to prevent duplicate tasks.</param>
        /// <returns></returns>
        public static TrackableTask<Snapshot> Scan(Snapshot snapshot, ConstraintNode constraints, String taskIdentifier = null)
        {
            try
            {
                return TrackableTask<Snapshot>
                    .Create(ManualScanner.Name, taskIdentifier, out UpdateProgress updateProgress, out CancellationToken cancellationToken)
                    .With(Task<Snapshot>.Run(() =>
                    {
                        Snapshot result = null;

                        try
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            Stopwatch stopwatch = new Stopwatch();
                            stopwatch.Start();

                            Int32 processedPages = 0;
                            ConcurrentBag<IList<SnapshotRegion>> regions = new ConcurrentBag<IList<SnapshotRegion>>();

                            ParallelOptions options = ParallelSettings.ParallelSettingsFastest.Clone();
                            options.CancellationToken = cancellationToken;

                            Parallel.ForEach(
                                snapshot.OptimizedSnapshotRegions,
                                options,
                                (region) =>
                                {
                                    // Check for canceled scan
                                    cancellationToken.ThrowIfCancellationRequested();

                                    if (!region.ReadGroup.CanCompare(constraints.HasRelativeConstraint()))
                                    {
                                        return;
                                    }

                                    SnapshotElementVectorComparer vectorComparer = new SnapshotElementVectorComparer(region: region, constraints: constraints);
                                    IList<SnapshotRegion> results = vectorComparer.Compare();

                                    if (!results.IsNullOrEmpty())
                                    {
                                        regions.Add(results);
                                    }

                                    // Update progress every N regions
                                    if (Interlocked.Increment(ref processedPages) % 32 == 0)
                                    {
                                        updateProgress((float)processedPages / (float)snapshot.RegionCount * 100.0f);
                                    }
                                });
                            //// End foreach Region

                            // Exit if canceled
                            cancellationToken.ThrowIfCancellationRequested();

                            result = new Snapshot(ManualScanner.Name, regions.SelectMany(region => region));
                            stopwatch.Stop();
                            Logger.Log(LogLevel.Info, "Scan complete in: " + stopwatch.Elapsed);
                        }
                        catch (OperationCanceledException ex)
                        {
                            Logger.Log(LogLevel.Warn, "Scan canceled", ex);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(LogLevel.Error, "Error performing scan", ex);
                        }

                        return result;
                    }, cancellationToken));
            }
            catch (TaskConflictException ex)
            {
                Logger.Log(LogLevel.Warn, "Unable to start scan. Scan is already queued.");
                throw ex;
            }
        }
    }
    //// End class
}
//// End namespace