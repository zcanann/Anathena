namespace Squalr.Cli
{
    using CommandLine;
    using Squalr.Engine.Common;
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

            Parser.Default.ParseArguments<ProcessOpenOptions, ProcessListOptions>(command.Args)
                .MapResult(
                    (ProcessOpenOptions options) => ProcessOpenOptions.Handle(options),
                    (ProcessListOptions options) => ProcessListOptions.Handle(options),
                    errs => 1
                );

            command.Handled = true;
        }
    }
    //// End class
}
//// End namespace
