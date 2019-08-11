using CommandLine;
using OpenDBDiff.Schema.SQLServer.Generates.Generates;
using OpenDBDiff.Schema.SQLServer.Generates.Model;
using OpenDBDiff.Schema.SQLServer.Generates.Options;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;

namespace OpenDBDiff.OCDB
{
    public class Program
    {
        private static SqlOption SqlFilter = new SqlOption();

        protected Program()
        {
        }

        private static int Main(string[] args)
        {
            bool completedSuccessfully = false;

            Parser.Default.ParseArguments<CommandlineOptions>(args)
                .WithParsed(options =>
                {
                    try
                    {
                        completedSuccessfully = Work(options);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(false);
            }

            return completedSuccessfully ? 0 : 1;
        }

        private static Boolean TestConnection(string connectionString1)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = connectionString1;
                connection.Open();
                connection.Close();
                return true;
            }
        }

        private static bool Work(CommandlineOptions options)
        {
            try
            {
                Database origin;
                Database destination;
                if (TestConnection(options.Before)
                    && TestConnection(options.After))
                {
                    Generate sql = new Generate();
                    sql.ConnectionString = options.Before;
                    Console.WriteLine("Reading first database...");
                    sql.Options = SqlFilter;
                    origin = sql.Process();

                    sql.ConnectionString = options.After;
                    Console.WriteLine("Reading second database...");
                    destination = sql.Process();
                    Console.WriteLine("Comparing databases schemas...");
                    origin = Generate.Compare(origin, destination);
                    // temporary work-around: run twice just like GUI
                    origin.ToSqlDiff(new System.Collections.Generic.List<Schema.Model.ISchemaBase>());

                    Console.WriteLine("Generating SQL file...");
                    var script = origin.ToSqlDiff(new System.Collections.Generic.List<Schema.Model.ISchemaBase>()).ToSQL();
                    if (!string.IsNullOrWhiteSpace(options.OutputFile))
                    {
                        Console.WriteLine("Writing action script to {0}", options.OutputFile);
                        SaveFile(options.OutputFile, script);
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine(script);
                        Console.WriteLine();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                string newIssueUri = System.Configuration.ConfigurationManager.AppSettings["OpenDBDiff.NewIssueUri"];
                if (string.IsNullOrEmpty(newIssueUri))
                    newIssueUri = "https://github.com/OpenDBDiff/OpenDBDiff/issues/new";

                Console.WriteLine($"{ex.Message}\r\n{ex.StackTrace}\r\n\r\nPlease report this issue at {newIssueUri}.");
                Console.WriteLine();
            }

            return false;
        }

        private static void SaveFile(string filenmame, string sql)
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
