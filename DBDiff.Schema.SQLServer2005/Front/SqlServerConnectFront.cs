using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using DBDiff.Front;

namespace DBDiff.Schema.SQLServer.Front
{
    public partial class SqlServerConnectFront : UserControl, IFront 
    {
        private string errorConnection;
        private Boolean isDatabaseFilled = false;

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
                if ((!isDatabaseFilled) && (!String.IsNullOrEmpty(ServerName)))
                {
                    FillDatabase();
                }
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

        private void FillDatabase()
        {
            try
            {
                if (!isDatabaseFilled)
                {
                    cboDatabase.Items.Clear();
                    using (SqlConnection conn = new SqlConnection(this.ConnectionString))
                    {
                        conn.Open();
                        using (SqlCommand command = new SqlCommand("SELECT name,database_id FROM sys.databases ORDER BY Name", conn))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    cboDatabase.Items.Add(reader["Name"].ToString());
                                }
                                isDatabaseFilled = true;
                            }
                        }
                    }
                }
            }
            catch
            {
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
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            isDatabaseFilled = false;
        }
    }
}
