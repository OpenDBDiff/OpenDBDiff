using System.Text;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Generates.SQLCommands
{
    internal static class FullTextIndexSQLCommand
    {
        public static string Get(DatabaseInfo.VersionTypeEnum version)
        {
            if (version == DatabaseInfo.VersionTypeEnum.SQLServer2005) return Get2005();
            //Fall back to highest compatible version
            return Get2008();

        }

        private static string Get2005()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ");
            sql.Append("FI.object_id, ");
            sql.Append("T.Name AS TableName,  ");
            sql.Append("FC.name AS FullTextCatalogName, ");
            sql.Append("I.name AS IndexName, ");
            sql.Append("FI.is_enabled, ");
            sql.Append("'['+ S.name + '].['+ T.name + '].[' + FC.name + ']' AS Name, ");
            sql.Append("C.name as ColumnName, ");
            sql.Append("FI.change_tracking_state_desc AS ChangeTracking, ");
            sql.Append("FL.name AS LanguageName ");
            sql.Append("FROM sys.fulltext_indexes FI ");
            sql.Append("INNER JOIN sys.fulltext_catalogs FC ON FC.fulltext_catalog_id = FI.fulltext_catalog_id  ");
            sql.Append("INNER JOIN sys.indexes I ON I.index_id = FI.unique_index_id and I.object_id = FI.object_id  ");
            sql.Append("INNER JOIN sys.tables T ON T.object_id = FI.object_id ");
            sql.Append("INNER JOIN sys.schemas S ON S.schema_id = T.schema_id ");
            sql.Append("INNER JOIN sys.fulltext_index_columns FIC ON FIC.object_id = FI.object_id ");
            sql.Append("INNER JOIN sys.columns C ON C.object_id = FIC.object_id AND C.column_id = FIC.column_id ");
            sql.Append("INNER JOIN sys.fulltext_languages FL ON FL.lcid = FIC.language_id ");
            sql.Append("ORDER BY OBJECT_NAME(FI.object_id), I.name  ");
            return sql.ToString();
        }

        private static string Get2008()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ");
            sql.Append("FI.object_id, ");
            sql.Append("T.Name AS TableName,  ");
            sql.Append("FC.name AS FullTextCatalogName, ");
            sql.Append("I.name AS IndexName, ");
            sql.Append("FI.is_enabled, ");
            sql.Append("'['+ S.name + '].['+ T.name + '].[' + FC.name + ']' AS Name, ");
            sql.Append("C.name as ColumnName, ");
            sql.Append("FL.name AS LanguageName,");
            sql.Append("DS.name AS FileGroupName, ");
            sql.Append("FI.change_tracking_state_desc AS ChangeTracking ");
            sql.Append("FROM sys.fulltext_indexes FI ");
            sql.Append("INNER JOIN sys.fulltext_catalogs FC ON FC.fulltext_catalog_id = FI.fulltext_catalog_id  ");
            sql.Append("INNER JOIN sys.indexes I ON I.index_id = FI.unique_index_id and I.object_id = FI.object_id  ");
            sql.Append("INNER JOIN sys.tables T ON T.object_id = FI.object_id ");
            sql.Append("INNER JOIN sys.schemas S ON S.schema_id = T.schema_id ");
            sql.Append("INNER JOIN sys.fulltext_index_columns FIC ON FIC.object_id = FI.object_id ");
            sql.Append("INNER JOIN sys.columns C ON C.object_id = FIC.object_id AND C.column_id = FIC.column_id ");
            sql.Append("INNER JOIN sys.data_spaces DS ON DS.data_space_id = FI.data_space_id ");
            sql.Append("INNER JOIN sys.fulltext_languages FL ON FL.lcid = FIC.language_id ");
            sql.Append("ORDER BY OBJECT_NAME(FI.object_id), I.name  ");
            return sql.ToString();
        }
    }
}
