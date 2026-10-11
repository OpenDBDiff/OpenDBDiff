select C.scale, C.precision, '[' + SCHEMA_NAME(O.schema_id) + '].[' + O.name + ']' AS ObjectName, QUOTENAME(C.name) AS name, TT.name AS TypeName, C.max_length from sys.columns C
INNER JOIN sys.types TT ON TT.user_type_id = C.user_type_id
INNER JOIN sys.objects O ON O.object_id = C.object_id
WHERE O.type = 'FT' ORDER BY O.object_id, C.column_id
