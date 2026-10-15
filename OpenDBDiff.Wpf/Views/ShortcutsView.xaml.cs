using System.Windows;
using System.Windows.Controls;
using OpenDBDiff.Wpf.ViewModels;

namespace OpenDBDiff.Wpf.Views
{
    public partial class ShortcutsView : UserControl
    {
        public ShortcutsView()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            ((ShortcutsViewModel)DataContext).Close();
        }
    }
}
