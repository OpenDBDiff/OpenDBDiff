using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class XMLSchema : SQLServerSchemaBase
    {
        private string text;

        public XMLSchema(Database parent):base(StatusEnum.ObjectTypeEnum.XMLSchema)
        {
            this.Parent = parent;            
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public string ToSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("CREATE XML SCHEMA COLLECTION ");
            sql.Append(this.FullName + " AS ");
            sql.Append("N'"+this.Text+"'");
            sql.Append("\r\nGO");
            return sql.ToString();
        }

        public override string ToSQLAdd()
        {
            return ToSQL();
        }

        public override string ToSQLDrop()
        {
            return "DROP XML SCHEMA COLLECTION " + FullName + "\r\nGO\r\n";
        }

        /// <summary>
        /// Devuelve el schema de diferencias del Schema en formato SQL.
        /// </summary>
        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == StatusEnum.ObjectStatusType.DropStatus)
            {
                listDiff.Add(ToSQLDrop(), 0, StatusEnum.ScripActionType.DropXMLSchema);
            }
            if (this.Status == StatusEnum.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(ToSQL(), 0, StatusEnum.ScripActionType.AddXMLSchema);
            }
            if (this.Status == StatusEnum.ObjectStatusType.AlterStatus)
            {
                listDiff.Add(ToSQLDrop(), 0, StatusEnum.ScripActionType.DropXMLSchema);
                listDiff.Add(ToSQL(), 0, StatusEnum.ScripActionType.AddXMLSchema);
            }
            return listDiff;
        }
    }
}
