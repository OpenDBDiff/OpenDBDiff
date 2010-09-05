using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public abstract class SQLServerSchemaBase:SchemaBase
    {
        protected SQLServerSchemaBase(Enums.ObjectType objectType):base("[", "]", objectType)
        {            
        }
    }
}
