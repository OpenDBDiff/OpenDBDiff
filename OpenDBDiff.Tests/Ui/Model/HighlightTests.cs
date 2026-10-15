using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Ui.Model
{
    /// <summary>
    /// Section 6.3's highlight: the matched substring of a list row is marked, and the row still shows the
    /// object's own spelling.
    /// </summary>
    [TestClass]
    public class HighlightTests
    {
        [TestMethod]
        public void WithNoSearchTheWholeNameIsOneUnmarkedRun()
        {
            var runs = Highlight.Split("dbo.DeleteScheme", string.Empty);

            Assert.AreEqual(1, runs.Count);
            Assert.AreEqual("dbo.DeleteScheme", runs[0].Text);
            Assert.IsFalse(runs[0].IsMatch);
        }

        [TestMethod]
        public void TheMatchedPartIsMarkedAndTheRestIsNot()
        {
            var runs = Highlight.Split("dbo.DeleteScheme", "delete");

            Assert.AreEqual(3, runs.Count, "the part before, the match, and the part after");
            Assert.AreEqual("dbo.", runs[0].Text);
            Assert.IsFalse(runs[0].IsMatch);
            Assert.AreEqual("Delete", runs[1].Text, "the run keeps the name's own casing");
            Assert.IsTrue(runs[1].IsMatch);
            Assert.AreEqual("Scheme", runs[2].Text);
            Assert.IsFalse(runs[2].IsMatch);
        }

        /// <summary>Section 6.3 says the search is case-insensitive; the highlight follows it, not the other way round.</summary>
        [TestMethod]
        public void TheSearchIsCaseInsensitiveAndSoIsTheMarking()
        {
            var runs = Highlight.Split("dbo.DeleteScheme", "DELETE");

            Assert.AreEqual("Delete", runs.Single(run => run.IsMatch).Text);
        }

        [TestMethod]
        public void EveryOccurrenceIsMarked()
        {
            var runs = Highlight.Split("dbo.SchemeScheme", "scheme");

            Assert.AreEqual(3, runs.Count, "one unmatched prefix and two matches");
            Assert.AreEqual(2, runs.Count(run => run.IsMatch));
        }

        [TestMethod]
        public void AMatchInTheMiddleLeavesRunsOnBothSides()
        {
            var runs = Highlight.Split("dbo.ReportView", "report");

            Assert.AreEqual(3, runs.Count);
            Assert.AreEqual("Report", runs[1].Text);
            Assert.AreEqual("View", runs[2].Text);
        }

        [TestMethod]
        public void RunsAlwaysReconstructTheName()
        {
            foreach (var name in new[] { "dbo.DeleteScheme", "dbo.A", "A", "dbo.RepeatRepeat" })
            {
                foreach (var query in new[] { "", "a", "e", "repeat", "dbo", "zzz" })
                {
                    Assert.AreEqual(name, string.Concat(Highlight.Split(name, query).Select(run => run.Text)),
                        "runs for " + name + " with the query '" + query + "'");
                }
            }
        }

        [TestMethod]
        public void AQueryThatMatchesNothingMarksNothing()
        {
            var runs = Highlight.Split("dbo.Scheme", "xyz");

            Assert.AreEqual(1, runs.Count);
            Assert.IsFalse(runs[0].IsMatch);
        }

        [TestMethod]
        public void AnEmptyNameHasNoRuns()
        {
            Assert.AreEqual(0, Highlight.Split(string.Empty, "dbo").Count);
            Assert.AreEqual(0, Highlight.Split(null, "dbo").Count);
        }

        [TestMethod]
        public void AQueryTheSameLengthAsTheNameMarksAllOfIt()
        {
            var runs = Highlight.Split("dbo", "DBO");

            Assert.AreEqual(1, runs.Count);
            Assert.IsTrue(runs[0].IsMatch);
        }

        /// <summary>
        /// The query is trimmed, because the search box's own text is, and a highlight that marked a trailing
        /// space would mark a character the name does not contain.
        /// </summary>
        [TestMethod]
        public void TheQueryIsTrimmedLikeTheFilterIs()
        {
            var runs = Highlight.Split("dbo.Scheme", "  scheme  ");

            Assert.AreEqual("Scheme", runs.Single(run => run.IsMatch).Text);
        }
    }
}
