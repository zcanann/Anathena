namespace Squalr.Source.Docking
{
    using GalaSoft.MvvmLight;
    using Squalr.Engine.Common.Logging;
    using Squalr.Theme;
    using Squalr.Theme.Layout.Serialization;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    /// <summary>
    /// Docking view model.
    /// </summary>
    public class DockingViewModel : ViewModelBase
    {
        /// <summary>
        /// Singleton instance of the <see cref="DockingViewModel" /> class
        /// </summary>
        private static Lazy<DockingViewModel> mainViewModelInstance = new Lazy<DockingViewModel>(
                () => { return new DockingViewModel(); },
                LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Collection of tools contained in the main docking panel.
        /// </summary>
        private HashSet<ToolViewModel> tools;

        /// <summary>
        /// Prevents a default instance of the <see cref="DockingViewModel" /> class from being created.
        /// </summary>
        private DockingViewModel()
        {
            this.tools = new HashSet<ToolViewModel>();
        }

        /// <summary>
        /// Gets the tools contained in the main docking panel.
        /// </summary>
        public IEnumerable<ToolViewModel> Tools
        {
            get
            {
                if (this.tools == null)
                {
                    this.tools = new HashSet<ToolViewModel>();
                }

                return this.tools;
            }
        }

        /// <summary>
        /// Gets the singleton instance of the <see cref="DockingViewModel" /> class.
        /// </summary>
        /// <returns>The singleton instance of the <see cref="DockingViewModel" /> class.</returns>
        public static DockingViewModel GetInstance()
        {
            return mainViewModelInstance.Value;
        }

        /// <summary>
        /// Registers a view model to the list of available view models for docking.
        /// </summary>
        /// <param name="observer">The tool to be added.</param>
        public void RegisterViewModel(ToolViewModel observer)
        {
            if (observer != null && !this.tools.Contains(observer))
            {
                this.tools?.Add(observer);
            }

            this.RaisePropertyChanged(nameof(this.Tools));
        }

        /// <summary>
        /// Loads and deserializes the saved layout from disk. If no layout found, the default is loaded from resources.
        /// </summary>
        /// <param name="dockManager">The docking root to which content is loaded.</param>
        /// <param name="fileName">Resource to load the layout from. This is optional.</param>
        /// <param name="fileName">Resource to load the layout from. This is optional.</param>
        public void LoadLayoutFromFile(DockingManager dockManager, String fileName, String fallbackResource = null)
        {
            try
            {
                XmlLayoutSerializer serializer = new XmlLayoutSerializer(dockManager);
                serializer.Deserialize(fileName);
            }
            catch
            {
                this.LoadLayoutFromResource(dockManager, fallbackResource);
            }
        }

        /// <summary>
        /// Loads and deserializes the saved layout from disk. If no layout found, the default is loaded from resources.
        /// </summary>
        /// <param name="dockManager">The docking root to which content is loaded.</param>
        /// <param name="resource">Resource to load the layout from. This is optional.</param>
        public void LoadLayoutFromResource(DockingManager dockManager, String resource)
        {
            String layoutResource = Assembly.GetEntryAssembly().GetManifestResourceNames()
                .FirstOrDefault(resourceName => resourceName.EndsWith(resource));

            if (String.IsNullOrEmpty(layoutResource))
            {
                Logger.Log(LogLevel.Warn, "Unable to load layout resource.");
                return;
            }

            try
            {
                // Attempt to load layout from resource name
                using (Stream stream = Assembly.GetEntryAssembly().GetManifestResourceStream(layoutResource))
                {
                    if (stream != null)
                    {
                        XmlLayoutSerializer serializer = new XmlLayoutSerializer(dockManager);
                        serializer.Deserialize(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "Error loading layout resource.", ex);
            }
        }

        /// <summary>
        /// Saves and deserializes the saved layout from disk.
        /// </summary>
        /// <param name="dockManager">The docking root to save.</param>
        public void SaveLayout(DockingManager dockManager, String fileName = null)
        {
            try
            {
                XmlLayoutSerializer serializer = new XmlLayoutSerializer(dockManager);
                serializer.Serialize(fileName);
            }
            catch
            {
            }
        }
    }
    //// End class
}
//// End namesapce