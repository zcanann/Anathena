namespace Squalr.Engine.Memory.Windows
{
    using Squalr.Engine.Common;
    using Squalr.Engine.Common.Extensions;
    using Squalr.Engine.Memory.Windows.Native;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using static Squalr.Engine.Memory.Windows.Native.Enumerations;
    using static Squalr.Engine.Memory.Windows.Native.Structures;

    /// <summary>
    /// Class for managing allocations in an external process.
    /// </summary>
    internal class WindowsMemoryAllocator : IMemoryAllocator
    {
        /// <summary>
        /// The number of retry attempts for memory allocation. This is used to prevent cases of bad luck for 64-bit memory allocation when a specific address is requested.
        /// </summary>
        private const Int32 AllocateRetryCount = 4;

        /// <summary>
        /// A windows constraint on the address alignment of an allocated virtual memory page.
        /// </summary>
        private const Int32 AllocAlignment = 0x10000;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsMemoryAllocator"/> class.
        /// </summary>
        public WindowsMemoryAllocator()
        {
        }

        /// <summary>
        /// Allocates memory in the opened process.
        /// </summary>
        /// <param name="size">The size of the memory allocation.</param>
        /// <returns>A pointer to the location of the allocated memory.</returns>
        public UInt64 AllocateMemory(Process process, Int32 size)
        {
            return WindowsMemoryAllocator.Allocate(process == null ? IntPtr.Zero : process.Handle, 0, size);
        }

        /// <summary>
        /// Allocates memory in the opened process.
        /// </summary>
        /// <param name="size">The size of the memory allocation.</param>
        /// <param name="allocAddress">The rough address of where the allocation should take place.</param>
        /// <returns>A pointer to the location of the allocated memory.</returns>
        public UInt64 AllocateMemory(Process process, Int32 size, UInt64 allocAddress)
        {
            return WindowsMemoryAllocator.Allocate(process == null ? IntPtr.Zero : process.Handle, allocAddress, size);
        }

        /// <summary>
        /// Deallocates memory in the opened process.
        /// </summary>
        /// <param name="address">The address to perform the region wide deallocation.</param>
        public void DeallocateMemory(Process process, UInt64 address)
        {
            NativeMethods.VirtualFreeEx(process == null ? IntPtr.Zero : process.Handle, address.ToIntPtr(), 0, MemoryReleaseFlags.Release);
        }

        /// <summary>
        /// Reserves a region of memory within the virtual address space of a specified process.
        /// </summary>
        /// <param name="processHandle">The handle to a process.</param>
        /// <param name="allocAddress">The rough address of where the allocation should take place.</param>
        /// <param name="size">The size of the region of memory to allocate, in bytes.</param>
        /// <param name="protectionFlags">The memory protection for the region of pages to be allocated.</param>
        /// <param name="allocationFlags">The type of memory allocation.</param>
        /// <returns>The base address of the allocated region</returns>
        private static UInt64 Allocate(
            IntPtr processHandle,
            UInt64 allocAddress,
            Int32 size,
            MemoryProtectionFlags protectionFlags = MemoryProtectionFlags.ExecuteReadWrite,
            MemoryAllocationFlags allocationFlags = MemoryAllocationFlags.Commit | MemoryAllocationFlags.Reserve)
        {
            if (allocAddress != 0)
            {
                /* A specific address has been given. We will modify it to support the following constraints:
                 *  - Aligned by 0x10000 / 65536
                 *  - Pointing to an unallocated region of memory
                 *  - Within +/- 2GB (using 1GB for safety) of address space of the originally specified address, such as to always be in range of a far jump instruction
                 * Note: A retry count has been put in place because VirtualAllocEx with an allocAddress specified may be invalid by the time we request the allocation.
                 */

                UInt64 result = 0;
                Int32 retryCount = 0;

                // Request all chunks of unallocated memory. These will be very large in a 64-bit process.
                IEnumerable<MemoryBasicInformation64> freeMemory = WindowsMemoryQuery.QueryUnallocatedMemory(
                    processHandle,
                    allocAddress.Subtract(Int32.MaxValue >> 1, wrapAround: false),
                    allocAddress.Add(Int32.MaxValue >> 1, wrapAround: false));

                // Convert to normalized regions
                IEnumerable<NormalizedRegion> regions = freeMemory.Select(x => new NormalizedRegion(x.BaseAddress.ToUInt64(), x.RegionSize.ToInt32()));

                // Chunk the large regions into smaller regions based on the allocation size (minimum size is the alloc alignment to prevent creating too many chunks)
                List<NormalizedRegion> subRegions = new List<NormalizedRegion>();
                foreach (NormalizedRegion region in regions)
                {
                    region.BaseAddress = region.BaseAddress.Subtract(region.BaseAddress.Mod(WindowsMemoryAllocator.AllocAlignment), wrapAround: false);
                    IEnumerable<NormalizedRegion> chunkedRegions = region.ChunkNormalizedRegion(Math.Max(size, WindowsMemoryAllocator.AllocAlignment)).Take(128).Where(x => x.RegionSize >= size);
                    subRegions.AddRange(chunkedRegions);
                }

                do
                {
                    // Sample a random chunk and attempt to allocate the memory
                    result = subRegions.ElementAt(StaticRandom.Next(0, subRegions.Count())).BaseAddress;
                    result = NativeMethods.VirtualAllocEx(processHandle, result.ToIntPtr(), size, allocationFlags, protectionFlags).ToUInt64();

                    if (result != 0 || retryCount >= WindowsMemoryAllocator.AllocateRetryCount)
                    {
                        break;
                    }

                    retryCount++;
                }
                while (result == 0);

                return result;
            }
            else
            {
                // Allocate a memory page
                return NativeMethods.VirtualAllocEx(processHandle, allocAddress.ToIntPtr(), size, allocationFlags, protectionFlags).ToUInt64();
            }
        }
    }
    //// End class
}
//// End namespace