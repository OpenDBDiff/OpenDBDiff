namespace OpenDBDiff.Schema.Model
{
    public interface IOptionFilter : IOptionsContainer<string>
    {
        bool IsItemIncluded(ISchemaBase item);
    }
}
