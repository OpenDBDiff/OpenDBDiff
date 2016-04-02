using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class ComparePartitionFunction : CompareBase<PartitionFunction>
    {
        protected override void DoUpdate<Root>(SchemaList<PartitionFunction, Root> originFields, PartitionFunction node)
        {
            if (!PartitionFunction.Compare(node, originFields[node.FullName]))
            {
                PartitionFunction newNode = node; //.Clone(originFields.Parent);
                newNode.Status = Enums.ObjectStatusType.RebuildStatus;
                originFields[node.FullName] = newNode;
            }
            else
            {
                if (!PartitionFunction.CompareValues(node, originFields[node.FullName]))
                {
                    PartitionFunction newNode = node.Clone(originFields.Parent);
                    if (newNode.Values.Count == originFields[node.FullName].Values.Count)
                        newNode.Status = Enums.ObjectStatusType.RebuildStatus;
                    else
                        newNode.Status = Enums.ObjectStatusType.AlterStatus;
                    newNode.Old = originFields[node.FullName].Clone(originFields.Parent);
                    originFields[node.FullName] = newNode;
                }
            }
        }
    }
}
