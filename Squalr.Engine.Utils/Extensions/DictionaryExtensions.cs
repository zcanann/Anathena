namespace Squalr.Engine.Common.Extensions
{
    using System.Collections.Generic;

    /// <summary>
    /// Extension methods for Dictionaries.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Adds the specified elements to the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="source">The source dictionary.</param>
        /// <param name="newItems">The elements to add.</param>
        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> source, params KeyValuePair<TKey, TValue>[] newItems)
        {
            foreach (KeyValuePair<TKey, TValue> element in newItems)
            {
                source.Add(element.Key, element.Value);
            }
        }
    }
    //// End class
}
//// End namespace