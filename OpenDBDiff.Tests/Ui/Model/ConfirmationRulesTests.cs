using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Ui.Model
{
    /// <summary>
    /// The destructive count of section 6.7 and the confirmation dialog's gating rules of section 6.8.
    /// </summary>
    [TestClass]
    public class ConfirmationRulesTests
    {
        [TestMethod]
        public void TheDescriptionsFollowTheOrderOfTheSpecification()
        {
            var kinds = DestructiveKinds.DropColumn | DestructiveKinds.DropObject | DestructiveKinds.DataLossRisk;

            CollectionAssert.AreEqual(
                new[] { "DROP", "DROP COLUMN", "data loss risk" },
                System.Linq.Enumerable.ToArray(DestructiveRules.Describe(kinds)));
        }

        [TestMethod]
        public void AnItemWithNoDestructiveKindHasNothingToReport()
        {
            Assert.AreEqual(string.Empty, DestructiveRules.Summary(DestructiveKinds.None));
            Assert.IsFalse(DestructiveRules.RequiresConfirmation(DestructiveKinds.None));
        }

        [TestMethod]
        public void MergingItemsCollectsEveryKind()
        {
            var alter = new DiffItem(Key("dbo.Orders"), DiffStatus.Alter);
            alter.DestructiveKinds = DestructiveKinds.DropColumn;

            var drop = new DiffItem(Key("dbo.Legacy"), DiffStatus.Drop);
            drop.DestructiveKinds = DestructiveKinds.DropObject;

            var merged = DestructiveRules.Merge(new[] { alter, drop });

            Assert.AreEqual(DestructiveKinds.DropColumn | DestructiveKinds.DropObject, merged);
            Assert.IsTrue(DestructiveRules.RequiresConfirmation(merged));
        }

        [TestMethod]
        public void TheTypedConfirmationIsAskedForOnProductionOrOnADestructiveChange()
        {
            // Section 6.8: "only if destination env = Prod or d > 0".
            Assert.IsTrue(ConfirmationRules.RequiresTypedConfirmation(EnvironmentTag.For(EnvironmentKind.Prod), 0));
            Assert.IsTrue(ConfirmationRules.RequiresTypedConfirmation(EnvironmentTag.None, 1));
            Assert.IsFalse(ConfirmationRules.RequiresTypedConfirmation(EnvironmentTag.None, 0));
            Assert.IsFalse(ConfirmationRules.RequiresTypedConfirmation(EnvironmentTag.For(EnvironmentKind.Test), 0));
        }

        [TestMethod]
        public void TheTypedNameMustMatchTheDatabaseExactlyAfterTrimming()
        {
            Assert.IsTrue(ConfirmationRules.AcceptsTypedConfirmation("VivaLDI", "VivaLDI"));
            Assert.IsTrue(ConfirmationRules.AcceptsTypedConfirmation("VivaLDI", "  VivaLDI  "));

            // Section 6.8: "case-sensitive match, trimmed".
            Assert.IsFalse(ConfirmationRules.AcceptsTypedConfirmation("VivaLDI", "vivaldi"));
            Assert.IsFalse(ConfirmationRules.AcceptsTypedConfirmation("VivaLDI", "VivaLD"));
            Assert.IsFalse(ConfirmationRules.AcceptsTypedConfirmation("VivaLDI", string.Empty));
            Assert.IsFalse(ConfirmationRules.AcceptsTypedConfirmation(string.Empty, "VivaLDI"));
        }

        [TestMethod]
        public void TheApplyButtonNeedsAValidTypedNameWhenOneIsRequired()
        {
            Assert.IsTrue(ConfirmationRules.CanApply(false, false, 3));
            Assert.IsTrue(ConfirmationRules.CanApply(true, true, 3));

            Assert.IsFalse(ConfirmationRules.CanApply(true, false, 3));
            Assert.IsFalse(ConfirmationRules.CanApply(false, true, 0), "nothing selected");
        }

        [TestMethod]
        public void TheCopyMatchesTheSpecificationsWords()
        {
            Assert.AreEqual("Type the database name to confirm", ConfirmationRules.TypeToConfirmText);
            Assert.AreEqual("Apply 7 changes to VivaLDI", ConfirmationRules.Title(7, "VivaLDI"));
            Assert.AreEqual("Apply 7 changes", ConfirmationRules.ApplyLabel(7));
            Assert.AreEqual("2 destructive statements. Dropped objects and columns cannot be recovered by this tool.",
                ConfirmationRules.DestructiveCallout(2));
        }

        private static DiffItemKey Key(string name)
        {
            var parts = name.Split('.');

            return new DiffItemKey(ObjectType.Table, parts[0], parts[1]);
        }
    }
}
