using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Compare
{
    internal class CompareCLRFunction : CompareBase<CLRFunction>
    {
        protected override void DoUpdate<Root>(SchemaList<CLRFunction, Root> originFields, CLRFunction node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                CLRFunction newNode = node; //.Clone(originFields.Parent);
                newNode.Status = ObjectStatus.Alter;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
