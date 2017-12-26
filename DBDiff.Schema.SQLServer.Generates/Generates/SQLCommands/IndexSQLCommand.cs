using System.Text;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Generates.SQLCommands
{
    internal static class IndexSQLCommand
    {
        public static string Get(DatabaseInfo.VersionTypeEnum version)
        {
            if (version == DatabaseInfo.VersionTypeEnum.SQLServer2005) return Get2005();
            if (version == DatabaseInfo.VersionTypeEnum.SQLServer2008 ||
                version == DatabaseInfo.VersionTypeEnum.SQLServer2008R2)
                return Get2008();

            //fall back to highest compatible version
            return GetAzure();

        }

        private static string Get2005()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT OO.type AS ObjectType, IC.key_ordinal, C.user_type_id, I.object_id, dsidx.Name as FileGroup, C.column_id,C.Name AS ColumnName, I.Name, I.index_id, I.type, is_unique, ignore_dup_key, is_primary_key, is_unique_constraint, fill_factor, is_padded, is_disabled, allow_row_locks, allow_page_locks, IC.is_descending_key, IC.is_included_column, ISNULL(ST.no_recompute,0) AS NoAutomaticRecomputation ");
            sql.Append("FROM sys.indexes I ");
            sql.Append("INNER JOIN sys.objects OO ON OO.object_id = I.object_id ");
            sql.Append("INNER JOIN sys.index_columns IC ON IC.index_id = I.index_id AND IC.object_id = I.object_id ");
            sql.Append("INNER JOIN sys.data_spaces AS dsidx ON dsidx.data_space_id = I.data_space_id ");
            sql.Append("INNER JOIN sys.columns C ON C.column_id = IC.column_id AND IC.object_id = C.object_id ");
            sql.Append("LEFT JOIN sys.stats AS ST ON ST.stats_id = I.index_id AND ST.object_id = I.object_id ");
            sql.Append("WHERE I.type IN (1,2,3) ");
            sql.Append("AND is_unique_constraint = 0 AND is_primary_key = 0 "); //AND I.object_id = " + table.Id.ToString(CultureInfo.InvariantCulture) + " ");
            sql.Append("AND objectproperty(I.object_id, 'IsMSShipped') <> 1 ");
            sql.Append("ORDER BY I.object_id, I.Name, IC.column_id");
            return sql.ToString();
        }

        private static string Get2008()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ISNULL(I.filter_definition,'') AS FilterDefinition, OO.type AS ObjectType, IC.key_ordinal, C.user_type_id, I.object_id, dsidx.Name as FileGroup, C.column_id,C.Name AS ColumnName, I.Name, I.index_id, I.type, is_unique, ignore_dup_key, is_primary_key, is_unique_constraint, fill_factor, is_padded, is_disabled, allow_row_locks, allow_page_locks, IC.is_descending_key, IC.is_included_column, ISNULL(ST.no_recompute,0) AS NoAutomaticRecomputation ");
            sql.Append("FROM sys.indexes I ");
            sql.Append("INNER JOIN sys.objects OO ON OO.object_id = I.object_id ");
            sql.Append("INNER JOIN sys.index_columns IC ON IC.index_id = I.index_id AND IC.object_id = I.object_id ");
            sql.Append("INNER JOIN sys.data_spaces AS dsidx ON dsidx.data_space_id = I.data_space_id ");
            sql.Append("INNER JOIN sys.columns C ON C.column_id = IC.column_id AND IC.object_id = C.object_id ");
            sql.Append("LEFT JOIN sys.stats AS ST ON ST.stats_id = I.index_id AND ST.object_id = I.object_id ");
            sql.Append("WHERE I.type IN (1,2,3) ");
            sql.Append("AND is_unique_constraint = 0 AND is_primary_key = 0 "); //AND I.object_id = " + table.Id.ToString(CultureInfo.InvariantCulture) + " ");
            sql.Append("AND objectproperty(I.object_id, 'IsMSShipped') <> 1 ");
            sql.Append("ORDER BY I.object_id, I.Name, IC.column_id");
            return sql.ToString();
        }

        private static string GetAzure()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ISNULL(I.filter_definition,'') AS FilterDefinition, OO.type AS ObjectType, IC.key_ordinal, C.user_type_id, I.object_id, '' as FileGroup, C.column_id,C.Name AS ColumnName, I.Name, I.index_id, I.type, is_unique, ignore_dup_key, is_primary_key, is_unique_constraint, fill_factor, is_padded, is_disabled, allow_row_locks, allow_page_locks, IC.is_descending_key, IC.is_included_column, ISNULL(ST.no_recompute,0) AS NoAutomaticRecomputation ");
            sql.Append("FROM sys.indexes I ");
            sql.Append("INNER JOIN sys.objects OO ON OO.object_id = I.object_id ");
            sql.Append("INNER JOIN sys.index_columns IC ON IC.index_id = I.index_id AND IC.object_id = I.object_id ");
            //sql.Append("INNER JOIN sys.data_spaces AS dsidx ON dsidx.data_space_id = I.data_space_id ");
            sql.Append("INNER JOIN sys.columns C ON C.column_id = IC.column_id AND IC.object_id = C.object_id ");
            sql.Append("LEFT JOIN sys.stats AS ST ON ST.stats_id = I.index_id AND ST.object_id = I.object_id ");
            sql.Append("WHERE I.type IN (1,2,3) ");
            sql.Append("AND is_unique_constraint = 0 AND is_primary_key = 0 "); //AND I.object_id = " + table.Id.ToString(CultureInfo.InvariantCulture) + " ");
            sql.Append("AND objectproperty(I.object_id, 'IsMSShipped') <> 1 ");
            sql.Append("ORDER BY I.object_id, I.Name, IC.column_id");
            return sql.ToString();
        }
    }
}
