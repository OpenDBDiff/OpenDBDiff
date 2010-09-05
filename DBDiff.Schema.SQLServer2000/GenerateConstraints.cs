using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Data.SqlClient;
using DBDiff.Schema.SQLServer2000.Model;


namespace DBDiff.Schema.SQLServer2000
{
    public class GenerateConstraints
    {
        private string connectioString;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="connectioString">Connection string de la base</param>
        public GenerateConstraints(string connectioString)
        {
            this.connectioString = connectioString;
        }

        private Constraints GetColumnsUNIQUE(Table table)
        {
            Constraints cons = new Constraints(table);
            Boolean isFirst = true;
            using (SqlConnection conn = new SqlConnection(connectioString))
            {
                using (SqlCommand command = new SqlCommand("SELECT SI.indid, SO.Name AS UniqueName,SCC.colid, SCC.Name from sysconstraints SC INNER JOIN sysobjects SO ON SC.constid = SO.ID INNER JOIN sysindexes SI ON SI.name = SO.Name INNER JOIN sysindexkeys SIK ON SIK.indid = SI.indid AND SC.ID = SIK.ID INNER JOIN syscolumns SCC ON SCC.colid = SIK.colid AND SCC.ID = SC.ID where SO.xtype = 'UQ' and SC.id = " + table.Id.ToString() + " ORDER BY SIK.keyno", conn))
                {
                    conn.Open();                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Constraint con = null;
                        while (reader.Read())
                        {                            
                            ConstraintColumn ccon = new ConstraintColumn(con);
                            if (isFirst)
                            {
                                if (con != null) cons.Add(con);
                                con = new Constraint(table);
                                con.Name = reader["UniqueName"].ToString();
                                con.Type = Constraint.ConstraintType.Unique;
                                con.Clustered = (int.Parse(reader["indid"].ToString()) == 1); /*Si el campo indid = 1, es clustered*/
                                isFirst = false;
                            }
                            ccon.Name = reader["Name"].ToString();
                            ((Database)table.Parent).Dependencies.Add(table.Id, Int32.Parse(reader["colid"].ToString()), table.Id,con);
                            con.Columns.Add(ccon);
                        }
                        if (con != null) cons.Add(con);
                    }
                }
            }
            return cons;
        }
        
        private Constraints GetColumnsPK(Table table)
        {
            Constraints cons = new Constraints(table);
            Boolean isFirst = true;
            using (SqlConnection conn = new SqlConnection(connectioString))
            {
                using (SqlCommand command = new SqlCommand("select SI.indid, SI.name AS PKName, SC.colid, SC.Name,SI.indid from sysindexes SI INNER JOIN sysindexkeys SIK ON SI.indid = SIK.indid AND SIK.id = SI.ID INNER JOIN syscolumns SC ON SC.colid = SIK.colid AND SC.id = SI.ID WHERE (SI.status & 0x800) = 0x800 AND SI.id = " + table.Id.ToString() + " ORDER BY SIK.keyno", conn))
                {
                    conn.Open();                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Constraint con = null;
                        while (reader.Read())
                        {                            
                            ConstraintColumn ccon = new ConstraintColumn(con);
                            if (isFirst)
                            {
                                if (con != null) cons.Add(con);
                                con = new Constraint(table);
                                con.Name = reader["PKName"].ToString();
                                con.Type = Constraint.ConstraintType.PrimaryKey;
                                con.Clustered = ((short)reader["indid"] == 1); /*Si el campo indid = 1, es clustered*/
                                isFirst = false;
                            }
                            ccon.Name = reader["Name"].ToString();
                            ((Database)table.Parent).Dependencies.Add(table.Id, (short)reader["colid"], table.Id,con);
                            con.Columns.Add(ccon);
                        }
                        if (con != null) cons.Add(con);
                    }
                }
            }
            return cons;
        }

        private Constraints GetColumnsFK(Table table)
        {
            Constraints cons = new Constraints(table);
            string lastFK = "";
            using (SqlConnection conn = new SqlConnection(connectioString))
            {
                using (SqlCommand command = new SqlCommand("SELECT OBJECTPROPERTY(SC.constId, 'CnstIsNotRepl') AS NotForReplication,SO2.Name AS FKName,SOF.id AS TableFId, SOF.Name AS TableName,SOR.id AS TableRId, SOR.Name AS TableRelationalName,SCF.colid AS ColFId,SCF.Name AS ColumnName,SCR.colid AS colRId, SCR.Name AS ColumnRelationalName, OBJECTPROPERTY(SC.constid,'CnstIsNotTrusted') AS WithNoCheck FROM sysforeignkeys SFK INNER JOIN sysconstraints SC ON SC.constid = SFK.constid INNER JOIN sysobjects SO ON SO.id = SC.id INNER JOIN sysobjects SO2 ON SO2.id = SC.constid INNER JOIN sysobjects SOF ON SOF.id = SFK.fkeyid INNER JOIN sysobjects SOR ON SOR.id = SFK.rkeyid INNER JOIN syscolumns SCF ON SCF.colid = SFK.fkey AND SCF.id = SOF.ID INNER JOIN syscolumns SCR ON SCR.colid = SFK.rkey AND SCR.id = SOR.ID WHERE SO.id = " + table.Id.ToString() + " ORDER BY SFK.keyno", conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Constraint con = null;
                        while (reader.Read())
                        {                            
                            ConstraintColumn ccon = new ConstraintColumn(con);

                            if (!lastFK.Equals(reader["FKName"].ToString()))
                            {
                                if (!String.IsNullOrEmpty(lastFK)) cons.Add(con);
                                con = new Constraint(table);
                                con.NotForReplication = reader["NotForReplication"].ToString().Equals("1");
                                con.Name = reader["FKName"].ToString();
                                con.Type = Constraint.ConstraintType.ForeignKey;
                                con.WithNoCheck = ((int)reader["WithNoCheck"] == 1);
                                con.RelationalTable = reader["TableRelationalName"].ToString();
                                con.RelationalTableId = (int)reader["TableRId"];
                                lastFK = reader["FKName"].ToString();
                            }
                            ccon.Name = reader["ColumnName"].ToString();
                            ccon.ColumnRelationalName = reader["ColumnRelationalName"].ToString();
                            table.DependenciesCount++;
                            ((Database)table.Parent).Dependencies.Add((int)reader["TableRId"], (short)reader["colRId"], table.Id, con);
                            ((Database)table.Parent).Dependencies.Add((int)reader["TableFId"], (short)reader["colFId"], table.Id, con);
                            con.Columns.Add(ccon);                            
                        }
                        if (con != null) cons.Add(con);
                    }
                }
            }
            return cons;
        }

        public Constraints GetConstraints(Table table)
        {            
            Constraints consFK = GetColumnsFK(table);
            Constraints consPK = GetColumnsPK(table);
            Constraints consUN = GetColumnsUNIQUE(table);
            foreach (Constraint con in consFK)
                consPK.Add(con);
            foreach (Constraint con in consUN)
                consPK.Add(con);
            return consPK;
        }
    }
}
