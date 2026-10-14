using OpenDBDiff.Abstractions.Schema.Events;

namespace OpenDBDiff.Abstractions.Schema.Model
{
    /// <summary>
    /// Produces a schema snapshot for one side of a comparison.
    /// </summary>
    /// <remarks>
    /// This contract used to live in OpenDBDiff.Abstractions.Ui (net48), which made the comparison
    /// pipeline unusable from any front end that is not WinForms. It has no UI types in it, so it
    /// lives with the schema model instead and every front end can drive a comparison.
    /// </remarks>
    public interface IGenerator
    {
        event ProgressEventHandler.ProgressHandler OnProgress;

        int GetMaxValue();

        IDatabase Process();
    }
}
