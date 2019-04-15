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

        public override void WriteLine(String message)
        {
            originalOut.WriteLine(String.Format("{0} {1}", this.GetPrefix(), message));
        }

        public override void Write(String message)
        {
            originalOut.Write(String.Format("{0} {1}", this.GetPrefix(), message));
        }

        private String GetPrefix()
        {
            if (SessionManager.Session == null || SessionManager.Session.OpenedProcess == null)
            {
                return "(Sqlr) ";
            }
            else
            {
                return "(Sqlr - " + SessionManager.Session.OpenedProcess.Id.ToString() + ") ";
            }
        }
    }
    //// End class
}
//// End namespace
