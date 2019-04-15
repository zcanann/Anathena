namespace Squalr.Engine.Common.DataStructures
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// A bidirectional dictionary supporting a mapping of two types.
    /// </summary>
    /// <typeparam name="TFirst">The first type.</typeparam>
    /// <typeparam name="TSecond">The second type.</typeparam>
    [Serializable]
    public class BiDictionary<TFirst, TSecond> : IDictionary<TFirst, TSecond>, IReadOnlyDictionary<TFirst, TSecond>, IDictionary
    {
        /// <summary>
        /// The dictionary mapping from the first type to the second type.
        /// </summary>
        private readonly IDictionary<TFirst, TSecond> firstToSecond;

        /// <summary>
        /// The dictionary mapping from the second type to the first type.
        /// </summary>
        [NonSerialized]
        private readonly IDictionary<TSecond, TFirst> secondToFirst;

        /// <summary>
        /// Initializes a new instance of the <see cref="BiDictionary{TFirst,TSecond}" /> class.
        /// </summary>
        public BiDictionary()
        {
            this.firstToSecond = new Dictionary<TFirst, TSecond>();
            this.secondToFirst = new Dictionary<TSecond, TFirst>();
        }

        /// <summary>
        /// Gets the number of elements contained in this dictionary.
        /// </summary>
        public Int32 Count
        {
            get { return this.firstToSecond.Count; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to this dictionary.
        /// </summary>
        Object ICollection.SyncRoot
        {
            get { return ((ICollection)this.firstToSecond).SyncRoot; }
        }

        /// <summary>
        /// Gets a value indicating whether access to this dictionary is synchronized (thread safe).
        /// </summary>
        Boolean ICollection.IsSynchronized
        {
            get { return ((ICollection)this.firstToSecond).IsSynchronized; }
        }

        /// <summary>
        /// Gets a value indicating whether this dictionary has a fixed size.
        /// </summary>
        Boolean IDictionary.IsFixedSize
        {
            get { return ((IDictionary)this.firstToSecond).IsFixedSize; }
        }

        /// <summary>
        /// Gets a value indicating whether this dictionary is read only.
        /// </summary>
        public Boolean IsReadOnly
        {
            get { return this.firstToSecond.IsReadOnly || this.secondToFirst.IsReadOnly; }
        }

        /// <summary>
        /// Gets a collection containing the keys of this dictionary.
        /// </summary>
        public ICollection<TFirst> Keys
        {
            get { return this.firstToSecond.Keys; }
        }

        /// <summary>
        /// Gets a collection containing the keys of this dictionary.
        /// </summary>
        ICollection IDictionary.Keys
        {
            get { return ((IDictionary)this.firstToSecond).Keys; }
        }

        /// <summary>
        /// Gets a collection containing the keys of this dictionary.
        /// </summary>
        IEnumerable<TFirst> IReadOnlyDictionary<TFirst, TSecond>.Keys
        {
            get { return ((IReadOnlyDictionary<TFirst, TSecond>)this.firstToSecond).Keys; }
        }

        /// <summary>
        /// Gets a collection containing the values of this dictionary.
        /// </summary>
        public ICollection<TSecond> Values
        {
            get { return this.firstToSecond.Values; }
        }

        /// <summary>
        /// Gets a collection containing the values of this dictionary.
        /// </summary>
        ICollection IDictionary.Values
        {
            get { return ((IDictionary)this.firstToSecond).Values; }
        }

        /// <summary>
        /// Gets a collection containing the values of this dictionary.
        /// </summary>
        IEnumerable<TSecond> IReadOnlyDictionary<TFirst, TSecond>.Values
        {
            get { return ((IReadOnlyDictionary<TFirst, TSecond>)this.firstToSecond).Values; }
        }

        /// <summary>
        /// Indexer to set or get the value of the specified key.
        /// </summary>
        /// <param name="key">The dictionary key.</param>
        /// <returns>The value at the specified key.</returns>
        public TSecond this[TFirst key]
        {
            get
            {
                return this.firstToSecond[key];
            }

            set
            {
                this.firstToSecond[key] = value;
                this.secondToFirst[value] = key;
            }
        }

        /// <summary>
        /// Indexer to set or get the value of the specified key.
        /// </summary>
        /// <param name="key">The dictionary key.</param>
        /// <returns>The value at the specified key.</returns>
        Object IDictionary.this[Object key]
        {
            get
            {
                return ((IDictionary)this.firstToSecond)[key];
            }

            set
            {
                ((IDictionary)this.firstToSecond)[key] = value;
                ((IDictionary)this.secondToFirst)[value] = key;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that iterates through the collection.</returns>
        public IEnumerator<KeyValuePair<TFirst, TSecond>> GetEnumerator()
        {
            return this.firstToSecond.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that iterates through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that iterates through the collection.</returns>
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)this.firstToSecond).GetEnumerator();
        }

        /// <summary>
        /// Adds an element with the provided key to the dictionary.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        public void Add(TFirst key, TSecond value)
        {
            this.firstToSecond.Add(key, value);
            this.secondToFirst.Add(value, key);
        }

        /// <summary>
        /// Adds an element with the provided key to the dictionary.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        void IDictionary.Add(Object key, Object value)
        {
            ((IDictionary)this.firstToSecond).Add(key, value);
            ((IDictionary)this.secondToFirst).Add(value, key);
        }

        /// <summary>
        /// Adds an element with the provided key to the dictionary.
        /// </summary>
        /// <param name="item">The key value pair to add.</param>
        void ICollection<KeyValuePair<TFirst, TSecond>>.Add(KeyValuePair<TFirst, TSecond> item)
        {
            this.firstToSecond.Add(item);
            this.secondToFirst.Add(new KeyValuePair<TSecond, TFirst>(item.Value, item.Key));
        }

        /// <summary>
        /// Determines whether this dictionary contains an element with the specified key.
        /// </summary>
        /// <param name="key">The dictionary key.</param>
        /// <returns>Returns true if found, otherwise false.</returns>
        public Boolean ContainsKey(TFirst key)
        {
            return this.firstToSecond.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether this dictionary contains an element with the specified key.
        /// </summary>
        /// <param name="key">The dictionary key.</param>
        /// <returns>Returns true if found, otherwise false.</returns>
        Boolean IDictionary.Contains(Object key)
        {
            return ((IDictionary)this.firstToSecond).Contains(key);
        }

        /// <summary>
        /// Determines whether this dictionary contains a specific value.
        /// </summary>
        /// <param name="item">The key value pair.</param>
        /// <returns>Returns true if found, otherwise false.</returns>
        Boolean ICollection<KeyValuePair<TFirst, TSecond>>.Contains(KeyValuePair<TFirst, TSecond> item)
        {
            return this.firstToSecond.Contains(item);
        }

        /// <summary>
        /// Gets a value associated with the specified key.
        /// </summary>
        /// <param name="key">The dictionary key.</param>
        /// <param name="value">The dictionary value.</param>
        /// <returns>True if retrieval succeeded, otherwise false.</returns>
        public Boolean TryGetValue(TFirst key, out TSecond value)
        {
            return this.firstToSecond.TryGetValue(key, out value);
        }

        /// <summary>
        /// Removes the element with the specified key from this dictionary.
        /// </summary>
        /// <param name="key">The dictionary key.</param>
        /// <returns>True if removal succeeded, otherwise false.</returns>
        public Boolean Remove(TFirst key)
        {
            TSecond value;

            if (this.firstToSecond.TryGetValue(key, out value))
            {
                this.firstToSecond.Remove(key);
                this.secondToFirst.Remove(value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the element with the specified key from this dictionary.
        /// </summary>
        /// <param name="key">The dictionary key.</param>
        void IDictionary.Remove(Object key)
        {
            IDictionary firstToSecond = (IDictionary)this.firstToSecond;

            if (!firstToSecond.Contains(key))
            {
                return;
            }

            Object value = firstToSecond[key];
            firstToSecond.Remove(key);
            ((IDictionary)this.secondToFirst).Remove(value);
        }

        /// <summary>
        /// Removes the first occurence of the specific object from this dictionary.
        /// </summary>
        /// <param name="item">The dictionary key value pair.</param>
        /// <returns>True if removal succeeded, otherwise false.</returns>
        Boolean ICollection<KeyValuePair<TFirst, TSecond>>.Remove(KeyValuePair<TFirst, TSecond> item)
        {
            return this.firstToSecond.Remove(item);
        }

        /// <summary>
        /// Removes all items from this dictionary.
        /// </summary>
        public void Clear()
        {
            this.firstToSecond.Clear();
            this.secondToFirst.Clear();
        }

        /// <summary>
        /// Copies the elements from this dictionary to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The destination array for the copied elements.</param>
        /// <param name="arrayIndex">The zero-based index at which the copying begins.</param>
        void ICollection<KeyValuePair<TFirst, TSecond>>.CopyTo(KeyValuePair<TFirst, TSecond>[] array, Int32 arrayIndex)
        {
            this.firstToSecond.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Copies the elements from this dictionary to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The destination array for the copied elements.</param>
        /// <param name="index">The zero-based index at which the copying begins.</param>
        void ICollection.CopyTo(Array array, Int32 index)
        {
            ((IDictionary)this.firstToSecond).CopyTo(array, index);
        }

        /// <summary>
        /// Called when this object is deserialized to reconstruct this object.
        /// </summary>
        /// <param name="context">The streaming context.</param>
        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            this.secondToFirst.Clear();

            foreach (KeyValuePair<TFirst, TSecond> item in this.firstToSecond)
            {
                this.secondToFirst.Add(item.Value, item.Key);
            }
        }
    }
    //// End class
}
//// End namespace