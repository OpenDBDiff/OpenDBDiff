using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class Roles : SchemaList<Role, Database>
    {
        public Roles(Database parent)
            : base(parent)
        {
        }
    }
}
