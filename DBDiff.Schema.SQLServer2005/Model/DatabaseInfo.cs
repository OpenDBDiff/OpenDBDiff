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
            SQLServer2008 = 3
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
                if ((versionNumber >= 10) && (versionNumber < 11)) version = VersionTypeEnum.SQLServer2008;
            }
        }
    }
}
