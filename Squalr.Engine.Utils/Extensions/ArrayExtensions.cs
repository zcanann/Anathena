namespace Squalr.Engine.Common.Extensions
{
    using System;

    /// <summary>
    /// Extension methods for arrays.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Returns the specified subarray.
        /// </summary>
        /// <typeparam name="T">The data type contained in the array.</typeparam>
        /// <param name="arrayA">The array in consideration.</param>
        /// <param name="index">The start index to take the subarray.</param>
        /// <param name="length">The size of the subarray.</param>
        /// <returns>Returns the specified subarray. Returns null if the specified index is out of bounds.</returns>
        public static T[] SubArray<T>(this T[] arrayA, Int32 index, Int32 length)
        {
            if (arrayA == null || arrayA.Length - index < length)
            {
                return null;
            }

            T[] result = new T[length];
            Array.Copy(arrayA, index, result, 0, length);

            return result;
        }

        /// <summary>
        /// Returns the specified subarray. Attempts to return the largest possible subarray if the specified length is out of bounds.
        /// </summary>
        /// <typeparam name="T">The data type contained in the array.</typeparam>
        /// <param name="arrayA">The array in consideration.</param>
        /// <param name="index">The start index to take the subarray.</param>
        /// <param name="length">The size of the subarray.</param>
        /// <returns>Returns the specified subarray. Returns null if the specified index is out of bounds.</returns>
        public static T[] LargestSubArray<T>(this T[] arrayA, Int64 index, Int64 length)
        {
            if (arrayA == null)
            {
                return null;
            }

            if (arrayA.Length - index < length)
            {
                length = arrayA.Length - index;
            }

            if (length <= 0)
            {
                return null;
            }

            T[] result = new T[length];
            Array.Copy(arrayA, index, result, 0, length);

            return result;
        }

        /// <summary>
        /// Combines two arrays into one array.
        /// </summary>
        /// <typeparam name="T">The data type contained in the arrays.</typeparam>
        /// <param name="arrayA">The first array to combine.</param>
        /// <param name="arrayB">The second array to combine.</param>
        /// <returns>An array containing the combined elements from the two arrays, or null if the combination fails.</returns>
        public static T[] Concat<T>(this T[] arrayA, T[] arrayB)
        {
            if (arrayA == null && arrayB == null)
            {
                return null;
            }

            if (arrayA == null)
            {
                return arrayB;
            }

            if (arrayB == null)
            {
                return arrayA;
            }

            Int32 oldLength = arrayA.Length;
            Array.Resize<T>(ref arrayA, arrayA.Length + arrayB.Length);
            Array.Copy(arrayB, 0, arrayA, oldLength, arrayB.Length);

            return arrayA;
        }
    }
    //// End class
}
//// End namespace