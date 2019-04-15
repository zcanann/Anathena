namespace Squalr.Engine.SpeedManipulator
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Manipulates thread speed in a target process.
    /// </summary>
    public class SpeedManipulator : ISpeedManipulator
    {
        public SpeedManipulator(Process openedProcess)
        {
            this.OpenedProcess = openedProcess;
        }

        /// <summary>
        /// Sets the speed in the external process.
        /// </summary>
        /// <param name="speed">The speed multiplier.</param>
        public void SetSpeed(Double speed)
        {
        }

        private Process OpenedProcess { get; set; }

        public void InstallHook()
        {
            if (this.OpenedProcess == null)
            {
                return;
            }
        }

        public void UninstallHook()
        {
        }
    }
    //// End interface
}
//// End namespace