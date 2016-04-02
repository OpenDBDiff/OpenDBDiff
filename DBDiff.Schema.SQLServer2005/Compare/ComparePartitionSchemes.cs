using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class ComparePartitionSchemes : CompareBase<PartitionScheme>
    {
        protected override void DoUpdate<Root>(SchemaList<PartitionScheme, Root> originFields, PartitionScheme node)
        {
            if (!PartitionScheme.Compare(node, originFields[node.FullName]))
            {
                PartitionScheme newNode = node; //.Clone(originFields.Parent);
                newNode.Status = Enums.ObjectStatusType.RebuildStatus;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
