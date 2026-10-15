using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using OpenDBDiff.SqlServer.Driver;
using OpenDBDiff.Ui.Model;
using OpenDBDiff.Wpf.Infrastructure;
using PropertyChanged;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>
    /// One database connection: the fields, its validity state and everything the strip's chip and the
    /// connection panel read (sections 6.1.1 to 6.1.4).
    /// </summary>
    /// <remarks>
    /// This is the WPF replacement for SqlServerConnectFront, and the three things that control did
    /// which this class has to reproduce, because leaving any of them out breaks the database list:
    /// it is read once per server and credential combination (its isDatabaseFilled flag, reset when the
    /// server, the authentication mode or the credentials change), it is read over a connection with no
    /// InitialCatalog so a database that happens to be offline cannot stop it, and a failed read is
    /// reported rather than left as an unexplained empty list.
    ///
    /// Phase 2 adds the state machine of section 6.1.3 on top of those fields. The rule it protects is
    /// that a successful test only counts while the fields it was run against are unchanged, which is
    /// why <see cref="MarkEdited"/> resets the state on every field edit rather than on Compare.
    /// </remarks>
    [AddINotifyPropertyChangedInterface]
    public sealed class ConnectionViewModel : Infrastructure.ObservableObject
    {
        private readonly Services.IDialogService? dialogs;

        private bool isServerFilled;
        private bool isDatabaseFilled;
        private bool isApplyingSavedConnection;

        private ConnectionValidity validity = ConnectionValidity.Unset;
        private EnvironmentKind environmentKind;
        private string customEnvironment = string.Empty;
        private string? testMessage;
        private SavedConnection? selectedSavedConnection;

        public ConnectionViewModel(string caption, Services.IDialogService? dialogs = null)
        {
            this.dialogs = dialogs;

            Caption = caption;
            SideLabel = caption.Replace(" database:", string.Empty);
            ServerName = "(local)";
            AuthenticationIndex = 0;
            UserName = "sa";
            Password = string.Empty;
            DatabaseName = string.Empty;

            TestCommand = new AsyncRelayCommand(TestAndReportAsync, () => !IsBusy);
            RetryDatabasesCommand = new RelayCommand(RetryDatabases);
            SaveAsCommand = new RelayCommand(SaveAs);
            AcceptEnvironmentSuggestionCommand = new RelayCommand(AcceptEnvironmentSuggestion);

            Servers.Add("(local)");
        }

        /// <summary>Set when the user saved this card; the shell persists the connection (section 6.1.6).</summary>
        public event EventHandler<SavedConnectionEventArgs>? Saved;

        /// <summary>Raised whenever a field changed, which is what marks results stale (section 9.6).</summary>
        public event EventHandler? Edited;

        /// <summary>"Source database:" or "Destination database:".</summary>
        public string Caption { get; set; }

        /// <summary>"Source" or "Destination", for the panel headings and the per-side progress.</summary>
        public string SideLabel { get; set; }

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

        /// <summary>
        /// Section 6.1.1 item 4: user name and password are hidden entirely under Windows
        /// authentication, rather than shown disabled as the old panel did.
        /// </summary>
        public bool ShowCredentials
        {
            get { return AuthenticationIndex == 1; }
        }

        /// <summary>The database list read is under way: the panel shows an inline spinner.</summary>
        public bool IsLoadingDatabases { get; private set; }

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

        public ObservableCollection<string> Servers { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> Databases { get; } = new ObservableCollection<string>();

        /// <summary>The shared saved-connection list, set by the shell so both cards offer the same entries.</summary>
        public ObservableCollection<SavedConnection>? SavedConnections { get; set; }

        /// <summary>
        /// The entry the card's combobox has picked (section 6.1.1 item 1): selecting one fills the
        /// fields below it.
        /// </summary>
        public SavedConnection? SelectedSavedConnection
        {
            get { return selectedSavedConnection; }
            set
            {
                selectedSavedConnection = value;

                if (value != null)
                {
                    ApplySavedConnection(value);
                }
            }
        }

        public AsyncRelayCommand TestCommand { get; }

        public RelayCommand RetryDatabasesCommand { get; }

        public RelayCommand SaveAsCommand { get; }

        public RelayCommand AcceptEnvironmentSuggestionCommand { get; }

        /// <summary>Section 6.1.3: the chip's dot, the Compare rule and the panel all read from this one state.</summary>
        public ConnectionValidity Validity
        {
            get { return validity; }
            private set
            {
                validity = value;
                RefreshValidity();
            }
        }

        public bool IsValid
        {
            get { return validity == ConnectionValidity.Valid; }
        }

        public bool IsTesting
        {
            get { return validity == ConnectionValidity.Testing; }
        }

        public bool HasTestFailure
        {
            get { return validity == ConnectionValidity.Failed; }
        }

        /// <summary>The chip's tooltip: the state, then the last result when there is one.</summary>
        public string StatusTooltip
        {
            get
            {
                var description = ConnectionRules.StatusDescription(validity);

                return string.IsNullOrEmpty(testMessage) ? description : description + " " + testMessage;
            }
        }

        /// <summary>Section 6.1.1 item 6: "Connected · SQL Server 2019 · 42 ms", or the error and category.</summary>
        public string? TestMessage
        {
            get { return testMessage; }
        }

        public string ChipText
        {
            get { return ConnectionRules.ChipText(ServerName, DatabaseName); }
        }

        public string ChipTooltip
        {
            get { return ConnectionRules.ChipTooltip(ServerName, DatabaseName); }
        }

        /// <summary>Section 6.1.2: the server is bold and capped at 24 characters.</summary>
        public string ServerDisplay
        {
            get { return ConnectionRules.ServerDisplay(ServerName); }
        }

        /// <summary>Section 6.1.2: "/ database", muted, and the part that truncates first.</summary>
        public string DatabaseDisplay
        {
            get { return string.IsNullOrWhiteSpace(DatabaseName) ? string.Empty : "/ " + DatabaseName; }
        }

        // ---- Environment tag (section 6.1.4) ------------------------------------------------------

        /// <summary>0 None, 1 Dev, 2 Test, 3 UAT, 4 Prod, 5 Custom.</summary>
        public int EnvironmentIndex
        {
            get { return (int)environmentKind; }
            set
            {
                environmentKind = (EnvironmentKind)value;
                RefreshEnvironment();
            }
        }

        public string CustomEnvironment
        {
            get { return customEnvironment; }
            set
            {
                customEnvironment = value;
                RefreshEnvironment();
            }
        }

        public EnvironmentTag Environment
        {
            get
            {
                return environmentKind == EnvironmentKind.Custom
                    ? EnvironmentTag.Custom(customEnvironment)
                    : EnvironmentTag.For(environmentKind);
            }
            set
            {
                var tag = value ?? EnvironmentTag.None;

                isApplyingSavedConnection = true;
                environmentKind = tag.Kind;
                customEnvironment = tag.Kind == EnvironmentKind.Custom ? tag.Text : string.Empty;
                isApplyingSavedConnection = false;

                RefreshEnvironment();
            }
        }

        public bool HasEnvironmentTag
        {
            get { return Environment.HasValue; }
        }

        /// <summary>True while the tag is the custom kind, so the panel can ask for its text.</summary>
        public bool IsCustomEnvironment
        {
            get { return environmentKind == EnvironmentKind.Custom; }
        }

        public string EnvironmentText
        {
            get { return Environment.Text; }
        }

        public string EnvironmentBackground
        {
            get { return EnvironmentPalette.Background(Environment.Kind); }
        }

        public string EnvironmentForeground
        {
            get { return EnvironmentPalette.Foreground(Environment.Kind); }
        }

        /// <summary>
        /// Section 6.1.4: a host or database name containing "prod" makes the panel propose the Prod
        /// tag. It is a proposal with an Accept link and is never applied silently, so this stays true
        /// until the user accepts it or tags the connection something else.
        /// </summary>
        public bool HasEnvironmentSuggestion
        {
            get { return EnvironmentPalette.ShouldSuggest(ServerName, DatabaseName) && !Environment.IsProduction; }
        }

        public string EnvironmentSuggestionText
        {
            get { return string.Format("This looks like production. Tag it {0}?", EnvironmentTag.For(EnvironmentKind.Prod).Text); }
        }

        public string ConnectionString
        {
            get
            {
                return UseSqlAuthentication
                    ? SqlConnectionStrings.ForDatabase(ServerName, DatabaseName, UserName, Password)
                    : SqlConnectionStrings.ForDatabase(ServerName, DatabaseName);
            }
            set
            {
                isApplyingSavedConnection = true;

                try
                {
                    ApplyConnectionString(value);
                }
                finally
                {
                    isApplyingSavedConnection = false;
                }

                MarkEdited();
            }
        }

        public bool UseSqlAuthentication
        {
            get { return AuthenticationIndex == 1; }
        }

        public string Describe()
        {
            return string.Format("Server: {0}, Database: {1}", ServerName, DatabaseName);
        }

        /// <summary>
        /// Section 6.1.1 item 6: runs the test off the UI thread and leaves the outcome on the panel.
        /// Returns whether the connection is usable, so the shell can run both sides in parallel.
        /// </summary>
        public async Task<bool> TestAsync()
        {
            if (!ConnectionRules.IsComplete(ServerName, DatabaseName))
            {
                Validity = ConnectionValidity.Unset;
                testMessage = "Enter a server and a database first.";
                OnPropertyChanged(nameof(TestMessage));
                return false;
            }

            if (validity == ConnectionValidity.Testing)
            {
                return false;
            }

            // The fields the test runs against, so a change while it is in flight cannot be mistaken
            // for a passing test of the new values (section 6.1.3).
            var snapshot = ConnectionString;

            Validity = ConnectionValidity.Testing;
            IsBusy = true;

            ConnectionTestResult result;

            try
            {
                result = await Task.Run(() => SqlConnectionTest.Run(snapshot)).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                result = new ConnectionTestResult(false, null, 0, ConnectionErrorCategory.Unknown, ex.Message, null);
            }
            finally
            {
                IsBusy = false;
            }

            testMessage = result.Message;
            OnPropertyChanged(nameof(TestMessage));

            if (ConnectionString != snapshot)
            {
                // The user edited while the test ran; the result belongs to the old fields.
                Validity = ConnectionRules.AfterEdit(ServerName, DatabaseName);
                return false;
            }

            Validity = result.Succeeded ? ConnectionValidity.Valid : ConnectionValidity.Failed;

            if (!result.Succeeded)
            {
                ErrorConnection = result.ErrorMessage;
            }

            return result.Succeeded;
        }

        public string? ErrorConnection { get; private set; }

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
        /// box was dropped down. A failure is left on the panel with a Retry link (section 6.1.1 item 5).
        /// </summary>
        public async Task LoadDatabasesAsync()
        {
            if (isDatabaseFilled || IsLoadingDatabases)
            {
                return;
            }

            IsLoadingDatabases = true;
            IsBusy = true;
            DatabaseError = null;

            try
            {
                var databases = new ObservableCollection<string>();
                var connectionString = UseSqlAuthentication
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
                IsLoadingDatabases = false;
                IsBusy = false;
            }
        }

        /// <summary>
        /// Applies a saved connection to this card (section 6.1.6). The password is deliberately not
        /// part of a saved connection (section 11.3), so it is left as it was.
        /// </summary>
        public void ApplySavedConnection(SavedConnection connection)
        {
            if (connection == null)
            {
                return;
            }

            isApplyingSavedConnection = true;

            try
            {
                ServerName = connection.Host;
                DatabaseName = connection.Database;
                AuthenticationIndex = connection.IntegratedSecurity ? 0 : 1;
                UserName = string.IsNullOrEmpty(connection.UserName) ? "sa" : connection.UserName;
                environmentKind = connection.Environment;
                customEnvironment = string.Empty;
            }
            finally
            {
                isApplyingSavedConnection = false;
            }

            InvalidateDatabases();
            RefreshEnvironment();
            MarkEdited();
        }

        /// <summary>
        /// Section 6.1.2 item 3: swaps this connection with the other side. The validity state travels
        /// with the settings, because the test that produced it ran against exactly these settings - a
        /// swap does not make a proven connection unproven.
        /// </summary>
        public void SwapWith(ConnectionViewModel other)
        {
            if (other == null)
            {
                return;
            }

            var host = ServerName;
            var database = DatabaseName;
            var authentication = AuthenticationIndex;
            var user = UserName;
            var password = Password;
            var kind = environmentKind;
            var custom = customEnvironment;
            var otherValidity = validity;
            var otherMessage = testMessage;
            var otherError = ErrorConnection;

            isApplyingSavedConnection = true;
            other.isApplyingSavedConnection = true;

            try
            {
                ServerName = other.ServerName;
                DatabaseName = other.DatabaseName;
                AuthenticationIndex = other.AuthenticationIndex;
                UserName = other.UserName;
                Password = other.Password;
                environmentKind = other.environmentKind;
                customEnvironment = other.customEnvironment;
                Validity = other.validity;
                testMessage = other.testMessage;
                ErrorConnection = other.ErrorConnection;

                other.ServerName = host;
                other.DatabaseName = database;
                other.AuthenticationIndex = authentication;
                other.UserName = user;
                other.Password = password;
                other.environmentKind = kind;
                other.customEnvironment = custom;
                other.Validity = otherValidity;
                other.testMessage = otherMessage;
                other.ErrorConnection = otherError;
            }
            finally
            {
                isApplyingSavedConnection = false;
                other.isApplyingSavedConnection = false;
            }

            InvalidateDatabases();
            other.InvalidateDatabases();
            RefreshEnvironment();
            other.RefreshEnvironment();
            RefreshText();
            other.RefreshText();
        }

        private async Task TestAndReportAsync()
        {
            await TestAsync().ConfigureAwait(true);
        }

        /// <summary>
        /// Section 6.1.1 item 5: the Retry link. The panel shows the spinner and the error itself, so
        /// the command has nowhere to put a result.
        /// </summary>
        private async void RetryDatabases()
        {
            await LoadDatabasesAsync().ConfigureAwait(true);
        }

        /// <summary>Section 6.1.1 item 7: stores this card under a name the user supplies.</summary>
        private void SaveAs()
        {
            if (dialogs == null)
            {
                return;
            }

            var suggested = string.IsNullOrWhiteSpace(DatabaseName) ? ServerName : DatabaseName;
            var name = dialogs.AskText("Save connection", "Name this connection:", suggested);

            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            var handler = Saved;

            if (handler != null)
            {
                handler(this, new SavedConnectionEventArgs(
                    new SavedConnection(name.Trim(), ServerName, DatabaseName, UseWindowsAuthentication, UserName, Environment.Kind)));
            }
        }

        private void AcceptEnvironmentSuggestion()
        {
            Environment = EnvironmentTag.For(EnvironmentKind.Prod);
        }

        private void ApplyConnectionString(string? value)
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

        /// <summary>
        /// Any edit to a field invalidates both the database list and the last test (sections 6.1.1 and
        /// 6.1.3), and tells the shell so it can mark results stale (section 9.6).
        /// </summary>
        private void MarkEdited()
        {
            if (isApplyingSavedConnection)
            {
                return;
            }

            Validity = ConnectionRules.AfterEdit(ServerName, DatabaseName);
            testMessage = null;

            RefreshText();

            Edited?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>The chip's text depends on the fields, so it is refreshed whenever they change.</summary>
        private void RefreshText()
        {
            OnPropertyChanged(nameof(TestMessage));
            OnPropertyChanged(nameof(ChipText));
            OnPropertyChanged(nameof(ChipTooltip));
            OnPropertyChanged(nameof(ServerDisplay));
            OnPropertyChanged(nameof(DatabaseDisplay));
            OnPropertyChanged(nameof(HasEnvironmentSuggestion));
            OnPropertyChanged(nameof(EnvironmentSuggestionText));
        }

        private void RefreshValidity()
        {
            OnPropertyChanged(nameof(IsValid));
            OnPropertyChanged(nameof(IsTesting));
            OnPropertyChanged(nameof(HasTestFailure));
            OnPropertyChanged(nameof(StatusTooltip));
            TestCommand.RaiseCanExecuteChanged();
        }

        private void RefreshEnvironment()
        {
            OnPropertyChanged(nameof(Environment));
            OnPropertyChanged(nameof(HasEnvironmentTag));
            OnPropertyChanged(nameof(IsCustomEnvironment));
            OnPropertyChanged(nameof(EnvironmentText));
            OnPropertyChanged(nameof(EnvironmentBackground));
            OnPropertyChanged(nameof(EnvironmentForeground));
            OnPropertyChanged(nameof(HasEnvironmentSuggestion));
        }

        private void OnServerNameChanged()
        {
            InvalidateDatabases();
            MarkEdited();
        }

        private void OnDatabaseNameChanged()
        {
            MarkEdited();
        }

        private void OnAuthenticationIndexChanged()
        {
            InvalidateDatabases();
            OnPropertyChanged(nameof(UseWindowsAuthentication));
            OnPropertyChanged(nameof(UseSqlAuthentication));
            OnPropertyChanged(nameof(ShowCredentials));
            MarkEdited();
        }

        private void OnUserNameChanged()
        {
            InvalidateDatabases();
            MarkEdited();
        }

        private void OnPasswordChanged()
        {
            InvalidateDatabases();
            MarkEdited();
        }

        private void InvalidateDatabases()
        {
            isDatabaseFilled = false;
            DatabaseError = null;
            OnPropertyChanged(nameof(DatabaseError));
        }
    }

    /// <summary>The saved connection a card asked the shell to store.</summary>
    public sealed class SavedConnectionEventArgs : EventArgs
    {
        public SavedConnectionEventArgs(SavedConnection connection)
        {
            Connection = connection;
        }

        public SavedConnection Connection { get; }
    }
}
