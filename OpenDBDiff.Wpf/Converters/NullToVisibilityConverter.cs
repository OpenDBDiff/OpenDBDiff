using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OpenDBDiff.Wpf.Converters
{
    /// <summary>
    /// Collapses an element whose bound value is missing or empty. Used for the inline messages on the
    /// connection panel: the layout keeps the space for them, and they appear only when there is
    /// something to say.
    /// </summary>
    public sealed class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return Visibility.Collapsed;
            }

            if (value is string text && text.Length == 0)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
