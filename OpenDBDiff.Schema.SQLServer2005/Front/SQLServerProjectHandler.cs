using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDBDiff.Front;
using OpenDBDiff.Schema.Model;

namespace OpenDBDiff.Schema.SQLServer.Generates.Front
{
    public class SQLServerProjectHandler : OpenDBDiff.Front.IProjectHandler
    {
        private SqlServerConnectFront DestinationControl;
        private SqlServerConnectFront SourceControl;
        private SQLServerGenerator SourceGenerator;
        private SQLServerGenerator DestinationGenerator;

        public IFront CreateDestinationSelector()
        {
            this.DestinationControl = new SqlServerConnectFront();
            this.DestinationControl.Location = new Point(1, 1);
            this.DestinationControl.Name = "mySqlConnectFront1";
            this.DestinationControl.Anchor = (AnchorStyles)((int)AnchorStyles.Bottom + (int)AnchorStyles.Left + (int)AnchorStyles.Right);
            this.DestinationControl.TabIndex = 10;
            this.DestinationControl.Text = "Destination Database:";
            this.DestinationControl.UserName = "sa";
            this.DestinationControl.Password = "";
            this.DestinationControl.ServerName = "(local)";
            this.DestinationControl.DatabaseIndex = 1;
            return this.DestinationControl;
        }

        public IFront CreateSourceSelector()
        {
            this.SourceControl = new SqlServerConnectFront();
            SourceControl.Location = new Point(1, 1);
            SourceControl.Name = "mySqlConnectFront1";
            SourceControl.Anchor = (AnchorStyles)((int)AnchorStyles.Bottom + (int)AnchorStyles.Left + (int)AnchorStyles.Right);
            SourceControl.TabIndex = 10;
            SourceControl.Text = "Source Database:";
            SourceControl.UserName = "sa";
            SourceControl.Password = "";
            SourceControl.ServerName = "(local)";
            SourceControl.DatabaseIndex = 1;
            return SourceControl;
        }

        public IDatabaseComparer GetDatabaseComparer()
        {
            return new SQLServerComparer();
        }

        public IGenerator SetDestinationGenerator(string connectionString, IOption options)
        {
            DestinationGenerator = new SQLServerGenerator(connectionString, options);
            return DestinationGenerator;
        }
        public IGenerator SetSourceGenerator(string connectionString, IOption options)
        {
            SourceGenerator = new SQLServerGenerator(connectionString, options);
            return SourceGenerator;
        }

        public string GetDestinationConnectionString()
        {
            return DestinationControl.ConnectionString;
        }

        public string GetDestinationDatabaseName()
        {
            return DestinationControl.DatabaseName;
        }

        public string GetDestinationServerName()
        {
            return DestinationControl.ServerName;
        }

        public string GetSourceConnectionString()
        {
            return SourceControl.ConnectionString;
        }

        public string GetSourceDatabaseName()
        {
            return SourceControl.DatabaseName;
        }


        public string GetSourceServerName()
        {
            return SourceControl.ServerName;
        }

        public IOption GetProjectOptions()
        {
            return new Options.SqlOption();
        }

        public OptionControl CreateOptionControl()
        {
            return new SqlOptionsFront();
        }

        public string GetScriptLanguage()
        {
            return "mssql";
        }

        public void Unload()
        {
            this.SourceControl.Dispose();
            this.DestinationControl.Dispose();
            this.SourceControl = null;
            this.DestinationControl = null;
        }

        public override string ToString()
        {
            return "SQLServer 2005 or higher";
        }
    }
}
