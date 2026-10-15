using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Ui.Model
{
    /// <summary>
    /// The diff alignment and the summary the detail pane header shows (sections 6.5.2, 6.5.3).
    /// </summary>
    [TestClass]
    public class DiffTextTests
    {
        [TestMethod]
        public void UnchangedDefinitions_AlignLineForLine()
        {
            var rows = DiffText.SideBySide("SELECT 1\nSELECT 2", "SELECT 1\nSELECT 2");

            Assert.AreEqual(2, rows.Count);
            Assert.IsTrue(rows.All(row => row.LeftKind == DiffLineKind.Unchanged && row.RightKind == DiffLineKind.Unchanged));
            Assert.IsTrue(rows.All(row => !row.IsChange));
        }

        /// <summary>
        /// A line that exists only in the source leaves a filler on the destination side, which is
        /// what keeps the two columns aligned.
        /// </summary>
        [TestMethod]
        public void AddedLine_LeavesAFillerOnTheDestinationSide()
        {
            var rows = DiffText.SideBySide("SELECT 1\nSELECT 2", "SELECT 1");

            var added = rows.Single(row => row.IsChange);

            Assert.AreEqual(DiffLineKind.Added, added.LeftKind);
            Assert.AreEqual("SELECT 2", added.Left);
            Assert.AreEqual(DiffLineKind.Filler, added.RightKind);
            Assert.IsNull(added.Right);
        }

        [TestMethod]
        public void RemovedLine_LeavesAFillerOnTheSourceSide()
        {
            var rows = DiffText.SideBySide("SELECT 1", "SELECT 1\nSELECT 2");

            var removed = rows.Single(row => row.IsChange);

            Assert.AreEqual(DiffLineKind.Removed, removed.RightKind);
            Assert.AreEqual("SELECT 2", removed.Right);
            Assert.AreEqual(DiffLineKind.Filler, removed.LeftKind);
        }

        [TestMethod]
        public void ChangedLine_IsTintedOnBothSides()
        {
            var rows = DiffText.SideBySide("SELECT Name FROM Users", "SELECT Name FROM Customer");

            var changed = rows.Single(row => row.IsChange);

            Assert.AreEqual(DiffLineKind.Changed, changed.LeftKind);
            Assert.AreEqual(DiffLineKind.Changed, changed.RightKind);
        }

        [TestMethod]
        public void LineNumbersSkipFillerCells()
        {
            var rows = DiffText.SideBySide("SELECT 1\nSELECT 2", "SELECT 1");

            Assert.AreEqual(1, rows[0].LeftLineNumber);
            Assert.AreEqual(1, rows[0].RightLineNumber);
            Assert.AreEqual(2, rows[1].LeftLineNumber);
            Assert.IsNull(rows[1].RightLineNumber, "the destination side has no second line");
        }

        [TestMethod]
        public void InlineViewPrefixesTheChangedLines()
        {
            var rows = DiffText.Inline("SELECT 1\nSELECT 2", "SELECT 1");

            Assert.AreEqual(DiffLineKind.Unchanged, rows[0].Kind);
            Assert.AreEqual("  SELECT 1", rows[0].Display);

            var added = rows.Single(row => row.Kind == DiffLineKind.Added);
            Assert.AreEqual("+ SELECT 2", added.Display);
        }

        [TestMethod]
        public void SummaryDescribesWholeObjectChangesInWords()
        {
            Assert.AreEqual("Whole object is new", DiffText.Summary(DiffStatus.Create, "SELECT 1", null));
            Assert.AreEqual("Whole object is only in the destination", DiffText.Summary(DiffStatus.Drop, null, "SELECT 1"));
            Assert.AreEqual("No differences", DiffText.Summary(DiffStatus.Identical, "SELECT 1", "SELECT 1"));
        }

        [TestMethod]
        public void SummaryCountsLinesWhenThereAreSome()
        {
            Assert.AreEqual("1 line added", DiffText.Summary(DiffStatus.Alter, "SELECT 1\nSELECT 2", "SELECT 1"));
            Assert.AreEqual("1 line removed", DiffText.Summary(DiffStatus.Alter, "SELECT 1", "SELECT 1\nSELECT 2"));
            Assert.AreEqual("1 lines added, 1 removed",
                DiffText.Summary(DiffStatus.Alter, "SELECT 1\nSELECT 2", "SELECT 1\nSELECT 3"));
        }

        [TestMethod]
        public void SummaryNeverClaimsToKnowMoreThanTheDiffDoes()
        {
            // Identical text but reported as an Alter: say only that the definitions differ.
            Assert.AreEqual("Definitions differ", DiffText.Summary(DiffStatus.Alter, "SELECT 1", "SELECT 1"));
        }

        /// <summary>
        /// Section 6.5.3: the changed words inside a changed line are marked, so an edit to one word does
        /// not look like a rewritten line. The unchanged parts of the line stay as their own runs.
        /// </summary>
        [TestMethod]
        public void ChangedLine_SplitsIntoChangedAndUnchangedRuns()
        {
            var rows = DiffText.SideBySide("SELECT a, b FROM T", "SELECT a, c FROM T");

            var changed = rows.First(row => row.LeftKind == DiffLineKind.Changed);

            Assert.IsTrue(changed.LeftRuns.Any(run => run.IsChanged), "the word that differs is a changed run");
            Assert.IsTrue(changed.LeftRuns.Any(run => !run.IsChanged), "the words around it are not");

            Assert.AreEqual("SELECT a, b FROM T", string.Concat(changed.LeftRuns.Select(run => run.Text)),
                "the runs reconstruct the original line exactly");
            Assert.AreEqual("SELECT a, c FROM T", string.Concat(changed.RightRuns.Select(run => run.Text)));
        }

        /// <summary>
        /// Word diffing only compares two strings, so it cannot know whether the line was added, removed
        /// or altered. The row stamps its own kind onto the runs, which is what the view tints by.
        /// </summary>
        [TestMethod]
        public void WordRuns_CarryTheKindOfTheirLine()
        {
            var rows = DiffText.SideBySide("SELECT a, b FROM T", "SELECT a, c FROM T");
            var changed = rows.First(row => row.LeftKind == DiffLineKind.Changed);

            Assert.IsTrue(changed.LeftRuns.All(run => run.Kind == DiffLineKind.Changed));
            Assert.IsTrue(changed.RightRuns.All(run => run.Kind == DiffLineKind.Changed));
        }

        [TestMethod]
        public void UnchangedLine_IsOneUnchangedRun()
        {
            var rows = DiffText.SideBySide("SELECT 1", "SELECT 1");

            var run = rows[0].LeftRuns.Single();

            Assert.AreEqual("SELECT 1", run.Text);
            Assert.IsFalse(run.IsChanged);
            Assert.AreEqual(DiffLineKind.Unchanged, run.Kind);
        }

        [TestMethod]
        public void FillerLine_IsOneUnchangedRunSoBothSidesStillReconstruct()
        {
            var rows = DiffText.SideBySide("SELECT 1\nSELECT 2", "SELECT 1");

            var filler = rows.First(row => row.RightKind == DiffLineKind.Filler);

            Assert.AreEqual("SELECT 2", string.Concat(filler.LeftRuns.Select(run => run.Text)));
            Assert.AreEqual(string.Empty, string.Concat(filler.RightRuns.Select(run => run.Text)));
        }

        /// <summary>
        /// The side-by-side view already shows a spacing-only difference as unchanged, which is why
        /// section 11's option has nothing to do there. Pinned on purpose: it is the library's behaviour,
        /// and a package bump that changed it would change what the diff viewer reports to a reader.
        /// </summary>
        [TestMethod]
        public void TheSideBySideViewTreatsASpacingOnlyChangeAsUnchanged()
        {
            var rows = DiffText.SideBySide("SELECT a,  b FROM T", "SELECT a, b FROM T");

            Assert.IsFalse(rows.Any(row => row.IsChange));
            Assert.AreEqual("SELECT a,  b FROM T", rows.Single(row => row.Left != null).Left,
                "the text shown is still the file's");
        }

        [TestMethod]
        public void ARealChangeIsStillAChangeInTheSideBySideView()
        {
            var rows = DiffText.SideBySide("SELECT a FROM T", "SELECT b FROM T");

            Assert.IsTrue(rows.Any(row => row.IsChange));
        }

        /// <summary>
        /// A line that moved is not a spacing change. Ignoring it would hide a real edit behind a setting
        /// about layout, so the inline pass only pairs lines that normalise to the same string.
        /// </summary>
        [TestMethod]
        public void JoiningOrSplittingLinesIsNotWhitespaceOnly()
        {
            var rows = DiffText.Inline("SELECT a,\n       b FROM T", "SELECT a, b FROM T", true);

            Assert.IsTrue(rows.Any(row => row.Kind != DiffLineKind.Unchanged));
        }

        [TestMethod]
        public void TheInlineViewCollapsesASpacingOnlyPairOnlyWhenTheOptionIsOn()
        {
            // Off, which is the default: the differ reports the pair and the view shows it.
            var asDiffered = DiffText.Inline("SELECT a FROM T", "SELECT  a  FROM T");

            Assert.IsTrue(asDiffered.Any(row => row.Kind != DiffLineKind.Unchanged));

            // On: one unchanged line, showing the source's text, which is the state the script brings.
            var relaxed = DiffText.Inline("SELECT a FROM T", "SELECT  a  FROM T", true);

            Assert.IsFalse(relaxed.Any(row => row.Kind != DiffLineKind.Unchanged));
            Assert.AreEqual(1, relaxed.Count);
            Assert.AreEqual("SELECT a FROM T", relaxed[0].Text);
        }

        [TestMethod]
        public void NormalizingCollapsesWhitespaceRunsAndTrims()
        {
            Assert.AreEqual("SELECT a FROM T", DiffText.Normalize("  SELECT\ta\r\n   FROM T  "));
            Assert.AreEqual(string.Empty, DiffText.Normalize(null));
            Assert.IsTrue(DiffText.SameIgnoringWhitespace("a  b", "a b"));
            Assert.IsFalse(DiffText.SameIgnoringWhitespace("a b", "ab"));
        }

        [TestMethod]
        public void TheSummaryNamesASpacingOnlyDifferenceWhenItIsIgnored()
        {
            // Off, which is the default: the pair counts as a line each way.
            Assert.AreEqual("1 lines added, 1 removed", DiffText.Summary(DiffStatus.Alter, "SELECT a", "SELECT  a"));

            // On: there is nothing to count, and the header says what the difference actually is.
            Assert.AreEqual("Only whitespace differs", DiffText.Summary(DiffStatus.Alter, "SELECT a", "SELECT  a", true));
        }

        [TestMethod]
        public void ARealDifferenceIsStillReportedAsLinesEvenWithTheOptionOn()
        {
            Assert.AreEqual("1 lines added, 1 removed",
                DiffText.Summary(DiffStatus.Alter, "SELECT a FROM T", "SELECT b FROM T", true));
        }
    }
}
