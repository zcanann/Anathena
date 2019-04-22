namespace Squalr.Cli
{
    using Squalr.Engine;
    using Squalr.Engine.Common.Logging;
    using System;

    public class LogListener : ILoggerObserver
    {
        public static Session Session { get; set; }

        public void OnLogEvent(LogLevel logLevel, String message, String innerMessage)
        {
            Console.WriteLine(message);
        }
    }
    //// End class
}
//// End namespace
