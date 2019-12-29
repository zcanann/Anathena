namespace Squalr.Cli.CommandHandlers.Process
{
    using CommandLine;
    using Squalr.Engine;
    using System;
    using System.Collections.Generic;

    public class ProcessCommandHandler : ICommandHandler
    {
        public void TryHandle(ref Session session, Command command)
        {
            Parser.Default.ParseArguments<ProcessOpenOptions, ProcessCloseOptions, ProcessListOptions>(command.Args)
                .MapResult(
                    (ProcessOpenOptions options) => options.Handle(),
                    (ProcessCloseOptions options) => options.Handle(),
                    (ProcessListOptions options) => options.Handle(),
                    errs => 1
                );

            command.Handled = true;
        }

        public IEnumerable<String> GetCommandAndAliases()
        {
            return new List<String>()
            {
                "proc",
                "process",
                "processes"
            };
        }
    }
    //// End class
}
//// End namespace
