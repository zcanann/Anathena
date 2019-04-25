namespace Squalr.Engine.Projects
{
    using Squalr.Engine.Common.Logging;
    using System;
    using System.IO;

    public class Project
    {
        public Project(String projectFilePath)
        {
            this.WatchFileSystem(projectFilePath);
        }

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

        public void Rename(String newProjectName)
        {

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