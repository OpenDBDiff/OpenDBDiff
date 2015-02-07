using System;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class TableType:SQLServerSchemaBase, ITable<TableType>
    {
        private Columns<TableType> columns;
        private SchemaList<Constraint, TableType> constraints;
        private SchemaList<Index, TableType> indexes;

        public TableType(Database parent)
            : base(parent, Enums.ObjectType.TableType)
        {
            columns = new Columns<TableType>(this);
            constraints = new SchemaList<Constraint, TableType>(this, parent.AllObjects);
            indexes = new SchemaList<Index, TableType>(this, parent.AllObjects);
        }

        public Columns<TableType> Columns
        {
            get { return columns; }
        }

        public SchemaList<Constraint, TableType> Constraints
        {
            get { return constraints; }
        }

        public SchemaList<Index, TableType> Indexes
        {
            get { return indexes; }
        }

        public override string ToSql()
        {
            string sql = "";
            if (columns.Count > 0)
            {
                sql += "CREATE TYPE " + FullName + " AS TABLE\r\n(\r\n";
                sql += columns.ToSql() + "\r\n";
                sql += constraints.ToSql();
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
            Enums.ScripActionType action = Enums.ScripActionType.AddTableType;
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
            Enums.ScripActionType action = Enums.ScripActionType.DropTableType;
            if (!GetWasInsertInDiffList(action))
            {
                SetWasInsertInDiffList(action);
                return new SQLScript(this.ToSqlDrop(), 0, action);
            }
            else
                return null;
        }

        public override SQLScriptList ToSqlDiff()
        {
            try
            {
                SQLScriptList list = new SQLScriptList();
                if (this.Status == Enums.ObjectStatusType.DropStatus)
                {
                    list.Add(Drop());
                }
                if (this.HasState(Enums.ObjectStatusType.CreateStatus))
                {
                    list.Add(Create());
                }
                if (this.Status == Enums.ObjectStatusType.AlterStatus)
                {
                    list.Add(ToSqlDrop() + ToSql(), 0, Enums.ScripActionType.AddTableType);
                }
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
