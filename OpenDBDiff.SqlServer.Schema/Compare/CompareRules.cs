using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Compare
{
    internal class CompareRules : CompareBase<Rule>
    {
        protected override void DoUpdate<Root>(SchemaList<Rule, Root> originFields, Rule node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                Rule newNode = node.Clone(originFields.Parent);
                newNode.Status = ObjectStatus.Alter;
                originFields[node.FullName] = newNode;
            }
        }

        protected override void DoNew<Root>(SchemaList<Rule, Root> originFields, Rule node)
        {
            Rule newNode = node.Clone(originFields.Parent);
            newNode.Status = ObjectStatus.Create;
            originFields.Add(newNode);
        }
    }
}
