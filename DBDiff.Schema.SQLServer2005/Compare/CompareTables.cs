using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareTables : CompareBase<Table>
    {
        protected override void DoUpdate<Root>(SchemaList<Table, Root> originFields, Table node)
        {
            if (node.Status != Enums.ObjectStatusType.DropStatus)
            {
                Table tablaOriginal = originFields[node.FullName];
                tablaOriginal.OriginalTable = (Table)originFields[node.FullName].Clone((Database)tablaOriginal.Parent);
                (new CompareColumns()).GenerateDifferences<Table>(tablaOriginal.Columns, node.Columns);
                (new CompareConstraints()).GenerateDifferences<Table>(tablaOriginal.Constraints, node.Constraints);
                (new CompareIndexes()).GenerateDifferences<Table>(tablaOriginal.Indexes, node.Indexes);
                (new CompareTablesOptions()).GenerateDifferences<Table>(tablaOriginal.Options, node.Options);
                (new CompareTriggers()).GenerateDifferences<Table>(tablaOriginal.Triggers, node.Triggers);
                (new CompareCLRTriggers()).GenerateDifferences<Table>(tablaOriginal.CLRTriggers, node.CLRTriggers);
                (new CompareFullTextIndex()).GenerateDifferences<Table>(tablaOriginal.FullTextIndex, node.FullTextIndex);
                if (!Table.CompareFileGroup(tablaOriginal, node))
                {
                    tablaOriginal.FileGroup = node.FileGroup;
                    /*Esto solo aplica a las tablas heap, el resto hace el campo en el filegroup del indice clustered*/
                    if (!tablaOriginal.HasClusteredIndex)
                        tablaOriginal.Status = Enums.ObjectStatusType.RebuildStatus;
                }
                if (!Table.CompareFileGroupText(tablaOriginal, node))
                {
                    tablaOriginal.FileGroupText = node.FileGroupText;
                    tablaOriginal.Status = Enums.ObjectStatusType.RebuildStatus;
                }
                if (node.HasChangeTracking != tablaOriginal.HasChangeTracking)
                {
                    tablaOriginal.HasChangeTracking = node.HasChangeTracking;
                    tablaOriginal.HasChangeTrackingTrackColumn = node.HasChangeTrackingTrackColumn;
                    tablaOriginal.Status += (int)Enums.ObjectStatusType.DisabledStatus;
                }
            }
        }

        /// <summary>
        /// Compara las colecciones de tablas de dos bases diferentes y marca el estado de los objetos
        /// dependiendo si existen o si deben borrarse.
        /// </summary>
        /// <param name="originTables"></param>
        /// Tablas originales, donde se guardaran los estados de las tablas.
        /// <param name="destinationTables">
        /// Tablas comparativas, que se usa para comparar con la base original.
        /// </param>
        /*public static void GenerateDifferences(SchemaList<Table, Database> originTables, SchemaList<Table, Database> destinationTables)
        {
            MarkDrop(originTables, destinationTables);

            foreach (Table node in destinationTables)
            {
                if (!originTables.Exists(node.FullName))
                {
                    node.Status = Enums.ObjectStatusType.CreateStatus;
                    node.Parent = originTables.Parent;
                    originTables.Add(node);
                }
                else
                {
                    if (node.Status != Enums.ObjectStatusType.DropStatus)
                    {
                        Table tablaOriginal = originTables[node.FullName];
                        tablaOriginal.OriginalTable = (Table)originTables[node.FullName].Clone((Database)tablaOriginal.Parent);
                        CompareColumns.GenerateDifferences<Table>(tablaOriginal.Columns, node.Columns);
                        CompareConstraints.GenerateDifferences<Table>(tablaOriginal.Constraints, node.Constraints);
                        CompareIndexes.GenerateDifferences(tablaOriginal.Indexes, node.Indexes);
                        CompareTablesOptions.GenerateDifferences(tablaOriginal.Options, node.Options);
                        (new CompareTriggers()).GenerateDifferences<Table>(tablaOriginal.Triggers, node.Triggers);
                        (new CompareCLRTriggers()).GenerateDifferences<Table>(tablaOriginal.CLRTriggers, node.CLRTriggers);
                        if (!Table.CompareFileGroup(tablaOriginal, node))
                        {
                            tablaOriginal.FileGroup = node.FileGroup;
                            //Esto solo aplica a las tablas heap, el resto hace el campo en el filegroup del indice clustered
                            if (!tablaOriginal.HasClusteredIndex)
                                tablaOriginal.Status = Enums.ObjectStatusType.RebuildStatus;
                        }
                        if (!Table.CompareFileGroupText(tablaOriginal, node))
                        {
                            tablaOriginal.FileGroupText = node.FileGroupText;
                            tablaOriginal.Status = Enums.ObjectStatusType.RebuildStatus;
                        }
                    }
                }
            }
        }*/
    }
}
