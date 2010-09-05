using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareConstraints:CompareBase<Constraint>
    {
        protected override void DoUpdate<Root>(SchemaList<Constraint, Root> CamposOrigen, Constraint node) 
        {
            Constraint origen = CamposOrigen[node.FullName];
            if (!Constraint.Compare(origen, node))
            {
                Constraint newNode = (Constraint)node.Clone(CamposOrigen.Parent);
                if (node.IsDisabled == origen.IsDisabled)
                {
                    newNode.Status = Enums.ObjectStatusType.AlterStatus;
                }
                else
                    newNode.Status = Enums.ObjectStatusType.AlterStatus + (int)Enums.ObjectStatusType.DisabledStatus;
                CamposOrigen[node.FullName] = newNode;
            }
            else
            {
                if (node.IsDisabled != origen.IsDisabled)
                {
                    Constraint newNode = (Constraint)node.Clone(CamposOrigen.Parent);
                    newNode.Status = Enums.ObjectStatusType.DisabledStatus;
                    CamposOrigen[node.FullName] = newNode;
                }
            }
        }

        protected override void DoNew<Root>(SchemaList<Constraint, Root> CamposOrigen, Constraint node) 
        {
            Constraint newNode = (Constraint)node.Clone(CamposOrigen.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            CamposOrigen.Add(newNode);
        }
    }
}
