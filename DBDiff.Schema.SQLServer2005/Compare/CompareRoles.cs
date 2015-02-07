using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareRoles : CompareBase<Role>
    {
        protected override void DoUpdate<Root>(SchemaList<Role, Root> CamposOrigen, Role node)
        {
            if (!node.Compare(CamposOrigen[node.FullName]))
            {
                Role newNode = node;
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                CamposOrigen[node.FullName] = newNode;
            }
        }
    }
}
