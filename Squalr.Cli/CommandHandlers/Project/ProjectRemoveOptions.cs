namespace Squalr.Cli.CommandHandlers.Project
{
    using CommandLine;
    using System;

    [Verb("remove", HelpText = "Removes a project item from the project.")]
    public class ProjectRemoveOptions
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
