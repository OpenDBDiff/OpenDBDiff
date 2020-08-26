using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Compare
{
    internal class CompareTableType : CompareBase<TableType>
    {
        protected override void DoUpdate<Root>(SchemaList<TableType, Root> originFields, TableType node)
        {
            if (node.Status != ObjectStatus.Drop)
            {
                TableType tablaOriginal = originFields[node.FullName];
                (new CompareColumns()).GenerateDifferences<TableType>(tablaOriginal.Columns, node.Columns);
                (new CompareConstraints()).GenerateDifferences<TableType>(tablaOriginal.Constraints, node.Constraints);
                (new CompareIndexes()).GenerateDifferences<TableType>(tablaOriginal.Indexes, node.Indexes);
            }
        }

        /*public static void GenerateDifferences(SchemaList<TableType, Database> originTables, SchemaList<TableType, Database> destinationTables)
        {
            MarkDrop(originTables, destinationTables);

            foreach (TableType node in destinationTables)
            {
                if (!originTables.Exists(node.FullName))
                {
                    node.Status = ObjectStatusType.CreateStatus;
                    node.Parent = originTables.Parent;
                    originTables.Add(node);
                }
                else
                {
                    if (node.Status != ObjectStatusType.DropStatus)
                    {
                        TableType tablaOriginal = originTables[node.FullName];
                        CompareColumns.GenerateDifferences<TableType>(tablaOriginal.Columns, node.Columns);
                        CompareConstraints.GenerateDifferences<TableType>(tablaOriginal.Constraints, node.Constraints);
                        CompareIndexes.GenerateDifferences(tablaOriginal.Indexes, node.Indexes);
                    }
                }
            }
        }*/
    }
}
