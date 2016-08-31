namespace DBDiff.Schema.Model
{
    public interface IOptionFilter
    {
        bool IsItemIncluded(ISchemaBase item);
    }
}
