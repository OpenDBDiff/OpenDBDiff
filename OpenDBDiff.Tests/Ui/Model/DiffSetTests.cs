using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Ui.Model
{
    /// <summary>
    /// The redesign's rules: default inclusion, filtering and search order, grouping, counts and the
    /// enabling of Apply. Section 16.3 lists these as the unit-level tests.
    /// </summary>
    [TestClass]
    public class DiffSetTests
    {
        [TestMethod]
        public void DropItemsAreNotIncludedByDefault()
        {
            var set = Set(Item(DiffStatus.Create, "Table", "Users"), Item(DiffStatus.Drop, "Table", "Legacy"));

            // Section 6.4.3 and 7.1 rule 4: a deliberate safety default.
            Assert.IsTrue(set.IsIncluded(Key("Table", "Users")));
            Assert.IsFalse(set.IsIncluded(Key("Table", "Legacy")));
            Assert.AreEqual(1, set.SelectedCount);
        }

        [TestMethod]
        public void CountsAreUnfilteredTotals()
        {
            var set = Set(Item(DiffStatus.Create, "Table", "A"), Item(DiffStatus.Alter, "View", "B"),
                          Item(DiffStatus.Alter, "View", "C"), Item(DiffStatus.Drop, "Table", "D"),
                          Item(DiffStatus.Identical, "Table", "E"));

            var counts = set.Counts;

            Assert.AreEqual(1, counts.Create);
            Assert.AreEqual(2, counts.Alter);
            Assert.AreEqual(1, counts.Drop);
            Assert.AreEqual(1, counts.Identical);
            Assert.AreEqual(4, counts.Differences);

            // A search must not change the chip counts.
            set.SearchText = "nothing matches this";
            Assert.AreEqual(4, set.Counts.Differences);
        }

        [TestMethod]
        public void IdenticalItemsAreHiddenUntilAskedFor()
        {
            var set = Set(Item(DiffStatus.Alter, "Table", "A"), Item(DiffStatus.Identical, "Table", "B"));

            Assert.AreEqual(1, set.VisibleItems.Count);

            set.ShowIdentical = true;

            Assert.AreEqual(2, set.VisibleItems.Count);
        }

        [TestMethod]
        public void StatusFiltersApplyAfterTheCountsAreTaken()
        {
            var set = Set(Item(DiffStatus.Create, "Table", "A"), Item(DiffStatus.Drop, "Table", "B"));

            set.StatusFilter.Remove(DiffStatus.Drop);

            Assert.AreEqual(1, set.VisibleItems.Count);
            Assert.AreEqual(2, set.Counts.Differences);
        }

        [TestMethod]
        public void SearchMatchesNameQualifiedNameAndTypeName()
        {
            var set = Set(Item(DiffStatus.Alter, "StoredProcedure", "GetScheme", "dbo"),
                          Item(DiffStatus.Alter, "Table", "Scheme"));

            set.SearchText = "dbo.GetScheme";
            Assert.AreEqual(1, set.VisibleItems.Count);

            set.SearchText = "proc";
            Assert.AreEqual(1, set.VisibleItems.Count);
            Assert.AreEqual("GetScheme", set.VisibleItems[0].Key.Name);

            set.SearchText = "scheme";
            Assert.AreEqual(2, set.VisibleItems.Count);
        }

        [TestMethod]
        public void DefinitionSearchIsOffByDefault()
        {
            var item = Item(DiffStatus.Alter, "View", "V");
            item.SourceDefinition = "SELECT COUNT(*) FROM Hidden";

            var set = Set(item);

            set.SearchText = "Hidden";
            Assert.AreEqual(0, set.VisibleItems.Count);

            set.SearchDefinitions = true;
            Assert.AreEqual(1, set.VisibleItems.Count);
        }

        [TestMethod]
        public void GroupsFollowTheSpecifiedOrderAndHideEmptyTypes()
        {
            var set = Set(Item(DiffStatus.Alter, "User", "reporting"), Item(DiffStatus.Alter, "Table", "Users"),
                          Item(DiffStatus.Alter, "View", "V"), Item(DiffStatus.Identical, "Function", "F"));

            var groups = set.Groups;

            CollectionAssert.AreEqual(
                new[] { ObjectType.Table, ObjectType.View, ObjectType.User },
                groups.Select(group => group.ObjectType).ToArray());
        }

        [TestMethod]
        public void GroupHeadersCountDifferencesNotObjects()
        {
            var set = Set(Item(DiffStatus.Alter, "Table", "A"), Item(DiffStatus.Identical, "Table", "B"));

            var group = set.Groups.Single();

            Assert.AreEqual(1, group.VisibleDifferenceCount);
            Assert.AreEqual("1 difference", group.CountText);

            set.ShowIdentical = true;
            Assert.AreEqual("1 difference", set.Groups.Single().CountText);
        }

        [TestMethod]
        public void GroupsWithMoreThanTwelveDifferencesStartCollapsed()
        {
            var small = Set(Enumerable.Range(0, 12).Select(i => Item(DiffStatus.Alter, "Table", "T" + i)).ToArray());
            var large = Set(Enumerable.Range(0, 13).Select(i => Item(DiffStatus.Alter, "Table", "T" + i)).ToArray());

            Assert.IsTrue(small.Groups.Single().IsExpanded);
            Assert.IsFalse(large.Groups.Single().IsExpanded);
        }

        [TestMethod]
        public void GroupCheckStateIsTriStateOverVisibleChildren()
        {
            var set = Set(Item(DiffStatus.Alter, "Table", "A"), Item(DiffStatus.Drop, "Table", "B"));

            // Both are visible; only A is included by default.
            Assert.AreEqual(GroupCheckState.Indeterminate, set.GroupCheckStateOf(set.Groups.Single()));

            set.IncludeAllVisible();
            Assert.AreEqual(GroupCheckState.Checked, set.GroupCheckStateOf(set.Groups.Single()));

            set.ExcludeAllVisible();
            Assert.AreEqual(GroupCheckState.Unchecked, set.GroupCheckStateOf(set.Groups.Single()));
        }

        [TestMethod]
        public void HiddenSelectionsAreStillCounted()
        {
            var set = Set(Item(DiffStatus.Alter, "Table", "Users"), Item(DiffStatus.Alter, "View", "V"));

            set.SearchText = "Users";

            Assert.AreEqual(2, set.SelectedCount);
            Assert.AreEqual(1, set.HiddenSelectedCount);
            Assert.AreEqual("2 of 2 selected", set.SelectionText);
        }

        [TestMethod]
        public void DestructiveCountCoversIncludedDropsOnly()
        {
            var set = Set(Item(DiffStatus.Alter, "Table", "Users"), Item(DiffStatus.Drop, "Table", "Legacy"));

            Assert.AreEqual(string.Empty, set.DestructiveText);

            set.SetIncluded(Key("Table", "Legacy"), true);

            Assert.AreEqual("\u00b7 1 destructive (DROP)", set.DestructiveText);
        }

        [TestMethod]
        public void ApplyIsBlockedWithAReasonUntilEverythingHolds()
        {
            var set = Set(Item(DiffStatus.Alter, "Table", "Users"));

            Assert.IsFalse(set.CanApply(AppState.Results, true, true, false).CanApply);
            Assert.AreEqual("Results are out of date. Compare again.", set.CanApply(AppState.Results, true, true, false).DisabledReason);

            Assert.IsFalse(set.CanApply(AppState.Setup, false, true, false).CanApply);
            Assert.IsFalse(set.CanApply(AppState.Results, false, false, false).CanApply);
            Assert.IsFalse(set.CanApply(AppState.Results, false, true, true).CanApply);
            Assert.IsTrue(set.CanApply(AppState.Results, false, true, false).CanApply);

            set.ExcludeAllVisible();
            Assert.IsFalse(set.CanApply(AppState.Results, false, true, false).CanApply);
        }

        [TestMethod]
        public void ApplyLabelNamesTheCountDatabaseAndServer()
        {
            var set = Set(Item(DiffStatus.Alter, "Table", "Users"), Item(DiffStatus.Alter, "Table", "B"));

            Assert.AreEqual("Review and apply 2 changes to VivaLDI on srv012023 \u2192",
                set.ApplyLabel("VivaLDI", "srv012023"));
        }

        [TestMethod]
        public void InclusionCarriesAcrossAComparisonForObjectsThatPersist()
        {
            var first = Set(Item(DiffStatus.Alter, "Table", "Users"), Item(DiffStatus.Alter, "Table", "Orders"));
            first.SetIncluded(Key("Table", "Orders"), false);

            var second = Set(Item(DiffStatus.Alter, "Table", "Users"), Item(DiffStatus.Alter, "Table", "Orders"),
                             Item(DiffStatus.Create, "Table", "New"));
            second.CarryInclusionFrom(first.IncludedKeys, first.ExcludedKeys);

            Assert.IsTrue(second.IsIncluded(Key("Table", "Users")));
            Assert.IsFalse(second.IsIncluded(Key("Table", "Orders")));
            Assert.IsTrue(second.IsIncluded(Key("Table", "New")), "a new Create is included by default");
        }

        [TestMethod]
        public void UnsupportedItemsAreNeverIncluded()
        {
            var item = Item(DiffStatus.Alter, "Table", "Odd");
            item.Applicability = Applicability.Unsupported;
            item.UnsupportedReason = "not scriptable";

            var set = Set(item);

            Assert.IsFalse(set.IsIncluded(item.Key));
            set.SetIncluded(item.Key, true);
            Assert.IsFalse(set.IsIncluded(item.Key));
            Assert.AreEqual(1, set.TotalDifferenceCount, "still counted as a difference");
        }

        [TestMethod]
        public void FlatListSortsByName()
        {
            var set = Set(Item(DiffStatus.Alter, "Table", "Zebra"), Item(DiffStatus.Alter, "Table", "Apple"), Item(DiffStatus.Alter, "View", "Middle"));
            set.ViewMode = ListViewMode.Flat;

            CollectionAssert.AreEqual(new[] { "Apple", "Middle", "Zebra" }, set.FlatItems.Select(item => item.Key.Name).ToArray());
        }

        [TestMethod]
        public void HiddenTypeCountReportsWhatTheFooterSays()
        {
            var set = Set(Item(DiffStatus.Alter, "Table", "A"), Item(DiffStatus.Alter, "View", "B"), Item(DiffStatus.Alter, "User", "C"));

            Assert.AreEqual(3, set.Groups.Count);
            Assert.AreEqual(0, set.HiddenTypeCount);

            set.StatusFilter.Clear();
            set.StatusFilter.Add(DiffStatus.Create);

            Assert.AreEqual(0, set.Groups.Count);
            Assert.AreEqual(3, set.HiddenTypeCount);
        }

        private static DiffSet Set(params DiffItem[] items)
        {
            return new DiffSet(items);
        }

        private static DiffItem Item(DiffStatus status, string objectType, string name, string schema = null)
        {
            return new DiffItem(new DiffItemKey(Parse(objectType), schema, name), status)
            {
                SourceDefinition = "source",
                DestinationDefinition = "destination"
            };
        }

        private static DiffItemKey Key(string objectType, string name)
        {
            return new DiffItemKey(Parse(objectType), null, name);
        }

        private static ObjectType Parse(string objectType)
        {
            return (ObjectType)System.Enum.Parse(typeof(ObjectType), objectType);
        }
    }
}
