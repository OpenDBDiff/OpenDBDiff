SELECT OBJECT_DEFINITION(T.object_id) AS Text,T.name,is_disabled,is_not_for_replication,is_instead_of_trigger 
FROM sys.triggers T 
WHERE T.parent_id = 0 AND T.parent_class = 0