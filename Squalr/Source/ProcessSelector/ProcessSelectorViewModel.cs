namespace Squalr.Source.ProcessSelector
{
    using GalaSoft.MvvmLight.Command;
    using Squalr.Content;
    using Squalr.Engine.Processes;
    using Squalr.Engine.Common.Extensions;
    using Squalr.Source.Docking;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// View model for the Process Selector.
    /// </summary>
    public class ProcessSelectorViewModel : ToolViewModel, IProcessObserver
    {
        /// <summary>
        /// Singleton instance of the <see cref="ProcessSelectorViewModel" /> class.
        /// </summary>
        private static Lazy<ProcessSelectorViewModel> processSelectorViewModelInstance = new Lazy<ProcessSelectorViewModel>(
                () => { return new ProcessSelectorViewModel(); },
                LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// A dummy process that detaches from the target process when selected.
        /// </summary>
        private NormalizedProcess detachProcess;

        /// <summary>
        /// The selected process.
        /// </summary>
        private NormalizedProcess selectedProcess;

        /// <summary>
        /// The list of running processes.
        /// </summary>
        private IEnumerable<NormalizedProcess> processList;

        /// <summary>
        /// Prevents a default instance of the <see cref="ProcessSelectorViewModel" /> class from being created.
        /// </summary>
        private ProcessSelectorViewModel() : base("Process Selector")
        {
            this.IconSource = Images.SelectProcess;
            this.RefreshProcessListCommand = new RelayCommand(() => Task.Run(() => this.RefreshProcessList()), () => true);
            this.SelectProcessCommand = new RelayCommand<NormalizedProcess>((process) => Task.Run(() => this.SelectProcess(process)), (process) => true);
            this.detachProcess = new NormalizedProcess("-- Detach from Process --");

            this.StartAutomaticProcessListRefresh();

            DockingViewModel.GetInstance().RegisterViewModel(this);

            // Subscribe to process events
            Processes.Default.Subscribe(this);
        }

        /// <summary>
        /// Gets the command to refresh the process list.
        /// </summary>
        public ICommand RefreshProcessListCommand { get; private set; }

        /// <summary>
        /// Gets the command to select a target process.
        /// </summary>
        public ICommand SelectProcessCommand { get; private set; }

        /// <summary>
        /// Gets or sets the list of processes running on the machine.
        /// </summary>
        public IEnumerable<NormalizedProcess> ProcessList
        {
            get
            {
                return this.processList;
            }

            set
            {
                this.processList = value;

                this.RaisePropertyChanged(nameof(this.ProcessList));
                this.RaisePropertyChanged(nameof(this.WindowedProcessList));
            }
        }

        /// <summary>
        /// Gets the processes with a window running on the machine, as well as the selected process.
        /// </summary>
        public IEnumerable<NormalizedProcess> WindowedProcessList
        {
            get
            {
                // Process list with the selected process at the top, and a detach option as the 2nd element
                return this.ProcessList?.Where(process => process.HasWindow && (process?.ProcessId ?? 0) != (this.SelectedProcess?.ProcessId ?? 0)).Select(process => process)
                    .PrependIfNotNull(this.SelectedProcess != null ? this.DetachProcess : null)
                    .PrependIfNotNull(this.SelectedProcess)
                    .Distinct();
            }
        }

        /// <summary>
        /// Gets or sets the selected process.
        /// </summary>
        public NormalizedProcess SelectedProcess
        {
            get
            {
                return this.selectedProcess;
            }

            set
            {
                if (value == this.DetachProcess)
                {
                    this.selectedProcess = null;
                    Processes.Default.OpenedProcess = null;
                    this.RaisePropertyChanged(nameof(this.WindowedProcessList));
                }
                else if (value != this.SelectedProcess)
                {
                    this.selectedProcess = value;
                    Processes.Default.OpenedProcess = value.Process;
                    this.RaisePropertyChanged(nameof(this.SelectedProcess));
                    this.RaisePropertyChanged(nameof(this.WindowedProcessList));
                }
            }
        }

        /// <summary>
        /// Gets or sets a dummy process that detaches from the target process when selected.
        /// </summary>
        public NormalizedProcess DetachProcess
        {
            get
            {
                return this.detachProcess;
            }

            set
            {
                this.detachProcess = value;
                this.RaisePropertyChanged(nameof(this.DetachProcess));
            }
        }

        /// <summary>
        /// Gets the name of the selected process.
        /// </summary>
        public String ProcessName
        {
            get
            {
                String processName = Processes.Default.OpenedProcess.ProcessName;
                return String.IsNullOrEmpty(processName) ? "Please Select a Process" : processName;
            }
        }

        /// <summary>
        /// Gets a singleton instance of the <see cref="ProcessSelectorViewModel"/> class.
        /// </summary>
        /// <returns>A singleton instance of the class.</returns>
        public static ProcessSelectorViewModel GetInstance()
        {
            return processSelectorViewModelInstance.Value;
        }

        /// <summary>
        /// Recieves a process update.
        /// </summary>
        /// <param name="process">The newly selected process.</param>>
        public void Update(Process process)
        {
            // Raise event to update process name in the view
            this.RaisePropertyChanged(nameof(this.ProcessName));

            this.RefreshProcessList();
        }

        /// <summary>
        /// Called when the visibility of this tool is changed.
        /// </summary>
        protected override void OnVisibilityChanged()
        {
            if (this.IsVisible)
            {
                this.RefreshProcessList();
            }
        }

        /// <summary>
        /// Makes the target process selection.
        /// </summary>
        /// <param name="process">The process being selected.</param>
        private void SelectProcess(NormalizedProcess process)
        {
            this.SelectedProcess = process;
            this.IsVisible = false;
        }

        private void StartAutomaticProcessListRefresh()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    this.RefreshProcessList();
                    await Task.Delay(5000);
                }
            });
        }

        /// <summary>
        /// Refreshes the process list.
        /// </summary>
        private void RefreshProcessList()
        {
            this.ProcessList = Processes.Default.GetProcesses().Select(process => new NormalizedProcess(process));
        }
    }
    //// End class
}
//// End namespace