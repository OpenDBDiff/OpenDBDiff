using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareDDLTriggers : CompareBase<Trigger>
    {
        protected override void DoUpdate<Root>(SchemaList<Trigger, Root> originFields, Trigger node)
        {
            if (!node.Compare(originFields[node.FullName]))
            {
                Trigger newNode = (Trigger)node.Clone(originFields.Parent);
                if (!newNode.Text.Equals(originFields[node.FullName].Text))
                    newNode.Status = Enums.ObjectStatusType.AlterStatus;
                if (node.IsDisabled != originFields[node.FullName].IsDisabled)
                    newNode.Status = newNode.Status + (int)Enums.ObjectStatusType.DisabledStatus;
                originFields[node.FullName] = newNode;
            }
        }

        protected override void DoNew<Root>(SchemaList<Trigger, Root> originFields, Trigger node)
        {
            Trigger newNode = (Trigger)node.Clone(originFields.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            originFields.Add(newNode);
        }
    }
}
