using System.Windows;
using System.Windows.Controls;
using OpenDBDiff.Wpf.ViewModels;

namespace OpenDBDiff.Wpf.Views
{
    /// <summary>Script preview (section 6.7.1).</summary>
    public partial class ScriptPreviewView : UserControl
    {
        public ScriptPreviewView()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ScriptPreviewViewModel)?.Close();
        }
    }
}
