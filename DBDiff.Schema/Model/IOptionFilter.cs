namespace DBDiff.Schema.Model
{
    public interface IOptionFilter : IOptionsContainer<string>
    {
        bool IsItemIncluded(ISchemaBase item);
    }
}
