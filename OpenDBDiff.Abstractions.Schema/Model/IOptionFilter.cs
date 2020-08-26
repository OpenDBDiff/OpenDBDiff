namespace OpenDBDiff.Abstractions.Schema.Model
{
    public interface IOptionFilter : IOptionsContainer<string>
    {
        bool IsItemIncluded(ISchemaBase item);
    }
}
