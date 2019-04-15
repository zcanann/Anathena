namespace Squalr.Engine.Scanning.Snapshots
{
    using Squalr.Engine.Common.DataTypes;
    using Squalr.Engine.Common.Logging;
    using Squalr.Engine.Memory;
    using Squalr.Engine.Scanning.Properties;
    using Squalr.Source.Prefilters;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class SnapshotManager
    {
        /// <summary>
        /// The size limit for snapshots to be saved in the snapshot history (256MB). TODO: Make this a setting.
        /// </summary>
        private const UInt64 SizeLimit = 1UL << 28;

        static SnapshotManager()
        {
            SnapshotManager.AccessLock = new Object();
            SnapshotManager.ObserverLock = new Object();
            SnapshotManager.Snapshots = new Stack<Snapshot>();
            SnapshotManager.DeletedSnapshots = new Stack<Snapshot>();
            SnapshotManager.SnapshotObservers = new List<ISnapshotObserver>();
        }

        /// <summary>
        /// Gets the snapshots being managed.
        /// </summary>
        public static Stack<Snapshot> Snapshots { get; private set; }

        /// <summary>
        /// Gets the deleted snapshots for the capability of redoing after undo.
        /// </summary>
        public static Stack<Snapshot> DeletedSnapshots { get; private set; }

        /// <summary>
        /// Gets or sets a lock to ensure multiple entities do not try and update the snapshot list at the same time.
        /// </summary>
        private static Object AccessLock { get; set; }

        /// <summary>
        /// Gets or sets a lock to ensure multiple entities do not try and update the snapshot list at the same time.
        /// </summary>
        private static Object ObserverLock { get; set; }

        /// <summary>
        /// Gets or sets objects observing changes in the active snapshot.
        /// </summary>
        private static List<ISnapshotObserver> SnapshotObservers { get; set; }

        /// <summary>
        /// Subscribes the given object to changes in the active snapshot.
        /// </summary>
        /// <param name="snapshotObserver">The object to observe active snapshot changes.</param>
        public static void Subscribe(ISnapshotObserver snapshotObserver)
        {
            lock (SnapshotManager.ObserverLock)
            {
                if (!SnapshotManager.SnapshotObservers.Contains(snapshotObserver))
                {
                    SnapshotManager.SnapshotObservers.Add(snapshotObserver);
                    snapshotObserver.Update(SnapshotManager.GetSnapshot(Snapshot.SnapshotRetrievalMode.FromActiveSnapshot, null));
                }
            }
        }

        /// <summary>
        /// Unsubscribes the given object from changes in the active snapshot.
        /// </summary>
        /// <param name="snapshotObserver">The object to observe active snapshot changes.</param>
        public static void Unsubscribe(ISnapshotObserver snapshotObserver)
        {
            lock (SnapshotManager.ObserverLock)
            {
                if (SnapshotManager.SnapshotObservers.Contains(snapshotObserver))
                {
                    SnapshotManager.SnapshotObservers.Remove(snapshotObserver);
                }
            }
        }

        /// <summary>
        /// Gets a snapshot based on the provided mode. Will not read any memory.
        /// </summary>
        /// <param name="snapshotCreationMode">The method of snapshot retrieval.</param>
        /// <returns>The collected snapshot.</returns>
        public static Snapshot GetSnapshot(Snapshot.SnapshotRetrievalMode snapshotCreationMode, DataType dataType)
        {
            switch (snapshotCreationMode)
            {
                case Snapshot.SnapshotRetrievalMode.FromActiveSnapshot:
                    return SnapshotManager.GetActiveSnapshot();
                case Snapshot.SnapshotRetrievalMode.FromActiveSnapshotOrPrefilter:
                    return SnapshotManager.GetActiveSnapshotCreateIfNone(dataType);
                case Snapshot.SnapshotRetrievalMode.FromSettings:
                    return SnapshotManager.CreateSnapshotFromSettings(dataType);
                case Snapshot.SnapshotRetrievalMode.FromUserModeMemory:
                    return SnapshotManager.CreateSnapshotFromUsermodeMemory(dataType);
                case Snapshot.SnapshotRetrievalMode.FromModules:
                    return SnapshotManager.CreateSnapshotFromModules(dataType);
                case Snapshot.SnapshotRetrievalMode.FromHeaps:
                    return SnapshotManager.CreateSnapshotFromHeaps(dataType);
                case Snapshot.SnapshotRetrievalMode.FromStack:
                    throw new NotImplementedException();
                default:
                    Logger.Log(LogLevel.Error, "Unknown snapshot retrieval mode");
                    return null;
            }
        }

        /// <summary>
        /// Returns the memory regions associated with the current snapshot. If none exist, a query will be done. Will not read any memory.
        /// </summary>
        /// <returns>The current active snapshot of memory in the target process.</returns>
        private static Snapshot GetActiveSnapshotCreateIfNone(DataType dataType)
        {
            lock (SnapshotManager.AccessLock)
            {
                if (SnapshotManager.Snapshots.Count == 0 || SnapshotManager.Snapshots.Peek() == null || SnapshotManager.Snapshots.Peek().ElementCount == 0)
                {
                    Snapshot snapshot = Prefilter.GetInstance().GetPrefilteredSnapshot(dataType);
                    snapshot.Alignment = ScanSettings.Default.Alignment;
                    return snapshot;
                }

                // Return the snapshot
                return SnapshotManager.Snapshots.Peek();
            }
        }

        /// <summary>
        /// Returns the memory regions associated with the current snapshot. If none exist, a query will be done. Will not read any memory.
        /// </summary>
        /// <returns>The current active snapshot of memory in the target process.</returns>
        private static Snapshot GetActiveSnapshot()
        {
            lock (SnapshotManager.AccessLock)
            {
                // Take a snapshot if there are none, or the current one is empty
                if (SnapshotManager.Snapshots.Count == 0 || SnapshotManager.Snapshots.Peek() == null || SnapshotManager.Snapshots.Peek().ElementCount == 0)
                {
                    return null;
                }

                // Return the snapshot
                return SnapshotManager.Snapshots.Peek();
            }
        }

        /// <summary>
        /// Creates a snapshot from all usermode memory. Will not read any memory.
        /// </summary>
        /// <returns>A snapshot created from usermode memory.</returns>
        private static Snapshot CreateSnapshotFromUsermodeMemory(DataType dataType)
        {
            MemoryProtectionEnum requiredPageFlags = 0;
            MemoryProtectionEnum excludedPageFlags = 0;
            MemoryTypeEnum allowedTypeFlags = MemoryTypeEnum.None | MemoryTypeEnum.Private | MemoryTypeEnum.Image;

            UInt64 startAddress = 0;
            UInt64 endAddress = Query.Default.GetMaxUsermodeAddress();

            List<ReadGroup> memoryRegions = new List<ReadGroup>();
            IEnumerable<NormalizedRegion> virtualPages = Query.Default.GetVirtualPages(
                requiredPageFlags,
                excludedPageFlags,
                allowedTypeFlags,
                startAddress,
                endAddress);

            foreach (NormalizedRegion virtualPage in virtualPages)
            {
                memoryRegions.Add(new ReadGroup(virtualPage.BaseAddress, virtualPage.RegionSize, dataType, ScanSettings.Default.Alignment));
            }

            return new Snapshot(null, memoryRegions);
        }

        /// <summary>
        /// Creates a new snapshot of memory in the target process. Will not read any memory.
        /// </summary>
        /// <returns>The snapshot of memory taken in the target process.</returns>
        private static Snapshot CreateSnapshotFromSettings(DataType dataType)
        {
            MemoryProtectionEnum requiredPageFlags = SnapshotManager.GetRequiredProtectionSettings();
            MemoryProtectionEnum excludedPageFlags = SnapshotManager.GetExcludedProtectionSettings();
            MemoryTypeEnum allowedTypeFlags = SnapshotManager.GetAllowedTypeSettings();

            UInt64 startAddress;
            UInt64 endAddress;

            if (ScanSettings.Default.IsUserMode)
            {
                startAddress = 0;
                endAddress = Query.Default.GetMaxUsermodeAddress();
            }
            else
            {
                startAddress = ScanSettings.Default.StartAddress;
                endAddress = ScanSettings.Default.EndAddress;
            }

            List<ReadGroup> memoryRegions = new List<ReadGroup>();
            IEnumerable<NormalizedRegion> virtualPages = Query.Default.GetVirtualPages(
                requiredPageFlags,
                excludedPageFlags,
                allowedTypeFlags,
                startAddress,
                endAddress);

            // Convert each virtual page to a snapshot region
            foreach (NormalizedRegion virtualPage in virtualPages)
            {
                memoryRegions.Add(new ReadGroup(virtualPage.BaseAddress, virtualPage.RegionSize, dataType, ScanSettings.Default.Alignment));
            }

            return new Snapshot(null, memoryRegions);
        }

        /// <summary>
        /// Creates a snapshot from modules in the selected process.
        /// </summary>
        /// <returns>The created snapshot.</returns>
        private static Snapshot CreateSnapshotFromModules(DataType dataType)
        {
            IList<ReadGroup> moduleGroups = Query.Default.GetModules().Select(region => new ReadGroup(region.BaseAddress, region.RegionSize, dataType, ScanSettings.Default.Alignment)).ToList();
            Snapshot moduleSnapshot = new Snapshot(null, moduleGroups);

            return moduleSnapshot;
        }

        /// <summary>
        /// Creates a snapshot from modules in the selected process.
        /// </summary>
        /// <returns>The created snapshot.</returns>
        private static Snapshot CreateSnapshotFromHeaps(DataType dataType)
        {
            // TODO: This currently grabs all usermode memory and excludes modules. A better implementation would involve actually grabbing heaps.
            Snapshot snapshot = SnapshotManager.CreateSnapshotFromUsermodeMemory(dataType);
            IEnumerable<NormalizedModule> modules = Query.Default.GetModules();

            MemoryProtectionEnum requiredPageFlags = 0;
            MemoryProtectionEnum excludedPageFlags = 0;
            MemoryTypeEnum allowedTypeFlags = MemoryTypeEnum.None | MemoryTypeEnum.Private | MemoryTypeEnum.Image;

            UInt64 startAddress = 0;
            UInt64 endAddress = Query.Default.GetMaxUsermodeAddress();

            List<ReadGroup> memoryRegions = new List<ReadGroup>();
            IEnumerable<NormalizedRegion> virtualPages = Query.Default.GetVirtualPages(
                requiredPageFlags,
                excludedPageFlags,
                allowedTypeFlags,
                startAddress,
                endAddress);

            foreach (NormalizedRegion virtualPage in virtualPages)
            {
                if (modules.Any(x => x.BaseAddress == virtualPage.BaseAddress))
                {
                    continue;
                }

                memoryRegions.Add(new ReadGroup(virtualPage.BaseAddress, virtualPage.RegionSize, dataType, ScanSettings.Default.Alignment));
            }

            return new Snapshot(null, memoryRegions);
        }

        /// <summary>
        /// Reverses an undo action.
        /// </summary>
        public static void RedoSnapshot()
        {
            lock (SnapshotManager.AccessLock)
            {
                if (SnapshotManager.DeletedSnapshots.Count == 0)
                {
                    return;
                }

                SnapshotManager.Snapshots.Push(SnapshotManager.DeletedSnapshots.Pop());
                SnapshotManager.NotifyObservers();
            }
        }

        /// <summary>
        /// Undoes the current active snapshot, reverting to the previous snapshot.
        /// </summary>
        public static void UndoSnapshot()
        {
            lock (SnapshotManager.AccessLock)
            {
                if (SnapshotManager.Snapshots.Count == 0)
                {
                    return;
                }

                SnapshotManager.DeletedSnapshots.Push(SnapshotManager.Snapshots.Pop());

                if (SnapshotManager.DeletedSnapshots.Peek() == null)
                {
                    SnapshotManager.DeletedSnapshots.Pop();
                }

                SnapshotManager.NotifyObservers();
            }
        }

        /// <summary>
        /// Clears all snapshot records.
        /// </summary>
        public static void ClearSnapshots()
        {
            lock (SnapshotManager.AccessLock)
            {
                SnapshotManager.Snapshots.Clear();
                SnapshotManager.DeletedSnapshots.Clear();
                SnapshotManager.NotifyObservers();

                // There can be multiple GB of deleted snapshots, so run the garbage collector ASAP for a performance boost.
                Task.Run(() => GC.Collect());
            }
        }

        /// <summary>
        /// Saves a new snapshot, which will become the current active snapshot.
        /// </summary>
        /// <param name="snapshot">The snapshot to save.</param>
        public static void SaveSnapshot(Snapshot snapshot)
        {
            lock (SnapshotManager.AccessLock)
            {
                // Remove null snapshot if exists
                if (SnapshotManager.Snapshots.Count != 0 && SnapshotManager.Snapshots.Peek() == null)
                {
                    SnapshotManager.Snapshots.Pop();
                }

                // Do not keep large snapshots in the undo history
                if (SnapshotManager.Snapshots.Count != 0 && SnapshotManager.Snapshots.Peek() != null && SnapshotManager.Snapshots.Peek().ByteCount > SnapshotManager.SizeLimit)
                {
                    SnapshotManager.Snapshots.Pop();
                }

                SnapshotManager.Snapshots.Push(snapshot);
                SnapshotManager.DeletedSnapshots.Clear();
                SnapshotManager.NotifyObservers();
            }
        }

        /// <summary>
        /// Notify all observing objects of an active snapshot change.
        /// </summary>
        private static void NotifyObservers()
        {
            lock (SnapshotManager.ObserverLock)
            {
                Snapshot activeSnapshot = SnapshotManager.GetSnapshot(Snapshot.SnapshotRetrievalMode.FromActiveSnapshot, null);

                foreach (ISnapshotObserver observer in SnapshotManager.SnapshotObservers)
                {
                    observer.Update(activeSnapshot);
                }
            }
        }

        /// <summary>
        /// Gets the allowed type settings for virtual memory queries based on the set type flags.
        /// </summary>
        /// <returns>The flags of the allowed types for virtual memory queries.</returns>
        private static MemoryTypeEnum GetAllowedTypeSettings()
        {
            MemoryTypeEnum result = 0;

            if (ScanSettings.Default.MemoryTypeNone)
            {
                result |= MemoryTypeEnum.None;
            }

            if (ScanSettings.Default.MemoryTypePrivate)
            {
                result |= MemoryTypeEnum.Private;
            }

            if (ScanSettings.Default.MemoryTypeImage)
            {
                result |= MemoryTypeEnum.Image;
            }

            if (ScanSettings.Default.MemoryTypeMapped)
            {
                result |= MemoryTypeEnum.Mapped;
            }

            return result;
        }

        /// <summary>
        /// Gets the required protection settings for virtual memory queries based on the set type flags.
        /// </summary>
        /// <returns>The flags of the required protections for virtual memory queries.</returns>
        private static MemoryProtectionEnum GetRequiredProtectionSettings()
        {
            MemoryProtectionEnum result = 0;

            if (ScanSettings.Default.RequiredWrite)
            {
                result |= MemoryProtectionEnum.Write;
            }

            if (ScanSettings.Default.RequiredExecute)
            {
                result |= MemoryProtectionEnum.Execute;
            }

            if (ScanSettings.Default.RequiredCopyOnWrite)
            {
                result |= MemoryProtectionEnum.CopyOnWrite;
            }

            return result;
        }

        /// <summary>
        /// Gets the excluded protection settings for virtual memory queries based on the set type flags.
        /// </summary>
        /// <returns>The flags of the excluded protections for virtual memory queries.</returns>
        private static MemoryProtectionEnum GetExcludedProtectionSettings()
        {
            MemoryProtectionEnum result = 0;

            if (ScanSettings.Default.ExcludedWrite)
            {
                result |= MemoryProtectionEnum.Write;
            }

            if (ScanSettings.Default.ExcludedExecute)
            {
                result |= MemoryProtectionEnum.Execute;
            }

            if (ScanSettings.Default.ExcludedCopyOnWrite)
            {
                result |= MemoryProtectionEnum.CopyOnWrite;
            }

            return result;
        }
    }
    //// End class
}
//// End namespace