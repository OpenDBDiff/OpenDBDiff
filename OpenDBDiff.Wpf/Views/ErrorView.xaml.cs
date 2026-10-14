using System.Windows;
using System.Windows.Controls;
using OpenDBDiff.Wpf.ViewModels;

namespace OpenDBDiff.Wpf.Views
{
    /// <summary>
    /// The error report. Everything the WinForms ErrorForm showed, plus a copy button so the detail
    /// can be pasted into an issue.
    /// </summary>
    public partial class ErrorView : UserControl
    {
        public ErrorView()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ErrorViewModel)?.Close();
        }
    }
}
