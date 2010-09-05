using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Text;
using MySql.Data.MySqlClient;
using DBDiff.Schema.Events;
using DBDiff.Schema.MySQL.Options;
using DBDiff.Schema.MySQL.Model;

namespace DBDiff.Schema.MySQL.Generates
{
    public class GenerateTables
    {        
        private string connectioString;
        private MySqlOption tableFilter;
        public event Progress.ProgressHandler OnTableProgress;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="connectioString">Connection string de la base</param>
        public GenerateTables(string connectioString, MySqlOption filter)
        {
            this.connectioString = connectioString;
            this.tableFilter = filter;
        }

        private static string GetSQLColumns(Table table)
        {
            string sql = "";
            sql += "SELECT TABLE_CATALOG, ";
            sql += "TABLE_SCHEMA, ";
            sql += "TABLE_NAME, ";
            sql += "COLUMN_NAME, ";
            sql += "ORDINAL_POSITION,"; 
            sql += "COLUMN_DEFAULT,"; 
            sql += "IF(STRCMP(IS_NULLABLE,'NO'),1,0) AS IS_NULLABLE,";
            sql += "DATA_TYPE, ";
            sql += "IFNULL(CHARACTER_MAXIMUM_LENGTH,0) AS CHARACTER_MAXIMUM_LENGTH,";
            sql += "CHARACTER_OCTET_LENGTH, ";
            sql += "IFNULL(NUMERIC_PRECISION,0) AS NUMERIC_PRECISION, ";
            sql += "IFNULL(NUMERIC_SCALE,0) AS NUMERIC_SCALE, ";
            sql += "CHARACTER_SET_NAME,  ";
            sql += "COLLATION_NAME, "; 
            sql += "COLUMN_TYPE, ";
            sql += "COLUMN_KEY, ";
            sql += "EXTRA, ";
            sql += "PRIVILEGES, ";
            sql += "COLUMN_COMMENT ";
	        sql += "FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + table.Name + "' AND TABLE_SCHEMA = '" + table.Parent.Name + "' ";
            sql += "ORDER BY ORDINAL_POSITION";
            return sql;
        }

        private Columns GetColumnsDatabase(Table table)
        {
            Columns cols = new Columns(table);
            using (MySqlConnection conn = new MySqlConnection(connectioString))
            {
                using (MySqlCommand command = new MySqlCommand(GetSQLColumns(table), conn))
                {
                    conn.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Column col = new Column(table);
                            col.Id = cols.Count+1;
                            col.CharacterSet = reader["CHARACTER_SET_NAME"].ToString();
                            col.Collation = reader["COLLATION_NAME"].ToString();
                            col.Comments = reader["COLUMN_COMMENT"].ToString();
                            col.DefaultValue = reader["COLUMN_DEFAULT"].ToString();
                            col.Extra = reader["EXTRA"].ToString();
                            col.Name = reader["COLUMN_NAME"].ToString();
                            col.Nullable = reader.GetBoolean("IS_NULLABLE");
                            col.OrdinalPosition = reader.GetInt32("ORDINAL_POSITION");
                            col.Precision = reader.GetInt32("NUMERIC_PRECISION");
                            col.Scale = reader.GetInt32("NUMERIC_SCALE");
                            col.Size = reader.GetInt32("CHARACTER_MAXIMUM_LENGTH");
                            col.Type = reader["COLUMN_TYPE"].ToString();
                            cols.Add(col);
                        }
                    }
                }
            }
            return cols;
        }

        private int GetTablesCount(Database database)
        {
            using (MySqlConnection conn = new MySqlConnection(connectioString))
            {
                conn.Open();
                using (MySqlCommand command = new MySqlCommand("select Count(*) from Information_schema.tables WHERE TABLE_SCHEMA = '" + database.Name + "'", conn))
                {
                    return int.Parse(command.ExecuteScalar().ToString());
                }
            }
        }

        private static string GetSQLTrigger(Table table)
        {
            string sql = "";
            sql += "SELECT TRIGGER_SCHEMA, TRIGGER_NAME, EVENT_MANIPULATION, EVENT_OBJECT_SCHEMA, EVENT_OBJECT_TABLE, ACTION_STATEMENT, ACTION_TIMING, SQL_MODE ";
            sql += "FROM INFORMATION_SCHEMA.TRIGGERS ";
            sql += "WHERE EVENT_OBJECT_TABLE = '" + table.Name + "' AND EVENT_OBJECT_SCHEMA = '" + table.Parent.Name + "' ";
            sql += "ORDER BY TRIGGER_NAME";
            return sql;
        }

        public TableTriggers GetTriggers(Table table)
        {
            if (table != null)
            {
                TableTriggers triggers = new TableTriggers(table);
                using (MySqlConnection conn = new MySqlConnection(connectioString))
                {
                    conn.Open();
                    using (MySqlCommand command = new MySqlCommand(GetSQLTrigger(table), conn))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TableTrigger trigger = new TableTrigger(table);
                                trigger.Id = triggers.Count + 1;
                                trigger.Name = reader["TRIGGER_NAME"].ToString();
                                trigger.Manipulation = reader["EVENT_MANIPULATION"].ToString();
                                trigger.Mode = reader["SQL_MODE"].ToString();
                                trigger.Text = reader["ACTION_STATEMENT"].ToString();
                                trigger.Timing = reader["ACTION_TIMING"].ToString();
                                triggers.Add(trigger);
                            }
                        }
                    }
                }
                return triggers;
            }
            else
                throw new ArgumentNullException("table");
        }

        public Tables Get(Database database)
        {
            Tables tables = new Tables(database);
            double tableCount = GetTablesCount(database);
            double tableIndex = 0;

            using (MySqlConnection conn = new MySqlConnection(connectioString))
            {
                database.Name = conn.Database;
                using (MySqlCommand command = new MySqlCommand("select TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE, ENGINE, VERSION, ROW_FORMAT, TABLE_ROWS, AVG_ROW_LENGTH, DATA_LENGTH, MAX_DATA_LENGTH, INDEX_LENGTH, DATA_FREE, IFNULL(AUTO_INCREMENT,0) AS AUTO_INCREMENT, CREATE_TIME, UPDATE_TIME, CHECK_TIME, TABLE_COLLATION, IFNULL(CHECKSUM,0) AS CHECKSUM, CREATE_OPTIONS, TABLE_COMMENT FROM Information_schema.tables WHERE TABLE_SCHEMA = '" + database.Name + "' ORDER BY TABLE_NAME", conn))
                {
                    conn.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Table table = new Table(database);
                            table.Id = (int)tableIndex;
                            table.Name = reader["TABLE_NAME"].ToString();
                            table.AutoIncrement = reader.GetInt32("AUTO_INCREMENT");
                            table.CheckSum = reader.GetBoolean("CHECKSUM");
                            table.Collation = reader["TABLE_COLLATION"].ToString();
                            table.Comments = reader["TABLE_COMMENT"].ToString();
                            table.CreateOptions = reader["CREATE_OPTIONS"].ToString();
                            table.Engine = reader["ENGINE"].ToString();
                            table.Constraints = (new GenerateConstraint(connectioString,tableFilter)).Get(table); /*Primero debe ir las constraints*/
                            table.Columns = GetColumnsDatabase(table);
                            table.Triggers = GetTriggers(table);
                            //table.Indexes = (new GenerateIndex(connectioString,tableFilter)).Get(table);
                            tables.Add(table);
                            tableIndex++;
                            OnTableProgress(this,new ProgressEventArgs((tableIndex / tableCount) * 100));
                        }
                    }
                }
            }
            //tables.Sort();
            tables.ToSQL();
            return tables;
        }
    }
}
