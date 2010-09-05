using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal class CompareTables:CompareBase<Table>
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
        public static void GenerateDiferences(Tables tablasOrigen, Tables tablasDestino)
        {
            MarkDrop(tablasOrigen, tablasDestino);

            foreach (Table node in tablasDestino)
            {
                if (!tablasOrigen.Exists(node.FullName))
                {
                    node.Status = Enums.ObjectStatusType.CreateStatus;
                    tablasOrigen.Add(node);
                }
                else
                {
                    if (node.Status != Enums.ObjectStatusType.DropStatus)
                    {
                        Table tablaOriginal = tablasOrigen[node.FullName];
                        tablaOriginal.OriginalTable = tablasOrigen[node.FullName].Clone((Database)tablaOriginal.Parent);
                        CompareColumns.GenerateDiferences(tablaOriginal.Columns, node.Columns);
                        CompareConstraints.GenerateDiferences(tablaOriginal.Constraints, node.Constraints);
                        CompareIndexes.GenerateDiferences(tablaOriginal.Indexes, node.Indexes);
                        CompareTablesOptions.GenerateDiferences(tablaOriginal.Options, node.Options);
                        CompareTriggers.GenerateDiferences(tablaOriginal.Triggers, node.Triggers);
                        if (!Table.CompareFileGroup(tablaOriginal, node))
                        {
                            tablaOriginal.FileGroup = node.FileGroup;
                            /*Esto solo aplica a las tablas heap, el resto hace el campo en el filegroup del indice clustered*/
                            if (!tablaOriginal.HasClusteredIndex)
                                tablaOriginal.Status = Enums.ObjectStatusType.AlterRebuildStatus;
                        }
                        if (!Table.CompareFileGroupText(tablaOriginal, node))
                        {
                            tablaOriginal.FileGroupText = node.FileGroupText;
                            tablaOriginal.Status = Enums.ObjectStatusType.AlterRebuildStatus;
                        }
                    }
                }
            }                       
        }
    }
}
