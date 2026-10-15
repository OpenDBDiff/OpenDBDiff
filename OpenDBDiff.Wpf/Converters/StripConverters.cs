using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Wpf.Converters
{
    /// <summary>
    /// Turns one of the model's hex colours into a brush. The connection strip draws the environment tag
    /// and the status dot from the model's palette (sections 6.1.2, 6.1.4) rather than from a second
    /// copy of the colours in XAML.
    /// </summary>
    public sealed class HexBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var hex = value as string;

            return string.IsNullOrEmpty(hex) ? Brushes.Transparent : StatusBrushConverter.Brush(hex);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// The chip's 8 px status dot (section 6.1.2): green Valid, grey Unset/Untested, red Failed,
    /// pulsing blue Testing. Colour is not the only signal - the chip's tooltip says which state it is.
    /// </summary>
    public sealed class ValidityDotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var validity = value is ConnectionValidity known ? known : ConnectionValidity.Unset;

            switch (validity)
            {
                case ConnectionValidity.Valid:
                    return StatusBrushConverter.Brush(StatusPalette.StatusForeground(DiffStatus.Create));
                case ConnectionValidity.Testing:
                    return StatusBrushConverter.Brush(StatusPalette.Accent);
                case ConnectionValidity.Failed:
                    return StatusBrushConverter.Brush(StatusPalette.StatusForeground(DiffStatus.Drop));
                default:
                    return StatusBrushConverter.Brush(StatusPalette.TextFaint);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Shows an element only while the bound value is missing or empty - the complement of
    /// <see cref="NullToVisibilityConverter"/>, which the search box's placeholder needs.
    /// </summary>
    public sealed class EmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return System.Windows.Visibility.Visible;
            }

            if (value is string text && text.Length == 0)
            {
                return System.Windows.Visibility.Visible;
            }

            return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
