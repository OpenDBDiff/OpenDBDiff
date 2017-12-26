select P.data_space_id AS ID, DS.Name AS FileGroupName,P.Name, F.name AS FunctionName 
from sys.partition_schemes P 
INNER JOIN sys.partition_functions F ON F.function_id = P.function_id 
INNER JOIN sys.destination_data_spaces DF ON DF.partition_scheme_id = P.data_space_id 
INNER JOIN sys.data_spaces DS ON DS.data_space_id = DF.data_space_id 
ORDER BY P.data_space_id, DF.destination_id 