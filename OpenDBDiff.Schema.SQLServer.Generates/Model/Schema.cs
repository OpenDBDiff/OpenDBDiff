using System.Text;

namespace OpenDBDiff.Schema.SQLServer.Generates.Model
{
    public class Schema : SQLServerSchemaBase
    {
        public Schema(Database parent)
            : base(parent, ObjectType.Schema)
        {
        }

        public override string ToSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("CREATE SCHEMA ");
            sql.Append("[" + this.Name + "] AUTHORIZATION [" + Owner + "]");
            sql.Append("\r\nGO\r\n");
            return sql.ToString();
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        public override string ToSqlDrop()
        {
            return "DROP SCHEMA [" + Name + "]\r\nGO\r\n";
        }

        /// <summary>
        /// Devuelve el schema de diferencias del Schema en formato SQL.
        /// </summary>
        public override SQLScriptList ToSqlDiff(System.Collections.Generic.ICollection<OpenDBDiff.Schema.Model.ISchemaBase> schemas)
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == ObjectStatus.Drop)
            {
                listDiff.Add(ToSqlDrop(), 0, ScriptAction.DropSchema);
            }
            if (this.Status == ObjectStatus.Create)
            {
                listDiff.Add(ToSql(), 0, ScriptAction.AddSchema);
            }
            return listDiff;
        }
    }
}
