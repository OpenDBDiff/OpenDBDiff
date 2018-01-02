using OpenDBDiff.Schema.Model;

namespace OpenDBDiff.Front
{

    public interface IGenerator
    {
        event Schema.Events.ProgressEventHandler.ProgressHandler OnProgress;

        int GetMaxValue();
        IDatabase Process();
    }
}