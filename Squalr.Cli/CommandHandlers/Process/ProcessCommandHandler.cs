namespace Squalr.Cli.CommandHandlers.Process
{
    using CommandLine;
    using Squalr.Engine;
    using System;

    public class ProcessCommandHandler : ICommandHandler
    {
        public void TryHandle(ref Session session, Command command)
        {
            if (!command.Name.Equals("proc", StringComparison.OrdinalIgnoreCase) &&
                !command.Name.Equals("process", StringComparison.OrdinalIgnoreCase) &&
                !command.Name.Equals("processes", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            Parser.Default.ParseArguments<ProcessOpenOptions, ProcessCloseOptions, ProcessListOptions>(command.Args)
                .MapResult(
                    (ProcessOpenOptions options) => options.Handle(),
                    (ProcessCloseOptions options) => options.Handle(),
                    (ProcessListOptions options) => options.Handle(),
                    errs => 1
                );

            command.Handled = true;
        }
    }
    //// End class
}
//// End namespace
