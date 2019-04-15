namespace Squalr.Engine.Memory
{
    using Squalr.Engine.Common.Logging;
    using Squalr.Engine.Memory.Windows;
    using System;
    using System.Threading;

    public static class Allocator
    {
        /// <summary>
        /// Singleton instance of the <see cref="WindowsMemoryAllocator"/> class.
        /// </summary>
        private static Lazy<IMemoryAllocator> allocatorImpl = new Lazy<IMemoryAllocator>(
            () => { return new WindowsMemoryAllocator(); },
            LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Gets the default memory allocator for the current operating system.
        /// </summary>
        public static IMemoryAllocator Default
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
                        return allocatorImpl.Value;
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