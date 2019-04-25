namespace Squalr.Engine.Memory
{
    using System;

    /// <summary>
    /// Flags that indicate the memory protection for a region of memory.
    /// </summary>
    [Flags]
    public enum MemoryProtectionEnum
    {
        /// <summary>
        /// Writable memory.
        /// </summary>
        Write = 0x1,

        /// <summary>
        /// Executable memory.
        /// </summary>
        Execute = 0x2,

        /// <summary>
        /// Memory marked as copy on write.
        /// </summary>
        CopyOnWrite = 0x4
    }

    /// <summary>
    /// Flags that indicate the memory type for a region of memory.
    /// </summary>
    [Flags]
    public enum MemoryTypeEnum
    {
        /// <summary>
        /// No other flags specified.
        /// </summary>
        None = 0x1,

        /// <summary>
        /// Indicates that the memory pages within the region are private (that is, not shared by other processes).
        /// </summary>
        Private = 0x2,

        /// <summary>
        /// Indicates that the memory pages within the region are mapped into the view of an image section.
        /// </summary>
        Image = 0x4,

        /// <summary>
        /// Indicates that the memory pages within the region are mapped into the view of a section.
        /// </summary>
        Mapped = 0x8
    }
    //// End enum
}
//// End namespace