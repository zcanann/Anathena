namespace Squalr.Engine.Memory
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// An interface for querying virtual memory.
    /// </summary>
    public interface IMemoryAllocator
    {
        /// <summary>
        /// Allocates memory in the opened process.
        /// </summary>
        /// <param name="size">The size of the memory allocation.</param>
        /// <returns>A pointer to the location of the allocated memory.</returns>
        UInt64 AllocateMemory(Process process, Int32 size);

        /// <summary>
        /// Allocates memory in the opened process.
        /// </summary>
        /// <param name="size">The size of the memory allocation.</param>
        /// <param name="allocAddress">The rough address of where the allocation should take place.</param>
        /// <returns>A pointer to the location of the allocated memory.</returns>
        UInt64 AllocateMemory(Process process, Int32 size, UInt64 allocAddress);

        /// <summary>
        /// Deallocates memory in the opened process.
        /// </summary>
        /// <param name="address">The address to perform the region wide deallocation.</param>
        void DeallocateMemory(Process process, UInt64 address);
    }
    //// End interface
}
//// End namespace