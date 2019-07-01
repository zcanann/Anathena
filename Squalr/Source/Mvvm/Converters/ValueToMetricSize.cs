namespace Squalr.Source.Mvvm.Converters
{
    using Squalr.Engine.Common;
    using Squalr.Engine.Common.DataTypes;
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Converts a size in bytes to the metric size (B, KB, MB, GB, TB, PB, EB).
    /// </summary>
    public class ValueToMetricSize : IValueConverter
    {
        /// <summary>
        /// Converts an Icon to a BitmapSource.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <param name="targetType">Type to convert to.</param>
        /// <param name="parameter">Optional conversion parameter.</param>
        /// <param name="culture">Globalization info.</param>
        /// <returns>Object with type of BitmapSource. If conversion cannot take place, returns null.</returns>
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            return Conversions.ValueToMetricSize((UInt64)System.Convert.ChangeType(value, DataTypeBase.UInt64));
        }

        /// <summary>
        /// Not used or implemented.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <param name="targetType">Type to convert to.</param>
        /// <param name="parameter">Optional conversion parameter.</param>
        /// <param name="culture">Globalization info.</param>
        /// <returns>Throws see <see cref="NotImplementedException" />.</returns>
        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    //// End class
}
//// End namespace