using System.Text;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Generates.SQLCommands
{
    internal static class TableSQLCommand
    {
        #region Table Count
        public static string GetTableCount(DatabaseInfo.VersionTypeEnum version)
        {
            if (version == DatabaseInfo.VersionTypeEnum.SQLServer2000) return GetTableCount2000();
            //Fall back to highest compatible version
            return GetTableCount2005();
        }

        private static string GetTableCount2000()
        {
            return "SELECT Count(*) FROM sysobjects SO WHERE type = 'U'";
        }

        private static string GetTableCount2005()
        {
            return "SELECT Count(*) from sys.tables";
        }
        #endregion

        #region Table Detail
        public static string GetTableDetail(DatabaseInfo.VersionTypeEnum version)
        {
            if (version == DatabaseInfo.VersionTypeEnum.SQLServer2000) return GetTableDetail2000();
            if (version == DatabaseInfo.VersionTypeEnum.SQLServer2005) return GetTableDetail2005();
            if (version == DatabaseInfo.VersionTypeEnum.SQLServer2008 ||
                version == DatabaseInfo.VersionTypeEnum.SQLServer2008R2) return GetTableDetail2008();
            //Fall back to highest compatible version
            return GetTableDetailAzure();
        }

        private static string GetTableDetailAzure()
        {
            var sql = new StringBuilder();
            sql.AppendLine("SELECT DISTINCT 0 AS HasChangeTrackingTrackColumn, 0 AS HasChangeTracking, TTT.lock_escalation_desc, T.type AS ObjectType, C.Name, C.is_filestream, C.is_sparse, S4.Name as OwnerType,C.user_type_id, C.Column_Id AS ID, C.max_length AS Size, C.Precision, C.Scale, ISNULL(C.Collation_Name,'') as Collation, C.Is_nullable AS IsNullable, C.Is_RowGuidcol AS IsRowGuid, C.Is_Computed AS IsComputed, C.Is_Identity AS IsIdentity, COLUMNPROPERTY(T.object_id,C.name,'IsIdNotForRepl') AS IsIdentityRepl,IDENT_SEED('[' + S1.name + '].[' + T.Name + ']') AS IdentSeed, IDENT_INCR('[' + S1.name + '].[' + T.Name + ']') AS IdentIncrement, ISNULL(CC.Definition,'') AS Formula, ISNULL(CC.Is_Persisted,0) AS FormulaPersisted, ");
            sql.AppendLine("CASE WHEN ISNULL(DEP.referencing_minor_id,0) = 0 THEN 0 ELSE 1 END AS HasComputedFormula, CASE WHEN ISNULL(IC.column_id,0) = 0 THEN 0 ELSE 1 END AS HasIndex, TY.Name AS Type, '[' + S3.Name + '].' + XSC.Name AS XMLSchema, C.Is_xml_document, TY.is_user_defined, ");
            sql.AppendLine("ISNULL(TT.Name,T.Name) AS TableName, T.object_id AS TableId,S1.name AS TableOwner,Text_In_Row_limit, large_value_types_out_of_row,ISNULL(objectproperty(T.object_id, N'TableHasVarDecimalStorageFormat'),0) AS HasVarDecimal,OBJECTPROPERTY(T.OBJECT_ID,'TableHasClustIndex') AS HasClusteredIndex, ");
            sql.AppendLine(" '' AS FileGroup, '' AS FileGroupText, '' AS FileGroupStream,ISNULL(DC.object_id,0) AS DefaultId, DC.name AS DefaultName, DC.definition AS DefaultDefinition, C.rule_object_id, C.default_object_id ");
            sql.AppendLine("FROM sys.columns C ");
            sql.AppendLine("INNER JOIN sys.objects T ON T.object_id = C.object_id ");
            sql.AppendLine("INNER JOIN sys.types TY ON TY.user_type_id = C.user_type_id ");
            sql.AppendLine("LEFT JOIN sys.indexes IDX ON IDX.object_id = T.object_id and IDX.index_id < 2 ");
            //sql.Append("LEFT JOIN sys.data_spaces AS DSIDX ON DSIDX.data_space_id = IDX.data_space_id ");
            sql.AppendLine("LEFT JOIN sys.table_types TT ON TT.type_table_object_id = C.object_id ");
            sql.AppendLine("LEFT JOIN sys.tables TTT ON TTT.object_id = C.object_id ");
            sql.AppendLine("LEFT JOIN sys.schemas S1 ON (S1.schema_id = TTT.schema_id and T.type = 'U') OR (S1.schema_id = TT.schema_id and T.type = 'TT')");
            sql.AppendLine("LEFT JOIN sys.xml_schema_collections XSC ON XSC.xml_collection_id = C.xml_collection_id ");
            sql.AppendLine("LEFT JOIN sys.schemas S3 ON S3.schema_id = XSC.schema_id ");
            sql.AppendLine("LEFT JOIN sys.schemas S4 ON S4.schema_id = TY.schema_id ");
            sql.AppendLine("LEFT JOIN sys.computed_columns CC ON CC.column_id = C.column_Id AND C.object_id = CC.object_id ");
            sql.AppendLine("LEFT JOIN sys.sql_expression_dependencies DEP ON DEP.referenced_id = C.object_id AND DEP.referenced_minor_id = C.column_Id AND DEP.referencing_id = C.object_id ");
            sql.AppendLine("LEFT JOIN sys.index_columns IC ON IC.object_id = T.object_id AND IC.column_Id = C.column_Id ");
            //sql.Append("LEFT JOIN sys.data_spaces AS lob ON lob.data_space_id = TTT.lob_data_space_id ");
            //sql.Append("LEFT JOIN sys.data_spaces AS filestr ON filestr.data_space_id = TTT.filestream_data_space_id ");
            sql.AppendLine("LEFT JOIN sys.default_constraints DC ON DC.parent_object_id = T.object_id AND parent_column_id = C.Column_Id ");
            //sql.Append("LEFT JOIN sys.change_tracking_tables CTT ON CTT.object_id = T.object_id ");
            sql.AppendLine("WHERE T.type IN ('U','TT') ");
            sql.AppendLine("ORDER BY ISNULL(TT.Name,T.Name),T.object_id,C.column_id");
            return sql.ToString();
        }

        private static string GetTableDetail2008()
        {
            string sql = "";
            sql += "SELECT DISTINCT (CASE WHEN ISNULL(CTT.is_track_columns_updated_on,0) <> 0 THEN is_track_columns_updated_on ELSE 0 END) AS HasChangeTrackingTrackColumn, (CASE WHEN ISNULL(CTT.object_id,0) <> 0 THEN 1 ELSE 0 END) AS HasChangeTracking, TTT.lock_escalation_desc, T.type AS ObjectType, C.Name, C.is_filestream, C.is_sparse, S4.Name as OwnerType,C.user_type_id, C.Column_Id AS ID, C.max_length AS Size, C.Precision, C.Scale, ISNULL(C.Collation_Name,'') as Collation, C.Is_nullable AS IsNullable, C.Is_RowGuidcol AS IsRowGuid, C.Is_Computed AS IsComputed, C.Is_Identity AS IsIdentity, COLUMNPROPERTY(T.object_id,C.name,'IsIdNotForRepl') AS IsIdentityRepl,IDENT_SEED('[' + S1.name + '].[' + T.Name + ']') AS IdentSeed, IDENT_INCR('[' + S1.name + '].[' + T.Name + ']') AS IdentIncrement, ISNULL(CC.Definition,'') AS Formula, ISNULL(CC.Is_Persisted,0) AS FormulaPersisted, CASE WHEN ISNULL(DEP.column_id,0) = 0 THEN 0 ELSE 1 END AS HasComputedFormula, CASE WHEN ISNULL(IC.column_id,0) = 0 THEN 0 ELSE 1 END AS HasIndex, TY.Name AS Type, '[' + S3.Name + '].' + XSC.Name AS XMLSchema, C.Is_xml_document, TY.is_user_defined, ISNULL(TT.Name,T.Name) AS TableName, T.object_id AS TableId,S1.name AS TableOwner,Text_In_Row_limit, large_value_types_out_of_row,ISNULL(objectproperty(T.object_id, N'TableHasVarDecimalStorageFormat'),0) AS HasVarDecimal,OBJECTPROPERTY(T.OBJECT_ID,'TableHasClustIndex') AS HasClusteredIndex,DSIDX.Name AS FileGroup,ISNULL(lob.Name,'') AS FileGroupText, ISNULL(filestr.Name,'') AS FileGroupStream,ISNULL(DC.object_id,0) AS DefaultId, DC.name AS DefaultName, DC.definition AS DefaultDefinition, C.rule_object_id, C.default_object_id ";
            sql += "FROM sys.columns C ";
            sql += "INNER JOIN sys.objects T ON T.object_id = C.object_id ";
            sql += "INNER JOIN sys.types TY ON TY.user_type_id = C.user_type_id ";
            sql += "LEFT JOIN sys.indexes IDX ON IDX.object_id = T.object_id and IDX.index_id < 2 ";
            sql += "LEFT JOIN sys.data_spaces AS DSIDX ON DSIDX.data_space_id = IDX.data_space_id ";
            sql += "LEFT JOIN sys.table_types TT ON TT.type_table_object_id = C.object_id ";
            sql += "LEFT JOIN sys.tables TTT ON TTT.object_id = C.object_id ";
            sql += "LEFT JOIN sys.schemas S1 ON (S1.schema_id = TTT.schema_id and T.type = 'U') OR (S1.schema_id = TT.schema_id and T.type = 'TT')";
            sql += "LEFT JOIN sys.xml_schema_collections XSC ON XSC.xml_collection_id = C.xml_collection_id ";
            sql += "LEFT JOIN sys.schemas S3 ON S3.schema_id = XSC.schema_id ";
            sql += "LEFT JOIN sys.schemas S4 ON S4.schema_id = TY.schema_id ";
            sql += "LEFT JOIN sys.computed_columns CC ON CC.column_id = C.column_Id AND C.object_id = CC.object_id ";
            sql += "LEFT JOIN sys.sql_dependencies DEP ON DEP.referenced_major_id = C.object_id AND DEP.referenced_minor_id = C.column_Id AND DEP.object_id = C.object_id ";
            sql += "LEFT JOIN sys.index_columns IC ON IC.object_id = T.object_id AND IC.column_Id = C.column_Id ";
            sql += "LEFT JOIN sys.data_spaces AS lob ON lob.data_space_id = TTT.lob_data_space_id ";
            sql += "LEFT JOIN sys.data_spaces AS filestr ON filestr.data_space_id = TTT.filestream_data_space_id ";
            sql += "LEFT JOIN sys.default_constraints DC ON DC.parent_object_id = T.object_id AND parent_column_id = C.Column_Id ";
            sql += "LEFT JOIN sys.change_tracking_tables CTT ON CTT.object_id = T.object_id ";
            sql += "WHERE T.type IN ('U','TT') ";
            sql += "ORDER BY ISNULL(TT.Name,T.Name),T.object_id,C.column_id";
            return sql;
        }

        private static string GetTableDetail2005()
        {
            string sql = "";
            sql += "SELECT DISTINCT T.type AS ObjectType, C.Name, S4.Name as OwnerType,";
            sql += "C.user_type_id, C.Column_Id AS ID, C.max_length AS Size, C.Precision, C.Scale, ISNULL(C.Collation_Name,'') as Collation, C.Is_nullable AS IsNullable, C.Is_RowGuidcol AS IsRowGuid, C.Is_Computed AS IsComputed, C.Is_Identity AS IsIdentity, COLUMNPROPERTY(T.object_id,C.name,'IsIdNotForRepl') AS IsIdentityRepl,IDENT_SEED('[' + S1.name + '].[' + T.Name + ']') AS IdentSeed, IDENT_INCR('[' + S1.name + '].[' + T.Name + ']') AS IdentIncrement, ISNULL(CC.Definition,'') AS Formula, ISNULL(CC.Is_Persisted,0) AS FormulaPersisted, CASE WHEN ISNULL(DEP.column_id,0) = 0 THEN 0 ELSE 1 END AS HasComputedFormula, CASE WHEN ISNULL(IC.column_id,0) = 0 THEN 0 ELSE 1 END AS HasIndex, TY.Name AS Type, '[' + S3.Name + '].' + XSC.Name AS XMLSchema, C.Is_xml_document, TY.is_user_defined, ";
            sql += "T.Name AS TableName, T.object_id AS TableId,S1.name AS TableOwner,Text_In_Row_limit, large_value_types_out_of_row,ISNULL(objectproperty(T.object_id, N'TableHasVarDecimalStorageFormat'),0) AS HasVarDecimal,OBJECTPROPERTY(T.OBJECT_ID,'TableHasClustIndex') AS HasClusteredIndex,DSIDX.Name AS FileGroup,ISNULL(LOB.Name,'') AS FileGroupText, ";
            sql += "ISNULL(DC.object_id,0) AS DefaultId, DC.name AS DefaultName, DC.definition AS DefaultDefinition, C.rule_object_id, C.default_object_id ";
            sql += "FROM sys.columns C ";
            sql += "INNER JOIN sys.tables T ON T.object_id = C.object_id ";
            sql += "INNER JOIN sys.types TY ON TY.user_type_id = C.user_type_id ";
            sql += "INNER JOIN sys.schemas S1 ON S1.schema_id = T.schema_id ";
            sql += "INNER JOIN sys.indexes IDX ON IDX.object_id = T.object_id and IDX.index_id < 2 ";
            sql += "INNER JOIN sys.data_spaces AS DSIDX ON DSIDX.data_space_id = IDX.data_space_id ";
            sql += "LEFT JOIN sys.xml_schema_collections XSC ON XSC.xml_collection_id = C.xml_collection_id ";
            sql += "LEFT JOIN sys.schemas S3 ON S3.schema_id = XSC.schema_id ";
            sql += "LEFT JOIN sys.schemas S4 ON S4.schema_id = TY.schema_id ";
            sql += "LEFT JOIN sys.computed_columns CC ON CC.column_id = C.column_Id AND C.object_id = CC.object_id ";
            sql += "LEFT JOIN sys.sql_dependencies DEP ON DEP.referenced_major_id = C.object_id AND DEP.referenced_minor_id = C.column_Id AND DEP.object_id = C.object_id ";
            sql += "LEFT JOIN sys.index_columns IC ON IC.object_id = T.object_id AND IC.column_Id = C.column_Id ";
            sql += "LEFT JOIN sys.data_spaces AS LOB ON LOB.data_space_id = T.lob_data_space_id ";
            sql += "LEFT JOIN sys.default_constraints DC ON DC.parent_object_id = T.object_id AND parent_column_id = C.Column_Id ";
            sql += "ORDER BY T.Name,T.object_id,C.column_id";
            return sql;
        }

        private static string GetTableDetail2000()
        {
            string sql = "";
            sql += "SELECT SO.name, ";
            sql += "SO.id as object_id, ";
            sql += "SU.name as Owner, ";
            sql += "OBJECTPROPERTY(SO.ID,'TableTextInRowLimit') AS Text_In_Row_limit,";
            sql += "0 AS HasVarDecimal, ";
            sql += "CONVERT(bit,0) AS large_value_types_out_of_row, ";
            sql += "F.groupname AS FileGroup, ";
            sql += "ISNULL(F2.groupname,'') AS FileGroupText, ";
            sql += "OBJECTPROPERTY(SO.ID,'TableHasClustIndex') AS HasClusteredIndex ";
            sql += "FROM sysobjects SO ";
            sql += "inner join sysindexes I ON I.id = SO.id and I.indid < 2 ";
            sql += "inner join sysfilegroups f on f.groupid = i.groupid ";
            sql += "left join sysindexes I2 ON I2.id = SO.id and I2.indid = 255 ";
            sql += "left join sysfilegroups f2 on f2.groupid = i2.groupid ";
            sql += "INNER JOIN sysusers SU ON SU.uid = SO.uid WHERE type = 'U' ORDER BY SO.name";
            return sql;
        }
        #endregion

    }
}
