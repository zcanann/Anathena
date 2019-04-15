namespace Squalr.Engine.Common.DataStructures
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;

    /// <summary>
    /// A collection of items for which property change events are observed. Fixes the poor .NET platform ObservableCollection implementation.
    /// </summary>
    /// <typeparam name="T">The type of the items contained in this collection.</typeparam>
    public class FullyObservableCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FullyObservableCollection{T}" /> class.
        /// </summary>
        public FullyObservableCollection() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FullyObservableCollection{T}" /> class.
        /// </summary>
        /// <param name="items">The initial items in the observable collection.</param>
        public FullyObservableCollection(List<T> items) : base(items)
        {
            this.ObserveAll();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FullyObservableCollection{T}" /> class.
        /// </summary>
        /// <param name="items">The initial items in the observable collection.</param>
        public FullyObservableCollection(IEnumerable<T> items) : base(items)
        {
            this.ObserveAll();
        }

        /// <summary>
        /// Occurs when a property is changed within an item.
        /// </summary>
        public event EventHandler<ItemPropertyChangedEventArgs> ItemPropertyChanged;

        /// <summary>
        /// Registers or unregisters items from observer events.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (T item in e.OldItems)
                {
                    item.PropertyChanged -= ChildPropertyChanged;
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (T item in e.NewItems)
                {
                    item.PropertyChanged += ChildPropertyChanged;
                }
            }

            base.OnCollectionChanged(e);
        }

        /// <summary>
        /// Event fired when an item property is changed.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected void OnItemPropertyChanged(ItemPropertyChangedEventArgs e)
        {
            this.ItemPropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Event fired when an item property is changed.
        /// </summary>
        /// <param name="index">The item index.</param>
        /// <param name="e">The event args.</param>
        protected void OnItemPropertyChanged(Int32 index, PropertyChangedEventArgs e)
        {
            this.OnItemPropertyChanged(new ItemPropertyChangedEventArgs(index, e));
        }

        /// <summary>
        /// Removes all items from this collection, and stops observing them.
        /// </summary>
        protected override void ClearItems()
        {
            foreach (T item in Items)
            {
                item.PropertyChanged -= ChildPropertyChanged;
            }

            base.ClearItems();
        }

        /// <summary>
        /// Observes changes in all items in the collection.
        /// </summary>
        private void ObserveAll()
        {
            foreach (T item in Items)
            {
                item.PropertyChanged += ChildPropertyChanged;
            }
        }

        /// <summary>
        /// Event fired when property changed notifier event raised in a child item.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event args.</param>
        private void ChildPropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            T typedSender = (T)sender;
            Int32 index = Items.IndexOf(typedSender);

            if (index < 0)
            {
                throw new ArgumentException("Received property notification from item not in collection");
            }

            this.OnItemPropertyChanged(index, e);
        }

        /// <summary>
        /// Provides data for the <see cref="FullyObservableCollection{T}.ItemPropertyChanged"/> event.
        /// </summary>
        public class ItemPropertyChangedEventArgs : PropertyChangedEventArgs
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ItemPropertyChangedEventArgs"/> class.
            /// </summary>
            /// <param name="index">The index in the collection of changed item.</param>
            /// <param name="name">The name of the property that changed.</param>
            public ItemPropertyChangedEventArgs(Int32 index, String name) : base(name)
            {
                CollectionIndex = index;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ItemPropertyChangedEventArgs"/> class.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <param name="args">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
            public ItemPropertyChangedEventArgs(Int32 index, PropertyChangedEventArgs args) : this(index, args.PropertyName)
            {
            }

            /// <summary>
            /// Gets the index in the collection for which the property change has occurred.
            /// </summary>
            /// <value>
            /// Index in parent collection.
            /// </value>
            public Int32 CollectionIndex { get; }
        }
    }
    //// End class
}
//// End namespace