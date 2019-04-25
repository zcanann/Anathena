namespace Squalr.Engine.Debuggers.Windows.DebugEngine
{
    using Microsoft.Diagnostics.Runtime.Interop;
    using Squalr.Engine.Common.Logging;
    using System;
    using System.Runtime.InteropServices;

    internal class OutputCallBacks : IDebugOutputCallbacksWide
    {
        public Int32 Output([In] DEBUG_OUTPUT Mask, [In, MarshalAs(UnmanagedType.LPWStr)] String text)
        {
            Logger.Log(LogLevel.Debug, text?.Trim());

            return 0;
        }
    }
    //// End class
}
//// End namespace
