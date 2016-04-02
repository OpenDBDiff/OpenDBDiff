using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareTableType : CompareBase<TableType>
    {
        protected override void DoUpdate<Root>(SchemaList<TableType, Root> originFields, TableType node)
        {
            if (node.Status != Enums.ObjectStatusType.DropStatus)
            {
                TableType tablaOriginal = originFields[node.FullName];
                (new CompareColumns()).GenerateDifferences<TableType>(tablaOriginal.Columns, node.Columns);
                (new CompareConstraints()).GenerateDifferences<TableType>(tablaOriginal.Constraints, node.Constraints);
                (new CompareIndexes()).GenerateDifferences<TableType>(tablaOriginal.Indexes, node.Indexes);
            }
        }

        /*public static void GenerateDifferences(SchemaList<TableType, Database> tablasOrigen, SchemaList<TableType, Database> tablasDestino)
        {
            MarkDrop(tablasOrigen, tablasDestino);

            foreach (TableType node in tablasDestino)
            {
                if (!tablasOrigen.Exists(node.FullName))
                {
                    node.Status = Enums.ObjectStatusType.CreateStatus;
                    node.Parent = tablasOrigen.Parent;
                    tablasOrigen.Add(node);
                }
                else
                {
                    if (node.Status != Enums.ObjectStatusType.DropStatus)
                    {
                        TableType tablaOriginal = tablasOrigen[node.FullName];
                        CompareColumns.GenerateDifferences<TableType>(tablaOriginal.Columns, node.Columns);
                        CompareConstraints.GenerateDifferences<TableType>(tablaOriginal.Constraints, node.Constraints);
                        CompareIndexes.GenerateDifferences(tablaOriginal.Indexes, node.Indexes);
                    }
                }
            }
        }*/
    }
}
