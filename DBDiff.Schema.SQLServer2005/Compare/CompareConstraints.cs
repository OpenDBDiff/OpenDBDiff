using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareConstraints:CompareBase<Constraint>
    {
        private static void DoUpdate<T>(Constraints<T> CamposOrigen, Constraint node) where T:ISchemaBase
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

        private static void DoNew<T>(Constraints<T> CamposOrigen, Constraint node) where T:ISchemaBase
        {
            Constraint newNode = (Constraint)node.Clone(CamposOrigen.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            CamposOrigen.Add(newNode);
        }


        private static void DoDelete(Constraint node)
        {
            node.Status = Enums.ObjectStatusType.DropStatus;
        }

        public static void GenerateDiferences<T>(Constraints<T> CamposOrigen, Constraints<T> CamposDestino) where T:ISchemaBase
        {
            bool has = true;
            int DestinoIndex = 0;
            int OrigenIndex = 0;
            int DestinoCount = CamposDestino.Count;
            int OrigenCount = CamposOrigen.Count;
            Constraint node;

            while (has)
            {
                has = false;
                if (DestinoCount > DestinoIndex)
                {
                    node = CamposDestino[DestinoIndex];
                    if (!CamposOrigen.Exists(node.FullName))
                        DoNew<T>(CamposOrigen, node);
                    else
                        DoUpdate<T>(CamposOrigen, node);

                    DestinoIndex++;
                    has = true;
                }

                if (OrigenCount > OrigenIndex)
                {
                    node = CamposOrigen[OrigenIndex];
                    if (!CamposDestino.Exists(node.FullName))
                        DoDelete(node);

                    OrigenIndex++;
                    has = true;
                }
            }
        }
    }
}
