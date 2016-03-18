using System;
using DBDiff.Schema.Misc;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal static class CompareDatabase
    {
        public static Database GenerateDiferences(Database Origen, Database Destino)
        {
            try
            {
                Database data = Origen;
                (new CompareTables()).GenerateDiferences<Database>(Origen.Tables, Destino.Tables);
                (new CompareAssemblies()).GenerateDiferences<Database>(Origen.Assemblies, Destino.Assemblies);
                (new CompareUserDataTypes()).GenerateDiferences<Database>(Origen.UserTypes, Destino.UserTypes);
                (new CompareXMLSchemas()).GenerateDiferences<Database>(Origen.XmlSchemas, Destino.XmlSchemas);
                (new CompareSchemas()).GenerateDiferences<Database>(Origen.Schemas, Destino.Schemas);
                (new CompareFileGroups()).GenerateDiferences<Database>(Origen.FileGroups, Destino.FileGroups);
                (new CompareRules()).GenerateDiferences<Database>(Origen.Rules, Destino.Rules);
                (new CompareDDLTriggers()).GenerateDiferences<Database>(Origen.DDLTriggers, Destino.DDLTriggers);
                (new CompareSynonyms()).GenerateDiferences<Database>(Origen.Synonyms, Destino.Synonyms);
                (new CompareUsers()).GenerateDiferences<Database>(Origen.Users, Destino.Users);
                (new CompareStoreProcedures()).GenerateDiferences<Database>(Origen.Procedures, Destino.Procedures);
                (new CompareCLRStoreProcedure()).GenerateDiferences<Database>(Origen.CLRProcedures, Destino.CLRProcedures);
                (new CompareCLRFunction()).GenerateDiferences<Database>(Origen.CLRFunctions, Destino.CLRFunctions);
                (new CompareViews()).GenerateDiferences<Database>(Origen.Views, Destino.Views);
                (new CompareFunctions()).GenerateDiferences<Database>(Origen.Functions, Destino.Functions);
                (new CompareRoles()).GenerateDiferences<Database>(Origen.Roles, Destino.Roles);
                (new ComparePartitionFunction()).GenerateDiferences<Database>(Origen.PartitionFunctions, Destino.PartitionFunctions);
                (new ComparePartitionSchemes()).GenerateDiferences<Database>(Origen.PartitionSchemes, Destino.PartitionSchemes);
                (new CompareTableType()).GenerateDiferences<Database>(Origen.TablesTypes, Destino.TablesTypes);
                (new CompareFullText()).GenerateDiferences<Database>(Origen.FullText, Destino.FullText);
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
