namespace Squalr.Engine.Scanning.Snapshots
{
    using Squalr.Engine.Common.DataTypes;
    using Squalr.Engine.Common.Logging;
    using Squalr.Engine.Memory;
    using Squalr.Engine.Scanning.Properties;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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
        public static Snapshot GetSnapshot(Process process, SnapshotRetrievalMode snapshotCreationMode, DataTypeBase dataType)
        {
            switch (snapshotCreationMode)
            {
                case SnapshotRetrievalMode.FromSettings:
                    return SnapshotQuery.CreateSnapshotFromSettings(process, dataType);
                case SnapshotRetrievalMode.FromUserModeMemory:
                    return SnapshotQuery.CreateSnapshotFromUsermodeMemory(process, dataType);
                case SnapshotRetrievalMode.FromModules:
                    return SnapshotQuery.CreateSnapshotFromModules(process, dataType);
                case SnapshotRetrievalMode.FromHeaps:
                    return SnapshotQuery.CreateSnapshotFromHeaps(process, dataType);
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
        private static Snapshot CreateSnapshotFromUsermodeMemory(Process process, DataTypeBase dataType)
        {
            MemoryProtectionEnum requiredPageFlags = 0;
            MemoryProtectionEnum excludedPageFlags = 0;
            MemoryTypeEnum allowedTypeFlags = MemoryTypeEnum.None | MemoryTypeEnum.Private | MemoryTypeEnum.Image;

            UInt64 startAddress = 0;
            UInt64 endAddress = MemoryQueryer.Instance.GetMaxUsermodeAddress(process);

            List<ReadGroup> memoryRegions = new List<ReadGroup>();
            IEnumerable<NormalizedRegion> virtualPages = MemoryQueryer.Instance.GetVirtualPages(
                process,
                requiredPageFlags,
                excludedPageFlags,
                allowedTypeFlags,
                startAddress,
                endAddress);

            foreach (NormalizedRegion virtualPage in virtualPages)
            {
                memoryRegions.Add(new ReadGroup(virtualPage.BaseAddress, virtualPage.RegionSize, dataType, ScanSettings.Alignment));
            }

            return new Snapshot(process, null, memoryRegions);
        }

        /// <summary>
        /// Creates a new snapshot of memory in the target process. Will not read any memory.
        /// </summary>
        /// <returns>The snapshot of memory taken in the target process.</returns>
        private static Snapshot CreateSnapshotFromSettings(Process process, DataTypeBase dataType)
        {
            MemoryProtectionEnum requiredPageFlags = SnapshotQuery.GetRequiredProtectionSettings();
            MemoryProtectionEnum excludedPageFlags = SnapshotQuery.GetExcludedProtectionSettings();
            MemoryTypeEnum allowedTypeFlags = SnapshotQuery.GetAllowedTypeSettings();

            UInt64 startAddress;
            UInt64 endAddress;

            if (ScanSettings.IsUserMode)
            {
                startAddress = 0;
                endAddress = MemoryQueryer.Instance.GetMaxUsermodeAddress(process);
            }
            else
            {
                startAddress = ScanSettings.StartAddress;
                endAddress = ScanSettings.EndAddress;
            }

            List<ReadGroup> memoryRegions = new List<ReadGroup>();
            IEnumerable<NormalizedRegion> virtualPages = MemoryQueryer.Instance.GetVirtualPages(
                process,
                requiredPageFlags,
                excludedPageFlags,
                allowedTypeFlags,
                startAddress,
                endAddress);

            // Convert each virtual page to a snapshot region
            foreach (NormalizedRegion virtualPage in virtualPages)
            {
                memoryRegions.Add(new ReadGroup(virtualPage.BaseAddress, virtualPage.RegionSize, dataType, ScanSettings.Alignment));
            }

            return new Snapshot(process, null, memoryRegions);
        }

        /// <summary>
        /// Creates a snapshot from modules in the selected process.
        /// </summary>
        /// <returns>The created snapshot.</returns>
        private static Snapshot CreateSnapshotFromModules(Process process, DataTypeBase dataType)
        {
            IList<ReadGroup> moduleGroups = MemoryQueryer.Instance.GetModules(process).Select(region => new ReadGroup(region.BaseAddress, region.RegionSize, dataType, ScanSettings.Alignment)).ToList();
            Snapshot moduleSnapshot = new Snapshot(process, null, moduleGroups);

            return moduleSnapshot;
        }

        /// <summary>
        /// Creates a snapshot from modules in the selected process.
        /// </summary>
        /// <returns>The created snapshot.</returns>
        private static Snapshot CreateSnapshotFromHeaps(Process process, DataTypeBase dataType)
        {
            // TODO: This currently grabs all usermode memory and excludes modules. A better implementation would involve actually grabbing heaps.
            Snapshot snapshot = SnapshotQuery.CreateSnapshotFromUsermodeMemory(process, dataType);
            IEnumerable<NormalizedModule> modules = MemoryQueryer.Instance.GetModules(process);

            MemoryProtectionEnum requiredPageFlags = 0;
            MemoryProtectionEnum excludedPageFlags = 0;
            MemoryTypeEnum allowedTypeFlags = MemoryTypeEnum.None | MemoryTypeEnum.Private | MemoryTypeEnum.Image;

            UInt64 startAddress = 0;
            UInt64 endAddress = MemoryQueryer.Instance.GetMaxUsermodeAddress(process);

            List<ReadGroup> memoryRegions = new List<ReadGroup>();
            IEnumerable<NormalizedRegion> virtualPages = MemoryQueryer.Instance.GetVirtualPages(
                process,
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

                memoryRegions.Add(new ReadGroup(virtualPage.BaseAddress, virtualPage.RegionSize, dataType, ScanSettings.Alignment));
            }

            return new Snapshot(process, null, memoryRegions);
        }

        /// <summary>
        /// Gets the allowed type settings for virtual memory queries based on the set type flags.
        /// </summary>
        /// <returns>The flags of the allowed types for virtual memory queries.</returns>
        private static MemoryTypeEnum GetAllowedTypeSettings()
        {
            MemoryTypeEnum result = 0;

            if (ScanSettings.MemoryTypeNone)
            {
                result |= MemoryTypeEnum.None;
            }

            if (ScanSettings.MemoryTypePrivate)
            {
                result |= MemoryTypeEnum.Private;
            }

            if (ScanSettings.MemoryTypeImage)
            {
                result |= MemoryTypeEnum.Image;
            }

            if (ScanSettings.MemoryTypeMapped)
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

            if (ScanSettings.RequiredWrite)
            {
                result |= MemoryProtectionEnum.Write;
            }

            if (ScanSettings.RequiredExecute)
            {
                result |= MemoryProtectionEnum.Execute;
            }

            if (ScanSettings.RequiredCopyOnWrite)
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

            if (ScanSettings.ExcludedWrite)
            {
                result |= MemoryProtectionEnum.Write;
            }

            if (ScanSettings.ExcludedExecute)
            {
                result |= MemoryProtectionEnum.Execute;
            }

            if (ScanSettings.ExcludedCopyOnWrite)
            {
                result |= MemoryProtectionEnum.CopyOnWrite;
            }

            return result;
        }
    }
    //// End class
}
//// End namespace