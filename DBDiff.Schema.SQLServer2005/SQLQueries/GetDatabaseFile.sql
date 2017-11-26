select file_id,
type,
name,
physical_name,
size,
max_size,
growth,
is_sparse,
is_percent_growth 
from sys.database_files WHERE data_space_id = {ID}
