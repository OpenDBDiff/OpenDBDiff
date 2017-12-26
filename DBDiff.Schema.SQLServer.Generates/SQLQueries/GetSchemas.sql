
SELECT S1.name,S1.schema_id, S2.name AS Owner FROM sys.schemas S1
INNER JOIN sys.database_principals S2 ON S2.principal_id = S1.principal_id 