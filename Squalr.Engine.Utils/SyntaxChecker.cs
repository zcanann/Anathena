namespace Squalr.Engine.Common
{
    using Squalr.Engine.Common.DataTypes;
    using System;
    using System.Globalization;

    /// <summary>
    /// A static class used to check syntax for various values and types.
    /// </summary>
    public static class SyntaxChecker
    {
        /// <summary>
        /// Checks if the provided string is a valid address.
        /// </summary>
        /// <param name="address">The address as a hex string.</param>
        /// <param name="mustBe32Bit">Whether or not the address must strictly be containable in 32 bits.</param>
        /// <returns>A boolean indicating if the address is parseable.</returns>
        public static Boolean CanParseAddress(String address, Boolean mustBe32Bit = false)
        {
            if (address == null)
            {
                return false;
            }

            // Remove 0x hex specifier
            if (address.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                address = address.Substring(2);
            }

            // Remove trailing 0s
            while (address.StartsWith("0") && address.Length > 1)
            {
                address = address.Substring(1);
            }

            if (mustBe32Bit)
            {
                return IsUInt32(address, true);
            }
            else
            {
                return IsUInt64(address, true);
            }
        }

        /// <summary>
        /// Determines if a value of the given type can be parsed from the given string.
        /// </summary>
        /// <param name="dataType">The type of the given value.</param>
        /// <param name="value">The value to be parsed.</param>
        /// <returns>A boolean indicating if the value is parseable.</returns>
        public static Boolean CanParseValue(DataType dataType, String value)
        {
            if (dataType == (DataType)null)
            {
                return false;
            }

            switch (dataType)
            {
                case DataType type when type == DataType.Byte:
                    return SyntaxChecker.IsByte(value);
                case DataType type when type == DataType.SByte:
                    return SyntaxChecker.IsSByte(value);
                case DataType type when type == DataType.Int16:
                    return SyntaxChecker.IsInt16(value);
                case DataType type when type == DataType.Int32:
                    return SyntaxChecker.IsInt32(value);
                case DataType type when type == DataType.Int64:
                    return SyntaxChecker.IsInt64(value);
                case DataType type when type == DataType.UInt16:
                    return SyntaxChecker.IsUInt16(value);
                case DataType type when type == DataType.UInt32:
                    return SyntaxChecker.IsUInt32(value);
                case DataType type when type == DataType.UInt64:
                    return SyntaxChecker.IsUInt64(value);
                case DataType type when type == DataType.Single:
                    return SyntaxChecker.IsSingle(value);
                case DataType type when type == DataType.Double:
                    return SyntaxChecker.IsDouble(value);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines if a hex value can be parsed from the given string.
        /// </summary>
        /// <param name="dataType">The type of the given value.</param>
        /// <param name="value">The value to be parsed.</param>
        /// <returns>A boolean indicating if the value is parseable as hex.</returns>
        public static Boolean CanParseHex(DataType dataType, String value)
        {
            if (value == null)
            {
                return false;
            }

            // Remove 0x hex specifier
            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(2);
            }

            // Remove trailing 0s
            while (value.StartsWith("0") && value.Length > 1)
            {
                value = value.Substring(1);
            }

            // Remove negative sign from signed integer types, as TryParse methods do not handle negative hex values
            switch (dataType)
            {
                case DataType type when type == DataType.Byte || type == DataType.Int16 || type == DataType.Int32 || type == DataType.Int64:
                    if (value.StartsWith("-"))
                    {
                        value = value.Substring(1);
                    }

                    break;
                default:
                    break;
            }

            switch (dataType)
            {
                case DataType type when type == DataType.Byte:
                    return IsByte(value, true);
                case DataType type when type == DataType.SByte:
                    return IsSByte(value, true);
                case DataType type when type == DataType.Int16:
                    return IsInt16(value, true);
                case DataType type when type == DataType.Int32:
                    return IsInt32(value, true);
                case DataType type when type == DataType.Int64:
                    return IsInt64(value, true);
                case DataType type when type == DataType.UInt16:
                    return IsUInt16(value, true);
                case DataType type when type == DataType.UInt32:
                    return IsUInt32(value, true);
                case DataType type when type == DataType.UInt64:
                    return IsUInt64(value, true);
                case DataType type when type == DataType.Single:
                    return IsSingle(value, true);
                case DataType type when type == DataType.Double:
                    return IsDouble(value, true);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines if the given string can be parsed as a byte.
        /// </summary>
        /// <param name="value">The value as a string.</param>
        /// <param name="isHex">Whether or not the value is encoded in hex.</param>
        /// <returns>A boolean indicating if the value could be parsed.</returns>
        private static Boolean IsByte(String value, Boolean isHex = false)
        {
            Byte temp;

            if (isHex)
            {
                return Byte.TryParse(value, NumberStyles.HexNumber, null, out temp);
            }
            else
            {
                return Byte.TryParse(value, out temp);
            }
        }

        /// <summary>
        /// Determines if the given string can be parsed as a signed byte.
        /// </summary>
        /// <param name="value">The value as a string.</param>
        /// <param name="isHex">Whether or not the value is encoded in hex.</param>
        /// <returns>A boolean indicating if the value could be parsed.</returns>
        private static Boolean IsSByte(String value, Boolean isHex = false)
        {
            SByte temp;

            if (isHex)
            {
                return SByte.TryParse(value, NumberStyles.HexNumber, null, out temp);
            }
            else
            {
                return SByte.TryParse(value, out temp);
            }
        }

        /// <summary>
        /// Determines if the given string can be parsed as a 16 bit integer.
        /// </summary>
        /// <param name="value">The value as a string.</param>
        /// <param name="isHex">Whether or not the value is encoded in hex.</param>
        /// <returns>A boolean indicating if the value could be parsed.</returns>
        private static Boolean IsInt16(String value, Boolean isHex = false)
        {
            Int16 temp;

            if (isHex)
            {
                return Int16.TryParse(value, NumberStyles.HexNumber, null, out temp);
            }
            else
            {
                return Int16.TryParse(value, out temp);
            }
        }

        /// <summary>
        /// Determines if the given string can be parsed as a 16 bit signed integer.
        /// </summary>
        /// <param name="value">The value as a string.</param>
        /// <param name="isHex">Whether or not the value is encoded in hex.</param>
        /// <returns>A boolean indicating if the value could be parsed.</returns>
        private static Boolean IsUInt16(String value, Boolean isHex = false)
        {
            UInt16 temp;

            if (isHex)
            {
                return UInt16.TryParse(value, NumberStyles.HexNumber, null, out temp);
            }
            else
            {
                return UInt16.TryParse(value, out temp);
            }
        }

        /// <summary>
        /// Determines if the given string can be parsed as a 32 bit integer.
        /// </summary>
        /// <param name="value">The value as a string.</param>
        /// <param name="isHex">Whether or not the value is encoded in hex.</param>
        /// <returns>A boolean indicating if the value could be parsed.</returns>
        private static Boolean IsInt32(String value, Boolean isHex = false)
        {
            Int32 temp;

            if (isHex)
            {
                return Int32.TryParse(value, NumberStyles.HexNumber, null, out temp);
            }
            else
            {
                return Int32.TryParse(value, out temp);
            }
        }

        /// <summary>
        /// Determines if the given string can be parsed as a 32 bit signed integer.
        /// </summary>
        /// <param name="value">The value as a string.</param>
        /// <param name="isHex">Whether or not the value is encoded in hex.</param>
        /// <returns>A boolean indicating if the value could be parsed.</returns>
        private static Boolean IsUInt32(String value, Boolean isHex = false)
        {
            UInt32 temp;

            if (isHex)
            {
                return UInt32.TryParse(value, NumberStyles.HexNumber, null, out temp);
            }
            else
            {
                return UInt32.TryParse(value, out temp);
            }
        }

        /// <summary>
        /// Determines if the given string can be parsed as a 64 bit integer.
        /// </summary>
        /// <param name="value">The value as a string.</param>
        /// <param name="isHex">Whether or not the value is encoded in hex.</param>
        /// <returns>A boolean indicating if the value could be parsed.</returns>
        private static Boolean IsInt64(String value, Boolean isHex = false)
        {
            Int64 temp;

            if (isHex)
            {
                return Int64.TryParse(value, NumberStyles.HexNumber, null, out temp);
            }
            else
            {
                return Int64.TryParse(value, out temp);
            }
        }

        /// <summary>
        /// Determines if the given string can be parsed as a 64 bit signed integer.
        /// </summary>
        /// <param name="value">The value as a string.</param>
        /// <param name="isHex">Whether or not the value is encoded in hex.</param>
        /// <returns>A boolean indicating if the value could be parsed.</returns>
        private static Boolean IsUInt64(String value, Boolean isHex = false)
        {
            UInt64 temp;

            if (isHex)
            {
                return UInt64.TryParse(value, NumberStyles.HexNumber, null, out temp);
            }
            else
            {
                return UInt64.TryParse(value, out temp);
            }
        }

        /// <summary>
        /// Determines if the given string can be parsed as a single precision floating point number.
        /// </summary>
        /// <param name="value">The value as a string.</param>
        /// <param name="isHex">Whether or not the value is encoded in hex.</param>
        /// <returns>A boolean indicating if the value could be parsed.</returns>
        private static Boolean IsSingle(String value, Boolean isHex = false)
        {
            Single temp;

            if (isHex && IsUInt32(value, isHex))
            {
                return Single.TryParse(Conversions.ParseHexStringAsPrimitiveString(DataType.Single, value), out temp);
            }
            else
            {
                return Single.TryParse(value, out temp);
            }
        }

        /// <summary>
        /// Determines if the given string can be parsed as a double precision floating point number.
        /// </summary>
        /// <param name="value">The value as a string.</param>
        /// <param name="isHex">Whether or not the value is encoded in hex.</param>
        /// <returns>A boolean indicating if the value could be parsed.</returns>
        private static Boolean IsDouble(String value, Boolean isHex = false)
        {
            Double temp;

            if (isHex && IsUInt64(value, isHex))
            {
                return Double.TryParse(Conversions.ParseHexStringAsPrimitiveString(DataType.Double, value), out temp);
            }
            else
            {
                return Double.TryParse(value, out temp);
            }
        }
    }
    //// End class
}
//// End namespace