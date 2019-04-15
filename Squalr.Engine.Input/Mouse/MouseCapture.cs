namespace Squalr.Engine.Input.Mouse
{
    using SharpDX;
    using SharpDX.DirectInput;
    using Squalr.Engine.Common.Logging;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Class to capture mouse input.
    /// </summary>
    internal class MouseCapture : IMouseSubject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MouseCapture" /> class.
        /// </summary>
        public MouseCapture()
        {
            this.Subjects = new List<IMouseObserver>();
            this.FindMouse();
        }

        /// <summary>
        /// An enumeration of possible mouse inputs.
        /// </summary>
        private enum MouseButtonEnum
        {
            /// <summary>
            /// The left mouse button.
            /// </summary>
            Left = 0,

            /// <summary>
            /// The middle mouse button.
            /// </summary>
            Middle = 1,

            /// <summary>
            /// The right mouse button.
            /// </summary>
            Right = 2,

            /// <summary>
            /// An additonal button featured on many mouses, often 'forwards' or 'backwards'.
            /// </summary>
            XButton1,

            /// <summary>
            /// An additonal button featured on many mouses, often 'forwards' or 'backwards'.
            /// </summary>
            XButton2
        }

        /// <summary>
        /// Gets or sets the DirectX input object to collect mouse input.
        /// </summary>
        private DirectInput DirectInput { get; set; }

        /// <summary>
        /// Gets or sets the mouse input object to capture input.
        /// </summary>
        private Mouse Mouse { get; set; }

        /// <summary>
        /// Gets or sets the current mouse state.
        /// </summary>
        private MouseState CurrentMouseState { get; set; }

        /// <summary>
        /// Gets or sets the previous mouse state.
        /// </summary>
        private MouseState PreviousMouseState { get; set; }

        /// <summary>
        /// Gets or sets the subjects that are observing mouse events.
        /// </summary>
        private List<IMouseObserver> Subjects { get; set; }

        /// <summary>
        /// Updates mouse capture, gathering the input state and raising necessary events.
        /// </summary>
        public void Update()
        {
            try
            {
                this.CurrentMouseState = this.Mouse.GetCurrentState();

                if (this.CurrentMouseState == null)
                {
                    return;
                }

                if (this.PreviousMouseState != null)
                {
                    if (this.CurrentMouseState.Buttons[(Int32)MouseButtonEnum.Left] == true && this.PreviousMouseState.Buttons[(Int32)MouseButtonEnum.Left] == false)
                    {
                        this.OnMouseDown(MouseButtonEnum.Left);
                    }

                    if (this.CurrentMouseState.Buttons[(Int32)MouseButtonEnum.Right] == true && this.PreviousMouseState.Buttons[(Int32)MouseButtonEnum.Right] == false)
                    {
                        this.OnMouseDown(MouseButtonEnum.Right);
                    }

                    if (this.CurrentMouseState.Buttons[(Int32)MouseButtonEnum.Left] == false && this.PreviousMouseState.Buttons[(Int32)MouseButtonEnum.Left] == true)
                    {
                        this.OnMouseUp(MouseButtonEnum.Left);
                    }
                }

                this.PreviousMouseState = this.CurrentMouseState;
            }
            catch (SharpDXException)
            {
                this.Mouse.Acquire();
            }
        }

        /// <summary>
        /// Subscribes to mouse capture events.
        /// </summary>
        /// <param name="subject">The observer to subscribe.</param>
        public void Subscribe(IMouseObserver subject)
        {
        }

        /// <summary>
        /// Unsubscribes from mouse capture events
        /// </summary>
        /// <param name="subject">The observer to unsubscribe</param>
        public void Unsubscribe(IMouseObserver subject)
        {
        }

        /// <summary>
        /// Notifies subscribers of a mouse down event.
        /// </summary>
        /// <param name="mouseButton">The mouse down button.</param>
        private void OnMouseDown(MouseButtonEnum mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButtonEnum.Left:
                    break;
                case MouseButtonEnum.Middle:
                    break;
                case MouseButtonEnum.Right:
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Notifies subscribers of a mouse up event.
        /// </summary>
        /// <param name="mouseButton">The mouse up button.</param>
        private void OnMouseUp(MouseButtonEnum mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButtonEnum.Left:
                    break;
                case MouseButtonEnum.Middle:
                    break;
                case MouseButtonEnum.Right:
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Finds any connected mouse devices.
        /// </summary>
        private void FindMouse()
        {
            try
            {
                this.DirectInput = new DirectInput();
                this.Mouse = new Mouse(this.DirectInput);
                this.Mouse.Acquire();

                Logger.Log(LogLevel.Info, "Mouse device found");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "No (optional) mouse found", ex);
            }
        }
    }
    //// End class
}
//// End namespace