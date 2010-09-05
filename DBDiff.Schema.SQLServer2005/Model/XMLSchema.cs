using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;
using System.Collections;

namespace DBDiff.Schema.SQLServer.Model
{
    public class XMLSchema : SQLServerSchemaBase
    {
        private string text;
        private List<ObjectDependency> dependencys;

        public XMLSchema(ISchemaBase parent)
            : base(Enums.ObjectType.XMLSchema)
        {
            this.Parent = parent;
            this.dependencys = new List<ObjectDependency>();
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

        public List<ObjectDependency> Dependencys
        {
            get { return dependencys; }
            set { dependencys = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public override string ToSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("CREATE XML SCHEMA COLLECTION ");
            sql.Append(this.FullName + " AS ");
            sql.Append("N'"+this.Text+"'");
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

        /// <summary>
        /// Compara dos store procedures y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(XMLSchema origen, XMLSchema destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (!origen.Text.Equals(destino.Text)) return false;
            return true;
        }

        private SQLScriptList ToSQLChangeColumns()
        {
            Hashtable fields = new Hashtable();
            SQLScriptList list = new SQLScriptList();
            if ((this.Status == Enums.ObjectStatusType.AlterStatus) || (this.Status == Enums.ObjectStatusType.AlterRebuildStatus))
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
                                    column.Parent.Status = Enums.ObjectStatusType.AlterRebuildDependenciesStatus;
                                list.AddRange(column.RebuildConstraint(true));
                                list.Add("ALTER TABLE " + column.Parent.FullName + " ALTER COLUMN " + column.ToSQLRedefine(null,0, "") + "\r\nGO\r\n", 0, Enums.ScripActionType.AlterColumn);
                                /*Si la columna va a ser eliminada o la tabla va a ser reconstruida, no restaura la columna*/
                                if ((column.Status != Enums.ObjectStatusType.DropStatus) && (column.Parent.Status != Enums.ObjectStatusType.AlterRebuildStatus))
                                    list.AddRange(column.Alter( Enums.ScripActionType.AlterColumnRestore));
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
        public SQLScriptList ToSQLDiff()
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
                list.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropXMLSchema);
                list.Add(ToSql(), 0, Enums.ScripActionType.AddXMLSchema);
            }
            return list;
        }
    }
}
