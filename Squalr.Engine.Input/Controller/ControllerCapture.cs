namespace Squalr.Engine.Input.Controller
{
    using SharpDX.DirectInput;
    using Squalr.Engine.Common.Logging;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Class to capture controller input.
    /// </summary>
    internal class ControllerCapture : IControllerSubject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerCapture" /> class.
        /// </summary>
        public ControllerCapture()
        {
            this.Subjects = new List<IControllerObserver>();
            this.FindController();
        }

        /// <summary>
        /// Gets or sets the DirectX input object to collect controller input.
        /// </summary>
        private DirectInput DirectInput { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the active joystick.
        /// </summary>
        private Guid JoystickGuid { get; set; }

        /// <summary>
        /// Gets or sets the joystick object.
        /// </summary>
        private Joystick Joystick { get; set; }

        /// <summary>
        /// Gets or sets the subjects that are observing controller events.
        /// </summary>
        private List<IControllerObserver> Subjects { get; set; }

        /// <summary>
        /// Updates controller capture, polling all input.
        /// </summary>
        public void Update()
        {
            if (this.Joystick == null)
            {
                return;
            }

            try
            {
                // TODO: maybe use this object
                JoystickState joystickState = this.Joystick.GetCurrentState();

                // Poll events from joystick
                this.Joystick.Poll();
                JoystickUpdate[] data = this.Joystick.GetBufferedData();

                if (data == null)
                {
                    return;
                }

                foreach (JoystickUpdate state in data)
                {
                    Console.WriteLine(state);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Subscribes to controller capture events.
        /// </summary>
        /// <param name="subject">The observer to subscribe.</param>
        public void Subscribe(IControllerObserver subject)
        {
        }

        /// <summary>
        /// Unsubscribes from controller capture events.
        /// </summary>
        /// <param name="subject">The observer to unsubscribe.</param>
        public void Unsubscribe(IControllerObserver subject)
        {
        }

        /// <summary>
        /// Finds any connected gamepad devices.
        /// </summary>
        private void FindController()
        {
            try
            {
                this.DirectInput = new DirectInput();

                // Find a Joystick Guid
                this.JoystickGuid = Guid.Empty;
                foreach (DeviceInstance deviceInstance in this.DirectInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
                {
                    this.JoystickGuid = deviceInstance.InstanceGuid;
                }

                // If Gamepad not found, look for a Joystick
                if (this.JoystickGuid == Guid.Empty)
                {
                    foreach (DeviceInstance deviceInstance in this.DirectInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
                    {
                        this.JoystickGuid = deviceInstance.InstanceGuid;
                    }
                }

                // If Joystick not found, throws an error
                if (this.JoystickGuid == Guid.Empty)
                {
                    // TODO: Resort to like xInput or something here??? IDK
                    return;
                }

                // Instantiate the joystick
                this.Joystick = new Joystick(this.DirectInput, this.JoystickGuid);

                // Query all suported ForceFeedback effects
                IList<EffectInfo> allEffects = this.Joystick.GetEffects();

                if (allEffects != null)
                {
                    foreach (EffectInfo effectInfo in allEffects)
                    {
                        Console.WriteLine("Effect available {0}", effectInfo.Name);
                    }
                }

                this.Joystick.Properties.BufferSize = 128;
                this.Joystick.Acquire();

                Logger.Log(LogLevel.Info, "Controller device found");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "No (optional) game controller found", ex);
            }
        }
    }
    //// End class
}
//// End namespace