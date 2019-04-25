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
        /// Initializes a new instance of the <see cref="WindowsMemoryAllocator"/> class.
        /// </summary>
        /// <param name="targetProcess">The target process.</param>
        public WindowsMemoryAllocator(Process targetProcess)
        {
            this.TargetProcess = targetProcess;
        }

        /// <summary>
        /// Gets or sets a reference to the target process.
        /// </summary>
        public Process TargetProcess { get; set; }

        /// <summary>
        /// Allocates memory in the opened process.
        /// </summary>
        /// <param name="size">The size of the memory allocation.</param>
        /// <returns>A pointer to the location of the allocated memory.</returns>
        public UInt64 AllocateMemory(Int32 size)
        {
            return Memory.Allocate(this.TargetProcess == null ? IntPtr.Zero : this.TargetProcess.Handle, 0, size);
        }

        /// <summary>
        /// Allocates memory in the opened process.
        /// </summary>
        /// <param name="size">The size of the memory allocation.</param>
        /// <param name="allocAddress">The rough address of where the allocation should take place.</param>
        /// <returns>A pointer to the location of the allocated memory.</returns>
        public UInt64 AllocateMemory(Int32 size, UInt64 allocAddress)
        {
            return Memory.Allocate(this.TargetProcess == null ? IntPtr.Zero : this.TargetProcess.Handle, allocAddress, size);
        }

        /// <summary>
        /// Deallocates memory in the opened process.
        /// </summary>
        /// <param name="address">The address to perform the region wide deallocation.</param>
        public void DeallocateMemory(UInt64 address)
        {
            Memory.Free(this.TargetProcess == null ? IntPtr.Zero : this.TargetProcess.Handle, address);
        }
    }
    //// End class
}
//// End namespace