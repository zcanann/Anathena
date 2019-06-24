namespace Squalr.Engine.Processes
{
    using System;
    using System.Diagnostics;
    using System.Drawing;

    /// <summary>
    /// Extension methods for the .NET process class.
    /// </summary>
    public static class ProcessExtensionMethods
    {
        /// <summary>
        /// Determines if the provided process is a system process.
        /// </summary>
        /// <param name="process">The process to check.</param>
        /// <returns>A value indicating whether or not the given process is a system process.</returns>
        public static Boolean IsSystemProcess(this Process process)
        {
            return ProcessQuery.Instance.IsProcessSystemProcess(process);
        }

        /// <summary>
        /// Determines if a process has a window.
        /// </summary>
        /// <param name="process">The process to check.</param>
        /// <returns>A value indicating whether or not the given process has a window.</returns>
        public static Boolean HasWindow(this Process process)
        {
            return ProcessQuery.Instance.IsProcessWindowed(process);
        }

        /// <summary>
        /// Determines if a process is 32-bit.
        /// </summary>
        /// <param name="process">The process to check.</param>
        /// <returns>A value indicating whether or not the given process is 32-bit.</returns>
        public static Boolean Is32Bit(this Process process)
        {
            return ProcessQuery.Instance.IsProcess32Bit(process);
        }

        /// <summary>
        /// Determines if a process is 64-bit.
        /// </summary>
        /// <param name="process">The process to check.</param>
        /// <returns>A value indicating whether or not the given process is 64-bit.</returns>
        public static Boolean Is64Bit(this Process process)
        {
            return ProcessQuery.Instance.IsProcess64Bit(process);
        }

        /// <summary>
        /// Fetches the icon associated with the provided process.
        /// </summary>
        /// <param name="process">The process to check.</param>
        /// <returns>An Icon associated with the given process. Returns null if there is no icon.</returns>
        public static Icon GetIcon(this Process process)
        {
            return ProcessQuery.Instance.GetIcon(process);
        }
    }
    //// End class
}
//// End namespace
