using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.Misc;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal static class CompareDatabase
    {        
        public static Database GenerateDiferences(Database Origen, Database Destino)
        {
            try
            {
                Database data = Origen;
                CompareTables.GenerateDiferences(Origen.Tables, Destino.Tables);
                (new CompareAssemblies()).GenerateDiferences(Origen.Assemblies, Destino.Assemblies);
                (new CompareUserDataTypes()).GenerateDiferences(Origen.UserTypes, Destino.UserTypes);
                (new CompareXMLSchemas()).GenerateDiferences(Origen.XmlSchemas, Destino.XmlSchemas);
                (new CompareSchemas()).GenerateDiferences(Origen.Schemas, Destino.Schemas);
                CompareFileGroups.GenerateDiferences(Origen.FileGroups, Destino.FileGroups);
                (new CompareRules()).GenerateDiferences(Origen.Rules, Destino.Rules);
                (new CompareDDLTriggers()).GenerateDiferences(Origen.DDLTriggers, Destino.DDLTriggers);
                (new CompareSynonyms()).GenerateDiferences(Origen.Synonyms, Destino.Synonyms);
                (new CompareUsers()).GenerateDiferences(Origen.Users, Destino.Users);
                (new CompareStoreProcedures()).GenerateDiferences(Origen.Procedures, Destino.Procedures);
                (new CompareCLRStoreProcedure()).GenerateDiferences(Origen.CLRProcedures, Destino.CLRProcedures);
                (new CompareViews()).GenerateDiferences(Origen.Views, Destino.Views);
                (new CompareFunctions()).GenerateDiferences(Origen.Functions, Destino.Functions);
                CompareRoles.GenerateDiferences(Origen.Roles, Destino.Roles);
                (new ComparePartitionFunction()).GenerateDiferences(Origen.PartitionFunctions, Destino.PartitionFunctions);
                (new ComparePartitionSchemes()).GenerateDiferences(Origen.PartitionSchemes, Destino.PartitionSchemes);
                CompareTableType.GenerateDiferences(Origen.TablesTypes, Destino.TablesTypes);
                (new CompareFullText()).GenerateDiferences(Origen.FullText, Destino.FullText);
                return data;
            }
            catch (SchemaException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SchemaException(ex.Message,ex);
            }
        }
    }
}
