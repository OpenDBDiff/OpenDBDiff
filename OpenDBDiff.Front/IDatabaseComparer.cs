using OpenDBDiff.Schema.Model;

namespace OpenDBDiff.Front
{

    public interface IDatabaseComparer
    {
        IDatabase Compare(IDatabase source, IDatabase destination);
    }
}