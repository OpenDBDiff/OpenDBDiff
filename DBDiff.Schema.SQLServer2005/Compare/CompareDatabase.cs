using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.Misc;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal static class CompareDatabase
    {        
        public static Database GenerateDiferences(Database Origen, Database Destino)
        {
            try
            {
                Database data = Origen;
                CompareTables.GenerateDiferences(Origen.Tables, Destino.Tables);
                CompareAssemblies.GenerateDiferences(Origen.Assemblies, Destino.Assemblies);
                CompareUserDataTypes.GenerateDiferences(Origen.UserTypes, Destino.UserTypes);
                CompareXMLSchemas.GenerateDiferences(Origen.XmlSchemas, Destino.XmlSchemas);
                CompareSchemas.GenerateDiferences(Origen.Schemas, Destino.Schemas);
                CompareFileGroups.GenerateDiferences(Origen.FileGroups, Destino.FileGroups);
                CompareRules.GenerateDiferences(Origen.Rules, Destino.Rules);
                CompareDDLTriggers.GenerateDiferences(Origen.DDLTriggers, Destino.DDLTriggers);
                CompareSynonyms.GenerateDiferences(Origen.Synonyms, Destino.Synonyms);
                CompareUsers.GenerateDiferences(Origen.Users, Destino.Users);
                CompareStoreProcedures.GenerateDiferences(Origen.Procedures, Destino.Procedures);
                CompareCLRStoreProcedure.GenerateDiferences(Origen.CLRProcedures, Destino.CLRProcedures);
                CompareViews.GenerateDiferences(Origen.Views, Destino.Views);
                CompareFunctions.GenerateDiferences(Origen.Functions, Destino.Functions);
                CompareRoles.GenerateDiferences(Origen.Roles, Destino.Roles);
                ComparePartitionFunction.GenerateDiferences(Origen.PartitionFunctions, Destino.PartitionFunctions); 
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
