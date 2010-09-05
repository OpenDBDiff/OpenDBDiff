using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Sybase.Data.AseClient;
using DBDiff.Front;

namespace DBDiff.Schema.Sybase.Front
{
    public partial class AseConnectFront : UserControl, IFront 
    {
        private string errorConnection;

        public AseConnectFront()
        {
            InitializeComponent();
        }

        public string ErrorConnection
        {
            get { return errorConnection; }
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

        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
        /// <value>The name of the server.</value>
        public string ServerName
        {
            get { return txtServer.Text; }
            set { txtServer.Text = value; }
        }

        public Boolean TestConnection()
        {
            try
            {
                AseConnection connection = new AseConnection();
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
                string sql = "Data Source='" + txtServer.Text + "';Database=" + txtDatabase.Text + ";";
                sql += "Uid=" + txtUsername.Text + ";Pwd=" + txtPassword.Text + ";";
                return sql;
            }
        }
    }
}
