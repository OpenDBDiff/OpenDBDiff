using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.SqlServer.Driver;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Driver
{
    /// <summary>
    /// The parts of the script and apply rules that do not need a server: which actions count as a
    /// destructive object drop, which statements carry a column drop, and the order items execute in.
    /// </summary>
    [TestClass]
    public class SqlServerScriptRulesTests
    {
        [TestMethod]
        public void TheObjectDropsOfSection97AreSpottedByAction()
        {
            Assert.IsTrue(SqlServerScriptBuilder.IsDropObjectAction(ScriptAction.DropTable));
            Assert.IsTrue(SqlServerScriptBuilder.IsDropObjectAction(ScriptAction.DropStoredProcedure));
            Assert.IsTrue(SqlServerScriptBuilder.IsDropObjectAction(ScriptAction.DropTrigger));
            Assert.IsTrue(SqlServerScriptBuilder.IsDropObjectAction(ScriptAction.DropUser));
            Assert.IsTrue(SqlServerScriptBuilder.IsDropObjectAction(ScriptAction.DropRole));

            // DropView and DropFunction share their enum value, so one name covers both.
            Assert.IsTrue(SqlServerScriptBuilder.IsDropObjectAction(ScriptAction.DropView));
        }

        [TestMethod]
        public void AnIndexOrConstraintDropIsNotOnTheSectionsList()
        {
            // Section 9.7 names object drops, column drops, narrowing types and TRUNCATE; an index or
            // constraint that has to be rebuilt on the way is not one of them, and flagging it would
            // demand a typed confirmation for a routine alter.
            Assert.IsFalse(SqlServerScriptBuilder.IsDropObjectAction(ScriptAction.DropIndex));
            Assert.IsFalse(SqlServerScriptBuilder.IsDropObjectAction(ScriptAction.DropConstraint));
            Assert.IsFalse(SqlServerScriptBuilder.IsDropObjectAction(ScriptAction.DropConstraintFK));
            Assert.IsFalse(SqlServerScriptBuilder.IsDropObjectAction(ScriptAction.AlterTable));
            Assert.IsFalse(SqlServerScriptBuilder.IsDropObjectAction(ScriptAction.AddTable));
        }

        [TestMethod]
        public void AColumnDropRidesOnTheTablesAlterStatement()
        {
            var kinds = DestructiveKinds.DropColumn;

            Assert.IsTrue(SqlServerScriptBuilder.IsDestructive(ScriptAction.AlterTable, kinds));
            Assert.IsTrue(SqlServerScriptBuilder.IsDestructive(ScriptAction.RebuildTable, kinds));

            // The flag is on the statement the column drop appears in, and nowhere else.
            Assert.IsFalse(SqlServerScriptBuilder.IsDestructive(ScriptAction.AddIndex, kinds));
            Assert.IsFalse(SqlServerScriptBuilder.IsDestructive(ScriptAction.AddConstraint, kinds));
        }

        [TestMethod]
        public void NothingIsDestructiveWithoutAKindOrADropAction()
        {
            Assert.IsFalse(SqlServerScriptBuilder.IsDestructive(ScriptAction.AlterTable, DestructiveKinds.None));
            Assert.IsFalse(SqlServerScriptBuilder.IsDestructive(ScriptAction.AddTable, DestructiveKinds.None));
            Assert.IsTrue(SqlServerScriptBuilder.IsDestructive(ScriptAction.DropTable, DestructiveKinds.None));
        }

        [TestMethod]
        public void ItemsExecuteInTheOrderTheirFirstStatementAppears()
        {
            var first = Key("dbo.A");
            var second = Key("dbo.B");

            var script = Script(new[]
            {
                Statement(second),
                Statement(first),
                Statement(second)
            });

            var order = SqlServerApplySession.OrderOf(script, new[] { first, second });

            CollectionAssert.AreEqual(new[] { second, first }, order.ToArray());
        }

        [TestMethod]
        public void AnIncludedItemWithNoStatementsStillAppearsInTheReport()
        {
            var silent = Key("dbo.Silent");
            var other = Key("dbo.Other");

            var order = SqlServerApplySession.OrderOf(Script(new[] { Statement(other) }), new[] { other, silent });

            CollectionAssert.AreEqual(new[] { other, silent }, order.ToArray());
        }

        [TestMethod]
        public void AnEmptySelectionHasNoOrder()
        {
            Assert.AreEqual(0, SqlServerApplySession.OrderOf(Script(new ScriptStatement[0]), new DiffItemKey[0]).Count);
            Assert.AreEqual(0, SqlServerApplySession.OrderOf(null, null).Count);
        }

        [TestMethod]
        public void TheGraphNormalisesBracketedAndPlainNamesTheSameWay()
        {
            Assert.AreEqual("dbo.Orders", SqlServerDependencyGraph.Normalize("[dbo].[Orders]"));
            Assert.AreEqual("dbo.Orders", SqlServerDependencyGraph.Normalize("dbo.Orders"));
            Assert.AreEqual(string.Empty, SqlServerDependencyGraph.Normalize(null));
        }

        private static GeneratedScript Script(IEnumerable<ScriptStatement> statements)
        {
            var list = statements.ToList();
            var byItem = new Dictionary<DiffItemKey, List<ScriptStatement>>();

            foreach (var statement in list)
            {
                var key = Key(statement.ObjectFullName);

                if (!byItem.ContainsKey(key))
                {
                    byItem[key] = new List<ScriptStatement>();
                }

                byItem[key].Add(statement);
            }

            return new GeneratedScript(list, byItem);
        }

        private static ScriptStatement Statement(DiffItemKey key)
        {
            return new ScriptStatement(key.FullName, ScriptAction.AlterTable, "print '" + key.Name + "'\r\n", true, false);
        }

        private static DiffItemKey Key(string fullName)
        {
            var parts = fullName.Split('.');

            return new DiffItemKey(ObjectType.Table, parts.Length > 1 ? parts[0] : "dbo", parts[parts.Length - 1]);
        }
    }
}
