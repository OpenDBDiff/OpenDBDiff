using System;
using System.Windows;
using System.Windows.Controls;
using OpenDBDiff.Wpf.ViewModels;

namespace OpenDBDiff.Wpf.Views
{
    /// <summary>
    /// One database connection panel.
    /// </summary>
    /// <remarks>
    /// The handlers here are for events WPF has no bindable equivalent for. The database list is read
    /// from the drop-down handler, as the WinForms control did, but that read is asynchronous - so the
    /// popup is reopened once the list arrives, otherwise the first drop-down shows an empty list and
    /// the databases appear only on the second attempt.
    /// </remarks>
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

        private void Server_DropDownOpened(object sender, EventArgs e)
        {
            ViewModel?.LoadServers();
        }

        private async void Database_DropDownOpened(object sender, EventArgs e)
        {
            if (ViewModel is null || sender is not ComboBox combo)
            {
                return;
            }

            var needsReading = !ViewModel.HasDatabases;

            await ViewModel.LoadDatabasesAsync().ConfigureAwait(true);

            // The popup opened before the read finished; open it again now there is something in it.
            if (needsReading && !combo.IsDropDownOpen && combo.IsKeyboardFocusWithin)
            {
                combo.IsDropDownOpen = true;
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
