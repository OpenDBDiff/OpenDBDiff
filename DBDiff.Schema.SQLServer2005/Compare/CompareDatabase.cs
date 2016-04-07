using System;
using DBDiff.Schema.Misc;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal static class CompareDatabase
    {
        public static Database GenerateDifferences(Database origin, Database destination)
        {
            try
            {
                Database data = origin;
                (new CompareTables()).GenerateDifferences<Database>(origin.Tables, destination.Tables);
                (new CompareAssemblies()).GenerateDifferences<Database>(origin.Assemblies, destination.Assemblies);
                (new CompareUserDataTypes()).GenerateDifferences<Database>(origin.UserTypes, destination.UserTypes);
                (new CompareXMLSchemas()).GenerateDifferences<Database>(origin.XmlSchemas, destination.XmlSchemas);
                (new CompareSchemas()).GenerateDifferences<Database>(origin.Schemas, destination.Schemas);
                (new CompareFileGroups()).GenerateDifferences<Database>(origin.FileGroups, destination.FileGroups);
                (new CompareRules()).GenerateDifferences<Database>(origin.Rules, destination.Rules);
                (new CompareDDLTriggers()).GenerateDifferences<Database>(origin.DDLTriggers, destination.DDLTriggers);
                (new CompareSynonyms()).GenerateDifferences<Database>(origin.Synonyms, destination.Synonyms);
                (new CompareUsers()).GenerateDifferences<Database>(origin.Users, destination.Users);
                (new CompareStoredProcedures()).GenerateDifferences<Database>(origin.Procedures, destination.Procedures);
                (new CompareCLRStoredProcedure()).GenerateDifferences<Database>(origin.CLRProcedures, destination.CLRProcedures);
                (new CompareCLRFunction()).GenerateDifferences<Database>(origin.CLRFunctions, destination.CLRFunctions);
                (new CompareViews()).GenerateDifferences<Database>(origin.Views, destination.Views);
                (new CompareFunctions()).GenerateDifferences<Database>(origin.Functions, destination.Functions);
                (new CompareRoles()).GenerateDifferences<Database>(origin.Roles, destination.Roles);
                (new ComparePartitionFunction()).GenerateDifferences<Database>(origin.PartitionFunctions, destination.PartitionFunctions);
                (new ComparePartitionSchemes()).GenerateDifferences<Database>(origin.PartitionSchemes, destination.PartitionSchemes);
                (new CompareTableType()).GenerateDifferences<Database>(origin.TablesTypes, destination.TablesTypes);
                (new CompareFullText()).GenerateDifferences<Database>(origin.FullText, destination.FullText);
                data.SourceInfo = destination.Info;
                return data;
            }
            catch (SchemaException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SchemaException(ex.Message, ex);
            }
        }
    }
}
