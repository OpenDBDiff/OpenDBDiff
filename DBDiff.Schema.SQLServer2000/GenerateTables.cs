using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data.SqlClient;
using DBDiff.Schema.Events;
using DBDiff.Schema.SQLServer2000.Model;

namespace DBDiff.Schema.SQLServer2000
{
    public class GenerateTables
    {        
        private string connectioString;
        public event Progress.ProgressHandler OnTableProgress;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="connectioString">Connection string de la base</param>
        public GenerateTables(string connectioString)
        {
            this.connectioString = connectioString;
        }

        private Constraints GetConstraint(Table table)
        {
            GenerateConstraints genConst = new GenerateConstraints(connectioString);
            return genConst.GetConstraints(table);
        }

        private string GetSQLColumns(Table table)
        {
            string sql = "";
            sql += "SELECT DISTINCT ISNULL(IDENT_SEED('" + table.Name + "'),0) AS IdentSeed,ISNULL(IDENT_INCR('" + table.Name + "'),0) AS IdentIncrement,COLUMNPROPERTY(sc2.id, sc2.name, 'IsIdentity') AS IsIdentity, COLUMNPROPERTY(sc2.id, sc2.name, 'IsIdNotForRepl') AS IsIdentityRepl, ";
            sql += "COLUMNPROPERTY(sc2.id, sc2.name, 'IsRowGuidCol') AS IsRowGuid,SCS2.text AS Formula,ST.Name AS Tipo, ST2.Name AS OriginalTipo, ISNULL(SC2.Collation,'') AS Collation, ";
            sql += "SC2.*, CASE WHEN ISNULL(SD.depid,0) = 0 THEN 0 ELSE 1 END AS HasComputedDepends, CASE WHEN ISNULL(SI.indid,0) = 0 THEN 0 ELSE 1 END AS HasIndexDepends ";
            sql += "from sysobjects SO ";
            sql += "INNER JOIN syscolumns SC2 ON SC2.id = SO.id ";
            sql += "INNER JOIN systypes ST ON ST.xusertype = SC2.xusertype ";
            sql += "INNER JOIN systypes ST2 ON ST2.xusertype = SC2.xtype ";
            sql += "LEFT JOIN syscomments SCS2 ON SC2.colId = SCS2.Number AND SC2.id = SCS2.id ";
            sql += "LEFT JOIN sysdepends SD ON SD.id = SD.depid and SD.depNumber = SC2.colId AND SD.id = SO.id ";
            sql += "LEFT JOIN sysindexes SI ON SI.id = SO.id AND SI.root > 0 LEFT JOIN sysindexkeys SIK ON SIK.indid = SI.indid AND SIK.ID = SI.ID AND SIK.colid = SC2.colid ";
            sql += "WHERE SO.id = " + table.Id.ToString() + " ORDER BY SC2.Colorder";
            return sql;
        }

        private Columns GetColumns(Table table)
        {
            Columns cols = GetColumnsDatabase(table);
            GetColumnsConstraintsDatabase(cols);
            return cols;
        }

        private void GetColumnsConstraintsDatabase(Columns columns)
        {
            Column column = null;
            //List<ColumnConstraint> cons = new List<ColumnConstraint>();
            using (SqlConnection conn = new SqlConnection(connectioString))
            {
                using (SqlCommand command = new SqlCommand("SELECT OBJECTPROPERTY(SCC.constId, 'CnstIsNotRepl') AS NotForReplication, SCC.ColId, RTRIM(Type) AS Type, Name, Text FROM sysobjects SO INNER JOIN syscomments SC ON SC.id = SO.id INNER JOIN sysconstraints SCC ON SCC.constid = SO.id WHERE type IN ('D','C') AND parent_obj = " + columns.Parent.Id.ToString(), conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        ColumnConstraint con = null;
                        while (reader.Read())
                        {
                            column = columns.GetById(int.Parse(reader["ColId"].ToString()));
                            con = new ColumnConstraint(column);
                            con.NotForReplication = reader["NotForReplication"].ToString().Equals("1");
                            con.Name = reader["Name"].ToString();
                            con.Type = reader["Type"].ToString().Equals("D") ? Constraint.ConstraintType.Default : Constraint.ConstraintType.Check;
                            con.Value = reader["Text"].ToString();
                            column.Constraints.Add(con);
                        }
                    }
                }
            }
        }

        private Columns GetColumnsDatabase(Table table)
        {
            Columns cols = new Columns(table);
            using (SqlConnection conn = new SqlConnection(connectioString))
            {
                using (SqlCommand command = new SqlCommand(GetSQLColumns(table), conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Column col = new Column(table);
                            col.Id = (short)reader["colid"];
                            col.Identity = ((int)reader["IsIdentity"] == 1);
                            col.IdentityForReplication = ((int)reader["IsIdentityRepl"] == 1);
                            if ((col.Identity) || (col.IdentityForReplication))
                            {
                                col.IdentitySeed = (int)(decimal)reader["IdentSeed"];
                                col.IdentityIncrement = (int)(decimal)reader["IdentIncrement"];
                            }
                            col.Name = reader["Name"].ToString();
                            col.HasComputedDependencies = ((int)reader["HasComputedDepends"] == 1);
                            col.HasIndexDependencies = ((int)reader["HasIndexDepends"] == 1);
                            col.ComputedFormula = reader["formula"].ToString();
                            col.IsComputed = reader["iscomputed"].ToString().Equals("1");
                            col.Nullable = reader["isnullable"].ToString().Equals("1");
                            col.Collation = reader["collation"].ToString();
                            col.Precision = int.Parse(reader["xprec"].ToString());
                            col.Scale = int.Parse(reader["xscale"].ToString());
                            if (!String.IsNullOrEmpty(reader["prec"].ToString()))
                                col.Size = int.Parse(reader["prec"].ToString());
                            col.IsRowGuid = reader["IsRowGuid"].ToString().Equals("1");
                            col.Type = reader["Tipo"].ToString();
                            col.OriginalType = reader["OriginalTipo"].ToString();
                            cols.Add(col);
                        }
                    }
                }
            }
            return cols;
        }

        private int GetTablesCount(Database database)
        {
            using (SqlConnection conn = new SqlConnection(connectioString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand("SELECT Count(*) FROM sysobjects SO WHERE type = 'U'", conn))
                {
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public TableTriggers GetTriggers(Table table)
        {
            ArrayList names = new ArrayList();
            string text = "";
            TableTriggers triggers = new TableTriggers(table);
            using (SqlConnection conn = new SqlConnection(connectioString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand("SELECT SO.name FROM sysobjects SO WHERE type = 'TR' and SO.parent_obj = " + table.Id.ToString() + " ORDER BY SO.name", conn))
                {                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            names.Add(reader["name"].ToString());
                        }
                    }
                }
                for (int index = 0; index < names.Count; index++)
                {
                    using (SqlCommand command = new SqlCommand("sp_helptext '" + names[index] + "'", conn))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            text = "";
                            while (reader.Read())
                            {
                                text += reader["text"].ToString();
                            }
                            TableTrigger trigger = new TableTrigger(table);
                            trigger.Name = names[index].ToString();
                            trigger.Text = text;
                            triggers.Add(trigger);
                        }
                    }
                }
            }
            return triggers;
        }

        public Tables Get(Database database)
        {
            Tables tables = new Tables(database);
            double tableCount = GetTablesCount(database);
            double tableIndex = 0;
            int textInRow;
            int isPinned;

            using (SqlConnection conn = new SqlConnection(connectioString))
            {
                using (SqlCommand command = new SqlCommand("SELECT SO.name,SO.id,SU.name as Owner, OBJECTPROPERTY(SO.ID,'TableTextInRowLimit') AS TextInRowLimit,OBJECTPROPERTY(SO.ID,'TableIsPinned') AS IsPinned FROM sysobjects SO INNER JOIN sysusers SU ON SU.uid = SO.uid WHERE type = 'U' ORDER BY SO.name", conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Table table = new Table(database);
                            table.Id = int.Parse(reader["id"].ToString());
                            table.Name = reader["Name"].ToString();
                            table.Owner = reader["Owner"].ToString();
                            textInRow = int.Parse(reader["TextInRowLimit"].ToString());
                            isPinned = int.Parse(reader["IsPinned"].ToString());
                            table.Constraints = GetConstraint(table); /*Primero debe ir las constraints*/
                            table.Columns = GetColumns(table);
                            table.Triggers = GetTriggers(table);
                            if (textInRow > 0) table.Options.Add("TextInRow", textInRow.ToString());
                            if (isPinned > 0) table.Options.Add("IsPinned","1");
                            tables.Add(table);
                            tableIndex++;
                            OnTableProgress(this,new ProgressEventArgs((tableIndex / tableCount) * 100));
                        }
                    }
                }
            }
            tables.Sort();
            return tables;
        }
    }
}
