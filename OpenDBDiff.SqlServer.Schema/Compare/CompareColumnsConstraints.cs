using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.SqlServer.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Compare
{
    internal class CompareColumnsConstraints : CompareBase<ColumnConstraint>
    {
        public static ColumnConstraint GenerateDifferences(Column originFields, Column destinationFields)
        {
            if ((originFields.DefaultConstraint == null) && (destinationFields.DefaultConstraint != null))
            {
                originFields.DefaultConstraint = destinationFields.DefaultConstraint.Clone(originFields);
                originFields.DefaultConstraint.Status = ObjectStatus.Create;
                originFields.DefaultConstraint.Parent.Status = ObjectStatus.Original;
                originFields.DefaultConstraint.Parent.Parent.Status = ObjectStatus.Alter;
            }
            else
            {
                if ((originFields.DefaultConstraint != null) && (destinationFields.DefaultConstraint != null))
                {
                    if (!ColumnConstraint.Compare(originFields.DefaultConstraint, destinationFields.DefaultConstraint))
                    {
                        originFields.DefaultConstraint = destinationFields.DefaultConstraint.Clone(originFields);
                        //Indico que hay un ALTER TABLE, pero sobre la columna, no seteo ningun estado.
                        originFields.DefaultConstraint.Status = ObjectStatus.Alter;
                        originFields.DefaultConstraint.Parent.Status = ObjectStatus.Original;
                        originFields.DefaultConstraint.Parent.Parent.Status = ObjectStatus.Alter;
                    }
                }
                else
                    if ((originFields.DefaultConstraint != null) && (destinationFields.DefaultConstraint == null))
                {
                    originFields.DefaultConstraint.Status = ObjectStatus.Drop;
                    originFields.DefaultConstraint.Parent.Status = ObjectStatus.Original;
                    originFields.DefaultConstraint.Parent.Parent.Status = ObjectStatus.Alter;
                }
            }
            /*foreach (ColumnConstraint node in destinationFields)
            {
                if (!originFields.Exists(node.FullName))
                {
                    node.Status = ObjectStatusType.CreateStatus;
                    originFields.Parent.Status = ObjectStatusType.OriginalStatus;
                    originFields.Parent.Parent.Status = ObjectStatusType.AlterStatus;
                    originFields.Add(node);
                }
                else
                {
                    if (!ColumnConstraint.Compare(originFields[node.FullName], node))
                    {
                        ColumnConstraint newNode = node.Clone(originFields.Parent);
                        //Indico que hay un ALTER TABLE, pero sobre la columna, no seteo ningun estado.
                        newNode.Status = ObjectStatusType.AlterStatus;
                        newNode.Parent.Status = ObjectStatusType.OriginalStatus;
                        newNode.Parent.Parent.Status = ObjectStatusType.AlterStatus;
                        originFields[node.FullName] = newNode;

                    }
                }
            }

            MarkDrop(originFields, destinationFields, node =>
            {
                node.Status = ObjectStatusType.DropStatus;
                originFields.Parent.Status = ObjectStatusType.OriginalStatus;
                originFields.Parent.Parent.Status = ObjectStatusType.AlterStatus;
            }
            );
            */
            return originFields.DefaultConstraint;
        }
    }
}
