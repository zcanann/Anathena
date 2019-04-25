namespace Squalr.Engine.Debuggers
{
    using Squalr.Engine.Common.Logging;
    using Squalr.Engine.Debuggers.Windows.DebugEngine;
    using System;
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    /// Factory for obtaining an object that enables debugging of a process.
    /// </summary>
    public static class DebuggerFactory
    {
        public enum DebuggerType
        {
            Default,
            WinDbg,
        }

        /// <summary>
        /// Gets an object that enables debugging of a process.
        /// </summary>
        /// <param name="targetProcess">The process to debug.</param>
        /// <param name="debugger">The debugger implementation to use.</param>
        /// <returns>An object that enables debugging of a process.</returns>
        public static IDebugger Create(Process targetProcess, DebuggerType debugger = DebuggerType.Default)
        {
            switch (debugger)
            {
                case DebuggerType.WinDbg:
                    return new DebugEngine(targetProcess);
                case DebuggerType.Default:
                default:
                    OperatingSystem os = Environment.OSVersion;
                    PlatformID platformid = os.Platform;
                    Exception ex;

                    switch (platformid)
                    {
                        case PlatformID.Win32NT:
                        case PlatformID.Win32S:
                        case PlatformID.Win32Windows:
                        case PlatformID.WinCE:
                            return new DebugEngine(targetProcess);
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