using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
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

        private string definition;
        private ConstraintType type;
        private ConstraintColumns columns;
        private string relationalTable;
        private int relationalTableId;
        private Boolean withNoCheck;
        private Boolean notForReplication;
        private int onUpdateCascade;
        private int onDeleteCascade;
        private Index index;
        private Boolean isDisabled;

        public Constraint(ISchemaBase parent)
            : base(parent,Enums.ObjectType.Constraint)
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
        public Index Index
        {
            get { return index; }
            set { index = value; }
        }

        /// <summary>
        /// Coleccion de columnas de la constraint.
        /// </summary>
        public ConstraintColumns Columns
        {
            get { return columns; }
            set { columns = value; }
        }

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
        public Boolean IsDisabled
        {
            get { return isDisabled; }
            set { isDisabled = value; }
        }

        /// <summary>
        /// Gets or sets the on delete cascade (only for FK).
        /// </summary>
        /// <value>The on delete cascade.</value>
        public int OnDeleteCascade
        {
            get { return onDeleteCascade; }
            set { onDeleteCascade = value; }
        }

        /// <summary>
        /// Gets or sets the on update cascade (only for FK).
        /// </summary>
        /// <value>The on update cascade.</value>
        public int OnUpdateCascade
        {
            get { return onUpdateCascade; }
            set { onUpdateCascade = value; }
        }
        
        /// <summary>
        /// Valor de la constraint (se usa para los Check Constraint).
        /// </summary>
        public string Definition
        {
            get { return definition; }
            set { definition = value; }
        }

        /// <summary>
        /// Indica si la constraint va a ser usada en replicacion.
        /// </summary>
        public Boolean NotForReplication
        {
            get { return notForReplication; }
            set { notForReplication = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [with no check].
        /// </summary>
        /// <value><c>true</c> if [with no check]; otherwise, <c>false</c>.</value>
        public Boolean WithNoCheck
        {
            get { return withNoCheck; }
            set { withNoCheck = value; }
        }

        /// <summary>
        /// Indica el tipo de constraint (PrimaryKey, ForeignKey, Unique o Default).
        /// </summary>
        public ConstraintType Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// ID de la tabla relacionada a la que hace referencia (solo aplica a FK)
        /// </summary>
        public int RelationalTableId
        {
            get { return relationalTableId; }
            set { relationalTableId = value; }
        }

        /// <summary>
        /// Nombre de la tabla relacionada a la que hace referencia (solo aplica a FK)
        /// </summary>
        public string RelationalTableFullName
        {
            get { return relationalTable; }
            set { relationalTable = value; }
        }       

        /// <summary>
        /// Compara dos campos y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(Constraint origen, Constraint destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (origen.NotForReplication != destino.NotForReplication) return false;
            if ((origen.RelationalTableFullName == null) && (destino.RelationalTableFullName != null)) return false;            
            if (origen.RelationalTableFullName != null)
                if (!origen.RelationalTableFullName.Equals(destino.RelationalTableFullName, StringComparison.CurrentCultureIgnoreCase)) return false;
            if ((origen.Definition == null) && (destino.Definition != null)) return false;
            if (origen.Definition != null)
                if ((!origen.Definition.Equals(destino.Definition)) && (!origen.Definition.Equals("(" + destino.Definition + ")"))) return false;
            /*Solo si la constraint esta habilitada, se chequea el is_trusted*/
            if (!destino.IsDisabled)
                if (origen.WithNoCheck != destino.WithNoCheck) return false;
            if (origen.OnUpdateCascade != destino.OnUpdateCascade) return false;
            if (origen.OnDeleteCascade != destino.OnDeleteCascade) return false;
            if (!ConstraintColumns.Compare(origen.Columns, destino.Columns)) return false;
            if ((origen.Index != null) && (destino.Index != null))
                return Index.Compare(origen.Index, destino.Index);
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
            var isDenali = database.Info.Version == DatabaseInfo.VersionTypeEnum.SQLServerDenali;
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
                if (!isDenali)
                {
                    if (Index.IsPadded) sql.Append("PAD_INDEX = ON, "); else sql.Append("PAD_INDEX  = OFF, ");
                }
                if (Index.IsAutoStatistics) sql.Append("STATISTICS_NORECOMPUTE = ON"); else sql.Append("STATISTICS_NORECOMPUTE  = OFF");
                if (Index.IgnoreDupKey) sql.Append(", IGNORE_DUP_KEY = ON"); else sql.Append(", IGNORE_DUP_KEY  = OFF");
                if (!isDenali)
                {
                    if (Index.AllowRowLocks) sql.Append(", ALLOW_ROW_LOCKS = ON"); else sql.Append(", ALLOW_ROW_LOCKS  = OFF");
                    if (Index.AllowPageLocks) sql.Append(", ALLOW_PAGE_LOCKS = ON"); else sql.Append(", ALLOW_PAGE_LOCKS  = OFF");
                    if (Index.FillFactor != 0) sql.Append(", FILLFACTOR = " + Index.FillFactor.ToString(CultureInfo.InvariantCulture));
                }
            }
            sql.Append(")");
            if (!isDenali)
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
            if (this.Type == Constraint.ConstraintType.ForeignKey)
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
                if (onUpdateCascade == 1) sql.Append(" ON UPDATE CASCADE");
                if (onDeleteCascade == 1) sql.Append(" ON DELETE CASCADE");
                if (onUpdateCascade == 2) sql.Append(" ON UPDATE SET NULL");
                if (onDeleteCascade == 2) sql.Append(" ON DELETE SET NULL");
                if (onUpdateCascade == 3) sql.Append(" ON UPDATE SET DEFAULT");
                if (onDeleteCascade == 3) sql.Append(" ON DELETE SET DEFAULT");
                sql.Append((NotForReplication ? " NOT FOR REPLICATION" : ""));
                return sql.ToString();
            }
            if (this.Type == Constraint.ConstraintType.Unique)
            {
                return ToSQLGeneric(ConstraintType.Unique);
            }
            if (this.Type == Constraint.ConstraintType.Check)
            {
                string sqlcheck = "";
                if (Parent.ObjectType != Enums.ObjectType.TableType)
                    sqlcheck = "CONSTRAINT [" + Name + "] ";

                return sqlcheck + "CHECK " + (NotForReplication ? "NOT FOR REPLICATION" : "") + " (" + definition + ")";
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
            if (this.Type == Constraint.ConstraintType.ForeignKey)
                action = Enums.ScripActionType.AddConstraintFK;
            if (this.Type == Constraint.ConstraintType.PrimaryKey)
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
            if (this.Type == Constraint.ConstraintType.ForeignKey)
                action = Enums.ScripActionType.DropConstraintFK;
            if (this.Type == Constraint.ConstraintType.PrimaryKey)
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
