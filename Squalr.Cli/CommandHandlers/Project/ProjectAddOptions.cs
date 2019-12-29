namespace Squalr.Cli.CommandHandlers.Project
{
    using CommandLine;
    using System;

    [Verb("add", HelpText = "Adds a new project item to the project. This item can come from scan results, or be a new item.")]
    public class ProjectAddOptions
    {
        public Int32 Handle()
        {
            if (SessionManager.Session == null)
            {
                Console.WriteLine("[Warn] - Not attached to any process.");

                return 1;
            }

            return 0;
        }

        [Option('a', "addresses", Required = false, HelpText = "Flag indicating that only addresses should be listed.")]
        public Boolean OnlyAddressess { get; private set; }

        [Value(0, MetaName = "result id", HelpText = "The ID of the address to add from the current scan results.")]
        public String ProcessTerm { get; set; }
    }
    //// End class
}
//// End namespace
