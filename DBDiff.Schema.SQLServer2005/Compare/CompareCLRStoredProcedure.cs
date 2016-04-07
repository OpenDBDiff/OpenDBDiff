using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareCLRStoredProcedure : CompareBase<CLRStoredProcedure>
    {
        protected override void DoUpdate<Root>(SchemaList<CLRStoredProcedure, Root> originFields, CLRStoredProcedure node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                CLRStoredProcedure newNode = node; //.Clone(originFields.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
