namespace DBDiff.Schema.Model
{
    public interface ISchemaList<T, P>
        where T : ISchemaBase
        where P : ISchemaBase
    {
        void Add(T item);
        SchemaList<T, P> Clone(P parentObject);
        bool Exists(string name);
        T Find(int id);
        T this[string name] { get; set; }
        T this[int index] { get; set; }
        P Parent { get; }
        int Count { get; }
    }
}
