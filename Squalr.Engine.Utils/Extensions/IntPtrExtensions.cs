namespace Squalr.Engine.Common.Extensions
{
    using System;

    /// <summary>
    /// Extension methods for converting and operating on <see cref="IntPtr"/> types.
    /// </summary>
    public static class IntPtrExtensions
    {
        /// <summary>
        /// Converts the given pointer to a <see cref="IntPtr"/>.
        /// </summary>
        /// <param name="intPtr">The pointer to convert.</param>
        /// <returns>The pointer converted to a <see cref="IntPtr"/>.</returns>
        public static unsafe IntPtr ToIntPtr(this UIntPtr intPtr)
        {
            return unchecked((IntPtr)intPtr.ToPointer());
        }

        /// <summary>
        /// Converts the given pointer to a <see cref="UIntPtr"/>.
        /// </summary>
        /// <param name="intPtr">The pointer to convert.</param>
        /// <returns>The pointer converted to a <see cref="UIntPtr"/>.</returns>
        public static unsafe UIntPtr ToUIntPtr(this IntPtr intPtr)
        {
            return unchecked((UIntPtr)intPtr.ToPointer());
        }

        /// <summary>
        /// Converts an <see cref="IntPtr"/> to an unsigned <see cref="UInt32"/>.
        /// </summary>
        /// <param name="intPtr">The pointer to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static UInt32 ToUInt32(this IntPtr intPtr)
        {
            return unchecked((UInt32)(Int32)intPtr);
        }

        /// <summary>
        /// Converts an <see cref="UIntPtr"/> to an unsigned <see cref="UInt32"/>.
        /// </summary>
        /// <param name="intPtr">The pointer to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static UInt32 ToUInt32(this UIntPtr intPtr)
        {
            return unchecked((UInt32)intPtr);
        }

        /// <summary>
        /// Converts an <see cref="IntPtr"/> to an unsigned <see cref="UInt64"/>.
        /// </summary>
        /// <param name="intPtr">The pointer to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static UInt64 ToUInt64(this IntPtr intPtr)
        {
            return unchecked((UInt64)(Int64)intPtr);
        }

        /// <summary>
        /// Converts an <see cref="UIntPtr"/> to an unsigned <see cref="UInt64"/>.
        /// </summary>
        /// <param name="intPtr">The pointer to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static UInt64 ToUInt64(this UIntPtr intPtr)
        {
            return unchecked((UInt64)intPtr);
        }
    }
    //// End class
}
//// End namespace