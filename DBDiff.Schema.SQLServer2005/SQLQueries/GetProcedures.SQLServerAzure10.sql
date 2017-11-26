SELECT ISNULL(CONVERT(varchar,AM.execute_as_principal_id),'CALLER') as ExecuteAs, P.type, AF.name AS assembly_name, AM.assembly_class, AM.assembly_id, AM.assembly_method, P.object_id, S.name as owner, P.name as name 
FROM sys.procedures P 
INNER JOIN sys.schemas S ON S.schema_id = P.schema_id 
,(SELECT null as execute_as_principal_id, null as assembly_class, null as assembly_id, null as assembly_method) AS AM,
(SELECT null AS name) AS AF