using System;
using System.Collections.Generic;
using System.Text;

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
        private VersionTypeEnum version;
        private string collation;
        private bool hasChangeTracking;
        private bool isChangeTrackingAutoCleanup;
        private int changeTrackingRetentionPeriod;
        private int changeTrackingPeriodUnits;
        private string changeTrackingPeriodUnitsDesc;
        private bool hasFullTextEnabled;

        public DatabaseInfo()
        {
            version = VersionTypeEnum.SQLServer2005;
        }

        public string Server
        {
            get;
            set;
        }

        public string Database
        {
            get;
            set;
        }
        
        public VersionTypeEnum Version
        {
            get { return version; }
        }

        public string Collation
        {
            get { return collation; }
            set { collation = value; }
        }

        public bool HasFullTextEnabled
        {
            get { return hasFullTextEnabled; }
            set { hasFullTextEnabled = value; }
        }

        public string ChangeTrackingPeriodUnitsDesc
        {
            get { return changeTrackingPeriodUnitsDesc; }
            set { changeTrackingPeriodUnitsDesc = value; }
        }

        public int ChangeTrackingPeriodUnits
        {
            get { return changeTrackingPeriodUnits; }
            set { changeTrackingPeriodUnits = value; }
        }

        public int ChangeTrackingRetentionPeriod
        {
            get { return changeTrackingRetentionPeriod; }
            set { changeTrackingRetentionPeriod = value; }
        }

        public bool IsChangeTrackingAutoCleanup
        {
            get { return isChangeTrackingAutoCleanup; }
            set { isChangeTrackingAutoCleanup = value; }
        }

        public bool HasChangeTracking
        {
            get { return hasChangeTracking; }
            set { hasChangeTracking = value; }
        }

        public float VersionNumber
        {
            get { return versionNumber; }
            set
            {
                versionNumber = value;
                if ((versionNumber >= 8) && (versionNumber < 9)) version = VersionTypeEnum.SQLServer2000;
                if ((versionNumber >= 9) && (versionNumber < 10)) version = VersionTypeEnum.SQLServer2005;
                if ((versionNumber >= 10) && (versionNumber < 10.25)) version = VersionTypeEnum.SQLServer2008;
                if ((versionNumber >= 10.25) && (versionNumber < 10.5)) version = VersionTypeEnum.SQLServerAzure10;
                if ((versionNumber >= 10.5) && (versionNumber < 11)) version = VersionTypeEnum.SQLServer2008R2;
                if ((versionNumber >= 11.0) && (versionNumber < 12)) version = VersionTypeEnum.SQLServer2008R2; // SQLServer2012
            }
        }

        public void SetEdition(int? edition)
        {
            if (edition.GetValueOrDefault() == 5)
            {
                this.version = VersionTypeEnum.SQLServerAzure10;
            }
        }
    }
}
