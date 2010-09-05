using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal static class CompareDatabase
    {        
        public static Database GenerateDiferences(Database Origen, Database Destino)
        {
            Database data = Origen;
            Thread t1 = new Thread(delegate()
            {
                data.Tables = CompareTables.GenerateDiferences(Origen.Tables, Destino.Tables);
            });
            Thread t2 = new Thread(delegate()
            {
                data.UserTypes = CompareUserDataTypes.GenerateDiferences(Origen.UserTypes, Destino.UserTypes);
                data.XmlSchemas = CompareXMLSchemas.GenerateDiferences(Origen.XmlSchemas, Destino.XmlSchemas);
                data.Schemas = CompareSchemas.GenerateDiferences(Origen.Schemas, Destino.Schemas);                
                data.FileGroups = CompareFileGroups.GenerateDiferences(Origen.FileGroups, Destino.FileGroups);
                data.Rules = CompareRules.GenerateDiferences(Origen.Rules, Destino.Rules);
                data.DDLTriggers = CompareTriggers.GenerateDiferences(Origen.DDLTriggers, Destino.DDLTriggers);
                data.Synonyms = CompareSynonyms.GenerateDiferences(Origen.Synonyms, Destino.Synonyms);
                data.Assemblies = CompareAssemblies.GenerateDiferences(Origen.Assemblies, Destino.Assemblies);
            });
            Thread t3 = new Thread(delegate()
            {
                data.Procedures = CompareStoreProcedures.GenerateDiferences(Origen.Procedures, Destino.Procedures);
                data.Views = CompareViews.GenerateDiferences(Origen.Views, Destino.Views);
            });
            t1.Start();
            t2.Start();
            t3.Start();
            t1.Join();
            t2.Join();
            t3.Join();
            return data;
        }
    }
}
