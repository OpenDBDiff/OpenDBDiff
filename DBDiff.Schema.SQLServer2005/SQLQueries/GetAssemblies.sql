select DISTINCT '[' + S2.Name + '].[' + AT.Name + ']' as UDTName, 
ISNULL('[' + A2.name + ']','') AS Dependency, 
ISNULL('[' + S3.Name + '].[' + A3.name + ']','') AS ObjectDependency, 
AF.assembly_id, A.clr_name,A.name,S.name AS Owner, A.permission_set_desc, A.is_visible, content 
FROM sys.assemblies A 
INNER JOIN sys.assembly_files AF ON AF.assembly_id = A.assembly_id 
LEFT JOIN sys.assembly_references AR ON A.assembly_id = AR.referenced_assembly_id 
LEFT JOIN sys.assemblies A2 ON A2.assembly_id = AR.assembly_id 
LEFT JOIN sys.schemas S1 ON S1.schema_id = A2.principal_id 
INNER JOIN sys.schemas S ON S.schema_id = A.principal_id 
LEFT JOIN sys.assembly_types AT ON AT.assembly_id = A.assembly_id 
LEFT JOIN sys.schemas S2 ON S2.schema_id = AT.schema_id 
LEFT JOIN sys.assembly_modules AM ON AM.assembly_id = A.assembly_id 
LEFT JOIN sys.objects A3 ON A3.object_id = AM.object_id 
LEFT JOIN sys.schemas S3 ON S3.schema_id = A3.schema_id 
ORDER BY A.name