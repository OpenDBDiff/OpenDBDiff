using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using OpenDBDiff.Wpf.Infrastructure;
using PropertyChanged;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>
    /// One database connection panel: server, database, authentication, credentials.
    /// </summary>
    /// <remarks>
    /// This is the WPF replacement for SqlServerConnectFront. The connection string is built and
    /// parsed exactly as that control did — Windows authentication sets IntegratedSecurity and
    /// TrustServerCertificate, SQL authentication adds UserID and Password — so saved connection
    /// strings and the reconnect behaviour stay compatible.
    /// </remarks>
    [AddINotifyPropertyChangedInterface]
    public sealed class ConnectionViewModel
    {
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

        public string? TestMessage { get; private set; }

        public string? ErrorConnection { get; private set; }

        public ObservableCollection<string> Servers { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> Databases { get; } = new ObservableCollection<string>();

        public AsyncRelayCommand TestCommand { get; }

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
        /// SqlDataSourceEnumerator) is not ported yet, so this starts with the local instance and
        /// accepts any name typed in; the list also collects the servers already in use.
        /// </summary>
        public void LoadServers()
        {
            if (!string.IsNullOrWhiteSpace(ServerName) && !Servers.Contains(ServerName))
            {
                Servers.Add(ServerName);
            }
        }

        public string ConnectionString
        {
            get
            {
                return IsSqlAuthenticationEnabled
                    ? Build(ServerName, DatabaseName, UserName, Password)
                    : Build(ServerName, DatabaseName);
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

                var builder = new SqlConnectionStringBuilder(value);

                ServerName = builder.DataSource;
                AuthenticationIndex = builder.IntegratedSecurity ? 0 : 1;

                if (builder.IntegratedSecurity)
                {
                    UserName = string.Empty;
                    Password = string.Empty;
                }
                else
                {
                    UserName = builder.UserID;
                    Password = builder.Password;
                }

                DatabaseName = builder.InitialCatalog;
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

        private static string Build(string server, string? database)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = server.Trim(),
                IntegratedSecurity = true,
                TrustServerCertificate = true
            };

            // Without a database the connection lands on the login's default database.
            if (!string.IsNullOrEmpty(database))
            {
                builder.InitialCatalog = database!.Trim();
            }

            return builder.ConnectionString;
        }

        private static string Build(string server, string? database, string? username, string? password)
        {
            var builder = new SqlConnectionStringBuilder(Build(server, database))
            {
                IntegratedSecurity = false,
                UserID = username,
                Password = password
            };

            return builder.ConnectionString;
        }

        /// <summary>
        /// Fills the database list from the server, the way the server combo did when it was dropped
        /// down. Errors are reported through <see cref="ErrorConnection"/> instead of a message box.
        /// </summary>
        public async Task LoadDatabasesAsync()
        {
            IsBusy = true;

            try
            {
                Databases.Clear();

                using (var connection = new SqlConnection(Build(ServerName, null, AuthenticationIndex == 1 ? UserName : null,
                                                                 AuthenticationIndex == 1 ? Password : null)))
                {
                    await connection.OpenAsync().ConfigureAwait(true);

                    using (var command = new SqlCommand("SELECT name FROM sys.databases ORDER BY name", connection))
                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(true))
                    {
                        while (await reader.ReadAsync().ConfigureAwait(true))
                        {
                            Databases.Add(reader.GetString(0));
                        }
                    }
                }

                ErrorConnection = null;
            }
            catch (Exception ex)
            {
                Databases.Clear();
                ErrorConnection = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
