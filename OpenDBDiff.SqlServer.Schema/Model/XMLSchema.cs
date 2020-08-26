using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace OpenDBDiff.SqlServer.Schema.Model
{
    public class XMLSchema : SQLServerSchemaBase
    {
        public XMLSchema(ISchemaBase parent)
            : base(parent, ObjectType.XMLSchema)
        {
            this.Dependencies = new List<ObjectDependency>();
        }

        /// <summary>
        /// Clona el objeto en una nueva instancia.
        /// </summary>
        public new XMLSchema Clone(ISchemaBase parent)
        {
            XMLSchema item = new XMLSchema(parent);
            item.Text = this.Text;
            item.Status = this.Status;
            item.Name = this.Name;
            item.Id = this.Id;
            item.Owner = this.Owner;
            item.Guid = this.Guid;
            item.Dependencies = this.Dependencies;
            return item;
        }

        public List<ObjectDependency> Dependencies { get; set; }

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
            if ((this.Status == ObjectStatus.Alter) || (this.Status == ObjectStatus.Rebuild))
            {
                foreach (ObjectDependency dependency in this.Dependencies)
                {
                    ISchemaBase itemDepens = ((Database)this.Parent).Find(dependency.Name);
                    if (dependency.IsCodeType)
                    {
                        list.AddRange(((ICode)itemDepens).Rebuild());
                    }
                    if (dependency.Type == ObjectType.Table)
                    {
                        Column column = ((Table)itemDepens).Columns[dependency.ColumnName];
                        if ((column.Parent.Status != ObjectStatus.Drop) && (column.Parent.Status != ObjectStatus.Create) && ((column.Status != ObjectStatus.Create)))
                        {
                            if (!fields.ContainsKey(column.FullName))
                            {
                                if (column.HasToRebuildOnlyConstraint)
                                    column.Parent.Status = ObjectStatus.RebuildDependencies;
                                list.AddRange(column.RebuildConstraint(true));
                                list.Add("ALTER TABLE " + column.Parent.FullName + " ALTER COLUMN " + column.ToSQLRedefine(null, 0, "") + "\r\nGO\r\n", 0, ScriptAction.AlterColumn);
                                /*Si la columna va a ser eliminada o la tabla va a ser reconstruida, no restaura la columna*/
                                if ((column.Status != ObjectStatus.Drop) && (column.Parent.Status != ObjectStatus.Rebuild))
                                    list.AddRange(column.Alter(ScriptAction.AlterColumnRestore));
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
        public override SQLScriptList ToSqlDiff(System.Collections.Generic.ICollection<ISchemaBase> schemas)
        {
            SQLScriptList list = new SQLScriptList();

            if (this.Status == ObjectStatus.Drop)
            {
                list.Add(ToSqlDrop(), 0, ScriptAction.DropXMLSchema);
            }
            if (this.Status == ObjectStatus.Create)
            {
                list.Add(ToSql(), 0, ScriptAction.AddXMLSchema);
            }
            if (this.Status == ObjectStatus.Alter)
            {
                list.AddRange(ToSQLChangeColumns());
                list.Add(ToSqlDrop() + ToSql(), 0, ScriptAction.AddXMLSchema);
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
