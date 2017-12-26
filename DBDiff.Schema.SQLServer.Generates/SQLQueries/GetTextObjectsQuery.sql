SELECT O.name, O.type, M.object_id, OBJECT_DEFINITION(M.object_id) AS Text FROM sys.sql_modules M 
INNER JOIN sys.objects O ON O.object_id = M.object_id 
WHERE {FILTER}