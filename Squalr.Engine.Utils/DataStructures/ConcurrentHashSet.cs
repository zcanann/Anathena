namespace Squalr.Engine.Common.DataStructures
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// A hash set data structure that can handle multithreaded access.
    /// </summary>
    /// <typeparam name="T">The type contained in the hash set.</typeparam>
    public class ConcurrentHashSet<T> : IEnumerable, IDisposable
    {
        /// <summary>
        /// Locking object for access to the data contained in this structure.
        /// </summary>
        private readonly ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        /// <summary>
        /// The actual underlying hashset that this object interfaces over.
        /// </summary>
        private readonly HashSet<T> hashSet = new HashSet<T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentHashSet{T}" /> class.
        /// </summary>
        public ConcurrentHashSet()
        {
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ConcurrentHashSet{T}" /> class.
        /// </summary>
        ~ConcurrentHashSet()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the number of items contained in the hash set.
        /// </summary>
        public Int32 Count
        {
            get
            {
                this.lockSlim.EnterReadLock();

                try
                {
                    return this.hashSet.Count;
                }
                finally
                {
                    if (this.lockSlim.IsReadLockHeld)
                    {
                        this.lockSlim.ExitReadLock();
                    }
                }
            }
        }

        /// <summary>
        /// Adds a new item to the hash set.
        /// </summary>
        /// <param name="item">The item to add to the hash set.</param>
        /// <returns>Returns the result of the add operation, which can succeed or fail.</returns>
        public Boolean Add(T item)
        {
            this.lockSlim.EnterWriteLock();

            try
            {
                return this.hashSet.Add(item);
            }
            finally
            {
                if (this.lockSlim.IsWriteLockHeld)
                {
                    this.lockSlim.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// Removes a new item to the hash set.
        /// </summary>
        /// <param name="item">The item to remove to the hash set.</param>
        /// <returns>Returns the result of the remove operation, which can succeed or fail.</returns>
        public Boolean Remove(T item)
        {
            this.lockSlim.EnterWriteLock();

            try
            {
                return this.hashSet.Remove(item);
            }
            finally
            {
                if (this.lockSlim.IsWriteLockHeld)
                {
                    this.lockSlim.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// Clears all items from the hash set.
        /// </summary>
        public void Clear()
        {
            this.lockSlim.EnterWriteLock();

            try
            {
                this.hashSet.Clear();
            }
            finally
            {
                if (this.lockSlim.IsWriteLockHeld)
                {
                    this.lockSlim.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// Determines if an item is contained in the hash set.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>Whether or not the item is contained in this hash set.</returns>
        public Boolean Contains(T item)
        {
            this.lockSlim.EnterReadLock();

            try
            {
                return this.hashSet.Contains(item);
            }
            finally
            {
                if (this.lockSlim.IsReadLockHeld)
                {
                    this.lockSlim.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Disposes of this object and all managed resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets an enumerator to the underlying hash set.
        /// </summary>
        /// <returns>An enumerator to the underlying hash set.</returns>
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)this.hashSet).GetEnumerator();
        }

        /// <summary>
        /// Converts the concurrent hash set to a list.
        /// </summary>
        /// <returns>The collection as a list.</returns>
        public List<T> ToList()
        {
            return this.hashSet.ToList();
        }

        /// <summary>
        /// Disposes of this object and all managed resources.
        /// </summary>
        /// <param name="disposing">Whether or not to dispose managed resources.</param>
        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                if (this.lockSlim != null)
                {
                    this.lockSlim.Dispose();
                }
            }
        }
    }
    //// End class
}
//// End namespace