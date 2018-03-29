SELECT ISNULL(CONVERT(varchar,AM.execute_as_principal_id),'CALLER') as ExecuteAs, O.type, AF.name AS assembly_name, AM.assembly_class, AM.assembly_id, AM.assembly_method, O.object_id, S.name as owner, O.name as name 
FROM sys.assembly_modules AM
LEFT JOIN sys.objects O ON AM.object_id = O.object_id 
INNER JOIN sys.schemas S ON S.schema_id = O.schema_id 
LEFT JOIN sys.assemblies AF ON AF.assembly_id = AM.assembly_id AND AF.is_user_defined = 1
UNION ALL
SELECT 'CALLER' as ExecuteAs, P.type, NULL AS assembly_name, NULL, NULL, NULL, P.object_id, S.name as owner, P.name as name 
FROM sys.procedures P 
INNER JOIN sys.schemas S ON S.schema_id = P.schema_id 
WHERE P.type = 'P'