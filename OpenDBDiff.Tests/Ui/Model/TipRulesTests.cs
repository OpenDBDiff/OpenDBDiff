using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Ui.Model
{
    /// <summary>
    /// Section 19's R4 tip: shown once, when it explains something, and never when it would be describing a
    /// setting the user has already changed.
    /// </summary>
    [TestClass]
    public class TipRulesTests
    {
        [TestMethod]
        public void TheTipShowsWhenThereAreUnselectedDrops()
        {
            Assert.IsTrue(TipRules.ShowsUnselectedDropTip(new List<string>(), 3, false));
        }

        [TestMethod]
        public void ItIsShownOnce()
        {
            var seen = new List<string> { TipRules.DropPreselectionId };

            Assert.IsFalse(TipRules.ShowsUnselectedDropTip(seen, 3, false), "a tip that comes back is a nag");
        }

        /// <summary>
        /// A comparison with no deletes in it has nothing to explain, and a tip that appeared anyway would be
        /// describing changes that are not there.
        /// </summary>
        [TestMethod]
        public void NoUnselectedDropsMeansNoTip()
        {
            Assert.IsFalse(TipRules.ShowsUnselectedDropTip(new List<string>(), 0, false));
            Assert.IsFalse(TipRules.ShowsUnselectedDropTip(new List<string>(), -1, false));
        }

        /// <summary>
        /// Once deletes are selected by default, the tip would state the opposite of what happens.
        /// </summary>
        [TestMethod]
        public void TheTipStaysQuietWhenDropsAreAlreadySelectedByDefault()
        {
            Assert.IsFalse(TipRules.ShowsUnselectedDropTip(new List<string>(), 3, true));
        }

        [TestMethod]
        public void AFileWithNoRecordOfAnyTipShowsIt()
        {
            Assert.IsTrue(TipRules.ShowsUnselectedDropTip(null, 1, false));
        }

        [TestMethod]
        public void AnotherTipInTheListDoesNotCountAsThisOne()
        {
            Assert.IsTrue(TipRules.ShowsUnselectedDropTip(new List<string> { "some-other-tip" }, 1, false));
        }

        /// <summary>Section 19 quotes the wording, so the wording is checked rather than retyped in the view.</summary>
        [TestMethod]
        public void TheWordingIsTheOneTheSpecificationAsksFor()
        {
            Assert.AreEqual("Drop items start unselected for safety", TipRules.DropPreselectionText);
            Assert.IsFalse(string.IsNullOrWhiteSpace(TipRules.DropPreselectionDetail));
            Assert.IsFalse(string.IsNullOrWhiteSpace(TipRules.DropPreselectionId));
        }
    }
}
