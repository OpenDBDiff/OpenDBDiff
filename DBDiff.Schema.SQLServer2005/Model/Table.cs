using System;
using System.Linq;
using System.Collections.Generic;
using DBDiff.Schema.Attributes;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class Table : SQLServerSchemaBase, IComparable<Table>, ITable<Table>
    {
        private int dependenciesCount;
        private List<ISchemaBase> dependencis;
        private Boolean? hasFileStream;

        public Table(ISchemaBase parent)
            : base(parent, Enums.ObjectType.Table)
        {
            dependenciesCount = -1;
            Columns = new Columns<Table>(this);
            Constraints = new SchemaList<Constraint, Table>(this, ((Database)parent).AllObjects);
            Options = new SchemaList<TableOption, Table>(this);
            Triggers = new SchemaList<Trigger, Table>(this, ((Database)parent).AllObjects);
            CLRTriggers = new SchemaList<CLRTrigger, Table>(this, ((Database)parent).AllObjects);
            Indexes = new SchemaList<Index, Table>(this, ((Database)parent).AllObjects);
            Partitions = new SchemaList<TablePartition, Table>(this, ((Database)parent).AllObjects);
            FullTextIndex = new SchemaList<FullTextIndex, Table>(this);
        }

        public string CompressType { get; set; }

        public string FileGroupText { get; set; }

        public Boolean HasChangeDataCapture { get; set; }

        public Boolean HasChangeTrackingTrackColumn { get; set; }

        public Boolean HasChangeTracking { get; set; }

        public string FileGroupStream { get; set; }

        public Boolean HasClusteredIndex { get; set; }

        public string FileGroup { get; set; }

        public Table OriginalTable { get; set; }

        [ShowItem("Constraints")]
        public SchemaList<Constraint, Table> Constraints { get; private set; }

        [ShowItem("Indexes", "Index")]
        public SchemaList<Index, Table> Indexes { get; private set; }

        [ShowItem("CLR Triggers")]
        public SchemaList<CLRTrigger, Table> CLRTriggers { get; private set; }

        [ShowItem("Triggers")]
        public SchemaList<Trigger, Table> Triggers { get; private set; }

        public SchemaList<FullTextIndex, Table> FullTextIndex { get; private set; }

        public SchemaList<TablePartition, Table> Partitions { get; set; }

        public SchemaList<TableOption, Table> Options { get; set; }

        /// <summary>
        /// Indica si la tabla tiene alguna columna que sea Identity.
        /// </summary>
        public Boolean HasIdentityColumn
        {
            get
            {
                foreach (Column col in Columns)
                {
                    if (col.IsIdentity) return true;
                }
                return false;
            }
        }

        public Boolean HasFileStream
        {
            get
            {
                if (hasFileStream == null)
                {
                    hasFileStream = false;
                    foreach (Column col in Columns)
                    {
                        if (col.IsFileStream) hasFileStream = true;
                    }
                }
                return hasFileStream.Value;
            }
        }

        public Boolean HasBlobColumn
        {
            get
            {
                foreach (Column col in Columns)
                {
                    if (col.IsBLOB) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Indica la cantidad de Constraints dependientes de otra tabla (FK) que tiene
        /// la tabla.
        /// </summary>
        public override int DependenciesCount
        {
            get
            {
                if (dependenciesCount == -1)
                    dependenciesCount = ((Database)Parent).Dependencies.DependenciesCount(Id,
                                                                                           Enums.ObjectType.Constraint);
                return dependenciesCount;
            }
        }

        #region IComparable<Table> Members

        /// <summary>
        /// Compara en primer orden por la operacion
        /// (Primero van los Drops, luego los Create y finalesmente los Alter).
        /// Si la operacion es la misma, ordena por cantidad de tablas dependientes.
        /// </summary>
        public int CompareTo(Table other)
        {
            if (other == null) throw new ArgumentNullException("other");
            if (Status == other.Status)
                return DependenciesCount.CompareTo(other.DependenciesCount);
            return other.Status.CompareTo(Status);
        }

        #endregion

        #region ITable<Table> Members

        /// <summary>
        /// Coleccion de campos de la tabla.
        /// </summary>
        [ShowItem("Columns", "Column")]
        public Columns<Table> Columns { get; set; }

        #endregion

        /// <summary>
        /// Clona el objeto Table en una nueva instancia.
        /// </summary>
        public override ISchemaBase Clone(ISchemaBase objectParent)
        {
            var table = new Table(objectParent)
            {
                Owner = Owner,
                Name = Name,
                Id = Id,
                Guid = Guid,
                Status = Status,
                FileGroup = FileGroup,
                FileGroupText = FileGroupText,
                FileGroupStream = FileGroupStream,
                HasClusteredIndex = HasClusteredIndex,
                HasChangeTracking = HasChangeTracking,
                HasChangeTrackingTrackColumn = HasChangeTrackingTrackColumn,
                HasChangeDataCapture = HasChangeDataCapture,
                dependenciesCount = DependenciesCount
            };
            table.Columns = Columns.Clone(table);
            table.Options = Options.Clone(table);
            table.CompressType = CompressType;
            table.Triggers = Triggers.Clone(table);
            table.Indexes = Indexes.Clone(table);
            table.Partitions = Partitions.Clone(table);
            table.Constraints = Constraints.Clone(table);
            return table;
        }

        public override string ToSql()
        {
            return ToSql(true);
        }

        /// <summary>
        /// Devuelve el schema de la tabla en formato SQL.
        /// </summary>
        public string ToSql(Boolean showFK)
        {
            Database database = null;
            ISchemaBase current = this;
            while (database == null && current.Parent != null)
            {
                database = current.Parent as Database;
                current = current.Parent;
            }
            var isAzure10 = database.Info.Version == DatabaseInfo.VersionTypeEnum.SQLServerAzure10;

            string sql = "";
            string sqlPK = "";
            string sqlUC = "";
            string sqlFK = "";
            if (Columns.Count > 0)
            {
                sql += "CREATE TABLE " + FullName + "\r\n(\r\n";
                sql += Columns.ToSql();
                if (Constraints.Count > 0)
                {
                    sql += ",\r\n";
                    Constraints.AsQueryable()
                        // Add the constraint if it's not in DropStatus
                        .Where(c => !c.HasState(Enums.ObjectStatusType.DropStatus))
                        .ToList()
                        .ForEach(item =>
                        {
                            if (item.Type == Constraint.ConstraintType.PrimaryKey)
                                sqlPK += "\t" + item.ToSql() + ",\r\n";
                            if (item.Type == Constraint.ConstraintType.Unique)
                                sqlUC += "\t" + item.ToSql() + ",\r\n";
                            if (showFK)
                                if (item.Type == Constraint.ConstraintType.ForeignKey)
                                    sqlFK += "\t" + item.ToSql() + ",\r\n";
                        });
                    sql += sqlPK + sqlUC + sqlFK;
                    sql = sql.Substring(0, sql.Length - 3) + "\r\n";
                }
                else
                {
                    sql += "\r\n";
                    if (!String.IsNullOrEmpty(CompressType))
                        sql += "WITH (DATA_COMPRESSION = " + CompressType + ")\r\n";
                }
                sql += ")";

                if (!isAzure10)
                {
                    if (!String.IsNullOrEmpty(FileGroup)) sql += " ON [" + FileGroup + "]";

                    if (!String.IsNullOrEmpty(FileGroupText))
                    {
                        if (HasBlobColumn)
                            sql += " TEXTIMAGE_ON [" + FileGroupText + "]";
                    }
                    if ((!String.IsNullOrEmpty(FileGroupStream)) && (HasFileStream))
                        sql += " FILESTREAM_ON [" + FileGroupStream + "]";
                }
                sql += "\r\n";
                sql += "GO\r\n";
                Constraints.ForEach(item =>
                                        {
                                            if (item.Type == Constraint.ConstraintType.Check)
                                                sql += item.ToSqlAdd() + "\r\n";
                                        });
                if (HasChangeTracking)
                    sql += ToSqlChangeTracking();
                sql += Indexes.ToSql();
                sql += FullTextIndex.ToSql();
                sql += Options.ToSql();
                sql += Triggers.ToSql();
            }
            return sql;
        }

        private string ToSqlChangeTracking()
        {
            string sql;
            if (HasChangeTracking)
            {
                sql = "ALTER TABLE " + FullName + " ENABLE CHANGE_TRACKING";
                if (HasChangeTrackingTrackColumn)
                    sql += " WITH(TRACK_COLUMNS_UPDATED = ON)";
            }
            else
                sql = "ALTER TABLE " + FullName + " DISABLE CHANGE_TRACKING";

            return sql + "\r\nGO\r\n";
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        public override string ToSqlDrop()
        {
            return "DROP TABLE " + FullName + "\r\nGO\r\n";
        }

        /*
                private SQLScriptList BuildSQLFileGroup()
                {
                    var listDiff = new SQLScriptList();

                    Boolean found = false;
                    Index clustered = Indexes.Find(item => item.Type == Index.IndexTypeEnum.Clustered);
                    if (clustered == null)
                    {
                        foreach (Constraint cons in Constraints)
                        {
                            if (cons.Index.Type == Index.IndexTypeEnum.Clustered)
                            {
                                listDiff.Add(cons.ToSqlDrop(FileGroup), dependenciesCount, Enums.ScripActionType.DropConstraint);
                                listDiff.Add(cons.ToSqlAdd(), dependenciesCount, Enums.ScripActionType.AddConstraint);
                                found = true;
                            }
                        }
                        if (!found)
                        {
                            Status = Enums.ObjectStatusType.RebuildStatus;
                            listDiff = ToSqlDiff();
                        }
                    }
                    else
                    {
                        listDiff.Add(clustered.ToSqlDrop(FileGroup), dependenciesCount, Enums.ScripActionType.DropIndex);
                        listDiff.Add(clustered.ToSqlAdd(), dependenciesCount, Enums.ScripActionType.AddIndex);
                    }
                    return listDiff;
                }
        */

        /// <summary>
        /// Devuelve el schema de diferencias de la tabla en formato SQL.
        /// </summary>
        public override SQLScriptList ToSqlDiff()
        {
            var listDiff = new SQLScriptList();

            if (Status != Enums.ObjectStatusType.OriginalStatus)
            {
                if (((Database)Parent).Options.Ignore.FilterTable)
                    RootParent.ActionMessage.Add(this);
            }

            if (Status == Enums.ObjectStatusType.DropStatus)
            {
                if (((Database)Parent).Options.Ignore.FilterTable)
                {
                    listDiff.Add(ToSqlDrop(), dependenciesCount, Enums.ScripActionType.DropTable);
                    listDiff.AddRange(ToSQLDropFKBelow());
                }
            }
            if (Status == Enums.ObjectStatusType.CreateStatus)
            {
                string sql = "";
                Constraints.ForEach(item =>
                                        {
                                            if (item.Type == Constraint.ConstraintType.ForeignKey)
                                                sql += item.ToSqlAdd() + "\r\n";
                                        });
                listDiff.Add(ToSql(false), dependenciesCount, Enums.ScripActionType.AddTable);
                listDiff.Add(sql, dependenciesCount, Enums.ScripActionType.AddConstraintFK);
            }
            if (HasState(Enums.ObjectStatusType.RebuildDependenciesStatus))
            {
                GenerateDependencis();
                listDiff.AddRange(ToSQLDropDependencis());
                listDiff.AddRange(Columns.ToSqlDiff());
                listDiff.AddRange(ToSQLCreateDependencis());
                listDiff.AddRange(Constraints.ToSqlDiff());
                listDiff.AddRange(Indexes.ToSqlDiff());
                listDiff.AddRange(Options.ToSqlDiff());
                listDiff.AddRange(Triggers.ToSqlDiff());
                listDiff.AddRange(CLRTriggers.ToSqlDiff());
                listDiff.AddRange(FullTextIndex.ToSqlDiff());
            }
            if (HasState(Enums.ObjectStatusType.AlterStatus))
            {
                listDiff.AddRange(Columns.ToSqlDiff());
                listDiff.AddRange(Constraints.ToSqlDiff());
                listDiff.AddRange(Indexes.ToSqlDiff());
                listDiff.AddRange(Options.ToSqlDiff());
                listDiff.AddRange(Triggers.ToSqlDiff());
                listDiff.AddRange(CLRTriggers.ToSqlDiff());
                listDiff.AddRange(FullTextIndex.ToSqlDiff());
            }
            if (HasState(Enums.ObjectStatusType.RebuildStatus))
            {
                GenerateDependencis();
                listDiff.AddRange(ToSQLRebuild());
                listDiff.AddRange(Columns.ToSqlDiff());
                listDiff.AddRange(Constraints.ToSqlDiff());
                listDiff.AddRange(Indexes.ToSqlDiff());
                listDiff.AddRange(Options.ToSqlDiff());
                //Como recrea la tabla, solo pone los nuevos triggers, por eso va ToSQL y no ToSQLDiff
                listDiff.Add(Triggers.ToSql(), dependenciesCount, Enums.ScripActionType.AddTrigger);
                listDiff.Add(CLRTriggers.ToSql(), dependenciesCount, Enums.ScripActionType.AddTrigger);
                listDiff.AddRange(FullTextIndex.ToSqlDiff());
            }
            if (HasState(Enums.ObjectStatusType.DisabledStatus))
            {
                listDiff.Add(ToSqlChangeTracking(), 0, Enums.ScripActionType.AlterTableChangeTracking);
            }
            return listDiff;
        }

        private string ToSQLTableRebuild()
        {
            string sql = "";
            string tempTable = "Temp" + Name;
            string listColumns = "";
            string listValues = "";
            Boolean IsIdentityNew = false;
            foreach (Column column in Columns)
            {
                if ((column.Status != Enums.ObjectStatusType.DropStatus) &&
                    !((column.Status == Enums.ObjectStatusType.CreateStatus) && column.IsNullable))
                {
                    if ((!column.IsComputed) && (!column.Type.ToLower().Equals("timestamp")))
                    {
                        /*Si la nueva columna a agregar es XML, no se inserta ese campo y debe ir a la coleccion de Warnings*/
                        /*Si la nueva columna a agregar es Identity, tampoco se debe insertar explicitamente*/
                        if (
                            !((column.Status == Enums.ObjectStatusType.CreateStatus) &&
                              ((column.Type.ToLower().Equals("xml") || (column.IsIdentity)))))
                        {
                            listColumns += "[" + column.Name + "],";
                            if (column.HasToForceValue)
                            {
                                if (column.HasState(Enums.ObjectStatusType.UpdateStatus))
                                    listValues += "ISNULL([" + column.Name + "]," + column.DefaultForceValue + "),";
                                else
                                    listValues += column.DefaultForceValue + ",";
                            }
                            else
                                listValues += "[" + column.Name + "],";
                        }
                        else
                        {
                            if (column.IsIdentity) IsIdentityNew = true;
                        }
                    }
                }
            }
            if (!String.IsNullOrEmpty(listColumns))
            {
                listColumns = listColumns.Substring(0, listColumns.Length - 1);
                listValues = listValues.Substring(0, listValues.Length - 1);
                sql += ToSQLTemp(tempTable) + "\r\n";
                if ((HasIdentityColumn) && (!IsIdentityNew))
                    sql += "SET IDENTITY_INSERT [" + Owner + "].[" + tempTable + "] ON\r\n";
                sql += "INSERT INTO [" + Owner + "].[" + tempTable + "] (" + listColumns + ")" + " SELECT " +
                       listValues + " FROM " + FullName + "\r\n";
                if ((HasIdentityColumn) && (!IsIdentityNew))
                    sql += "SET IDENTITY_INSERT [" + Owner + "].[" + tempTable + "] OFF\r\nGO\r\n\r\n";
                sql += "DROP TABLE " + FullName + "\r\nGO\r\n";

                if (HasFileStream)
                {
                    Constraints.ForEach(item =>
                                            {
                                                if ((item.Type == Constraint.ConstraintType.Unique) &&
                                                    (item.Status != Enums.ObjectStatusType.DropStatus))
                                                {
                                                    sql += "EXEC sp_rename N'[" + Owner + "].[Temp_XX_" + item.Name +
                                                           "]',N'" + item.Name + "', 'OBJECT'\r\nGO\r\n";
                                                }
                                            });
                }
                sql += "EXEC sp_rename N'[" + Owner + "].[" + tempTable + "]',N'" + Name +
                       "', 'OBJECT'\r\nGO\r\n\r\n";
                sql += OriginalTable.Options.ToSql();
            }
            else
                sql = "";
            return sql;
        }

        private SQLScriptList ToSQLRebuild()
        {
            var listDiff = new SQLScriptList();
            listDiff.AddRange(ToSQLDropDependencis());
            listDiff.Add(ToSQLTableRebuild(), dependenciesCount, Enums.ScripActionType.RebuildTable);
            listDiff.AddRange(ToSQLCreateDependencis());
            return listDiff;
        }

        private string ToSQLTemp(String TableName)
        {
            string sql = "";
            sql += "CREATE TABLE [" + Owner + "].[" + TableName + "]\r\n(\r\n";

            Columns.Sort();

            for (int index = 0; index < Columns.Count; index++)
            {
                if (Columns[index].Status != Enums.ObjectStatusType.DropStatus)
                {
                    sql += "\t" + Columns[index].ToSql(true);
                    if (index != Columns.Count - 1)
                        sql += ",";
                    sql += "\r\n";
                }
            }
            if (HasFileStream)
            {
                sql = sql.Substring(0, sql.Length - 2);
                sql += ",\r\n";
                Constraints.ForEach(item =>
                                        {
                                            if ((item.Type == Constraint.ConstraintType.Unique) &&
                                                (item.Status != Enums.ObjectStatusType.DropStatus))
                                            {
                                                item.Name = "Temp_XX_" + item.Name;
                                                sql += "\t" + item.ToSql() + ",\r\n";
                                                item.SetWasInsertInDiffList(Enums.ScripActionType.AddConstraint);
                                                item.Name = item.Name.Substring(8, item.Name.Length - 8);
                                            }
                                        });
                sql = sql.Substring(0, sql.Length - 3) + "\r\n";
            }
            else
            {
                sql += "\r\n";
                if (!String.IsNullOrEmpty(CompressType))
                    sql += "WITH (DATA_COMPRESSION = " + CompressType + ")\r\n";
            }
            sql += ")";
            if (!String.IsNullOrEmpty(FileGroup)) sql += " ON [" + FileGroup + "]";
            if (!String.IsNullOrEmpty(FileGroupText))
            {
                if (HasBlobColumn)
                    sql += " TEXTIMAGE_ON [" + FileGroupText + "]";
            }
            if ((!String.IsNullOrEmpty(FileGroupStream)) && (HasFileStream))
                sql += " FILESTREAM_ON [" + FileGroupStream + "]";

            sql += "\r\n";
            sql += "GO\r\n";
            return sql;
        }

        private void GenerateDependencis()
        {
            List<ISchemaBase> myDependencis;
            /*Si el estado es AlterRebuildDependeciesStatus, busca las dependencias solamente en las columnas que fueron modificadas*/
            if (Status == Enums.ObjectStatusType.RebuildDependenciesStatus)
            {
                myDependencis = new List<ISchemaBase>();
                for (int ic = 0; ic < Columns.Count; ic++)
                {
                    if ((Columns[ic].Status == Enums.ObjectStatusType.RebuildDependenciesStatus) ||
                        (Columns[ic].Status == Enums.ObjectStatusType.AlterStatus))
                        myDependencis.AddRange(((Database)Parent).Dependencies.Find(Id, 0, Columns[ic].DataUserTypeId));
                }
                /*Si no encuentra ninguna, toma todas las de la tabla*/
                if (myDependencis.Count == 0)
                    myDependencis.AddRange(((Database)Parent).Dependencies.Find(Id));
            }
            else
                myDependencis = ((Database)Parent).Dependencies.Find(Id);

            dependencis = new List<ISchemaBase>();
            for (int j = 0; j < myDependencis.Count; j++)
            {
                ISchemaBase item = null;
                if (myDependencis[j].ObjectType == Enums.ObjectType.Index)
                    item = Indexes[myDependencis[j].FullName];
                if (myDependencis[j].ObjectType == Enums.ObjectType.Constraint)
                    item =
                        ((Database)Parent).Tables[myDependencis[j].Parent.FullName].Constraints[
                            myDependencis[j].FullName];
                if (myDependencis[j].ObjectType == Enums.ObjectType.Default)
                    item = Columns[myDependencis[j].FullName].DefaultConstraint;
                if (myDependencis[j].ObjectType == Enums.ObjectType.View)
                    item = ((Database)Parent).Views[myDependencis[j].FullName];
                if (myDependencis[j].ObjectType == Enums.ObjectType.Function)
                    item = ((Database)Parent).Functions[myDependencis[j].FullName];
                if (item != null)
                    dependencis.Add(item);
            }
        }

        /// <summary>
        /// Genera una lista de FK que deben ser eliminadas previamente a la eliminacion de la tablas.
        /// Esto pasa porque para poder eliminar una tabla, hay que eliminar antes todas las constraints asociadas.
        /// </summary>
        private SQLScriptList ToSQLDropFKBelow()
        {
            var listDiff = new SQLScriptList();
            Constraints.ForEach(constraint =>
                                    {
                                        if ((constraint.Type == Constraint.ConstraintType.ForeignKey) &&
                                            (((Table)constraint.Parent).DependenciesCount <= DependenciesCount))
                                        {
                                            /*Si la FK pertenece a la misma tabla, no se debe explicitar el DROP CONSTRAINT antes de hacer el DROP TABLE*/
                                            if (constraint.Parent.Id != constraint.RelationalTableId)
                                            {
                                                listDiff.Add(constraint.Drop());
                                            }
                                        }
                                    });
            return listDiff;
        }

        /// <summary>
        /// Genera una lista de script de DROPS de todas los constraints dependientes de la tabla.
        /// Se usa cuando se quiere reconstruir una tabla y todos sus objectos dependientes.
        /// </summary>
        private SQLScriptList ToSQLDropDependencis()
        {
            bool addDependencie = true;
            var listDiff = new SQLScriptList();
            //Se buscan todas las table constraints.
            for (int index = 0; index < dependencis.Count; index++)
            {
                if ((dependencis[index].Status == Enums.ObjectStatusType.OriginalStatus) ||
                    (dependencis[index].Status == Enums.ObjectStatusType.DropStatus))
                {
                    addDependencie = true;
                    if (dependencis[index].ObjectType == Enums.ObjectType.Constraint)
                    {
                        if ((((Constraint)dependencis[index]).Type == Constraint.ConstraintType.Unique) &&
                            ((HasFileStream) || (OriginalTable.HasFileStream)))
                            addDependencie = false;
                        if ((((Constraint)dependencis[index]).Type != Constraint.ConstraintType.ForeignKey) &&
                            (dependencis[index].Status == Enums.ObjectStatusType.DropStatus))
                            addDependencie = false;
                    }
                    if (addDependencie)
                        listDiff.Add(dependencis[index].Drop());
                }
            }
            //Se buscan todas las columns constraints.
            Columns.ForEach(column =>
                                {
                                    if (column.DefaultConstraint != null)
                                    {
                                        if (((column.DefaultConstraint.Status == Enums.ObjectStatusType.OriginalStatus) ||
                                             (column.DefaultConstraint.Status == Enums.ObjectStatusType.DropStatus) ||
                                             (column.DefaultConstraint.Status == Enums.ObjectStatusType.AlterStatus)) &&
                                            (column.Status != Enums.ObjectStatusType.CreateStatus))
                                            listDiff.Add(column.DefaultConstraint.Drop());
                                    }
                                });
            return listDiff;
        }

        private SQLScriptList ToSQLCreateDependencis()
        {
            bool addDependencie = true;
            var listDiff = new SQLScriptList();
            //Las constraints de deben recorrer en el orden inverso.
            for (int index = dependencis.Count - 1; index >= 0; index--)
            {
                if ((dependencis[index].Status == Enums.ObjectStatusType.OriginalStatus) &&
                    (dependencis[index].Parent.Status != Enums.ObjectStatusType.DropStatus))
                {
                    addDependencie = true;
                    if (dependencis[index].ObjectType == Enums.ObjectType.Constraint)
                    {
                        if ((((Constraint)dependencis[index]).Type == Constraint.ConstraintType.Unique) &&
                            (HasFileStream))
                            addDependencie = false;
                    }
                    if (addDependencie)
                        listDiff.Add(dependencis[index].Create());
                }
            }
            //Se buscan todas las columns constraints.
            for (int index = Columns.Count - 1; index >= 0; index--)
            {
                if (Columns[index].DefaultConstraint != null)
                {
                    if ((Columns[index].DefaultConstraint.CanCreate) &&
                        (Columns.Parent.Status != Enums.ObjectStatusType.RebuildStatus))
                        listDiff.Add(Columns[index].DefaultConstraint.Create());
                }
            }
            return listDiff;
        }

        /// <summary>
        /// Compara dos tablas y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean CompareFileGroup(Table origin, Table destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (origin == null) throw new ArgumentNullException("origin");
            if ((!String.IsNullOrEmpty(destination.FileGroup) && (!String.IsNullOrEmpty(origin.FileGroup))))
                if (!destination.FileGroup.Equals(origin.FileGroup))
                    return false;
            return true;
        }

        /// <summary>
        /// Compara dos tablas y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean CompareFileGroupText(Table origin, Table destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (origin == null) throw new ArgumentNullException("origin");
            if ((!String.IsNullOrEmpty(destination.FileGroupText) && (!String.IsNullOrEmpty(origin.FileGroupText))))
                if (!destination.FileGroupText.Equals(origin.FileGroupText))
                    return false;
            return true;
        }
    }
}
