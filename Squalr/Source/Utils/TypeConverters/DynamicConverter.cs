namespace Squalr.Source.Utils.TypeConverters
{
    using Squalr.Engine.Common;
    using Squalr.Engine.Common.DataTypes;
    using Squalr.Engine.Projects.Items;
    using System;
    using System.ComponentModel;
    using System.Globalization;

    /// <summary>
    /// Dynamic type converter for use in the property viewer.
    /// </summary>
    public class DynamicConverter : StringConverter
    {
        /// <summary>
        /// Converts a value to the proper dynamic type.
        /// </summary>
        /// <param name="context">Type descriptor context.</param>
        /// <param name="culture">Globalization info.</param>
        /// <param name="value">The value being converted.</param>
        /// <param name="destinationType">The target type to convert to.</param>
        /// <returns>The converted value.</returns>
        public override Object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, Object value, Type destinationType)
        {
            String valueString = (value == null) ? String.Empty : value.ToString();
            DataTypeBase dataType = (value == null) ? null : value.GetType();
            Boolean isHex = false;

            if (typeof(AddressItem).IsAssignableFrom(context?.Instance?.GetType()))
            {
                isHex = (context.Instance as AddressItem).IsValueHex;
            }

            if (value == null || !SyntaxChecker.CanParseValue(dataType, valueString))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            return isHex ? Conversions.ParsePrimitiveAsHexString(dataType, valueString) : valueString;
        }

        /// <summary>
        /// Converts an address string to the corresponding value.
        /// </summary>
        /// <param name="context">Type descriptor context.</param>
        /// <param name="culture">Globalization info.</param>
        /// <param name="value">The value being converted.</param>
        /// <returns>The converted value.</returns>
        public override Object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, Object value)
        {
            DataTypeBase dataType = null;
            Boolean isHex = false;

            if (typeof(AddressItem).IsAssignableFrom(context.Instance.GetType()))
            {
                dataType = (context.Instance as AddressItem)?.DataType;
                isHex = (context.Instance as AddressItem).IsValueHex;
            }

            if (dataType == (DataTypeBase)null || !value.GetType().IsAssignableFrom(typeof(String)))
            {
                return base.ConvertFrom(context, culture, value);
            }

            if (!(isHex ? SyntaxChecker.CanParseHex(dataType, value as String) : SyntaxChecker.CanParseValue(dataType, value as String)))
            {
                return base.ConvertFrom(context, culture, value);
            }

            return isHex ? Conversions.ParseHexStringAsPrimitive(dataType, value as String) : Conversions.ParsePrimitiveStringAsPrimitive(dataType, value as String);
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