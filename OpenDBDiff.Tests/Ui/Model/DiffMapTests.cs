using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Ui.Model
{
    /// <summary>
    /// Section 6.5's minimap: consecutive changed rows become one band, every band is a fraction of the
    /// whole diff, and the band knows which kind of change it marks.
    /// </summary>
    [TestClass]
    public class DiffMapTests
    {
        [TestMethod]
        public void ADiffWithNoChangesHasNoBands()
        {
            Assert.AreEqual(0, DiffMap.Bands(Enumerable.Repeat(DiffLineKind.Unchanged, 10).ToList()).Count);
            Assert.AreEqual(0, DiffMap.Bands(new List<DiffLineKind>()).Count);
            Assert.AreEqual(0, DiffMap.Bands((IReadOnlyList<DiffLineKind>)null).Count);
        }

        [TestMethod]
        public void OneChangedRowIsABandAtItsOwnFraction()
        {
            var bands = DiffMap.Bands(new List<DiffLineKind>
            {
                DiffLineKind.Unchanged,
                DiffLineKind.Changed,
                DiffLineKind.Unchanged,
                DiffLineKind.Unchanged
            });

            var band = bands.Single();

            Assert.AreEqual(0.25, band.Top, 0.0001);
            Assert.AreEqual(0.25, band.Height, 0.0001);
            Assert.AreEqual(DiffLineKind.Changed, band.Kind);
            Assert.AreEqual(1, band.FirstRow);
            Assert.AreEqual(1, band.LastRow);
        }

        /// <summary>A hunk is a stretch of changes, so adjacent rows of one kind are one band.</summary>
        [TestMethod]
        public void AdjacentChangesOfOneKindMergeIntoOneBand()
        {
            var bands = DiffMap.Bands(new List<DiffLineKind>
            {
                DiffLineKind.Unchanged,
                DiffLineKind.Added,
                DiffLineKind.Added,
                DiffLineKind.Added
            });

            var band = bands.Single();

            Assert.AreEqual(0.25, band.Top, 0.0001);
            Assert.AreEqual(0.75, band.Height, 0.0001);
            Assert.AreEqual(1, band.FirstRow);
            Assert.AreEqual(3, band.LastRow);
        }

        [TestMethod]
        public void ChangesOfDifferentKindsStaySeparate()
        {
            var bands = DiffMap.Bands(new List<DiffLineKind>
            {
                DiffLineKind.Added,
                DiffLineKind.Changed
            });

            Assert.AreEqual(2, bands.Count);
            Assert.AreEqual(DiffLineKind.Added, bands[0].Kind);
            Assert.AreEqual(DiffLineKind.Changed, bands[1].Kind);
            Assert.AreEqual(0.5, bands[1].Top, 0.0001);
        }

        [TestMethod]
        public void AnUnchangedRowBetweenTwoChangesSplitsTheBand()
        {
            var bands = DiffMap.Bands(new List<DiffLineKind>
            {
                DiffLineKind.Added,
                DiffLineKind.Unchanged,
                DiffLineKind.Added
            });

            Assert.AreEqual(2, bands.Count);
            Assert.AreEqual(1.0 / 3, bands[0].Height, 0.0001);
            Assert.AreEqual(2.0 / 3, bands[1].Top, 0.0001);
        }

        /// <summary>Every band has to land inside the strip, whatever the diff looks like.</summary>
        [TestMethod]
        public void EveryBandStaysInsideTheStrip()
        {
            var kinds = new List<DiffLineKind>
            {
                DiffLineKind.Added, DiffLineKind.Unchanged, DiffLineKind.Removed, DiffLineKind.Changed,
                DiffLineKind.Unchanged, DiffLineKind.Filler, DiffLineKind.Changed, DiffLineKind.Changed
            };

            foreach (var band in DiffMap.Bands(kinds))
            {
                Assert.IsTrue(band.Top >= 0 && band.Top < 1, "top " + band.Top);
                Assert.IsTrue(band.Height > 0 && band.Top + band.Height <= 1.0001, "bottom " + (band.Top + band.Height));
            }
        }

        /// <summary>A filler row is not a change on its own; the line it stands for is on the other side.</summary>
        [TestMethod]
        public void AFillerRowWithNoChangeBesideItIsNotABand()
        {
            Assert.AreEqual(0, DiffMap.Bands(new List<DiffLineKind> { DiffLineKind.Filler, DiffLineKind.Filler }).Count);
        }

        [TestMethod]
        public void TheSideBySideViewMapsASourceOnlyLineToAdded()
        {
            var rows = DiffText.SideBySide("SELECT 1\nSELECT 2", "SELECT 1");

            var bands = DiffMap.Bands(rows);

            Assert.AreEqual(DiffLineKind.Added, bands.Single().Kind);
        }

        [TestMethod]
        public void TheSideBySideViewMapsADestinationOnlyLineToRemoved()
        {
            var rows = DiffText.SideBySide("SELECT 1", "SELECT 1\nSELECT 2");

            Assert.AreEqual(DiffLineKind.Removed, DiffMap.Bands(rows).Single().Kind);
        }

        [TestMethod]
        public void TheSideBySideViewMapsAPairChangedOnBothSidesToChanged()
        {
            var rows = DiffText.SideBySide("SELECT a FROM T", "SELECT b FROM T");

            Assert.AreEqual(DiffLineKind.Changed, DiffMap.Bands(rows).Single().Kind);
        }

        /// <summary>
        /// The inline view reports a modified line as a removed row and an added one, so the pair has to be
        /// merged back: one changed line is one hunk to a reader, whatever the differ's rows look like.
        /// </summary>
        [TestMethod]
        public void TheInlineViewMergesARemovedAndAddedPairIntoOneChangedBand()
        {
            var rows = DiffText.Inline("SELECT a\nSELECT b", "SELECT a\nSELECT c");

            var band = DiffMap.Bands(rows).Single();

            Assert.AreEqual(DiffLineKind.Changed, band.Kind);
            Assert.AreEqual(1, band.FirstRow);
            Assert.AreEqual(2, band.LastRow, "the band covers both of the pair's rows");
        }

        /// <summary>
        /// Section 13.5 wants a text alternative for anything that carries meaning as a mark, and section 10
        /// wants a name: the strip's bands are coloured rectangles, so each one says what it is.
        /// </summary>
        [TestMethod]
        public void ABandSaysWhatItIsAndWhere()
        {
            var single = DiffMap.Bands(new List<DiffLineKind> { DiffLineKind.Unchanged, DiffLineKind.Added })[0];

            Assert.AreEqual("Added \u00b7 row 2", single.Tooltip);

            var run = DiffMap.Bands(new List<DiffLineKind>
            {
                DiffLineKind.Removed,
                DiffLineKind.Removed,
                DiffLineKind.Removed
            })[0];

            Assert.AreEqual("Removed \u00b7 rows 1-3", run.Tooltip);
        }

        [TestMethod]
        public void APureInsertionInTheInlineViewIsStillOneAddedBand()
        {
            var rows = DiffText.Inline("SELECT a\nSELECT b", "SELECT a");

            var band = DiffMap.Bands(rows).Single();

            Assert.AreEqual(DiffLineKind.Added, band.Kind);
            Assert.AreEqual(1, band.FirstRow);
            Assert.AreEqual(1, band.LastRow);
        }
    }
}
