namespace Squalr.Cli
{
    using System;
    using System.IO;

    class PrefixedReader : TextReader
    {
        private TextReader originalIn;

        public PrefixedReader(PrefixedWriter prefixedWriter)
        {
            this.PrefixedWriter = prefixedWriter;
            originalIn = Console.In;
        }

        private PrefixedWriter PrefixedWriter { get; set; }

        public override String ReadLine()
        {
            String input = originalIn.ReadLine();

            this.PrefixedWriter.ClearHasPrefix();

            return input;
        }
    }
    //// End class
}
//// End namespace
