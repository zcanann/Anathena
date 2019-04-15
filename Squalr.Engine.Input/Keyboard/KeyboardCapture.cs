namespace Squalr.Engine.Input.Keyboard
{
    using SharpDX;
    using SharpDX.DirectInput;
    using Squalr.Engine.Common.Logging;
    using Squalr.Engine.Common.Observables;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class to capture keyboard input.
    /// </summary>
    public class KeyboardCapture : IObservable<KeyState>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardCapture" /> class.
        /// </summary>
        public KeyboardCapture()
        {
            this.Observers = new HashSet<IObserver<KeyState>>();
            this.ObserverLock = new Object();
            this.FindKeyboard();
        }

        /// <summary>
        /// Gets or sets the DirectX input object to collect system input.
        /// </summary>
        private DirectInput DirectInput { get; set; }

        /// <summary>
        /// Gets or sets the keyboard input object to capture input.
        /// </summary>
        private Keyboard Keyboard { get; set; }

        /// <summary>
        /// Gets or sets the current state of the keyboard.
        /// </summary>
        private KeyboardState CurrentKeyboardState { get; set; }

        /// <summary>
        /// Gets or sets the previous state of the keyboard.
        /// </summary>
        private KeyboardState PreviousKeyboardState { get; set; }

        /// <summary>
        /// Gets or sets the observers that are observing keyboard events.
        /// </summary>
        private HashSet<IObserver<KeyState>> Observers { get; set; }

        /// <summary>
        /// Gets or sets a lock for the observer list.
        /// </summary>
        private Object ObserverLock { get; set; }

        /// <summary>
        /// Subscribes to keyboard capture events.
        /// </summary>
        /// <param name="observer">The observer to subscribe.</param>
        public IDisposable Subscribe(IObserver<KeyState> observer)
        {
            lock (this.ObserverLock)
            {
                this.Observers.Add(observer);

                return new Unsubscriber<KeyState>(this.Observers, observer);
            }
        }

        /// <summary>
        /// Unsubscribes from keyboard capture events.
        /// </summary>
        /// <param name="subject">The observer to unsubscribe.</param>
        public void Unsubscribe(IObserver<KeyState> subject)
        {
            lock (this.ObserverLock)
            {
                this.Observers.Remove(subject);
            }
        }

        /// <summary>
        /// Unsubscribes from keyboard capture events.
        /// </summary>
        /// <param name="subject">The weak observer to unsubscribe.</param>
        public void Unsubscribe(IDisposable subject)
        {
            subject?.Dispose();
        }

        /// <summary>
        /// Updates keyboard capture, gathering the input state and raising necessary events.
        /// </summary>
        public void Update()
        {
            try
            {
                this.CurrentKeyboardState = this.Keyboard.GetCurrentState();

                if (this.PreviousKeyboardState == null)
                {
                    this.PreviousKeyboardState = this.CurrentKeyboardState;
                    return;
                }

                if (this.CurrentKeyboardState == null || this.PreviousKeyboardState == null)
                {
                    return;
                }

                HashSet<Key> heldKeys = new HashSet<Key>(this.CurrentKeyboardState.PressedKeys);
                HashSet<Key> releasedKeys = new HashSet<Key>(this.PreviousKeyboardState.PressedKeys);
                HashSet<Key> pressedKeys = new HashSet<Key>(heldKeys);
                HashSet<Key> downKeys = new HashSet<Key>(this.CurrentKeyboardState.PressedKeys);

                heldKeys.ExceptWith(this.PreviousKeyboardState.PressedKeys);
                releasedKeys.ExceptWith(this.CurrentKeyboardState.PressedKeys);

                KeyState keyState = new KeyState(heldKeys, releasedKeys, downKeys, heldKeys);

                this.NotifyKeyState(keyState);

                this.PreviousKeyboardState = this.CurrentKeyboardState;
            }
            catch (SharpDXException)
            {
                try
                {
                    this.Keyboard.Acquire();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Notifies observers of the current keyboard state.
        /// </summary>
        /// <param name="keyState">The keyboard state.</param>
        public void NotifyKeyState(KeyState keyState)
        {
            lock (this.ObserverLock)
            {
                IObserver<KeyState>[] observers = this.Observers.ToArray();

                foreach (IObserver<KeyState> observer in observers)
                {
                    if (keyState == null)
                    {
                        observer.OnError(new ArgumentNullException());
                    }
                    else
                    {
                        observer.OnNext(keyState);
                    }
                }

                foreach (IObserver<KeyState> observer in observers)
                {
                    observer.OnCompleted();
                }
            }
        }


        /// <summary>
        /// Finds any connected keyboard devices.
        /// </summary>
        private void FindKeyboard()
        {
            try
            {
                this.DirectInput = new DirectInput();
                this.Keyboard = new Keyboard(this.DirectInput);
                this.Keyboard.Acquire();

                Logger.Log(LogLevel.Info, "Keyboard device found");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Unable to acquire keyboard device", ex);
            }
        }
    }
    //// End class
}
//// End namespace