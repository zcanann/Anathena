namespace Squalr.Engine.Scanning.Scanners.Pointers
{
    using Squalr.Engine.Common;
    using Squalr.Engine.Common.Logging;
    using Squalr.Engine.Scanning.Scanners.Constraints;
    using Squalr.Engine.Scanning.Scanners.Pointers.SearchKernels;
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
    public static class PointerRebase
    {
        /// <summary>
        /// The name of this scan.
        /// </summary>
        private const String Name = "Pointer Rescan";

        /// <summary>
        /// Performs a pointer scan for a given address.
        /// </summary>
        /// <param name="address">The address for which to perform a pointer scan.</param>
        /// <param name="maxOffset">The maximum pointer offset.</param>
        /// <param name="depth">The maximum pointer search depth.</param>
        /// <param name="alignment">The pointer scan alignment.</param>
        /// <param name="taskIdentifier">The unique identifier to prevent duplicate tasks.</param>
        /// <returns>Atrackable task that returns the scan results.</returns>
        public static TrackableTask<PointerBag> Scan(PointerBag previousPointerBag, Boolean readMemory, Boolean performUnchangedScan, String taskIdentifier = null)
        {
            try
            {
                TrackableTask<PointerBag> pointerScanTask = TrackableTask<PointerBag>.Create(PointerRebase.Name, taskIdentifier, out UpdateProgress updateProgress, out CancellationToken cancellationToken);

                return pointerScanTask.With(Task<PointerBag>.Run(() =>
                    {
                        try
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            Stopwatch stopwatch = new Stopwatch();
                            stopwatch.Start();

                            ScanConstraint scanConstraint = new ScanConstraint(ScanConstraint.ConstraintType.Unchanged);
                            scanConstraint.SetElementType(previousPointerBag.PointerSize.ToDataType());

                            IList<Level> oldLevels = previousPointerBag.Levels;
                            IList<Level> newLevels = new List<Level>();

                            for (Int32 levelIndex = 0; levelIndex < oldLevels.Count; levelIndex++)
                            {
                                Snapshot updatedStaticPointers = oldLevels[levelIndex].StaticPointers;
                                Snapshot updatedHeapPointers = oldLevels[levelIndex].HeapPointers;

                                // Step 1) Re-read values of all pointers
                                if (readMemory)
                                {
                                    TrackableTask<Snapshot> staticValueCollector = ValueCollector.CollectValues(updatedStaticPointers);

                                    // Does not apply to target address
                                    if (levelIndex > 0)
                                    {
                                        TrackableTask<Snapshot> heapValueCollector = ValueCollector.CollectValues(updatedHeapPointers);
                                        updatedHeapPointers = heapValueCollector.Result;
                                    }

                                    updatedStaticPointers = staticValueCollector.Result;
                                }

                                // Step 2) A neat (optional) trick: Scan for unchanged values to filter out dynamic pointers
                                if (performUnchangedScan)
                                {
                                    TrackableTask<Snapshot> staticValueScanner = ManualScanner.Scan(updatedStaticPointers, scanConstraint);

                                    // Does not apply to target address
                                    if (levelIndex > 0)
                                    {
                                        TrackableTask<Snapshot> heapValueScanner = ManualScanner.Scan(updatedHeapPointers, scanConstraint);
                                        updatedHeapPointers = heapValueScanner.Result;
                                    }

                                    updatedStaticPointers = staticValueScanner.Result;
                                }

                                Stopwatch levelStopwatch = new Stopwatch();
                                levelStopwatch.Start();

                                // Step 3) Rebase heap onto new previous heap
                                if (levelIndex > 0)
                                {
                                    IVectorSearchKernel heapSearchKernel = SearchKernelFactory.GetSearchKernel(newLevels.Last().HeapPointers, previousPointerBag.MaxOffset, previousPointerBag.PointerSize);
                                    TrackableTask<Snapshot> heapFilterTask = PointerFilter.Filter(pointerScanTask, updatedHeapPointers, heapSearchKernel, newLevels.Last().HeapPointers, previousPointerBag.MaxOffset);

                                    updatedHeapPointers = heapFilterTask.Result;
                                }

                                // Step 4) Filter static pointers that still point into the updated heap
                                IVectorSearchKernel staticSearchKernel = SearchKernelFactory.GetSearchKernel(updatedHeapPointers, previousPointerBag.MaxOffset, previousPointerBag.PointerSize);
                                TrackableTask<Snapshot> staticFilterTask = PointerFilter.Filter(pointerScanTask, updatedStaticPointers, staticSearchKernel, updatedHeapPointers, previousPointerBag.MaxOffset);

                                updatedStaticPointers = staticFilterTask.Result;

                                levelStopwatch.Stop();
                                Logger.Log(LogLevel.Info, "Pointer rebase from level " + (levelIndex) + " => " + (levelIndex + 1) + " completed in: " + levelStopwatch.Elapsed);

                                newLevels.Add(new Level(updatedHeapPointers, updatedStaticPointers));
                            }

                            // Exit if canceled
                            cancellationToken.ThrowIfCancellationRequested();

                            PointerBag pointerBag = new PointerBag(newLevels, previousPointerBag.MaxOffset, previousPointerBag.PointerSize);

                            stopwatch.Stop();
                            Logger.Log(LogLevel.Info, "Pointer rebase complete in: " + stopwatch.Elapsed);

                            return pointerBag;
                        }
                        catch (OperationCanceledException ex)
                        {
                            Logger.Log(LogLevel.Warn, "Pointer rebase canceled", ex);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(LogLevel.Error, "Error performing pointer rebase", ex);
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