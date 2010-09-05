using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using DBDiff.Front;

namespace DBDiff.Schema.SQLServer.Front
{
    public partial class SqlServerConnectFront : UserControl, IFront 
    {
        private string errorConnection;
        private Boolean isDatabaseFilled = false;
        private Thread thread = null;
        private delegate void clearCombo();
        private delegate void addCombo(string item);

        public SqlServerConnectFront()
        {
            InitializeComponent();
            //cboAuthentication.SelectedIndex = 0;
        }

        private void cboAuthentication_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtUsername.Enabled = cboAuthentication.SelectedIndex == 1;
            txtPassword.Enabled = cboAuthentication.SelectedIndex == 1;
            isDatabaseFilled = false;
            ClearDatabase();
        }

        public string ErrorConnection
        {
            get { return errorConnection; }
        }

        /// <summary>
        /// Gets or sets the index of the database.
        /// </summary>
        /// <value>The index of the database.</value>
        public int DatabaseIndex
        {
            get { return cboDatabase.SelectedIndex; }
            set 
            {
                if (cboDatabase.Items.Count > 0)
                    cboDatabase.SelectedIndex = value; 
            }
        }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
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
        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
        /// <value>The name of the server.</value>
        public string ServerName
        {
            get { return cboServer.Text; }
            set { cboServer.Text = value; }
        }

        /// <summary>
        /// Gets or sets the type of the connection.
        /// </summary>
        /// <value>The type of the connection.</value>
        public int ConnectionType
        {
            get { return cboAuthentication.SelectedIndex; }
            set
            {
                cboAuthentication.SelectedIndex = value;
                //txtUsername.Text = "";
            }
        }

        public Boolean TestConnection()
        {
            try
            {
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = this.ConnectionString;
                connection.Open();
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                errorConnection = ex.Message;
                return false;
            }
        }

        public string ConnectionString
        {
            get
            {
                string sql = "Data Source=" + cboServer.Text + ";Initial Catalog=" + cboDatabase.Text + ";";
                if (cboAuthentication.SelectedIndex == 1)
                    sql += "User Id=" + txtUsername.Text + ";Password=" + txtPassword.Text + ";";
                else
                    sql += "Integrated Security=SSPI;";
                return sql;
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            if (TestConnection())
                MessageBox.Show(this, "Test succesful!", "Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(this, "Test failed!\r\n" + ErrorConnection, "Test", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void AddComboItem(string item)
        {
            if (!this.InvokeRequired)
                cboDatabase.Items.Add(item);
            else
            {
                addCombo add = new addCombo(AddComboItem);
                this.Invoke(add, new string[] { item });
            }
        }

        private void ClearDatabase()
        {
            if (!this.InvokeRequired)
                cboDatabase.Items.Clear();
            else
            {
                clearCombo clear = new clearCombo(ClearDatabase);
                this.Invoke(clear);
            }
        }

        private void FillDatabase()
        {
            if (!isDatabaseFilled)
            {
                String connectionString = this.ConnectionString;
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

        private void cboDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                FillDatabase();
            }
            catch (Exception ex)
            {
                cboDatabase.Items.Clear();
                MessageBox.Show(this, ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
