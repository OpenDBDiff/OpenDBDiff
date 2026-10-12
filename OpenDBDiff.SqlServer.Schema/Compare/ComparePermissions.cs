using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Compare
{
    internal class ComparePermissions : CompareBase<Permission>
    {
        protected override void DoUpdate<Root>(SchemaList<Permission, Root> originFields, Permission node)
        {
            Permission origin = originFields[node.FullName];
            if (!node.Compare(origin))
            {
                Permission newNode = node;
                newNode.Parent = originFields.Parent;
                newNode.PreviousState = origin.State;
                newNode.Status = ObjectStatus.Alter;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
