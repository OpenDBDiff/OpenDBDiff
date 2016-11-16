namespace DBDiff.Schema.Model
{
    public interface IDatabase : ISchemaBase
    {
        bool IsCaseSensity { get; }
        SqlAction ActionMessage { get; }
        IOption Options { get; }
    }
}
