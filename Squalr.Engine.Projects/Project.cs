namespace Squalr.Engine.Projects
{
    using Squalr.Engine.Common.Logging;
    using Squalr.Engine.Projects.Items;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class Project
    {
        public Project(String projectFilePathOrName)
        {
            if (!Path.IsPathRooted(projectFilePathOrName))
            {
                projectFilePathOrName = Path.Combine(ProjectSettings.ProjectRoot, projectFilePathOrName);
            }

            this.FilePath = projectFilePathOrName;

            this.WatchFileSystem(this.FilePath);

            // Debugging
            this.ProjectItems = new List<ProjectItem>()
            {
                new PointerItem()
            };
        }

        public String FilePath { get; private set; }

        public String Name
        {
            get
            {
                return ProjectQueryer.ProjectPathToName(this.FilePath);
            }
        }

        public IEnumerable<ProjectItem> ProjectItems { get; private set; }

        private FileSystemWatcher FileSystemWatcher { get; set; }

        public static Project Create(String projectFilePath)
        {
            try
            {
                Directory.CreateDirectory(projectFilePath);

                return new Project(projectFilePath);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Error creating project", ex);
                return null;
            }
        }

        public void Rename(String newProjectPathOrName)
        {
            try
            {
                if (!Path.IsPathRooted(newProjectPathOrName))
                {
                    newProjectPathOrName = Path.Combine(ProjectSettings.ProjectRoot, newProjectPathOrName);
                }

                Directory.Move(this.FilePath, newProjectPathOrName);

                this.FilePath = newProjectPathOrName;


                this.WatchFileSystem(this.FilePath);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Unable to rename project", ex);
            }
        }

        private void WatchFileSystem(String projectRootPath)
        {
            this.FileSystemWatcher = new FileSystemWatcher
            {
                Path = projectRootPath,
                Filter = "*.*",
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                EnableRaisingEvents = true,
            };

            this.FileSystemWatcher.Changed += new FileSystemEventHandler(this.OnFileSystemChanged);
        }

        private void OnFileSystemChanged(Object source, FileSystemEventArgs args)
        {
            switch (args.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                    break;
                case WatcherChangeTypes.Created:
                    break;
                case WatcherChangeTypes.Deleted:
                    break;
                case WatcherChangeTypes.Renamed:
                    break;
                default:
                    break;
            }
        }
    }
    //// End class
}
//// End namespace