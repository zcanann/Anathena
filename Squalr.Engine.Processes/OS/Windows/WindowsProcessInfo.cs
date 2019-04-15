namespace Squalr.Engine.Processes.OS.Windows
{
    using Squalr.Engine.Common.DataStructures;
    using Squalr.Engine.Processes.OS.Windows.Native;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;

    /// <summary>
    /// A class responsible for collecting all running processes on a Windows system.
    /// </summary>
    internal class WindowsProcessInfo : IProcessQueryer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsProcessInfo" /> class.
        /// </summary>
        public WindowsProcessInfo()
        {
        }

        /// <summary>
        /// Collection of process ids that have caused access issues.
        /// </summary>
        private static TtlCache<Int32> SystemProcessCache = new TtlCache<Int32>(TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(15));

        /// <summary>
        /// Collection of process ids for which an icon could not be fetched.
        /// </summary>
        private static TtlCache<Int32> NoIconProcessCache = new TtlCache<Int32>(TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(15));

        /// <summary>
        /// Collection of icons fetched from processes.
        // TODO: For now we will not expire the TTL icons. This may cause cosmetic bugs. Icons are currently not disposed,
        // so putting a TTL on this would cause a memory leak.
        /// </summary>
        private static TtlCache<Int32, Icon> IconCache = new TtlCache<Int32, Icon>();

        /// <summary>
        /// Collection of processes with a window.
        /// </summary>
        private static TtlCache<Int32> WindowedProcessCache = new TtlCache<Int32>(TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(15));

        /// <summary>
        /// Collection of processes without a window.
        /// </summary>
        private static TtlCache<Int32> NoWindowProcessCache = new TtlCache<Int32>(TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(5));

        /// <summary>
        /// Represents an empty icon.
        /// </summary>
        private const Icon NoIcon = null;

        /// <summary>
        /// Gets all running processes on the system.
        /// </summary>
        /// <returns>An enumeration of see <see cref="Process" />.</returns>
        public IEnumerable<NormalizedProcess> GetProcesses()
        {
            return Process.GetProcesses().Select(x => new NormalizedProcess(x));
        }

        /// <summary>
        /// Determines if the provided process is a system process.
        /// </summary>
        /// <param name="process">The process to check.</param>
        /// <returns>A value indicating whether or not the given process is a system process.</returns>
        public Boolean IsProcessSystemProcess(Process process)
        {
            if (WindowsProcessInfo.SystemProcessCache.Contains(process.Id))
            {
                return true;
            }

            if (process.SessionId == 0 || process.BasePriority == 13)
            {
                WindowsProcessInfo.SystemProcessCache.Add(process.Id);
                return true;
            }

            try
            {
                if (process.PriorityBoostEnabled)
                {
                    // Accessing this field will cause an access exception for system processes. This saves
                    // time because handling the exception is faster than failing to fetch the icon later
                    return false;
                }
            }
            catch
            {
                WindowsProcessInfo.SystemProcessCache.Add(process.Id);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if a process has a window.
        /// </summary>
        /// <param name="process">The process to check.</param>
        /// <returns>A value indicating whether or not the given process has a window.</returns>
        public Boolean IsProcessWindowed(Process process)
        {
            if (WindowsProcessInfo.WindowedProcessCache.Contains(process.Id))
            {
                return true;
            }

            if (WindowsProcessInfo.NoWindowProcessCache.Contains(process.Id))
            {
                return false;
            }

            // Check if the window handle is set
            if (process.MainWindowHandle != IntPtr.Zero)
            {
                WindowsProcessInfo.WindowedProcessCache.Add(process.Id);
                return true;
            }

            // Ignore system processes
            if (this.IsProcessSystemProcess(process))
            {
                WindowsProcessInfo.NoWindowProcessCache.Add(process.Id);
                return false;
            }

            // Window handle was not set, so to be certain we must enumerate the process threads, looking for window threads
            foreach (ProcessThread threadInfo in process.Threads)
            {
                if (NativeMethods.EnumWindows((IntPtr hWnd, Int32 lParam) =>
                    {
                        if (NativeMethods.GetWindowThreadProcessId(hWnd, out _) == lParam)
                        {
                            if (NativeMethods.IsWindowVisible(hWnd))
                            {
                                WindowsProcessInfo.WindowedProcessCache.Add(process.Id);
                                return true;
                            }
                        }

                        return false;
                    }, threadInfo.Id))
                {
                    return true;
                }
            }

            WindowsProcessInfo.NoWindowProcessCache.Add(process.Id);
            return false;
        }

        private static IntPtr[] GetWindowHandlesForThread(Int32 threadHandle)
        {
            List<IntPtr> results = new List<IntPtr>();

            return results.ToArray();
        }

        /// <summary>
        /// Fetches the icon associated with the provided process.
        /// </summary>
        /// <param name="process">The process to check.</param>
        /// <returns>An Icon associated with the given process. Returns null if there is no icon.</returns>
        public Icon GetIcon(Process process)
        {
            Icon icon = null;

            if (WindowsProcessInfo.NoIconProcessCache.Contains(process.Id))
            {
                return WindowsProcessInfo.NoIcon;
            }

            if (WindowsProcessInfo.IconCache.TryGetValue(process.Id, out icon))
            {
                return icon;
            }

            if (this.IsProcessSystemProcess(process))
            {
                WindowsProcessInfo.NoIconProcessCache.Add(process.Id);
                return WindowsProcessInfo.NoIcon;
            }

            try
            {
                IntPtr iconHandle = NativeMethods.ExtractIcon(process.Handle, process.MainModule.FileName, 0);

                if (iconHandle == IntPtr.Zero)
                {
                    WindowsProcessInfo.NoIconProcessCache.Add(process.Id);
                    return WindowsProcessInfo.NoIcon;
                }

                icon = Icon.FromHandle(iconHandle);
                WindowsProcessInfo.IconCache.Add(process.Id, icon);

                return icon;
            }
            catch
            {
                WindowsProcessInfo.NoIconProcessCache.Add(process.Id);
                return WindowsProcessInfo.NoIcon;
            }
        }

        /// <summary>
        /// Determines if this program is 32 bit
        /// </summary>
        /// <returns>A boolean indicating if this program is 32 bit or not</returns>
        public Boolean IsSelf32Bit()
        {
            return !Environment.Is64BitProcess;
        }

        /// <summary>
        /// Determines if this program is 64 bit
        /// </summary>
        /// <returns>A boolean indicating if this program is 64 bit or not</returns>
        public Boolean IsSelf64Bit()
        {
            return Environment.Is64BitProcess;
        }

        /// <summary>
        /// Determines if a process is 32 bit
        /// </summary>
        /// <param name="process">The process to check</param>
        /// <returns>Returns true if the process is 32 bit, otherwise false</returns>
        public Boolean IsProcess32Bit(Process process)
        {
            Boolean isWow64;

            // First do the simple check if seeing if the OS is 32 bit, in which case the process wont be 64 bit
            if (this.IsOperatingSystem32Bit())
            {
                return true;
            }

            // No process provided, assume 32 bit
            if (process == null)
            {
                return true;
            }

            try
            {
                if (process == null || !NativeMethods.IsWow64Process(process.Handle, out isWow64))
                {
                    // Error, assume 32 bit
                    return true;
                }
            }
            catch
            {
                // Error, assume 32 bit
                return true;
            }

            return isWow64;
        }

        /// <summary>
        /// Determines if a process is 64 bit
        /// </summary>
        /// <param name="process">The process to check</param>
        /// <returns>Returns true if the process is 64 bit, otherwise false</returns>
        public Boolean IsProcess64Bit(Process process)
        {
            return !this.IsProcess32Bit(process);
        }

        /// <summary>
        /// Determines if the operating system is 32 bit
        /// </summary>
        /// <returns>A boolean indicating if the OS is 32 bit or not</returns>
        public Boolean IsOperatingSystem32Bit()
        {
            return !Environment.Is64BitOperatingSystem;
        }

        /// <summary>
        /// Determines if the operating system is 64 bit
        /// </summary>
        /// <returns>A boolean indicating if the OS is 64 bit or not</returns>
        public Boolean IsOperatingSystem64Bit()
        {
            return Environment.Is64BitOperatingSystem;
        }
    }
    //// End class
}
//// End namespace