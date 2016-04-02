using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareRules : CompareBase<Rule>
    {
        protected override void DoUpdate<Root>(SchemaList<Rule, Root> originFields, Rule node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                Rule newNode = node.Clone(originFields.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                originFields[node.FullName] = newNode;
            }
        }

        protected override void DoNew<Root>(SchemaList<Rule, Root> originFields, Rule node)
        {
            Rule newNode = node.Clone(originFields.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            originFields.Add(newNode);
        }
    }
}
