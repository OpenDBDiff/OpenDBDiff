using OpenDBDiff.SqlServer.Ui.Util;
using OpenDBDiff.Abstractions.Ui;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace OpenDBDiff.SqlServer.Ui
{
    public partial class SqlServerConnectFront : UserControl, IFront
    {
        private Boolean isDatabaseFilled = false;
        private Boolean isServerFilled = false;

        private delegate void clearCombo();

        private delegate void addCombo(string item);

        public SqlServerConnectFront()
        {
            InitializeComponent();
            cboAuthentication.SelectedIndex = 1;
        }

        private void cboAuthentication_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtUsername.Enabled = cboAuthentication.SelectedIndex == 1;
            txtPassword.Enabled = cboAuthentication.SelectedIndex == 1;
            isDatabaseFilled = false;
            ClearDatabase();
        }

        public string ErrorConnection { get; private set; }

        public bool UseWindowsAuthentication
        {
            get
            {
                return cboAuthentication.SelectedIndex == 0;
            }
            set
            {
                cboAuthentication.SelectedIndex = (value ? 0 : 1);
            }
        }

        public string DatabaseName
        {
            get { return cboDatabase.Text; }
            set { cboDatabase.Text = value; }
        }

        public string UserName
        {
            get { return txtUsername.Text; }
            set { txtUsername.Text = value; }
        }

        public string Password
        {
            get { return txtPassword.Text; }
            set { txtPassword.Text = value; }
        }

        public override string Text
        {
            get { return lblName.Text; }
            set { lblName.Text = value; }
        }

        public string ServerName
        {
            get { return cboServer.Text; }
            set { cboServer.Text = value; }
        }

        public Boolean TestConnection()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = this.ConnectionString;
                    connection.Open();
                    connection.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorConnection = ex.Message;
                return false;
            }
        }

        private string BuildConnectionString(string server, string database)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = server.Trim();

            // if database is ommitted the connection will be established to the default database for the user
            if (!string.IsNullOrEmpty(database))
                builder.InitialCatalog = database.Trim();

            builder.IntegratedSecurity = true;
            builder.TrustServerCertificate = true;
            return builder.ConnectionString;
        }

        private string BuildConnectionString(string server, string database, string username, string password)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(BuildConnectionString(server, database));

            builder.IntegratedSecurity = false;
            builder.UserID = username;
            builder.Password = password;
            return builder.ConnectionString;
        }

        public string ConnectionStringToDefaultDatabase
        {
            get
            {
                if (cboAuthentication.SelectedIndex == 1)
                    return BuildConnectionString(cboServer.Text, null, txtUsername.Text, txtPassword.Text);
                else
                    return BuildConnectionString(cboServer.Text, null);
            }
        }

        public string ConnectionStringToMasterDatabase
        {
            get
            {
                if (cboAuthentication.SelectedIndex == 1)
                    return BuildConnectionString(cboServer.Text, "master", txtUsername.Text, txtPassword.Text);
                else
                    return BuildConnectionString(cboServer.Text, "master");
            }
        }

        public string ConnectionString
        {
            get
            {
                if (cboAuthentication.SelectedIndex == 1)
                    return BuildConnectionString(cboServer.Text, cboDatabase.Text, txtUsername.Text, txtPassword.Text);
                else
                    return BuildConnectionString(cboServer.Text, cboDatabase.Text);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    var builder = new SqlConnectionStringBuilder(value);

                    ServerName = builder.DataSource;
                    UseWindowsAuthentication = builder.IntegratedSecurity;
                    if (UseWindowsAuthentication)
                    {
                        UserName = "";
                        Password = "";
                    }
                    else
                    {
                        UserName = builder.UserID;
                        Password = builder.Password;
                    }
                    DatabaseName = builder.InitialCatalog;
                }
                else
                {
                    cboAuthentication.SelectedIndex = 1;
                    UserName = "";
                    Password = "";
                    ServerName = "(local)";
                    DatabaseName = "";
                }
            }
        }

        public Control Control
        {
            get
            {
                return this;
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            if (TestConnection())
                MessageBox.Show(this, "Test successful!", "Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(this, "Test failed!\r\n" + ErrorConnection, "Test", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void AddComboItem(string item)
        {
            if (!InvokeRequired)
                cboDatabase.Items.Add(item);
            else
            {
                addCombo add = new addCombo(AddComboItem);
                Invoke(add, new string[] { item });
            }
        }

        private void ClearDatabase()
        {
            if (!InvokeRequired)
                cboDatabase.Items.Clear();
            else
            {
                clearCombo clear = new clearCombo(ClearDatabase);
                Invoke(clear);
            }
        }

        private void FillDatabase()
        {
            if (!isDatabaseFilled)
            {
                String connectionString = ConnectionStringToDefaultDatabase;
                ClearDatabase();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand("SELECT name,database_id FROM sys.databases ORDER BY Name", conn))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AddComboItem(reader["Name"].ToString());
                            }
                            isDatabaseFilled = true;
                        }
                    }
                }
            }
        }

        private void cboServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            isDatabaseFilled = false;
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            isDatabaseFilled = false;
            ClearDatabase();
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            isDatabaseFilled = false;
            ClearDatabase();
        }

        private void cboServer_DropDown(object sender, EventArgs e)
        {
            try
            {
                if (!isServerFilled)
                {
                    this.Cursor = Cursors.WaitCursor;
                    SqlServerList.Get().ForEach(item => cboServer.Items.Add(item));
                    isServerFilled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void cboDatabase_DropDown(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                FillDatabase();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                cboDatabase.Items.Clear();
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void cboServer_TextChanged(object sender, EventArgs e)
        {
            isDatabaseFilled = false;
        }

        public override string ToString()
        {
            return string.Format($"Server: {ServerName}, Database: {DatabaseName}");
        }

        public object Clone()
        {
            var clone = new SqlServerConnectFront
            {
                ServerName = this.ServerName,
                UseWindowsAuthentication = this.UseWindowsAuthentication,
                UserName = this.UserName,
                Password = this.Password,
                DatabaseName = this.DatabaseName
            };

            clone.Location = new System.Drawing.Point(1, 1);
            clone.Name = "SourceControl";
            clone.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            return clone;
        }

        public void SetSettingsFrom(IFront other)
        {
            if (other is SqlServerConnectFront sql)
            {
                this.ServerName = sql.ServerName;
                this.DatabaseName = sql.DatabaseName;
                this.UseWindowsAuthentication = sql.UseWindowsAuthentication;
                this.UserName = sql.UserName;
                this.Password = sql.Password;
            }
        }
    }
}
