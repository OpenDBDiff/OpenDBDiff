using System.Windows;
using System.Windows.Controls;
using OpenDBDiff.Wpf.ViewModels;

namespace OpenDBDiff.Wpf.Views
{
    /// <summary>
    /// The options dialog. Nothing is applied on OK: the view model writes straight into the option
    /// object, so the comparison picks the settings up the next time it runs.
    /// </summary>
    public partial class OptionsView : UserControl
    {
        public OptionsView()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as OptionsViewModel)?.Close();
        }
    }
}
