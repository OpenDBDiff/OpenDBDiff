SELECT O.Type, '[' + S1.Name + '].[' + XS.Name +']' AS XMLName, '[' + S.Name + '].[' + O.Name +']' AS TableName, '[' + S.Name + '].[' + O.Name + '].[' + C.Name + ']' AS ColumnName from sys.columns C 
INNER JOIN sys.xml_schema_collections XS ON XS.xml_collection_id = C.xml_collection_id 
INNER JOIN sys.objects O ON O.object_id = C.object_id 
INNER JOIN sys.schemas S ON S.schema_id = O.schema_id 
INNER JOIN sys.schemas S1 ON S1.schema_id = XS.schema_id 
ORDER BY XS.xml_collection_id