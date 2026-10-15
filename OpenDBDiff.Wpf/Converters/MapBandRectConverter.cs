using System;
using System.Globalization;
using System.Windows.Data;

namespace OpenDBDiff.Wpf.Converters
{
    /// <summary>
    /// Turns a minimap band's fractions into the pixels its border is drawn at (section 6.5).
    /// </summary>
    /// <remarks>
    /// The model gives fractions because it has no idea how tall the diff is; the strip is the thing that
    /// knows. Both values come from the same two fractions and the canvas height, and the parameter picks
    /// which one is wanted, which is why this is a multi-value converter with a parameter rather than two
    /// converters that would each have to be told the same three things.
    /// </remarks>
    public sealed class MapBandRectConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 3)
            {
                return 0.0;
            }

            var top = AsDouble(values[0]);
            var height = AsDouble(values[1]);
            var canvas = AsDouble(values[2]);
            var wanted = parameter as string;

            if (canvas <= 0)
            {
                return 0.0;
            }

            if (string.Equals(wanted, "height", StringComparison.OrdinalIgnoreCase))
            {
                // A single changed row in a long diff would be a fraction of a pixel, so a band is never
                // thinner than two: a mark nobody can see is worse than a mark that is slightly too wide.
                return Math.Max(2.0, height * canvas);
            }

            var offset = top * canvas;

            // The last band would otherwise be drawn off the bottom edge by the minimum height above.
            return Math.Max(0.0, Math.Min(offset, canvas - Math.Max(2.0, height * canvas)));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static double AsDouble(object value)
        {
            return value is double number ? number : 0.0;
        }
    }
}
