namespace Squalr.Source.Debugger
{
    using GalaSoft.MvvmLight.Command;
    using Squalr.Engine.Debuggers;
    using Squalr.Engine.Projects.Items;
    using Squalr.Engine.Utils;
    using Squalr.Engine.Utils.DataStructures;
    using Squalr.Source.Docking;
    using Squalr.Source.ProjectExplorer;
    using Squalr.Source.ProjectExplorer.ProjectItems;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// View model for the Code Tracer.
    /// </summary>
    internal class CodeTracerViewModel : ToolViewModel
    {
        /// <summary>
        /// Singleton instance of the <see cref="CodeTracerViewModel" /> class.
        /// </summary>
        private static Lazy<CodeTracerViewModel> codeTracerViewModelInstance = new Lazy<CodeTracerViewModel>(
                () => { return new CodeTracerViewModel(); },
                LazyThreadSafetyMode.ExecutionAndPublication);

        private FullyObservableCollection<CodeTraceResult> results;

        /// <summary>
        /// The selected code trace results.
        /// </summary>
        private IEnumerable<CodeTraceResult> selectedCodeTraceResults;

        private CancellationTokenSource debuggerCancellationTokenSource;

        /// <summary>
        /// Prevents a default instance of the <see cref="CodeTracerViewModel" /> class from being created.
        /// </summary>
        private CodeTracerViewModel() : base("Code Tracer")
        {
            DockingViewModel.GetInstance().RegisterViewModel(this);

            this.Results = new FullyObservableCollection<CodeTraceResult>();

            this.FindWhatWritesCommand = new RelayCommand<ProjectItemView>((projectItem) => this.FindWhatWrites(projectItem));
            this.FindWhatReadsCommand = new RelayCommand<ProjectItemView>((projectItem) => this.FindWhatReads(projectItem));
            this.FindWhatAccessesCommand = new RelayCommand<ProjectItemView>((projectItem) => this.FindWhatAccesses(projectItem));
            this.StopTraceCommand = new RelayCommand(() => this.CancelTrace());
            this.SelectInstructionCommand = new RelayCommand<Object>((selectedItems) => this.SelectedCodeTraceResults = (selectedItems as IList)?.Cast<CodeTraceResult>(), (selectedItems) => true);
            this.AddInstructionCommand = new RelayCommand<CodeTraceResult>((codeTraceResult) => this.AddCodeTraceResult(codeTraceResult));
            this.AddInstructionsCommand = new RelayCommand<Object>((selectedItems) => this.AddCodeTraceResults(this.SelectedCodeTraceResults));
        }

        /// <summary>
        /// Gets a command to find what writes to an address.
        /// </summary>
        public ICommand FindWhatWritesCommand { get; private set; }

        /// <summary>
        /// Gets a command to find what reads from an address.
        /// </summary>
        public ICommand FindWhatReadsCommand { get; private set; }

        /// <summary>
        /// Gets a command to find what accesses an an address.
        /// </summary>
        public ICommand FindWhatAccessesCommand { get; private set; }

        /// <summary>
        /// Gets a command to stop recording events.
        /// </summary>
        public ICommand StopTraceCommand { get; private set; }

        /// <summary>
        /// Gets or sets the command to select scan results.
        /// </summary>
        public ICommand SelectInstructionCommand { get; private set; }

        /// <summary>
        /// Gets the command to add a scan result to the project explorer.
        /// </summary>
        public ICommand AddInstructionCommand { get; private set; }

        /// <summary>
        /// Gets the command to add all selected scan results to the project explorer.
        /// </summary>
        public ICommand AddInstructionsCommand { get; private set; }

        public FullyObservableCollection<CodeTraceResult> Results
        {
            get
            {
                return this.results;
            }

            set
            {
                this.results = value;
            }
        }

        public Boolean IsTracing
        {
            get
            {
                return this.DebuggerCancellationTokenSource != null;
            }
        }

        /// <summary>
        /// Gets or sets the selected code trace results.
        /// </summary>
        public IEnumerable<CodeTraceResult> SelectedCodeTraceResults
        {
            get
            {
                return this.selectedCodeTraceResults;
            }

            set
            {
                this.selectedCodeTraceResults = value;
                this.RaisePropertyChanged(nameof(this.SelectedCodeTraceResults));
            }
        }

        private CancellationTokenSource DebuggerCancellationTokenSource
        {
            get
            {
                return this.debuggerCancellationTokenSource;
            }

            set
            {
                this.debuggerCancellationTokenSource = value;
                this.RaisePropertyChanged(nameof(this.IsTracing));
            }
        }

        /// <summary>
        /// Gets a singleton instance of the <see cref="DebuggerViewModel"/> class.
        /// </summary>
        /// <returns>A singleton instance of the class.</returns>
        public static CodeTracerViewModel GetInstance()
        {
            return codeTracerViewModelInstance.Value;
        }

        /// <summary>
        /// Adds the given code trace result to the project explorer.
        /// </summary>
        /// <param name="codeTraceResult">The code trace result to add to the project explorer.</param>
        private void AddCodeTraceResult(CodeTraceResult codeTraceResult)
        {
            InstructionItem instructionItem = new InstructionItem(codeTraceResult.Address, "", "nop", new Byte[] { 0x90 });

            ProjectExplorerViewModel.GetInstance().AddProjectItems(instructionItem);
        }

        /// <summary>
        /// Adds the given code trace results to the project explorer.
        /// </summary>
        /// <param name="codeTraceResults">The code trace results to add to the project explorer.</param>
        private void AddCodeTraceResults(IEnumerable<CodeTraceResult> codeTraceResults)
        {
            if (codeTraceResults == null)
            {
                return;
            }

            IEnumerable<InstructionItem> projectItems = codeTraceResults.Select(
                codeTraceEvent => new InstructionItem(codeTraceEvent.Address, "", "nop", new Byte[] { 0x90 }));

            ProjectExplorerViewModel.GetInstance().AddProjectItems(projectItems.ToArray());
        }

        private void FindWhatWrites(ProjectItemView projectItemView)
        {
            ProjectItem projectItem = projectItemView?.ProjectItem;

            if (projectItem is AddressItem)
            {
                this.CancelTrace();
                this.Results.Clear();

                AddressItem addressItem = projectItem as AddressItem;

                BreakpointSize size = Debugger.Default.SizeToBreakpointSize((UInt32)Conversions.SizeOf(addressItem.DataType));
                this.DebuggerCancellationTokenSource = Debugger.Default.FindWhatWrites(addressItem.CalculatedAddress, size, this.CodeTraceEvent);
                this.ShowExecute();
            }
        }

        private void FindWhatReads(ProjectItemView projectItemView)
        {
            ProjectItem projectItem = projectItemView?.ProjectItem;

            if (projectItem is AddressItem)
            {
                this.CancelTrace();
                this.Results.Clear();

                AddressItem addressItem = projectItem as AddressItem;

                BreakpointSize size = Debugger.Default.SizeToBreakpointSize((UInt32)Conversions.SizeOf(addressItem.DataType));
                this.DebuggerCancellationTokenSource = Debugger.Default.FindWhatReads(addressItem.CalculatedAddress, size, this.CodeTraceEvent);
                this.ShowExecute();
            }
        }

        private void FindWhatAccesses(ProjectItemView projectItemView)
        {
            ProjectItem projectItem = projectItemView?.ProjectItem;

            if (projectItem is AddressItem)
            {
                this.CancelTrace();
                this.Results.Clear();

                AddressItem addressItem = projectItem as AddressItem;

                BreakpointSize size = Debugger.Default.SizeToBreakpointSize((UInt32)Conversions.SizeOf(addressItem.DataType));
                this.DebuggerCancellationTokenSource = Debugger.Default.FindWhatAccesses(addressItem.CalculatedAddress, size, this.CodeTraceEvent);
                this.ShowExecute();
            }
        }

        private void CancelTrace()
        {
            this.DebuggerCancellationTokenSource?.Cancel();
            this.DebuggerCancellationTokenSource = null;
        }

        private void CodeTraceEvent(CodeTraceInfo codeTraceInfo)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                CodeTraceResult result = this.Results.FirstOrDefault(results => results.Address == codeTraceInfo.Instruction.Address);

                // Insert or increment
                if (result != null)
                {
                    result.Count++;
                }
                else
                {
                    this.Results.Add(new CodeTraceResult(codeTraceInfo));
                }
            }));
        }
    }
    //// End class
}
//// End namespace