SELECT  
name, 
data_space_id AS [ID], 
is_default, 
is_read_only, 
type 
FROM sys.filegroups ORDER BY name