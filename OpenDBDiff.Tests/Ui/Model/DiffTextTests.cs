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
    }
}
