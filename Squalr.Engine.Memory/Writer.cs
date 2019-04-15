namespace Squalr.Engine.Memory
{
    using Squalr.Engine.Common.Logging;
    using Squalr.Engine.Memory.Windows;
    using System;
    using System.Threading;

    public static class Writer
    {
        /// <summary>
        /// Singleton instance of the <see cref="WindowsMemoryWriter"/> class.
        /// </summary>
        private static Lazy<IMemoryWriter> windowsMemoryWriterInstance = new Lazy<IMemoryWriter>(
            () => { return new WindowsMemoryWriter(); },
            LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Gets the default memory writer for the current operating system.
        /// </summary>
        public static IMemoryWriter Default
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
                        return windowsMemoryWriterInstance.Value;
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