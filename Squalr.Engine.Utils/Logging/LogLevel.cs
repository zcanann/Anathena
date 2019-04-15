namespace Squalr.Engine.Common.Logging
{
    /// <summary>
    /// The possible channels to which we can log messages.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Debugging information.
        /// </summary>
        Debug,

        /// <summary>
        /// Standard information.
        /// </summary>
        Info,

        /// <summary>
        /// Warning messages.
        /// </summary>
        Warn,

        /// <summary>
        /// Error messages.
        /// </summary>
        Error,

        /// <summary>
        /// Severe error messages.
        /// </summary>
        Fatal,
    }
    //// End enum
}
//// End namespace