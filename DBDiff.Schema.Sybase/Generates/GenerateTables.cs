using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Data.SqlClient;
using DBDiff.Schema.Events;
using DBDiff.Schema.Sybase.Model;
using DBDiff.Schema.Sybase.Options;
using Sybase.Data.AseClient;

namespace DBDiff.Schema.Sybase.Generates
{
    public class GenerateTables
    {        
        private string connectioString;
        private AseOption objectFilter;
        public event Progress.ProgressHandler OnTableProgress;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="connectioString">Connection string de la base</param>
        public GenerateTables(string connectioString, AseOption filter)
        {
            this.connectioString = connectioString;
            this.objectFilter = filter;
        }

        private string GetSQLColumns()
        {
            string sql = "";
            sql += "select ('[' + U.name + '].[' + O.name + ']') AS TableName, SC.name AS Name, ";
            sql += "ST.name AS Type, ";
            sql += "SC.length AS Size,  ";
            sql += "ISNULL(SC.prec,0) AS Precision,  ";
            sql += "ISNULL(SC.scale,0) AS Scale, ";
            sql += "colid, ";
            sql += "CASE WHEN (status & 8) = 8 THEN 1 ELSE 0 END AS IsNullable, ";
            sql += "CASE WHEN (status & 128) = 128 THEN 1 ELSE 0 END AS IsIdentity ";
            sql += "from syscolumns SC ";
            sql += "INNER JOIN sysobjects O ON O.id = SC.id ";
            sql += "INNER JOIN sysusers U ON U.uid = O.uid ";
            sql += "INNER JOIN systypes ST ON ST.usertype = SC.usertype ";
            sql += "WHERE O.type = 'U' ";
            sql += "ORDER BY TableName, colid";
            return sql;
        }

        private void SetColumnsDatabase(Tables tables)
        {
            using (AseConnection conn = new AseConnection(connectioString))
            {
                using (AseCommand command = new AseCommand(GetSQLColumns(), conn))
                {
                    conn.Open();
                    using (AseDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Table table = tables[reader["TableName"].ToString()];
                            Column col = new Column(table);
                            col.Id = (short)reader["colid"];
                            col.Identity = ((int)reader["IsIdentity"]) == 1;
                            col.Name = reader["Name"].ToString();
                            col.Nullable = ((int)reader["IsNullable"]) == 1;
                            col.Precision = (byte)reader["Precision"];
                            col.Scale = (byte)reader["Scale"];
                            if (!String.IsNullOrEmpty(reader["Size"].ToString()))
                                col.Size = (int)reader["Size"];
                            col.Type = reader["Type"].ToString();
                            col.HasIndexDependencies = false;
                            if (objectFilter.OptionFilter.FilterColumnPosition)
                                col.Position = table.Columns.Count + 1;
                            else
                                col.Position = 0;
                            table.Columns.Add(col);
                        }
                    }
                }
            }
        }

        private int GetTablesCount()
        {
            using (AseConnection conn = new AseConnection(connectioString))
            {
                conn.Open();
                using (AseCommand command = new AseCommand("select Count(*) from sysobjects WHERE type = 'U'", conn))
                {
                    return (int)command.ExecuteScalar();
                }
            }
        }

        private static string GetSQLTrigger()
        {
            string sql = "";
            sql += "select ('[' + U.name + '].[' + OBJECT_NAME(O.id) + ']') AS TableName, T1.id, T1.name, U.name as owner ";
            sql += "from sysobjects O ";
            sql += "INNER JOIN sysobjects T1 ON T1.id = O.deltrig AND O.type = 'U' ";
            sql += "INNER JOIN sysusers U ON U.uid = T1.uid ";
            sql += " UNION ";
            sql += "select ('[' + U.name + '].[' + OBJECT_NAME(O.id) + ']') AS TableName, T1.id, T1.name, U.name as owner ";
            sql += "from sysobjects O ";
            sql += "INNER JOIN sysobjects T1 ON T1.id = O.instrig AND O.type = 'U' ";
            sql += "INNER JOIN sysusers U ON U.uid = T1.uid ";
            sql += " UNION ";
            sql += "select ('[' + U.name + '].[' + OBJECT_NAME(O.id) + ']') AS TableName, T1.id, T1.name, U.name as owner ";
            sql += "from sysobjects O ";
            sql += "INNER JOIN sysobjects T1 ON T1.id = O.updtrig AND O.type = 'U' ";
            sql += "INNER JOIN sysusers U ON U.uid = T1.uid ";
            sql += "ORDER BY TableName";
            return sql;
        }

        public void SetTriggers(Tables tables)
        {
            string text = "";
            using (AseConnection conn = new AseConnection(connectioString))
            {
                conn.Open();
                using (AseCommand command = new AseCommand(GetSQLTrigger(), conn))
                {
                    using (AseDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TableTrigger trigger = new TableTrigger(tables[reader["TableName"].ToString()]);
                            trigger.Id = (int)reader["id"];
                            trigger.Name = reader["name"].ToString();
                            trigger.Owner = reader["owner"].ToString();
                            tables[reader["TableName"].ToString()].Triggers.Add(trigger);
                        }
                    }
                }
                foreach (Table table in tables)
                {
                    for (int index = 0; index < table.Triggers.Count; index++)
                    {
                        using (AseCommand command = new AseCommand("sp_helptext '" + table.Triggers[index].Name + "'", conn))
                        {
                            using (AseDataReader reader = command.ExecuteReader())
                            {
                                text = "";
                                reader.NextResult();
                                while (reader.Read())
                                {
                                    text += reader["text"].ToString();
                                }
                                table.Triggers[index].Text = text;
                            }
                        }
                    }
                }
            }
        }

        public Tables Get(Database database)
        {
            Tables tables = new Tables(database);
            double tableCount = GetTablesCount();
            double tableIndex = 0;
            int stats = 0;

            using (AseConnection conn = new AseConnection(connectioString))
            {
                using (AseCommand command = new AseCommand("select O.sysstat2, O.id, O.name, U.name as Owner FROM sysobjects O INNER JOIN sysusers U ON U.uid = O.uid where type = 'U' ORDER BY O.name", conn))
                {
                    conn.Open();
                    using (AseDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Table table = new Table(database);
                            table.Id = (int)reader["id"];
                            table.Name = reader["name"].ToString();
                            table.Owner = reader["Owner"].ToString();
                            stats = (int)reader["sysstat2"];
                            if ((stats & 8192) == 8192) table.LockType = Table.LockTypeEnum.LockAllPages;
                            if ((stats & 16384) == 16384) table.LockType = Table.LockTypeEnum.LockDataPages;
                            if ((stats & 32768) == 32768) table.LockType = Table.LockTypeEnum.LockDataRows;
                            //table.Constraints = (new GenerateConstraint(connectioString,tableFilter)).Get(table); /*Primero debe ir las constraints*/                            
                            //table.Columns = GetColumns(table);
                            //table.Indexes = (new GenerateIndex(connectioString,tableFilter)).Get(table);
                            tables.Add(table);
                            tableIndex++;
                            OnTableProgress(this,new ProgressEventArgs((tableIndex / tableCount) * 100));
                        }
                    }
                }
            }
            if (objectFilter.OptionFilter.FilterTrigger)
                SetTriggers(tables);
            SetColumnsDatabase(tables);
            //tables.Sort();
            tables.ToSQL();
            return tables;
        }
    }
}
