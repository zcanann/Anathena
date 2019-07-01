namespace Squalr.Engine.Common
{
    using Squalr.Engine.Common.DataTypes;
    using Squalr.Engine.Common.Extensions;
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Collection of methods to convert values from one format to another format.
    /// </summary>
    public class Conversions
    {
        /// <summary>
        /// Parse a string containing a non-hex value and return the value.
        /// </summary>
        /// <param name="dataType">The type the string represents.</param>
        /// <param name="value">The string to convert.</param>
        /// <returns>The value converted from the given string.</returns>
        public static Object ParsePrimitiveStringAsPrimitive(DataTypeBase dataType, String value)
        {
            switch (dataType)
            {
                case DataTypeBase type when type == DataTypeBase.Byte:
                    return Byte.Parse(value);
                case DataTypeBase type when type == DataTypeBase.Char:
                    return Byte.Parse(value);
                case DataTypeBase type when type == DataTypeBase.SByte:
                    return SByte.Parse(value);
                case DataTypeBase type when type == DataTypeBase.Int16:
                    return Int16.Parse(value);
                case DataTypeBase type when type == DataTypeBase.Int32:
                    return Int32.Parse(value);
                case DataTypeBase type when type == DataTypeBase.Int64:
                    return Int64.Parse(value);
                case DataTypeBase type when type == DataTypeBase.UInt16:
                    return UInt16.Parse(value);
                case DataTypeBase type when type == DataTypeBase.UInt32:
                    return UInt32.Parse(value);
                case DataTypeBase type when type == DataTypeBase.UInt64:
                    return UInt64.Parse(value);
                case DataTypeBase type when type == DataTypeBase.Single:
                    return Single.Parse(value);
                case DataTypeBase type when type == DataTypeBase.Double:
                    return Double.Parse(value);
                case DataTypeBase type when type == DataTypeBase.IntPtr:
                    return !Environment.Is64BitProcess ? new IntPtr(Int32.Parse(value)) : new IntPtr(Int64.Parse(value));
                case DataTypeBase type when type == DataTypeBase.UIntPtr:
                    return !Environment.Is64BitProcess ? new UIntPtr(UInt32.Parse(value)) : new UIntPtr(UInt64.Parse(value));
                default:
                    return null;
            }
        }

        /// <summary>
        /// Converts a string containing hex characters to the given data type.
        /// </summary>
        /// <param name="dataType">The type to convert the parsed hex to.</param>
        /// <param name="value">The hex string to parse.</param>
        /// <returns>The converted value from the hex.</returns>
        public static Object ParseHexStringAsPrimitive(DataTypeBase dataType, String value)
        {
            return ParsePrimitiveStringAsPrimitive(dataType, ParseHexStringAsPrimitiveString(dataType, value));
        }

        /// <summary>
        /// Parses a raw value as a hex string.
        /// </summary>
        /// <param name="dataType">The data type of the value.</param>
        /// <param name="value">The raw value.</param>
        /// <param name="signHex">Whether to sign the hex value for signed interger types.</param>
        /// <returns>The converted hex string.</returns>
        public static String ParsePrimitiveAsHexString(DataTypeBase dataType, Object value, Boolean signHex = false)
        {
            return ParsePrimitiveStringAsHexString(dataType, value?.ToString(), signHex);
        }

        /// <summary>
        /// Converts a string containing dec characters to the hex equivalent for the given data type.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <param name="value">The hex string to parse.</param>
        /// <param name="signHex">Whether to sign the hex value for signed interger types.</param>
        /// <returns>The converted value from the hex.</returns>
        public static String ParsePrimitiveStringAsHexString(DataTypeBase dataType, String value, Boolean signHex = false)
        {
            Object realValue = ParsePrimitiveStringAsPrimitive(dataType, value);

            switch (dataType)
            {
                case DataTypeBase type when type == DataTypeBase.Byte || type == DataTypeBase.Char:
                    return (signHex && (Byte)realValue < 0) ? ("-" + (-(Byte)realValue).ToString("X")) : ((Byte)realValue).ToString("X");
                case DataTypeBase type when type == DataTypeBase.SByte:
                    return ((SByte)realValue).ToString("X");
                case DataTypeBase type when type == DataTypeBase.Int16:
                    return (signHex && (Int16)realValue < 0) ? ("-" + (-(Int16)realValue).ToString("X")) : ((Int16)realValue).ToString("X");
                case DataTypeBase type when type == DataTypeBase.Int32:
                    return (signHex && (Int32)realValue < 0) ? ("-" + (-(Int32)realValue).ToString("X")) : ((Int32)realValue).ToString("X");
                case DataTypeBase type when type == DataTypeBase.Int64:
                    return (signHex && (Int64)realValue < 0) ? ("-" + (-(Int64)realValue).ToString("X")) : ((Int64)realValue).ToString("X");
                case DataTypeBase type when type == DataTypeBase.UInt16:
                    return ((UInt16)realValue).ToString("X");
                case DataTypeBase type when type == DataTypeBase.UInt32:
                    return ((UInt32)realValue).ToString("X");
                case DataTypeBase type when type == DataTypeBase.UInt64:
                    return ((UInt64)realValue).ToString("X");
                case DataTypeBase type when type == DataTypeBase.Single:
                    return BitConverter.ToUInt32(BitConverter.GetBytes((Single)realValue), 0).ToString("X");
                case DataTypeBase type when type == DataTypeBase.Double:
                    return BitConverter.ToUInt64(BitConverter.GetBytes((Double)realValue), 0).ToString("X");
                case DataTypeBase type when type == DataTypeBase.IntPtr:
                    return ((IntPtr)realValue).ToString("X");
                case DataTypeBase type when type == DataTypeBase.UIntPtr:
                    return ((UIntPtr)realValue).ToIntPtr().ToString("X");
                default:
                    return null;
            }
        }

        /// <summary>
        /// Converts a string containing hex characters to the dec equivalent for the given data type.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <param name="value">The dec string to parse.</param>
        /// <returns>The converted value from the dec.</returns>
        public static String ParseHexStringAsPrimitiveString(DataTypeBase dataType, String value)
        {
            Boolean signedHex = false;

            switch (dataType)
            {
                case DataTypeBase type when type == DataTypeBase.Byte || type == DataTypeBase.Int16 || type == DataTypeBase.Int32 || type == DataTypeBase.Int64:
                    if (value.StartsWith("-"))
                    {
                        value = value.Substring(1);
                        signedHex = true;
                    }

                    break;
                default:
                    break;
            }

            UInt64 realValue = Conversions.AddressToValue(value);
            String result = String.Empty;

            // Negate the parsed value if the hex string is signed
            if (signedHex)
            {
                realValue = (-realValue.ToInt64()).ToUInt64();
            }

            switch (dataType)
            {
                case DataTypeBase type when type == DataTypeBase.Byte:
                    return realValue.ToString();
                case DataTypeBase type when type == DataTypeBase.Char:
                    return realValue.ToString();
                case DataTypeBase type when type == DataTypeBase.SByte:
                    return unchecked((SByte)realValue).ToString();
                case DataTypeBase type when type == DataTypeBase.Int16:
                    return unchecked((Int16)realValue).ToString();
                case DataTypeBase type when type == DataTypeBase.Int32:
                    return unchecked((Int32)realValue).ToString();
                case DataTypeBase type when type == DataTypeBase.Int64:
                    return unchecked((Int64)realValue).ToString();
                case DataTypeBase type when type == DataTypeBase.UInt16:
                    return realValue.ToString();
                case DataTypeBase type when type == DataTypeBase.UInt32:
                    return realValue.ToString();
                case DataTypeBase type when type == DataTypeBase.UInt64:
                    return realValue.ToString();
                case DataTypeBase type when type == DataTypeBase.Single:
                    return BitConverter.ToSingle(BitConverter.GetBytes(unchecked((UInt32)realValue)), 0).ToString();
                case DataTypeBase type when type == DataTypeBase.Double:
                    return BitConverter.ToDouble(BitConverter.GetBytes(realValue), 0).ToString();
                case DataTypeBase type when type == DataTypeBase.IntPtr:
                    return ((IntPtr)realValue).ToString();
                case DataTypeBase type when type == DataTypeBase.UIntPtr:
                    return ((UIntPtr)realValue).ToIntPtr().ToString();
                default:
                    return null;
            }
        }

        /// <summary>
        /// Converts a given value to hex.
        /// </summary>
        /// <typeparam name="T">The data type of the value being converted.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <param name="formatAsAddress">Whether to use a zero padded address format.</param>
        /// <param name="includePrefix">Whether to include the '0x' hex prefix.</param>
        /// <returns>The value converted to hex.</returns>
        public static String ToHex<T>(T value, Boolean formatAsAddress = true, Boolean includePrefix = false)
        {
            Type dataType = value.GetType();

            // If a pointer type, parse as a long integer
            if (dataType == DataTypeBase.IntPtr)
            {
                dataType = DataTypeBase.Int64;
            }
            else if (dataType == DataTypeBase.UIntPtr)
            {
                dataType = DataTypeBase.UInt64;
            }

            String result = Conversions.ParsePrimitiveStringAsHexString(dataType, value.ToString());

            if (formatAsAddress)
            {
                if (result.Length <= 8)
                {
                    result = result.PadLeft(8, '0');
                }
                else
                {
                    result = result.PadLeft(16, '0');
                }
            }

            if (includePrefix)
            {
                result = "0x" + result;
            }

            return result;
        }

        /// <summary>
        /// Converts an address string to a raw value.
        /// </summary>
        /// <param name="address">The address hex string.</param>
        /// <returns>The raw value as a <see cref="UInt64"/></returns>
        public static UInt64 AddressToValue(String address)
        {
            if (String.IsNullOrEmpty(address))
            {
                return 0;
            }

            if (address.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                address = address.Substring("0x".Length);
            }

            address = address.TrimStart('0');

            if (String.IsNullOrEmpty(address))
            {
                return 0;
            }

            return UInt64.Parse(address, System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// Gets the size of the given data type.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>The size of the given type.</returns>
        public static Int32 SizeOf(DataTypeBase dataType)
        {
            switch (dataType)
            {
                case DataTypeBase type when type == DataTypeBase.Byte:
                    return sizeof(Byte);
                case DataTypeBase type when type == DataTypeBase.Char:
                    return sizeof(Char);
                case DataTypeBase type when type == DataTypeBase.SByte:
                    return sizeof(SByte);
                case DataTypeBase type when type == DataTypeBase.Int16:
                    return sizeof(Int16);
                case DataTypeBase type when type == DataTypeBase.Int32:
                    return sizeof(Int32);
                case DataTypeBase type when type == DataTypeBase.Int64:
                    return sizeof(Int64);
                case DataTypeBase type when type == DataTypeBase.UInt16:
                    return sizeof(UInt16);
                case DataTypeBase type when type == DataTypeBase.UInt32:
                    return sizeof(UInt32);
                case DataTypeBase type when type == DataTypeBase.UInt64:
                    return sizeof(UInt64);
                case DataTypeBase type when type == DataTypeBase.Single:
                    return sizeof(Single);
                case DataTypeBase type when type == DataTypeBase.Double:
                    return sizeof(Double);
                default:
                    return Marshal.SizeOf(dataType);
            }
        }

        /// <summary>
        /// Converts an array of bytes to an object.
        /// </summary>
        /// <typeparam name="T">The data type of the object.</typeparam>
        /// <param name="byteArray">The array of bytes.</param>
        /// <returns>The converted object.</returns>
        /// <exception cref="ArgumentException">If unable to handle the conversion.</exception>
        public static T BytesToObject<T>(Byte[] byteArray)
        {
            DataTypeBase dataType = typeof(T);

            switch (dataType)
            {
                case DataTypeBase type when type == typeof(Boolean):
                    return (T)(Object)BitConverter.ToBoolean(byteArray, 0);
                case DataTypeBase type when type == DataTypeBase.Byte:
                    return (T)(Object)byteArray[0];
                case DataTypeBase type when type == DataTypeBase.Char:
                    return (T)(Object)BitConverter.ToChar(byteArray, 0);
                case DataTypeBase type when type == DataTypeBase.Int16:
                    return (T)(Object)BitConverter.ToInt16(byteArray, 0);
                case DataTypeBase type when type == DataTypeBase.Int32:
                    return (T)(Object)BitConverter.ToInt32(byteArray, 0);
                case DataTypeBase type when type == DataTypeBase.Int64:
                    return (T)(Object)BitConverter.ToInt64(byteArray, 0);
                case DataTypeBase type when type == DataTypeBase.SByte:
                    return (T)(Object)unchecked((SByte)byteArray[0]);
                case DataTypeBase type when type == DataTypeBase.UInt16:
                    return (T)(Object)BitConverter.ToUInt16(byteArray, 0);
                case DataTypeBase type when type == DataTypeBase.UInt32:
                    return (T)(Object)BitConverter.ToUInt32(byteArray, 0);
                case DataTypeBase type when type == DataTypeBase.UInt64:
                    return (T)(Object)BitConverter.ToUInt64(byteArray, 0);
                case DataTypeBase type when type == DataTypeBase.Single:
                    return (T)(Object)BitConverter.ToSingle(byteArray, 0);
                case DataTypeBase type when type == DataTypeBase.Double:
                    return (T)(Object)BitConverter.ToDouble(byteArray, 0);
                default:
                    throw new ArgumentException("Invalid type provided");
            }
        }

        /// <summary>
        /// Gets the name of the specified type
        /// </summary>
        /// <param name="dataType">The type from which to get the name.</param>
        /// <returns>The name of the type.</returns>
        public static String DataTypeToName(DataTypeBase dataType)
        {
            switch (dataType)
            {
                case DataTypeBase type when type == typeof(Boolean):
                    return "Boolean";
                case DataTypeBase type when type == DataTypeBase.Byte:
                    return "Byte";
                case DataTypeBase type when type == DataTypeBase.Char:
                    return "Char";
                case DataTypeBase type when type == DataTypeBase.SByte:
                    return "SByte";
                case DataTypeBase type when type == DataTypeBase.Int16:
                    return "Int16";
                case DataTypeBase type when type == DataTypeBase.Int32:
                    return "Int32";
                case DataTypeBase type when type == DataTypeBase.Int64:
                    return "Int64";
                case DataTypeBase type when type == DataTypeBase.UInt16:
                    return "UInt16";
                case DataTypeBase type when type == DataTypeBase.UInt32:
                    return "UInt32";
                case DataTypeBase type when type == DataTypeBase.UInt64:
                    return "UInt64";
                case DataTypeBase type when type == DataTypeBase.Single:
                    return "Single";
                case DataTypeBase type when type == DataTypeBase.Double:
                    return "Double";
                default:
                    return "Unknown Type";
            }
        }

        public static String ValueToMetricSize(UInt64 value)
        {
            // Note: UInt64s run out around EB
            String[] suffix = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };

            if (value == 0)
            {
                return "0" + suffix[0];
            }

            Int32 place = Convert.ToInt32(Math.Floor(Math.Log(value, 1024)));
            Double number = Math.Round(value / Math.Pow(1024, place), 1);

            return number.ToString() + suffix[place];
        }
    }
    //// End class
}
//// End namespace