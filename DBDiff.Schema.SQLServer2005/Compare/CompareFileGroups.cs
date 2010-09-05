using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareFileGroups:CompareBase<FileGroup>
    {
        protected override void DoNew<Root>(SchemaList<FileGroup, Root> CamposOrigen, FileGroup node)
        {
            FileGroup newNode = (FileGroup)node.Clone(CamposOrigen.Parent);
            newNode.Status = Enums.ObjectStatusType.CreateStatus;
            /*If the Logical File Name exists in another filegroup,
             * we must change the new Logical File Name.
             */
            CamposOrigen.ForEach(file =>
            {
                if (file.Status != Enums.ObjectStatusType.DropStatus)
                {
                    file.Files.ForEach(group =>
                    {
                        newNode.Files.ForEach(ngroup =>
                        {
                            if (group.CompareFullNameTo(group.FullName, ngroup.FullName) == 0)
                            {
                                newNode.Files[ngroup.FullName].Name = group.Name + "_2";
                            }
                        });
                    });
                }
            });
            CamposOrigen.Add(newNode);
        }

        protected override void DoUpdate<Root>(SchemaList<FileGroup, Root> CamposOrigen, FileGroup node)
        {
            if (!FileGroup.Compare(node, CamposOrigen[node.FullName]))
            {
                FileGroup newNode = (FileGroup)node.Clone(CamposOrigen.Parent);
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                CamposOrigen[node.FullName] = newNode;
            }
        }
    }
}