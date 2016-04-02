using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareCLRFunction : CompareBase<CLRFunction>
    {
        protected override void DoUpdate<Root>(SchemaList<CLRFunction, Root> originFields, CLRFunction node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                CLRFunction newNode = node; //.Clone(originFields.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
