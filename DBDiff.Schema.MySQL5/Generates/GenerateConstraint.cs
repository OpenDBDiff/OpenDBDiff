using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using MySql.Data.MySqlClient;
using DBDiff.Schema.MySQL.Model;
using DBDiff.Schema.MySQL.Options;

namespace DBDiff.Schema.MySQL.Generates
{
    public class GenerateConstraint
    {
        private string connectioString;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="connectioString">Connection string de la base</param>
        public GenerateConstraint(string connectioString, MySqlOption filter)
        {
            this.connectioString = connectioString;
        }

        private static string GetSQL(Table table)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT TC.TABLE_SCHEMA,");
            sql.Append("TC.CONSTRAINT_NAME,");
            sql.Append("TC.CONSTRAINT_TYPE,");
            sql.Append("COLUMN_NAME,");
            sql.Append("ORDINAL_POSITION,");
            sql.Append("IFNULL(POSITION_IN_UNIQUE_CONSTRAINT,0) AS POSITION_IN_UNIQUE_CONSTRAINT,");
            sql.Append("REFERENCED_TABLE_SCHEMA,");
            sql.Append("REFERENCED_TABLE_NAME,");
            sql.Append("REFERENCED_COLUMN_NAME ");
            sql.Append("FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ");
            sql.Append("INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU ON KCU.CONSTRAINT_SCHEMA = TC.CONSTRAINT_SCHEMA AND KCU.CONSTRAINT_NAME = TC.CONSTRAINT_NAME AND KCU.TABLE_NAME = TC.TABLE_NAME AND KCU.TABLE_SCHEMA = TC.TABLE_SCHEMA ");
            sql.Append("WHERE TC.TABLE_NAME = '" + table.Name + "' AND TC.TABLE_SCHEMA = '" + table.Parent.Name + "' ");
            sql.Append("AND CONSTRAINT_TYPE <> 'UNIQUE'");
            return sql.ToString();
        }

        public Constraints Get(Table table)
        {
            Constraints cons = null;
            string last = "";
            using (MySqlConnection conn = new MySqlConnection(connectioString))
            {
                using (MySqlCommand command = new MySqlCommand(GetSQL(table), conn))
                {
                    conn.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        Constraint con = null;
                        while (reader.Read())
                        {                            
                            if (cons == null) cons = new Constraints(table);
                            ConstraintColumn ccon = new ConstraintColumn(con);
                            if (!last.Equals(reader["CONSTRAINT_NAME"].ToString()))
                            {
                                if (!String.IsNullOrEmpty(last)) cons.Add(con);
                                con = new Constraint(table);
                                con.TypeText = reader["CONSTRAINT_TYPE"].ToString();
                                con.Name = reader["CONSTRAINT_NAME"].ToString();
                                con.Owner = reader["TABLE_SCHEMA"].ToString();
                                last = reader["CONSTRAINT_NAME"].ToString();
                                cons.Add(con);
                            }
                            ccon.Name = reader["COLUMN_NAME"].ToString();
                            ccon.OrdinalPosition = reader.GetInt32("ORDINAL_POSITION");
                            ccon.PositionUniqueConstraint = reader.GetInt32("POSITION_IN_UNIQUE_CONSTRAINT");
                            if (con.Type == Constraint.ConstraintType.ForeignKey)
                            {
                                ccon.PositionUniqueConstraint = reader.GetInt32("POSITION_IN_UNIQUE_CONSTRAINT");
                                ccon.ReferencedColumnName = reader["REFERENCED_COLUMN_NAME"].ToString();
                                ccon.ReferencedSchemaName = reader["REFERENCED_TABLE_SCHEMA"].ToString();
                                ccon.ReferencedTableName = reader["REFERENCED_TABLE_NAME"].ToString();
                            }
                            table.DependenciesCount++;
                            //((Database)table.Parent).ConstraintDependencies.Add((int)reader["TableRelationalId"], (int)reader["ColumnRelationalId"], table.Id, con);
                            //((Database)table.Parent).ConstraintDependencies.Add((int)reader["TableId"], (int)reader["ColumnId"], table.Id, con);
                            con.Columns.Add(ccon);
                        }                        
                    }
                }
            }
            return cons;
        }
    }
}
