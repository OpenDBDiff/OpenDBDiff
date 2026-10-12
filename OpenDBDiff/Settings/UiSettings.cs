using System;

namespace OpenDBDiff.Settings
{
    public class UiSettings
    {
        private const int SingletonId = 1;

        public int Id { get; set; } = SingletonId;

        public int SchemaSplitterDistance { get; set; }

        public static UiSettings Load()
        {
            try
            {
                using (var db = Project.GetDatabase())
                {
                    return db.GetCollection<UiSettings>("uisettings").FindById(SingletonId) ?? new UiSettings();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"OpenDBDiff\settings.liteDb ui settings load: {0}", ex);
                return new UiSettings();
            }
        }

        public void Save()
        {
            try
            {
                using (var db = Project.GetDatabase())
                {
                    db.GetCollection<UiSettings>("uisettings").Upsert(this);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"OpenDBDiff\settings.liteDb ui settings save: {0}", ex);
            }
        }
    }
}
