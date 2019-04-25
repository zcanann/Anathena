namespace Squalr.Engine.Memory
{
    using Squalr.Engine.Common.Logging;
    using Squalr.Engine.Memory.Windows;
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Instantiates the proper memory queryer based on the host OS.
    /// </summary>
    public static class MemoryQueryerFactory
    {
        /// <summary>
        /// Creates the memory queryer for the current operating system.
        /// </summary>
        /// <param name="targetProcess">The process from which the memory queryer queries memory.</param>
        /// <returns>An instance of a memory queryer.</returns>
        public static IMemoryQueryer Create(Process targetProcess)
        {
            OperatingSystem os = Environment.OSVersion;
            PlatformID platformid = os.Platform;
            Exception ex;

            switch (platformid)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    return new WindowsMemoryQuery(targetProcess);
                case PlatformID.Unix:
                    ex = new Exception("Unix operating system is not supported");
                    break;
                case PlatformID.MacOSX:
                    ex = new Exception("MacOSX operating system is not supported");
                    break;
                default:
                    ex = new Exception("Unknown operating system");
                    break;
            }

            Logger.Log(LogLevel.Fatal, "Unsupported Operating System", ex);
            throw ex;
        }
    }
    //// End class
}
//// End namespace