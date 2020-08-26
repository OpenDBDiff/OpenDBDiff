using OpenDBDiff.Abstractions.Schema.Model;

namespace OpenDBDiff.Abstractions.Ui
{

    public interface IGenerator
    {
        event Schema.Events.ProgressEventHandler.ProgressHandler OnProgress;

        int GetMaxValue();
        IDatabase Process();
    }
}
