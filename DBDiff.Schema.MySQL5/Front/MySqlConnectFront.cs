using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using DBDiff.Front;

namespace DBDiff.Schema.MySQL.Front
{
    public partial class MySqlConnectFront : UserControl,IFront 
    {
        private string errorConnection;

        public MySqlConnectFront()
        {
            InitializeComponent();
        }

        public string ErrorConnection
        {
            get { return errorConnection; }
        }

        public Boolean TestConnection()
        {
            try
            {
                MySqlConnection connection = new MySqlConnection();
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
                string sql = "User Id=" + txtUsername.Text + ";Password=" + txtPassword.Text + ";";
                if (!String.IsNullOrEmpty(txtDefaultSchema.Text))
                    sql += "Database=" + txtDefaultSchema.Text + ";";
                if (!chkPipe.Checked)
                {
                    sql += "Data Source=" + txtServer.Text + ";";
                    if (!String.IsNullOrEmpty(txtPort.Text))
                        sql += "Port=" + txtPort.Text + ";";
                }
                else
                {
                    sql += "Data Source=" + txtPipeName.Text + ";";
                    sql += "Port=-1";
                }
                return sql;
            }
        }

        private void chkPipe_CheckedChanged(object sender, EventArgs e)
        {
            txtPort.Enabled = !chkPipe.Checked;
            txtServer.Enabled = !chkPipe.Checked;
            txtPipeName.Enabled = chkPipe.Checked;
        }

        
        private void btnTest_Click(object sender, EventArgs e)
        {
            if (TestConnection())
                MessageBox.Show(this, "Test succesful!", "Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(this, "Test failed!\r\n" + ErrorConnection, "Test", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
