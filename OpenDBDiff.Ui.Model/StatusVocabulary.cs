using System.Collections.Generic;
using OpenDBDiff.Abstractions.Schema;

namespace OpenDBDiff.Ui.Model
{
    /// <summary>
    /// The words and colours of the status model (sections 3 and 13.2).
    /// </summary>
    /// <remarks>
    /// Kept here rather than in either front end so the two shells cannot drift apart on vocabulary,
    /// and so the colour tokens can be asserted against the specification in a test.
    /// </remarks>
    public static class StatusVocabulary
    {
        /// <summary>The SQL verb: what the script will do.</summary>
        public static string Term(DiffStatus status)
        {
            switch (status)
            {
                case DiffStatus.Create: return "Create";
                case DiffStatus.Alter: return "Alter";
                case DiffStatus.Drop: return "Drop";
                default: return "Identical";
            }
        }

        /// <summary>The badge text: uppercase SQL verb.</summary>
        public static string Badge(DiffStatus status)
        {
            return Term(status).ToUpperInvariant();
        }

        /// <summary>
        /// The plain-language label the summary chips use, for readers who are not DBAs. The mapping
        /// is stated in the chip's tooltip (section 3).
        /// </summary>
        public static string PlainLanguage(DiffStatus status)
        {
            switch (status)
            {
                case DiffStatus.Create: return "New";
                case DiffStatus.Alter: return "Changed";
                case DiffStatus.Drop: return "Missing";
                default: return "Identical";
            }
        }

        /// <summary>The tooltip that states the plain-language to SQL mapping.</summary>
        public static string ChipTooltip(DiffStatus status)
        {
            switch (status)
            {
                case DiffStatus.Create:
                    return "New: objects in the source that are missing from the destination (will be created).";
                case DiffStatus.Alter:
                    return "Changed: objects that exist in both, with different definitions (will be altered).";
                case DiffStatus.Drop:
                    return "Missing: objects in the destination that are not in the source (will be dropped).";
                default:
                    return "Identical: objects that exist in both with no difference.";
            }
        }
    }

    /// <summary>
    /// The colour tokens of section 13.2, as hex strings so no drawing type is needed. A test checks
    /// them against the specification's values.
    /// </summary>
    public static class StatusPalette
    {
        public const string Background = "#F4F5F7";
        public const string Surface = "#FFFFFF";
        public const string SurfaceAlt = "#EEF0F4";
        public const string BorderSubtle = "#DDE2EA";
        public const string BorderStrong = "#CBD2DE";
        public const string Text = "#1B2230";
        public const string TextMuted = "#5B6577";
        public const string TextFaint = "#8A93A5";
        public const string Accent = "#2F5BEA";
        public const string AccentBackground = "#E5ECFF";
        public const string AccentForeground = "#1F4FD1";
        public const string SelectionBackground = "#EEF3FF";
        public const string WarningBackground = "#FFF8E6";
        public const string WarningForeground = "#7A4A00";
        public const string EnvironmentProductionBackground = "#FEF0C7";
        public const string EnvironmentProductionForeground = "#93370D";
        public const string Danger = "#B42318";
        public const string Focus = "#1F4FD1";
        public const string Highlight = "#FFE58F";

        public static string StatusBackground(DiffStatus status)
        {
            switch (status)
            {
                case DiffStatus.Create: return "#E3F4EA";
                case DiffStatus.Alter: return "#E5ECFF";
                case DiffStatus.Drop: return "#FDE8E6";
                default: return SurfaceAlt;
            }
        }

        public static string StatusForeground(DiffStatus status)
        {
            switch (status)
            {
                case DiffStatus.Create: return "#0F6B38";
                case DiffStatus.Alter: return "#1F4FD1";
                case DiffStatus.Drop: return "#A5210F";
                default: return TextMuted;
            }
        }

        public static string StatusBorder(DiffStatus status)
        {
            switch (status)
            {
                case DiffStatus.Create: return "#9BD3B4";
                case DiffStatus.Alter: return "#A9BDF5";
                case DiffStatus.Drop: return "#F2A9A0";
                default: return BorderSubtle;
            }
        }
    }

    /// <summary>
    /// The fixed, DBA-friendly group order of section 6.4.2. Types outside the list sort last, by name,
    /// so a new object type still appears.
    /// </summary>
    public static class TypeOrder
    {
        private static readonly ObjectType[] Order =
        {
            ObjectType.Schema,
            ObjectType.Table,
            ObjectType.View,
            ObjectType.StoredProcedure,
            ObjectType.CLRStoredProcedure,
            ObjectType.Function,
            ObjectType.CLRFunction,
            ObjectType.Trigger,
            ObjectType.CLRTrigger,
            ObjectType.UserDataType,
            ObjectType.TableType,
            ObjectType.Synonym,
            ObjectType.Default,
            ObjectType.Rule,
            ObjectType.PartitionFunction,
            ObjectType.PartitionScheme,
            ObjectType.Partition,
            ObjectType.FullText,
            ObjectType.FullTextIndex,
            ObjectType.FileGroup,
            ObjectType.File,
            ObjectType.Assembly,
            ObjectType.AssemblyFile,
            ObjectType.XMLSchema,
            ObjectType.Role,
            ObjectType.User,
            ObjectType.Permission
        };

        private static readonly Dictionary<ObjectType, int> Positions = BuildPositions();

        public static int Of(ObjectType objectType)
        {
            return Positions.TryGetValue(objectType, out var position) ? position : int.MaxValue;
        }

        private static Dictionary<ObjectType, int> BuildPositions()
        {
            var positions = new Dictionary<ObjectType, int>();

            for (var i = 0; i < Order.Length; i++)
            {
                positions[Order[i]] = i;
            }

            return positions;
        }
    }
}
