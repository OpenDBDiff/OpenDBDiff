using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareStoreProcedures:CompareBase<StoreProcedure>
    {
        protected override void DoUpdate<Root>(SchemaList<StoreProcedure, Root> CamposOrigen, StoreProcedure node)
        {
            if (!node.Compare(CamposOrigen[node.FullName]))
            {
                StoreProcedure newNode = node;//.Clone(CamposOrigen.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                CamposOrigen[node.FullName] = newNode;
            }
        }
    }
}
