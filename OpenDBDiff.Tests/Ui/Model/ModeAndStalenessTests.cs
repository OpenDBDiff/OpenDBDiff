using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Ui.Model
{
    /// <summary>
    /// The Schema / Data mode switch (section 6.1.5) and the stale-results rules (section 9.6).
    /// </summary>
    [TestClass]
    public class ModeAndStalenessTests
    {
        [TestMethod]
        public void DataModeNeedsASchemaCompareAndATableOnBothSides()
        {
            Assert.IsFalse(CompareModeRules.IsDataEnabled(false, 0), "nothing compared yet");
            Assert.IsFalse(CompareModeRules.IsDataEnabled(false, 5), "a table count without a compare proves nothing");
            Assert.IsFalse(CompareModeRules.IsDataEnabled(true, 0), "no table exists on both sides");

            Assert.IsTrue(CompareModeRules.IsDataEnabled(true, 1));
            Assert.IsTrue(CompareModeRules.IsDataEnabled(true, 12));
        }

        [TestMethod]
        public void OnlyTablesThatExistOnBothSidesCount()
        {
            var items = new[]
            {
                Item(DiffStatus.Identical, ObjectType.Table, "Both"),
                Item(DiffStatus.Alter, ObjectType.Table, "Changed"),
                Item(DiffStatus.Create, ObjectType.Table, "OnlyInSource"),
                Item(DiffStatus.Drop, ObjectType.Table, "OnlyInDestination"),
                Item(DiffStatus.Alter, ObjectType.View, "NotATable"),
                Item(DiffStatus.Identical, ObjectType.StoredProcedure, "AlsoNotATable")
            };

            Assert.AreEqual(2, CompareModeRules.TablesOnBothSides(items));
            Assert.IsTrue(CompareModeRules.IsDataEnabled(true, CompareModeRules.TablesOnBothSides(items)));
        }

        [TestMethod]
        public void NoTablesOnBothSidesLeavesDataModeDisabled()
        {
            var items = new[] { Item(DiffStatus.Create, ObjectType.Table, "New"), Item(DiffStatus.Drop, ObjectType.Table, "Old") };

            Assert.AreEqual(0, CompareModeRules.TablesOnBothSides(items));
            Assert.IsFalse(CompareModeRules.IsDataEnabled(true, CompareModeRules.TablesOnBothSides(items)));
            Assert.AreEqual("Compare schema first, then compare data for tables that exist on both sides.",
                CompareModeRules.DataDisabledTooltip);
        }

        [TestMethod]
        public void EveryReasonFromSection96MakesResultsStale()
        {
            Assert.IsFalse(StaleRules.MakesResultsStale(CompareStaleness.Fresh));

            Assert.IsTrue(StaleRules.MakesResultsStale(CompareStaleness.ConnectionChanged));
            Assert.IsTrue(StaleRules.MakesResultsStale(CompareStaleness.DirectionSwapped));
            Assert.IsTrue(StaleRules.MakesResultsStale(CompareStaleness.OptionsChanged));
            Assert.IsTrue(StaleRules.MakesResultsStale(CompareStaleness.ProjectLoaded));
            Assert.IsTrue(StaleRules.MakesResultsStale(CompareStaleness.Applied));
        }

        [TestMethod]
        public void TheBannerAndRibbonUseTheSpecifiedCopy()
        {
            Assert.AreEqual("Results are out of date. Compare again to refresh.", StaleRules.BannerText);
            Assert.AreEqual("From an earlier comparison", StaleRules.RibbonText);
        }

        private static DiffItem Item(DiffStatus status, ObjectType type, string name)
        {
            return new DiffItem(new DiffItemKey(type, null, name), status);
        }
    }
}
