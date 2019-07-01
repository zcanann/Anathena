namespace Squalr.Source.Editors.OffsetEditor
{
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using Squalr.Engine.Common.DataStructures;
    using Squalr.Source.Mvvm;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// View model for the Offset Editor.
    /// </summary>
    public class OffsetEditorViewModel : ViewModelBase
    {
        /// <summary>
        /// The collection of offsets.
        /// </summary>
        private IList<PrimitiveBinding<Int32>> offsets;

        /// <summary>
        /// Singleton instance of the <see cref="OffsetEditorViewModel" /> class.
        /// </summary>
        private static Lazy<OffsetEditorViewModel> offsetEditorViewModelInstance = new Lazy<OffsetEditorViewModel>(
                () => { return new OffsetEditorViewModel(); },
                LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetEditorViewModel" /> class.
        /// </summary>
        public OffsetEditorViewModel()
        {
            this.AddOffsetCommand = new RelayCommand(() => Task.Run(() => this.AddOffset()), () => true);
            this.RemoveOffsetCommand = new RelayCommand(() => Task.Run(() => this.RemoveSelectedOffset()), () => true);
            this.UpdateActiveValueCommand = new RelayCommand<Int32>((offset) => Task.Run(() => this.UpdateActiveValue(offset)), (offset) => true);
            this.AccessLock = new Object();
            this.Offsets = new FullyObservableCollection<PrimitiveBinding<Int32>>();
        }

        /// <summary>
        /// Gets a command to add an offset to the offset list.
        /// </summary>
        public ICommand AddOffsetCommand { get; private set; }

        /// <summary>
        /// Gets a command to remove an offset from the offset list.
        /// </summary>
        public ICommand RemoveOffsetCommand { get; private set; }

        /// <summary>
        /// Gets a command to update the active offset value.
        /// </summary>
        public ICommand UpdateActiveValueCommand { get; private set; }

        /// <summary>
        /// Gets or sets the collection of offsets.
        /// </summary>
        public FullyObservableCollection<PrimitiveBinding<Int32>> Offsets
        {
            get
            {
                lock (this.AccessLock)
                {
                    if (this.offsets == null)
                    {
                        this.offsets = new List<PrimitiveBinding<Int32>>();
                    }

                    return new FullyObservableCollection<PrimitiveBinding<Int32>>(this.offsets);
                }
            }

            set
            {
                lock (this.AccessLock)
                {
                    this.offsets = value == null ? new List<PrimitiveBinding<Int32>>() : new List<PrimitiveBinding<Int32>>(value);
                    this.RaisePropertyChanged(nameof(this.Offsets));
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the selected offset.
        /// </summary>
        public Int32 SelectedOffsetIndex { get; set; }

        /// <summary>
        /// Gets or sets the offset list lock.
        /// </summary>
        private Object AccessLock { get; set; }

        /// <summary>
        /// Gets or sets the active offset value.
        /// </summary>
        private Int32 ActiveOffsetValue { get; set; }

        /// <summary>
        /// Gets a singleton instance of the <see cref="OffsetEditorViewModel" /> class.
        /// </summary>
        /// <returns>A singleton instance of the class.</returns>
        public static OffsetEditorViewModel GetInstance()
        {
            return OffsetEditorViewModel.offsetEditorViewModelInstance.Value;
        }

        /// <summary>
        /// Adds the currently selected offset to the offset list.
        /// </summary>
        private void AddOffset()
        {
            lock (this.AccessLock)
            {
                this.offsets.Add(new PrimitiveBinding<Int32>(this.ActiveOffsetValue));
            }

            this.RaisePropertyChanged(nameof(this.Offsets));
        }

        /// <summary>
        /// Removes the currently selected offset from the offset list.
        /// </summary>
        private void RemoveSelectedOffset()
        {
            Int32 removalIndex = this.SelectedOffsetIndex;

            lock (this.AccessLock)
            {
                if (removalIndex < 0)
                {
                    removalIndex = 0;
                }

                if (removalIndex < this.offsets.Count)
                {
                    this.offsets.RemoveAt(removalIndex);
                }
            }

            this.RaisePropertyChanged(nameof(this.Offsets));
        }

        /// <summary>
        /// Updates the active offset value.
        /// </summary>
        /// <param name="offset">The active offset value.</param>
        private void UpdateActiveValue(Int32 offset)
        {
            this.ActiveOffsetValue = offset;
        }
    }
    //// End class
}
//// End namespace