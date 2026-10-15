using System.Windows;
using System.Windows.Controls;
using OpenDBDiff.Wpf.ViewModels;

namespace OpenDBDiff.Wpf.Views
{
    /// <summary>
    /// The confirmation dialog (section 6.8). Cancel carries IsDefault so that pressing Enter does not
    /// apply anything.
    /// </summary>
    public partial class ConfirmationView : UserControl
    {
        public ConfirmationView()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ConfirmationViewModel)?.Cancel();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ConfirmationViewModel)?.Apply();
        }
    }
}
