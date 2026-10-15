using System.Collections.Generic;

namespace OpenDBDiff.Ui.Model
{
    /// <summary>
    /// One marked stretch of a diff's minimap (section 6.5): where it starts and how tall it is, both as a
    /// fraction of the whole diff, and what kind of change it is.
    /// </summary>
    /// <remarks>
    /// Fractions rather than pixels, because the strip has to sit beside a diff of any height and the model
    /// has no idea how tall that is. The view scales; the model decides which rows are a change.
    /// </remarks>
    public sealed class MapBand
    {
        public MapBand(double top, double height, DiffLineKind kind, int firstRow, int lastRow)
        {
            Top = top;
            Height = height;
            Kind = kind;
            FirstRow = firstRow;
            LastRow = lastRow;
        }

        /// <summary>The top edge, 0 at the first row.</summary>
        public double Top { get; }

        public double Height { get; }

        public DiffLineKind Kind { get; }

        /// <summary>The first and last row the band covers, so a reader can be told where it is.</summary>
        public int FirstRow { get; }

        public int LastRow { get; }

        /// <summary>
        /// What the band says when a reader stops on it. The rows are rows of the diff, not line numbers of
        /// either file, and the words say so rather than promising a line number the strip cannot know.
        /// </summary>
        public string Tooltip
        {
            get
            {
                var what = Kind == DiffLineKind.Added ? "Added" : Kind == DiffLineKind.Removed ? "Removed" : "Changed";
                var where = FirstRow == LastRow
                    ? string.Format("row {0}", FirstRow + 1)
                    : string.Format("rows {0}-{1}", FirstRow + 1, LastRow + 1);

                return string.Format("{0} · {1}", what, where);
            }
        }
    }

    /// <summary>
    /// The minimap of section 6.5: "a minimap strip (12 wide) at the right edge of the diff shows hunk
    /// positions".
    /// </summary>
    /// <remarks>
    /// It is a rule rather than a view because what counts as a change is already a rule, and because the
    /// arithmetic - consecutive changed rows become one band, and every band is a fraction of the same
    /// whole - is the kind of thing that is wrong in a way nobody notices until a hundred-row file has a
    /// two-pixel band where a scrollbar should be.
    /// </remarks>
    public static class DiffMap
    {
        /// <summary>
        /// The bands for a list of per-row kinds, in row order. Rows that are not changes produce no band,
        /// and adjacent changes of the same kind merge into one, so the strip shows hunks rather than rows.
        /// </summary>
        public static IReadOnlyList<MapBand> Bands(IReadOnlyList<DiffLineKind> kinds)
        {
            var bands = new List<MapBand>();

            if (kinds == null || kinds.Count == 0)
            {
                return bands;
            }

            var total = kinds.Count;
            var start = -1;
            var kind = DiffLineKind.Unchanged;

            for (var i = 0; i <= total; i++)
            {
                var current = i < total ? ChangeOf(kinds[i]) : DiffLineKind.Unchanged;

                if (current == DiffLineKind.Unchanged)
                {
                    if (start >= 0)
                    {
                        bands.Add(new MapBand((double)start / total, (double)(i - start) / total, kind, start, i - 1));
                        start = -1;
                    }

                    continue;
                }

                if (start < 0)
                {
                    start = i;
                    kind = current;
                    continue;
                }

                if (current != kind)
                {
                    bands.Add(new MapBand((double)start / total, (double)(i - start) / total, kind, start, i - 1));
                    start = i;
                    kind = current;
                }
            }

            return bands;
        }

        /// <summary>
        /// The bands for a side-by-side diff, where each row has two sides. A line only on the source side is
        /// added, only on the destination side is removed, and one that differs on both is changed; where a
        /// row somehow carries two kinds the added one wins, because a line that exists nowhere else is the
        /// more alarming of the two.
        /// </summary>
        public static IReadOnlyList<MapBand> Bands(IReadOnlyList<SideBySideRow> rows)
        {
            if (rows == null)
            {
                return new List<MapBand>();
            }

            var kinds = new List<DiffLineKind>(rows.Count);

            foreach (var row in rows)
            {
                kinds.Add(Strongest(row.LeftKind, row.RightKind));
            }

            return Bands(kinds);
        }

        /// <summary>
        /// The bands for the inline diff. Its rows are one kind each, but a modified line arrives there as a
        /// removed row and an added one - so a single changed line would otherwise draw as a red band and a
        /// green one, and the strip would claim two hunks where a reader sees one. The pair is merged back
        /// into one band, which is the same pairing section 6.5.3's whitespace option makes.
        /// </summary>
        public static IReadOnlyList<MapBand> Bands(IReadOnlyList<InlineRow> rows)
        {
            if (rows == null)
            {
                return new List<MapBand>();
            }

            var kinds = new List<DiffLineKind>(rows.Count);

            foreach (var row in rows)
            {
                kinds.Add(row.Kind);
            }

            return MergePairs(Bands(kinds));
        }

        /// <summary>A removed row immediately followed by an added one is one changed hunk, not two.</summary>
        private static IReadOnlyList<MapBand> MergePairs(IReadOnlyList<MapBand> bands)
        {
            var merged = new List<MapBand>(bands.Count);

            for (var i = 0; i < bands.Count; i++)
            {
                var band = bands[i];

                if (i + 1 < bands.Count && Opposed(band, bands[i + 1]))
                {
                    var next = bands[i + 1];
                    var bottom = next.Top + next.Height;

                    merged.Add(new MapBand(band.Top, bottom - band.Top, DiffLineKind.Changed, band.FirstRow, next.LastRow));
                    i++;
                    continue;
                }

                merged.Add(band);
            }

            return merged;
        }

        private static bool Opposed(MapBand first, MapBand second)
        {
            return (first.Kind == DiffLineKind.Removed && second.Kind == DiffLineKind.Added)
                || (first.Kind == DiffLineKind.Added && second.Kind == DiffLineKind.Removed);
        }

        private static DiffLineKind Strongest(DiffLineKind left, DiffLineKind right)
        {
            var leftChange = ChangeOf(left);
            var rightChange = ChangeOf(right);

            if (leftChange == DiffLineKind.Added || rightChange == DiffLineKind.Added)
            {
                return DiffLineKind.Added;
            }

            if (leftChange == DiffLineKind.Removed || rightChange == DiffLineKind.Removed)
            {
                return DiffLineKind.Removed;
            }

            return leftChange == DiffLineKind.Changed || rightChange == DiffLineKind.Changed
                ? DiffLineKind.Changed
                : DiffLineKind.Unchanged;
        }

        /// <summary>Filler and unchanged rows are not changes; the rest are, as they are drawn.</summary>
        private static DiffLineKind ChangeOf(DiffLineKind kind)
        {
            switch (kind)
            {
                case DiffLineKind.Added:
                case DiffLineKind.Removed:
                case DiffLineKind.Changed:
                    return kind;
                default:
                    return DiffLineKind.Unchanged;
            }
        }
    }
}
