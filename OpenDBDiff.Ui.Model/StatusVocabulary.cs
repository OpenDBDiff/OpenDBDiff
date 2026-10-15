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
        // The values live in Theme (sections 13.2 and 13.4) rather than here, so the palette the shell's
        // converters read and the palette the XAML tokens define cannot drift apart. These were constants
        // until the dark theme arrived; they are properties now because the answer depends on the theme in
        // force, and the light values are unchanged.
        public static string Background
        {
            get { return Theme.Current.Background; }
        }

        public static string Surface
        {
            get { return Theme.Current.Surface; }
        }

        public static string SurfaceAlt
        {
            get { return Theme.Current.SurfaceAlt; }
        }

        public static string BorderSubtle
        {
            get { return Theme.Current.BorderSubtle; }
        }

        public static string BorderStrong
        {
            get { return Theme.Current.BorderStrong; }
        }

        public static string Text
        {
            get { return Theme.Current.Text; }
        }

        public static string TextMuted
        {
            get { return Theme.Current.TextMuted; }
        }

        public static string TextFaint
        {
            get { return Theme.Current.TextFaint; }
        }

        public static string Accent
        {
            get { return Theme.Current.Accent; }
        }

        public static string AccentBackground
        {
            get { return Theme.Current.AccentBackground; }
        }

        public static string AccentForeground
        {
            get { return Theme.Current.AccentForeground; }
        }

        public static string SelectionBackground
        {
            get { return Theme.Current.SelectionBackground; }
        }

        public static string WarningBackground
        {
            get { return Theme.Current.WarningBackground; }
        }

        public static string WarningForeground
        {
            get { return Theme.Current.WarningForeground; }
        }

        public static string EnvironmentProductionBackground
        {
            get { return Theme.Current.EnvironmentProductionBackground; }
        }

        public static string EnvironmentProductionForeground
        {
            get { return Theme.Current.EnvironmentProductionForeground; }
        }

        public static string Danger
        {
            get { return Theme.Current.Danger; }
        }

        public static string Focus
        {
            get { return Theme.Current.Focus; }
        }

        public static string Highlight
        {
            get { return Theme.Current.Highlight; }
        }

        public static string StatusBackground(DiffStatus status)
        {
            switch (status)
            {
                case DiffStatus.Create: return Theme.Current.CreateBackground;
                case DiffStatus.Alter: return Theme.Current.AlterBackground;
                case DiffStatus.Drop: return Theme.Current.DropBackground;
                default: return SurfaceAlt;
            }
        }

        public static string StatusForeground(DiffStatus status)
        {
            switch (status)
            {
                case DiffStatus.Create: return Theme.Current.CreateForeground;
                case DiffStatus.Alter: return Theme.Current.AlterForeground;
                case DiffStatus.Drop: return Theme.Current.DropForeground;
                default: return TextMuted;
            }
        }

        public static string StatusBorder(DiffStatus status)
        {
            switch (status)
            {
                case DiffStatus.Create: return Theme.Current.CreateBorder;
                case DiffStatus.Alter: return Theme.Current.AlterBorder;
                case DiffStatus.Drop: return Theme.Current.DropBorder;
                default: return BorderSubtle;
            }
        }

        /// <summary>
        /// The tint a diff line is drawn on (sections 6.5.2 and 13.4). It is not the badge tint: in the
        /// dark theme a line is a translucent wash over the surface, which is what lets four rows of code
        /// read as a difference without four blocks of colour in the window.
        /// </summary>
        public static string DiffBackground(DiffLineKind kind)
        {
            return Theme.Current.DiffBackground(kind);
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
