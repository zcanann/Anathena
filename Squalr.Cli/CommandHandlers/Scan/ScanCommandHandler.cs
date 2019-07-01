namespace Squalr.Cli.CommandHandlers.Scan
{
    using CommandLine;
    using Squalr.Engine;
    using System;

    public class ScanCommandHandler : ICommandHandler
    {
        public void TryHandle(ref Session session, Command command)
        {
            if (!command.Name.Equals("scan", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            command.Handled = true;

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
        }
    }
    //// End class
}
//// End namespace
