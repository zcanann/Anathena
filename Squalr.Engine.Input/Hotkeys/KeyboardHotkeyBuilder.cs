namespace Squalr.Engine.Input.HotKeys
{
    using Keyboard;
    using SharpDX.DirectInput;
    using Squalr.Engine.Common.Extensions;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A keyboard hotkey builder, which is used to construct a keyboard hotkey.
    /// </summary>
    [DataContract]
    public class KeyboardHotkeyBuilder : HotkeyBuilder, IObserver<KeyState>
    {
        /// <summary>
        /// The default delay in miliseconds between hotkey activations.
        /// </summary>
        private const Int32 DefaultActivationDelay = 150;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardHotkeyBuilder" /> class.
        /// </summary>
        /// <param name="callBackFunction">The callback function for this hotkey.</param>
        /// <param name="keyboardHotkey">The keyboard hotkey to edit.</param>
        public KeyboardHotkeyBuilder(Action callBackFunction, KeyboardHotkey keyboardHotkey = null) : base(callBackFunction)
        {
            this.SetHotkey(keyboardHotkey);

            this.Subscription = InputManager.GetInstance().GetKeyboardCapture().WeakSubscribe(this);
        }

        /// <summary>
        /// Gets or sets the hotkey being constructed.
        /// </summary>
        private KeyboardHotkey KeyboardHotkey { get; set; }

        private IDisposable Subscription { get; set; }

        public void OnNext(KeyState value)
        {
            if (value.DownKeys.IsNullOrEmpty())
            {
                return;
            }

            foreach (Key key in value.DownKeys)
            {
                this.KeyboardHotkey.AddKey(key);
            }

            this.OnHotkeysUpdated();
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }

        /// <summary>
        /// Clears the hotkeys associated with this builder.
        /// </summary>
        public void ClearHotkeys()
        {
            this.KeyboardHotkey.ClearHotkey();
            this.OnHotkeysUpdated();
        }

        public void SetHotkey(KeyboardHotkey keyboardHotkey)
        {
            this.KeyboardHotkey = (keyboardHotkey?.CopyTo(this.KeyboardHotkey) as KeyboardHotkey) ?? new KeyboardHotkey();
        }

        /// <summary>
        /// Creates a hotkey from this hotkey builder.
        /// </summary>
        /// <returns>The built hotkey.</returns>
        public override Hotkey Build(Hotkey targetHotkey)
        {
            return this.KeyboardHotkey?.CopyTo(targetHotkey);
        }

        /// <summary>
        /// Gets the string representation of the hotkey inputs.
        /// </summary>
        /// <returns>The string representation of hotkey inputs.</returns>
        public override String ToString()
        {
            return this.KeyboardHotkey.ToString();
        }
    }
    //// End class
}
//// End namespace