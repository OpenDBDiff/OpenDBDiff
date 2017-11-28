using DBDiff.Front;
using DBDiff.Schema.Events;
using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Generates;

namespace DBDiff.Schema.SQLServer.Generates.Front
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
            if (OnProgress != null)
            {
                this.Generate.OnProgress += new ProgressEventHandler.ProgressHandler(args => OnProgress.Invoke(args));
            }
            
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
