using System;
using System.Collections.Generic;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

namespace OpenDBDiff.Ui.Model
{
    /// <summary>A run of characters within one line, tinted when it differs (section 6.5.3).</summary>
    public sealed class WordRun
    {
        public WordRun(string text, bool isChanged)
            : this(text, isChanged, DiffLineKind.Unchanged)
        {
        }

        public WordRun(string text, bool isChanged, DiffLineKind kind)
        {
            Text = text;
            IsChanged = isChanged;
            Kind = kind;
        }

        public string Text { get; }

        public bool IsChanged { get; }

        /// <summary>
        /// The kind of line this run belongs to. Word diffing does not know the line's status - it only
        /// compares two strings - so the side-by-side row stamps it on the way out, which is what lets
        /// the view pick the right tint without reaching back up the tree.
        /// </summary>
        public DiffLineKind Kind { get; }
    }

    /// <summary>Both sides of one changed line, split into same and different runs.</summary>
    public sealed class WordDiffPair
    {
        public WordDiffPair(IReadOnlyList<WordRun> left, IReadOnlyList<WordRun> right)
        {
            Left = left;
            Right = right;
        }

        public IReadOnlyList<WordRun> Left { get; }

        public IReadOnlyList<WordRun> Right { get; }
    }

    /// <summary>
    /// The diff navigation of section 6.5.3: F7 and Shift+F7 jump to the next and previous change hunk,
    /// and the changed words within a changed line are tinted more strongly than the line.
    /// </summary>
    public static class HunkRules
    {
        /// <summary>A run is a hunk when it is a change and the row before it is not.</summary>
        public static IReadOnlyList<int> HunkStarts(IReadOnlyList<SideBySideRow> rows)
        {
            var starts = new List<int>();

            if (rows == null)
            {
                return starts;
            }

            for (var i = 0; i < rows.Count; i++)
            {
                if (!rows[i].IsChange)
                {
                    continue;
                }

                if (i == 0 || !rows[i - 1].IsChange)
                {
                    starts.Add(i);
                }
            }

            return starts;
        }

        public static IReadOnlyList<int> HunkStarts(IReadOnlyList<InlineRow> rows)
        {
            var starts = new List<int>();

            if (rows == null)
            {
                return starts;
            }

            for (var i = 0; i < rows.Count; i++)
            {
                if (rows[i].Kind == DiffLineKind.Unchanged)
                {
                    continue;
                }

                if (i == 0 || rows[i - 1].Kind == DiffLineKind.Unchanged)
                {
                    starts.Add(i);
                }
            }

            return starts;
        }

        /// <summary>
        /// The next hunk after <paramref name="current"/>, wrapping to the first; -1 when there is none.
        /// </summary>
        public static int Next(IReadOnlyList<int> starts, int current)
        {
            if (starts == null || starts.Count == 0)
            {
                return -1;
            }

            foreach (var start in starts)
            {
                if (start > current)
                {
                    return start;
                }
            }

            return starts[0];
        }

        /// <summary>The previous hunk before <paramref name="current"/>, wrapping to the last.</summary>
        public static int Previous(IReadOnlyList<int> starts, int current)
        {
            if (starts == null || starts.Count == 0)
            {
                return -1;
            }

            for (var i = starts.Count - 1; i >= 0; i--)
            {
                if (starts[i] < current)
                {
                    return starts[i];
                }
            }

            return starts[starts.Count - 1];
        }

        /// <summary>
        /// Word-level tinting (section 6.5.3): the changed words inside a changed line are marked, so a
        /// one-character edit does not look like a rewritten line.
        /// </summary>
        public static WordDiffPair Words(string left, string right)
        {
            var builder = new SideBySideDiffBuilder(new Differ());
            var model = builder.BuildDiffModel(left ?? string.Empty, right ?? string.Empty);

            var leftRuns = Runs(model.OldText.Lines.Count > 0 ? model.OldText.Lines[0] : null);
            var rightRuns = Runs(model.NewText.Lines.Count > 0 ? model.NewText.Lines[0] : null);

            return new WordDiffPair(leftRuns, rightRuns);
        }

        private static IReadOnlyList<WordRun> Runs(DiffPiece line)
        {
            var runs = new List<WordRun>();

            if (line == null)
            {
                return runs;
            }

            // A line the differ found nothing to change in has no sub-pieces at all, and the view still
            // needs its text: an unchanged run is the honest answer and keeps the two sides reconstructing.
            if (line.SubPieces == null || line.SubPieces.Count == 0)
            {
                runs.Add(new WordRun(line.Text ?? string.Empty, false));
                return runs;
            }

            foreach (var piece in line.SubPieces)
            {
                if (piece.Type == ChangeType.Imaginary)
                {
                    continue;
                }

                var text = piece.Text ?? string.Empty;
                var changed = piece.Type != ChangeType.Unchanged;

                if (runs.Count > 0 && runs[runs.Count - 1].IsChanged == changed)
                {
                    var last = runs[runs.Count - 1];
                    runs[runs.Count - 1] = new WordRun(last.Text + text, changed);
                }
                else
                {
                    runs.Add(new WordRun(text, changed));
                }
            }

            return runs;
        }
    }
}
