using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using System;

namespace OpenDBDiff.SqlServer.Schema.Model
{
    public class FullText : SQLServerSchemaBase
    {
        public FullText(ISchemaBase parent)
            : base(parent, ObjectType.FullText)
        {

        }

        public override string FullName
        {
            get { return "[" + Name + "]"; }
        }

        public string Path { get; set; }

        public Boolean IsDefault { get; set; }

        public Boolean IsAccentSensity { get; set; }

        public string FileGroupName { get; set; }

        public override string ToSql()
        {
            Database database = (Database)this.Parent;

            string sql = "CREATE FULLTEXT CATALOG " + FullName + " ";
            if (!IsAccentSensity)
                sql += "WITH ACCENT_SENSITIVITY = OFF\r\n";
            else
                sql += "WITH ACCENT_SENSITIVITY = ON\r\n";
            if (!String.IsNullOrEmpty(this.Path))
            {
                if (!database.Options.Ignore.FilterFullTextPath)
                    sql += "--";
                sql += "IN PATH N'" + Path + "'\r\n";
            }
            if (IsDefault)
                sql += "AS DEFAULT\r\n";
            sql += "AUTHORIZATION [" + Owner + "]\r\n";
            return sql + "GO\r\n";
        }

        private string ToSqlAlterDefault()
        {
            if (IsDefault)
            {
                string sql = "ALTER FULLTEXT CATALOG " + FullName + "\r\n";
                sql += "AS DEFAULT";
                sql += "\r\nGO\r\n";
                return sql;
            }
            else return "";

        }

        private string ToSqlAlterOwner()
        {
            string sql = "ALTER AUTHORIZATION ON FULLTEXT CATALOG::" + FullName + "\r\n";
            sql += "TO [" + Owner + "]\r\nGO\r\n";
            return sql;
        }

        private string ToSqlAlter()
        {
            string sql = "ALTER FULLTEXT CATALOG " + FullName + "\r\n";
            sql += "REBUILD WITH ACCENT_SENSITIVITY = ";
            if (IsAccentSensity) sql += "ON"; else sql += "OFF";
            sql += "\r\nGO\r\n";
            return sql;
        }

        public override string ToSqlDrop()
        {
            return "DROP FULLTEXT CATALOG " + FullName + "\r\nGO\r\n";
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        public override SQLScriptList ToSqlDiff(System.Collections.Generic.ICollection<ISchemaBase> schemas)
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == ObjectStatus.Drop)
            {
                listDiff.Add(ToSqlDrop(), 0, ScriptAction.DropFullText);
            }
            if (this.Status == ObjectStatus.Create)
            {
                listDiff.Add(ToSql(), 0, ScriptAction.AddFullText);
            }
            if (this.HasState(ObjectStatus.Alter))
            {
                listDiff.Add(ToSqlAlter(), 0, ScriptAction.AddFullText);
            }
            if (this.HasState(ObjectStatus.Disabled))
            {
                listDiff.Add(ToSqlAlterDefault(), 0, ScriptAction.AddFullText);
            }
            if (this.HasState(ObjectStatus.ChangeOwner))
            {
                listDiff.Add(ToSqlAlterOwner(), 0, ScriptAction.AddFullText);
            }
            return listDiff;
        }

        /// <summary>
        /// Compara dos Synonyms y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public Boolean Compare(FullText destination)
        {
            Database database = (Database)this.Parent;
            if (destination == null) throw new ArgumentNullException("destination");
            if (!this.IsAccentSensity.Equals(destination.IsAccentSensity)) return false;
            if (!this.IsDefault.Equals(destination.IsDefault)) return false;
            if ((!String.IsNullOrEmpty(this.FileGroupName)) && (!String.IsNullOrEmpty(destination.FileGroupName)))
                if (!this.FileGroupName.Equals(destination.FileGroupName)) return false;
            if (database.Options.Ignore.FilterFullTextPath)
                if ((!String.IsNullOrEmpty(this.Path)) && (!String.IsNullOrEmpty(destination.Path)))
                    return this.Path.Equals(destination.Path, StringComparison.CurrentCultureIgnoreCase);
            return true;
        }
    }
}
