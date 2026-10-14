using System.Windows;
using System.Windows.Controls;
using OpenDBDiff.Wpf.ViewModels;

namespace OpenDBDiff.Wpf.Views
{
    /// <summary>
    /// The comparison window. The OK button stays disabled until the run has finished or failed, and
    /// asks its view model to close rather than closing the window itself.
    /// </summary>
    public partial class ProgressView : UserControl
    {
        public ProgressView()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ProgressViewModel)?.Close();
        }
    }
}
