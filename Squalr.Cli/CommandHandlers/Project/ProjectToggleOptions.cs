namespace Squalr.Cli.CommandHandlers.Project
{
    using CommandLine;
    using System;

    [Verb("toggle", HelpText = "Toggles the specified project item")]
    public class ProjectToggleOptions
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
    }
    //// End class
}
//// End namespace
