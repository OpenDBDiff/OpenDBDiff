using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using System;

namespace OpenDBDiff.SqlServer.Schema.Model
{
    public class Synonym : SQLServerSchemaBase
    {
        public Synonym(ISchemaBase parent)
            : base(parent, ObjectType.Synonym)
        {
        }

        public override ISchemaBase Clone(ISchemaBase parent)
        {
            Synonym item = new Synonym(parent);
            item.Id = this.Id;
            item.Name = this.Name;
            item.Owner = this.Owner;
            item.Value = this.Value;
            item.Guid = this.Guid;
            return item;
        }

        public string Value { get; set; }

        public override string ToSql()
        {
            string sql = "CREATE SYNONYM " + FullName + " FOR " + Value + "\r\nGO\r\n";
            return sql;
        }

        public override string ToSqlDrop()
        {
            return "DROP SYNONYM " + FullName + "\r\nGO\r\n";
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        /// <summary>
        /// Devuelve el schema de diferencias del Schema en formato SQL.
        /// </summary>
        public override SQLScriptList ToSqlDiff(System.Collections.Generic.ICollection<ISchemaBase> schemas)
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == ObjectStatus.Drop)
            {
                listDiff.Add(ToSqlDrop(), 0, ScriptAction.DropSynonyms);
            }
            if (this.Status == ObjectStatus.Create)
            {
                listDiff.Add(ToSql(), 0, ScriptAction.AddSynonyms);
            }
            if (this.Status == ObjectStatus.Alter)
            {
                listDiff.Add(ToSqlDrop(), 0, ScriptAction.DropSynonyms);
                listDiff.Add(ToSql(), 0, ScriptAction.AddSynonyms);
            }
            return listDiff;
        }

        /// <summary>
        /// Compara dos Synonyms y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(Synonym origin, Synonym destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (origin == null) throw new ArgumentNullException("origin");
            if (!origin.Value.Equals(destination.Value)) return false;
            return true;
        }
    }
}
