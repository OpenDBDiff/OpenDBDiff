SELECT DISTINCT T2.Name as ParentName, S1.name AS OwnerType, T.name AS TypeName, O.type, A.name AS AssemblyName, EP.*,S.name as Owner, O.name AS ObjectName, I1.name AS IndexName FROM sys.extended_properties EP 
LEFT JOIN sys.objects O ON O.object_id = EP.major_id 
LEFT JOIN sys.schemas S ON S.schema_id = O.schema_id 
LEFT JOIN sys.assemblies A ON A.assembly_id = EP.major_id 
LEFT JOIN sys.types T ON T.user_type_id = EP.major_id 
LEFT JOIN sys.schemas S1 ON S1.schema_id = T.schema_id 
LEFT JOIN sys.indexes I1 ON I1.index_id = EP.minor_id AND I1.object_id = O.object_ID AND class = 7 
LEFT JOIN sys.tables T2 ON T2.object_id = O.parent_object_id AND class = 1
ORDER BY major_id