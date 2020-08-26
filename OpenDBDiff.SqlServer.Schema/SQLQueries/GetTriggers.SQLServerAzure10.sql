SELECT T.object_id, O.type AS ObjectType, ISNULL(CONVERT(varchar,AM.execute_as_principal_id),'CALLER') as ExecuteAs, AF.name AS assembly_name, AM.assembly_class, AM.assembly_id, AM.assembly_method, T.type, CAST(ISNULL(tei.object_id,0) AS bit) AS IsInsert, CAST(ISNULL(teu.object_id,0) AS bit) AS IsUpdate, CAST(ISNULL(ted.object_id,0) AS bit) AS IsDelete, T.parent_id, S.name AS Owner,T.name,is_disabled,is_not_for_replication,is_instead_of_trigger 
FROM sys.triggers T 
INNER JOIN sys.objects O ON O.object_id = T.parent_id 
INNER JOIN sys.schemas S ON S.schema_id = O.schema_id 
LEFT JOIN sys.trigger_events AS tei ON tei.object_id = T.object_id and tei.type=1 
LEFT JOIN sys.trigger_events AS teu ON teu.object_id = T.object_id and teu.type=2 
LEFT JOIN sys.trigger_events AS ted ON ted.object_id = T.object_id and ted.type=3 

,(SELECT null as execute_as_principal_id, null as assembly_class, null as assembly_id, null as assembly_method) AS AM,
(SELECT null AS name) AS AF
ORDER BY T.parent_id