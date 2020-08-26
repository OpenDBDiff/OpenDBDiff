using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using System;

namespace OpenDBDiff.SqlServer.Schema.Model
{
    public class TableOption : SQLServerSchemaBase
    {
        public TableOption(string Name, string value, ISchemaBase parent)
            : base(parent, ObjectType.TableOption)
        {
            this.Name = Name;
            this.Value = value;
        }

        public TableOption(ISchemaBase parent)
            : base(parent, ObjectType.TableOption)
        {
        }

        /// <summary>
        /// Clona el objeto en una nueva instancia.
        /// </summary>
        public override ISchemaBase Clone(ISchemaBase parent)
        {
            TableOption option = new TableOption(parent);
            option.Name = this.Name;
            option.Status = this.Status;
            option.Value = this.Value;
            return option;
        }

        public string Value { get; set; }

        /// <summary>
        /// Compara dos indices y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(TableOption origin, TableOption destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (origin == null) throw new ArgumentNullException("origin");
            if (!destination.Value.Equals(origin.Value)) return false;
            return true;
        }

        public override string ToSqlDrop()
        {
            if (this.Name.Equals("TextInRow"))
                return "EXEC sp_tableoption " + Parent.Name + ", 'text in row','off'\r\nGO\r\n";
            if (this.Name.Equals("LargeValues"))
                return "EXEC sp_tableoption " + Parent.Name + ", 'large value types out of row','0'\r\nGO\r\n";
            if (this.Name.Equals("VarDecimal"))
                return "EXEC sp_tableoption " + Parent.Name + ", 'vardecimal storage format','0'\r\nGO\r\n";
            if (this.Name.Equals("LockEscalation"))
                return "";
            return "";
        }

        public override string ToSql()
        {
            if (this.Name.Equals("TextInRow"))
                return "EXEC sp_tableoption " + Parent.Name + ", 'text in row'," + Value + "\r\nGO\r\n";
            if (this.Name.Equals("LargeValues"))
                return "EXEC sp_tableoption " + Parent.Name + ", 'large value types out of row'," + Value + "\r\nGO\r\n";
            if (this.Name.Equals("VarDecimal"))
                return "EXEC sp_tableoption " + Parent.Name + ", 'vardecimal storage format','1'\r\nGO\r\n";
            if (this.Name.Equals("LockEscalation"))
            {
                if ((!this.Value.Equals("TABLE")) || (this.Status != ObjectStatus.Original))
                    return "ALTER TABLE " + Parent.Name + " SET (LOCK_ESCALATION = " + Value + ")\r\nGO\r\n";
            }
            return "";
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        public override SQLScriptList ToSqlDiff(System.Collections.Generic.ICollection<ISchemaBase> schemas)
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == ObjectStatus.Drop)
                listDiff.Add(ToSqlDrop(), 0, ScriptAction.AddOptions);
            if (this.Status == ObjectStatus.Create)
                listDiff.Add(ToSql(), 0, ScriptAction.DropOptions);
            if (this.Status == ObjectStatus.Alter)
            {
                listDiff.Add(ToSqlDrop(), 0, ScriptAction.DropOptions);
                listDiff.Add(ToSql(), 0, ScriptAction.AddOptions);
            }
            return listDiff;
        }
    }
}
