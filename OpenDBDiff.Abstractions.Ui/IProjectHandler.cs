using OpenDBDiff.Schema.Model;
using OpenDBDiff.Abstractions.Ui;

namespace OpenDBDiff.Abstractions.Ui
{
    public interface IProjectHandler
    {
        IFront CreateSourceSelector();
        IFront CreateDestinationSelector();
        string GetSourceConnectionString();
        string GetDestinationConnectionString();
        string GetSourceServerName();
        string GetSourceDatabaseName();
        string GetDestinationServerName();
        OptionControl CreateOptionControl();
        string GetDestinationDatabaseName();

        IGenerator SetSourceGenerator(string connectionString, IOption options);
        IGenerator SetDestinationGenerator(string connectionString, IOption options);
        IDatabaseComparer GetDatabaseComparer();
        IOption GetDefaultProjectOptions();
        string GetScriptLanguage();
        void Unload();
    }
}
