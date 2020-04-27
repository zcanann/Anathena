namespace Squalr.Cli.CommandHandlers.Project
{
    using CommandLine;
    using System;

    [Verb("toggle", HelpText = "Toggles the specified project item")]
    public class ProjectToggleOptions
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
