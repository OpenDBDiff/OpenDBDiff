using System;
using DBDiff.Schema.Misc;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal static class CompareDatabase
    {
        public static Database GenerateDifferences(Database Origen, Database Destino)
        {
            try
            {
                Database data = Origen;
                (new CompareTables()).GenerateDifferences<Database>(Origen.Tables, Destino.Tables);
                (new CompareAssemblies()).GenerateDifferences<Database>(Origen.Assemblies, Destino.Assemblies);
                (new CompareUserDataTypes()).GenerateDifferences<Database>(Origen.UserTypes, Destino.UserTypes);
                (new CompareXMLSchemas()).GenerateDifferences<Database>(Origen.XmlSchemas, Destino.XmlSchemas);
                (new CompareSchemas()).GenerateDifferences<Database>(Origen.Schemas, Destino.Schemas);
                (new CompareFileGroups()).GenerateDifferences<Database>(Origen.FileGroups, Destino.FileGroups);
                (new CompareRules()).GenerateDifferences<Database>(Origen.Rules, Destino.Rules);
                (new CompareDDLTriggers()).GenerateDifferences<Database>(Origen.DDLTriggers, Destino.DDLTriggers);
                (new CompareSynonyms()).GenerateDifferences<Database>(Origen.Synonyms, Destino.Synonyms);
                (new CompareUsers()).GenerateDifferences<Database>(Origen.Users, Destino.Users);
                (new CompareStoreProcedures()).GenerateDifferences<Database>(Origen.Procedures, Destino.Procedures);
                (new CompareCLRStoreProcedure()).GenerateDifferences<Database>(Origen.CLRProcedures, Destino.CLRProcedures);
                (new CompareCLRFunction()).GenerateDifferences<Database>(Origen.CLRFunctions, Destino.CLRFunctions);
                (new CompareViews()).GenerateDifferences<Database>(Origen.Views, Destino.Views);
                (new CompareFunctions()).GenerateDifferences<Database>(Origen.Functions, Destino.Functions);
                (new CompareRoles()).GenerateDifferences<Database>(Origen.Roles, Destino.Roles);
                (new ComparePartitionFunction()).GenerateDifferences<Database>(Origen.PartitionFunctions, Destino.PartitionFunctions);
                (new ComparePartitionSchemes()).GenerateDifferences<Database>(Origen.PartitionSchemes, Destino.PartitionSchemes);
                (new CompareTableType()).GenerateDifferences<Database>(Origen.TablesTypes, Destino.TablesTypes);
                (new CompareFullText()).GenerateDifferences<Database>(Origen.FullText, Destino.FullText);
                data.SourceInfo = Destino.Info;
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
