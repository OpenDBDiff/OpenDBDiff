using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.Schema.SQLServer.Generates.Model;

namespace OpenDBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareCLRStoredProcedure : CompareBase<CLRStoredProcedure>
    {
        protected override void DoUpdate<Root>(SchemaList<CLRStoredProcedure, Root> originFields, CLRStoredProcedure node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                CLRStoredProcedure newNode = node; //.Clone(originFields.Parent);
                newNode.Status = ObjectStatus.Alter;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
