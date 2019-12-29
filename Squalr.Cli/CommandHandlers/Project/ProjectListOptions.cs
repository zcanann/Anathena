namespace Squalr.Cli.CommandHandlers.Project
{
    using CommandLine;
    using System;

    [Verb("list", HelpText = "List current project items.")]
    public class ProjectListOptions
    {
        public Int32 Handle()
        {
            Console.WriteLine();

            return 0;
        }

        [Option('a', "addresses", Required = false, HelpText = "Flag indicating that only addresses should be listed.")]
        public Boolean OnlyAddressess { get; private set; }

        [Option('i', "instructions", Required = false, HelpText = "Flag indicating that only instructions should be listed.")]
        public Boolean OnlyInstructions { get; private set; }
    }
    //// End class
}
//// End namespace
