using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;

namespace OpenDBDiff.Tests.Model
{
    /// <summary>
    /// The schema tree's visibility rules. These used to be reachable only by opening a window, so
    /// they are pinned here now that both front ends call the same code.
    /// </summary>
    [TestClass]
    public class SchemaVisibilityTests
    {
        [TestMethod]
        public void UnchangedObject_IsHiddenUnlessUnchangedItemsAreShown()
        {
            var item = Item(ObjectStatus.Original);

            Assert.IsFalse(SchemaVisibilityEvaluator.IsVisible(item, SchemaVisibility.Default));
            Assert.IsTrue(SchemaVisibilityEvaluator.IsVisible(item, SchemaVisibility.Unchanged));
        }

        [TestMethod]
        public void NewObject_IsHiddenWhenNewItemsAreHidden()
        {
            var item = Item(ObjectStatus.Create);

            Assert.IsTrue(SchemaVisibilityEvaluator.IsVisible(item, SchemaVisibility.Default));
            Assert.IsFalse(SchemaVisibilityEvaluator.IsVisible(item, SchemaVisibility.Missing | SchemaVisibility.Changed));
        }

        [TestMethod]
        public void DroppedObject_IsHiddenWhenMissingItemsAreHidden()
        {
            var item = Item(ObjectStatus.Drop);

            Assert.IsTrue(SchemaVisibilityEvaluator.IsVisible(item, SchemaVisibility.Default));
            Assert.IsFalse(SchemaVisibilityEvaluator.IsVisible(item, SchemaVisibility.New | SchemaVisibility.Changed));
        }

        [DataTestMethod]
        [DataRow(ObjectStatus.Alter)]
        [DataRow(ObjectStatus.AlterBody)]
        [DataRow(ObjectStatus.AlterWhitespace)]
        [DataRow(ObjectStatus.Bind)]
        [DataRow(ObjectStatus.ChangeOwner)]
        [DataRow(ObjectStatus.Disabled)]
        [DataRow(ObjectStatus.DropOlder)]
        [DataRow(ObjectStatus.PermissionSet)]
        [DataRow(ObjectStatus.Rebuild)]
        [DataRow(ObjectStatus.RebuildDependencies)]
        [DataRow(ObjectStatus.Update)]
        public void ChangedObject_IsHiddenWhenChangedItemsAreHidden(ObjectStatus status)
        {
            var item = Item(status);

            Assert.IsTrue(SchemaVisibilityEvaluator.IsVisible(item, SchemaVisibility.Default));
            Assert.IsFalse(SchemaVisibilityEvaluator.IsVisible(item, SchemaVisibility.New | SchemaVisibility.Missing));
        }

        [TestMethod]
        public void CombinedStatuses_AreVisibleIfAnyOfThemIs()
        {
            var item = Item(ObjectStatus.Drop | ObjectStatus.Alter);

            Assert.IsTrue(SchemaVisibilityEvaluator.IsVisible(item, SchemaVisibility.Missing));
            Assert.IsTrue(SchemaVisibilityEvaluator.IsVisible(item, SchemaVisibility.Changed));
            Assert.IsFalse(SchemaVisibilityEvaluator.IsVisible(item, SchemaVisibility.New));
        }

        /// <summary>
        /// The WinForms tree throws when it meets a status it has no rule for, which only ever fired
        /// in front of a user. This is the same check, run in CI.
        /// </summary>
        [TestMethod]
        public void EveryStatus_HasAVisibilityRule()
        {
            var unhandled = SchemaVisibilityEvaluator.UnhandledStatuses();

            Assert.AreEqual(0, unhandled.Count,
                "ObjectStatus values with no visibility rule: " + string.Join(", ", unhandled.Select(status => status.ToString())));
        }

        private static ISchemaBase Item(ObjectStatus status)
        {
            var item = new Mock<ISchemaBase>();
            item.SetupGet(schema => schema.Status).Returns(status);

            // Mirrors SchemaBase.HasState.
            item.Setup(schema => schema.HasState(It.IsAny<ObjectStatus>()))
                .Returns((ObjectStatus find) => (status & find) == find);

            return item.Object;
        }
    }
}
