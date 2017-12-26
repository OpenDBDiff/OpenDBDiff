SELECT obj.object_id, Name, SCHEMA_NAME(obj.schema_id) AS Owner, ISNULL(smobj.definition, ssmobj.definition) AS [Definition] from sys.objects obj 
LEFT OUTER JOIN sys.sql_modules AS smobj ON smobj.object_id = obj.object_id
LEFT OUTER JOIN sys.system_sql_modules AS ssmobj ON ssmobj.object_id = obj.object_id 
where obj.type='D'
return sql;