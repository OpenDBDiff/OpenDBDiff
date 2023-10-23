using CommandLine;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Generates;
using OpenDBDiff.SqlServer.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Options;
using System;
using System.Diagnostics;
using System.IO;

namespace OpenDBDiff.CLI
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

        private static bool TestConnection(string connectionString1)
        {
            using (var connection = new SqlConnection())
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
                    sql.Options = SqlFilter;
                    if (options.IgnoreFilters != "") {
                      Console.WriteLine("Apply ignore filters...");
                      var ignoreDict = sql.Options.Ignore.GetOptions();
                      foreach (string opts in options.IgnoreFilters.Split(';')) {
                        if (opts.Trim() != "") {
                          string[] opt = opts.Split('=');
                          ignoreDict[opt[0].Trim()] = (bool)(bool.Parse(opt[1].Trim()));
                        }
                      }
                      sql.Options.Ignore.SetOptions(ignoreDict);
                    }
                    sql.ConnectionString = options.Before;
                    Console.WriteLine("Reading first database...");
                    origin = sql.Process();
                    sql.ConnectionString = options.After;
                    Console.WriteLine("Reading second database...");
                    destination = sql.Process();
                    Console.WriteLine("Comparing databases schemas...");
                    origin = Generate.Compare(origin, destination);
                    // temporary work-around: run twice just like GUI
                    origin.ToSqlDiff(new System.Collections.Generic.List<ISchemaBase>());

                    Console.WriteLine("Generating SQL file...");
                    var script = origin.ToSqlDiff(new System.Collections.Generic.List<ISchemaBase>()).ToSQL();
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
                string newIssueUri = ConfigurationFactory.Instance?.GetValue<string>("OpenDBDiff:NewIssueUri");
                if (string.IsNullOrWhiteSpace(newIssueUri))
                    newIssueUri = "https://github.com/OpenDBDiff/OpenDBDiff/issues/new";

                Console.WriteLine($"{ex.Message}\r\n{ex.StackTrace}\r\n\r\nPlease report this issue at {newIssueUri}.");
                Console.WriteLine();
            }

            return false;
        }

        private static void SaveFile(string filenmame, string sql)
        {
            if (!string.IsNullOrWhiteSpace(filenmame))
            {
                using (var fs = new FileStream(filenmame, FileMode.Create))
                using (var writer = new StreamWriter(fs))
                writer.Write(sql);
            }
        }
    }
}
