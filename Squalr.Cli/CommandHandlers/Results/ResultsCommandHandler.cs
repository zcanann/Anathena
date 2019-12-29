namespace Squalr.Cli.CommandHandlers.Results
{
    using CommandLine;
    using Squalr.Engine;
    using System;
    using System.Collections.Generic;

    public class ResultsCommandHandler : ICommandHandler
    {
        public void TryHandle(ref Session session, Command command)
        {
            Parser.Default.ParseArguments<ResultsListOptions>(command.Args)
                .MapResult(
                    (ResultsListOptions options) => options.Handle(),
                    errs => 1
                );

            command.Handled = true;
        }

        public IEnumerable<String> GetCommandAndAliases()
        {
            return new List<String>()
            {
                "res",
                "result",
                "results"
            };
        }
    }
    //// End class
}
//// End namespace
