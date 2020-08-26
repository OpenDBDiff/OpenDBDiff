namespace OpenDBDiff.SqlServer.Schema.Model
{
    public class DatabaseInfo
    {
        public enum SQLServerVersion
        {
            SQLServer2000,
            SQLServer2005,
            SQLServer2008,
            SQLServer2008R2,

            // Azure will be reporting v11 instead of v10.25 soon...
            // http://social.msdn.microsoft.com/Forums/en-US/ssdsgetstarted/thread/ad7aae98-26ac-4979-848d-517a86c3fa5c/
            SQLServerAzure10, /*Azure*/

            SQLServer2012,
            SQLServer2014,
            SQLServer2016,
            SQLServer2017,
        }

        public enum SQLServerEdition
        {
            Personal = 1,
            Standard = 2,
            Enterprise = 3,
            Express = 4,
            Azure = 5
        }

        private float versionNumber;

        public DatabaseInfo()
        {
            Version = SQLServerVersion.SQLServer2005;
        }

        public string Server { get; set; }

        public string Database { get; set; }

        public SQLServerVersion Version { get; private set; }

        public SQLServerEdition Edition { get; private set; }

        public string Collation { get; set; }

        public bool HasFullTextEnabled { get; set; }

        public string ChangeTrackingPeriodUnitsDesc { get; set; }

        public int ChangeTrackingPeriodUnits { get; set; }

        public int ChangeTrackingRetentionPeriod { get; set; }

        public bool IsChangeTrackingAutoCleanup { get; set; }

        public bool HasChangeTracking { get; set; }

        public float VersionNumber
        {
            get { return versionNumber; }
            set
            {
                versionNumber = value;

                SQLServerVersion version = this.Version;

                // https://buildnumbers.wordpress.com/sqlserver/
                if (versionNumber >= 8) version = SQLServerVersion.SQLServer2000;
                if (versionNumber >= 9) version = SQLServerVersion.SQLServer2005;
                if (versionNumber >= 10) version = SQLServerVersion.SQLServer2008;
                if (versionNumber >= 10.25) version = SQLServerVersion.SQLServerAzure10;
                if (versionNumber >= 10.5) version = SQLServerVersion.SQLServer2008R2;
                if (versionNumber >= 11.0) version = SQLServerVersion.SQLServer2012;
                if (versionNumber >= 12.0) version = SQLServerVersion.SQLServer2014;
                if (versionNumber >= 13.0) version = SQLServerVersion.SQLServer2016;
                if (versionNumber >= 14.0) version = SQLServerVersion.SQLServer2017;

                this.Version = version;
            }
        }

        public void SetEdition(SQLServerEdition edition)
        {
            this.Edition = edition;

            if (edition == SQLServerEdition.Azure)
            {
                this.Version = SQLServerVersion.SQLServerAzure10;
            }
        }
    }
}
