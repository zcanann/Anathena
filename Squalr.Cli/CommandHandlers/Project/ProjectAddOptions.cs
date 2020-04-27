namespace Squalr.Cli.CommandHandlers.Project
{
    using CommandLine;
    using Squalr.Engine.Projects;
    using System;

    [Verb("add", HelpText = "Adds a new project item to the project. This item can come from scan results, or be a new item.")]
    public class ProjectAddOptions
    {
        public Int32 Handle()
        {
            if (SessionManager.Project == null)
            {
                Console.WriteLine("[Warn] - No project open.");

                return -1;
            }

            return 0;
        }
    }
    //// End class
}
//// End namespace
