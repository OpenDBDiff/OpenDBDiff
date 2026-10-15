using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Ui.Model
{
    /// <summary>
    /// Section 6.5.3's navigation and word-level tinting: F7 and Shift+F7 move between change hunks, and
    /// a changed line is split into the words that changed and the ones that did not.
    /// </summary>
    [TestClass]
    public class HunkRulesTests
    {
        [TestMethod]
        public void AHunkStartsWhereAChangeFollowsSomethingUnchanged()
        {
            var rows = new List<SideBySideRow>
            {
                Row(DiffLineKind.Unchanged, DiffLineKind.Unchanged),
                Row(DiffLineKind.Changed, DiffLineKind.Changed),
                Row(DiffLineKind.Changed, DiffLineKind.Changed),
                Row(DiffLineKind.Unchanged, DiffLineKind.Unchanged),
                Row(DiffLineKind.Added, DiffLineKind.Filler)
            };

            CollectionAssert.AreEqual(new[] { 1, 4 }, HunkRules.HunkStarts(rows).ToArray());
        }

        [TestMethod]
        public void AnInlineDiffHasHunksToo()
        {
            var rows = new List<InlineRow>
            {
                new InlineRow(DiffLineKind.Unchanged, "  ", "a"),
                new InlineRow(DiffLineKind.Added, "+ ", "b"),
                new InlineRow(DiffLineKind.Unchanged, "  ", "c"),
                new InlineRow(DiffLineKind.Removed, "- ", "d")
            };

            CollectionAssert.AreEqual(new[] { 1, 3 }, HunkRules.HunkStarts(rows).ToArray());
        }

        [TestMethod]
        public void NextAndPreviousWrapAround()
        {
            var starts = new[] { 2, 7, 12 };

            Assert.AreEqual(7, HunkRules.Next(starts, 2));
            Assert.AreEqual(12, HunkRules.Next(starts, 7));
            Assert.AreEqual(2, HunkRules.Next(starts, 12), "F7 at the last hunk comes back to the first");

            Assert.AreEqual(7, HunkRules.Previous(starts, 12));
            Assert.AreEqual(2, HunkRules.Previous(starts, 7));
            Assert.AreEqual(12, HunkRules.Previous(starts, 2), "Shift+F7 at the first hunk goes to the last");
        }

        [TestMethod]
        public void NoHunksMeansNoMovement()
        {
            Assert.AreEqual(-1, HunkRules.Next(new int[0], 0));
            Assert.AreEqual(-1, HunkRules.Previous(new int[0], 0));
            Assert.AreEqual(0, HunkRules.HunkStarts(new List<SideBySideRow>()).Count);
        }

        [TestMethod]
        public void AChangedLineIsSplitIntoWhatChangedAndWhatDidNot()
        {
            var pair = HunkRules.Words("[Id] [int] NULL", "[Id] [int] NOT NULL");

            // Both sides reconstruct exactly, whatever granularity the diff works at.
            Assert.AreEqual("[Id] [int] NULL", Concat(pair.Left));
            Assert.AreEqual("[Id] [int] NOT NULL", Concat(pair.Right));

            // The text neither side changed is the same on both sides, and what was added is tinted.
            Assert.AreEqual(Concat(pair.Left.Where(run => !run.IsChanged)), Concat(pair.Right.Where(run => !run.IsChanged)));
            Assert.AreEqual(string.Empty, Concat(pair.Left.Where(run => run.IsChanged)), "nothing was removed");
            Assert.IsTrue(Concat(pair.Right.Where(run => run.IsChanged)).Contains("NOT"), "the insertion is the changed run");
        }

        [TestMethod]
        public void AnIdenticalPairOfLinesHasNothingTinted()
        {
            var pair = HunkRules.Words("same", "same");

            Assert.AreEqual("same", Concat(pair.Left));
            Assert.IsFalse(pair.Left.Any(run => run.IsChanged));
            Assert.IsFalse(pair.Right.Any(run => run.IsChanged));
        }

        [TestMethod]
        public void AdjacentRunsOfTheSameKindAreJoined()
        {
            var pair = HunkRules.Words("a b c d", "a X c d");

            // Runs alternate, never two in a row with the same flag.
            for (var i = 1; i < pair.Right.Count; i++)
            {
                Assert.AreNotEqual(pair.Right[i - 1].IsChanged, pair.Right[i].IsChanged);
            }
        }

        [TestMethod]
        public void MissingDefinitionsDoNotThrow()
        {
            var pair = HunkRules.Words(null, null);

            Assert.AreEqual(0, pair.Left.Count);
            Assert.AreEqual(0, pair.Right.Count);
        }

        private static SideBySideRow Row(DiffLineKind left, DiffLineKind right)
        {
            return new SideBySideRow("l", "r", left, right, 1, 1);
        }

        private static string Concat(IEnumerable<WordRun> runs)
        {
            return string.Concat(runs.Select(run => run.Text));
        }
    }
}
