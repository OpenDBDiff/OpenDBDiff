SELECT
    P.class,
    P.permission_name,
    P.state,
    grantee.name AS grantee_name,
    COALESCE(OS.name, S.name, TS.name, XS.name) AS securable_schema,
    COALESCE(O.name, S.name, DP.name, A.name, T.name, X.name) AS securable_name,
    DP.type AS securable_principal_type,
    C.name AS column_name
FROM sys.database_permissions P
INNER JOIN sys.database_principals grantee ON grantee.principal_id = P.grantee_principal_id
LEFT JOIN sys.objects O ON P.class = 1 AND O.object_id = P.major_id
LEFT JOIN sys.schemas OS ON OS.schema_id = O.schema_id
LEFT JOIN sys.columns C ON P.class = 1 AND P.minor_id <> 0 AND C.object_id = P.major_id AND C.column_id = P.minor_id
LEFT JOIN sys.schemas S ON P.class = 3 AND S.schema_id = P.major_id
LEFT JOIN sys.database_principals DP ON P.class = 4 AND DP.principal_id = P.major_id
LEFT JOIN sys.assemblies A ON P.class = 5 AND A.assembly_id = P.major_id
LEFT JOIN sys.types T ON P.class = 6 AND T.user_type_id = P.major_id
LEFT JOIN sys.schemas TS ON TS.schema_id = T.schema_id
LEFT JOIN sys.xml_schema_collections X ON P.class = 10 AND X.xml_collection_id = P.major_id
LEFT JOIN sys.schemas XS ON XS.schema_id = X.schema_id
WHERE P.class IN (0, 1, 3, 4, 5, 6, 10)
    AND grantee.name NOT IN ('dbo', 'sys', 'INFORMATION_SCHEMA')
    AND (P.class <> 1 OR O.is_ms_shipped = 0)
    AND (P.class = 0 OR COALESCE(O.name, S.name, DP.name, A.name, T.name, X.name) IS NOT NULL)
ORDER BY P.class, securable_schema, securable_name, column_name, P.permission_name, grantee.name
