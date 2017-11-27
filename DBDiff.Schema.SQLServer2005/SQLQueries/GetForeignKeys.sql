SELECT FK.object_id, C.user_type_id ,FK.parent_object_id,S.Name AS Owner, S2.Name AS ReferenceOwner, C2.Name AS ColumnName, C2.column_id AS ColumnId, C.name AS ColumnRelationalName, C.column_id AS ColumnRelationalId, T.object_id AS TableRelationalId, FK.Parent_object_id AS TableId, T.Name AS TableRelationalName, FK.Name, FK.is_disabled, FK.is_not_for_replication, FK.is_not_trusted, FK.delete_referential_action, FK.update_referential_action 
FROM sys.foreign_keys FK 
INNER JOIN sys.tables T ON T.object_id = FK.referenced_object_id 
INNER JOIN sys.schemas S2 ON S2.schema_id = T.schema_id 
INNER JOIN sys.foreign_key_columns FKC ON FKC.constraint_object_id = FK.object_id 
INNER JOIN sys.columns C ON C.object_id = FKC.referenced_object_id AND C.column_id = referenced_column_id 
INNER JOIN sys.columns C2 ON C2.object_id = FKC.parent_object_id AND C2.column_id = parent_column_id 
INNER JOIN sys.schemas S ON S.schema_id = FK.schema_id 
ORDER BY FK.parent_object_id, FK.Name, ColumnId