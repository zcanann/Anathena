namespace Squalr.Cli.CommandHandlers.Project
{
    using CommandLine;
    using System;

    [Verb("delete", HelpText = "Deletes the current project, pending confirmation.")]
    public class ProjectDeleteOptions
    {
        public Int32 Handle()
        {

            return 0;
        }

        [Option('a', "addresses", Required = false, HelpText = "Flag indicating that only addresses should be listed.")]
        public Boolean OnlyAddressess { get; private set; }
    }
    //// End class
}
//// End namespace
