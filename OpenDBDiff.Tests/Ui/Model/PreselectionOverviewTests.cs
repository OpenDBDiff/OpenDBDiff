using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Ui.Model
{
    /// <summary>
    /// The pre-selection overview of section 6.5.4: the type table, the heading and the "Needs
    /// attention" list that decides what gets looked at first.
    /// </summary>
    [TestClass]
    public class PreselectionOverviewTests
    {
        [TestMethod]
        public void TheHeadingCountsDifferencesAndTypes()
        {
            var overview = OverviewRules.Build(new[]
            {
                Item(DiffStatus.Create, ObjectType.Table, "dbo.A"),
                Item(DiffStatus.Alter, ObjectType.View, "dbo.B"),
                Item(DiffStatus.Identical, ObjectType.Function, "dbo.C")
            }, null);

            Assert.AreEqual("2 differences across 2 object types", overview.Heading);
        }

        [TestMethod]
        public void TheTypeTableIsInTheReviewListsOrderAndCountsEachStatus()
        {
            var overview = OverviewRules.Build(new[]
            {
                Item(DiffStatus.Alter, ObjectType.View, "dbo.V"),
                Item(DiffStatus.Create, ObjectType.Table, "dbo.T1"),
                Item(DiffStatus.Drop, ObjectType.Table, "dbo.T2"),
                Item(DiffStatus.Alter, ObjectType.Table, "dbo.T3")
            }, null);

            CollectionAssert.AreEqual(
                new[] { ObjectType.Table, ObjectType.View },
                overview.Rows.Select(row => row.Type).ToArray());

            var tables = overview.Rows[0];

            Assert.AreEqual("Tables", tables.Title);
            Assert.AreEqual(1, tables.Create);
            Assert.AreEqual(1, tables.Alter);
            Assert.AreEqual(1, tables.Drop);
            Assert.AreEqual(3, tables.DifferenceCount);
        }

        [TestMethod]
        public void IdenticalObjectsAreNotInTheTable()
        {
            var overview = OverviewRules.Build(new[] { Item(DiffStatus.Identical, ObjectType.Table, "dbo.A") }, null);

            Assert.AreEqual(0, overview.Rows.Count);
            Assert.AreEqual("0 differences across 0 object types", overview.Heading);
        }

        [TestMethod]
        public void TheAttentionListNamesTheBlockedItemsFirstThenTheDrops()
        {
            var blocked = Item(DiffStatus.Create, ObjectType.View, "dbo.Report");
            var dropped = Item(DiffStatus.Drop, ObjectType.Table, "dbo.Legacy");

            var warning = new DependencyWarning(
                blocked.Key, dropped.Key, DependencyWarningKind.RequiredDependencyNotIncluded,
                DependencySeverity.Blocking, "Depends on dbo.Legacy", new DependencyFix(DependencyFixKind.SelectDependency, dropped.Key));

            var overview = OverviewRules.Build(new[] { blocked, dropped }, new[] { warning });

            Assert.AreEqual(2, overview.Attention.Count);
            Assert.AreEqual(AttentionReason.Dependency, overview.Attention[0].Reason);
            Assert.IsTrue(overview.Attention[0].IsBlocking);
            Assert.AreEqual(AttentionReason.Drop, overview.Attention[1].Reason);
            Assert.IsFalse(overview.Attention[1].IsBlocking);
        }

        [TestMethod]
        public void AnObjectIsListedOnceWithTheReasonThatMattersMore()
        {
            var blocked = Item(DiffStatus.Drop, ObjectType.Table, "dbo.Legacy");

            var warning = new DependencyWarning(
                blocked.Key, blocked.Key, DependencyWarningKind.DependentNotDropped,
                DependencySeverity.Blocking, "Still needed by dbo.Report", new DependencyFix(DependencyFixKind.DeselectItem, blocked.Key));

            var overview = OverviewRules.Build(new[] { blocked }, new[] { warning });

            Assert.AreEqual(1, overview.Attention.Count);
            Assert.AreEqual(AttentionReason.Dependency, overview.Attention[0].Reason);
        }

        [TestMethod]
        public void SomethingThatCannotBeScriptedAndSomethingUnchangedAreHandled()
        {
            var unsupported = Item(DiffStatus.Alter, ObjectType.Table, "dbo.Odd");
            unsupported.Applicability = Applicability.Unsupported;
            unsupported.UnsupportedReason = "not scriptable";

            var overview = OverviewRules.Build(new[] { unsupported }, null);

            Assert.AreEqual(1, overview.Attention.Count);
            Assert.AreEqual(AttentionReason.Unsupported, overview.Attention[0].Reason);
            Assert.AreEqual("not scriptable", overview.Attention[0].Text);
        }

        [TestMethod]
        public void AnUnsupportedItemWithNoReasonStillSaysSomething()
        {
            var unsupported = Item(DiffStatus.Alter, ObjectType.Table, "dbo.Odd");
            unsupported.Applicability = Applicability.Unsupported;

            var overview = OverviewRules.Build(new[] { unsupported }, null);

            Assert.AreEqual("The engine cannot script this object.", overview.Attention[0].Text);
        }

        [TestMethod]
        public void AnEmptyComparisonHasNoAttention()
        {
            var overview = OverviewRules.Build(new List<DiffItem>(), null);

            Assert.IsFalse(overview.HasAttention);
            Assert.AreEqual("0 differences across 0 object types", overview.Heading);
        }

        private static DiffItem Item(DiffStatus status, ObjectType type, string name)
        {
            var parts = name.Split('.');

            return new DiffItem(new DiffItemKey(type, parts[0], parts[1]), status);
        }
    }
}
