namespace Squalr.Engine.Projects
{
    using Squalr.Engine.Common.Logging;
    using Squalr.Engine.Projects.Properties;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class ProjectManager
    {
        public ProjectManager()
        {

        }

        public static IEnumerable<Project> Projects
        {
            get
            {
                return Directory.EnumerateDirectories(ProjectSettings.Default.ProjectRoot).Select(path => new Project(new DirectoryInfo(path).Name)).ToList();
            }
        }

        public static void RenameProject(String oldProjectPath, String newProjectPath)
        {
            try
            {
                Directory.Move(oldProjectPath, newProjectPath);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Unable to rename project", ex);
            }
        }
    }
    //// End class
}
//// End namespace