namespace Squalr.Engine.SpeedManipulator
{
    using System;

    /// <summary>
    /// An interface for an object that manipulates thread speed in a target process
    /// </summary>
    public interface ISpeedManipulator
    {
        /// <summary>
        /// Sets the speed in the external process.
        /// </summary>
        /// <param name="speed">The speed multiplier.</param>
        void SetSpeed(Double speed);
    }
    //// End interface
}
//// End namespace