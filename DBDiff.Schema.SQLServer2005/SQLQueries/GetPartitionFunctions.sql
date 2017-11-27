select PRV.value, T.name AS TypeName, PP.max_length, PP.precision, PP.scale, PF.Name, PF.function_id, fanout, boundary_value_on_right AS IsRight 
from sys.partition_functions PF 
INNER JOIN sys.partition_parameters PP ON PP.function_id = PF.function_id 
INNER JOIN sys.types T ON T.system_type_id = PP.system_type_id 
INNER JOIN sys.partition_range_values PRV ON PRV.parameter_id = PP.parameter_id and PP.function_id = PRV.function_id 
ORDER BY PP.function_id, PRV.parameter_id, boundary_id
