using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using OpenDBDiff.Wpf.ViewModels;

namespace OpenDBDiff.Wpf.Views
{
    /// <summary>
    /// The shell window. Everything in it is bound except two things a Window cannot bind: the save on
    /// close, and Ctrl+F, which has to move focus to the search box.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private MainViewModel? ViewModel
        {
            get { return DataContext as MainViewModel; }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                SummaryBar.FocusSearch();
                e.Handled = true;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ViewModel?.SaveWindowState();
        }
    }
}
