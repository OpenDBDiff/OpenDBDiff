using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Model
{
    public class Default : SQLServerSchemaBase
    {
        public Default(ISchemaBase parent)
            : base(parent, ObjectType.Default)
        {
        }

        public new Default Clone(ISchemaBase parent)
        {
            Default item = new Default(parent);
            item.Id = this.Id;
            item.Name = this.Name;
            item.Owner = this.Owner;
            item.Value = this.Value;
            return item;
        }

        public string Value { get; set; }

        public string ToSQLAddBind()
        {
            string sql = "";
            sql += "EXEC sp_bindefault N'" + Name + "', N'" + this.Parent.Name + "'\r\nGO\r\n";
            return sql;
        }

        public string ToSQLAddUnBind()
        {
            string sql = "";
            sql += "EXEC sp_unbindefault @objname=N'" + this.Parent.Name + "'\r\nGO\r\n";
            return sql;
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        public override string ToSqlDrop()
        {
            return "DROP DEFAULT " + FullName + "\r\nGO\r\n";
        }

        public override string ToSql()
        {
            return "";
        }

        /// <summary>
        /// Devuelve el schema de diferencias del Schema en formato SQL.
        /// </summary>
        public override SQLScriptList ToSqlDiff(System.Collections.Generic.ICollection<ISchemaBase> schemas)
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == ObjectStatus.Drop)
            {
                listDiff.Add(ToSqlDrop(), 0, ScriptAction.DropRule);
            }
            if (this.Status == ObjectStatus.Create)
            {
                listDiff.Add(ToSql(), 0, ScriptAction.AddRule);
            }
            if (this.Status == ObjectStatus.Alter)
            {
                listDiff.Add(ToSqlDrop(), 0, ScriptAction.DropRule);
                listDiff.Add(ToSql(), 0, ScriptAction.AddRule);
            }
            return listDiff;
        }
    }
}
