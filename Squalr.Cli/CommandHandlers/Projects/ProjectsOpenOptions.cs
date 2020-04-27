namespace Squalr.Cli.CommandHandlers.Projects
{
    using CommandLine;
    using Squalr.Engine.Projects;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Verb("open", HelpText = "Opens a project from disk")]
    public class ProjectsOpenOptions
    {
        public Int32 Handle()
        {
            if (String.IsNullOrWhiteSpace(this.ProjectTerm))
            {
                Console.WriteLine("[Error] - Please specify a project term.");

                return -1;
            }

            Project project = null;
            IEnumerable<String> projectNames = ProjectQueryer.GetProjectNames();
            String projectName = String.Empty;

            if (Int32.TryParse(this.ProjectTerm, out Int32 index))
            {
                projectName = projectNames.Skip(index).FirstOrDefault();
            }
            else
            {
                // Try exact match
                projectName = projectNames.Where(x => x.Equals(this.ProjectTerm)).FirstOrDefault();

                // Try non-case sensitive match
                if (String.IsNullOrWhiteSpace(projectName))
                {
                    projectName = projectNames.Where(x => x.Equals(this.ProjectTerm, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                }

                // Try contains (match case)
                if (String.IsNullOrWhiteSpace(projectName))
                {
                    projectName = projectNames.Where(x => x.Contains(this.ProjectTerm)).FirstOrDefault();
                }

                // Try contains (no match case)
                if (String.IsNullOrWhiteSpace(projectName))
                {
                    projectName = projectNames.Where(x => x.Contains(this.ProjectTerm, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                }
            }

            if (!String.IsNullOrWhiteSpace(projectName))
            {
                project = new Project(projectName);
            }

            if (project == null)
            {
                Console.WriteLine("[Error] - Unable to find specified project.");

                return -1;
            }

            SessionManager.Project = project;

            Console.WriteLine("Project opened: " + project.Name);

            return 0;
        }

        [Value(0, MetaName = "project term", HelpText = "A term to identify the project. This can be an index, or a string in the project name.")]
        public String ProjectTerm { get; set; }
    }
    //// End class
}
//// End namespace
