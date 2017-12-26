using System.Text;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Generates.SQLCommands
{
    internal static class ConstraintSQLCommand
    {
        public static string GetUniqueKey(DatabaseInfo.VersionTypeEnum version)
        {
            if (version == DatabaseInfo.VersionTypeEnum.SQLServer2005) return GetUniqueKey2005();
            if (version == DatabaseInfo.VersionTypeEnum.SQLServer2008 ||
                version == DatabaseInfo.VersionTypeEnum.SQLServer2008R2) return GetUniqueKey2008();
            //Fall back to highest compatible version
            return GetUniqueKeyAzure();
        }

        public static string GetCheck(DatabaseInfo.VersionTypeEnum version)
        {
            if (version == DatabaseInfo.VersionTypeEnum.SQLServer2005) return GetCheck2005();
            //Fall back to highest compatible version            
            return GetCheck2008();
        }

        public static string GetPrimaryKey(DatabaseInfo.VersionTypeEnum version, Table table)
        {
            if (version == DatabaseInfo.VersionTypeEnum.SQLServer2000) return GetPrimaryKey2000(table);
            if (version == DatabaseInfo.VersionTypeEnum.SQLServer2005) return GetPrimaryKey2005();
            if (version == DatabaseInfo.VersionTypeEnum.SQLServer2008 ||
                version == DatabaseInfo.VersionTypeEnum.SQLServer2008R2)
                return GetPrimaryKey2008();
            //Fall back to highest compatible version            
            return GetPrimaryKeyAzure();
        }

        private static string GetUniqueKeyAzure()
        {
            //File Groups not supported in Azure
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT O.type as ObjectType, S.Name as Owner, I.object_Id AS id,'' as FileGroup, C.user_type_id, C.column_id, I.Index_id, C.Name AS ColumnName, I.Name, I.type, I.fill_factor, I.is_padded, I.allow_row_locks, I.allow_page_locks, I.ignore_dup_key, I.is_disabled, IC.is_descending_key, IC.is_included_column ");
            sql.Append("FROM sys.indexes I ");
            sql.Append("INNER JOIN sys.objects O ON O.object_id = I.object_id ");
            sql.Append("INNER JOIN sys.schemas S ON S.schema_id = O.schema_id ");
            sql.Append("INNER JOIN sys.index_columns IC ON IC.index_id = I.index_id AND IC.object_id = I.object_id ");
            sql.Append("INNER JOIN sys.columns C ON C.column_id = IC.column_id AND IC.object_id = C.object_id ");
            //sql.Append("LEFT JOIN sys.data_spaces AS dsidx ON dsidx.data_space_id = I.data_space_id ");
            sql.Append("WHERE is_unique_constraint = 1 AND O.type <> 'TF' ORDER BY I.object_id,I.Name");
            return sql.ToString();
        }

        private static string GetUniqueKey2008()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT O.type as ObjectType, S.Name as Owner, I.object_Id AS id,dsidx.Name as FileGroup, C.user_type_id, C.column_id, I.Index_id, C.Name AS ColumnName, I.Name, I.type, I.fill_factor, I.is_padded, I.allow_row_locks, I.allow_page_locks, I.ignore_dup_key, I.is_disabled, IC.is_descending_key, IC.is_included_column ");
            sql.Append("FROM sys.indexes I ");
            sql.Append("INNER JOIN sys.objects O ON O.object_id = I.object_id ");
            sql.Append("INNER JOIN sys.schemas S ON S.schema_id = O.schema_id ");
            sql.Append("INNER JOIN sys.index_columns IC ON IC.index_id = I.index_id AND IC.object_id = I.object_id ");
            sql.Append("INNER JOIN sys.columns C ON C.column_id = IC.column_id AND IC.object_id = C.object_id ");
            sql.Append("LEFT JOIN sys.data_spaces AS dsidx ON dsidx.data_space_id = I.data_space_id ");
            sql.Append("WHERE is_unique_constraint = 1 AND O.type <> 'TF' ORDER BY I.object_id,I.Name");
            return sql.ToString();
        }

        private static string GetUniqueKey2005()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT O.type as ObjectType, S.Name as Owner, I.object_Id AS id,dsidx.Name as FileGroup, C.user_type_id, C.column_id, I.Index_id, C.Name AS ColumnName, I.Name, I.type, I.fill_factor, I.is_padded, I.allow_row_locks, I.allow_page_locks, I.ignore_dup_key, I.is_disabled, IC.is_descending_key, IC.is_included_column ");
            sql.Append("FROM sys.indexes I ");
            sql.Append("INNER JOIN sys.objects O ON O.object_id = I.object_id ");
            sql.Append("INNER JOIN sys.schemas S ON S.schema_id = O.schema_id ");
            sql.Append("INNER JOIN sys.index_columns IC ON IC.index_id = I.index_id AND IC.object_id = I.object_id ");
            sql.Append("INNER JOIN sys.columns C ON C.column_id = IC.column_id AND IC.object_id = C.object_id ");
            sql.Append("LEFT JOIN sys.data_spaces AS dsidx ON dsidx.data_space_id = I.data_space_id ");
            sql.Append("WHERE is_unique_constraint = 1 AND O.type <> 'TF' ORDER BY I.object_id,I.Name");
            return sql.ToString();
        }

        private static string GetCheck2008()
        {
            string sql;
            sql = "SELECT  ";
            sql += "CC.parent_object_id, ";
            sql += "O.type as ObjectType, ";
            sql += "CC.object_id AS ID, ";
            sql += "CC.parent_column_id, ";
            sql += "CC.name, ";
            sql += "CC.type, ";
            sql += "CC.definition, ";
            sql += "CC.is_disabled, ";
            sql += "CC.is_not_trusted AS WithCheck, ";
            sql += "CC.is_not_for_replication, ";
            sql += "0, ";
            sql += "schema_name(CC.schema_id) AS Owner ";
            sql += "FROM sys.check_constraints CC ";
            sql += "INNER JOIN sys.objects O ON O.object_id = CC.parent_object_id ";
            sql += "ORDER BY CC.parent_object_id,CC.name";
            return sql;
        }

        private static string GetCheck2005()
        {
            string sql;
            sql = "SELECT  ";
            sql += "CC.parent_object_id, ";
            sql += "O.Type as ObjectType, ";
            sql += "CC.object_id AS ID, ";
            sql += "CC.parent_column_id, ";
            sql += "CC.name, ";
            sql += "CC.type, ";
            sql += "CC.definition, ";
            sql += "CC.is_disabled, ";
            sql += "CC.is_not_trusted AS WithCheck, ";
            sql += "CC.is_not_for_replication, ";
            sql += "0, ";
            sql += "schema_name(CC.schema_id) AS Owner ";
            sql += "FROM sys.check_constraints CC ";
            sql += "INNER JOIN sys.objects O ON O.object_id = CC.parent_object_id ";
            sql += "ORDER BY CC.parent_object_id,CC.name";
            return sql;
        }

        private static string GetPrimaryKeyAzure()
        {
            //File Groups not supported in Azure
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT O.type as ObjectType, S.Name as Owner, IC.key_ordinal, C.user_type_id, I.object_id AS ID, '' AS FileGroup, C.column_id, I.Index_id, C.Name AS ColumnName, I.Name, I.type, I.fill_factor, I.is_padded, I.allow_row_locks, I.allow_page_locks, I.ignore_dup_key, I.is_disabled, IC.is_descending_key, IC.is_included_column, CONVERT(bit,INDEXPROPERTY(I.object_id,I.name,'IsAutoStatistics')) AS IsAutoStatistics ");
            sql.Append("FROM sys.indexes I ");
            sql.Append("INNER JOIN sys.objects O ON O.object_id = I.object_id ");
            sql.Append("INNER JOIN sys.schemas S ON S.schema_id = O.schema_id ");
            sql.Append("INNER JOIN sys.index_columns IC ON IC.index_id = I.index_id AND IC.object_id = I.object_id ");
            sql.Append("INNER JOIN sys.columns C ON C.column_id = IC.column_id AND IC.object_id = C.object_id ");
            //sql.Append("LEFT JOIN sys.data_spaces AS dsidx ON dsidx.data_space_id = I.data_space_id ");
            sql.Append("WHERE is_primary_key = 1 AND O.type <> 'TF' ORDER BY I.object_id");
            return sql.ToString();
        }

        private static string GetPrimaryKey2008()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT O.type as ObjectType, S.Name as Owner, IC.key_ordinal, C.user_type_id, I.object_id AS ID, dsidx.Name AS FileGroup, C.column_id, I.Index_id, C.Name AS ColumnName, I.Name, I.type, I.fill_factor, I.is_padded, I.allow_row_locks, I.allow_page_locks, I.ignore_dup_key, I.is_disabled, IC.is_descending_key, IC.is_included_column, CONVERT(bit,INDEXPROPERTY(I.object_id,I.name,'IsAutoStatistics')) AS IsAutoStatistics ");
            sql.Append("FROM sys.indexes I ");
            sql.Append("INNER JOIN sys.objects O ON O.object_id = I.object_id ");
            sql.Append("INNER JOIN sys.schemas S ON S.schema_id = O.schema_id ");
            sql.Append("INNER JOIN sys.index_columns IC ON IC.index_id = I.index_id AND IC.object_id = I.object_id ");
            sql.Append("INNER JOIN sys.columns C ON C.column_id = IC.column_id AND IC.object_id = C.object_id ");
            sql.Append("LEFT JOIN sys.data_spaces AS dsidx ON dsidx.data_space_id = I.data_space_id ");
            sql.Append("WHERE is_primary_key = 1 AND O.type <> 'TF' ORDER BY I.object_id");
            return sql.ToString();
        }

        private static string GetPrimaryKey2005()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT O.type as ObjectType, S.Name as Owner, IC.key_ordinal, C.user_type_id, I.object_id AS ID, dsidx.Name AS FileGroup, C.column_id, I.Index_id, C.Name AS ColumnName, I.Name, I.type, I.fill_factor, I.is_padded, I.allow_row_locks, I.allow_page_locks, I.ignore_dup_key, I.is_disabled, IC.is_descending_key, IC.is_included_column, CONVERT(bit,INDEXPROPERTY(I.object_id,I.name,'IsAutoStatistics')) AS IsAutoStatistics ");
            sql.Append("FROM sys.indexes I ");
            sql.Append("INNER JOIN sys.objects O ON O.object_id = I.object_id ");
            sql.Append("INNER JOIN sys.schemas S ON S.schema_id = O.schema_id ");
            sql.Append("INNER JOIN sys.index_columns IC ON IC.index_id = I.index_id AND IC.object_id = I.object_id ");
            sql.Append("INNER JOIN sys.columns C ON C.column_id = IC.column_id AND IC.object_id = C.object_id ");
            sql.Append("INNER JOIN sys.data_spaces AS dsidx ON dsidx.data_space_id = I.data_space_id ");
            sql.Append("WHERE is_primary_key = 1 AND O.type <> 'TF' ORDER BY I.object_id");
            return sql.ToString();
        }

        private static string GetPrimaryKey2000(Table table)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT CONVERT(tinyint,CASE WHEN SI.indid = 0 THEN 0 WHEN SI.indid = 1 THEN 1 WHEN SI.indid > 1 THEN 2 END) AS Type,f.groupname AS FileGroup,CONVERT(int,SI.indid) AS Index_id, CONVERT(int,SI.indid) AS ID, SI.name, SC.colid, SC.Name AS ColumnName, CONVERT(bit,0) AS is_included_column, SIK.keyno AS key_ordinal, CONVERT(bit,INDEXPROPERTY(SI.id,SI.name,'IsPadIndex')) AS is_padded, CONVERT(bit,INDEXPROPERTY(SI.id,SI.name,'IsRowLockDisallowed')) AS allow_row_locks, CONVERT(bit,INDEXPROPERTY(SI.id,SI.name,'IsPageLockDisallowed')) AS allow_page_locks, CONVERT(bit,INDEXPROPERTY(SI.id,SI.name,'IsAutoStatistics')) AS IsAutoStatistics, CONVERT(tinyint,INDEXPROPERTY(SI.id,SI.name,'IndexFillFactor')) AS fill_factor,  INDEXKEY_PROPERTY(SI.id, SI.indid,SC.colid,'IsDescending') AS is_descending_key, CONVERT(bit,0) AS is_disabled, CONVERT(bit,0) AS is_included_column ");
            sql.Append("FROM sysindexes SI INNER JOIN sysindexkeys SIK ON SI.indid = SIK.indid AND SIK.id = SI.ID ");
            sql.Append("INNER JOIN syscolumns SC ON SC.colid = SIK.colid AND SC.id = SI.ID ");
            sql.Append("inner join sysfilegroups f on f.groupid = SI.groupid ");
            sql.Append("WHERE (SI.status & 0x800) = 0x800 AND SI.id = " + table.Id.ToString() + " ORDER BY SIK.keyno");
            return sql.ToString();
        }
    }
}
