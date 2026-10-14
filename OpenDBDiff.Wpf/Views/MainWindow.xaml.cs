using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using OpenDBDiff.Wpf.ViewModels;

namespace OpenDBDiff.Wpf.Views
{
    /// <summary>
    /// The main window. There is deliberately almost no logic here: bindings do the work, and the two
    /// handlers below exist only for events that a TreeView and a TabControl do not expose as
    /// bindable properties.
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

        /// <summary>
        /// TreeView.SelectedItem is read-only, so the selection is pushed to the view model here.
        /// </summary>
        private void SchemaTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (ViewModel != null)
            {
                ViewModel.Tree.SelectedNode = e.NewValue as SchemaNodeViewModel;
            }
        }

        /// <summary>
        /// The synchronized script is regenerated for the current selection when its tab is shown,
        /// which is what the WinForms front end did in its SelectedIndexChanged handler.
        /// </summary>
        private void WorkTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabs && tabs.SelectedIndex == 1 && ViewModel != null)
            {
                ViewModel.RefreshSyncScriptCommand.Execute(null);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ViewModel?.SaveWindowState();
        }
    }
}
