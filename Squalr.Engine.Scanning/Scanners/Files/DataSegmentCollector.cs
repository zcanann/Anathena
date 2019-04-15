namespace Squalr.Engine.Scanning.Scanners
{
    using Squalr.Engine.Common;
    using Squalr.Engine.Common.Logging;
    using Squalr.Engine.Memory;
    using Squalr.Engine.Scanning.Snapshots;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using static Squalr.Engine.Common.TrackableTask;

    /// <summary>
    /// Collect data segment values for a given snapshot.
    /// </summary>
    public static class DataSegmentCollector
    {
        /// <summary>
        /// The name of this scan.
        /// </summary>
        private const String Name = "Data Segment Collector";

        public static TrackableTask<Snapshot> CollectDataSegments(Boolean ignoreSystemModules = true)
        {
            return TrackableTask<Snapshot>
                .Create(DataSegmentCollector.Name, out UpdateProgress updateProgress, out CancellationToken cancellationToken)
                .With(Task<Snapshot>.Run(() =>
                {
                    try
                    {
                        Int32 processedModules = 0;
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();

                        // Construct a snapshot from static data segments in target process modules
                        IEnumerable<NormalizedModule> modules = Query.Default.GetModules();
                        DirectoryInfo systemPathInfo = new DirectoryInfo(Environment.SystemDirectory);
                        ConcurrentBag<IList<SnapshotRegion>> dataRegions = new ConcurrentBag<IList<SnapshotRegion>>();

                        Parallel.ForEach(
                            modules,
                            ParallelSettings.ParallelSettingsFastest,
                            (module) =>
                            {
                                DirectoryInfo modulePathInfo = new DirectoryInfo(module.FullPath);

                                // Collect module data segments, unless it is a system module and ignore is set
                                if (!ignoreSystemModules || modulePathInfo.Parent != systemPathInfo)
                                {
                                    IList<SnapshotRegion> dataSegments = MetaData.Default.GetDataSegments(module.BaseAddress, module.FullPath);
                                    dataRegions.Add(dataSegments);
                                }

                                // Update progress every N regions
                                if (Interlocked.Increment(ref processedModules) % 32 == 0)
                                {
                                    updateProgress((float)processedModules / (float)modules.Count() * 100.0f);
                                }
                            });

                        Snapshot result = new Snapshot(DataSegmentCollector.Name, dataRegions.SelectMany(region => region));
                        cancellationToken.ThrowIfCancellationRequested();

                        stopwatch.Stop();
                        Logger.Log(LogLevel.Info, "Data segments collected in: " + stopwatch.Elapsed);
                        return result;
                    }
                    catch (OperationCanceledException ex)
                    {
                        Logger.Log(LogLevel.Warn, "Data segment collection canceled", ex);
                        return null;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogLevel.Error, "Error performing data segment collection", ex);
                        return null;
                    }
                }, cancellationToken));
        }
    }
    //// End class
}
//// End namespace