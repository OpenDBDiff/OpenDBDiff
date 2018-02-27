using System;
using OpenDBDiff.Schema.Model;

namespace OpenDBDiff.Schema.SQLServer.Generates.Model
{
    public class TableType : SQLServerSchemaBase, ITable<TableType>
    {
        public TableType(Database parent)
            : base(parent, ObjectType.TableType)
        {
            Columns = new Columns<TableType>(this);
            Constraints = new SchemaList<Constraint, TableType>(this, parent.AllObjects);
            Indexes = new SchemaList<Index, TableType>(this, parent.AllObjects);
        }

        public Columns<TableType> Columns { get; private set; }

        public SchemaList<Constraint, TableType> Constraints { get; private set; }

        public SchemaList<Index, TableType> Indexes { get; private set; }

        public override string ToSql()
        {
            string sql = "";
            if (Columns.Count > 0)
            {
                sql += "CREATE TYPE " + FullName + " AS TABLE\r\n(\r\n";
                sql += Columns.ToSql() + "\r\n";
                sql += Constraints.ToSql();
                sql += ")";
                sql += "\r\nGO\r\n";
            }
            return sql;
        }

        public override string ToSqlDrop()
        {
            return "DROP TYPE " + FullName + "\r\nGO\r\n";
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        public override SQLScript Create()
        {
            ScriptAction action = ScriptAction.AddTableType;
            if (!GetWasInsertInDiffList(action))
            {
                SetWasInsertInDiffList(action);
                return new SQLScript(this.ToSqlAdd(), 0, action);
            }
            else
                return null;
        }

        public override SQLScript Drop()
        {
            ScriptAction action = ScriptAction.DropTableType;
            if (!GetWasInsertInDiffList(action))
            {
                SetWasInsertInDiffList(action);
                return new SQLScript(this.ToSqlDrop(), 0, action);
            }
            else
                return null;
        }

        public override SQLScriptList ToSqlDiff(System.Collections.Generic.ICollection<ISchemaBase> schemas)
        {
            try
            {
                SQLScriptList list = new SQLScriptList();
                if (this.Status == ObjectStatus.Drop)
                {
                    list.Add(Drop());
                }
                if (this.HasState(ObjectStatus.Create))
                {
                    list.Add(Create());
                }
                if (this.Status == ObjectStatus.Alter)
                {
                    list.Add(ToSqlDrop() + ToSql(), 0, ScriptAction.AddTableType);
                }
                return list;
            }
            catch
            {
                return null;
            }
        }
    }
}
