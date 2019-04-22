namespace Squalr.Cli
{
    using CommandLine;
    using System;

    [Verb("next", HelpText = "Starts the next scan")]
    public class NextScanOptions
    {
        public Int32 Handle()
        {
            Console.WriteLine("Alright!");
            return 0;
        }

        [Option('d', "data-type", Required = false, HelpText = "The data type of the scan.")]
        public String DataType { get; private set; }
    }
    //// End class
}
//// End namespace
