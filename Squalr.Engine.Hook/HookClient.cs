namespace Squalr.Engine.Engine.Hook
{
    using Squalr.Engine.Common.Logging;
    using SqualrHookClient.Source;
    using System;

    /// <summary>
    /// Provides capability to access the hook in the target process.
    /// </summary>
    [Serializable]
    public class HookClient : HookClientBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HookClient" /> class
        /// </summary>
        public HookClient()
        {
        }

        /// <summary>
        /// Injects the hook into the specified process
        /// </summary>
        /// <param name="process">The process to inject into</param>
        public override void Inject(Int32 processId)
        {
            String channelName = null;

            /*
            // Initialize the IPC server, giving the server access to the interfaces defined here
            IChannel server = EasyHook.RemoteHooking.IpcCreateServer<HookClient>(ref channelName, WellKnownObjectMode.Singleton, this);

            try
            {
                // Inject DLL into target process
                EasyHook.RemoteHooking.Inject(
                    processId,
                    EasyHook.InjectionOptions.Default,
                    typeof(HookServer).Assembly.Location,
                    typeof(HookServer).Assembly.Location,
                    channelName);
            }
            catch (Exception ex)
            {
                Output.Log(LogLevel.Error, "Unable to Hook Process, some features may not be available", ex);
            }
            */
        }

        /// <summary>
        /// Allows the server to ping the client to ensure that it is still alive.
        /// </summary>
        public override void Ping()
        {
            Logger.Log(LogLevel.Debug, "Hook pinged client");
        }

        /// <summary>
        /// Logs a message from the server to the client.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="innerMessage">The inner log message.</param>
        public override void Log(String message, String innerMessage = null)
        {
            Logger.Log(LogLevel.Info, "[Hook] " + message, innerMessage);
        }

        /// <summary>
        /// Uninjects the hook from the external process.
        /// </summary>
        public override void Uninject()
        {
            base.Uninject();
        }
    }
    //// End class
}
//// End namespace