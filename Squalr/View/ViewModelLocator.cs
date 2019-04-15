namespace Squalr.View
{
    using Source.DotNetExplorer;
    using Source.Main;
    using Source.Results;
    using Source.Snapshots;
    using Squalr.Properties;
    using Squalr.Source.ChangeLog;
    using Squalr.Source.Controls;
    using Squalr.Source.Debugger;
    using Squalr.Source.Docking;
    using Squalr.Source.Editors.HotkeyEditor;
    using Squalr.Source.Editors.OffsetEditor;
    using Squalr.Source.Editors.ScriptEditor;
    using Squalr.Source.Editors.TextEditor;
    using Squalr.Source.Editors.ValueEditor;
    using Squalr.Source.Output;
    using Squalr.Source.ProcessSelector;
    using Squalr.Source.ProjectExplorer;
    using Squalr.Source.ProjectExplorer.Dialogs;
    using Squalr.Source.PropertyViewer;
    using Squalr.Source.Scanning;
    using Squalr.Source.Tasks;

    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    internal class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
        }

        /// <summary>
        /// Gets the Docking view model.
        /// </summary>
        public DockingViewModel DockingViewModel
        {
            get
            {
                return DockingViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Action Scheduler view model.
        /// </summary>
        public TaskTrackerViewModel TaskTrackerViewModel
        {
            get
            {
                return TaskTrackerViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Process Selector view model.
        /// </summary>
        public ProcessSelectorViewModel ProcessSelectorViewModel
        {
            get
            {
                return ProcessSelectorViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Property Viewer view model.
        /// </summary>
        public PropertyViewerViewModel PropertyViewerViewModel
        {
            get
            {
                return PropertyViewerViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets a Output view model.
        /// </summary>
        public OutputViewModel OutputViewModel
        {
            get
            {
                return OutputViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets a Change Log view model. Note: Not a singleton, will create a new object.
        /// </summary>
        public ChangeLogViewModel ChangeLogViewModel
        {
            get
            {
                return ChangeLogViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Script Editor view model.
        /// </summary>
        public ScriptEditorViewModel ScriptEditorViewModel
        {
            get
            {
                return ScriptEditorViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Text Editor view model.
        /// </summary>
        public TextEditorViewModel TextEditorViewModel
        {
            get
            {
                return TextEditorViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Value Editor view model.
        /// </summary>
        public ValueEditorViewModel ValueEditorViewModel
        {
            get
            {
                return ValueEditorViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets a Offset Editor view model.
        /// </summary>
        public OffsetEditorViewModel OffsetEditorViewModel
        {
            get
            {
                return OffsetEditorViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets a Hotkey Editor view model.
        /// </summary>
        public HotkeyEditorViewModel HotkeyEditorViewModel
        {
            get
            {
                return HotkeyEditorViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Settings view model.
        /// </summary>
        public SettingsViewModel SettingsViewModel
        {
            get
            {
                return SettingsViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Main view model.
        /// </summary>
        public MainViewModel MainViewModel
        {
            get
            {
                return MainViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Change Counter view model.
        /// </summary>
        public ChangeCounterViewModel ChangeCounterViewModel
        {
            get
            {
                return ChangeCounterViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Input Correlator view model.
        /// </summary>
        public InputCorrelatorViewModel InputCorrelatorViewModel
        {
            get
            {
                return InputCorrelatorViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Manual Scanner view model.
        /// </summary>
        public ManualScannerViewModel ManualScannerViewModel
        {
            get
            {
                return ManualScannerViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Pointer Scanner view model.
        /// </summary>
        public PointerScannerViewModel PointerScannerViewModel
        {
            get
            {
                return PointerScannerViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Project Explorer view model.
        /// </summary>
        public ProjectExplorerViewModel ProjectExplorerViewModel
        {
            get
            {
                return ProjectExplorerViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Snapshot Manager view model.
        /// </summary>
        public SnapshotManagerViewModel SnapshotManagerViewModel
        {
            get
            {
                return SnapshotManagerViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Scan Results view model.
        /// </summary>
        public ScanResultsViewModel ScanResultsViewModel
        {
            get
            {
                return ScanResultsViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Pointer Scan Results view model.
        /// </summary>
        public PointerScanResultsViewModel PointerScanResultsViewModel
        {
            get
            {
                return PointerScanResultsViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the .Net Explorer view model.
        /// </summary>
        public DotNetExplorerViewModel DotNetExplorerViewModel
        {
            get
            {
                return DotNetExplorerViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Value Collector view model.
        /// </summary>
        public ValueCollectorViewModel ValueCollectorViewModel
        {
            get
            {
                return ValueCollectorViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Debugger view model.
        /// </summary>
        public DebuggerViewModel DebuggerViewModel
        {
            get
            {
                return DebuggerViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Debugger view model.
        /// </summary>
        public DisassemblyViewModel DisassemblyViewModel
        {
            get
            {
                return DisassemblyViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Debugger view model.
        /// </summary>
        public CodeTracerViewModel CodeTracerViewModel
        {
            get
            {
                return CodeTracerViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Delete Project Dialog view model.
        /// </summary>
        public DeleteProjectDialogViewModel DeleteProjectDialogViewModel
        {
            get
            {
                return DeleteProjectDialogViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the create Project Dialog view model.
        /// </summary>
        public CreateProjectDialogViewModel CreateProjectDialogViewModel
        {
            get
            {
                return CreateProjectDialogViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the rename Project Dialog view model.
        /// </summary>
        public RenameProjectDialogViewModel RenameProjectDialogViewModel
        {
            get
            {
                return RenameProjectDialogViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Select Project Dialog view model.
        /// </summary>
        public SelectProjectDialogViewModel SelectProjectDialogViewModel
        {
            get
            {
                return SelectProjectDialogViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets a HexDec box view model.
        /// </summary>
        public HexDecBoxViewModel HexDecBoxViewModel
        {
            get
            {
                return new HexDecBoxViewModel();
            }
        }
    }
    //// End class
}
//// End namespace