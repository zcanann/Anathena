namespace Squalr.Cli.CommandHandlers.Results
{
    using CommandLine;
    using Squalr.Engine.Scanning.Snapshots;
    using System;

    [Verb("list", HelpText = "List scan results.")]
    public class ResultsListOptions
    {
        private const UInt64 PageSize = 16;

        public Int32 Handle()
        {
            Snapshot results = SessionManager.Session.SnapshotManager.GetActiveSnapshot();
            UInt64 pageStart = this.Page * ResultsListOptions.PageSize;
            UInt64 pageEnd = Math.Min(pageStart + ResultsListOptions.PageSize, results.ElementCount);
            UInt64 pageCount = results.ElementCount / ResultsListOptions.PageSize;

            Console.WriteLine("--------------------------");
            Console.WriteLine("Results for page " + this.Page + " / " + pageCount);
            Console.WriteLine("# " + "\t\t" + "Address" + "\t\t" + "Value");
            Console.WriteLine("--------------------------");

            for (UInt64 index = pageStart; index < pageEnd; index++)
            {
                Console.WriteLine(index + "\t\t" + results[index].BaseAddress + "\t\t" + results[index].LoadCurrentValue());
            }

            Console.WriteLine();

            return 0;
        }

        [Option('p', "page", Required = false, HelpText = "Specifies the page of results to list")]
        public UInt64 Page { get; private set; }
    }
    //// End class
}
//// End namespace
