using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Ui.Model
{
    /// <summary>
    /// The connection strip's rules (sections 6.1.2 and 6.1.3): validity transitions, the Compare
    /// enablement rule and chip truncation.
    /// </summary>
    [TestClass]
    public class ConnectionRulesTests
    {
        [TestMethod]
        public void AnIncompleteConnectionIsUnset()
        {
            Assert.AreEqual(ConnectionValidity.Unset, ConnectionRules.AfterEdit("", "VivaLDI"));
            Assert.AreEqual(ConnectionValidity.Unset, ConnectionRules.AfterEdit("srv012023", ""));
            Assert.AreEqual(ConnectionValidity.Unset, ConnectionRules.AfterEdit("   ", "VivaLDI"));
        }

        [TestMethod]
        public void ACompletedConnectionDropsBackToUntestedAfterAnEdit()
        {
            // Section 6.1.3: Untested means "fields complete, not tested since last edit".
            Assert.AreEqual(ConnectionValidity.Untested, ConnectionRules.AfterEdit("srv012023", "VivaLDI"));
        }

        [TestMethod]
        public void CompareNeedsBothSidesValid()
        {
            Assert.IsTrue(ConnectionRules.CanCompare(ConnectionValidity.Valid, ConnectionValidity.Valid));

            Assert.IsFalse(ConnectionRules.CanCompare(ConnectionValidity.Valid, ConnectionValidity.Untested));
            Assert.IsFalse(ConnectionRules.CanCompare(ConnectionValidity.Untested, ConnectionValidity.Valid));
            Assert.IsFalse(ConnectionRules.CanCompare(ConnectionValidity.Unset, ConnectionValidity.Valid));
            Assert.IsFalse(ConnectionRules.CanCompare(ConnectionValidity.Failed, ConnectionValidity.Valid));
            Assert.IsFalse(ConnectionRules.CanCompare(ConnectionValidity.Testing, ConnectionValidity.Valid));
        }

        [TestMethod]
        public void TwoUntestedSidesAreTestedBeforeComparing()
        {
            // Section 6.1.3: both Untested -> test both in parallel, then proceed if both succeed.
            Assert.IsTrue(ConnectionRules.ShouldTestBothBeforeCompare(ConnectionValidity.Untested, ConnectionValidity.Untested));

            Assert.IsFalse(ConnectionRules.ShouldTestBothBeforeCompare(ConnectionValidity.Valid, ConnectionValidity.Untested));
            Assert.IsFalse(ConnectionRules.ShouldTestBothBeforeCompare(ConnectionValidity.Untested, ConnectionValidity.Valid));
            Assert.IsFalse(ConnectionRules.ShouldTestBothBeforeCompare(ConnectionValidity.Valid, ConnectionValidity.Valid));
        }

        [TestMethod]
        public void CompareAcceptsAClickOnlyWhenItCanDoSomething()
        {
            // Section 6.1.3 promises that clicking Compare with both sides Untested tests them, so the
            // button has to be live in that state as well as when both are Valid.
            Assert.IsTrue(ConnectionRules.CanInvokeCompare(ConnectionValidity.Valid, ConnectionValidity.Valid));
            Assert.IsTrue(ConnectionRules.CanInvokeCompare(ConnectionValidity.Untested, ConnectionValidity.Untested));

            Assert.IsFalse(ConnectionRules.CanInvokeCompare(ConnectionValidity.Valid, ConnectionValidity.Untested));
            Assert.IsFalse(ConnectionRules.CanInvokeCompare(ConnectionValidity.Untested, ConnectionValidity.Valid));
            Assert.IsFalse(ConnectionRules.CanInvokeCompare(ConnectionValidity.Unset, ConnectionValidity.Untested));
            Assert.IsFalse(ConnectionRules.CanInvokeCompare(ConnectionValidity.Failed, ConnectionValidity.Valid));
            Assert.IsFalse(ConnectionRules.CanInvokeCompare(ConnectionValidity.Testing, ConnectionValidity.Valid));
        }

        [TestMethod]
        public void EveryValidityStateHasItsOwnDescription()
        {
            CollectionAssert.AllItemsAreUnique(new[]
            {
                ConnectionRules.StatusDescription(ConnectionValidity.Unset),
                ConnectionRules.StatusDescription(ConnectionValidity.Untested),
                ConnectionRules.StatusDescription(ConnectionValidity.Testing),
                ConnectionRules.StatusDescription(ConnectionValidity.Valid),
                ConnectionRules.StatusDescription(ConnectionValidity.Failed)
            });

            Assert.IsTrue(ConnectionRules.HasError(ConnectionValidity.Failed));
            Assert.IsFalse(ConnectionRules.HasError(ConnectionValidity.Valid));
        }

        [TestMethod]
        public void TheServerIsCappedAtTwentyFourCharactersThenEllipsised()
        {
            // Section 6.1.2: "Server never truncates (cap at 24 chars then ellipsis)".
            var shortName = "srv012023";
            Assert.AreEqual(shortName, ConnectionRules.ServerDisplay(shortName));

            var exactly24 = new string('a', 24);
            Assert.AreEqual(exactly24, ConnectionRules.ServerDisplay(exactly24));

            var over24 = new string('a', 30);
            Assert.AreEqual(new string('a', 24) + "\u2026", ConnectionRules.ServerDisplay(over24));
        }

        [TestMethod]
        public void TheChipShowsTheServerThenTheDatabaseAndKeepsTheFullTextInTheTooltip()
        {
            Assert.AreEqual("srv012023 / VivaLDI", ConnectionRules.ChipText("srv012023", "VivaLDI"));
            Assert.AreEqual("srv012023 / VivaLDI", ConnectionRules.ChipTooltip("srv012023", "VivaLDI"));

            // A long server is capped in the chip but stays whole in the tooltip.
            var server = "a-very-long-server-name-012023";
            Assert.AreEqual("a-very-long-server-name-\u2026 / VivaLDI", ConnectionRules.ChipText(server, "VivaLDI"));
            Assert.AreEqual("a-very-long-server-name-012023 / VivaLDI", ConnectionRules.ChipTooltip(server, "VivaLDI"));
        }

        [TestMethod]
        public void AnUnsetChipSaysSoRatherThanShowingASlash()
        {
            Assert.AreEqual("Not set", ConnectionRules.ChipText(null, null));
            Assert.AreEqual("Not set up yet", ConnectionRules.ChipTooltip("", ""));
        }
    }
}
