using System.Windows;
using System.Windows.Controls;
using OpenDBDiff.Wpf.ViewModels;

namespace OpenDBDiff.Wpf.Views
{
    /// <summary>
    /// One database connection panel. The three handlers are for events with no bindable equivalent:
    /// the combo boxes are filled when they are opened, as the WinForms control did on drop-down, and
    /// a PasswordBox cannot bind its content.
    /// </summary>
    public partial class ConnectionPanel : UserControl
    {
        public ConnectionPanel()
        {
            InitializeComponent();
        }

        private ConnectionViewModel? ViewModel
        {
            get { return DataContext as ConnectionViewModel; }
        }

        private async void Server_DropDownOpened(object sender, System.EventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.LoadServers();
                await ViewModel.LoadDatabasesAsync().ConfigureAwait(true);
            }
        }

        private async void Database_DropDownOpened(object sender, System.EventArgs e)
        {
            if (ViewModel != null)
            {
                await ViewModel.LoadDatabasesAsync().ConfigureAwait(true);
            }
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && sender is PasswordBox box)
            {
                ViewModel.Password = box.Password;
            }
        }
    }
}
