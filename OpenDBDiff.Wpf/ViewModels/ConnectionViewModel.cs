using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using OpenDBDiff.SqlServer.Driver;
using OpenDBDiff.Wpf.Infrastructure;
using PropertyChanged;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>
    /// One database connection panel: server, database, authentication, credentials.
    /// </summary>
    /// <remarks>
    /// This is the WPF replacement for SqlServerConnectFront, and the three things that control did
    /// which this class has to reproduce, because leaving any of them out breaks the database list:
    /// it is read once per server and credential combination (its isDatabaseFilled flag, reset when the
    /// server, the authentication mode or the credentials change), it is read over a connection with no
    /// InitialCatalog so a database that happens to be offline cannot stop it, and a failed read is
    /// reported rather than left as an unexplained empty list.
    /// </remarks>
    [AddINotifyPropertyChangedInterface]
    public sealed class ConnectionViewModel
    {
        private bool isServerFilled;
        private bool isDatabaseFilled;

        public ConnectionViewModel(string caption)
        {
            Caption = caption;
            ServerName = "(local)";
            AuthenticationIndex = 0;
            UserName = "sa";
            Password = string.Empty;
            DatabaseName = string.Empty;
            TestCommand = new AsyncRelayCommand(TestAsync, () => !IsBusy);
            Servers.Add("(local)");
        }

        /// <summary>"Source database:" or "Destination database:".</summary>
        public string Caption { get; set; }

        public string ServerName { get; set; }

        public string DatabaseName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        /// <summary>0 = Windows authentication, 1 = SQL Server authentication, as the combo box entries.</summary>
        public int AuthenticationIndex { get; set; }

        public bool UseWindowsAuthentication
        {
            get { return AuthenticationIndex == 0; }
        }

        public bool IsSqlAuthenticationEnabled
        {
            get { return AuthenticationIndex == 1; }
        }

        public bool IsBusy { get; private set; }

        /// <summary>True once the database list has been read for the current server and credentials.</summary>
        public bool HasDatabases
        {
            get { return isDatabaseFilled; }
        }

        /// <summary>
        /// Why the database list is empty, when it is. Shown on the panel: the WinForms control put
        /// this in a message box, and a list that fails to fill without saying so is indistinguishable
        /// from a server that has no databases.
        /// </summary>
        public string? DatabaseError { get; private set; }

        public string? TestMessage { get; private set; }

        public string? ErrorConnection { get; private set; }

        public ObservableCollection<string> Servers { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> Databases { get; } = new ObservableCollection<string>();

        public AsyncRelayCommand TestCommand { get; }

        public string ConnectionString
        {
            get
            {
                return IsSqlAuthenticationEnabled
                    ? SqlConnectionStrings.ForDatabase(ServerName, DatabaseName, UserName, Password)
                    : SqlConnectionStrings.ForDatabase(ServerName, DatabaseName);
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    ServerName = "(local)";
                    DatabaseName = string.Empty;
                    UserName = string.Empty;
                    Password = string.Empty;
                    AuthenticationIndex = 1;
                    return;
                }

                var settings = SqlConnectionStrings.Parse(value);

                ServerName = settings.ServerName;
                AuthenticationIndex = settings.IntegratedSecurity ? 0 : 1;
                UserName = settings.UserName;
                Password = settings.Password;
                DatabaseName = settings.DatabaseName;
            }
        }

        public string Describe()
        {
            return string.Format("Server: {0}, Database: {1}", ServerName, DatabaseName);
        }

        public bool TestConnection()
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                }

                ErrorConnection = null;
                return true;
            }
            catch (Exception ex)
            {
                ErrorConnection = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Runs the connection test and reports the outcome on the panel rather than in a message
        /// box, which is the one behaviour change from the WinForms control.
        /// </summary>
        private async Task TestAsync()
        {
            IsBusy = true;

            try
            {
                var ok = await Task.Run(TestConnection).ConfigureAwait(true);
                TestMessage = ok ? "Connection successful." : ErrorConnection;
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Server names the user can pick from. Automatic discovery (the WinForms control used
        /// SqlDataSourceEnumerator) is not ported yet, so this lists the servers already in use and
        /// accepts any name typed in.
        /// </summary>
        public void LoadServers()
        {
            if (isServerFilled)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(ServerName) && !Servers.Contains(ServerName))
            {
                Servers.Add(ServerName);
            }

            isServerFilled = true;
        }

        /// <summary>
        /// Reads the database list from the server, once per server and credential combination, over a
        /// connection with no InitialCatalog - which is what the WinForms control did when its database
        /// box was dropped down.
        /// </summary>
        public async Task LoadDatabasesAsync()
        {
            if (isDatabaseFilled || IsBusy)
            {
                return;
            }

            IsBusy = true;
            DatabaseError = null;

            try
            {
                var databases = new ObservableCollection<string>();
                var connectionString = IsSqlAuthenticationEnabled
                    ? SqlConnectionStrings.ForDefaultDatabase(ServerName, UserName, Password)
                    : SqlConnectionStrings.ForDefaultDatabase(ServerName);

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(true);

                    using (var command = new SqlCommand("SELECT name FROM sys.databases ORDER BY name", connection))
                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(true))
                    {
                        while (await reader.ReadAsync().ConfigureAwait(true))
                        {
                            databases.Add(reader.GetString(0));
                        }
                    }
                }

                // Only replace the list once the read has succeeded, so a failed refresh cannot wipe a
                // list that was working.
                Databases.Clear();

                foreach (var database in databases)
                {
                    Databases.Add(database);
                }

                isDatabaseFilled = true;
            }
            catch (Exception ex)
            {
                // Kept on the panel rather than thrown: the caller is a drop-down handler and has
                // nowhere to put an exception.
                DatabaseError = string.Format("Could not list the databases on {0}: {1}", ServerName, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// The list depends on the server and the credentials, so changing either makes it stale -
        /// the same invalidation the WinForms control did from its server box and credential fields.
        /// </summary>
        private void OnServerNameChanged()
        {
            InvalidateDatabases();
        }

        private void OnAuthenticationIndexChanged()
        {
            InvalidateDatabases();
        }

        private void OnUserNameChanged()
        {
            InvalidateDatabases();
        }

        private void OnPasswordChanged()
        {
            InvalidateDatabases();
        }

        private void InvalidateDatabases()
        {
            isDatabaseFilled = false;
            DatabaseError = null;
        }
    }
}
