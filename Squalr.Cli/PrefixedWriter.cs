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

        private Boolean HasPrefixedCurrentLine { get; set; }

        public void ClearHasPrefix()
        {
            this.HasPrefixedCurrentLine = false;
        }

        public override void WriteLine()
        {
            this.WriteLine(String.Empty);
        }

        public override void WriteLine(String message)
        {
            originalOut.WriteLine(String.Format("{0}{1}", this.GetPrefix(), message));

            this.HasPrefixedCurrentLine = false;

            // This will force a prefix to appear on the newline
            this.Write("");
        }

        public override void Write(String message)
        {
            originalOut.Write(String.Format("{0}{1}", this.GetPrefix(), message));

            this.HasPrefixedCurrentLine = true;
        }

        private String GetPrefix()
        {
            if (this.HasPrefixedCurrentLine)
            {
                return String.Empty;
            }
            else if (SessionManager.Session == null || SessionManager.Session.OpenedProcess == null)
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
