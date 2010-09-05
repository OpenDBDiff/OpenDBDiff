using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareDDLTriggers : CompareBase<Trigger>
    {
        protected override void DoUpdate<Root>(SchemaList<Trigger, Root> CamposOrigen, Trigger node)
        {
            if (!node.Compare(CamposOrigen[node.FullName]))
            {
                Trigger newNode = (Trigger)node.Clone(CamposOrigen.Parent);
                if (!newNode.Text.Equals(CamposOrigen[node.FullName].Text))
                    newNode.Status = Enums.ObjectStatusType.AlterStatus;
                if (node.IsDisabled != CamposOrigen[node.FullName].IsDisabled)
                    newNode.Status = newNode.Status + (int)Enums.ObjectStatusType.DisabledStatus;
                CamposOrigen[node.FullName] = newNode;
            }            
        }

        protected override void DoNew<Root>(SchemaList<Trigger, Root> CamposOrigen, Trigger node)
        {
            Trigger newNode = (Trigger)node.Clone(CamposOrigen.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            CamposOrigen.Add(newNode);
        }
    }
}
