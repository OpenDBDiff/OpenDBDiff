using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Wpf.Converters
{
    /// <summary>
    /// Section 6.5.3's changed-word tint. The line carries one tint and the run inside it carries a
    /// deeper one, so a reader can see which words moved without the whole line looking rewritten.
    /// </summary>
    /// <remarks>
    /// The run knows the kind of line it sits in: word diffing only compares two strings, so the
    /// side-by-side row stamps the line's status onto its runs. That is what keeps this converter to a
    /// single binding.
    /// </remarks>
    public sealed class WordRunBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is WordRun run) || !run.IsChanged)
            {
                return Brushes.Transparent;
            }

            // Section 6.5.3's tint comes from the palette in force, so the changed words of a line are
            // drawn in the theme's colours rather than in the light theme's whatever the theme is.
            return StatusBrushConverter.Brush(Theme.Current.WordTint(run.Kind));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
