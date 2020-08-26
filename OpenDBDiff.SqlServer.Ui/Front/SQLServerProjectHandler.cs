using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.Abstractions.Ui;
using OpenDBDiff.Schema.SQLServer.Generates.Options;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenDBDiff.SqlServer.Ui
{
    public class SQLServerProjectHandler : IProjectHandler
    {
        private SqlServerConnectFront DestinationControl;
        private SqlServerConnectFront SourceControl;
        private SQLServerGenerator SourceGenerator;
        private SQLServerGenerator DestinationGenerator;

        private SqlOption Option;

        public IFront CreateDestinationSelector()
        {
            this.DestinationControl = new SqlServerConnectFront
            {
                ServerName = "(local)",
                UseWindowsAuthentication = true,
                UserName = "sa",
                Password = "",
                DatabaseName = ""
            };

            this.DestinationControl.Location = new Point(1, 1);
            this.DestinationControl.Name = "DestinationControl";
            this.DestinationControl.Anchor = (AnchorStyles)((int)AnchorStyles.Bottom + (int)AnchorStyles.Left + (int)AnchorStyles.Right);
            this.DestinationControl.TabIndex = 10;
            this.DestinationControl.Text = "Destination database:";

            return this.DestinationControl;
        }

        public IFront CreateSourceSelector()
        {
            this.SourceControl = new SqlServerConnectFront
            {
                ServerName = "(local)",
                UseWindowsAuthentication = true,
                UserName = "sa",
                Password = "",
                DatabaseName = ""
            };

            SourceControl.Location = new Point(1, 1);
            SourceControl.Name = "SourceControl";
            SourceControl.Anchor = (AnchorStyles)((int)AnchorStyles.Bottom + (int)AnchorStyles.Left + (int)AnchorStyles.Right);
            SourceControl.TabIndex = 10;
            SourceControl.Text = "Source database:";

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

        public IOption GetDefaultProjectOptions()
        {
            if (Option == null)
            {
                Option = new Schema.SQLServer.Generates.Options.SqlOption();
            }
            return Option;
        }

        public void SetProjectOptions(IOption option)
        {
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }
            else if (!(option is SqlOption))
            {
                throw new NotSupportedException($"This project handler only supports {nameof(SqlOption)} options. {option.GetType().Name} not supported");
            }
            Option = option as SqlOption;
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
