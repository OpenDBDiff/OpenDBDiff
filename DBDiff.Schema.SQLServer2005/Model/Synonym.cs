using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Synonym : SQLServerSchemaBase
    {
        private string value;

        public Synonym(ISchemaBase parent)
            : base(StatusEnum.ObjectTypeEnum.Synonym)
        {
            this.Parent = parent;
        }

        public Synonym Clone(ISchemaBase parent)
        {
            Synonym item = new Synonym(parent);
            item.Id = this.Id;
            item.Name = this.Name;
            item.Owner = this.Owner;
            item.Value = this.Value;
            item.Guid = this.Guid;
            return item;
        }

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public string ToSQL()
        {
            string sql = "CREATE SYNONYM " + FullName + " FOR " + value + "\r\nGO\r\n";
            return sql;
        }

        public override string ToSQLDrop()
        {
            return "DROP SYNONYM " + FullName + "\r\nGO\r\n";
        }

        public override string ToSQLAdd()
        {
            return ToSQL();
        }

        /// <summary>
        /// Devuelve el schema de diferencias del Schema en formato SQL.
        /// </summary>
        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == StatusEnum.ObjectStatusType.DropStatus)
            {
                listDiff.Add(ToSQLDrop(), 0, StatusEnum.ScripActionType.DropSynonyms);
            }
            if (this.Status == StatusEnum.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(ToSQL(), 0, StatusEnum.ScripActionType.AddSynonyms);
            }
            if (this.Status == StatusEnum.ObjectStatusType.AlterStatus)
            {
                listDiff.Add(ToSQLDrop(), 0, StatusEnum.ScripActionType.DropSynonyms);
                listDiff.Add(ToSQL(), 0, StatusEnum.ScripActionType.AddSynonyms);
            }
            return listDiff;
        }

        /// <summary>
        /// Compara dos Synonyms y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(Synonym origen, Synonym destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (!origen.Value.Equals(destino.Value)) return false;
            return true;
        }
    }
}
