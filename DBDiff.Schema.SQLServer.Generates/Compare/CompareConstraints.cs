using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareConstraints : CompareBase<Constraint>
    {
        protected override void DoUpdate<Root>(SchemaList<Constraint, Root> originFields, Constraint node)
        {
            Constraint origin = originFields[node.FullName];
            if (!Constraint.Compare(origin, node))
            {
                Constraint newNode = (Constraint)node.Clone(originFields.Parent);
                if (node.IsDisabled == origin.IsDisabled)
                {
                    newNode.Status = Enums.ObjectStatusType.AlterStatus;
                }
                else
                    newNode.Status = Enums.ObjectStatusType.AlterStatus + (int)Enums.ObjectStatusType.DisabledStatus;
                originFields[node.FullName] = newNode;
            }
            else
            {
                if (node.IsDisabled != origin.IsDisabled)
                {
                    Constraint newNode = (Constraint)node.Clone(originFields.Parent);
                    newNode.Status = Enums.ObjectStatusType.DisabledStatus;
                    originFields[node.FullName] = newNode;
                }
            }
        }

        protected override void DoNew<Root>(SchemaList<Constraint, Root> originFields, Constraint node)
        {
            Constraint newNode = (Constraint)node.Clone(originFields.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            originFields.Add(newNode);
        }
    }
}
