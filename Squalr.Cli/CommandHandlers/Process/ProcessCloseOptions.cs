namespace Squalr.Cli.CommandHandlers.Process
{
    using CommandLine;
    using System;

    [Verb("close", HelpText = "Detaches from a running process.")]
    public class ProcessCloseOptions
    {
        public Int32 Handle()
        {
            if (SessionManager.Session == null)
            {
                Console.WriteLine("[Warn] - Not attached to any process.");

                return -1;
            }

            SessionManager.Session.Destroy();
            SessionManager.Session = null;

            Console.WriteLine("Detatched from process.");

            return 0;
        }
    }
    //// End class
}
//// End namespace
