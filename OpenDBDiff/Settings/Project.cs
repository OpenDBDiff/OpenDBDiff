using LiteDB;
using OpenDBDiff.Abstractions.Schema.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OpenDBDiff.SqlServer.Schema.Options;

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

        public Schema.OptionIgnore SqlOptionIgnore { get; set; }
        //public SqlOptionIgnore OptionIgnore { get; set; }

        public Schema.OptionDefault SqlOptionDefault { get; set; }

        public Schema.OptionScript SqlOptionScript { get; set; }

        public Schema.OptionComparison SqlOptionComparison { get; set; }

        public Schema.OptionFilter SqlOptionFilter { get; set; }

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

        public static void SaveLastConfiguration(string connectionStringSource, string connectionStringDestination, IOption option)
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

            //last.OptionIgnore = (SqlOptionIgnore)option.Ignore;

            SqlOptionIgnore Ignore = (SqlOptionIgnore)option.Ignore;
            SqlOptionDefault Default = (SqlOptionDefault)option.Defaults;
            SqlOptionScript Script = (SqlOptionScript)option.Script;
            SqlOptionComparison Comparison = (SqlOptionComparison)option.Comparison;
            SqlOptionFilter Filter = (SqlOptionFilter)option.Filters;

            Schema.Assemblies assemblies = new Schema.Assemblies
            {
                CLRAggregates = Ignore.FilterCLRAggregate,
                CLRFunctions = Ignore.FilterCLRFunction,
                CLRStore = Ignore.FilterCLRStoredProcedure,
                CLRUDT = Ignore.FilterCLRUDT,
                CLRTrigger = Ignore.FilterCLRTrigger
            };
            Schema.Constraints constraints = new Schema.Constraints
            {
                ConstraintPK = Ignore.FilterConstraintPK,
                ConstraintFK = Ignore.FilterConstraintFK,
                ConstraintUK = Ignore.FilterConstraintUK,
                ConstraintCheck = Ignore.FilterConstraintCheck
            };
            Schema.Indexes indexes = new Schema.Indexes
            {
                RowLock = Ignore.FilterIndexRowLock,
                FillFactor = Ignore.FilterIndexFillFactor,
                IncludeColumns = Ignore.FilterIndexIncludeColumns,
                FilterColumns = Ignore.FilterIndexFilter
            };
            Schema.Tables tables = new Schema.Tables
            {
                ColumnCollation = Ignore.FilterColumnCollation,
                ColumnOrder = Ignore.FilterColumnOrder,
                ColumnIdentity = Ignore.FilterColumnIdentity,
                TableOption = Ignore.FilterTableOption,
                LockEscalation = Ignore.FilterTableLockEscalation,
                ChangeTracking = Ignore.FilterTableChangeTracking,
            };

            Schema.OptionIgnore optionIgnore = new Schema.OptionIgnore
            {
                Assemblies = assemblies,
                Constraints = constraints,
                Indexes = indexes,
                Tables = tables,

                Assemblie = Ignore.FilterAssemblies,
                Constraint = Ignore.FilterConstraint,
                Index = Ignore.FilterIndex,
                Table = Ignore.FilterTable,

                TableFileGroup = Ignore.FilterTableFileGroup,
                FullText = Ignore.FilterFullText,
                FullTextPath = Ignore.FilterFullTextPath,
                Users = Ignore.FilterUsers,
                Roles = Ignore.FilterRoles,
                Schema = Ignore.FilterSchema,
                Permission = Ignore.FilterPermission,

                DDLTriggers = Ignore.FilterDDLTriggers,
                Function = Ignore.FilterFunction,
                PartitionFunction = Ignore.FilterPartitionFunction,
                PartitionScheme = Ignore.FilterPartitionScheme,
                Rules = Ignore.FilterRules,
                StoredProcedure = Ignore.FilterStoredProcedure,
                Synonyms = Ignore.FilterSynonyms,
                Trigger = Ignore.FilterTrigger,
                UserDataType = Ignore.FilterUserDataType,
                View = Ignore.FilterView,
                XMLSchema = Ignore.FilterXMLSchema,

                NotForReplication = Ignore.FilterNotForReplication,
                ExtendedProperties = Ignore.FilterExtendedProperties,
            };

            Schema.OptionDefault optionDefault = new Schema.OptionDefault
            {
                DefaultIntegerValue = Default.DefaultIntegerValue,
                DefaultRealValue = Default.DefaultRealValue,
                DefaultTextValue = Default.DefaultTextValue,
                DefaultDateValue = Default.DefaultDateValue,
                DefaultVariantValue = Default.DefaultVariantValue,
                DefaultNTextValue = Default.DefaultNTextValue,
                DefaultBlobValue = Default.DefaultBlobValue,
                DefaultUniqueValue = Default.DefaultUniqueValue,
                UseDefaultValueIfExists = Default.UseDefaultValueIfExists,
                DefaultTime = Default.DefaultTime,
                DefaultXml = Default.DefaultXml
            };

            Schema.OptionScript optionScript = new Schema.OptionScript
            {
                AlterObjectOnSchemaBinding = Script.AlterObjectOnSchemaBinding
            };

            Schema.OptionComparison optionComparison = new Schema.OptionComparison
            {
                IgnoreWhiteSpacesInCode = Comparison.IgnoreWhiteSpacesInCode,
                ReloadComparisonOnUpdate = Comparison.ReloadComparisonOnUpdate,
                CaseSensityInCode = Comparison.CaseSensityInCode,
                CaseSensityType = Comparison.CaseSensityType
            };

            Schema.OptionFilter optionFilter = new Schema.OptionFilter
            {
                Items = Filter.Items
            };

            last.SqlOptionIgnore = optionIgnore;
            last.SqlOptionDefault = optionDefault;
            last.SqlOptionScript = optionScript;
            last.SqlOptionComparison = optionComparison;
            last.SqlOptionFilter = optionFilter;

            //last.Options = option;

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
                    showErrors = MessageBox.Show($"{ex.Message}\n\nDo you want to see further errors?\n\n{ex.ToString()}", "Project error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes;

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
