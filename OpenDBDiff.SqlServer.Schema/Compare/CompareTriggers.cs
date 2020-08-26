using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Compare
{
    internal class CompareTriggers : CompareBase<Trigger>
    {
        protected override void DoNew<Root>(SchemaList<Trigger, Root> originFields, Trigger node)
        {
            Trigger newNode = (Trigger)node.Clone(originFields.Parent);
            newNode.Status = ObjectStatus.Create;
            originFields.Add(newNode);
        }

        protected override void DoUpdate<Root>(SchemaList<Trigger, Root> originFields, Trigger node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                Trigger newNode = (Trigger)node.Clone(originFields.Parent);
                if (!newNode.Text.Equals(originFields[node.FullName].Text))
                    newNode.Status = ObjectStatus.Alter;
                if (node.IsDisabled != originFields[node.FullName].IsDisabled)
                    newNode.Status = newNode.Status + (int)ObjectStatus.Disabled;
                originFields[node.FullName] = newNode;
            }
        }
    }
}
