namespace Squalr.Source.Controls
{
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using Squalr.Engine.DataTypes;
    using Squalr.Engine.Utils;
    using System;
    using System.Windows.Input;

    /// <summary>
    /// The view model for a HexDec box.
    /// </summary>
    public class HexDecBoxViewModel : ViewModelBase
    {
        /// <summary>
        /// The active text.
        /// </summary>
        private String text;

        /// <summary>
        /// The data type being represented.
        /// </summary>
        private DataType elementType;

        /// <summary>
        /// A value indicating whether the value is displayed as hex.
        /// </summary>
        private Boolean isHex;

        /// <summary>
        /// 
        /// </summary>
        public HexDecBoxViewModel()
        {
            this.DataType = DataType.Int32;

            this.ConvertDecCommand = new RelayCommand(() => this.ConvertDec());
            this.ConvertHexCommand = new RelayCommand(() => this.ConvertHex());
            this.SwitchDecCommand = new RelayCommand(() => this.SwitchDec());
            this.SwitchHexCommand = new RelayCommand(() => this.SwitchHex());
        }

        /// <summary>
        /// Gets a command to reinterpret the text as decimal.
        /// </summary>
        public ICommand SwitchDecCommand { get; private set; }

        /// <summary>
        /// Gets a command to reinterpret the text as hex.
        /// </summary>
        public ICommand SwitchHexCommand { get; private set; }

        /// <summary>
        /// Gets a command to convert the text to decimal.
        /// </summary>
        public ICommand ConvertDecCommand { get; private set; }

        /// <summary>
        /// Gets a command to convert the text to hex.
        /// </summary>
        public ICommand ConvertHexCommand { get; private set; }

        /// <summary>
        /// Gets this instance. Allows for binding to the entire view model w/ property change events (useful for converters).
        /// </summary>
        public HexDecBoxViewModel Self
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Gets or sets the active text.
        /// </summary>
        public String Text
        {
            get
            {
                return this.text;
            }

            set
            {
                if (this.text == value)
                {
                    return;
                }

                this.text = value;
                this.RaisePropertyChanged(nameof(this.Text));
                this.RaisePropertyChanged(nameof(this.IsValid));
                this.RaisePropertyChanged(nameof(this.Self));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the value is displayed as hex.
        /// </summary>
        public Boolean IsHex
        {
            get
            {
                return this.isHex;
            }

            set
            {
                this.isHex = value;
                this.RaisePropertyChanged(nameof(this.IsHex));
                this.RaisePropertyChanged(nameof(this.IsDec));
                this.RaisePropertyChanged(nameof(this.IsValid));
                this.RaisePropertyChanged(nameof(this.Self));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the value is displayed as dec.
        /// </summary>
        public Boolean IsDec
        {
            get
            {
                return !this.isHex;
            }

            set
            {
                this.isHex = !value;
                this.RaisePropertyChanged(nameof(this.IsHex));
                this.RaisePropertyChanged(nameof(this.IsDec));
                this.RaisePropertyChanged(nameof(this.IsValid));
                this.RaisePropertyChanged(nameof(this.Self));
            }
        }

        /// <summary>
        /// Gets or sets the data type being represented.
        /// </summary>
        public DataType DataType
        {
            get
            {
                return this.elementType;
            }

            set
            {
                this.elementType = value;
                this.RaisePropertyChanged(nameof(this.DataType));
                this.RaisePropertyChanged(nameof(this.IsValid));
                this.RaisePropertyChanged(nameof(this.Self));
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current value is valid for the current data type.
        /// </summary>
        public Boolean IsValid
        {
            get
            {
                if (this.IsHex && SyntaxChecker.CanParseHex(this.DataType, this.Text))
                {
                    return true;
                }
                else if (SyntaxChecker.CanParseValue(this.DataType, this.Text))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the value as its standard decimal representation.
        /// </summary>
        /// <returns>The decimal value.</returns>
        public String GetValueAsDecimal()
        {
            if (!this.IsValid)
            {
                return null;
            }

            if (this.IsHex)
            {
                return Conversions.ParseHexStringAsPrimitiveString(this.DataType, this.Text);
            }
            else
            {
                return this.Text;
            }
        }

        /// <summary>
        /// Gets the value as a hexedecimal representation.
        /// </summary>
        /// <returns>The hexedecimal value string.</returns>
        public String GetValueAsHexidecimal()
        {
            if (!this.IsValid)
            {
                return null;
            }

            if (this.IsHex)
            {
                return this.Text;
            }
            else
            {
                return Conversions.ParsePrimitiveStringAsHexString(this.DataType, this.Text);
            }
        }

        /// <summary>
        /// Gets the raw value being represented.
        /// </summary>
        /// <returns>The raw value.</returns>
        public Object GetValue()
        {
            if (!this.IsValid)
            {
                return null;
            }

            if (this.IsHex)
            {
                return Conversions.ParseHexStringAsPrimitive(this.DataType, this.Text);
            }
            else
            {
                return Conversions.ParsePrimitiveStringAsPrimitive(this.DataType, this.Text);
            }
        }

        /// <summary>
        /// Sets the raw value being represented.
        /// </summary>
        /// <param name="value">The raw value.</param>
        public void SetValue(Object value)
        {
            String valueString = value?.ToString();

            if (!SyntaxChecker.CanParseValue(this.DataType, valueString))
            {
                return;
            }

            if (this.IsHex)
            {
                this.Text = Conversions.ParsePrimitiveStringAsHexString(this.DataType, valueString);
            }
            else
            {
                this.Text = valueString;
            }
        }

        /// <summary>
        /// Reinterprets the text as decimal.
        /// </summary>
        private void SwitchDec()
        {
            this.IsHex = false;
        }

        /// <summary>
        /// Reinterprets the text as hex.
        /// </summary>
        private void SwitchHex()
        {
            this.IsHex = true;
        }

        /// <summary>
        /// Converts the text to decimal.
        /// </summary>
        private void ConvertDec()
        {
            if (SyntaxChecker.CanParseHex(this.DataType, this.Text))
            {
                this.Text = Conversions.ParseHexStringAsPrimitiveString(this.DataType, this.Text);
            }

            this.SwitchDec();
        }

        /// <summary>
        /// Converts the text to hex.
        /// </summary>
        private void ConvertHex()
        {
            if (SyntaxChecker.CanParseValue(this.DataType, this.Text))
            {
                this.Text = Conversions.ParsePrimitiveStringAsHexString(this.DataType, this.Text);
            }

            this.SwitchHex();
        }
    }
    //// End class
}
//// End namespace