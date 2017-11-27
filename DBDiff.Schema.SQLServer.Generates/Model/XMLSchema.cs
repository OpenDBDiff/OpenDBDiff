using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class XMLSchema : SQLServerSchemaBase
    {
        public XMLSchema(ISchemaBase parent)
            : base(parent, Enums.ObjectType.XMLSchema)
        {
            this.Dependencys = new List<ObjectDependency>();
        }

        /// <summary>
        /// Clona el objeto en una nueva instancia.
        /// </summary>
        public XMLSchema Clone(ISchemaBase parent)
        {
            XMLSchema item = new XMLSchema(parent);
            item.Text = this.Text;
            item.Status = this.Status;
            item.Name = this.Name;
            item.Id = this.Id;
            item.Owner = this.Owner;
            item.Guid = this.Guid;
            item.Dependencys = this.Dependencys;
            return item;
        }

        public List<ObjectDependency> Dependencys { get; set; }

        public string Text { get; set; }

        public override string ToSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("CREATE XML SCHEMA COLLECTION ");
            sql.Append(this.FullName + " AS ");
            sql.Append("N'" + this.Text + "'");
            sql.Append("\r\nGO\r\n");
            return sql.ToString();
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        public override string ToSqlDrop()
        {
            return "DROP XML SCHEMA COLLECTION " + FullName + "\r\nGO\r\n";
        }

        private SQLScriptList ToSQLChangeColumns()
        {
            Hashtable fields = new Hashtable();
            SQLScriptList list = new SQLScriptList();
            if ((this.Status == Enums.ObjectStatusType.AlterStatus) || (this.Status == Enums.ObjectStatusType.RebuildStatus))
            {
                foreach (ObjectDependency dependency in this.Dependencys)
                {
                    ISchemaBase itemDepens = ((Database)this.Parent).Find(dependency.Name);
                    if (dependency.IsCodeType)
                    {
                        list.AddRange(((ICode)itemDepens).Rebuild());
                    }
                    if (dependency.Type == Enums.ObjectType.Table)
                    {
                        Column column = ((Table)itemDepens).Columns[dependency.ColumnName];
                        if ((column.Parent.Status != Enums.ObjectStatusType.DropStatus) && (column.Parent.Status != Enums.ObjectStatusType.CreateStatus) && ((column.Status != Enums.ObjectStatusType.CreateStatus)))
                        {
                            if (!fields.ContainsKey(column.FullName))
                            {
                                if (column.HasToRebuildOnlyConstraint)
                                    column.Parent.Status = Enums.ObjectStatusType.RebuildDependenciesStatus;
                                list.AddRange(column.RebuildConstraint(true));
                                list.Add("ALTER TABLE " + column.Parent.FullName + " ALTER COLUMN " + column.ToSQLRedefine(null, 0, "") + "\r\nGO\r\n", 0, Enums.ScripActionType.AlterColumn);
                                /*Si la columna va a ser eliminada o la tabla va a ser reconstruida, no restaura la columna*/
                                if ((column.Status != Enums.ObjectStatusType.DropStatus) && (column.Parent.Status != Enums.ObjectStatusType.RebuildStatus))
                                    list.AddRange(column.Alter(Enums.ScripActionType.AlterColumnRestore));
                                fields.Add(column.FullName, column.FullName);
                            }
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Devuelve el schema de diferencias del Schema en formato SQL.
        /// </summary>
        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList list = new SQLScriptList();

            if (this.Status == Enums.ObjectStatusType.DropStatus)
            {
                list.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropXMLSchema);
            }
            if (this.Status == Enums.ObjectStatusType.CreateStatus)
            {
                list.Add(ToSql(), 0, Enums.ScripActionType.AddXMLSchema);
            }
            if (this.Status == Enums.ObjectStatusType.AlterStatus)
            {
                list.AddRange(ToSQLChangeColumns());
                list.Add(ToSqlDrop() + ToSql(), 0, Enums.ScripActionType.AddXMLSchema);
            }
            return list;
        }

        public bool Compare(XMLSchema obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            if (!this.Text.Equals(obj.Text)) return false;
            return true;
        }
    }
}
