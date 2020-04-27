namespace Squalr.Engine.Projects
{
    using Squalr.Engine.Common.Logging;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class ProjectQueryer
    {
        public static IEnumerable<String> GetProjectNames()
        {
            try
            {
                return Directory.EnumerateDirectories(ProjectSettings.ProjectRoot).Select(path => ProjectQueryer.ProcessPath(path));
            }
            catch(Exception ex)
            {
                Logger.Log(LogLevel.Error, "Error reading project files from disk", ex);

                return new List<String>();
            }
        }

        public static IEnumerable<Project> GetProjects()
        {
            return ProjectQueryer.GetProjectNames().Select(name => new Project(name));
        }

        private static String ProcessPath(String path)
        {
            return new DirectoryInfo(path).Name;
        }
    }
    //// End class
}
//// End namespace