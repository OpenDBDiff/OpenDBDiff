using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.Schema.SQLServer.Generates.Model;

namespace OpenDBDiff.Schema.SQLServer.Generates.Compare
{
    internal class ComparePartitionFunction : CompareBase<PartitionFunction>
    {
        protected override void DoUpdate<Root>(SchemaList<PartitionFunction, Root> originFields, PartitionFunction node)
        {
            if (!PartitionFunction.Compare(node, originFields[node.FullName]))
            {
                PartitionFunction newNode = node; //.Clone(originFields.Parent);
                newNode.Status = ObjectStatus.Rebuild;
                originFields[node.FullName] = newNode;
            }
            else
            {
                if (!PartitionFunction.CompareValues(node, originFields[node.FullName]))
                {
                    PartitionFunction newNode = node.Clone(originFields.Parent);
                    if (newNode.Values.Count == originFields[node.FullName].Values.Count)
                        newNode.Status = ObjectStatus.Rebuild;
                    else
                        newNode.Status = ObjectStatus.Alter;
                    newNode.Old = originFields[node.FullName].Clone(originFields.Parent);
                    originFields[node.FullName] = newNode;
                }
            }
        }
    }
}
