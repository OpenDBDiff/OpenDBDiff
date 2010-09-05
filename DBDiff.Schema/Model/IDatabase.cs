using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBDiff.Schema.Model
{
    public interface IDatabase:ISchemaBase
    {
        bool IsCaseSensity { get; }
        SqlAction ActionMessage { get; }
    }
}
