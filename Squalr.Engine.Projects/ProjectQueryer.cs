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
                return Directory.EnumerateDirectories(ProjectSettings.ProjectRoot).Select(path => ProjectQueryer.ProjectPathToName(path));
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Error reading project names from disk.", ex);

                return new List<String>();
            }
        }

        public static IEnumerable<String> GetProjectPaths()
        {
            try
            {
                return Directory.EnumerateDirectories(ProjectSettings.ProjectRoot);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Error reading project paths from disk.", ex);

                return new List<String>();
            }
        }

        public static IEnumerable<Project> GetProjects()
        {
            return ProjectQueryer.GetProjectPaths().Select(path => new Project(path));
        }

        public static String ProjectNameToPath(String projectName)
        {
            try
            {
                return Path.Combine(ProjectSettings.ProjectRoot, projectName);
            }
            catch(Exception ex)
            {
                Logger.Log(LogLevel.Error, "Error converting project name to path.", ex);

                return String.Empty;
            }
        }

        public static String ProjectPathToName(String path)
        {
            try
            {
                return new DirectoryInfo(path).Name;
            }
            catch(Exception ex)
            {
                Logger.Log(LogLevel.Error, "Error converting project path to name.", ex);

                return String.Empty;
            }
        }
    }
    //// End class
}
//// End namespace