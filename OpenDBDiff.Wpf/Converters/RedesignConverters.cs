using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Wpf.Converters
{
    /// <summary>
    /// Maps a status to one of its three tokens: "bg", "fg" or "border" (section 13.2). The values come
    /// from the model's palette, so the tokens have one definition, and a test checks them against the
    /// specification.
    /// </summary>
    public sealed class StatusBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = value is DiffStatus known ? known : DiffStatus.Identical;

            switch (parameter as string)
            {
                case "fg":
                    return Brush(StatusPalette.StatusForeground(status));
                case "border":
                    return Brush(StatusPalette.StatusBorder(status));
                default:
                    return Brush(StatusPalette.StatusBackground(status));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        internal static Brush Brush(string hex)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
        }
    }

    /// <summary>The badge text: CREATE, ALTER, DROP (section 6.4.3).</summary>
    public sealed class StatusBadgeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return StatusVocabulary.Badge(value is DiffStatus known ? known : DiffStatus.Identical);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>Background tint for one diff line (section 6.5.3), including the hatched filler (6.5.2).</summary>
    public sealed class DiffLineBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value is DiffLineKind kind ? kind : DiffLineKind.Unchanged)
            {
                case DiffLineKind.Added:
                    return StatusBrushConverter.Brush(StatusPalette.StatusBackground(DiffStatus.Create));
                case DiffLineKind.Removed:
                    return StatusBrushConverter.Brush(StatusPalette.StatusBackground(DiffStatus.Drop));
                case DiffLineKind.Changed:
                    return StatusBrushConverter.Brush(StatusPalette.StatusBackground(DiffStatus.Alter));
                case DiffLineKind.Filler:
                    return StatusBrushConverter.Brush(StatusPalette.SurfaceAlt);
                default:
                    return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>The disclosure glyph of a group header (section 6.4.2).</summary>
    public sealed class ExpandedGlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool expanded && expanded ? "\u25be" : "\u25b8";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>The focused row's background, distinct from its checkbox state (section 6.4.3).</summary>
    public sealed class SelectionBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool selected && selected
                ? (Brush)Application.Current.FindResource("SelectionBg")
                : Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>Grouped (0) or flat (1) for the view mode combo box.</summary>
    public sealed class GroupedViewIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool grouped && grouped ? 0 : 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is int index && index == 0;
        }
    }

    /// <summary>Shows an element only while the bound flag is false.</summary>
    public sealed class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool flag && flag ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
