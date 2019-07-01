namespace Squalr.Source.Scanning
{
    using GalaSoft.MvvmLight.Command;
    using Squalr.Engine.Common.DataStructures;
    using Squalr.Engine.Input.HotKeys;
    using Squalr.Source.Docking;
    using Squalr.Source.Editors.HotkeyEditor;
    using Squalr.View;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// View model for the Input Correlator.
    /// </summary>
    internal class InputCorrelatorViewModel : ToolViewModel
    {
        /// <summary>
        /// Singleton instance of the <see cref="InputCorrelatorViewModel" /> class.
        /// </summary>
        private static Lazy<InputCorrelatorViewModel> inputCorrelatorViewModelInstance = new Lazy<InputCorrelatorViewModel>(
                () => { return new InputCorrelatorViewModel(); },
                LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Prevents a default instance of the <see cref="InputCorrelatorViewModel" /> class from being created.
        /// </summary>
        private InputCorrelatorViewModel() : base("Input Correlator")
        {
            this.NewHotkeyCommand = new RelayCommand(() => this.NewHotkey(), () => true);
            this.RemoveHotkeyCommand = new RelayCommand<Hotkey>((hotkey) => this.RemoveHotkey(hotkey), (hotkey) => true);
            this.StartScanCommand = new RelayCommand(() => Task.Run(() => this.StartScan()), () => true);
            this.StopScanCommand = new RelayCommand(() => Task.Run(() => this.StopScan()), () => true);
            this.InputCorrelator = null; // new InputCorrelator(this.ScanCountUpdated);

            DockingViewModel.GetInstance().RegisterViewModel(this);
        }

        public ICommand StartScanCommand { get; private set; }

        public ICommand StopScanCommand { get; private set; }

        public ICommand NewHotkeyCommand { get; private set; }

        public ICommand RemoveHotkeyCommand { get; private set; }

        public FullyObservableCollection<Hotkey> Hotkeys
        {
            get
            {
                throw new NotImplementedException();
               //// return this.InputCorrelator.HotKeys;
            }
        }

        public Int32 ScanCount
        {
            get
            {
                throw new NotImplementedException();
                //// return this.InputCorrelator.ScanCount;
            }
        }

        private InputCorrelator InputCorrelator { get; set; }

        /// <summary>
        /// Gets a singleton instance of the <see cref="InputCorrelatorViewModel"/> class.
        /// </summary>
        /// <returns>A singleton instance of the class.</returns>
        public static InputCorrelatorViewModel GetInstance()
        {
            return inputCorrelatorViewModelInstance.Value;
        }

        private void NewHotkey()
        {
            HotkeyEditorModel hotkeyEditor = new HotkeyEditorModel();
            Hotkey newHotkey = hotkeyEditor.EditValue(context: null, provider: null, value: null) as Hotkey;

            if (newHotkey != null)
            {
                throw new NotImplementedException();
                ////this.InputCorrelator.HotKeys.Add(newHotkey);
                this.RaisePropertyChanged(nameof(this.Hotkeys));
            }
        }

        private void RemoveHotkey(Hotkey hotkey)
        {
            throw new NotImplementedException();
            //// this.InputCorrelator.HotKeys.Remove(hotkey);
            this.RaisePropertyChanged(nameof(this.Hotkeys));
        }

        private void ScanCountUpdated()
        {
            this.RaisePropertyChanged(nameof(this.ScanCount));
        }

        private void StartScan()
        {
            //// this.InputCorrelator.Start();
        }

        private void StopScan()
        {
            //// this.InputCorrelator.Cancel();
        }
    }
    //// End class
}
//// End namespace