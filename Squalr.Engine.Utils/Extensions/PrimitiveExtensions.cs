namespace Squalr.Engine.Common.Extensions
{
    using System;

    /// <summary>
    /// Extension methods for primitive data types.
    /// </summary>
    public static class PrimitiveExtensions
    {
        /// <summary>
        /// Bounds the value between the min and the maximum values provided.
        /// </summary>
        /// <typeparam name="T">Type data type of the value.</typeparam>
        /// <param name="self">The value in consideration.</param>
        /// <param name="min">The minimum value allowed.</param>
        /// <param name="max">The maximum value allowed.</param>
        /// <returns>The value clamped between the minimum and maximum values.</returns>
        public static T Clamp<T>(this T self, T min, T max) where T : IComparable<T>
        {
            if (self.CompareTo(min) < 0)
            {
                return min;
            }
            else if (self.CompareTo(max) > 0)
            {
                return max;
            }
            else
            {
                return self;
            }
        }
    }
    //// End calss
}
//// End namespace