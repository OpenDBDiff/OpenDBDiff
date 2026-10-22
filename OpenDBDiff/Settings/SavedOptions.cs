using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenDBDiff.Settings
{
    /// <summary>
    /// The comparison options (exclusions, ignore flags, defaults, script and comparison settings)
    /// as last applied in the options dialog, stored in settings.liteDb so they survive a restart.
    /// </summary>
    public class SavedOptions
    {
        private const int SingletonId = 1;
        private const string CollectionName = "options";

        public int Id { get; set; } = SingletonId;

        // A list instead of a dictionary: filter patterns such as "dbo.dtproperties" aren't valid BSON field names.
        public List<SavedFilter> Filters { get; set; }

        public Dictionary<string, string> Defaults { get; set; }

        public Dictionary<string, bool> Ignore { get; set; }

        public Dictionary<string, bool> Script { get; set; }

        public Dictionary<string, string> Comparison { get; set; }

        public static void Save(IOption option)
        {
            try
            {
                var saved = new SavedOptions
                {
                    Filters = option.Filters.GetOptions().Select(f => new SavedFilter { Pattern = f.Key, ObjectType = f.Value }).ToList(),
                    Defaults = new Dictionary<string, string>(option.Defaults.GetOptions()),
                    Ignore = new Dictionary<string, bool>(option.Ignore.GetOptions()),
                    Script = new Dictionary<string, bool>(option.Script.GetOptions()),
                    Comparison = new Dictionary<string, string>(option.Comparison.GetOptions()),
                };

                using (var db = Project.GetDatabase())
                {
                    db.GetCollection<SavedOptions>(CollectionName).Upsert(saved);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"OpenDBDiff\settings.liteDb options save: {0}", ex);
            }
        }

        /// <summary>
        /// Returns the saved options laid over <paramref name="defaults"/>, or <paramref name="defaults"/> itself
        /// when nothing is saved. Settings added in a newer version keep their default value.
        /// </summary>
        public static IOption Load(IOption defaults)
        {
            try
            {
                SavedOptions saved;
                using (var db = Project.GetDatabase())
                {
                    saved = db.GetCollection<SavedOptions>(CollectionName).FindById(SingletonId);
                }

                return saved == null ? defaults : new MergedOption(saved, defaults);
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"OpenDBDiff\settings.liteDb options load: {0}", ex);
                return defaults;
            }
        }

        private static IDictionary<string, T> Merge<T>(IDictionary<string, T> defaults, IDictionary<string, T> saved)
        {
            var merged = new Dictionary<string, T>(defaults);
            if (saved != null)
            {
                foreach (var pair in saved.Where(p => merged.ContainsKey(p.Key)))
                    merged[pair.Key] = pair.Value;
            }
            return merged;
        }

        // Only read through GetOptions(): the project handler copies it into its own option type.
        private sealed class MergedOption : IOption
        {
            public MergedOption(SavedOptions saved, IOption defaults)
            {
                Filters = saved.Filters == null
                    ? (IOptionFilter)new Container(defaults.Filters.GetOptions())
                    : new Container(saved.Filters
                        .Where(f => !string.IsNullOrEmpty(f.Pattern) && Enum.TryParse<ObjectType>(f.ObjectType, true, out _))
                        .GroupBy(f => f.Pattern)
                        .ToDictionary(g => g.Key, g => g.First().ObjectType));
                Defaults = new Container(Merge(defaults.Defaults.GetOptions(), saved.Defaults));
                Ignore = new Container<bool>(Merge(defaults.Ignore.GetOptions(), saved.Ignore));
                Script = new Container<bool>(Merge(defaults.Script.GetOptions(), saved.Script));

                var comparison = Merge(defaults.Comparison.GetOptions(), saved.Comparison);
                bool.TryParse(comparison.TryGetValue("ReloadComparisonOnUpdate", out var reload) ? reload : null, out var reloadOnUpdate);
                Comparison = new Container(comparison) { ReloadComparisonOnUpdate = reloadOnUpdate };
            }

            public IOptionFilter Filters { get; }

            public IOptionsContainer<string> Defaults { get; }

            public IOptionsContainer<bool> Ignore { get; }

            public IOptionsContainer<bool> Script { get; }

            public IOptionComparison Comparison { get; }

            public string Serialize() => ToString();
        }

        private class Container<T> : IOptionsContainer<T>
        {
            private readonly IDictionary<string, T> options;

            public Container(IDictionary<string, T> options)
            {
                this.options = options;
            }

            public IDictionary<string, T> GetOptions() => new Dictionary<string, T>(options);
        }

        private sealed class Container : Container<string>, IOptionFilter, IOptionComparison
        {
            public Container(IDictionary<string, string> options) : base(options) { }

            public bool ReloadComparisonOnUpdate { get; set; }

            public bool IsItemIncluded(ISchemaBase item) => throw new NotSupportedException();
        }
    }

    public class SavedFilter
    {
        public string Pattern { get; set; }

        public string ObjectType { get; set; }
    }
}
