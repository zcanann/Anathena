namespace Squalr.Cli.CommandHandlers.Project
{
    using CommandLine;
    using Squalr.Engine;
    using System;

    public class ProjectCommandHandler : ICommandHandler
    {
        public void TryHandle(ref Session session, Command command)
        {
            if (!command.Name.Equals("proj", StringComparison.OrdinalIgnoreCase) &&
                !command.Name.Equals("project", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            Parser.Default.ParseArguments<ProjectAddOptions, ProjectRemoveOptions, ProjectListOptions>(command.Args)
                .MapResult(
                    (ProjectAddOptions options) => options.Handle(),
                    (ProjectRemoveOptions options) => options.Handle(),
                    (ProjectListOptions options) => options.Handle(),
                    errs => 1
                );

            command.Handled = true;
        }
    }
    //// End class
}
//// End namespace
