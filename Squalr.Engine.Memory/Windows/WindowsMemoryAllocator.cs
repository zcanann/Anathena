namespace Squalr.Engine.Memory.Windows
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Class for managing allocations in an external process.
    /// </summary>
    internal class WindowsMemoryAllocator : IMemoryAllocator
    {
        /// <summary>
        /// The chunk size for memory regions. Prevents large allocations.
        /// </summary>
        private const Int32 ChunkSize = 2000000000;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsAdapter"/> class.
        /// </summary>
        public WindowsMemoryAllocator()
        {
        }

        /// <summary>
        /// Gets a reference to the target process. This is an optimization to minimize accesses to the Processes component of the Engine.
        /// </summary>
        public Process ExternalProcess { get; set; }

        /// <summary>
        /// Recieves a process update. This is an optimization over grabbing the process from the <see cref="IProcessInfo"/> component
        /// of the <see cref="EngineCore"/> every time we need it, which would be cumbersome when doing hundreds of thousands of memory read/writes.
        /// </summary>
        /// <param name="process">The newly selected process.</param>
        public void Update(Process process)
        {
            this.ExternalProcess = process;
        }

        /// <summary>
        /// Allocates memory in the opened process.
        /// </summary>
        /// <param name="size">The size of the memory allocation.</param>
        /// <returns>A pointer to the location of the allocated memory.</returns>
        public UInt64 AllocateMemory(Int32 size)
        {
            return Memory.Allocate(this.ExternalProcess == null ? IntPtr.Zero : this.ExternalProcess.Handle, 0, size);
        }

        /// <summary>
        /// Allocates memory in the opened process.
        /// </summary>
        /// <param name="size">The size of the memory allocation.</param>
        /// <param name="allocAddress">The rough address of where the allocation should take place.</param>
        /// <returns>A pointer to the location of the allocated memory.</returns>
        public UInt64 AllocateMemory(Int32 size, UInt64 allocAddress)
        {
            return Memory.Allocate(this.ExternalProcess == null ? IntPtr.Zero : this.ExternalProcess.Handle, allocAddress, size);
        }

        /// <summary>
        /// Deallocates memory in the opened process.
        /// </summary>
        /// <param name="address">The address to perform the region wide deallocation.</param>
        public void DeallocateMemory(UInt64 address)
        {
            Memory.Free(this.ExternalProcess == null ? IntPtr.Zero : this.ExternalProcess.Handle, address);
        }
    }
    //// End class
}
//// End namespace