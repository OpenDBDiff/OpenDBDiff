using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Globalization;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class UserDataType : SQLServerSchemaBase
    {
        private string type;
        private int size;
        private Boolean allowNull;
        private int precision;
        private int scale;
        private Default _default;
        private Rule rule;
        private List<ObjectDependency> dependencys;
        private Boolean isAssembly;
        private int assemblyId;
        private string assemblyClass;
        private string assemblyName;

        public UserDataType(ISchemaBase parent)
            : base(parent, Enums.ObjectType.UserDataType)
        {          
            this._default = new Default(this);
            this.rule = new Rule(this);
            this.dependencys = new List<ObjectDependency>();
        }

        /// <summary>
        /// Clona el objeto Column en una nueva instancia.
        /// </summary>
        public UserDataType Clone(ISchemaBase parent)
        {
            UserDataType item = new UserDataType(parent);
            item.Name = this.Name;
            item.Id = this.Id;
            item.Owner = this.Owner;
            item.Guid = this.Guid;
            item.AllowNull = this.AllowNull;
            item.Precision = this.Precision;
            item.Scale = this.Scale;
            item.Size = this.Size;
            item.Type = this.Type;            
            item.Default = this.Default.Clone(this);
            item.Rule = this.Rule.Clone(this);
            item.Dependencys = this.Dependencys;
            item.IsAssembly = this.IsAssembly;
            item.AssemblyClass = this.AssemblyClass;
            item.AssemblyId = this.AssemblyId;
            item.AssemblyName = this.AssemblyName;
            return item;
        }

        public List<ObjectDependency> Dependencys
        {
            get { return dependencys; }
            set { dependencys = value; }
        }

        public Rule Rule
        {
            get { return rule; }
            set { rule = value; }
        }

        public Default Default
        {
            get { return _default; }
            set { _default = value; }
        }

        public string AssemblyName
        {
            get { return assemblyName; }
            set { assemblyName = value; }
        }

        public Boolean IsAssembly
        {
            get { return isAssembly; }
            set { isAssembly = value; }
        }

        public string AssemblyClass
        {
            get { return assemblyClass; }
            set { assemblyClass = value; }
        }

        public int AssemblyId
        {
            get { return assemblyId; }
            set { assemblyId = value; }
        }

        /// <summary>
        /// Cantidad de digitos que permite el campo (solo para campos Numeric).
        /// </summary>
        public int Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// Cantidad de decimales que permite el campo (solo para campos Numeric).
        /// </summary>
        public int Precision
        {
            get { return precision; }
            set { precision = value; }
        }

        public Boolean AllowNull
        {
            get { return allowNull; }
            set { allowNull = value; }
        }

        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public static Boolean CompareRule(UserDataType origen, UserDataType destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if ((origen.Rule.Name != null) && (destino.Rule.Name == null)) return false;
            if ((origen.Rule.Name == null) && (destino.Rule.Name != null)) return false;
            if (origen.Rule.Name != null)
                if (!origen.Rule.Name.Equals(destino.Rule.Name)) return false;
            return true;
        }

        public static Boolean CompareDefault(UserDataType origen, UserDataType destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if ((origen.Default.Name != null) && (destino.Default.Name == null)) return false;
            if ((origen.Default.Name == null) && (destino.Default.Name != null)) return false;
            if (origen.Default.Name != null)
                if (!origen.Default.Name.Equals(destino.Default.Name)) return false;
            return true;
        }

        public override string ToSql()
        {
            string sql = "CREATE TYPE " + FullName;
            if (!IsAssembly)
            {
                sql += " FROM [" + type + "]";
                if (Type.Equals("binary") || Type.Equals("varbinary") || Type.Equals("varchar") || Type.Equals("char") || Type.Equals("nchar") || Type.Equals("nvarchar")) sql += "(" + Size.ToString(CultureInfo.InvariantCulture) + ")";
                if (Type.Equals("numeric") || Type.Equals("decimal")) sql += " (" + Precision.ToString(CultureInfo.InvariantCulture) + "," + Scale.ToString(CultureInfo.InvariantCulture) + ")";
                if (AllowNull)
                    sql += " NULL";
                else
                    sql += " NOT NULL";
            }
            else
            {
                sql += " EXTERNAL NAME [" + AssemblyName + "].[" + AssemblyClass + "]";
            }
            sql += "\r\nGO\r\n";
            return sql + ToSQLAddBinds();
        }

        public override string ToSqlDrop()
        {
            return "DROP TYPE " + FullName + "\r\nGO\r\n";
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        private string ToSQLAddBinds()
        {
            string sql = "";
            if (!String.IsNullOrEmpty(Default.Name))
                sql += Default.ToSQLAddBind();
            if (!String.IsNullOrEmpty(Rule.Name))
                sql += Rule.ToSQLAddBind();
            return sql;
        }

        private SQLScriptList RebuildDependencys(Table table)
        {
            SQLScriptList list = new SQLScriptList();
            List<ISchemaBase> items = ((Database)table.Parent).Dependencies.Find(table.Id);
            items.ForEach(item =>
            {
                ISchemaBase realItem = ((Database)table.Parent).Find(item.FullName);
                if (realItem.IsCodeType)
                    list.AddRange(((ICode)realItem).Rebuild());
            });
            return list;
        }

        private SQLScriptList  ToSQLChangeColumns()
        {
            Hashtable fields = new Hashtable();
            SQLScriptList list = new SQLScriptList();
            SQLScriptList listDependencys = new SQLScriptList();
            if ((this.Status == Enums.ObjectStatusType.AlterStatus) || (this.Status == Enums.ObjectStatusType.RebuildStatus))
            {
                foreach (ObjectDependency dependency in this.Dependencys)
                {
                    ISchemaBase itemDepens = ((Database)this.Parent).Find(dependency.Name);
                    if (dependency.IsCodeType)
                    {
                        if (itemDepens != null)
                            list.AddRange(((ICode)itemDepens).Rebuild());
                    }
                    if (dependency.Type == Enums.ObjectType.Table)
                    {                        
                        Column column = ((Table)itemDepens).Columns[dependency.ColumnName];
                        if ((column.Parent.Status != Enums.ObjectStatusType.DropStatus) && (column.Parent.Status != Enums.ObjectStatusType.CreateStatus) && ((column.Status != Enums.ObjectStatusType.CreateStatus) || (column.IsComputed)))
                        {
                            if (!fields.ContainsKey(column.FullName))
                            {
                                listDependencys.AddRange(RebuildDependencys((Table)itemDepens));
                                if (column.HasToRebuildOnlyConstraint)
                                    //column.Parent.Status = Enums.ObjectStatusType.AlterRebuildDependenciesStatus;
                                    list.AddRange(column.RebuildDependencies());
                                if (!column.IsComputed)
                                {
                                    list.AddRange(column.RebuildConstraint(true));
                                    list.Add("ALTER TABLE " + column.Parent.FullName + " ALTER COLUMN " + column.ToSQLRedefine(this.Type, this.Size, null) + "\r\nGO\r\n", 0, Enums.ScripActionType.AlterColumn);
                                    /*Si la columna va a ser eliminada o la tabla va a ser reconstruida, no restaura la columna*/
                                    if ((column.Status != Enums.ObjectStatusType.DropStatus) && (column.Parent.Status != Enums.ObjectStatusType.RebuildStatus))
                                        list.AddRange(column.Alter(Enums.ScripActionType.AlterColumnRestore));
                                }
                                else
                                {
                                    if (column.Status != Enums.ObjectStatusType.CreateStatus)
                                    {
                                        if (!column.GetWasInsertInDiffList(Enums.ScripActionType.AlterColumnFormula))
                                        {
                                            column.SetWasInsertInDiffList(Enums.ScripActionType.AlterColumnFormula);
                                            list.Add(column.ToSqlDrop(), 0, Enums.ScripActionType.AlterColumnFormula);
                                            List<ISchemaBase> drops = ((Database)column.Parent.Parent).Dependencies.Find(column.Parent.Id, column.Id, 0);
                                            drops.ForEach(item =>
                                            {
                                                if (item.Status != Enums.ObjectStatusType.CreateStatus) list.Add(item.Drop());
                                                if (item.Status != Enums.ObjectStatusType.DropStatus) list.Add(item.Create());
                                            });
                                            /*Si la columna va a ser eliminada o la tabla va a ser reconstruida, no restaura la columna*/
                                            if ((column.Status != Enums.ObjectStatusType.DropStatus) && (column.Parent.Status != Enums.ObjectStatusType.RebuildStatus))
                                                list.Add(column.ToSqlAdd(), 0, Enums.ScripActionType.AlterColumnFormulaRestore);
                                        }
                                    }
                                }
                                fields.Add(column.FullName, column.FullName);
                            }
                        }
                    }
                }
            }
            list.AddRange(listDependencys);
            return list;
        }
        
        public String AssemblyFullName
        {
            get
            {
                if (this.IsAssembly)
                    return this.AssemblyName + "." + this.AssemblyClass;
                return "";
            }
        }

        private Boolean HasAnotherUDTClass()
        {
            if (this.IsAssembly)
            {
                /*Si existe otro UDT con el mismo assembly que se va a crear, debe ser borrado ANTES de crearse el nuevo*/
                UserDataType other = ((Database)this.Parent).UserTypes.Find(item => (item.Status == Enums.ObjectStatusType.DropStatus) && (item.assemblyName + "." + item.AssemblyClass).Equals((this.assemblyName + "." + this.AssemblyClass)));
                if (other != null)
                    return true;
            }
            return false;
        }

        private string SQLDropOlder()
        {
            UserDataType other = ((Database)this.Parent).UserTypes.Find(item => (item.Status == Enums.ObjectStatusType.DropStatus) && (item.assemblyName + "." + item.AssemblyClass).Equals((this.assemblyName + "." + this.AssemblyClass)));
            return other.ToSqlDrop();
        }

        public override SQLScript Create()
        {
            Enums.ScripActionType action = Enums.ScripActionType.AddUserDataType;
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
            Enums.ScripActionType action = Enums.ScripActionType.DropUserDataType;
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
                    if (!HasAnotherUDTClass())
                        list.Add(Drop());
                }
                if (this.HasState(Enums.ObjectStatusType.CreateStatus))
                {
                    list.Add(Create());
                }
                if (this.Status == Enums.ObjectStatusType.AlterStatus)
                {
                    if (this.Default.Status == Enums.ObjectStatusType.CreateStatus)
                        list.Add(this.Default.ToSQLAddBind(), 0, Enums.ScripActionType.AddUserDataType);
                    if (this.Default.Status == Enums.ObjectStatusType.DropStatus)
                        list.Add(this.Default.ToSQLAddUnBind(), 0, Enums.ScripActionType.UnbindRuleType);
                    if (this.Rule.Status == Enums.ObjectStatusType.CreateStatus)
                        list.Add(this.Rule.ToSQLAddBind(), 0, Enums.ScripActionType.AddUserDataType);
                    if (this.Rule.Status == Enums.ObjectStatusType.DropStatus)
                        list.Add(this.Rule.ToSQLAddUnBind(), 0, Enums.ScripActionType.UnbindRuleType);
                }
                if (this.Status == Enums.ObjectStatusType.RebuildStatus)
                {
                    list.AddRange(ToSQLChangeColumns());
                    if (!this.GetWasInsertInDiffList(Enums.ScripActionType.DropUserDataType))
                    {
                        list.Add(ToSqlDrop() + ToSql(), 0, Enums.ScripActionType.AddUserDataType);
                    }
                    else
                        list.Add(Create());
                }
                if (this.HasState(Enums.ObjectStatusType.DropOlderStatus))
                {
                    list.Add(this.SQLDropOlder(), 0, Enums.ScripActionType.AddUserDataType);
                }
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool Compare(UserDataType obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            if (this.Scale != obj.Scale) return false;
            if (this.Precision != obj.Precision) return false;
            if (this.AllowNull != obj.AllowNull) return false;
            if (this.Size != obj.Size) return false;
            if (!this.Type.Equals(obj.Type)) return false;
            if (this.IsAssembly != obj.IsAssembly) return false;
            if (!this.AssemblyClass.Equals(obj.AssemblyClass)) return false;
            if (!this.AssemblyName.Equals(obj.AssemblyName)) return false;
            if (!CompareDefault(this, obj)) return false;
            if (!CompareRule(this, obj)) return false;
            return true;
        }
    }
}
