using System;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.Schema.SQLServer.Model
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
