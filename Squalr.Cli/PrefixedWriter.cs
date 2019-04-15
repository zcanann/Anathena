namespace Squalr.Cli
{
    using System;
    using System.IO;
    using System.Text;

    class PrefixedWriter : TextWriter
    {
        private TextWriter originalOut;

        public PrefixedWriter()
        {
            originalOut = Console.Out;
        }

        public override Encoding Encoding
        {
            get
            {
                return new ASCIIEncoding();
            }
        }

        public override void WriteLine()
        {
            this.WriteLine("");
        }

        public override void WriteLine(string message)
        {
            originalOut.WriteLine(String.Format("{0} {1}", "(Squalr) ", message));
        }
 public override void Write(string message)
        {
            originalOut.Write(String.Format("{0} {1}", "(Squalr) ", message));
        }
    }
    //// End class
}
//// End namespace
