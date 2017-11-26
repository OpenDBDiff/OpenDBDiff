select '[' + A.Name + ']' AS Name, AF.content AS FileContent, AF.File_Id AS FileId, AF.Name AS FileName
FROM sys.assemblies A 
INNER JOIN sys.assembly_files AF ON AF.assembly_id = A.assembly_id 
ORDER BY A.Name 