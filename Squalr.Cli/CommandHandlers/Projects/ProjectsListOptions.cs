namespace Squalr.Cli.CommandHandlers.Projects
{
    using CommandLine;
    using Squalr.Engine.Projects;
    using System;

    [Verb("list", HelpText = "List current project items.")]
    public class ProjectsListOptions
    {
        public Int32 Handle()
        {
            Console.WriteLine();

            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("# " + "\t|\t" + "Name");
            Console.WriteLine("----------------------------------------------");

            Int32 index = 0;

            foreach (String next in ProjectQueryer.GetProjectNames())
            {
                Console.WriteLine((index++) + "\t|\t" + next);
            }

            Console.WriteLine();

            return 0;
        }
    }
    //// End class
}
//// End namespace
