# Set-ExecutionPolicy Bypass -Scope Process # not best but needed to run script within PowerShell CLI

# Build OpenDBDiff on Windows for .NET Framework 4 without any dev tools already installed
# Downloads NuGet.exe (to get recent compiler and dependencies), monoresgen (compile .resx)
$ErrorActionPreference = "Inquire"

mkdir net4 -ea 0
cd net4
mkdir nuget -ea 0
cd nuget
curl.exe -LO https://dist.nuget.org/win-x86-commandline/latest/nuget.exe

.\nuget.exe install Microsoft.Net.Compilers.Toolset -Version 4.7.0
.\nuget.exe install System.Runtime.CompilerServices.Unsafe -Version 4.5.3
.\nuget.exe install TSQL.Parser -Version 1.5.2
.\nuget.exe install Microsoft.Data.SqlClient -Version 5.1.1
.\nuget.exe install Microsoft.Extensions.Configuration.Binder -Version 6.0.0
.\nuget.exe install Microsoft.Extensions.Configuration.EnvironmentVariables -Version 6.0.0
.\nuget.exe install Microsoft.Extensions.Configuration.Json -Version 6.0.0
.\nuget.exe install CommandLineParser -Version 2.8.0
.\nuget.exe install DiffPlex -Version 1.6.3
.\nuget.exe install Essy.Tools.InputBox -Version 1.0.0
.\nuget.exe install jacobslusser.ScintillaNET -Version 3.6.3
.\nuget.exe install LiteDB -Version 5.0.13

copy TSQL.Parser.1.5.2\lib\net452\TSQL_Parser.dll ..
copy CommandLineParser.2.8.0\lib\net461\CommandLine.dll ..
copy Microsoft.Data.SqlClient.5.1.1\lib\net462\Microsoft.Data.SqlClient.dll ..
copy Microsoft.Extensions.Configuration.6.0.0\lib\net461\Microsoft.Extensions.Configuration.dll ..
copy Microsoft.Extensions.Configuration.Abstractions.6.0.0\lib\net461\Microsoft.Extensions.Configuration.Abstractions.dll ..
copy Microsoft.Extensions.Configuration.Binder.6.0.0\lib\net461\Microsoft.Extensions.Configuration.Binder.dll ..
copy Microsoft.Extensions.Configuration.EnvironmentVariables.6.0.0\lib\net461\Microsoft.Extensions.Configuration.EnvironmentVariables.dll ..
copy Microsoft.Extensions.Configuration.FileExtensions.6.0.0\lib\net461\Microsoft.Extensions.Configuration.FileExtensions.dll ..
copy Microsoft.Extensions.Configuration.Json.6.0.0\lib\net461\Microsoft.Extensions.Configuration.Json.dll ..
copy Microsoft.Extensions.FileProviders.Physical.6.0.0\lib\net461\Microsoft.Extensions.FileProviders.Physical.dll ..
copy Microsoft.Extensions.FileProviders.Abstractions.6.0.0\lib\net461\Microsoft.Extensions.FileProviders.Abstractions.dll ..
copy Microsoft.Extensions.Primitives.6.0.0\lib\net461\Microsoft.Extensions.Primitives.dll ..
copy System.Memory.4.5.4\lib\net461\System.Memory.dll ..
copy System.Runtime.CompilerServices.Unsafe.4.5.3\lib\net461\System.Runtime.CompilerServices.Unsafe.dll ..
copy Microsoft.Identity.Client.4.47.2\lib\net461\Microsoft.Identity.Client.dll ..
copy Microsoft.Data.SqlClient.SNI.5.1.0\build\net462\Microsoft.Data.SqlClient.SNI.x64.dll ..
copy System.Buffers.4.5.1\lib\net461\System.Buffers.dll ..
copy jacobslusser.ScintillaNET.3.6.3\lib\net40\ScintillaNET.dll ..
copy LiteDB.5.0.13\lib\net45\LiteDB.dll ..
copy DiffPlex.1.6.3\lib\net40\DiffPlex.dll ..
copy Essy.Tools.InputBox.1.0.0\lib\net20\InputBox.dll ..

curl.exe -LO https://github.com/mono/mono/raw/main/mcs/tools/resgen/monoresgen.cs
$PSDefaultParameterValues['Out-File:Encoding'] = 'utf8'
echo 'public class Consts { public const string MonoVersion = ""; }' >> monoresgen.cs
.\Microsoft.Net.Compilers.Toolset.4.7.0\tasks\net472\csc.exe /out:monoresgen.exe monoresgen.cs

cd ..\..

cd OpenDBDiff.Abstractions.Schema
..\net4\nuget\Microsoft.Net.Compilers.Toolset.4.7.0\tasks\net472\csc.exe `
 /t:library /out:..\net4\OpenDBDiff.Abstractions.Schema.dll `
 /recurse:*.cs ..\OpenDBDiff\Properties\AssemblyVersionInfo.cs

cd ..\OpenDBDiff.SqlServer.Schema
..\net4\nuget\Microsoft.Net.Compilers.Toolset.4.7.0\tasks\net472\csc.exe `
 /t:library /out:..\net4\OpenDBDiff.SqlServer.Schema.dll `
 /recurse:*.cs ..\OpenDBDiff\Properties\AssemblyVersionInfo.cs `
 /r:..\net4\OpenDBDiff.Abstractions.Schema.dll `
 /r:..\net4\TSQL_Parser.dll,..\net4\Microsoft.Data.SqlClient.dll `
 /res:'SQLQueries\GetAssemblies.sql,OpenDBDiff.SqlServer.Schema.SQLQueries.GetAssemblies.sql' `
 /res:'SQLQueries\GetAssemblyFiles.sql,OpenDBDiff.SqlServer.Schema.SQLQueries.GetAssemblyFiles.sql' `
 /res:'SQLQueries\GetDatabaseFile.sql,OpenDBDiff.SqlServer.Schema.SQLQueries.GetDatabaseFile.sql' `
 /res:'SQLQueries\GetDDLTriggers.sql,OpenDBDiff.SqlServer.Schema.SQLQueries.GetDDLTriggers.sql' `
 /res:'SQLQueries\GetDefaults.sql,OpenDBDiff.SqlServer.Schema.SQLQueries.GetDefaults.sql' `
 /res:'SQLQueries\GetExtendedProperties.sql,OpenDBDiff.SqlServer.Schema.SQLQueries.GetExtendedProperties.sql' `
 /res:'SQLQueries\GetFileGroups.sql,OpenDBDiff.SqlServer.Schema.SQLQueries.GetFileGroups.sql' `
 /res:'SQLQueries\GetForeignKeys.sql,OpenDBDiff.SqlServer.Schema.SQLQueries.GetForeignKeys.sql' `
 /res:'SQLQueries\GetFullTextCatalogs.sql,OpenDBDiff.SqlServer.Schema.SQLQueries.GetFullTextCatalogs.sql' `
 /res:'SQLQueries\GetParameters.sql,OpenDBDiff.SqlServer.Schema.SQLQueries.GetParameters.sql' `
 /res:'SQLQueries\GetPartitionFunctions.sql,OpenDBDiff.SqlServer.Schema.SQLQueries.GetPartitionFunctions.sql' `
 /res:'SQLQueries\GetPartitionSchemes.sql,OpenDBDiff.SqlServer.Schema.SQLQueries.GetPartitionSchemes.sql' `
 /res:'SQLQueries\GetProcedures.sql,OpenDBDiff.SqlServer.Schema.SQLQueries.GetProcedures.sql' `
 /res:'SQLQueries\GetProcedures.SQLServerAzure10.sql,OpenDBDiff.SqlServer.Schema.SQLQueries.GetProcedures.SQLServerAzure10.sql' `
 /res:'SQLQueries\GetRules.sql,OpenDBDiff.SqlServer.Schema.SQLQueries.GetRules.sql' `
 /res:'SQLQueries\GetSchemas.sql,OpenDBDiff.SqlServer.Schema.SQLQueries.GetSchemas.sql' `
 /res:'SQLQueries\GetSQLColumnsDependencies.sql,OpenDBDiff.SqlServer.Schema.SQLQueries.GetSQLColumnsDependencies.sql' `
 /res:'SQLQueries\GetSQLXMLSchema.sql,OpenDBDiff.SqlServer.Schema.SQLQueries.GetSQLXMLSchema.sql' `
 /res:'SQLQueries\GetSynonyms.sql,OpenDBDiff.SqlServer.Schema.SQLQueries.GetSynonyms.sql' `
 /res:'SQLQueries\GetTextObjectsQuery.sql,OpenDBDiff.SqlServer.Schema.SQLQueries.GetTextObjectsQuery.sql' `
 /res:'SQLQueries\GetTriggers.sql,OpenDBDiff.SqlServer.Schema.SQLQueries.GetTriggers.sql' `
 /res:'SQLQueries\GetTriggers.SQLServerAzure10.sql,OpenDBDiff.SqlServer.Schema.SQLQueries.GetTriggers.SQLServerAzure10.sql' `
 /res:'SQLQueries\GetXMLSchemaCollections.sql,OpenDBDiff.SqlServer.Schema.SQLQueries.GetXMLSchemaCollections.sql'

cd ..\OpenDBDiff.CLI
..\net4\nuget\Microsoft.Net.Compilers.Toolset.4.7.0\tasks\net472\csc.exe `
 /out:..\net4\OpenDBDiff.CLI.exe /recurse:*.cs ..\OpenDBDiff\Properties\AssemblyVersionInfo.cs `
 /r:..\net4\OpenDBDiff.Abstractions.Schema.dll,..\net4\OpenDBDiff.SqlServer.Schema.dll `
 /r:..\net4\CommandLine.dll,..\net4\Microsoft.Data.SqlClient.dll `
 /r:..\net4\Microsoft.Extensions.Configuration.dll,..\net4\Microsoft.Extensions.Configuration.Abstractions.dll `
 /r:..\net4\Microsoft.Extensions.Configuration.Binder.dll,..\net4\Microsoft.Extensions.Configuration.EnvironmentVariables.dll `
 /r:..\net4\Microsoft.Extensions.Configuration.FileExtensions.dll,..\net4\Microsoft.Extensions.Configuration.Json.dll

cd ..\OpenDBDiff.Abstractions.Ui
..\net4\nuget\Microsoft.Net.Compilers.Toolset.4.7.0\tasks\net472\csc.exe `
 /t:library /out:..\net4\OpenDBDiff.Abstractions.Ui.dll `
 /recurse:*.cs ..\OpenDBDiff\Properties\AssemblyVersionInfo.cs `
 /reference:..\net4\OpenDBDiff.Abstractions.Schema.dll

cd ..\OpenDBDiff.SqlServer.Ui
cd Properties
..\..\net4\nuget\monoresgen.exe Resources.resx
cd ..
..\net4\nuget\Microsoft.Net.Compilers.Toolset.4.7.0\tasks\net472\csc.exe `
 /t:library /out:..\net4\OpenDBDiff.SqlServer.Ui.dll `
 /recurse:*.cs ..\OpenDBDiff\Properties\AssemblyVersionInfo.cs `
 /r:..\net4\OpenDBDiff.Abstractions.Schema.dll,..\net4\OpenDBDiff.Abstractions.Ui.dll,..\net4\OpenDBDiff.SqlServer.Schema.dll `
 /r:,..\net4\Microsoft.Data.SqlClient.dll `
 /res:'Properties\Resources.resources,OpenDBDiff.SqlServer.Ui.Properties.Resources.resources'
del Properties\Resources.resources

cd ..\OpenDBDiff
cd Properties
..\..\net4\nuget\monoresgen.exe Resources.resx
cd ..\UI
..\..\net4\nuget\monoresgen.exe /compile DatabaseProgressControl.resx DataCompareForm.resx ErrorForm.resx `
 ListProjectsForm.resx MainForm.resx OptionForm.resx ProgressForm.resx SchemaTreeView.resx 
cd ..
..\net4\nuget\Microsoft.Net.Compilers.Toolset.4.7.0\tasks\net472\csc.exe `
 /out:..\net4\OpenDBDiff.exe /recurse:*.cs `
 /r:..\net4\OpenDBDiff.Abstractions.Schema.dll,..\net4\OpenDBDiff.Abstractions.Ui.dll `
 /r:..\net4\OpenDBDiff.SqlServer.Schema.dll,..\net4\OpenDBDiff.SqlServer.Ui.dll `
 /r:..\net4\Microsoft.Data.SqlClient.dll,..\net4\ScintillaNET.dll,..\net4\LiteDB.dll,..\net4\DiffPlex.dll,..\net4\InputBox.dll `
 /res:'Properties\Resources.resources,OpenDBDiff.Properties.Resources.resources' `
 /res:'UI\DatabaseProgressControl.resources,OpenDBDiff.Ui.DatabaseProgressControl.resources' `
 /res:'UI\DataCompareForm.resources,OpenDBDiff.Ui.DataCompareForm.resources' `
 /res:'UI\ErrorForm.resources,OpenDBDiff.Ui.ErrorForm.resources' `
 /res:'UI\ListProjectsForm.resources,OpenDBDiff.UI.ListProjectsForm.resources' `
 /res:'UI\MainForm.resources,OpenDBDiff.UI.MainForm.resources' `
 /res:'UI\OptionForm.resources,OpenDBDiff.UI.OptionForm.resources' `
 /res:'UI\ProgressForm.resources,OpenDBDiff.UI.ProgressForm.resources' `
 /res:'UI\SchemaTreeView.resources,OpenDBDiff.Ui.SchemaTreeView.resources'
del Properties\Resources.resources
del UI\*.resources

cd ..\net4
dir *.exe
pause