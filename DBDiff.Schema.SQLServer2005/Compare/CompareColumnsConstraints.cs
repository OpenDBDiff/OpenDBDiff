using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareColumnsConstraints : CompareBase<ColumnConstraint>
    {
        public static ColumnConstraint GenerateDifferences(Column originFields, Column destinationFields)
        {
            if ((originFields.DefaultConstraint == null) && (destinationFields.DefaultConstraint != null))
            {
                originFields.DefaultConstraint = destinationFields.DefaultConstraint.Clone(originFields);
                originFields.DefaultConstraint.Status = Enums.ObjectStatusType.CreateStatus;
                originFields.DefaultConstraint.Parent.Status = Enums.ObjectStatusType.OriginalStatus;
                originFields.DefaultConstraint.Parent.Parent.Status = Enums.ObjectStatusType.AlterStatus;
            }
            else
            {
                if ((originFields.DefaultConstraint != null) && (destinationFields.DefaultConstraint != null))
                {
                    if (!ColumnConstraint.Compare(originFields.DefaultConstraint, destinationFields.DefaultConstraint))
                    {
                        originFields.DefaultConstraint = destinationFields.DefaultConstraint.Clone(originFields);
                        //Indico que hay un ALTER TABLE, pero sobre la columna, no seteo ningun estado.
                        originFields.DefaultConstraint.Status = Enums.ObjectStatusType.AlterStatus;
                        originFields.DefaultConstraint.Parent.Status = Enums.ObjectStatusType.OriginalStatus;
                        originFields.DefaultConstraint.Parent.Parent.Status = Enums.ObjectStatusType.AlterStatus;
                    }
                }
                else
                    if ((originFields.DefaultConstraint != null) && (destinationFields.DefaultConstraint == null))
                {
                    originFields.DefaultConstraint.Status = Enums.ObjectStatusType.DropStatus;
                    originFields.DefaultConstraint.Parent.Status = Enums.ObjectStatusType.OriginalStatus;
                    originFields.DefaultConstraint.Parent.Parent.Status = Enums.ObjectStatusType.AlterStatus;
                }
            }
            /*foreach (ColumnConstraint node in destinationFields)
            {
                if (!originFields.Exists(node.FullName))
                {
                    node.Status = Enums.ObjectStatusType.CreateStatus;
                    originFields.Parent.Status = Enums.ObjectStatusType.OriginalStatus;
                    originFields.Parent.Parent.Status = Enums.ObjectStatusType.AlterStatus;
                    originFields.Add(node);
                }
                else
                {
                    if (!ColumnConstraint.Compare(originFields[node.FullName], node))
                    {
                        ColumnConstraint newNode = node.Clone(originFields.Parent);
                        //Indico que hay un ALTER TABLE, pero sobre la columna, no seteo ningun estado.
                        newNode.Status = Enums.ObjectStatusType.AlterStatus;
                        newNode.Parent.Status = Enums.ObjectStatusType.OriginalStatus;
                        newNode.Parent.Parent.Status = Enums.ObjectStatusType.AlterStatus;
                        originFields[node.FullName] = newNode;

                    }
                }
            }

            MarkDrop(originFields, destinationFields, node =>
            {
                node.Status = Enums.ObjectStatusType.DropStatus;
                originFields.Parent.Status = Enums.ObjectStatusType.OriginalStatus;
                originFields.Parent.Parent.Status = Enums.ObjectStatusType.AlterStatus;
            }
            );
            */
            return originFields.DefaultConstraint;
        }
    }
}
