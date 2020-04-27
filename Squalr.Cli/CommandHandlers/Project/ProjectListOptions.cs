namespace Squalr.Cli.CommandHandlers.Project
{
    using CommandLine;
    using Squalr.Engine.Projects;
    using Squalr.Engine.Projects.Items;
    using System;

    [Verb("list", HelpText = "List current project items.")]
    public class ProjectListOptions
    {
        public Int32 Handle()
        {
            if (SessionManager.Project == null)
            {
                Console.WriteLine("[Warn] - No project open.");

                return -1;
            }

            Console.WriteLine();

            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine("Enabled " + "\t|\t" + "Name" + "\t|\t" + "Address" + "\t|\t" + "Value" + "\t|\t" + "Description");
            Console.WriteLine("------------------------------------------------------------------");

            foreach (ProjectItem next in SessionManager.Project.ProjectItems)
            {
                Console.WriteLine((next.IsEnabled ? "X" : "") + "\t|\t" + next.Name + "\t|\t" + "TODO" + "\t|\t" + "TODO" + "\t|\t" + next.Description);
            }

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
