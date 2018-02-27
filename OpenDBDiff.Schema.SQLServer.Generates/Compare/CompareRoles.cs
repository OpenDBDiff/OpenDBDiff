using OpenDBDiff.Schema.Model;
using OpenDBDiff.Schema.SQLServer.Generates.Model;

namespace OpenDBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareRoles : CompareBase<Role>
    {
        protected override void DoUpdate<Root>(SchemaList<Role, Root> originFields, Role node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                Role newNode = node;
                newNode.Status = ObjectStatus.Alter;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
