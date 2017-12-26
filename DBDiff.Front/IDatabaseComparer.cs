using DBDiff.Schema.Model;

namespace DBDiff.Front
{

    public interface IDatabaseComparer
    {
        IDatabase Compare(IDatabase source, IDatabase destination);
    }
}