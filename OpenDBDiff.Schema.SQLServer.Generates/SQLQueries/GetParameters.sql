
select AP.is_output, AP.scale, AP.precision, '[' + SCHEMA_NAME(O.schema_id) + '].['+  O.name + ']' AS ObjectName, AP.name, TT.name AS TypeName, AP.max_length from sys.all_parameters AP 
INNER JOIN sys.types TT ON TT.user_type_id = AP.user_type_id 
INNER JOIN sys.objects O ON O.object_id = AP.object_id 
WHERE type = 'FS' AND AP.name <> '' ORDER BY O.object_id, AP.parameter_id 