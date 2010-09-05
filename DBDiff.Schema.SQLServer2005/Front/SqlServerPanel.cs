using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace DBDiff.Schema.SQLServer.Front
{
    public partial class SqlServerPanel : UserControl
    {
        public SqlServerPanel()
        {
            InitializeComponent();
        }

        public string ConnectionStringSource
        {
            get { return sqlServerConnectFront1.ConnectionString; }
        }

        public string ConnectionStringTarget
        {
            get { return sqlServerConnectFront2.ConnectionString; }
        }

        public string SourceUserName
        {
            get { return sqlServerConnectFront1.UserName; }
            set { sqlServerConnectFront1.UserName = value; }
        }

        public string SourcePassword
        {
            get { return sqlServerConnectFront1.Password; }
            set { sqlServerConnectFront1.Password = value; }
        }

        public string SourceServerName
        {
            get { return sqlServerConnectFront1.ServerName; }
            set { sqlServerConnectFront1.ServerName = value; }
        }

        public int SourceDatabaseIndex
        {
            get { return sqlServerConnectFront1.DatabaseIndex; }
            set { sqlServerConnectFront1.DatabaseIndex = value; }
        }

        public string TargetUserName
        {
            get { return sqlServerConnectFront2.UserName; }
            set { sqlServerConnectFront2.UserName = value; }
        }

        public string TargetPassword
        {
            get { return sqlServerConnectFront2.Password; }
            set { sqlServerConnectFront2.Password = value; }
        }

        public string TargetServerName
        {
            get { return sqlServerConnectFront2.ServerName; }
            set { sqlServerConnectFront2.ServerName = value; }
        }

        public int TargetDatabaseIndex
        {
            get { return sqlServerConnectFront2.DatabaseIndex; }
            set { sqlServerConnectFront2.DatabaseIndex = value; }
        }
    }
}
