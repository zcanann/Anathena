namespace Squalr.Engine.Memory
{
    using Squalr.Engine.Common.Logging;
    using Squalr.Engine.Memory.Windows;
    using System;
    using System.Threading;

    /// <summary>
    /// Instantiates the proper memory reader based on the host OS.
    /// </summary>
    public static class MemoryReader
    {
        /// <summary>
        /// Singleton instance of the <see cref="WindowsMemoryReader"/> class.
        /// </summary>
        private static readonly Lazy<WindowsMemoryReader> windowsMemoryReaderInstance = new Lazy<WindowsMemoryReader>(
            () => { return new WindowsMemoryReader(); },
            LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Creates the memory reader for the current operating system.
        /// </summary>
        /// <param name="targetProcess">The process from which the memory reader reads memory.</param>
        /// <returns>An instance of a memory reader.</returns>
        public static IMemoryReader Instance
        {
            get
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
                        return MemoryReader.windowsMemoryReaderInstance.Value;
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
    }
    //// End class
}
//// End namespace