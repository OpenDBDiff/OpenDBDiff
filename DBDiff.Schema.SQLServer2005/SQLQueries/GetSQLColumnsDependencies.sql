SELECT TT.type, 0 AS IsComputed, T.user_type_id,'[' + S.Name + '].[' + TT.Name + ']' AS TableName, '[' + S.Name + '].[' + TT.Name + '].[' + C.Name + ']' AS ColumnName,'[' + S2.Name + '].[' + T.Name + ']' AS TypeName FROM sys.types T 
INNER JOIN sys.columns C ON C.user_type_id = T.user_type_id 
INNER JOIN sys.objects TT ON TT.object_id = C.object_id 
INNER JOIN sys.schemas S ON S.schema_id = TT.schema_id 
INNER JOIN sys.schemas S2 ON S2.schema_id = T.schema_id 
WHERE is_user_defined = 1 
UNION 
SELECT TT.type, 1 AS IsComputed, T.user_type_id, '[' + S.Name + '].[' + TT.Name + ']' AS TableName, '[' + S.Name + '].[' + TT.Name + '].[' + C2.Name + ']' AS ColumnName, '[' + S2.Name + '].[' + T.Name + ']' AS TypeName FROM sys.types T 
INNER JOIN sys.columns C ON C.user_type_id = T.user_type_id 
INNER JOIN sys.sql_dependencies DEP ON DEP.referenced_major_id = C.object_id AND DEP.referenced_minor_id = C.column_Id AND DEP.object_id = C.object_id 
INNER JOIN sys.columns C2 ON C2.column_id = DEP.column_id AND C2.object_id = DEP.object_id 
INNER JOIN sys.objects TT ON TT.object_id = C2.object_id 
INNER JOIN sys.schemas S ON S.schema_id = TT.schema_id 
INNER JOIN sys.schemas S2 ON S2.schema_id = T.schema_id 
WHERE is_user_defined = 1 

UNION 
SELECT TT.type, 0 AS IsComputed, T.user_type_id,'[' + S.Name + '].[' + TT.Name + ']' AS TableName, '[' + S.Name + '].[' + TT.Name + '].[' + C.Name + ']' AS ColumnName,'[' + S2.Name + '].[' + T.Name + ']' AS TypeName from sys.sql_dependencies DEP 
INNER JOIN sys.objects TT ON DEP.object_id = TT.object_id 
INNER JOIN sys.schemas S ON S.schema_id = TT.schema_id 
INNER JOIN sys.parameters C ON C.object_id = TT.object_id AND C.parameter_id = DEP.referenced_minor_id 
INNER JOIN sys.types T ON C.user_type_id = T.user_type_id 
INNER JOIN sys.schemas S2 ON S2.schema_id = T.schema_id 
WHERE is_user_defined = 1 

ORDER BY IsComputed DESC,T.user_type_id