using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class Defaults : SchemaList<Default, Database>
    {
        public Defaults(Database parent)
            : base(parent)
        {
        }
    }
}
