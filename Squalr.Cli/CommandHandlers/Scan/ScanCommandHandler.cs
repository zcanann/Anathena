namespace Squalr.Cli.CommandHandlers.Scan
{
    using CommandLine;
    using Squalr.Engine;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ScanCommandHandler : ICommandHandler
    {
        public void TryHandle(ref Session session, Command command)
        {
            if (SessionManager.Session == null)
            {
                Console.WriteLine("[Error] No active session to scan.");

                return;
            }

            Parser.Default.ParseArguments<NewScanOptions, NextScanOptions>(command.Args)
                .MapResult(
                    (NewScanOptions options) => options.Handle(),
                    (NextScanOptions options) => options.Handle(),
                    errs => 1
                );

            command.Handled = true;
        }

        public IEnumerable<String> GetCommandAndAliases()
        {
            return new List<String>()
            {
                "scan"
            };
        }
    }
    //// End class
}
//// End namespace
