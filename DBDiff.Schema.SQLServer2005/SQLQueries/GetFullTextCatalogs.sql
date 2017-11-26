SELECT S.Name as Owner,F.name AS FileGroupName, fulltext_catalog_id, FC.Name, path, FC.is_default, is_accent_sensitivity_on 
FROM 
	sys.fulltext_catalogs FC 
		LEFT JOIN sys.filegroups F ON F.data_space_id = FC.data_space_id 
		INNER JOIN sys.schemas S ON S.schema_id = FC.principal_id