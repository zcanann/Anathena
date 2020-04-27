namespace Squalr.Cli.CommandHandlers.Projects
{
    using CommandLine;
    using Squalr.Engine;
    using System;
    using System.Collections.Generic;

    public class ProjectsCommandHandler : ICommandHandler
    {
        public String GetCommandName()
        {
            return "Projects";
        }

        public void TryHandle(ref Session session, Command command)
        {
            Parser.Default.ParseArguments<ProjectsNewOptions, ProjectsDeleteOptions, ProjectsListOptions, ProjectsOpenOptions>(command.Args)
                .MapResult(
                    (ProjectsNewOptions options) => options.Handle(),
                    (ProjectsDeleteOptions options) => options.Handle(),
                    (ProjectsListOptions options) => options.Handle(),
                    (ProjectsOpenOptions options) => options.Handle(),
                    errs => 1
                );

            command.Handled = true;
        }

        public IEnumerable<String> GetCommandAndAliases()
        {
            return new List<String>()
            {
                "projs",
                "projects"
            };
        }
    }
    //// End class
}
//// End namespace
