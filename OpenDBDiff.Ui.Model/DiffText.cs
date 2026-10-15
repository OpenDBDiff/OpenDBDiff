using System;
using System.Text;
using System.Collections.Generic;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

namespace OpenDBDiff.Ui.Model
{
    public enum DiffLineKind
    {
        Unchanged,

        /// <summary>Present only in the source, so it will be created.</summary>
        Added,

        /// <summary>Present only in the destination, so it will be dropped.</summary>
        Removed,

        /// <summary>Present on both sides with different text.</summary>
        Changed,

        /// <summary>Blank filler inserted so the two sides stay aligned (hatched in the design).</summary>
        Filler
    }

    /// <summary>One aligned row of the side-by-side view: left is the source, right the destination.</summary>
    public sealed class SideBySideRow
    {
        private WordDiffPair pair;
        private bool pairComputed;

        public SideBySideRow(string left, string right, DiffLineKind leftKind, DiffLineKind rightKind, int? leftNumber, int? rightNumber)
        {
            Left = left;
            Right = right;
            LeftKind = leftKind;
            RightKind = rightKind;
            LeftLineNumber = leftNumber;
            RightLineNumber = rightNumber;
        }

        public string Left { get; }

        public string Right { get; }

        public DiffLineKind LeftKind { get; }

        public DiffLineKind RightKind { get; }

        public int? LeftLineNumber { get; }

        public int? RightLineNumber { get; }

        /// <summary>
        /// The left line split into the runs that changed and the runs that did not (section 6.5.3), so
        /// a one-word edit does not look like a rewritten line. A line that is not a change is one
        /// unchanged run, and the diff is only computed for the rows that need it.
        /// </summary>
        public IReadOnlyList<WordRun> LeftRuns
        {
            get { return LeftKind == DiffLineKind.Changed ? Stamp(Pair.Left, LeftKind) : Single(Left, LeftKind); }
        }

        public IReadOnlyList<WordRun> RightRuns
        {
            get { return RightKind == DiffLineKind.Changed ? Stamp(Pair.Right, RightKind) : Single(Right, RightKind); }
        }

        public bool IsChange
        {
            get
            {
                return LeftKind == DiffLineKind.Added
                    || LeftKind == DiffLineKind.Removed
                    || LeftKind == DiffLineKind.Changed
                    || RightKind == DiffLineKind.Added
                    || RightKind == DiffLineKind.Removed
                    || RightKind == DiffLineKind.Changed;
            }
        }

        private WordDiffPair Pair
        {
            get
            {
                if (!pairComputed)
                {
                    pair = HunkRules.Words(Left, Right);
                    pairComputed = true;
                }

                return pair;
            }
        }

        private static IReadOnlyList<WordRun> Single(string text, DiffLineKind kind)
        {
            return new List<WordRun> { new WordRun(text ?? string.Empty, false, kind) };
        }

        private static IReadOnlyList<WordRun> Stamp(IReadOnlyList<WordRun> runs, DiffLineKind kind)
        {
            var stamped = new List<WordRun>(runs.Count);

            foreach (var run in runs)
            {
                stamped.Add(new WordRun(run.Text, run.IsChanged, kind));
            }

            return stamped;
        }
    }

    public sealed class InlineRow
    {
        public InlineRow(DiffLineKind kind, string prefix, string text)
        {
            Kind = kind;
            Prefix = prefix;
            Text = text;
        }

        public DiffLineKind Kind { get; }

        public string Prefix { get; }

        public string Text { get; }

        public string Display
        {
            get { return Prefix + Text; }
        }
    }

    /// <summary>
    /// The diff itself: alignment for the side-by-side view, prefixes for the inline view, and the
    /// plain-language summary the detail pane header shows (sections 6.5.2, 6.5.3).
    /// </summary>
    /// <remarks>
    /// This is deliberately not in either front end: both shells need the same alignment, and aligning
    /// two definitions is exactly the kind of thing that should be tested rather than eyeballed.
    /// </remarks>
    public static class DiffText
    {
        /// <summary>
        /// The aligned rows of section 6.5.2. Section 11's "Ignore whitespace in diff" needs nothing here:
        /// the line differ already treats a row whose sides differ only in spacing as unchanged, which the
        /// tests pin so a package bump cannot change it quietly.
        /// </summary>
        public static IReadOnlyList<SideBySideRow> SideBySide(string sourceDefinition, string destinationDefinition)
        {
            var model = new SideBySideDiffBuilder(new Differ())
                .BuildDiffModel(destinationDefinition ?? string.Empty, sourceDefinition ?? string.Empty);

            var rows = new List<SideBySideRow>();
            var count = Math.Max(model.NewText.Lines.Count, model.OldText.Lines.Count);
            var leftNumber = 1;
            var rightNumber = 1;

            for (var i = 0; i < count; i++)
            {
                var source = i < model.NewText.Lines.Count ? model.NewText.Lines[i] : null;
                var destination = i < model.OldText.Lines.Count ? model.OldText.Lines[i] : null;

                var left = TextOf(source);
                var right = TextOf(destination);

                if (left == null && right == null)
                {
                    continue;
                }

                var leftKind = KindOf(source, right == null && left != null, destination);
                var rightKind = KindOf(destination, left == null && right != null, source);

                if (source != null && destination != null
                    && source.Type == ChangeType.Modified && destination.Type == ChangeType.Modified)
                {
                    leftKind = DiffLineKind.Changed;
                    rightKind = DiffLineKind.Changed;
                }

                rows.Add(new SideBySideRow(
                    left,
                    right,
                    leftKind,
                    rightKind,
                    left == null ? (int?)null : leftNumber++,
                    right == null ? (int?)null : rightNumber++));
            }

            return rows;
        }

        public static IReadOnlyList<InlineRow> Inline(string sourceDefinition, string destinationDefinition)
        {
            return Inline(sourceDefinition, destinationDefinition, false);
        }

        public static IReadOnlyList<InlineRow> Inline(string sourceDefinition, string destinationDefinition, bool ignoreWhitespace)
        {
            var model = new InlineDiffBuilder(new Differ())
                .BuildDiffModel(destinationDefinition ?? string.Empty, sourceDefinition ?? string.Empty);

            var rows = new List<InlineRow>();

            foreach (var line in model.Lines)
            {
                switch (line.Type)
                {
                    case ChangeType.Inserted:
                        rows.Add(new InlineRow(DiffLineKind.Added, "+ ", line.Text));
                        break;
                    case ChangeType.Deleted:
                        rows.Add(new InlineRow(DiffLineKind.Removed, "- ", line.Text));
                        break;
                    case ChangeType.Modified:
                        rows.Add(new InlineRow(DiffLineKind.Changed, "~ ", line.Text));
                        break;
                    default:
                        rows.Add(new InlineRow(DiffLineKind.Unchanged, "  ", line.Text));
                        break;
                }
            }

            return ignoreWhitespace ? WhitespaceInsensitive(rows) : rows;
        }

        /// <summary>
        /// The header's plain-language summary. The engine will supply this in time (section 14); until
        /// then it is derived from the diff, which is honest and never claims more than it knows.
        /// </summary>
        public static string Summary(DiffStatus status, string sourceDefinition, string destinationDefinition)
        {
            return Summary(status, sourceDefinition, destinationDefinition, false);
        }

        public static string Summary(DiffStatus status, string sourceDefinition, string destinationDefinition, bool ignoreWhitespace)
        {
            switch (status)
            {
                case DiffStatus.Create:
                    return "Whole object is new";
                case DiffStatus.Drop:
                    return "Whole object is only in the destination";
                case DiffStatus.Identical:
                    return "No differences";
            }

            var lines = Inline(sourceDefinition, destinationDefinition, ignoreWhitespace);
            var added = 0;
            var removed = 0;

            foreach (var line in lines)
            {
                if (line.Kind == DiffLineKind.Added)
                {
                    added++;
                }
                else if (line.Kind == DiffLineKind.Removed)
                {
                    removed++;
                }
            }

            if (added == 0 && removed == 0)
            {
                // With the option on, a change that is spacing and nothing else is worth naming rather
                // than leaving as "differs": it is exactly what the reader asked to be told about.
                var onlySpacing = ignoreWhitespace
                    && !string.Equals(sourceDefinition ?? string.Empty, destinationDefinition ?? string.Empty, StringComparison.Ordinal);

                return onlySpacing ? "Only whitespace differs" : "Definitions differ";
            }

            if (added > 0 && removed > 0)
            {
                return string.Format("{0} lines added, {1} removed", added, removed);
            }

            return added > 0
                ? (added == 1 ? "1 line added" : added + " lines added")
                : (removed == 1 ? "1 line removed" : removed + " lines removed");
        }

        /// <summary>
        /// Section 11's whitespace-insensitive view of a line: every run of whitespace becomes one space
        /// and the ends are trimmed. Two lines that reduce to the same string differ only in spacing.
        /// </summary>
        public static string Normalize(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return string.Empty;
            }

            var builder = new StringBuilder(line.Length);
            var pendingSpace = false;

            foreach (var character in line)
            {
                if (char.IsWhiteSpace(character))
                {
                    pendingSpace = builder.Length > 0;
                    continue;
                }

                if (pendingSpace)
                {
                    builder.Append(' ');
                    pendingSpace = false;
                }

                builder.Append(character);
            }

            return builder.ToString();
        }

        public static bool SameIgnoringWhitespace(string left, string right)
        {
            return string.Equals(Normalize(left), Normalize(right), StringComparison.Ordinal);
        }

        /// <summary>
        /// Section 11's "Ignore whitespace in diff" in the inline view. The line differ reports a
        /// spacing-only change as a removed line and an added one - so the inline view shows a change
        /// where the side-by-side view shows none, and this is the option that makes the two agree. Both
        /// orders are recognised, because the pair's order depends on which side the line came from, and
        /// both collapse to one unchanged line: the source's, because the source is the state the script
        /// is bringing.
        /// </summary>
        private static IReadOnlyList<InlineRow> WhitespaceInsensitive(IReadOnlyList<InlineRow> rows)
        {
            var relaxed = new List<InlineRow>(rows.Count);

            for (var i = 0; i < rows.Count; i++)
            {
                var row = rows[i];

                if (i + 1 < rows.Count && Pairs(row, rows[i + 1]))
                {
                    var text = row.Kind == DiffLineKind.Added ? row.Text : rows[i + 1].Text;

                    relaxed.Add(new InlineRow(DiffLineKind.Unchanged, "  ", text));
                    i++;
                    continue;
                }

                relaxed.Add(row);
            }

            return relaxed;
        }

        private static bool Pairs(InlineRow first, InlineRow second)
        {
            var opposed = (first.Kind == DiffLineKind.Removed && second.Kind == DiffLineKind.Added)
                || (first.Kind == DiffLineKind.Added && second.Kind == DiffLineKind.Removed);

            return opposed && SameIgnoringWhitespace(first.Text, second.Text);
        }

        private static string TextOf(DiffPiece piece)
        {
            if (piece == null || piece.Type == ChangeType.Imaginary)
            {
                return null;
            }

            return piece.Text ?? string.Empty;
        }

        private static DiffLineKind KindOf(DiffPiece piece, bool aloneOnThisSide, DiffPiece otherSide)
        {
            if (piece == null || piece.Type == ChangeType.Imaginary)
            {
                return DiffLineKind.Filler;
            }

            switch (piece.Type)
            {
                case ChangeType.Inserted:
                    return DiffLineKind.Added;
                case ChangeType.Deleted:
                    return DiffLineKind.Removed;
                case ChangeType.Modified:
                    return DiffLineKind.Changed;
                default:
                    // A line that exists on this side only, because the other side is filler.
                    return aloneOnThisSide ? DiffLineKind.Changed : DiffLineKind.Unchanged;
            }
        }
    }
}
