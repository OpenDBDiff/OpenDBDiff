using OpenDBDiff.Front;
using OpenDBDiff.Schema.Events;
using OpenDBDiff.Schema.Model;
using OpenDBDiff.Schema.SQLServer.Generates.Generates;

namespace OpenDBDiff.Schema.SQLServer.Generates.Front
{
    public class SQLServerGenerator : IGenerator
    {
        private readonly Generate Generate;

        public event ProgressEventHandler.ProgressHandler OnProgress;

        public SQLServerGenerator(string connectionString, IOption option)
        {
            this.Generate = new Generate()
            {
                ConnectionString = connectionString,
                Options = new Options.SqlOption(option)
            };
            this.Generate.OnProgress += new ProgressEventHandler.ProgressHandler(args =>
            {
                if (OnProgress != null)
                    OnProgress.Invoke(args);
            });

        }

        public int GetMaxValue()
        {
            return Generate.MaxValue;
        }

        public IDatabase Process()
        {
            return this.Generate.Process();
        }
    }
}
