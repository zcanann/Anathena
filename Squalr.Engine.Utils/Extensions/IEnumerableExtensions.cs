namespace Squalr.Engine.Common.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods for IEnumerable interfaces.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Determines if a collection is null or empty.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="enumeration">The enumeration to iterate through.</param>
        /// <returns>True if the collection is null or empty, otherwise false.</returns>
        public static Boolean IsNullOrEmpty<T>(this IEnumerable<T> enumeration)
        {
            return enumeration == null || !enumeration.Any();
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();

            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static Stack<T> Clone<T>(this Stack<T> stack)
        {
            return new Stack<T>(stack.Reverse());
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.ShuffleIterator(new Random());
        }

        private static IEnumerable<T> ShuffleIterator<T>(this IEnumerable<T> source, Random random)
        {
            IList<T> buffer = source.ToList();

            for (Int32 index = 0; index < buffer.Count; index++)
            {
                Int32 j = random.Next(index, buffer.Count);

                yield return buffer[j];

                buffer[j] = buffer[index];
            }
        }

        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, Int32 size)
        {
            T[] bucket = null;
            Int32 count = 0;

            foreach (T item in source)
            {
                if (bucket == null)
                {
                    bucket = new T[size];
                }

                bucket[count++] = item;

                if (count != size)
                {
                    continue;
                }

                yield return bucket.Select(x => x);

                bucket = null;
                count = 0;
            }

            // Return the last bucket with all remaining elements
            if (bucket != null && count > 0)
            {
                yield return bucket.Take(count);
            }
        }

        public static UInt64 Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, UInt64> summation)
        {
            UInt64 total = 0;

            foreach (TSource item in source)
            {
                total += summation(item);
            }

            return total;
        }

        public static IEnumerable<T> SoftClone<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Select(item => (T)item).ToArray();
        }

        /// <summary>
        /// A foreach extension method to perform an action on all elements in an enumeration.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="enumeration">The enumeration to iterate through.</param>
        /// <param name="action">The action to perform for each item.</param>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }

            return enumeration;
        }

        /// <summary>
        /// Gets the type contained in the IEnumerable interface.
        /// </summary>
        /// <param name="source">The enumeration of which we will get the child type.</param>
        /// <returns>A type contained in the IEnumerable interface. Returns null if none found.</returns>
        public static Type GetElementType(this IEnumerable source)
        {
            Type enumerableType = source.GetType();

            if (enumerableType.IsArray)
            {
                return enumerableType.GetElementType();
            }

            if (enumerableType.IsGenericType)
            {
                return enumerableType.GetGenericArguments().First();
            }

            return null;
        }

        /// <summary>
        /// Adds a single element to the end of an IEnumerable, if it is not null.
        /// </summary>
        /// <typeparam name="T">Type of enumerable to return.</typeparam>
        /// <param name="source">The source enumerable.</param>
        /// <param name="element">The element to append.</param>
        /// <returns>IEnumerable containing all the input elements, followed by the specified additional element.</returns>
        public static IEnumerable<T> AppendIfNotNull<T>(this IEnumerable<T> source, T element)
        {
            if (element == null)
            {
                return source;
            }

            return source.Append(element);
        }

        /// <summary>
        /// Adds a single element to the start of an IEnumerable, if it is not null.
        /// </summary>
        /// <typeparam name="T">Type of enumerable to return.</typeparam>
        /// <param name="tail">The source enumerable.</param>
        /// <param name="head">The element to prepend.</param>
        /// <returns>IEnumerable containing the specified additional element, followed by all the input elements.</returns>
        public static IEnumerable<T> PrependIfNotNull<T>(this IEnumerable<T> tail, T head)
        {
            if (head == null)
            {
                return tail;
            }

            return tail.Prepend<T>(head);
        }

        /// <summary>
        /// Concatenation iterater to allow appending and prepending information.
        /// </summary>
        /// <typeparam name="T">Type of enumerable to return.</typeparam>
        /// <param name="extraElement">The element being added.</param>
        /// <param name="source">The IEnumerable source.</param>
        /// <param name="insertAtStart">Whether or not to append or prepend.</param>
        /// <returns>IEnumerable with all source elements and the new one either appended or prepended.</returns>
        private static IEnumerable<T> ConcatIterator<T>(T extraElement, IEnumerable<T> source, Boolean insertAtStart)
        {
            if (insertAtStart)
            {
                yield return extraElement;
            }

            foreach (T e in source)
            {
                yield return e;
            }

            if (!insertAtStart)
            {
                yield return extraElement;
            }
        }
    }
    //// End class
}
//// End namespace