using System;
using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareColumns
    {
        public void GenerateDifferences<T>(Columns<T> originFields, Columns<T> destinationFields) where T : ISchemaBase
        {
            int restPosition = 0;
            int sumPosition = 0;

            foreach (Column node in originFields)
            {
                if (!destinationFields.Exists(node.FullName))
                {
                    node.Status = Enums.ObjectStatusType.DropStatus;
                    restPosition++;
                }
                else
                    originFields[node.FullName].Position -= restPosition;
            }
            foreach (Column node in destinationFields)
            {
                if (!originFields.Exists(node.FullName))
                {
                    Column newNode = node.Clone(originFields.Parent);
                    if ((newNode.Position == 1) || ((newNode.DefaultConstraint == null) && (!newNode.IsNullable) && (!newNode.IsComputed) && (!newNode.IsIdentity) && (!newNode.IsIdentityForReplication)))
                    {
                        newNode.Status = Enums.ObjectStatusType.CreateStatus;
                        newNode.Parent.Status = Enums.ObjectStatusType.RebuildStatus;
                    }
                    else
                        newNode.Status = Enums.ObjectStatusType.CreateStatus;
                    sumPosition++;
                    originFields.Add(newNode);
                }
                else
                {
                    Column originField = originFields[node.FullName];
                    /*ColumnConstraint oldDefault = null;
                    if (originField.DefaultConstraint != null)
                        oldDefault = originField.DefaultConstraint.Clone(originField);*/
                    Boolean IsColumnEqual = Column.Compare(originField, node);
                    if ((!IsColumnEqual) || (originField.Position != node.Position))
                    {
                        if (Column.CompareIdentity(originField, node))
                        {

                            if (node.HasToRebuildOnlyConstraint)
                            {
                                node.Status = Enums.ObjectStatusType.AlterStatus;
                                if ((originField.IsNullable) && (!node.IsNullable))
                                    node.Status += (int)Enums.ObjectStatusType.UpdateStatus;
                            }
                            else
                            {
                                if (node.HasToRebuild(originField.Position + sumPosition, originField.Type, originField.IsFileStream))
                                    node.Status = Enums.ObjectStatusType.RebuildStatus;
                                else
                                {
                                    if (!IsColumnEqual)
                                    {
                                        node.Status = Enums.ObjectStatusType.AlterStatus;
                                        if ((originField.IsNullable) && (!node.IsNullable))
                                            node.Status += (int)Enums.ObjectStatusType.UpdateStatus;
                                    }
                                }
                            }
                            if (node.Status != Enums.ObjectStatusType.RebuildStatus)
                            {
                                if (!Column.CompareRule(originField, node))
                                {
                                    node.Status += (int)Enums.ObjectStatusType.BindStatus;
                                }
                            }
                        }
                        else
                        {
                            node.Status = Enums.ObjectStatusType.RebuildStatus;
                        }
                        originFields[node.FullName] = node.Clone(originFields.Parent);
                    }
                    originFields[node.FullName].DefaultConstraint = CompareColumnsConstraints.GenerateDifferences(originField, node);
                }
            }
        }
    }
}

