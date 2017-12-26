using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class UserDataType : SQLServerSchemaBase
    {
        public UserDataType(ISchemaBase parent)
            : base(parent, Enums.ObjectType.UserDataType)
        {
            Default = new Default(this);
            Rule = new Rule(this);
            Dependencys = new List<ObjectDependency>();
        }

        public List<ObjectDependency> Dependencys { get; private set; }

        public Rule Rule { get; private set; }

        public Default Default { get; private set; }

        public string AssemblyName { get; set; }

        public Boolean IsAssembly { get; set; }

        public string AssemblyClass { get; set; }

        public int AssemblyId { get; set; }

        /// <summary>
        /// Cantidad de digitos que permite el campo (solo para campos Numeric).
        /// </summary>
        public int Scale { get; set; }

        /// <summary>
        /// Cantidad de decimales que permite el campo (solo para campos Numeric).
        /// </summary>
        public int Precision { get; set; }

        public Boolean AllowNull { get; set; }

        public int Size { get; set; }

        public string Type { get; set; }

        public String AssemblyFullName
        {
            get
            {
                if (IsAssembly)
                    return AssemblyName + "." + AssemblyClass;
                return "";
            }
        }

        /// <summary>
        /// Clona el objeto Column en una nueva instancia.
        /// </summary>
        public override ISchemaBase Clone(ISchemaBase parent)
        {
            var item = new UserDataType(parent)
            {
                Name = Name,
                Id = Id,
                Owner = Owner,
                Guid = Guid,
                AllowNull = AllowNull,
                Precision = Precision,
                Scale = Scale,
                Size = Size,
                Type = Type,
                Default = Default.Clone(this),
                Rule = Rule.Clone(this),
                Dependencys = Dependencys,
                IsAssembly = IsAssembly,
                AssemblyClass = AssemblyClass,
                AssemblyId = AssemblyId,
                AssemblyName = AssemblyName
            };
            return item;
        }

        public static Boolean CompareRule(UserDataType origin, UserDataType destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (origin == null) throw new ArgumentNullException("origin");
            if ((origin.Rule.Name != null) && (destination.Rule.Name == null)) return false;
            if ((origin.Rule.Name == null) && (destination.Rule.Name != null)) return false;
            if (origin.Rule.Name != null)
                if (!origin.Rule.Name.Equals(destination.Rule.Name)) return false;
            return true;
        }

        public static Boolean CompareDefault(UserDataType origin, UserDataType destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (origin == null) throw new ArgumentNullException("origin");
            if ((origin.Default.Name != null) && (destination.Default.Name == null)) return false;
            if ((origin.Default.Name == null) && (destination.Default.Name != null)) return false;
            if (origin.Default.Name != null)
                if (!origin.Default.Name.Equals(destination.Default.Name)) return false;
            return true;
        }

        public override string ToSql()
        {
            string sql = "CREATE TYPE " + FullName;
            if (!IsAssembly)
            {
                sql += " FROM [" + Type + "]";
                if (Type.Equals("binary") || Type.Equals("varbinary") || Type.Equals("varchar") || Type.Equals("char") ||
                    Type.Equals("nchar") || Type.Equals("nvarchar"))
                    sql += "(" + Size.ToString(CultureInfo.InvariantCulture) + ")";
                if (Type.Equals("numeric") || Type.Equals("decimal"))
                    sql += " (" + Precision.ToString(CultureInfo.InvariantCulture) + "," +
                           Scale.ToString(CultureInfo.InvariantCulture) + ")";
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
            var list = new SQLScriptList();
            List<ISchemaBase> items = ((Database)table.Parent).Dependencies.Find(table.Id);
            items.ForEach(item =>
                              {
                                  ISchemaBase realItem = ((Database)table.Parent).Find(item.FullName);
                                  if (realItem.IsCodeType)
                                      list.AddRange(((ICode)realItem).Rebuild());
                              });
            return list;
        }

        private SQLScriptList ToSQLChangeColumns()
        {
            var fields = new Hashtable();
            var list = new SQLScriptList();
            var listDependencys = new SQLScriptList();
            if ((Status == Enums.ObjectStatusType.AlterStatus) || (Status == Enums.ObjectStatusType.RebuildStatus))
            {
                foreach (ObjectDependency dependency in Dependencys)
                {
                    ISchemaBase itemDepens = ((Database)Parent).Find(dependency.Name);
                    /*Si la dependencia es una funcion o una vista, reconstruye el objecto*/
                    if (dependency.IsCodeType)
                    {
                        if (itemDepens != null)
                            list.AddRange(((ICode)itemDepens).Rebuild());
                    }
                    /*Si la dependencia es una tabla, reconstruye los indices, constraint y columnas asociadas*/
                    if (dependency.Type == Enums.ObjectType.Table)
                    {
                        Column column = ((Table)itemDepens).Columns[dependency.ColumnName];
                        if ((column.Parent.Status != Enums.ObjectStatusType.DropStatus) &&
                            (column.Parent.Status != Enums.ObjectStatusType.CreateStatus) &&
                            ((column.Status != Enums.ObjectStatusType.CreateStatus) || (column.IsComputed)))
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
                                    list.Add(
                                        "ALTER TABLE " + column.Parent.FullName + " ALTER COLUMN " +
                                        column.ToSQLRedefine(Type, Size, null) + "\r\nGO\r\n", 0,
                                        Enums.ScripActionType.AlterColumn);
                                    /*Si la columna va a ser eliminada o la tabla va a ser reconstruida, no restaura la columna*/
                                    if ((column.Status != Enums.ObjectStatusType.DropStatus) &&
                                        (column.Parent.Status != Enums.ObjectStatusType.RebuildStatus))
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
                                            List<ISchemaBase> drops =
                                                ((Database)column.Parent.Parent).Dependencies.Find(column.Parent.Id,
                                                                                                    column.Id, 0);
                                            drops.ForEach(item =>
                                                              {
                                                                  if (item.Status != Enums.ObjectStatusType.CreateStatus)
                                                                      list.Add(item.Drop());
                                                                  if (item.Status != Enums.ObjectStatusType.DropStatus)
                                                                      list.Add(item.Create());
                                                              });
                                            /*Si la columna va a ser eliminada o la tabla va a ser reconstruida, no restaura la columna*/
                                            if ((column.Status != Enums.ObjectStatusType.DropStatus) &&
                                                (column.Parent.Status != Enums.ObjectStatusType.RebuildStatus))
                                                list.Add(column.ToSqlAdd(), 0,
                                                         Enums.ScripActionType.AlterColumnFormulaRestore);
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

        private Boolean HasAnotherUDTClass()
        {
            if (IsAssembly)
            {
                /*Si existe otro UDT con el mismo assembly que se va a crear, debe ser borrado ANTES de crearse el nuevo*/
                UserDataType other =
                    ((Database)Parent).UserTypes.Find(
                        item =>
                        (item.Status == Enums.ObjectStatusType.DropStatus) &&
                        (item.AssemblyName + "." + item.AssemblyClass).Equals((AssemblyName + "." + AssemblyClass)));
                if (other != null)
                    return true;
            }
            return false;
        }

        private string SQLDropOlder()
        {
            UserDataType other =
                ((Database)Parent).UserTypes.Find(
                    item =>
                    (item.Status == Enums.ObjectStatusType.DropStatus) &&
                    (item.AssemblyName + "." + item.AssemblyClass).Equals((AssemblyName + "." + AssemblyClass)));
            return other.ToSqlDrop();
        }

        public override SQLScript Create()
        {
            Enums.ScripActionType action = Enums.ScripActionType.AddUserDataType;
            if (!GetWasInsertInDiffList(action))
            {
                SetWasInsertInDiffList(action);
                return new SQLScript(ToSqlAdd(), 0, action);
            }
            else
                return null;
        }

        public override SQLScript Drop()
        {
            const Enums.ScripActionType action = Enums.ScripActionType.DropUserDataType;
            if (!GetWasInsertInDiffList(action))
            {
                SetWasInsertInDiffList(action);
                return new SQLScript(ToSqlDrop(), 0, action);
            }
            return null;
        }

        public override SQLScriptList ToSqlDiff()
        {
            try
            {
                var list = new SQLScriptList();
                if (Status == Enums.ObjectStatusType.DropStatus)
                {
                    if (!HasAnotherUDTClass())
                        list.Add(Drop());
                }
                if (HasState(Enums.ObjectStatusType.CreateStatus))
                {
                    list.Add(Create());
                }
                if (Status == Enums.ObjectStatusType.AlterStatus)
                {
                    if (Default.Status == Enums.ObjectStatusType.CreateStatus)
                        list.Add(Default.ToSQLAddBind(), 0, Enums.ScripActionType.AddUserDataType);
                    if (Default.Status == Enums.ObjectStatusType.DropStatus)
                        list.Add(Default.ToSQLAddUnBind(), 0, Enums.ScripActionType.UnbindRuleType);
                    if (Rule.Status == Enums.ObjectStatusType.CreateStatus)
                        list.Add(Rule.ToSQLAddBind(), 0, Enums.ScripActionType.AddUserDataType);
                    if (Rule.Status == Enums.ObjectStatusType.DropStatus)
                        list.Add(Rule.ToSQLAddUnBind(), 0, Enums.ScripActionType.UnbindRuleType);
                }
                if (Status == Enums.ObjectStatusType.RebuildStatus)
                {
                    list.AddRange(ToSQLChangeColumns());
                    if (!GetWasInsertInDiffList(Enums.ScripActionType.DropUserDataType))
                    {
                        list.Add(ToSqlDrop() + ToSql(), 0, Enums.ScripActionType.AddUserDataType);
                    }
                    else
                        list.Add(Create());
                }
                if (HasState(Enums.ObjectStatusType.DropOlderStatus))
                {
                    list.Add(SQLDropOlder(), 0, Enums.ScripActionType.AddUserDataType);
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
            if (Scale != obj.Scale) return false;
            if (Precision != obj.Precision) return false;
            if (AllowNull != obj.AllowNull) return false;
            if (Size != obj.Size) return false;
            if (!Type.Equals(obj.Type)) return false;
            if (IsAssembly != obj.IsAssembly) return false;
            if (!AssemblyClass.Equals(obj.AssemblyClass)) return false;
            if (!AssemblyName.Equals(obj.AssemblyName)) return false;
            if (!CompareDefault(this, obj)) return false;
            if (!CompareRule(this, obj)) return false;
            return true;
        }
    }
}
