using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Compare
{
    internal class CompareXMLSchemas : CompareBase<XMLSchema>
    {
        protected override void DoUpdate<Root>(SchemaList<XMLSchema, Root> originFields, XMLSchema node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                XMLSchema newNode = node.Clone(originFields.Parent);
                newNode.Status = ObjectStatus.Alter;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
