namespace Squalr.Source.Debugger
{
    using GalaSoft.MvvmLight.Command;
    using Squalr.Engine.Architecture;
    using Squalr.Engine.Memory;
    using Squalr.Engine.OS;
    using Squalr.Engine.Projects.Items;
    using Squalr.Engine.Utils.DataStructures;
    using Squalr.Engine.Utils.Extensions;
    using Squalr.Source.Docking;
    using Squalr.Source.ProjectExplorer;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// View model for the Disassembly.
    /// </summary>
    internal class DisassemblyViewModel : ToolViewModel, IProcessObserver
    {
        /// <summary>
        /// Singleton instance of the <see cref="DisassemblyViewModel" /> class.
        /// </summary>
        private static Lazy<DisassemblyViewModel> disassemblyViewModelInstance = new Lazy<DisassemblyViewModel>(
                () => { return new DisassemblyViewModel(); },
                LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// The base address from which dissassembly begins.
        /// </summary>
        private UInt64 baseAddress;

        /// <summary>
        /// The collection of visible instructions.
        /// </summary>
        private FullyObservableCollection<InstructionItem> instructions;

        /// <summary>
        /// The selected instructions.
        /// </summary>
        private IEnumerable<InstructionItem> selectedInstructions;

        /// <summary>
        /// Prevents a default instance of the <see cref="DisassemblyViewModel" /> class from being created.
        /// </summary>
        private DisassemblyViewModel() : base("Disassembly")
        {
            this.Instructions = new FullyObservableCollection<InstructionItem>();

            this.SelectInstructionsCommand = new RelayCommand<Object>((selectedItems) => this.SelectedInstructions = (selectedItems as IList)?.Cast<InstructionItem>(), (selectedItems) => true);
            this.AddInstructionCommand = new RelayCommand<InstructionItem>((instruction) => Task.Run(() => this.AddInstruction(instruction)), (scanResult) => true);
            this.AddInstructionsCommand = new RelayCommand<Object>((selectedItems) => Task.Run(() => this.AddInstructions(this.SelectedInstructions)), (selectedItems) => true);

            DockingViewModel.GetInstance().RegisterViewModel(this);

            Task.Run(() => Processes.Default.Subscribe(this));
        }

        /// <summary>
        /// Gets or sets the command to select instructions.
        /// </summary>
        public ICommand SelectInstructionsCommand { get; private set; }

        /// <summary>
        /// Gets the command to add an instruction to the project explorer.
        /// </summary>
        public ICommand AddInstructionCommand { get; private set; }

        /// <summary>
        /// Gets the command to add instructions to the project explorer.
        /// </summary>
        public ICommand AddInstructionsCommand { get; private set; }

        /// <summary>
        /// Gets or sets the selected scan results.
        /// </summary>
        public IEnumerable<InstructionItem> SelectedInstructions
        {
            get
            {
                return this.selectedInstructions;
            }

            set
            {
                this.selectedInstructions = value;
                this.RaisePropertyChanged(nameof(this.SelectedInstructions));
            }
        }

        /// <summary>
        /// Gets or sets the base address from which dissassembly begins.
        /// </summary>
        public UInt64 BaseAddress
        {
            get
            {
                return this.baseAddress;
            }

            set
            {
                this.baseAddress = value;
                this.RaisePropertyChanged(nameof(this.BaseAddress));
            }
        }

        /// <summary>
        /// Gets or sets the collection of visible instructions.
        /// </summary>
        public FullyObservableCollection<InstructionItem> Instructions
        {
            get
            {
                return this.instructions;
            }

            set
            {
                this.instructions = value;
                this.RaisePropertyChanged(nameof(this.Instructions));
            }
        }

        /// <summary>
        /// Gets a singleton instance of the <see cref="DisassemblyViewModel"/> class.
        /// </summary>
        /// <returns>A singleton instance of the class.</returns>
        public static DisassemblyViewModel GetInstance()
        {
            return disassemblyViewModelInstance.Value;
        }

        public void Update(Process process)
        {
            if (process != null)
            {
                this.BaseAddress = Query.Default.GetModules().FirstOrDefault()?.BaseAddress ?? 0UL;

                this.LoadInstructions();
            }
        }

        /// <summary>
        /// Adds the given instruction to the project explorer.
        /// </summary>
        /// <param name="instruction">The instruction to add to the project explorer.</param>
        private void AddInstruction(InstructionItem instruction)
        {
            ProjectExplorerViewModel.GetInstance().AddProjectItems(instruction);
        }

        /// <summary>
        /// Adds the given instructions to the project explorer.
        /// </summary>
        /// <param name="instructions">The instructions to add to the project explorer.</param>
        private void AddInstructions(IEnumerable<InstructionItem> instructions)
        {
            if (instructions == null)
            {
                return;
            }

            ProjectExplorerViewModel.GetInstance().AddProjectItems(instructions.ToArray());
        }

        /// <summary>
        /// Loads the instructions to display.
        /// </summary>
        private void LoadInstructions()
        {
            return;
            Byte[] bytes = Reader.Default.ReadBytes(this.BaseAddress, 200, out _);

            if (bytes.IsNullOrEmpty())
            {
                return;
            }

            Boolean isProcess32Bit = Processes.Default.IsOpenedProcess32Bit();

            // Disassemble instructions
            IEnumerable<Instruction> disassembledInstructions = Disassembler.Default.Disassemble(bytes, isProcess32Bit, this.BaseAddress);
            IList<InstructionItem> instructions = new List<InstructionItem>();

            foreach (Instruction disassembledInstruction in disassembledInstructions)
            {
                String moduleName = String.Empty;
                UInt64 address = Query.Default.AddressToModule(disassembledInstruction.Address, out moduleName);

                instructions.Add(new InstructionItem(address, moduleName, disassembledInstruction.Mnemonic, disassembledInstruction.Bytes));
            }

            this.Instructions = new FullyObservableCollection<InstructionItem>(instructions);
        }
    }
    //// End class
}
//// End namespace