namespace Squalr.Engine.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Extension methods for all objects.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Prints the method that called this function, as well as any provided parameters.
        /// </summary>
        /// <param name="self">The object for which to print the debug tag.</param>
        /// <param name="callerName">The function from which this method was invoked.</param>
        /// <param name="parameters">Any aditional parameters to print.</param>
        /// <returns>Returns the same object being operated on, allowing for lock(Object.PrintDebugTag()) for lock debugging.</returns>
        public static Object PrintDebugTag(this Object self, [CallerMemberName] String callerName = "", params String[] parameters)
        {
            // Write calling class and method name
            String tag = "[" + self.GetType().Name + "] - " + callerName;

            // Write parameters
            if (parameters.Length > 0)
            {
                (new List<String>(parameters)).ForEach(x => tag += " " + x);
            }

            Console.WriteLine(tag);

            return self;
        }
    }
    //// End calss
}
//// End namespace