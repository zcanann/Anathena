namespace Squalr.Engine.Common.Extensions
{
    using System.Collections.Generic;

    /// <summary>
    /// A class that enables circular linked list functions, over a normal linked list.
    /// </summary>
    public static class LinkedListExtensions
    {
        /// <summary>
        /// Gets the next node in the linked list. If none is present, this will return the first.
        /// </summary>
        /// <typeparam name="T">The data type contained in the linked list.</typeparam>
        /// <param name="current">The node of which we are taking the next.</param>
        /// <returns>The next node in the circular linked list.</returns>
        public static LinkedListNode<T> NextOrFirst<T>(this LinkedListNode<T> current)
        {
            return current.Next ?? current.List?.First;
        }

        /// <summary>
        /// Gets the previous node in the linked list. If none is present, this will return the first.
        /// </summary>
        /// <typeparam name="T">The data type contained in the linked list.</typeparam>
        /// <param name="current">The node of which we are taking the previous.</param>
        /// <returns>The previous node in the circular linked list.</returns>
        public static LinkedListNode<T> PreviousOrLast<T>(this LinkedListNode<T> current)
        {
            return current.Previous ?? current.List?.Last;
        }
    }
    //// End class
}
//// End namespace