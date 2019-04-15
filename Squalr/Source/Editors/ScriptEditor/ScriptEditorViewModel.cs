namespace Squalr.Source.Editors.ScriptEditor
{
    using GalaSoft.MvvmLight.Command;
    using global::Engine.Scripting.Templates;
    using Squalr.Source.Docking;
    using System;
    using System.Threading;
    using System.Windows.Input;

    /// <summary>
    /// View model for the Script Editor.
    /// </summary>
    public class ScriptEditorViewModel : ToolViewModel
    {
        /// <summary>
        /// Singleton instance of the <see cref="ScriptEditorViewModel" /> class.
        /// </summary>
        private static Lazy<ScriptEditorViewModel> scriptEditorViewModelInstance = new Lazy<ScriptEditorViewModel>(
                () => { return new ScriptEditorViewModel(); },
                LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Prevents a default instance of the <see cref="ScriptEditorViewModel" /> class.
        /// </summary>
        private ScriptEditorViewModel() : base("Script Editor")
        {
            this.UpdateScriptCommand = new RelayCommand<String>((script) => this.UpdateScript(script), (script) => true);
            this.SaveScriptCommand = new RelayCommand<String>((script) => this.SaveScript(script), (script) => true);

            DockingViewModel.GetInstance().RegisterViewModel(this);
        }

        /// <summary>
        /// Gets a command to save the active script.
        /// </summary>
        public ICommand SaveScriptCommand { get; private set; }

        /// <summary>
        /// Gets a command to update the active script text.
        /// </summary>
        public ICommand UpdateScriptCommand { get; private set; }

        /// <summary>
        /// Gets the active script text.
        /// </summary>
        public String Script { get; private set; }

        /// <summary>
        /// Gets a singleton instance of the <see cref="ScriptEditorViewModel"/> class.
        /// </summary>
        /// <returns>A singleton instance of the class.</returns>
        public static ScriptEditorViewModel GetInstance()
        {
            return ScriptEditorViewModel.scriptEditorViewModelInstance.Value;
        }

        /// <summary>
        /// Gets the code injection script template.
        /// </summary>
        /// <returns>The code injection script template.</returns>
        public String GetCodeInjectionTemplate()
        {
            CodeInjectionTemplate codeInjectionTemplate = new CodeInjectionTemplate();

            return codeInjectionTemplate.TransformText();
        }

        /// <summary>
        /// Gets the graphics injection script template.
        /// </summary>
        /// <returns>The graphics injection script template.</returns>
        public String GetGraphicsInjectionTemplate()
        {
            GraphicsInjectionTemplate graphicsInjectionTemplate = new GraphicsInjectionTemplate();

            return graphicsInjectionTemplate.TransformText();
        }

        /// <summary>
        /// Updates the active script.
        /// </summary>
        /// <param name="script">The raw script text.</param>
        private void UpdateScript(String script)
        {
            this.Script = script;
        }

        /// <summary>
        /// Saves the provided script.
        /// </summary>
        /// <param name="script">The raw script to save.</param>
        private void SaveScript(String script)
        {
            this.UpdateScript(script);
        }
    }
    //// End class
}
//// End namespace