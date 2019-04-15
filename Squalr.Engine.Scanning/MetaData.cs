namespace Squalr.Engine.Scanning
{
    using Squalr.Engine.Common.Logging;
    using Squalr.Engine.Scanning.Windows;
    using System;
    using System.Threading;

    public static class MetaData
    {
        /// <summary>
        /// Singleton instance of the <see cref="IMetaData"/> class.
        /// </summary>
        private static Lazy<IMetaData> windowsMemoryReaderInstance = new Lazy<IMetaData>(
            () => { return new WindowsMetaData(); },
            LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Gets the default meta data reader for the target process.
        /// </summary>
        public static IMetaData Default
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
                        return windowsMemoryReaderInstance.Value;
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

        /// <summary>
        /// Gets the default meta data reader for the0. windows operating system.
        /// </summary>
        public static IMetaData Windows
        {
            get
            {
                return windowsMemoryReaderInstance.Value;
            }
        }
    }
    //// End class
}
//// End namespace