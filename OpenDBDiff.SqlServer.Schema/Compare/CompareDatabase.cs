using OpenDBDiff.Abstractions.Schema.Misc;
using OpenDBDiff.SqlServer.Schema.Model;
using System;

namespace OpenDBDiff.SqlServer.Schema.Compare
{
    internal static class CompareDatabase
    {
        /// <summary>
        /// Generates the differences to migrate a schema from origin to destination
        /// </summary>
        /// <param name="origin">The Origin schema is the schema before our generated actions are applied.</param>
        /// <param name="destination">The Destination schema is the schema after our actions are applied.</param>
        /// <returns></returns>
        /// <exception cref="SchemaException"></exception>
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
            catch (Exception ex)
            {
                throw new SchemaException(ex.Message, ex);
            }
        }
    }
}
