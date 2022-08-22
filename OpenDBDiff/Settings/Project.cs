using LiteDB;
using OpenDBDiff.Abstractions.Schema.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace OpenDBDiff.Settings
{
    public class Project
    {
        private const string settingsFile = "settings.liteDb";
        private static bool showErrors;

        public enum ProjectType
        {
            SQLServer = 1
        }

        public string ConnectionStringDestination { get; set; }

        public string ConnectionStringSource { get; set; }

        public Guid Id { get; set; }

        public bool IsLastConfiguration { get; set; }
        [BsonIgnore] public IOption Options { get; set; }

        public string ProjectName { get; set; }

        public DateTime SavedDateTime { get; private set; }

        public ProjectType Type { get; set; }

        private static string SettingsFilePath
        {
            get
            {
                var userLocalAppDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), nameof(OpenDBDiff));
                if (!Directory.Exists(userLocalAppDataDirectory)) Directory.CreateDirectory(userLocalAppDataDirectory);

                return Path.Combine(userLocalAppDataDirectory, settingsFile);
            }
        }

        public static int Delete(Guid id)
        {
            using (var db = GetDatabase())
            {
                var projects = db.GetCollection<Project>("projects");
                return projects.DeleteMany(p => p.Id == id);
            }
        }

        public static IEnumerable<Project> GetAll()
        {
            using (var db = GetDatabase())
            {
                return db.GetCollection<Project>("projects").FindAll().ToArray();
            }
        }

        public static Project GetLastConfiguration()
        {
            using (var db = GetDatabase())
            {
                var projects = db.GetCollection<Project>("projects");
                return projects.Query()
                    .Where(p => p.IsLastConfiguration)
                    .OrderBy(p => p.ProjectName)
                    .FirstOrDefault();
            }
        }

        public static void SaveLastConfiguration(String connectionStringSource, String connectionStringDestination)
        {
            var last = GetLastConfiguration() ?? new Project
            {
                Id = Guid.NewGuid(),
                ProjectName = "LastConfiguration",
                Type = ProjectType.SQLServer,
                IsLastConfiguration = true
            };
            last.ConnectionStringSource = connectionStringSource;
            last.ConnectionStringDestination = connectionStringDestination;
            Upsert(last);
        }

        public static bool Upsert(Project item)
        {
            try
            {
                using (var db = GetDatabase())
                {
                    var projects = db.GetCollection<Project>("projects");
                    item.SavedDateTime = DateTime.Now;
                    return projects.Upsert(item);
                }
            }
            catch (Exception ex)
            {
                if (showErrors)
                    showErrors = MessageBox.Show($"{ex.Message}\n\nDo you want to see further errors?\n\n{ex}", "Project error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes;

                return false;
            }
        }

        protected virtual string GetSerializedOptions()
        {
            return Options.Serialize();
        }

        private static LiteDatabase GetDatabase() => new LiteDatabase(SettingsFilePath);
    }
}
