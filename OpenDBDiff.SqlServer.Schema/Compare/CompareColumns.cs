using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model;
using System;

namespace OpenDBDiff.SqlServer.Schema.Compare
{
    public class CompareColumns
    {
        public void GenerateDifferences<T>(Columns<T> originFields, Columns<T> destinationFields) where T : ISchemaBase
        {
            int restPosition = 0;
            int sumPosition = 0;

            foreach (Column node in originFields)
            {
                if (!destinationFields.Contains(node.FullName))
                {
                    node.Status = ObjectStatus.Drop;
                    restPosition++;
                }
                else
                    originFields[node.FullName].Position -= restPosition;
            }
            foreach (Column node in destinationFields)
            {
                if (!originFields.Contains(node.FullName))
                {
                    Column newNode = node.Clone(originFields.Parent);
                    if ((newNode.Position == 1) || ((newNode.DefaultConstraint == null) && (!newNode.IsNullable) && (!newNode.IsComputed) && (!newNode.IsIdentity) && (!newNode.IsIdentityForReplication)))
                    {
                        newNode.Status = ObjectStatus.Create;
                        newNode.Parent.Status = ObjectStatus.Rebuild;
                    }
                    else
                        newNode.Status = ObjectStatus.Create;
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
                                node.Status = ObjectStatus.Alter;
                                if ((originField.IsNullable) && (!node.IsNullable))
                                    node.Status += (int)ObjectStatus.Update;
                            }
                            else
                            {
                                if (node.HasToRebuild(originField.Position + sumPosition, originField.Type, originField.IsFileStream))
                                    node.Status = ObjectStatus.Rebuild;
                                else
                                {
                                    if (!IsColumnEqual)
                                    {
                                        node.Status = ObjectStatus.Alter;
                                        if ((originField.IsNullable) && (!node.IsNullable))
                                            node.Status += (int)ObjectStatus.Update;
                                    }
                                }
                            }
                            if (node.Status != ObjectStatus.Rebuild)
                            {
                                if (!Column.CompareRule(originField, node))
                                {
                                    node.Status += (int)ObjectStatus.Bind;
                                }
                            }
                        }
                        else
                        {
                            node.Status = ObjectStatus.Rebuild;
                        }
                        originFields[node.FullName] = node.Clone(originFields.Parent);
                    }
                    originFields[node.FullName].DefaultConstraint = CompareColumnsConstraints.GenerateDifferences(originField, node);
                }
            }
        }
    }
}

