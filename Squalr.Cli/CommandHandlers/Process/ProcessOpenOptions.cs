namespace Squalr.Cli
{
    using CommandLine;
    using System;

    [Verb("open", HelpText = "Opens a running process.")]
    public class ProcessOpenOptions
    {
        public static Int32 Handle(ProcessOpenOptions options)
        {
            if (options.ProcessId == null)
            {
                Console.WriteLine("[Error] - Please specify a process id.");

                return 1;
            }

            return 0;
        }

        [Option('n', "non-invasive", Required = false, HelpText = "Non-invasive attach")]
        public Boolean NonInvasive { get; private set; }

        [Value(0, MetaName = "pid", HelpText = "The process id to open")]
        public Int32? ProcessId { get; set; }
    }
    //// End class
}
//// End namespace
