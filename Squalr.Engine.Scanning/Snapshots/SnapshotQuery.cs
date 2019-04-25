namespace Squalr.Engine.Scanning.Snapshots
{
    using Squalr.Engine.Common.DataTypes;
    using Squalr.Engine.Common.Logging;
    using Squalr.Engine.Memory;
    using Squalr.Engine.Scanning.Properties;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class SnapshotQuery
    {
        [Flags]
        public enum SnapshotRetrievalMode
        {
            FromSettings,
            FromUserModeMemory,
            FromHeaps,
            FromStack,
            FromModules,
        }

        /// <summary>
        /// Gets a snapshot based on the provided mode. Will not read any memory.
        /// </summary>
        /// <param name="snapshotCreationMode">The method of snapshot retrieval.</param>
        /// <returns>The collected snapshot.</returns>
        public static Snapshot GetSnapshot(SnapshotRetrievalMode snapshotCreationMode, DataType dataType)
        {
            switch (snapshotCreationMode)
            {
                case SnapshotRetrievalMode.FromSettings:
                    return SnapshotQuery.CreateSnapshotFromSettings(dataType);
                case SnapshotRetrievalMode.FromUserModeMemory:
                    return SnapshotQuery.CreateSnapshotFromUsermodeMemory(dataType);
                case SnapshotRetrievalMode.FromModules:
                    return SnapshotQuery.CreateSnapshotFromModules(dataType);
                case SnapshotRetrievalMode.FromHeaps:
                    return SnapshotQuery.CreateSnapshotFromHeaps(dataType);
                case SnapshotRetrievalMode.FromStack:
                    throw new NotImplementedException();
                default:
                    Logger.Log(LogLevel.Error, "Unknown snapshot retrieval mode");
                    return null;
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
            UInt64 endAddress = MemoryQueryerFactory.Default.GetMaxUsermodeAddress();

            List<ReadGroup> memoryRegions = new List<ReadGroup>();
            IEnumerable<NormalizedRegion> virtualPages = MemoryQueryerFactory.Default.GetVirtualPages(
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
            MemoryProtectionEnum requiredPageFlags = SnapshotQuery.GetRequiredProtectionSettings();
            MemoryProtectionEnum excludedPageFlags = SnapshotQuery.GetExcludedProtectionSettings();
            MemoryTypeEnum allowedTypeFlags = SnapshotQuery.GetAllowedTypeSettings();

            UInt64 startAddress;
            UInt64 endAddress;

            if (ScanSettings.Default.IsUserMode)
            {
                startAddress = 0;
                endAddress = MemoryQueryerFactory.Default.GetMaxUsermodeAddress();
            }
            else
            {
                startAddress = ScanSettings.Default.StartAddress;
                endAddress = ScanSettings.Default.EndAddress;
            }

            List<ReadGroup> memoryRegions = new List<ReadGroup>();
            IEnumerable<NormalizedRegion> virtualPages = MemoryQueryerFactory.Default.GetVirtualPages(
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
            IList<ReadGroup> moduleGroups = MemoryQueryerFactory.Default.GetModules().Select(region => new ReadGroup(region.BaseAddress, region.RegionSize, dataType, ScanSettings.Default.Alignment)).ToList();
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
            Snapshot snapshot = SnapshotQuery.CreateSnapshotFromUsermodeMemory(dataType);
            IEnumerable<NormalizedModule> modules = MemoryQueryerFactory.Default.GetModules();

            MemoryProtectionEnum requiredPageFlags = 0;
            MemoryProtectionEnum excludedPageFlags = 0;
            MemoryTypeEnum allowedTypeFlags = MemoryTypeEnum.None | MemoryTypeEnum.Private | MemoryTypeEnum.Image;

            UInt64 startAddress = 0;
            UInt64 endAddress = MemoryQueryerFactory.Default.GetMaxUsermodeAddress();

            List<ReadGroup> memoryRegions = new List<ReadGroup>();
            IEnumerable<NormalizedRegion> virtualPages = MemoryQueryerFactory.Default.GetVirtualPages(
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