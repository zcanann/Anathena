namespace Squalr.Engine.Common.Logging
{
    using System;

    /// <summary>
    /// Defines the interface for an object that observes output log events.
    /// </summary>
    public interface ILoggerObserver
    {
        /// <summary>
        /// Logs a message to output.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The log message.</param>
        /// <param name="innerMessage">The log inner message.</param>
        void OnLogEvent(LogLevel logLevel, String message, String innerMessage);
    }
    //// End interface
}
//// End namespace