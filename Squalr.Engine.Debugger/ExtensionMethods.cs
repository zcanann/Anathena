namespace Squalr.Engine.Debuggers
{
    using System;

    public static class ExtensionMethods
    {
        public static UInt32 ToUInt32(this BreakpointSize breakPointSize)
        {
            switch (breakPointSize)
            {
                case BreakpointSize.B1:
                    return 1;
                case BreakpointSize.B2:
                    return 2;
                case BreakpointSize.B4:
                    return 4;
                case BreakpointSize.B8:
                    return 8;
                default:
                    throw new ArgumentException("Invalid breakpoint size enumeration value");
            }
        }

        public static BreakpointSize SizeToBreakpointSize(this IDebugger debugger, UInt32 variableSize)
        {
            if (variableSize <= 0)
            {
                return BreakpointSize.B1;
            }
            else if (variableSize <= 2)
            {
                return BreakpointSize.B2;
            }
            else if (variableSize <= 4)
            {
                return BreakpointSize.B4;
            }
            else
            {
                return BreakpointSize.B8;
            }
        }
    }
    //// End class
}
//// End namespace
