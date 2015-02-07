using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareCLRTriggers : CompareBase<CLRTrigger>
    {
        protected override void DoUpdate<Root>(SchemaList<CLRTrigger, Root> CamposOrigen, CLRTrigger node)
        {
            if (!node.Compare(CamposOrigen[node.FullName]))
            {
                CLRTrigger newNode = node;
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                CompareExtendedProperties(newNode, CamposOrigen[node.FullName]);
                CamposOrigen[node.FullName] = newNode;
            }
        }
    }
}
