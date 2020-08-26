using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Compare
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
