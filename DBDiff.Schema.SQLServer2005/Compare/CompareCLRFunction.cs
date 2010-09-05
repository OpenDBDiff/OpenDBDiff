using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareCLRFunction : CompareBase<CLRFunction>
    {
        protected override void DoUpdate<Root>(SchemaList<CLRFunction, Root> CamposOrigen, CLRFunction node)
        {
            if (!node.Compare(CamposOrigen[node.FullName]))
            {
                CLRFunction newNode = node;//.Clone(CamposOrigen.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                CamposOrigen[node.FullName] = newNode;
            }
        }
    }
}
