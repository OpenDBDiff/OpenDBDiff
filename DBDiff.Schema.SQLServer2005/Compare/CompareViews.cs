using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Compare
{
    internal class CompareViews
    {
        private static void DoUpdate(SchemaList<View, Database> CamposOrigen, View node)
        {
            if (!node.Compare(CamposOrigen[node.FullName]))
            {
                View newNode = (View)node.Clone(CamposOrigen.Parent);
                newNode.DependenciesOut.AddRange(CamposOrigen[node.FullName].DependenciesOut);
                newNode.DependenciesIn.AddRange(CamposOrigen[node.FullName].DependenciesIn);

                newNode.Status = Enums.ObjectStatusType.AlterStatus;

                if (newNode.IsSchemaBinding)
                    newNode.Status += (int)Enums.ObjectStatusType.RebuildDependenciesStatus;
                if (newNode.HasToRebuild)
                    newNode.Status += (int)Enums.ObjectStatusType.RebuildStatus;
                else
                    newNode.Status += (int)Enums.ObjectStatusType.AlterBodyStatus;

                CamposOrigen[node.FullName] = newNode;
            }
            CompareIndexes.GenerateDiferences(CamposOrigen[node.FullName].Indexes, node.Indexes);
        }

        private static void DoNew(SchemaList<View, Database> CamposOrigen, View node)
        {
            View newNode = (View)node.Clone(CamposOrigen.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            CamposOrigen.Add(newNode);
            newNode.DependenciesIn.ForEach(dep =>
            {
                ISchemaBase item = CamposOrigen.Parent.Find(dep);
                if (item != null)
                {
                    if (item.IsCodeType)
                        ((ICode)item).DependenciesOut.Add(newNode.FullName);
                }
            }
            );
        }

        private static void DoDelete(View node)
        {
            node.Status = Enums.ObjectStatusType.DropStatus;
        }

        public static void GenerateDiferences(SchemaList<View, Database> CamposOrigen, SchemaList<View, Database> CamposDestino)
        {
            bool has = true;
            int DestinoIndex = 0;
            int OrigenIndex = 0;
            int DestinoCount = CamposDestino.Count;
            int OrigenCount = CamposOrigen.Count;
            View node;

            while (has)
            {
                has = false;
                if (DestinoCount > DestinoIndex)
                {
                    node = CamposDestino[DestinoIndex];
                    if (!CamposOrigen.Exists(node.FullName))
                        DoNew(CamposOrigen, node);
                    else
                        DoUpdate(CamposOrigen, node);

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
