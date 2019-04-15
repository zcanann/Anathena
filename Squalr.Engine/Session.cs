namespace Squalr.Engine
{
    using Squalr.Engine.Common.Logging;
    using Squalr.Engine.SpeedManipulator;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public class Session
    {
        public Session(Process processToOpen)
        {
            Logger.Log(LogLevel.Info, "Attached to process: " + processToOpen?.ProcessName);

            this.OpenedProcess = processToOpen;

            this.ListenForProcessDeath();
        }

        /// <summary>
        /// Gets a reference to the target process.
        /// </summary>
        public Process OpenedProcess { get; private set; }

        public ISpeedManipulator SpeedManipulator { get; private set; }

        public void Destroy()
        {
        }

        /// <summary>
        /// Listens for process death and detaches from the process if it closes.
        /// </summary>
        private void ListenForProcessDeath()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        if (this.OpenedProcess?.HasExited ?? false)
                        {
                            this.OpenedProcess = null;
                        }
                    }
                    catch
                    {
                    }

                    await Task.Delay(200);
                }
            });
        }
    }
    //// End class
}
//// End namespace
