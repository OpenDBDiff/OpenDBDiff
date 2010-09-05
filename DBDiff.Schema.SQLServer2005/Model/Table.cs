using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Table : SQLServerSchemaBase, IComparable<Table>
    {
        private Columns columns;
        private Constraints constraints;
        private TableOptions options;
        private Triggers triggers;
        private Table originalTable;
        private Indexes indexes;
        private Boolean hasClusteredIndex;
        private int dependenciesCount;
        private string fileGroup;
        private string fileGroupText;
        private List<ISchemaBase> dependencis = null;
        private Dictionary<int, bool> depencyTracker = new Dictionary<int, bool>();

        public Table(Database parent):base(StatusEnum.ObjectTypeEnum.Table)
        {
            dependenciesCount = -1;
            columns = new Columns(this);
            constraints = new Constraints(this);
            options = new TableOptions(this);
            triggers = new Triggers(this);
            indexes = new Indexes(this);
            this.Parent = parent;
        }

        /// <summary>
        /// Clona el objeto Table en una nueva instancia.
        /// </summary>
        public Table Clone(Database objectParent)
        {
            Table table = new Table(objectParent);
            table.Owner = this.Owner;
            table.Name = this.Name;
            table.Id = this.Id;
            table.Guid = this.Guid;
            table.Status = this.Status;
            table.FileGroup = this.FileGroup;
            table.FileGroupText = this.FileGroupText;
            table.HasClusteredIndex = this.HasClusteredIndex;
            table.DependenciesCount = this.DependenciesCount;
            table.Columns = this.Columns.Clone(table);
            table.Options = this.Options.Clone(table);
            table.Triggers = this.Triggers.Clone(table);
            table.Indexes = this.Indexes.Clone(table);
            return table;
        }

        public Indexes Indexes
        {
            get { return indexes; }
            set { indexes = value; }
        }

        public Triggers Triggers
        {
            get { return triggers; }
            set { triggers = value; }
        }

        public TableOptions Options
        {
            get { return options; }
            set { options = value; }
        }

        public string FileGroupText
        {
            get { return fileGroupText; }
            set { fileGroupText = value; }
        }

        /// <summary>
        /// Indica si la tabla tiene algun indice Clustered (de lo contrario, es una tabla Heap)
        /// </summary>
        public Boolean HasClusteredIndex
        {
            get { return hasClusteredIndex; }
            set { hasClusteredIndex = value; }
        }

        /// <summary>
        /// Indica si la tabla tiene alguna columna que sea Identity.
        /// </summary>
        public Boolean HasIdentityColumn
        {
            get 
            {
                foreach (Column col in this.Columns)
                {
                    if (col.IsIdentity) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Objeto tabla con la definicion original de la tabla. Se usa cuando se requiere regenerar la tabla.
        /// </summary>
        public Table OriginalTable
        {
            get { return originalTable; }
            set { originalTable = value; }
        }

        /// <summary>
        /// Colecion de constraints de la tabla.
        /// </summary>
        public Constraints Constraints
        {
            get { return constraints; }
            set { constraints = value; }
        }

        /// <summary>
        /// Coleccion de campos de la tabla.
        /// </summary>
        public Columns Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        /// <summary>
        /// Filegroup donde se encuentra la tabla.
        /// </summary>
        public string FileGroup
        {
            get { return fileGroup; }
            set { fileGroup = value; }
        }

        public Boolean HasBlobColumn
        {
            get
            {
                foreach (Column col in this.Columns)
                {
                    if (col.IsBLOB) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Devuelve el schema de la tabla en formato SQL.
        /// </summary>
        public string ToSQL()
        {
            string sql = "";
            sql += "CREATE TABLE " + FullName + "\r\n(\r\n";
            sql += columns.ToSQL();
            if (constraints.Count > 0)
            {
                sql += ",\r\n";
                sql += constraints.ToSQL();
            }
            else
                sql += "\r\n";
            sql += ")";
            if (!String.IsNullOrEmpty(FileGroup)) sql+= " ON [" + FileGroup + "]";
            if (!String.IsNullOrEmpty(FileGroupText))
            {
                if (this.HasBlobColumn)
                    sql += " TEXTIMAGE_ON [" + FileGroupText + "]";
            }
            sql += "\r\n";
            sql += "GO\r\n";
            sql += constraints.ToSQLCheck();
            sql += indexes.ToSQL();
            sql += options.ToSQL();
            sql += triggers.ToSQL();
            return sql;

        }

        public override string ToSQLAdd()
        {
            return ToSQL();
        }

        public override string ToSQLDrop()
        {
            return "DROP TABLE " + FullName + "\r\nGO\r\n";
        }

        private SQLScriptList BuildSQLFileGroup()
        {
            SQLScriptList listDiff = new SQLScriptList();

            Boolean found = false;
            Index clustered = indexes.Find(Index.IndexTypeEnum.Clustered);
            if (clustered == null)
            {
                foreach (Constraint cons in constraints)
                {
                    if (cons.Index.Type == Index.IndexTypeEnum.Clustered)
                    {
                        listDiff.Add(cons.ToSQLDrop(FileGroup), dependenciesCount, StatusEnum.ScripActionType.DropConstraint);
                        listDiff.Add(cons.ToSQLAdd(), dependenciesCount, StatusEnum.ScripActionType.AddConstraint);
                        found = true;
                    }
                }
                if (!found)
                {
                    this.Status = StatusEnum.ObjectStatusType.AlterRebuildStatus;
                    listDiff = ToSQLDiff();
                }
            }
            else
            {
                listDiff.Add(clustered.ToSQLDrop(FileGroup), dependenciesCount, StatusEnum.ScripActionType.DropIndex);
                listDiff.Add(clustered.ToSQLAdd(), dependenciesCount, StatusEnum.ScripActionType.AddIndex);
            }
            return listDiff;
        }

        /// <summary>
        /// Devuelve el schema de diferencias de la tabla en formato SQL.
        /// </summary>
        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == StatusEnum.ObjectStatusType.DropStatus)
            {
                listDiff.Add(ToSQLDrop(), dependenciesCount, StatusEnum.ScripActionType.DropTable);
                listDiff.Add(ToSQLDropFKBelow());
            }
            if (this.Status == StatusEnum.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(ToSQL(), dependenciesCount, StatusEnum.ScripActionType.AddTable);
            }
            if (this.Status == StatusEnum.ObjectStatusType.AlterRebuildDependeciesStatus)
            {
                GenerateDependencis();
                listDiff.Add(ToSQLDropDependencis());
                listDiff.Add(columns.ToSQLDiff());
                listDiff.Add(ToSQLCreateDependencis());
                listDiff.Add(constraints.ToSQLDiff());
                listDiff.Add(indexes.ToSQLDiff());
                listDiff.Add(options.ToSQLDiff());
                listDiff.Add(triggers.ToSQLDiff());
            }
            if (this.Status == StatusEnum.ObjectStatusType.AlterStatus)
            {
                listDiff.Add(columns.ToSQLDiff());
                listDiff.Add(constraints.ToSQLDiff());
                listDiff.Add(indexes.ToSQLDiff());
                listDiff.Add(options.ToSQLDiff());
                listDiff.Add(triggers.ToSQLDiff());
            }
            if (this.Status == StatusEnum.ObjectStatusType.AlterRebuildStatus)
            {
                GenerateDependencis();
                listDiff.Add(ToSQLRebuild());
                listDiff.Add(columns.ToSQLDiff());
                listDiff.Add(constraints.ToSQLDiff());
                listDiff.Add(indexes.ToSQLDiff());
                listDiff.Add(options.ToSQLDiff());
                //Como recrea la tabla, solo pone los nuevos triggers, por eso va ToSQL y no ToSQLDiff
                listDiff.Add(triggers.ToSQL(), dependenciesCount, StatusEnum.ScripActionType.AddTrigger); 
            }
            return listDiff;
        }

        private string ToSQLTableRebuild()
        {
            string sql = "";
            string tempTable = "Temp" + Name;
            string listColumns = "";
            string listValues = "";
            foreach (Column column in this.Columns)
            {
                if ((column.Status != StatusEnum.ObjectStatusType.DropStatus) && !((column.Status == StatusEnum.ObjectStatusType.CreateStatus) && (column.Nullable == true)))
                {
                    if ((!column.IsComputed) && (!column.Type.ToLower().Equals("timestamp")))
                    {
                        /*Si la nueva columna a agregar es XML, no se inserta ese campo y debe ir a la coleccion de Warnings*/
                        if (!((column.Status == StatusEnum.ObjectStatusType.CreateStatus) && (column.Type.ToLower().Equals("xml"))))
                        {
                            listColumns += "[" + column.Name + "],";
                            if (column.HasToForceValue)
                            {
                                if (column.Status == StatusEnum.ObjectStatusType.AlterStatusUpdate)
                                    listValues += "ISNULL([" + column.Name + "]," + column.DefaultForceValue + "),";
                                else
                                    listValues += column.DefaultForceValue + ",";
                            }
                            else
                                listValues += "[" + column.Name + "],";
                        }
                    }
                }
            }
            listColumns = listColumns.Substring(0, listColumns.Length - 1);
            listValues = listValues.Substring(0, listValues.Length - 1);
            sql += ToSQLTemp(tempTable) + "\r\n";
            if (HasIdentityColumn)
                sql += "SET IDENTITY_INSERT [" + Owner + "].[" + tempTable + "] ON\r\n";
            sql += "INSERT INTO [" + Owner + "].[" + tempTable + "] (" + listColumns + ")" + " SELECT " + listValues + " FROM " + this.FullName + "\r\n";
            if (HasIdentityColumn)
                sql += "SET IDENTITY_INSERT [" + Owner + "].[" + tempTable + "] OFF\r\nGO\r\n\r\n";
            sql += "DROP TABLE " + this.FullName + "\r\nGO\r\n";
            sql += "EXEC sp_rename N'[" + Owner + "].[" + tempTable + "]',N'" + this.Name + "', 'OBJECT'\r\nGO\r\n\r\n";
            sql += OriginalTable.Options.ToSQL();
            return sql;
        }

        private SQLScriptList ToSQLRebuild()
        {
            SQLScriptList listDiff = new SQLScriptList();            
            listDiff.Add(ToSQLDropDependencis());
            listDiff.Add(ToSQLTableRebuild(), dependenciesCount, StatusEnum.ScripActionType.RebuildTable);
            listDiff.Add(ToSQLCreateDependencis());
            return listDiff;
        }

        private string ToSQLTemp(String TableName)
        {
            string sql = "";            
            sql += "CREATE TABLE [" + Owner + "].[" + TableName + "]\r\n(\r\n";
            
            this.Columns.Sort();
            
            for (int index = 0; index < this.Columns.Count; index++)
            {
                if (this.Columns[index].Status != StatusEnum.ObjectStatusType.DropStatus)
                {
                    sql += "\t" + this.Columns[index].ToSQL(true);
                    if (index != this.Columns.Count - 1)
                        sql += ",";
                    sql += "\r\n";
                }
            }
            sql += ")";
            if (!String.IsNullOrEmpty(this.FileGroup)) sql += " ON [" + this.FileGroup + "]";
            if (!String.IsNullOrEmpty(FileGroupText))
            {
                if (this.HasBlobColumn)
                    sql += " TEXTIMAGE_ON [" + FileGroupText + "]";
            }
            sql += "\r\n";
            sql += "GO\r\n";
            return sql;
        }

        private StatusEnum.ScripActionType GetDropActionConstraint(ISchemaBase cons)
        {
            if (cons.ObjectType == StatusEnum.ObjectTypeEnum.Constraint)
            {
                StatusEnum.ScripActionType actionDrop = StatusEnum.ScripActionType.DropConstraint;
                if (((Constraint)cons).Type == Constraint.ConstraintType.ForeignKey)
                {
                    actionDrop = StatusEnum.ScripActionType.DropConstraintFK;
                }
                if (((Constraint)cons).Type == Constraint.ConstraintType.PrimaryKey)
                {
                    actionDrop = StatusEnum.ScripActionType.DropConstraintPK;
                }
                return actionDrop;
            }
            else
                return StatusEnum.ScripActionType.DropIndex;
        }

        private StatusEnum.ScripActionType GetAddActionConstraint(ISchemaBase cons)
        {
            if (cons.ObjectType == StatusEnum.ObjectTypeEnum.Constraint)
            {
                StatusEnum.ScripActionType actionAdd = StatusEnum.ScripActionType.AddConstraint;
                if (((Constraint)cons).Type == Constraint.ConstraintType.ForeignKey)
                {
                    actionAdd = StatusEnum.ScripActionType.AddConstraintFK;
                }
                if (((Constraint)cons).Type == Constraint.ConstraintType.PrimaryKey)
                {
                    actionAdd = StatusEnum.ScripActionType.AddConstraintPK;
                }
                return actionAdd;
            }
            else
                return StatusEnum.ScripActionType.AddIndex;
        }

        private void GenerateDependencis()
        {
            List<ISchemaBase> myDependencis = null;
            /*Si el estado es AlterRebuildDependeciesStatus, busca las dependencias solamente en las columnas que fueron modificadas*/
            if (this.Status == StatusEnum.ObjectStatusType.AlterRebuildDependeciesStatus)
            {
                myDependencis = new List<ISchemaBase>();
                for (int ic = 0; ic < this.Columns.Count; ic++)
                {
                    if ((this.Columns[ic].Status == StatusEnum.ObjectStatusType.AlterRebuildDependeciesStatus) || (this.Columns[ic].Status == StatusEnum.ObjectStatusType.AlterStatus))
                        myDependencis.AddRange(((Database)Parent).Dependencies.Find(this.Id, 0, this.Columns[ic].DataUserTypeId));
                }
                /*Si no encuentra ninguna, toma todas las de la tabla*/
                if (myDependencis.Count == 0)
                    myDependencis.AddRange(((Database)Parent).Dependencies.Find(this.Id));
            }
            else
                myDependencis = ((Database)Parent).Dependencies.Find(this.Id);

            dependencis = new List<ISchemaBase>();
            for (int j = 0; j < myDependencis.Count; j++)
            {
                ISchemaBase item = null;
                if (myDependencis[j].ObjectType == StatusEnum.ObjectTypeEnum.Index)
                    item = indexes[myDependencis[j].FullName];
                if (myDependencis[j].ObjectType == StatusEnum.ObjectTypeEnum.Constraint)
                    item = ((Database)Parent).Tables[myDependencis[j].Parent.FullName].Constraints[myDependencis[j].FullName];
                if (myDependencis[j].ObjectType == StatusEnum.ObjectTypeEnum.Default)
                    item = columns[myDependencis[j].FullName].Constraints[0];
                if (myDependencis[j].ObjectType == StatusEnum.ObjectTypeEnum.View)
                    item = ((Database)Parent).Views[myDependencis[j].FullName];
                if (item != null)
                    dependencis.Add(item);
            }
        }

        private SQLScriptList ToSQLDropFKBelow()
        {
            SQLScriptList listDiff = new SQLScriptList();
            constraints.ForEach (constraint => 
            {
                if ((constraint.Type == Constraint.ConstraintType.ForeignKey) && (((Table)constraint.Parent).DependenciesCount <= this.DependenciesCount))
                {
                    /*Si la FK pertenece a la misma tabla, no se debe explicitar el DROP CONSTRAINT antes de hacer el DROP TABLE*/
                    if (constraint.Parent.Id != constraint.RelationalTableId)
                        listDiff.Add(constraint.ToSQLDrop(), 0, StatusEnum.ScripActionType.DropConstraintFK);
                }
            });
            return listDiff;
        }

        private SQLScriptList ToSQLDropDependencis()
        {
            StatusEnum.ScripActionType action;
            SQLScriptList listDiff = new SQLScriptList();           
            //Se buscan todas las table constraints.
            for (int index = 0; index < dependencis.Count; index++)
            {
                if (dependencis[index].Status == StatusEnum.ObjectStatusType.OriginalStatus)
                {
                    action = GetDropActionConstraint(dependencis[index]);
                    if (!dependencis[index].GetWasInsertInDiffList(action))
                    {
                        listDiff.Add(dependencis[index].ToSQLDrop(), dependenciesCount, action);
                        dependencis[index].SetWasInsertInDiffList(action);
                    }
                }
            }
            //Se buscan todas las columns constraints.
            for (int index = 0; index < columns.Count; index++)
            {
                for (int cindex = 0; cindex < columns[index].Constraints.Count; cindex++)
                {
                    if ((columns[index].Constraints[cindex].Status != StatusEnum.ObjectStatusType.CreateStatus) && ((columns[index].Constraints[cindex].Status == StatusEnum.ObjectStatusType.OriginalStatus) || (this.Status == StatusEnum.ObjectStatusType.AlterRebuildStatus)))
                    //if (columns[index].Constraints[cindex].Status == StatusEnum.ObjectStatusType.OriginalStatus) 
                        listDiff.Add(columns[index].Constraints[cindex].ToSQLDrop(), dependenciesCount, StatusEnum.ScripActionType.DropConstraint);
                }
            }
            return listDiff;
        }

        private SQLScriptList ToSQLCreateDependencis()
        {
            SQLScriptList listDiff = new SQLScriptList();
            StatusEnum.ScripActionType action;
            //Las constraints de deben recorrer en el orden inverso.
            for (int index = dependencis.Count - 1; index >= 0; index--)
            {
                if ((dependencis[index].Status == StatusEnum.ObjectStatusType.OriginalStatus) && (dependencis[index].Parent.Status != StatusEnum.ObjectStatusType.DropStatus))
                {
                    action = GetAddActionConstraint(dependencis[index]);
                    if (!dependencis[index].GetWasInsertInDiffList(action))
                    {
                        listDiff.Add(dependencis[index].ToSQLAdd(), dependenciesCount, GetAddActionConstraint(dependencis[index]));
                        dependencis[index].SetWasInsertInDiffList(action);
                    }
                }
            }
            //Se buscan todas las columns constraints.
            for (int index = columns.Count - 1; index >= 0; index--)
            {
                for (int cindex = 0; cindex < columns[index].Constraints.Count; cindex++)
                {
                    if ((this.Status == StatusEnum.ObjectStatusType.AlterRebuildDependeciesStatus) && (columns[index].Constraints[cindex].Status == StatusEnum.ObjectStatusType.OriginalStatus))
                        listDiff.Add(columns[index].Constraints[cindex].ToSQLAdd(), dependenciesCount, StatusEnum.ScripActionType.AddConstraint);
                }
            }
            return listDiff;
        }

        /// <summary>
        /// Indica la cantidad de Constraints dependientes de otra tabla (FK) que tiene
        /// la tabla.
        /// </summary>
        public int DependenciesCount
        {
            get 
            {
                if (dependenciesCount == -1)
                    dependenciesCount = FindDependenciesCount(this.Id);
                return dependenciesCount; 
            }
            set 
            { 
                dependenciesCount = value; 
            }
        }

        private int FindDependenciesCount(int tableId)
        {
            int count = 0;
            int relationalTableId;
            Constraints constraints = ((Database)Parent).Dependencies.FindNotOwner(tableId);
            for (int index = 0; index < constraints.Count; index++)
            {
                Constraint cons = constraints[index];
                relationalTableId = constraints[index].RelationalTableId; //((Table)constraints[index].Parent).Id;
                if ((cons.Type == Constraint.ConstraintType.ForeignKey) && (relationalTableId == tableId))
                {
                    if (!depencyTracker.ContainsKey(tableId))
                    {
                        depencyTracker.Add(tableId, true);
                        count += 1 + FindDependenciesCount(((Table)cons.Parent).Id);
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Compara en primer orden por la operacion 
        /// (Primero van los Drops, luego los Create y finalesmente los Alter).
        /// Si la operacion es la misma, ordena por cantidad de tablas dependientes.
        /// </summary>
        public int CompareTo(Table other)
        {
            if (other == null) throw new ArgumentNullException("other");
            if (this.Status == other.Status)
                return this.DependenciesCount.CompareTo(other.DependenciesCount);
            else
                return other.Status.CompareTo(this.Status);
        }

        /// <summary>
        /// Compara dos tablas y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean CompareFileGroup(Table origen, Table destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if ((!String.IsNullOrEmpty(destino.FileGroup) && (!String.IsNullOrEmpty(origen.FileGroup))))
                if (!destino.FileGroup.Equals(origen.FileGroup))
                    return false;
            return true;
        }

        /// <summary>
        /// Compara dos tablas y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean CompareFileGroupText(Table origen, Table destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if ((!String.IsNullOrEmpty(destino.FileGroupText) && (!String.IsNullOrEmpty(origen.FileGroupText))))
                if (!destino.FileGroupText.Equals(origen.FileGroupText))
                    return false;
            return true;
        }
    }
}
