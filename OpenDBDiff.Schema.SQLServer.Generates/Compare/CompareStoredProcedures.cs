using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.Schema.SQLServer.Generates.Model;

namespace OpenDBDiff.Schema.SQLServer.Generates.Compare
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
                    newNode.Status = ObjectStatus.AlterWhitespace;
                }
                else
                {
                    newNode.Status = ObjectStatus.Alter;
                }

                originFields[node.FullName] = newNode;
            }
        }
    }
}
