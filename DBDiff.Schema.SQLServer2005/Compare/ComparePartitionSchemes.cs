using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class ComparePartitionSchemes : CompareBase<PartitionScheme>
    {
        protected override void DoUpdate<Root>(SchemaList<PartitionScheme, Root> CamposOrigen, PartitionScheme node)
        {
            if (!PartitionScheme.Compare(node, CamposOrigen[node.FullName]))
            {
                PartitionScheme newNode = node;//.Clone(CamposOrigen.Parent);
                newNode.Status = Enums.ObjectStatusType.RebuildStatus;
                CamposOrigen[node.FullName] = newNode;
            }
        }
    }
}
