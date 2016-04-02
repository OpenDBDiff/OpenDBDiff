using System;
using System.Data.SqlClient;
using System.IO;
using DBDiff.Schema.SQLServer.Generates.Generates;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.SQLServer.Generates.Options;

namespace DBDiff.OCDB
{
    public class Program
    {
        private static SqlOption SqlFilter = new SqlOption();

        static int Main(string[] args)
        {
            bool completedSuccessfully = false;
            try
            {
                Argument arguments = new Argument(args);
                if (arguments.Validate())
                    completedSuccessfully = Work(arguments);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return completedSuccessfully ? 0 : 1;
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

        static bool Work(Argument arguments)
        {
            bool completedSuccessfully = false;
            try
            {
                Database origin;
                Database destination;
                if (TestConnection(arguments.ConnectionString1, arguments.ConnectionString2))
                {
                    Generate sql = new Generate();
                    sql.ConnectionString = arguments.ConnectionString1;
                    Console.WriteLine("Reading first database...");
                    sql.Options = SqlFilter;
                    origin = sql.Process();

                    sql.ConnectionString = arguments.ConnectionString2;
                    Console.WriteLine("Reading second database...");
                    destination = sql.Process();
                    Console.WriteLine("Comparing databases schemas...");
                    origin = Generate.Compare(origin, destination);
                    if (!arguments.OutputAll)
                    {
                        // temporary work-around: run twice just like GUI
                        origin.ToSqlDiff();
                    }

                    Console.WriteLine("Generating SQL file...");
                    SaveFile(arguments.OutputFile, arguments.OutputAll ? origin.ToSql() : origin.ToSqlDiff().ToSQL());
                    completedSuccessfully = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("{0}\r\n{1}\r\n\r\nPlease report this issue at http://opendbiff.codeplex.com/workitem/list/basic\r\n\r\n", ex.Message, ex.StackTrace));
            }

            return completedSuccessfully;
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
