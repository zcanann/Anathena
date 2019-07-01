namespace Squalr.Cli.CommandHandlers.Results
{
    using CommandLine;
    using Squalr.Engine;
    using System;

    public class ResultsCommandHandler : ICommandHandler
    {
        public void TryHandle(ref Session session, Command command)
        {
            if (!command.Name.Equals("res", StringComparison.OrdinalIgnoreCase) &&
                !command.Name.Equals("result", StringComparison.OrdinalIgnoreCase) &&
                !command.Name.Equals("results", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            Parser.Default.ParseArguments<ResultsListOptions>(command.Args)
                .MapResult(
                    (ResultsListOptions options) => options.Handle(),
                    errs => 1
                );

            command.Handled = true;
        }
    }
    //// End class
}
//// End namespace
