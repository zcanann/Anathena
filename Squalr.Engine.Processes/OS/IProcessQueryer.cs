namespace Squalr.Engine.Processes.OS
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;

    /// <summary>
    /// An interface for manipulations and queries to processes on the system.
    /// </summary>
    public interface IProcessQueryer
    {
        /// <summary>
        /// Gets all running processes on the system.
        /// </summary>
        /// <returns>An enumeration of see <see cref="ExternalProcess" />.</returns>
        IEnumerable<Process> GetProcesses();

        /// <summary>
        /// Fetches the icon associated with the provided process.
        /// </summary>
        /// <param name="process">The process to check.</param>
        /// <returns>An Icon associated with the given process. Returns null if there is no icon.</returns>
        Icon GetIcon(Process process);

        /// <summary>
        /// Determines if this program is 32 bit.
        /// </summary>
        /// <returns>A boolean indicating if this program is 32 bit or not.</returns>
        Boolean IsSelf32Bit();

        /// <summary>
        /// Determines if this program is 64 bit.
        /// </summary>
        /// <returns>A boolean indicating if this program is 64 bit or not.</returns>
        Boolean IsSelf64Bit();

        /// <summary>
        /// Determines if a process is 32 bit.
        /// </summary>
        /// <param name="process">The process to check.</param>
        /// <returns>Returns true if the process is 32 bit, otherwise false.</returns>
        Boolean IsProcess32Bit(Process process);

        /// <summary>
        /// Determines if a process is 64 bit.
        /// </summary>
        /// <param name="process">The process to check.</param>
        /// <returns>Returns true if the process is 64 bit, otherwise false.</returns>
        Boolean IsProcess64Bit(Process process);

        /// <summary>
        /// Determines if a process is a system process.
        /// </summary>
        /// <param name="process">The process to check.</param>
        /// <returns>Returns true if the process is a system process, otherwise false.</returns>
        Boolean IsProcessSystemProcess(Process process);

        /// <summary>
        /// Determines if a process has a window.
        /// </summary>
        /// <param name="process">The process to check.</param>
        /// <returns>Returns true if the process has a window, otherwise false.</returns>
        Boolean IsProcessWindowed(Process process);

        /// <summary>
        /// Determines if the operating system is 32 bit.
        /// </summary>
        /// <returns>A boolean indicating if the OS is 32 bit or not.</returns>
        Boolean IsOperatingSystem32Bit();

        /// <summary>
        /// Determines if the operating system is 64 bit.
        /// </summary>
        /// <returns>A boolean indicating if the OS is 64 bit or not.</returns>
        Boolean IsOperatingSystem64Bit();
    }
    //// End interface
}
//// End namespace