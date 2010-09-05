using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class FullTexts : SchemaList<FullText, Database>
    {
        public FullTexts(Database parent)
            : base(parent)
        {
        }
    }
}
