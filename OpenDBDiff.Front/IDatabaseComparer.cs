using OpenDBDiff.Schema.Model;

namespace OpenDBDiff.Front
{

    public interface IDatabaseComparer
    {
        IDatabase Compare(IDatabase origin, IDatabase destination);
    }
}
