namespace Squalr.Engine.Processes
{
    using Squalr.Engine.Common.Logging;
    using Squalr.Engine.Processes.OS;
    using Squalr.Engine.Processes.OS.Windows;
    using System;
    using System.Threading;

    public class ProcessQuery
    {
        /// <summary>
        /// Singleton instance of the <see cref="WindowsProcessInfo"/> class.
        /// </summary>
        private static readonly Lazy<IProcessQueryer> windowsProcessInfoInstance = new Lazy<IProcessQueryer>(
            () => { return new WindowsProcessInfo(); },
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static IProcessQueryer Instance
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
                        return ProcessQuery.windowsProcessInfoInstance.Value;
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