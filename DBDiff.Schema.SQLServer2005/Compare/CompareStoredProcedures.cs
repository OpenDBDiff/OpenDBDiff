using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareStoredProcedures : CompareBase<StoredProcedure>
    {
        protected override void DoUpdate<Root>(SchemaList<StoredProcedure, Root> originFields, StoredProcedure node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                StoredProcedure newNode = node; //.Clone(originFields.Parent);

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
