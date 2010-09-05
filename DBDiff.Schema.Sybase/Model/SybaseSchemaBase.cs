using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.Sybase.Model
{
    public abstract class SybaseSchemaBase:SchemaBase
    {
        protected SybaseSchemaBase(StatusEnum.ObjectTypeEnum objectType)
            : base("[", "]", objectType)
        {
        }
    }
}
