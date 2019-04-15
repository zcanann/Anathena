namespace Squalr.Engine.Common.Logging
{
    using Squalr.Engine.Common.DataStructures;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Handles recieving logging events, processing them, and sending them to observers.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Gets or sets the list of output masks to apply to all logged messages.
        /// </summary>
        private static IList<LoggingMask> loggingMasks = new List<LoggingMask>();

        /// <summary>
        /// Gets or sets the set of active observers.
        /// </summary>
        private static ConcurrentHashSet<ILoggerObserver> observers = new ConcurrentHashSet<ILoggerObserver>();

        /// <summary>
        /// Subscribes the listener to logging events.
        /// </summary>
        /// <param name="listener">The object that wants to listen to logging events.</param>
        public static void Subscribe(ILoggerObserver listener)
        {
            Logger.observers.Add(listener);
        }

        /// <summary>
        /// Unsubscribes the listener from logging events.
        /// </summary>
        /// <param name="listener">The object that wants to stop listening to logging events.</param>
        public static void Unsubscribe(ILoggerObserver listener)
        {
            Logger.observers.Remove(listener);
        }

        /// <summary>
        /// Logs a message to output, filtering out sensitive text with a specific output mask.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The log message.</param>
        /// <param name="innerMessage">The log inner message.</param>
        /// <param name="outputMask">The output masking filter.</param>
        public static void Log(LogLevel logLevel, String message, String innerMessage, LoggingMask outputMask)
        {
            message = outputMask.ApplyFilter(message);
            innerMessage = outputMask.ApplyFilter(innerMessage);

            Logger.Log(logLevel, message, innerMessage);
        }

        /// <summary>
        /// Logs a message to output.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exception">An exception to be shown as the log inner message.</param>
        public static void Log(LogLevel logLevel, String message, Exception exception)
        {
            Logger.Log(logLevel, message, exception?.ToString());
        }

        /// <summary>
        /// Logs a message to output.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The log message.</param>
        /// <param name="innerMessage">The log inner message.</param>
        public static void Log(LogLevel logLevel, String message, String innerMessage = null)
        {
            foreach (LoggingMask outputMask in Logger.loggingMasks)
            {
                message = outputMask.ApplyFilter(message);
                innerMessage = outputMask.ApplyFilter(innerMessage);
            }

            foreach (ILoggerObserver observer in Logger.observers.ToList())
            {
                observer.OnLogEvent(logLevel, message, innerMessage);
            }
        }

        /// <summary>
        /// Adds a new output mask to the list of applied output masks.
        /// </summary>
        /// <param name="outputMask">The output mask to add.</param>
        public static void AddOutputMask(LoggingMask outputMask)
        {
            Logger.loggingMasks.Add(outputMask);
        }

        /// <summary>
        /// Removes an output mask from the list of applied output masks.
        /// </summary>
        /// <param name="outputMask">The output mask to remove.</param>
        public static void RemoveOutputMask(LoggingMask outputMask)
        {
            if (Logger.loggingMasks.Contains(outputMask))
            {
                Logger.loggingMasks.Remove(outputMask);
            }
        }
    }
    //// End class
}
//// End namespace
