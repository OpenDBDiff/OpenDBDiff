using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using DBDiff.Schema.SQLServer2000.Model;

namespace DBDiff.Schema.SQLServer2000.Compare
{
    public class CompareTables
    {
        /// <summary>
        /// Compara las colecciones de tablas de dos bases diferentes y marca el estado de los objetos
        /// dependiendo si existen o si deben borrarse.
        /// </summary>
        /// <param name="tablasOrigen"></param>
        /// Tablas originales, donde se guardaran los estados de las tablas.
        /// <param name="tablasDestino">
        /// Tablas comparativas, que se usa para comparar con la base original.
        /// </param>
        public Tables GenerateDiferences(Tables tablasOrigen, Tables tablasDestino)
        {
            foreach (Table node in tablasDestino)
            {
                if (!tablasOrigen.Find(node.FullName))
                {
                    node.Status = StatusEnum.ObjectStatusType.CreateStatus;
                    tablasOrigen.Add(node);
                }
                else
                {
                    tablasOrigen[node.FullName].OriginalTable = tablasOrigen[node.FullName].Clone((Database)tablasOrigen[node.FullName].Parent);
                    tablasOrigen[node.FullName].Columns = (new CompareColumns()).GenerateDiferences(tablasOrigen[node.FullName].Columns, node.Columns);
                    tablasOrigen[node.FullName].Constraints = (new CompareConstraints()).GenerateDiferences(tablasOrigen[node.FullName].Constraints, node.Constraints);
                    tablasOrigen[node.FullName].Options = (new CompareTablesOptions()).GenerateDiferences(tablasOrigen[node.FullName].Options, node.Options);
                    tablasOrigen[node.FullName].Triggers = (new CompareTriggers()).GenerateDiferences(tablasOrigen[node.FullName].Triggers, node.Triggers);
                }
            }
            foreach (Table node in tablasOrigen)
            {
                if (!tablasDestino.Find(node.FullName))
                {
                    node.Status = StatusEnum .ObjectStatusType.DropStatus;
                }
            }            
            return tablasOrigen;
        }
    }
}
