using System;
using OpenDBDiff.Abstractions.Schema;

namespace OpenDBDiff.Ui.Model
{
    /// <summary>
    /// The four words the redesign uses, per section 3 of the specification.
    /// </summary>
    public enum DiffStatus
    {
        /// <summary>Exists in source, not in destination. CREATE.</summary>
        Create,

        /// <summary>Exists in both, definitions differ. ALTER.</summary>
        Alter,

        /// <summary>Exists in destination, not in source. DROP.</summary>
        Drop,

        /// <summary>Exists in both, no difference.</summary>
        Identical
    }

    /// <summary>Why an item is destructive, reported by the engine, never by reading SQL text (section 9.7).</summary>
    [Flags]
    public enum DestructiveKinds
    {
        None = 0,
        DropObject = 1,
        DropColumn = 2,
        DataLossRisk = 4
    }

    public enum Applicability
    {
        Supported,

        /// <summary>Reported by the engine but not scriptable; shown, never included (section 9.5).</summary>
        Unsupported
    }

    public enum ListViewMode
    {
        Grouped,
        Flat
    }

    public enum DetailMode
    {
        SideBySide,
        Inline,
        Script
    }

    /// <summary>Where the application is, per section 4.</summary>
    public enum AppState
    {
        Setup,
        Comparing,
        Results,
        Confirming,
        Applying,
        Report
    }

    /// <summary>
    /// The stable identity of an object, which is what inclusion is remembered by so it survives a
    /// re-compare (section 7.1 rule 5).
    /// </summary>
    public sealed class DiffItemKey : IEquatable<DiffItemKey>
    {
        public DiffItemKey(ObjectType objectType, string schema, string name, string parentName = null)
        {
            ObjectType = objectType;
            Schema = schema ?? string.Empty;
            Name = name ?? string.Empty;
            ParentName = parentName;
        }

        public ObjectType ObjectType { get; }

        public string Schema { get; }

        public string Name { get; }

        /// <summary>Set for table-scoped objects such as triggers and constraints.</summary>
        public string ParentName { get; }

        /// <summary>The schema-qualified name shown in the list.</summary>
        public string FullName
        {
            get { return string.IsNullOrEmpty(Schema) ? Name : Schema + "." + Name; }
        }

        public bool Equals(DiffItemKey other)
        {
            return other != null
                && ObjectType == other.ObjectType
                && string.Equals(Schema, other.Schema, StringComparison.OrdinalIgnoreCase)
                && string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase)
                && string.Equals(ParentName ?? string.Empty, other.ParentName ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DiffItemKey);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = (int)ObjectType;
                hash = (hash * 397) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(Schema);
                hash = (hash * 397) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(Name);
                hash = (hash * 397) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(ParentName ?? string.Empty);
                return hash;
            }
        }

        public override string ToString()
        {
            return FullName;
        }
    }

    /// <summary>
    /// One difference, per section 8.1.
    /// </summary>
    public sealed class DiffItem
    {
        public DiffItem(DiffItemKey key, DiffStatus status)
        {
            Key = key;
            Status = status;
        }

        public DiffItemKey Key { get; }

        public DiffStatus Status { get; }

        public DestructiveKinds DestructiveKinds { get; set; } = DestructiveKinds.None;

        /// <summary>
        /// Drops are destructive by definition; column drops and data-loss risks need the engine and
        /// arrive with the additions in section 14.
        /// </summary>
        public bool IsDestructive
        {
            get { return Status == DiffStatus.Drop || DestructiveKinds != DestructiveKinds.None; }
        }

        /// <summary>Plain-language summary for the detail pane header, e.g. "3 columns added, 1 removed".</summary>
        public string DiffSummary { get; set; } = string.Empty;

        /// <summary>The source definition; null for a Drop.</summary>
        public string SourceDefinition { get; set; }

        /// <summary>The destination definition; null for a Create.</summary>
        public string DestinationDefinition { get; set; }

        /// <summary>The SQL this item would run.</summary>
        public string Script { get; set; } = string.Empty;

        public Applicability Applicability { get; set; } = Applicability.Supported;

        public string UnsupportedReason { get; set; }

        /// <summary>Short list for the Action Bar's destructive count, e.g. "DROP".</summary>
        public string DestructiveDescription
        {
            get
            {
                if (!IsDestructive)
                {
                    return string.Empty;
                }

                return Status == DiffStatus.Drop ? "DROP" : "DROP COLUMN";
            }
        }

        public bool IsSelectable
        {
            get { return Applicability == Applicability.Supported; }
        }
    }
}
