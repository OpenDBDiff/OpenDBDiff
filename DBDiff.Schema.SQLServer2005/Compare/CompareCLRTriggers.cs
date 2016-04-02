using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareCLRTriggers : CompareBase<CLRTrigger>
    {
        protected override void DoUpdate<Root>(SchemaList<CLRTrigger, Root> originFields, CLRTrigger node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                CLRTrigger newNode = node;
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                CompareExtendedProperties(newNode, originFields[node.FullName]);
                originFields[node.FullName] = newNode;
            }
        }
    }
}
