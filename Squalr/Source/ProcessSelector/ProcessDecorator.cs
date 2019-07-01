namespace Squalr.Source.ProcessSelector
{
    using Squalr.Engine.Processes;
    using System;
    using System.Diagnostics;
    using System.Drawing;

    public class ProcessDecorator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessDecorator" /> class.
        /// </summary>
        /// <param name="process">The process to decorate.</param>
        /// <param name="icon">The icon associated with the process.</param>
        public ProcessDecorator(Process process)
        {
            this.Process = process;

            this.ProcessName = this.Process?.ProcessName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessDecorator" /> class. This is a constructor for a dummy process.
        /// </summary>
        /// <param name="processName">The name of the dummy process.</param>
        public ProcessDecorator(String processName)
        {
            this.Process = null;
            this.ProcessName = processName;
        }

        /// <summary>
        /// Gets the decorated process.
        /// </summary>
        public Process Process { get; private set; }

        /// <summary>
        /// Gets the id of the running process.
        /// </summary>
        public Int32 ProcessId
        {
            get
            {
                return this.Process?.Id ?? 0;
            }
        }

        /// <summary>
        /// Gets the name of the running process.
        /// </summary>
        public String ProcessName { get; private set; }

        /// <summary>
        /// Gets the time that the process began execution.
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return this.Process?.StartTime ?? DateTime.MinValue;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the process is a system process.
        /// </summary>
        public Boolean IsSystemProcess
        {
            get
            {
                return this.Process?.IsSystemProcess() ?? false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the process has a window.
        /// </summary>
        public Boolean HasWindow
        {
            get
            {
                return this.Process?.HasWindow() ?? false;
            }
        }

        /// <summary>
        /// Gets the icon associated with this process.
        /// </summary>
        public Icon Icon
        {
            get
            {
                return this.Process?.GetIcon();
            }
        }

        /// <summary>
        /// Determines if this process is the same as another process.
        /// </summary>
        /// <param name="other">The other process.</param>
        /// <returns>A value indicating if the processes are the same.</returns>
        public Boolean Equals(ProcessDecorator other)
        {
            if (other != null && this.ProcessId == other.ProcessId)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the hashcode for this object, which is the underlying process id.
        /// </summary>
        /// <returns>The hashcode for this object.</returns>
        public override Int32 GetHashCode()
        {
            return this.ProcessId;
        }
    }
    //// End class
}
//// End namespace