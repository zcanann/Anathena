namespace Squalr.View
{
    using Source.DotNetExplorer;
    using Source.Results;
    using Source.Snapshots;
    using Squalr.Properties;
    using Squalr.Source.Debugger;
    using Squalr.Source.Editors.HotkeyEditor;
    using Squalr.Source.Editors.OffsetEditor;
    using Squalr.Source.Editors.ScriptEditor;
    using Squalr.Source.Editors.TextEditor;
    using Squalr.Source.Output;
    using Squalr.Source.ProcessSelector;
    using Squalr.Source.ProjectExplorer;
    using Squalr.Source.PropertyViewer;
    using Squalr.Source.Scanning;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Provides the template required to view a pane.
    /// </summary>
    internal class ViewTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// The template for the Process Selector.
        /// </summary>
        private DataTemplate processSelectorViewTemplate;

        /// <summary>
        /// The template for the Property Viewer.
        /// </summary>
        private DataTemplate propertyViewerViewTemplate;

        /// <summary>
        /// The template for the Output.
        /// </summary>
        private DataTemplate outputViewTemplate;

        /// <summary>
        /// The template for the Offset Editor.
        /// </summary>
        private DataTemplate offsetEditorViewTemplate;

        /// <summary>
        /// The template for the Script Editor.
        /// </summary>
        private DataTemplate scriptEditorViewTemplate;

        /// <summary>
        /// The template for the Text Editor.
        /// </summary>
        private DataTemplate textEditorViewTemplate;

        /// <summary>
        /// The template for the Hotkey Editor.
        /// </summary>
        private DataTemplate hotkeyEditorViewTemplate;

        /// <summary>
        /// The template for the Change Counter.
        /// </summary>
        private DataTemplate changeCounterViewTemplate;

        /// <summary>
        /// The template for the Input Correlator.
        /// </summary>
        private DataTemplate inputCorrelatorViewTemplate;

        /// <summary>
        /// The template for the Label Thresholder.
        /// </summary>
        private DataTemplate labelThresholderViewTemplate;

        /// <summary>
        /// The template for the Pointer Scanner.
        /// </summary>
        private DataTemplate pointerScannerViewTemplate;

        /// <summary>
        /// The template for the Snapshot Manager.
        /// </summary>
        private DataTemplate snapshotManagerViewTemplate;

        /// <summary>
        /// The template for the Scan Results.
        /// </summary>
        private DataTemplate scanResultsViewTemplate;

        /// <summary>
        /// The template for the Pointer Scan Results.
        /// </summary>
        private DataTemplate pointerScanResultsViewTemplate;

        /// <summary>
        /// The template for the .Net Explorer.
        /// </summary>
        private DataTemplate dotNetExplorerViewTemplate;

        /// <summary>
        /// The template for the Project Explorer.
        /// </summary>
        private DataTemplate projectExplorerViewTemplate;

        /// <summary>
        /// The template for the Settings.
        /// </summary>
        private DataTemplate settingsViewTemplate;

        /// <summary>
        /// The template for the Debugger.
        /// </summary>
        private DataTemplate debuggerViewTemplate;

        /// <summary>
        /// The template for the Disassembly.
        /// </summary>
        private DataTemplate disassemblyViewTemplate;

        /// <summary>
        /// The template for the code.
        /// </summary>
        private DataTemplate codeTracerViewTemplate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewTemplateSelector" /> class.
        /// </summary>
        public ViewTemplateSelector()
        {
            this.DataTemplates = new Dictionary<Type, DataTemplate>();
        }

        /// <summary>
        /// Gets or sets the template for the Data Template Error display.
        /// </summary>
        public DataTemplate DataTemplateErrorViewTemplate { get; set; }

        /// <summary>
        /// Gets or sets the mapping for all data templates.
        /// </summary>
        protected Dictionary<Type, DataTemplate> DataTemplates { get; set; }

        /// <summary>
        /// Returns the required template to display the given view model.
        /// </summary>
        /// <param name="item">The view model.</param>
        /// <param name="container">The dependency object.</param>
        /// <returns>The template associated with the provided view model.</returns>
        public override DataTemplate SelectTemplate(Object item, DependencyObject container)
        {
            if (this.DataTemplates.ContainsKey(item.GetType()))
            {
                return this.DataTemplates[item.GetType()];
            }

            return this.DataTemplateErrorViewTemplate;
        }

        /// <summary>
        /// Gets or sets the template for the Process Selector.
        /// </summary>
        public DataTemplate ProcessSelectorViewTemplate
        {
            get
            {
                return this.processSelectorViewTemplate;
            }

            set
            {
                this.processSelectorViewTemplate = value;
                this.DataTemplates[typeof(ProcessSelectorViewModel)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the template for the Property Viewer.
        /// </summary>
        public DataTemplate PropertyViewerViewTemplate
        {
            get
            {
                return this.propertyViewerViewTemplate;
            }

            set
            {
                this.propertyViewerViewTemplate = value;
                this.DataTemplates[typeof(PropertyViewerViewModel)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the template for the Output.
        /// </summary>
        public DataTemplate OutputViewTemplate
        {
            get
            {
                return this.outputViewTemplate;
            }

            set
            {
                this.outputViewTemplate = value;
                this.DataTemplates[typeof(OutputViewModel)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the template for the Offset Editor.
        /// </summary>
        public DataTemplate OffsetEditorViewTemplate
        {
            get
            {
                return this.offsetEditorViewTemplate;
            }

            set
            {
                this.offsetEditorViewTemplate = value;
                this.DataTemplates[typeof(OffsetEditorViewModel)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the template for the Script Editor.
        /// </summary>
        public DataTemplate ScriptEditorViewTemplate
        {
            get
            {
                return this.scriptEditorViewTemplate;
            }

            set
            {
                this.scriptEditorViewTemplate = value;
                this.DataTemplates[typeof(ScriptEditorViewModel)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the template for the Text Editor.
        /// </summary>
        public DataTemplate TextEditorViewTemplate
        {
            get
            {
                return this.textEditorViewTemplate;
            }

            set
            {
                this.textEditorViewTemplate = value;
                this.DataTemplates[typeof(TextEditorViewModel)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the template for the Hotkey Editor.
        /// </summary>
        public DataTemplate HotkeyEditorViewTemplate
        {
            get
            {
                return this.hotkeyEditorViewTemplate;
            }

            set
            {
                this.hotkeyEditorViewTemplate = value;
                this.DataTemplates[typeof(HotkeyEditorViewModel)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the template for the Settings.
        /// </summary>
        public DataTemplate SettingsViewTemplate
        {
            get
            {
                return this.settingsViewTemplate;
            }

            set
            {
                this.settingsViewTemplate = value;
                this.DataTemplates[typeof(SettingsViewModel)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the template for the Change Counter.
        /// </summary>
        public DataTemplate ChangeCounterViewTemplate
        {
            get
            {
                return this.changeCounterViewTemplate;
            }

            set
            {
                this.changeCounterViewTemplate = value;
                this.DataTemplates[typeof(ChangeCounterViewModel)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the template for the Input Correlator.
        /// </summary>
        public DataTemplate InputCorrelatorViewTemplate
        {
            get
            {
                return this.inputCorrelatorViewTemplate;
            }

            set
            {
                this.inputCorrelatorViewTemplate = value;
                this.DataTemplates[typeof(InputCorrelatorViewModel)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the template for the Pointer Scanner.
        /// </summary>
        public DataTemplate PointerScannerViewTemplate
        {
            get
            {
                return this.pointerScannerViewTemplate;
            }

            set
            {
                this.pointerScannerViewTemplate = value;
                this.DataTemplates[typeof(PointerScannerViewModel)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the template for the Snapshot Manager.
        /// </summary>
        public DataTemplate SnapshotManagerViewTemplate
        {
            get
            {
                return this.snapshotManagerViewTemplate;
            }

            set
            {
                this.snapshotManagerViewTemplate = value;
                this.DataTemplates[typeof(SnapshotManagerViewModel)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the template for the Scan Results.
        /// </summary>
        public DataTemplate ScanResultsViewTemplate
        {
            get
            {
                return this.scanResultsViewTemplate;
            }

            set
            {
                this.scanResultsViewTemplate = value;
                this.DataTemplates[typeof(ScanResultsViewModel)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the template for the Pointer Scan Results.
        /// </summary>
        public DataTemplate PointerScanResultsViewTemplate
        {
            get
            {
                return this.pointerScanResultsViewTemplate;
            }

            set
            {
                this.pointerScanResultsViewTemplate = value;
                this.DataTemplates[typeof(PointerScanResultsViewModel)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the template for the .Net Explorer.
        /// </summary>
        public DataTemplate DotNetExplorerViewTemplate
        {
            get
            {
                return this.dotNetExplorerViewTemplate;
            }

            set
            {
                this.dotNetExplorerViewTemplate = value;
                this.DataTemplates[typeof(DotNetExplorerViewModel)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the template for the Project Explorer.
        /// </summary>
        public DataTemplate ProjectExplorerViewTemplate
        {
            get
            {
                return this.projectExplorerViewTemplate;
            }

            set
            {
                this.projectExplorerViewTemplate = value;
                this.DataTemplates[typeof(ProjectExplorerViewModel)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the template for the Debugger.
        /// </summary>
        public DataTemplate DebuggerViewTemplate
        {
            get
            {
                return this.debuggerViewTemplate;
            }

            set
            {
                this.debuggerViewTemplate = value;
                this.DataTemplates[typeof(DebuggerViewModel)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the template for the Disassembly.
        /// </summary>
        public DataTemplate DisassemblyViewTemplate
        {
            get
            {
                return this.disassemblyViewTemplate;
            }

            set
            {
                this.disassemblyViewTemplate = value;
                this.DataTemplates[typeof(DisassemblyViewModel)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the template for the Code Tracer.
        /// </summary>
        public DataTemplate CodeTracerViewTemplate
        {
            get
            {
                return this.codeTracerViewTemplate;
            }

            set
            {
                this.codeTracerViewTemplate = value;
                this.DataTemplates[typeof(CodeTracerViewModel)] = value;
            }
        }
    }
    //// End class
}
//// End namespace