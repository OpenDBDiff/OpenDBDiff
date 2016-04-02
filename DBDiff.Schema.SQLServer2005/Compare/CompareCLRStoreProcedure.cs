using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareCLRStoreProcedure : CompareBase<CLRStoreProcedure>
    {
        protected override void DoUpdate<Root>(SchemaList<CLRStoreProcedure, Root> originFields, CLRStoreProcedure node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                CLRStoreProcedure newNode = node; //.Clone(originFields.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
