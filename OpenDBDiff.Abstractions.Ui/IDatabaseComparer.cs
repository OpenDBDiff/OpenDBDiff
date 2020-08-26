using OpenDBDiff.Abstractions.Schema.Model;

namespace OpenDBDiff.Abstractions.Ui
{
    public interface IDatabaseComparer
    {
        IDatabase Compare(IDatabase origin, IDatabase destination);
    }
}
