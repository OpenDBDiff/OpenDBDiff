using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Generates.SQLCommands
{
    internal class DatabaseSQLCommand
    {
        public static string GetVersion(Database databaseSchema)
        {
            string sql;
            sql = "SELECT SUBSTRING(CONVERT(varchar,SERVERPROPERTY('productversion')),1,PATINDEX('.',CONVERT(varchar,SERVERPROPERTY('productversion')))+2) AS Version";
            return sql;
        }

        public static string Get(DatabaseInfo.VersionTypeEnum version, Database databaseSchema)
        {
            if (version == DatabaseInfo.VersionTypeEnum.SQLServer2005) return Get2005(databaseSchema);
            if (version == DatabaseInfo.VersionTypeEnum.SQLServer2008) return Get2008(databaseSchema);
            return "";
        }

        private static string Get2005(Database databaseSchema)
        {
            string sql;
            sql = "SELECT DATABASEPROPERTYEX('" + databaseSchema.Name + "','IsFulltextEnabled') AS IsFullTextEnabled, DATABASEPROPERTYEX('" + databaseSchema.Name + "','Collation') AS Collation";
            return sql;
        }

        private static string Get2008(Database databaseSchema)
        {
            string sql;
            sql = "SELECT DATABASEPROPERTYEX('" + databaseSchema.Name + "','IsFulltextEnabled') AS IsFullTextEnabled, DATABASEPROPERTYEX('" + databaseSchema.Name + "','Collation') AS Collation";
            return sql;
        }
    }
}
