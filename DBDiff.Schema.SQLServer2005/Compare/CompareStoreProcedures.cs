using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareStoreProcedures : CompareBase<StoreProcedure>
    {
        protected override void DoUpdate<Root>(SchemaList<StoreProcedure, Root> originFields, StoreProcedure node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                StoreProcedure newNode = node; //.Clone(originFields.Parent);

                if (node.CompareExceptWhitespace(originFields[node.FullName]))
                {
                    newNode.Status = Enums.ObjectStatusType.AlterWhitespaceStatus;
                }
                else
                {
                    newNode.Status = Enums.ObjectStatusType.AlterStatus;
                }

                originFields[node.FullName] = newNode;
            }
        }
    }
}
