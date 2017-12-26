namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class DatabaseInfo
    {
        public enum VersionTypeEnum
        {
            SQLServer2000 = 1,
            SQLServer2005 = 2,
            SQLServer2008 = 3,
            SQLServer2008R2 = 4,
            // Azure will be reporting v11 instead of v10.25 soon...
            // http://social.msdn.microsoft.com/Forums/en-US/ssdsgetstarted/thread/ad7aae98-26ac-4979-848d-517a86c3fa5c/
            SQLServerAzure10 = 5, /*Azure*/
            SQLServer2012 = 6,
        }

        private float versionNumber;

        public DatabaseInfo()
        {
            Version = VersionTypeEnum.SQLServer2005;
        }

        public string Server { get; set; }

        public string Database { get; set; }

        public VersionTypeEnum Version { get; private set; }

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
                if ((versionNumber >= 8) && (versionNumber < 9)) Version = VersionTypeEnum.SQLServer2000;
                if ((versionNumber >= 9) && (versionNumber < 10)) Version = VersionTypeEnum.SQLServer2005;
                if ((versionNumber >= 10) && (versionNumber < 10.25)) Version = VersionTypeEnum.SQLServer2008;
                if ((versionNumber >= 10.25) && (versionNumber < 10.5)) Version = VersionTypeEnum.SQLServerAzure10;
                if ((versionNumber >= 10.5) && (versionNumber < 11)) Version = VersionTypeEnum.SQLServer2008R2;
                if ((versionNumber >= 11.0) && (versionNumber < 13)) Version = VersionTypeEnum.SQLServer2008R2; // SQLServer2012, SQLServer2014
            }
        }

        public void SetEdition(int? edition)
        {
            if (edition.GetValueOrDefault() == 5)
            {
                this.Version = VersionTypeEnum.SQLServerAzure10;
            }
        }
    }
}
