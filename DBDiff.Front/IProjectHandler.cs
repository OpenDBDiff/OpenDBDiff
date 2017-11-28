using System;
using System.Windows.Forms;
using DBDiff.Schema.Model;

namespace DBDiff.Front
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
        IOption GetProjectOptions();
        string GetScriptLanguage();
        void Unload();
    }
}