using System;
using System.Globalization;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class Constraint : SQLServerSchemaBase
    {
        public enum ConstraintType
        {
            None = 0,
            PrimaryKey = 1,
            ForeignKey = 2,
            Default = 3,
            Unique = 4,
            Check = 5
        }

        public Constraint(ISchemaBase parent)
            : base(parent, Enums.ObjectType.Constraint)
        {
            this.Columns = new ConstraintColumns(this);
            this.Index = new Index(parent);
        }

        /// <summary>
        /// Clona el objeto Column en una nueva instancia.
        /// </summary>
        public override ISchemaBase Clone(ISchemaBase parent)
        {
            Constraint col = new Constraint(parent);
            col.Id = this.Id;
            col.Name = this.Name;
            col.NotForReplication = this.NotForReplication;
            col.RelationalTableFullName = this.RelationalTableFullName;
            col.Status = this.Status;
            col.Type = this.Type;
            col.WithNoCheck = this.WithNoCheck;
            col.OnDeleteCascade = this.OnDeleteCascade;
            col.OnUpdateCascade = this.OnUpdateCascade;
            col.Owner = this.Owner;
            col.Columns = this.Columns.Clone();
            col.Index = (Index)this.Index.Clone(parent);
            col.IsDisabled = this.IsDisabled;
            col.Definition = this.Definition;
            col.Guid = this.Guid;
            return col;
        }

        /// <summary>
        /// Informacion sobre le indice asociado al Constraint.
        /// </summary>
        public Index Index { get; set; }

        /// <summary>
        /// Coleccion de columnas de la constraint.
        /// </summary>
        public ConstraintColumns Columns { get; set; }

        /// <summary>
        /// Indica si la constraint tiene asociada un indice Clustered.
        /// </summary>
        public Boolean HasClusteredIndex
        {
            get
            {
                if (Index != null)
                    return (Index.Type == Index.IndexTypeEnum.Clustered);
                return false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this constraint is disabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this constraint is disabled; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsDisabled { get; set; }

        /// <summary>
        /// Gets or sets the on delete cascade (only for FK).
        /// </summary>
        /// <value>The on delete cascade.</value>
        public int OnDeleteCascade { get; set; }

        /// <summary>
        /// Gets or sets the on update cascade (only for FK).
        /// </summary>
        /// <value>The on update cascade.</value>
        public int OnUpdateCascade { get; set; }

        /// <summary>
        /// Valor de la constraint (se usa para los Check Constraint).
        /// </summary>
        public string Definition { get; set; }

        /// <summary>
        /// Indica si la constraint va a ser usada en replicacion.
        /// </summary>
        public Boolean NotForReplication { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [with no check].
        /// </summary>
        /// <value><c>true</c> if [with no check]; otherwise, <c>false</c>.</value>
        public Boolean WithNoCheck { get; set; }

        /// <summary>
        /// Indica el tipo de constraint (PrimaryKey, ForeignKey, Unique o Default).
        /// </summary>
        public ConstraintType Type { get; set; }

        /// <summary>
        /// ID de la tabla relacionada a la que hace referencia (solo aplica a FK)
        /// </summary>
        public int RelationalTableId { get; set; }

        /// <summary>
        /// Nombre de la tabla relacionada a la que hace referencia (solo aplica a FK)
        /// </summary>
        public string RelationalTableFullName { get; set; }

        /// <summary>
        /// Compara dos campos y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(Constraint origin, Constraint destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (origin == null) throw new ArgumentNullException("origin");
            if (origin.NotForReplication != destination.NotForReplication) return false;
            if ((origin.RelationalTableFullName == null) && (destination.RelationalTableFullName != null)) return false;
            if (origin.RelationalTableFullName != null)
                if (!origin.RelationalTableFullName.Equals(destination.RelationalTableFullName, StringComparison.CurrentCultureIgnoreCase)) return false;
            if ((origin.Definition == null) && (destination.Definition != null)) return false;
            if (origin.Definition != null)
                if ((!origin.Definition.Equals(destination.Definition)) && (!origin.Definition.Equals("(" + destination.Definition + ")"))) return false;
            /*Solo si la constraint esta habilitada, se chequea el is_trusted*/
            if (!destination.IsDisabled)
                if (origin.WithNoCheck != destination.WithNoCheck) return false;
            if (origin.OnUpdateCascade != destination.OnUpdateCascade) return false;
            if (origin.OnDeleteCascade != destination.OnDeleteCascade) return false;
            if (!ConstraintColumns.Compare(origin.Columns, destination.Columns)) return false;
            if ((origin.Index != null) && (destination.Index != null))
                return Index.Compare(origin.Index, destination.Index);
            return true;
        }

        private string ToSQLGeneric(ConstraintType consType)
        {
            Database database = null;
            ISchemaBase current = this;
            while (database == null && current.Parent != null)
            {
                database = current.Parent as Database;
                current = current.Parent;
            }
            var isAzure10 = database.Info.Version == DatabaseInfo.VersionTypeEnum.SQLServerAzure10;
            string typeConstraint = "";
            StringBuilder sql = new StringBuilder();
            if (Index.Type == Index.IndexTypeEnum.Clustered) typeConstraint = "CLUSTERED";
            if (Index.Type == Index.IndexTypeEnum.Nonclustered) typeConstraint = "NONCLUSTERED";
            if (Index.Type == Index.IndexTypeEnum.XML) typeConstraint = "XML";
            if (Index.Type == Index.IndexTypeEnum.Heap) typeConstraint = "HEAP";
            if (Parent.ObjectType != Enums.ObjectType.TableType)
                sql.Append("CONSTRAINT [" + Name + "] ");
            else
                sql.Append("\t");
            if (consType == ConstraintType.PrimaryKey)
                sql.Append("PRIMARY KEY " + typeConstraint + "\r\n\t(\r\n");
            else
                sql.Append("UNIQUE " + typeConstraint + "\r\n\t(\r\n");

            this.Columns.Sort();

            for (int j = 0; j < this.Columns.Count; j++)
            {
                sql.Append("\t\t[" + this.Columns[j].Name + "]");
                if (this.Columns[j].Order) sql.Append(" DESC"); else sql.Append(" ASC");
                if (j != this.Columns.Count - 1) sql.Append(",");
                sql.Append("\r\n");
            }
            sql.Append("\t)");
            sql.Append(" WITH (");
            if (Parent.ObjectType == Enums.ObjectType.TableType)
                if (Index.IgnoreDupKey) sql.Append("IGNORE_DUP_KEY = ON"); else sql.Append("IGNORE_DUP_KEY  = OFF");
            else
            {
                if (!isAzure10)
                {
                    if (Index.IsPadded) sql.Append("PAD_INDEX = ON, "); else sql.Append("PAD_INDEX  = OFF, ");
                }
                if (Index.IsAutoStatistics) sql.Append("STATISTICS_NORECOMPUTE = ON"); else sql.Append("STATISTICS_NORECOMPUTE  = OFF");
                if (Index.IgnoreDupKey) sql.Append(", IGNORE_DUP_KEY = ON"); else sql.Append(", IGNORE_DUP_KEY  = OFF");
                if (!isAzure10)
                {
                    if (Index.AllowRowLocks) sql.Append(", ALLOW_ROW_LOCKS = ON"); else sql.Append(", ALLOW_ROW_LOCKS  = OFF");
                    if (Index.AllowPageLocks) sql.Append(", ALLOW_PAGE_LOCKS = ON"); else sql.Append(", ALLOW_PAGE_LOCKS  = OFF");
                    if (Index.FillFactor != 0) sql.Append(", FILLFACTOR = " + Index.FillFactor.ToString(CultureInfo.InvariantCulture));
                }
            }
            sql.Append(")");
            if (!isAzure10)
            {
                if (!String.IsNullOrEmpty(Index.FileGroup)) sql.Append(" ON [" + Index.FileGroup + "]");
            }
            return sql.ToString();
        }

        /// <summary>
        /// Devuelve el schema de la tabla en formato SQL.
        /// </summary>
        public override string ToSql()
        {
            if (this.Type == ConstraintType.PrimaryKey)
            {
                return ToSQLGeneric(ConstraintType.PrimaryKey);
            }
            if (this.Type == ConstraintType.ForeignKey)
            {
                StringBuilder sql = new StringBuilder();
                StringBuilder sqlReference = new StringBuilder();
                int indexc = 0;

                this.Columns.Sort();
                sql.Append("CONSTRAINT [" + Name + "] FOREIGN KEY\r\n\t(\r\n");
                foreach (ConstraintColumn column in this.Columns)
                {
                    sql.Append("\t\t[" + column.Name + "]");
                    sqlReference.Append("\t\t[" + column.ColumnRelationalName + "]");
                    if (indexc != this.Columns.Count - 1)
                    {
                        sql.Append(",");
                        sqlReference.Append(",");
                    }
                    sql.Append("\r\n");
                    sqlReference.Append("\r\n");
                    indexc++;
                }
                sql.Append("\t)\r\n");
                sql.Append("\tREFERENCES " + this.RelationalTableFullName + "\r\n\t(\r\n");
                sql.Append(sqlReference + "\t)");
                if (OnUpdateCascade == 1) sql.Append(" ON UPDATE CASCADE");
                if (OnDeleteCascade == 1) sql.Append(" ON DELETE CASCADE");
                if (OnUpdateCascade == 2) sql.Append(" ON UPDATE SET NULL");
                if (OnDeleteCascade == 2) sql.Append(" ON DELETE SET NULL");
                if (OnUpdateCascade == 3) sql.Append(" ON UPDATE SET DEFAULT");
                if (OnDeleteCascade == 3) sql.Append(" ON DELETE SET DEFAULT");
                sql.Append((NotForReplication ? " NOT FOR REPLICATION" : ""));
                return sql.ToString();
            }
            if (this.Type == ConstraintType.Unique)
            {
                return ToSQLGeneric(ConstraintType.Unique);
            }
            if (this.Type == ConstraintType.Check)
            {
                string sqlcheck = "";
                if (Parent.ObjectType != Enums.ObjectType.TableType)
                    sqlcheck = "CONSTRAINT [" + Name + "] ";

                return sqlcheck + "CHECK " + (NotForReplication ? "NOT FOR REPLICATION" : "") + " (" + Definition + ")";
            }
            return "";
        }

        public override string ToSqlAdd()
        {
            return "ALTER TABLE " + Parent.FullName + (WithNoCheck ? " WITH NOCHECK" : "") + " ADD " + ToSql() + "\r\nGO\r\n";
        }

        public override string ToSqlDrop()
        {
            return ToSqlDrop(null);
        }

        public override SQLScript Create()
        {
            Enums.ScripActionType action = Enums.ScripActionType.AddConstraint;
            if (this.Type == ConstraintType.ForeignKey)
                action = Enums.ScripActionType.AddConstraintFK;
            if (this.Type == ConstraintType.PrimaryKey)
                action = Enums.ScripActionType.AddConstraintPK;
            if (!GetWasInsertInDiffList(action))
            {
                SetWasInsertInDiffList(action);
                return new SQLScript(this.ToSqlAdd(), ((Table)Parent).DependenciesCount, action);
            }
            else
                return null;
        }

        public override SQLScript Drop()
        {
            Enums.ScripActionType action = Enums.ScripActionType.DropConstraint;
            if (this.Type == ConstraintType.ForeignKey)
                action = Enums.ScripActionType.DropConstraintFK;
            if (this.Type == ConstraintType.PrimaryKey)
                action = Enums.ScripActionType.DropConstraintPK;
            if (!GetWasInsertInDiffList(action))
            {
                SetWasInsertInDiffList(action);
                return new SQLScript(this.ToSqlDrop(), ((Table)Parent).DependenciesCount, action);
            }
            else
                return null;
        }

        public string ToSqlDrop(string FileGroupName)
        {
            string sql = "ALTER TABLE " + ((Table)Parent).FullName + " DROP CONSTRAINT [" + Name + "]";
            if (!String.IsNullOrEmpty(FileGroupName)) sql += " WITH (MOVE TO [" + FileGroupName + "])";
            sql += "\r\nGO\r\n";
            return sql;
        }

        public string ToSQLEnabledDisabled()
        {
            StringBuilder sql = new StringBuilder();
            if (this.IsDisabled)
                return "ALTER TABLE " + Parent.FullName + " NOCHECK CONSTRAINT [" + Name + "]\r\nGO\r\n";
            else
            {
                return "ALTER TABLE " + Parent.FullName + " CHECK CONSTRAINT [" + Name + "]\r\nGO\r\n";
            }
        }

        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList list = new SQLScriptList();
            if (this.Status != Enums.ObjectStatusType.OriginalStatus)
                RootParent.ActionMessage[Parent.FullName].Add(this);

            if (this.HasState(Enums.ObjectStatusType.DropStatus))
            {
                if (this.Parent.Status != Enums.ObjectStatusType.RebuildStatus)
                    list.Add(Drop());
            }
            if (this.HasState(Enums.ObjectStatusType.CreateStatus))
                list.Add(Create());
            if (this.HasState(Enums.ObjectStatusType.AlterStatus))
            {
                list.Add(Drop());
                list.Add(Create());
            }
            if (this.HasState(Enums.ObjectStatusType.DisabledStatus))
            {
                list.Add(this.ToSQLEnabledDisabled(), ((Table)Parent).DependenciesCount, Enums.ScripActionType.AlterConstraint);
            }
            /*if (this.Status == StatusEnum.ObjectStatusType.ChangeFileGroup)
            {
                list.Add(this.ToSQLDrop(this.Index.FileGroup), ((Table)Parent).DependenciesCount, actionDrop);
                list.Add(this.ToSQLAdd(), ((Table)Parent).DependenciesCount, actionAdd);
            }*/
            return list;
        }
    }
}
