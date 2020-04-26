namespace Squalr.Cli.CommandHandlers.Results
{
    using CommandLine;
    using Squalr.Engine;
    using System;
    using System.Collections.Generic;

    public class ResultsCommandHandler : ICommandHandler
    {
        public String GetCommandName()
        {
            return "Results";
        }

        public void TryHandle(ref Session session, Command command)
        {
            if (SessionManager.Session == null)
            {
                Console.WriteLine("[Error] No active session from which results can be listed.");

                return;
            }

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
