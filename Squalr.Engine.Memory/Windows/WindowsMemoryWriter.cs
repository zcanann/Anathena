namespace Squalr.Engine.Memory.Windows
{
    using Squalr.Engine.Common.DataTypes;
    using Squalr.Engine.Common.Extensions;
    using Squalr.Engine.Memory.Windows.Native;
    using System;
    using System.Diagnostics;
    using System.Text;
    using static Squalr.Engine.Memory.Windows.Native.Enumerations;

    /// <summary>
    /// Class for memory editing a remote process.
    /// </summary>
    internal class WindowsMemoryWriter : IMemoryWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsMemoryWriter"/> class.
        /// </summary>
        public WindowsMemoryWriter()
        {
        }

        /// <summary>
        /// Writes a value to memory in the opened process.
        /// </summary>
        /// <param name="elementType">The data type to write.</param>
        /// <param name="address">The address to write to.</param>
        /// <param name="value">The value to write.</param>
        public void Write(Process process, DataTypeBase elementType, UInt64 address, Object value)
        {
            Byte[] bytes;

            switch (elementType)
            {
                case DataTypeBase type when type == DataTypeBase.Byte || type == typeof(Boolean):
                    bytes = new Byte[] { (Byte)value };
                    break;
                case DataTypeBase type when type == DataTypeBase.SByte:
                    bytes = new Byte[] { unchecked((Byte)(SByte)value) };
                    break;
                case DataTypeBase type when type == DataTypeBase.Char:
                    bytes = Encoding.UTF8.GetBytes(new Char[] { (Char)value });
                    break;
                case DataTypeBase type when type == DataTypeBase.Int16:
                    bytes = BitConverter.GetBytes((Int16)value);
                    break;
                case DataTypeBase type when type == DataTypeBase.Int32:
                    bytes = BitConverter.GetBytes((Int32)value);
                    break;
                case DataTypeBase type when type == DataTypeBase.Int64:
                    bytes = BitConverter.GetBytes((Int64)value);
                    break;
                case DataTypeBase type when type == DataTypeBase.UInt16:
                    bytes = BitConverter.GetBytes((UInt16)value);
                    break;
                case DataTypeBase type when type == DataTypeBase.UInt32:
                    bytes = BitConverter.GetBytes((UInt32)value);
                    break;
                case DataTypeBase type when type == DataTypeBase.UInt64:
                    bytes = BitConverter.GetBytes((UInt64)value);
                    break;
                case DataTypeBase type when type == DataTypeBase.Single:
                    bytes = BitConverter.GetBytes((Single)value);
                    break;
                case DataTypeBase type when type == DataTypeBase.Double:
                    bytes = BitConverter.GetBytes((Double)value);
                    break;
                default:
                    throw new ArgumentException("Invalid type provided");
            }

            this.WriteBytes(process, address, bytes);
        }

        /// <summary>
        /// Writes the values of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="address">The address where the value is written.</param>
        /// <param name="value">The value to write.</param>
        public void Write<T>(Process process, UInt64 address, T value)
        {
            this.Write(process, typeof(T), address, (Object)value);
        }

        /// <summary>
        /// Write an array of bytes in the remote process.
        /// </summary>
        /// <param name="address">The address where the array is written.</param>
        /// <param name="byteArray">The array of bytes to write.</param>
        public void WriteBytes(Process process, UInt64 address, Byte[] byteArray)
        {
            IntPtr processHandle = process == null ? IntPtr.Zero : process.Handle;

            MemoryProtectionFlags oldProtection;
            Int32 bytesWritten;
            Boolean success = false;

            try
            {
                NativeMethods.VirtualProtectEx(processHandle, address.ToIntPtr(), byteArray.Length, MemoryProtectionFlags.ExecuteReadWrite, out oldProtection);

                // Write the data to the target process
                if (NativeMethods.WriteProcessMemory(processHandle, address.ToIntPtr(), byteArray, byteArray.Length, out bytesWritten))
                {
                    success = bytesWritten == byteArray.Length;
                }

                NativeMethods.VirtualProtectEx(processHandle, address.ToIntPtr(), byteArray.Length, oldProtection, out oldProtection);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Writes a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="address">The address where the string is written.</param>
        /// <param name="text">The text to write.</param>
        /// <param name="encoding">The encoding used.</param>
        public void WriteString(Process process, UInt64 address, String text, Encoding encoding)
        {
            // Write the text
            this.WriteBytes(process, address, encoding.GetBytes(text + '\0'));
        }
    }
    //// End class
}
//// End namespace