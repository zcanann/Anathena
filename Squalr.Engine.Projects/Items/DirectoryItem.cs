namespace Squalr.Engine.Projects.Items
{
    using Squalr.Engine.Common.DataStructures;
    using Squalr.Engine.Common.Logging;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Defines a directory in the project.
    /// </summary>
    public class DirectoryItem : ProjectItem
    {
        /// <summary>
        /// The child project items under this directory.
        /// </summary>
        private FullyObservableCollection<ProjectItem> childItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryItem" /> class.
        /// </summary>
        public DirectoryItem(String directoryPath) : base(directoryPath)
        {
            // Bypass setters to avoid re-saving
            this.name = (new DirectoryInfo(directoryPath)).Name;

            this.childItems = this.BuildChildren();
            this.WatchForUpdates();
        }

        /// <summary>
        /// Creates a directory item from the specified project directory path, instantiating all children.
        /// </summary>
        /// <param name="directoryPath">The path to the project directory or subdirectory.</param>
        /// <returns>The instantiated directory item.</returns>
        public static DirectoryItem FromDirectory(String directoryPath)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    throw new Exception("Directory does not exist: " + directoryPath);
                }

                return new DirectoryItem(directoryPath);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Error loading file", ex);
                throw ex;
            }
        }

        /// <summary>
        /// Gets the child project items under this directory.
        /// </summary>
        public FullyObservableCollection<ProjectItem> ChildItems
        {
            get
            {
                return this.childItems;
            }

            private set
            {
                this.childItems = value;
                this.RaisePropertyChanged(nameof(this.ChildItems));
            }
        }

        /// <summary>
        /// Gets or sets an object to watch for file system changes under this directory.
        /// </summary>
        private FileSystemWatcher FileSystemWatcher { get; set; }

        /// <summary>
        /// Updates all project items under this directory.
        /// </summary>
        public override void Update()
        {
            IEnumerable<ProjectItem> children = this.ChildItems?.ToArray();

            if (children != null)
            {
                foreach (ProjectItem child in children)
                {
                    child.Update();
                }
            }
        }

        /// <summary>
        /// Adds the specified project item to this directory.
        /// </summary>
        /// <param name="projectItem">The project item to add.</param>
        public void AddChild(ProjectItem projectItem)
        {
            try
            {
                projectItem.Parent = this;
                this.ChildItems.Add(projectItem);
                projectItem.Save();
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Unable to add project item", ex);
            }
        }

        /// <summary>
        /// Removes the specified project item from this directory.
        /// </summary>
        /// <param name="projectItem">The project item to remove.</param>
        public void RemoveChild(ProjectItem projectItem)
        {
            try
            {
                if (this.ChildItems.Contains(projectItem))
                {
                    projectItem.Parent = null;
                    this.ChildItems.Remove(projectItem);
                    
                    if (projectItem is DirectoryItem)
                    {
                        Directory.Delete(projectItem.FullPath, recursive: true);
                    }
                    else
                    {
                        File.Delete(projectItem.FullPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Unable to delete project item", ex);
                // TODO: Probably do a full refresh at this point due to possible de-synchronization
            }
        }

        /// <summary>
        /// Gets the list of files in the directory Name passed.
        /// </summary>
        /// <returns>Returns the List of File info for this directory.
        /// Return null if an exception is raised.</returns>
        public FullyObservableCollection<ProjectItem> BuildChildren()
        {
            FullyObservableCollection<ProjectItem> projectItems = new FullyObservableCollection<ProjectItem>();

            try
            {
                IEnumerable<DirectoryInfo> subdirectories = Directory.GetDirectories(this.FullPath).Select(subdirectory => new DirectoryInfo(subdirectory));

                foreach (DirectoryInfo subdirectory in subdirectories)
                {
                    try
                    {
                        projectItems.Add(DirectoryItem.FromDirectory(subdirectory.FullName));
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogLevel.Error, "Error loading directory", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Error fetching directories", ex);
            }

            try
            {
                foreach (FileInfo file in Directory.GetFiles(this.FullPath).Select(directoryFile => new FileInfo(directoryFile)))
                {
                    try
                    {
                        ProjectItem projectItem = ProjectItem.FromFile(file.FullName, this);

                        if (projectItem != null)
                        {
                            projectItems.Add(projectItem);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogLevel.Error, "Error reading project item", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Error fetching files", ex);
            }

            return projectItems;
        }

        /// <summary>
        /// Saves this directory to disk.
        /// </summary>
        public override void Save()
        {
            try
            {
                if (!Directory.Exists(this.FullPath))
                {
                    Directory.CreateDirectory(this.FullPath);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Error creating directory within project.", ex);
            }
        }

        /// <summary>
        /// Initializes the filesystem watcher to listen for filesystem changes.
        /// </summary>
        private void WatchForUpdates()
        {
            this.FileSystemWatcher = new FileSystemWatcher(this.FullPath, "*.*")
            {
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                EnableRaisingEvents = true,
            };

            this.FileSystemWatcher.Changed += new FileSystemEventHandler(OnFilesOrDirectoriesChanged);
        }

        /// <summary>
        /// Method invoked when files or directories change under the project root.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="args">The filesystem change event args.</param>
        private void OnFilesOrDirectoriesChanged(Object source, FileSystemEventArgs args)
        {
            switch (args.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    break;
                case WatcherChangeTypes.Deleted:
                    break;
                case WatcherChangeTypes.Changed:
                    break;
                case WatcherChangeTypes.Renamed:
                    this.RaisePropertyChanged(nameof(this.ChildItems));
                    break;
            }
        }
    }
    //// End class
}
//// End namespace