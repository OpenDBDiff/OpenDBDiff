using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Compare
{
    internal class CompareCLRTriggers : CompareBase<CLRTrigger>
    {
        protected override void DoUpdate<Root>(SchemaList<CLRTrigger, Root> originFields, CLRTrigger node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                CLRTrigger newNode = node;
                newNode.Status = ObjectStatus.Alter;
                CompareExtendedProperties(newNode, originFields[node.FullName]);
                originFields[node.FullName] = newNode;
            }
        }
    }
}
