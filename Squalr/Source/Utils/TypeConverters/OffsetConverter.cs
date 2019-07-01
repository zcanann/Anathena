namespace Squalr.Source.Utils.TypeConverters
{
    using Squalr.Engine.Common;
    using Squalr.Engine.Common.DataTypes;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Offset type converter for use in the property viewer.
    /// </summary>
    public class OffsetConverter : TypeConverter
    {
        /// <summary>
        /// Converts a value to an offset string.
        /// </summary>
        /// <param name="context">Type descriptor context.</param>
        /// <param name="culture">Globalization info.</param>
        /// <param name="value">The value being converted.</param>
        /// <param name="destinationType">The target type to convert to.</param>
        /// <returns>The converted value.</returns>
        public override Object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, Object value, Type destinationType)
        {
            if (value == null)
            {
                return "(None)";
            }

            if (value.GetType().GetInterfaces().Contains(typeof(IEnumerable)))
            {
                IEnumerable<Object> trueValue = (value as IEnumerable).Cast<Object>();

                if (trueValue.Count() <= 0)
                {
                    return "(None)";
                }

                return String.Join(", ", trueValue.Select(x => Conversions.ParsePrimitiveAsHexString(x.GetType(), x, signHex: true)));
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Converts a string to the corresponding value type.
        /// </summary>
        /// <param name="context">Type descriptor context.</param>
        /// <param name="culture">Globalization info.</param>
        /// <param name="value">The value being converted.</param>
        /// <returns>The converted value.</returns>
        public override Object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, Object value)
        {
            if (value is String)
            {
                IEnumerable<String> offsetStrings = (value as String).Split(',').Select(offset => offset.Trim());

                if (offsetStrings.All(offset => SyntaxChecker.CanParseHex(DataTypeBase.Int32, offset)))
                {
                    return offsetStrings.Select(offset => (Int32)Conversions.ParseHexStringAsPrimitive(DataTypeBase.Int32, offset)).ToArray();
                }
            }

            // Return an invalid object, such that it gets rejected by the property updater -- Note: This exception is not thrown
            return new ArgumentException();
        }

        /// <summary>
        /// Determines if this converter can convert to the given source type.
        /// </summary>
        /// <param name="context">Type descriptor context.</param>
        /// <param name="sourceType">The source type.</param>
        /// <returns>True if this converter can convert to the given type.</returns>
        public override Boolean CanConvertTo(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        /// <summary>
        /// Determines if this converter can convert from the given source type.
        /// </summary>
        /// <param name="context">Type descriptor context.</param>
        /// <param name="sourceType">The source type.</param>
        /// <returns>True if this converter can convert from the given type.</returns>
        public override Boolean CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }
    }
    //// End class
}
//// End namespace