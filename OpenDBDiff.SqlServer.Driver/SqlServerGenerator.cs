using OpenDBDiff.Abstractions.Schema.Events;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Generates;
using OpenDBDiff.SqlServer.Schema.Options;

namespace OpenDBDiff.SqlServer.Driver
{
    /// <summary>
    /// Loads one SQL Server database into the schema model, reporting progress while it does.
    /// </summary>
    public class SqlServerGenerator : IGenerator
    {
        private readonly Generate generate;

        public event ProgressEventHandler.ProgressHandler OnProgress;

        public SqlServerGenerator(string connectionString, IOption option)
        {
            generate = new Generate
            {
                ConnectionString = connectionString,
                Options = new SqlOption(option)
            };

            generate.OnProgress += args => OnProgress?.Invoke(args);
        }

        public int GetMaxValue()
        {
            // Static on Generate: the schema loader reports one global maximum.
            return Generate.MaxValue;
        }

        public IDatabase Process()
        {
            return generate.Process();
        }
    }
}
