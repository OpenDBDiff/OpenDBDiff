using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Compare
{
    internal class ComparePartitionSchemes : CompareBase<PartitionScheme>
    {
        protected override void DoUpdate<Root>(SchemaList<PartitionScheme, Root> originFields, PartitionScheme node)
        {
            if (!PartitionScheme.Compare(node, originFields[node.FullName]))
            {
                PartitionScheme newNode = node; //.Clone(originFields.Parent);
                newNode.Status = ObjectStatus.Rebuild;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
