namespace Squalr.Engine.Scanning.Scanners.Pointers
{
    using Squalr.Engine.Common;
    using Squalr.Engine.Common.Logging;
    using Squalr.Engine.Scanning.Scanners.Pointers.Structures;
    using Squalr.Engine.Scanning.Snapshots;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using static Squalr.Engine.Common.TrackableTask;

    /// <summary>
    /// Scans for pointers in the target process.
    /// </summary>
    public static class PointerRetargetScan
    {
        /// <summary>
        /// The name of this scan.
        /// </summary>
        private const String Name = "Pointer Retarget";

        /// <summary>
        /// Performs a pointer scan for a given address.
        /// </summary>
        /// <param name="address">The address for which to perform a pointer scan.</param>
        /// <param name="maxOffset">The maximum pointer offset.</param>
        /// <param name="depth">The maximum pointer search depth.</param>
        /// <param name="alignment">The pointer scan alignment.</param>
        /// <param name="taskIdentifier">The unique identifier to prevent duplicate tasks.</param>
        /// <returns>Atrackable task that returns the scan results.</returns>
        public static TrackableTask<PointerBag> Scan(UInt64 newAddress, Int32 alignment, PointerBag oldPointerBag, String taskIdentifier = null)
        {
            try
            {
                return TrackableTask<PointerBag>
                    .Create(PointerRetargetScan.Name, taskIdentifier, out UpdateProgress updateProgress, out CancellationToken cancellationToken)
                    .With(Task<PointerBag>.Run(() =>
                {
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();

                        // Step 1) Create a snapshot of the new target address
                        Snapshot targetAddress = new Snapshot(new SnapshotRegion[] { new SnapshotRegion(new ReadGroup(newAddress, oldPointerBag.PointerSize.ToSize(), oldPointerBag.PointerSize.ToDataType(), alignment), 0, oldPointerBag.PointerSize.ToSize()) });

                        // Step 2) Collect heap pointers
                        Snapshot heapPointers = SnapshotManager.GetSnapshot(Snapshot.SnapshotRetrievalMode.FromHeaps, oldPointerBag.PointerSize.ToDataType());
                        TrackableTask<Snapshot> heapValueCollector = ValueCollector.CollectValues(heapPointers);
                        heapPointers = heapValueCollector.Result;

                        // Step 3) Rebuild levels
                        IList<Level> levels = new List<Level>();

                        if (oldPointerBag.Depth > 0)
                        {
                            // Create 1st level with target address and previous static pointers
                            levels.Add(new Level(targetAddress, oldPointerBag.Levels.First().StaticPointers));

                            // Copy over all old static pointers, and replace the heaps with a full heap
                            foreach (Level level in oldPointerBag.Levels.Skip(1))
                            {
                                levels.Add(new Level(heapPointers, level.StaticPointers));
                            }
                        }

                        // Exit if canceled
                        cancellationToken.ThrowIfCancellationRequested();

                        // Step 4) Perform a rebase from the old static addresses onto the new heaps
                        PointerBag newPointerBag = new PointerBag(levels, oldPointerBag.MaxOffset, oldPointerBag.PointerSize);
                        TrackableTask<PointerBag> pointerRebaseTask = PointerRebase.Scan(newPointerBag, readMemory: true, performUnchangedScan: true);
                        PointerBag rebasedPointerBag = pointerRebaseTask.Result;

                        stopwatch.Stop();
                        Logger.Log(LogLevel.Info, "Pointer retarget complete in: " + stopwatch.Elapsed);

                        return rebasedPointerBag;
                    }
                    catch (OperationCanceledException ex)
                    {
                        Logger.Log(LogLevel.Warn, "Pointer retarget canceled", ex);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogLevel.Error, "Error performing pointer retarget", ex);
                    }

                    return null;
                }, cancellationToken));
            }
            catch (TaskConflictException ex)
            {
                Logger.Log(LogLevel.Warn, "A pointer scan is already scheduled.");
                throw ex;
            }
        }
    }
    //// End class
}
//// End namespace