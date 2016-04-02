using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareXMLSchemas : CompareBase<XMLSchema>
    {
        protected override void DoUpdate<Root>(SchemaList<XMLSchema, Root> originFields, XMLSchema node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                XMLSchema newNode = node.Clone(originFields.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
