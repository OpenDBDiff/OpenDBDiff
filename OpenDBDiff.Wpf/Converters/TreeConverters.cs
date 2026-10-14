using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using OpenDBDiff.Wpf.ViewModels;

namespace OpenDBDiff.Wpf.Converters
{
    /// <summary>
    /// Turns a tree node's meaning into the brush the WinForms tree painted it with.
    /// </summary>
    public sealed class NodeColourToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colour = value is NodeColour node ? node : NodeColour.Default;

            return Brush(colour switch
            {
                NodeColour.Create => "Status.Create.Text",
                NodeColour.Drop => "Status.Drop",
                NodeColour.Alter => "Status.Alter",
                NodeColour.AlterWhitespace => "Status.AlterWhitespace",
                NodeColour.Rebuild => "Status.Rebuild",
                NodeColour.Mixed => "Status.Mixed",
                _ => "Status.Original"
            });
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        internal static Brush Brush(string key)
        {
            return Application.Current?.TryFindResource(key) as Brush ?? Brushes.Black;
        }
    }

    /// <summary>
    /// Background for one diff line, matching the marker colours the WinForms diff pane used.
    /// </summary>
    public sealed class DiffLineToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var kind = value is DiffLineKind line ? line : DiffLineKind.Unchanged;

            return kind switch
            {
                DiffLineKind.Inserted => NodeColourToBrushConverter.Brush("Diff.Inserted"),
                DiffLineKind.Deleted => NodeColourToBrushConverter.Brush("Diff.Deleted"),
                DiffLineKind.ModifiedNew => NodeColourToBrushConverter.Brush("Diff.Modified.New"),
                DiffLineKind.ModifiedOld => NodeColourToBrushConverter.Brush("Diff.Modified.Old"),
                _ => Brushes.Transparent
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
