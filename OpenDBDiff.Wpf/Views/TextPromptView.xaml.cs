using System.Windows;
using System.Windows.Controls;
using OpenDBDiff.Wpf.ViewModels;

namespace OpenDBDiff.Wpf.Views
{
    public partial class TextPromptView : UserControl
    {
        public TextPromptView()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ((TextPromptViewModel)DataContext).Cancel();
        }
    }
}
