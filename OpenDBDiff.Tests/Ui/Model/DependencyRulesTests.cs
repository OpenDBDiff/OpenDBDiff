using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Ui.Model
{
    /// <summary>
    /// The dependency rules of section 9.4: which combination blocks an apply, which only warns, and
    /// what a fix would do.
    /// </summary>
    [TestClass]
    public class DependencyRulesTests
    {
        [TestMethod]
        public void ADependencyThatIsCreatedAndNotIncludedBlocksTheApply()
        {
            var view = Item(DiffStatus.Create, ObjectType.View, "dbo.Report");
            var table = Item(DiffStatus.Create, ObjectType.Table, "dbo.NewTable");

            var graph = new FakeGraph().With(view.Key, table.Key);
            var warnings = DependencyRules.Analyze(graph, new[] { view, table }, key => key.Equals(view.Key));

            var warning = warnings.Single(w => w.Kind == DependencyWarningKind.RequiredDependencyNotIncluded);

            Assert.AreEqual(DependencySeverity.Blocking, warning.Severity);
            Assert.AreEqual(table.Key, warning.Related);
            Assert.IsTrue(warning.Message.Contains("dbo.NewTable"), warning.Message);
            Assert.IsTrue(warning.Message.Contains("does not exist in the destination"), warning.Message);
            Assert.AreEqual(DependencyFixKind.SelectDependency, warning.Fix.Kind);
        }

        [TestMethod]
        public void ADependencyThatIsOnlyAlteredAndNotIncludedWarns()
        {
            var procedure = Item(DiffStatus.Create, ObjectType.StoredProcedure, "dbo.P");
            var table = Item(DiffStatus.Alter, ObjectType.Table, "dbo.Orders");

            var graph = new FakeGraph().With(procedure.Key, table.Key);
            var warnings = DependencyRules.Analyze(graph, new[] { procedure, table }, key => key.Equals(procedure.Key));

            var warning = warnings.Single(w => w.Kind == DependencyWarningKind.DependencyAlterNotIncluded);

            Assert.AreEqual(DependencySeverity.Warning, warning.Severity);
            Assert.IsFalse(DependencyRules.Analyze(graph, new[] { procedure, table }, key => key.Equals(procedure.Key))
                .Any(w => w.IsBlocking));
        }

        [TestMethod]
        public void ADependencyThatIsAlreadyIncludedIsNotAWarning()
        {
            var view = Item(DiffStatus.Create, ObjectType.View, "dbo.Report");
            var table = Item(DiffStatus.Create, ObjectType.Table, "dbo.NewTable");

            var graph = new FakeGraph().With(view.Key, table.Key);
            var warnings = DependencyRules.Analyze(graph, new[] { view, table }, key => true);

            Assert.AreEqual(0, warnings.Count);
        }

        [TestMethod]
        public void ADependencyThatIsUnchangedInTheDestinationIsNotAProblem()
        {
            // Section 9.4's fourth row.
            var view = Item(DiffStatus.Alter, ObjectType.View, "dbo.Report");
            var table = Item(DiffStatus.Identical, ObjectType.Table, "dbo.Orders");

            var graph = new FakeGraph().With(view.Key, table.Key);

            Assert.AreEqual(0, DependencyRules.Analyze(graph, new[] { view, table }, key => key.Equals(view.Key)).Count);
        }

        [TestMethod]
        public void DroppingSomethingAnIncludedItemStillNeedsBlocksTheApply()
        {
            var table = Item(DiffStatus.Drop, ObjectType.Table, "dbo.Orders");
            var view = Item(DiffStatus.Identical, ObjectType.View, "dbo.Report");

            var graph = new FakeGraph().With(view.Key, table.Key);

            // The view depends on the table, so dropping the table while including it breaks the view.
            var warnings = DependencyRules.Analyze(graph, new[] { table, view }, key => key.Equals(table.Key) || key.Equals(view.Key));
            var warning = warnings.Single(w => w.Kind == DependencyWarningKind.DependentNotDropped);

            Assert.AreEqual(DependencySeverity.Blocking, warning.Severity);
            Assert.AreEqual(table.Key, warning.Item);
            Assert.AreEqual(DependencyFixKind.DeselectItem, warning.Fix.Kind);
        }

        [TestMethod]
        public void DroppingSomethingWhoseDependentsAreAlsoDroppedIsFine()
        {
            var table = Item(DiffStatus.Drop, ObjectType.Table, "dbo.Orders");
            var view = Item(DiffStatus.Drop, ObjectType.View, "dbo.Report");

            var graph = new FakeGraph().With(view.Key, table.Key);
            var warnings = DependencyRules.Analyze(graph, new[] { table, view }, key => true);

            Assert.AreEqual(0, warnings.Count);
        }

        [TestMethod]
        public void AnExcludedItemIsNeverWarnedAbout()
        {
            var view = Item(DiffStatus.Create, ObjectType.View, "dbo.Report");
            var table = Item(DiffStatus.Create, ObjectType.Table, "dbo.NewTable");

            var graph = new FakeGraph().With(view.Key, table.Key);

            Assert.AreEqual(0, DependencyRules.Analyze(graph, new[] { view, table }, key => false).Count);
        }

        [TestMethod]
        public void ACycleIsAWarningNotABlock()
        {
            var first = Item(DiffStatus.Create, ObjectType.View, "dbo.A");
            var second = Item(DiffStatus.Create, ObjectType.View, "dbo.B");

            var graph = new FakeGraph().With(first.Key, second.Key).With(second.Key, first.Key);
            var warnings = DependencyRules.Analyze(graph, new[] { first, second }, key => true);

            var cycles = warnings.Where(w => w.Kind == DependencyWarningKind.Circular).ToList();

            Assert.AreEqual(1, cycles.Count, "one warning per cycle, not one per edge");
            Assert.IsFalse(cycles[0].IsBlocking);
            Assert.IsFalse(cycles[0].Fix.Exists);
        }

        [TestMethod]
        public void TheApplyTooltipCountsTheBlockingProblems()
        {
            Assert.AreEqual("Fix 1 dependency problem first", DependencyRules.FixFirstText(1));
            Assert.AreEqual("Fix 3 dependency problems first", DependencyRules.FixFirstText(3));
        }

        [TestMethod]
        public void AnEmptyGraphIsNotAProblem()
        {
            var item = Item(DiffStatus.Create, ObjectType.Table, "dbo.A");
            var warnings = DependencyRules.Analyze(new FakeGraph(), new[] { item }, key => true);

            Assert.AreEqual(0, warnings.Count);
        }

        private static DiffItem Item(DiffStatus status, ObjectType type, string name)
        {
            var parts = name.Split('.');

            return new DiffItem(new DiffItemKey(type, parts.Length > 1 ? parts[0] : null, parts[parts.Length - 1]), status);
        }

        /// <summary>A graph written as pairs, so a test reads as the relationship it is about.</summary>
        private sealed class FakeGraph : IDependencyGraph
        {
            private readonly Dictionary<DiffItemKey, List<DiffItemKey>> dependencies = new Dictionary<DiffItemKey, List<DiffItemKey>>();
            private readonly Dictionary<DiffItemKey, List<DiffItemKey>> dependents = new Dictionary<DiffItemKey, List<DiffItemKey>>();

            /// <summary>Records that <paramref name="dependent"/> needs <paramref name="dependency"/>.</summary>
            public FakeGraph With(DiffItemKey dependent, DiffItemKey dependency)
            {
                if (!dependencies.ContainsKey(dependent))
                {
                    dependencies[dependent] = new List<DiffItemKey>();
                }

                if (!dependents.ContainsKey(dependency))
                {
                    dependents[dependency] = new List<DiffItemKey>();
                }

                dependencies[dependent].Add(dependency);
                dependents[dependency].Add(dependent);

                return this;
            }

            public IReadOnlyList<DiffItemKey> DependenciesOf(DiffItemKey key)
            {
                List<DiffItemKey> found;

                return dependencies.TryGetValue(key, out found) ? found : new List<DiffItemKey>();
            }

            public IReadOnlyList<DiffItemKey> DependentsOf(DiffItemKey key)
            {
                List<DiffItemKey> found;

                return dependents.TryGetValue(key, out found) ? found : new List<DiffItemKey>();
            }
        }
    }
}
