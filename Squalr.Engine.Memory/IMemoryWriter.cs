namespace Squalr.Engine.Memory
{
    using Squalr.Engine.Common.DataTypes;
    using System;
    using System.Diagnostics;

    /// <summary>
    /// An interface for writing virtual memory.
    /// </summary>
    public interface IMemoryWriter
    {
        /// <summary>
        /// Writes a value to memory in the opened process.
        /// </summary>
        /// <param name="elementType">The data type to write.</param>
        /// <param name="address">The address to write to.</param>
        /// <param name="value">The value to write.</param>
        void Write(Process process, DataType elementType, UInt64 address, Object value);

        /// <summary>
        /// Writes a value to memory in the opened process.
        /// </summary>
        /// <typeparam name="T">The data type to write.</typeparam>
        /// <param name="address">The address to write to.</param>
        /// <param name="value">The value to write.</param>
        void Write<T>(Process process, UInt64 address, T value);

        /// <summary>
        /// Writes a value to memory in the opened process.
        /// </summary>
        /// <param name="address">The address to write to.</param>
        /// <param name="values">The value to write.</param>
        void WriteBytes(Process process, UInt64 address, Byte[] values);
    }
    //// End interface
}
//// End namespace