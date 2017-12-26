using DBDiff.Schema.Model;

namespace DBDiff.Front
{

    public interface IGenerator
    {
        event Schema.Events.ProgressEventHandler.ProgressHandler OnProgress;

        int GetMaxValue();
        IDatabase Process();
    }
}