using OpenDBDiff.Schema.Model;
using OpenDBDiff.Schema.SQLServer.Generates.Model;

namespace OpenDBDiff.Schema.SQLServer.Generates.Compare
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
                    newNode.Status = ObjectStatus.Alter;
                }
                else
                    newNode.Status = ObjectStatus.Alter + (int)ObjectStatus.Disabled;
                originFields[node.FullName] = newNode;
            }
            else
            {
                if (node.IsDisabled != origin.IsDisabled)
                {
                    Constraint newNode = (Constraint)node.Clone(originFields.Parent);
                    newNode.Status = ObjectStatus.Disabled;
                    originFields[node.FullName] = newNode;
                }
            }
        }

        protected override void DoNew<Root>(SchemaList<Constraint, Root> originFields, Constraint node)
        {
            Constraint newNode = (Constraint)node.Clone(originFields.Parent);
            newNode.Status = ObjectStatus.Create;
            originFields.Add(newNode);
        }
    }
}
