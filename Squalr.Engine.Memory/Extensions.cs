namespace Squalr.Engine.Memory
{
    using Squalr.Engine.Common.DataTypes;
    using System;

    public static class Extensions
    {
        /// <summary>
        /// Reads a value from the opened processes memory.
        /// </summary>
        /// <param name="elementType">The data type to read.</param>
        /// <param name="address">The address to read from.</param>
        /// <param name="success">Whether or not the read succeeded.</param>
        /// <returns>The value read from memory.</returns>
        public static Object Read(this IMemoryReader memory, DataType elementType, UInt64 address, out Boolean success)
        {
            return Reader.Default.Read(elementType, address, out success);
        }

        /// <summary>
        /// Reads a value from the opened processes memory.
        /// </summary>
        /// <typeparam name="T">The data type to read.</typeparam>
        /// <param name="address">The address to read from.</param>
        /// <param name="success">Whether or not the read succeeded.</param>
        /// <returns>The value read from memory.</returns>
        public static T Read<T>(this IMemoryReader memory, UInt64 address, out Boolean success)
        {
            return Reader.Default.Read<T>(address, out success);
        }
    }
    //// End class
}
//// End namespace