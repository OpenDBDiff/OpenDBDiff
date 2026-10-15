using System.Windows;
using System.Windows.Controls;
using OpenDBDiff.Wpf.ViewModels;

namespace OpenDBDiff.Wpf.Views
{
    /// <summary>
    /// Group rows and item rows are the same list, so one template selector is all that separates them
    /// (section 6.4.2, 6.4.3).
    /// </summary>
    public sealed class ResultsRowTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? GroupTemplate { get; set; }

        public DataTemplate? ItemTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            var row = item as ResultsRowViewModel;

            return row != null && row.Kind == ResultsRowKind.Group ? GroupTemplate : ItemTemplate;
        }
    }
}
