namespace Squalr.Cli
{
    using CommandLine;
    using Squalr.Engine.Processes;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Verb("list", HelpText = "List running processes.")]
    public class ProcessListOptions
    {
        public static Int32 Handle(ProcessListOptions options)
        {
            IEnumerable<NormalizedProcess> processes = Query.Default.GetProcesses();

            if (options.IsWindowed)
            {
                processes = processes.Where(x => x.HasWindow);
            }

            if (!String.IsNullOrEmpty(options.SearchTerm))
            {
                processes = Query.Default.GetProcesses().Where(x => x.ProcessName.Contains(options.SearchTerm, options.MatchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase));
            }

            if (!options.IncludeSystemProcesses)
            {
                processes = processes.Where(x => !x.IsSystemProcess);
            }

            if (options.Limit > 0)
            {
                processes = processes.Take(options.Limit);
            }

            Console.WriteLine("PID" + "\t\t" + "Process Name");
            Console.WriteLine("--------------------------");

            foreach (NormalizedProcess process in processes)
            {
                Console.WriteLine(process.ProcessId + "\t\t" + process.ProcessName);
            }

            Console.WriteLine();

            return 0;
        }

        [Option('w', "windowed", Required = false, HelpText = "Flag indicating that the process must have a window.")]
        public Boolean IsWindowed { get; private set; }

        [Option('s', "search-term", Required = false, HelpText = "Lists processes containing the specified string in their process name.")]
        public String SearchTerm { get; private set; }

        [Option('c', "match-case", Required = false, HelpText = "A flag indicating that search terms must match exactly.")]
        public Boolean MatchCase { get; private set; }

        [Option('x', "system-processes", Required = false, HelpText = "Flag indicating that system processes should be listed. These are hidden by default.")]
        public Boolean IncludeSystemProcesses { get; private set; }

        [Option('l', "limit", Required = false, HelpText = "Specifies the maximum number of processes to print")]
        public Int32 Limit { get; private set; }
    }
    //// End class
}
//// End namespace
