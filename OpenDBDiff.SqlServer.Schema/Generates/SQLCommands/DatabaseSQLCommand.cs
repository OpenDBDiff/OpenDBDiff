using OpenDBDiff.SqlServer.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Generates.SQLCommands
{
    internal class DatabaseSQLCommand
    {
        public static string GetVersion(Database databaseSchema)
        {
            string sql;
            sql = "SELECT SERVERPROPERTY('productversion') AS Version, SERVERPROPERTY('EngineEdition') AS Edition";
            return sql;
        }

        public static string Get(DatabaseInfo.SQLServerVersion version, DatabaseInfo.SQLServerEdition edition, Database databaseSchema)
        {
            switch (version)
            {
                case DatabaseInfo.SQLServerVersion.SQLServer2005:
                    return Get2005(databaseSchema);

                case DatabaseInfo.SQLServerVersion.SQLServer2008:
                    return Get2008(databaseSchema);

                case DatabaseInfo.SQLServerVersion.SQLServer2008R2:
                    return Get2008R2(databaseSchema);

                case DatabaseInfo.SQLServerVersion.SQLServerAzure10:
                    return GetAzure(databaseSchema);

                default:
                    if (edition == DatabaseInfo.SQLServerEdition.Azure)
                        return GetAzure(databaseSchema);
                    else
                        return Get2008R2(databaseSchema);
            }
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

        private static string Get2008R2(Database databaseSchema)
        {
            string sql;
            sql = "SELECT DATABASEPROPERTYEX('" + databaseSchema.Name + "','IsFulltextEnabled') AS IsFullTextEnabled, DATABASEPROPERTYEX('" + databaseSchema.Name + "','Collation') AS Collation";
            return sql;
        }

        private static string GetAzure(Database databaseSchema)
        {
            string sql;
            //DATABASEPROPERTYEX('IsFullTextEnabled') is deprecated http://technet.microsoft.com/en-us/library/cc646010(SQL.110).aspx
            sql = "SELECT 0 AS IsFullTextEnabled, DATABASEPROPERTYEX('" + databaseSchema.Name + "','Collation') AS Collation";
            return sql;
        }
    }
}
