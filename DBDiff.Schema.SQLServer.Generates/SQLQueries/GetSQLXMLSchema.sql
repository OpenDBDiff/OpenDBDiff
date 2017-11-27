SELECT  
xsc.name, 
xsc.xml_collection_id AS [ID], 
sch.name AS Owner, 
XML_SCHEMA_NAMESPACE(sch.Name, xsc.name) AS Text 
FROM sys.xml_schema_collections AS xsc 
INNER JOIN sys.schemas AS sch ON xsc.schema_id = sch.schema_id 
WHERE xsc.schema_id <> 4