namespace Squalr.Cli
{
    using CommandLine;
    using Squalr.Engine.Processes;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    [Verb("list", HelpText = "List running processes.")]
    public class ProcessListOptions
    {
        public Int32 Handle()
        {
            IEnumerable<Process> processes = ProcessQuery.Instance.GetProcesses();

            if (this.IsWindowed)
            {
                processes = processes.Where(x => x.HasWindow());
            }

            if (!String.IsNullOrEmpty(this.SearchTerm))
            {
                processes = ProcessQuery.Instance.GetProcesses().Where(x => x.ProcessName.Contains(this.SearchTerm, this.MatchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase));
            }

            if (!this.IncludeSystemProcesses)
            {
                processes = processes.Where(x => !x.IsSystemProcess());
            }

            if (this.Limit > 0)
            {
                processes = processes.Take(this.Limit);
            }

            Console.WriteLine("PID" + "\t\t" + "Process Name");
            Console.WriteLine("--------------------------");

            foreach (Process process in processes)
            {
                Console.WriteLine(process.Id + "\t\t" + process.ProcessName);
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
