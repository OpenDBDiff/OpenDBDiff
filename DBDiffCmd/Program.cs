using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBDiff.Schema.SQLServer.Options;
using System.IO;
using System.Data.SqlClient;

namespace DBDiff.OCDB
{
    public class Program
    {
        private static SqlOption SqlFilter = new SqlOption();

        static void Main(string[] args)
        {
            try
            {
                Argument arguments = new Argument(args);
                if (arguments.Validate())
                    Work(arguments);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        static Boolean TestConnection(string connectionString1, string connectionString2)
        {
            try
            {
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = connectionString1;
                connection.Open();
                connection.Close();
                connection.ConnectionString = connectionString2;
                connection.Open();
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static void Work(Argument arguments)
        {
            try
            {
                DBDiff.Schema.SQLServer.Model.Database origen;
                DBDiff.Schema.SQLServer.Model.Database destino;
                if (TestConnection(arguments.ConnectionString1, arguments.ConnectionString2))
                {
                    DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
                    sql.ConnectionString = arguments.ConnectionString1;
                    System.Console.WriteLine("Reading first database...");                    
                    origen = sql.Process(SqlFilter);                    
                    sql.ConnectionString = arguments.ConnectionString2;
                    System.Console.WriteLine("Reading second database...");
                    destino = sql.Process(SqlFilter);
                    System.Console.WriteLine("Comparing databases schemas...");
                    origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);
                    System.Console.WriteLine("Generating SQL file...");
                    SaveFile(arguments.OutputFile, origen.ToSql());
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        static void SaveFile(string filenmame, string sql)
        {
            if (!String.IsNullOrEmpty(filenmame))
            {
                StreamWriter writer = new StreamWriter(filenmame, false);
                writer.Write(sql);
                writer.Close();
            }
        }
    }
}
