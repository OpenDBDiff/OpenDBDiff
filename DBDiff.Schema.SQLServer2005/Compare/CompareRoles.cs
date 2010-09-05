using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Compare
{
    internal class CompareRoles : CompareBase<Role>
    {
        protected override void DoUpdate<Root>(SchemaList<Role, Root> CamposOrigen, Role node)
        {
            if (!node.Compare(CamposOrigen[node.FullName]))
            {
                Role newNode = node;
                newNode.Status = Enums.ObjectStatusType.AlterStatus;
                CamposOrigen[node.FullName] = newNode;
            }
        }
    }
}
