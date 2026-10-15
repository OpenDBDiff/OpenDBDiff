using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Ui.Model
{
    /// <summary>
    /// Section 9.8's undo: fifty steps of inclusion changes, reset on each compare, and a redo tail that
    /// a new change discards.
    /// </summary>
    [TestClass]
    public class InclusionHistoryTests
    {
        [TestMethod]
        public void AFreshStackHasNothingToUndo()
        {
            var history = new InclusionHistory();

            Assert.IsFalse(history.CanUndo);
            Assert.IsFalse(history.CanRedo);
            Assert.AreEqual(0, history.Count);
        }

        [TestMethod]
        public void TheCompareItselfIsTheBaseline()
        {
            var history = new InclusionHistory();
            history.Reset("compare", Keys("A"));

            Assert.AreEqual(1, history.Count);
            Assert.IsFalse(history.CanUndo, "there is nothing before the compare to return to");
        }

        [TestMethod]
        public void UndoReturnsTheStateBeforeTheChangeAndCarriesItsLabel()
        {
            var history = new InclusionHistory();
            history.Reset("compare", Keys("A"));
            history.Record("Select dbo.B", Keys("A", "B"));

            Assert.AreEqual("Select dbo.B", history.UndoLabel, "what Ctrl+Z would take back");

            var restored = history.Undo();

            Assert.AreEqual("compare", restored.Label, "the state to go back to");
            Assert.AreEqual(1, restored.Included.Count);
            Assert.IsFalse(history.CanUndo);
            Assert.IsTrue(history.CanRedo);
        }

        [TestMethod]
        public void RedoWalksForwardAgain()
        {
            var history = new InclusionHistory();
            history.Reset("compare", Keys("A"));
            history.Record("Select dbo.B", Keys("A", "B"));
            history.Undo();

            Assert.AreEqual("Select dbo.B", history.RedoLabel);

            var restored = history.Redo();

            Assert.AreEqual(2, restored.Included.Count);
            Assert.IsFalse(history.CanRedo);
        }

        [TestMethod]
        public void ANewChangeAfterAnUndoDropsTheRedoTail()
        {
            var history = new InclusionHistory();
            history.Reset("compare", Keys("A"));
            history.Record("Select dbo.B", Keys("A", "B"));
            history.Undo();
            history.Record("Select dbo.C", Keys("A", "C"));

            Assert.IsFalse(history.CanRedo, "a single undo stack, not two");
            Assert.AreEqual(2, history.Count, "the compare, then the new change");
        }

        [TestMethod]
        public void TheStackHoldsFiftySteps()
        {
            var history = new InclusionHistory();
            history.Reset("compare", Keys("A"));

            for (var i = 0; i < 80; i++)
            {
                history.Record("step " + i, Keys("A"));
            }

            Assert.AreEqual(InclusionHistory.MaxSteps, history.Count);

            // The oldest steps fall off the bottom, so the newest are still reachable.
            Assert.AreEqual("step 79", history.UndoLabel);

            for (var i = 0; i < InclusionHistory.MaxSteps - 1; i++)
            {
                history.Undo();
            }

            Assert.IsFalse(history.CanUndo);
        }

        [TestMethod]
        public void ANewCompareResetsEverything()
        {
            var history = new InclusionHistory();
            history.Reset("compare", Keys("A"));
            history.Record("Select dbo.B", Keys("A", "B"));

            history.Reset("compare again", Keys("C"));

            Assert.AreEqual(1, history.Count);
            Assert.IsFalse(history.CanUndo);
            CollectionAssert.AreEqual(new[] { "compare again" }, history.Labels.ToArray());
        }

        [TestMethod]
        public void UndoingPastTheEndIsHarmless()
        {
            var history = new InclusionHistory();

            Assert.IsNull(history.Undo());
            Assert.IsNull(history.Redo());

            history.Reset("compare", Keys("A"));

            Assert.IsNull(history.Undo());
            Assert.IsNull(history.Redo());
        }

        private static IReadOnlyList<DiffItemKey> Keys(params string[] names)
        {
            var keys = new List<DiffItemKey>();

            foreach (var name in names)
            {
                keys.Add(new DiffItemKey(ObjectType.Table, "dbo", name));
            }

            return keys;
        }
    }
}
