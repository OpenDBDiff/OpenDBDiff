using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Ui.Model
{
    /// <summary>
    /// The rules behind section 9.6's optional background check: what a stamp is, when two of them describe
    /// different databases, when the check is worth running, and what it says when it fires.
    /// </summary>
    [TestClass]
    public class DestinationStampTests
    {
        private static readonly DateTime Morning = new DateTime(2026, 10, 15, 9, 0, 0);
        private static readonly DateTime Later = new DateTime(2026, 10, 15, 9, 20, 0);

        [TestMethod]
        public void TheSameNumbersDescribeTheSameDatabase()
        {
            var baseline = new DestinationStamp(Morning, 100);

            Assert.IsTrue(baseline.SameAs(new DestinationStamp(Morning, 100)));
        }

        /// <summary>An alter moves the newest modification date, which is why the date is in the stamp.</summary>
        [TestMethod]
        public void ANewerModificationIsAChange()
        {
            Assert.IsFalse(new DestinationStamp(Morning, 100).SameAs(new DestinationStamp(Later, 100)));
        }

        /// <summary>
        /// A drop moves neither date nor... it moves only the count, so a stamp of dates alone would miss it.
        /// </summary>
        [TestMethod]
        public void AChangedCountIsAChangeEvenWhenTheNewestDateIsUnmoved()
        {
            Assert.IsFalse(new DestinationStamp(Morning, 100).SameAs(new DestinationStamp(Morning, 101)));
            Assert.IsFalse(new DestinationStamp(Morning, 100).SameAs(new DestinationStamp(Morning, 99)));
        }

        [TestMethod]
        public void AnEmptyDatabaseComparesAsEqual()
        {
            var empty = new DestinationStamp(null, 0);

            Assert.IsTrue(empty.SameAs(new DestinationStamp(null, 0)));
            Assert.IsFalse(empty.SameAs(new DestinationStamp(Morning, 0)), "the first object appearing is a change");
        }

        /// <summary>
        /// A check that could not read the destination must not report a change it never saw, and two failed
        /// reads must not look like one either.
        /// </summary>
        [TestMethod]
        public void AMissingStampIsNotAChange()
        {
            var stamp = new DestinationStamp(Morning, 1);

            Assert.IsFalse(stamp.SameAs(null));
            Assert.IsFalse(DestinationStamp.Same(stamp, null));
            Assert.IsFalse(DestinationStamp.Same(null, stamp));
            Assert.IsTrue(DestinationStamp.Same(null, null));
            Assert.IsTrue(DestinationStamp.Same(stamp, new DestinationStamp(Morning, 1)));
            Assert.IsFalse(DestinationStamp.Same(stamp, new DestinationStamp(Later, 1)));
        }

        [TestMethod]
        public void EqualityAndHashingFollowTheComparison()
        {
            var baseline = new DestinationStamp(Morning, 100);

            Assert.AreEqual(baseline, new DestinationStamp(Morning, 100));
            Assert.AreNotEqual(baseline, new DestinationStamp(Later, 100));
            Assert.AreEqual(baseline.GetHashCode(), new DestinationStamp(Morning, 100).GetHashCode());
            Assert.IsTrue(baseline.Equals((object)new DestinationStamp(Morning, 100)));
            Assert.IsFalse(baseline.Equals("not a stamp"));
        }

        [TestMethod]
        public void TheChangeIsDescribedInWordsThatSayWhichWay()
        {
            var baseline = new DestinationStamp(Morning, 100);

            Assert.AreEqual("1 object was added", new DestinationStamp(Later, 101).DescribeChangeFrom(baseline));
            Assert.AreEqual("3 objects were added", new DestinationStamp(Later, 103).DescribeChangeFrom(baseline));
            Assert.AreEqual("1 object was removed", new DestinationStamp(Later, 99).DescribeChangeFrom(baseline));
            Assert.AreEqual("2 objects were removed", new DestinationStamp(Later, 98).DescribeChangeFrom(baseline));
            Assert.AreEqual("an object was written to", new DestinationStamp(Later, 100).DescribeChangeFrom(baseline));
            Assert.AreEqual("the destination could not be read", new DestinationStamp(Later, 100).DescribeChangeFrom(null));
        }

        /// <summary>
        /// Section 9.6's check is optional and costs a query, so it runs only when the answer could change
        /// anything: results on screen, nothing already stale, and a destination to watch.
        /// </summary>
        [TestMethod]
        public void TheCheckOnlyRunsWhenThereIsSomethingToLearn()
        {
            Assert.IsTrue(StaleRules.WorthWatching(AppState.Results, CompareStaleness.Fresh, true));

            foreach (var state in new[]
            {
                AppState.Setup, AppState.Comparing, AppState.Confirming, AppState.Applying, AppState.Report
            })
            {
                Assert.IsFalse(StaleRules.WorthWatching(state, CompareStaleness.Fresh, true),
                    "nothing to learn in " + state);
            }

            Assert.IsFalse(StaleRules.WorthWatching(AppState.Results, CompareStaleness.Applied, true),
                "the results are already known to be out of date");
            Assert.IsFalse(StaleRules.WorthWatching(AppState.Results, CompareStaleness.ChangedExternally, true),
                "the check has already fired, and firing again adds nothing");
            Assert.IsFalse(StaleRules.WorthWatching(AppState.Results, CompareStaleness.Fresh, false),
                "there is no destination to watch");
        }

        /// <summary>
        /// External change is a stale reason with its own wording: the useful news is not that the results
        /// are old but that somebody else is editing the database the user is about to write to.
        /// </summary>
        [TestMethod]
        public void TheExternalChangeReasonHasItsOwnBanner()
        {
            Assert.IsTrue(StaleRules.MakesResultsStale(CompareStaleness.ChangedExternally));
            Assert.AreEqual(StaleRules.ExternalChangeBannerText, StaleRules.BannerFor(CompareStaleness.ChangedExternally));
            Assert.AreNotEqual(StaleRules.BannerText, StaleRules.ExternalChangeBannerText);
            Assert.AreEqual(StaleRules.BannerText, StaleRules.BannerFor(CompareStaleness.Applied));
            Assert.AreEqual(StaleRules.BannerText, StaleRules.BannerFor(CompareStaleness.ConnectionChanged));
        }
    }
}
