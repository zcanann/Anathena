namespace Squalr.Cli
{
    using CommandLine;
    using System;

    [Verb("new", HelpText = "Starts a new scan")]
    public class NewScanOptions
    {
        public Int32 Handle()
        {

            return 0;
        }

        [Option('d', "data-type", Required = false, HelpText = "The data type of the scan.")]
        public String DataType { get; private set; }
    }
    //// End class
}
//// End namespace
