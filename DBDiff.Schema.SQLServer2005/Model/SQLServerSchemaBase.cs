using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public abstract class SQLServerSchemaBase:SchemaBase, ISQLServerSchemaBase
    {
        private List<ExtendedProperty> extendedProperties;

        protected SQLServerSchemaBase(Enums.ObjectType objectType):base("[", "]", objectType)
        {
            extendedProperties = new List<ExtendedProperty>();
        }

        public List<ExtendedProperty> ExtendedProperties
        {
            get { return extendedProperties; }
        }

        protected SQLScriptList ToSqlDiffExtendedProperties()
        {            
            SQLScriptList list = new SQLScriptList();
            if (this.Status != Enums.ObjectStatusType.CreateStatus)
            {
                extendedProperties.ForEach(item =>
                    {
                        if (item.Status == Enums.ObjectStatusType.CreateStatus)
                            list.Add(item.Create());
                        if (item.Status == Enums.ObjectStatusType.DropStatus)
                            list.Add(item.Drop());
                    });
            }
            return list;
        }
    }
}
