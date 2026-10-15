using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Ui.Model
{
    /// <summary>
    /// The script model behind the confirmation dialog (section 6.8): the statement list, the
    /// destructive flagging and the transaction decision.
    /// </summary>
    [TestClass]
    public class ScriptBundleTests
    {
        [TestMethod]
        public void FullTextAndFileGroupStatementsCannotRunInATransaction()
        {
            // Section 6.8 names full-text index changes as the example.
            Assert.IsFalse(ScriptTransactionRules.IsTransactional(ScriptAction.AddFullTextIndex));
            Assert.IsFalse(ScriptTransactionRules.IsTransactional(ScriptAction.DropFullTextIndex));
            Assert.IsFalse(ScriptTransactionRules.IsTransactional(ScriptAction.AddFileGroup));
            Assert.IsFalse(ScriptTransactionRules.IsTransactional(ScriptAction.AlterFile));
            Assert.IsFalse(ScriptTransactionRules.IsTransactional(ScriptAction.UseDatabase));

            Assert.IsTrue(ScriptTransactionRules.IsTransactional(ScriptAction.AddTable));
            Assert.IsTrue(ScriptTransactionRules.IsTransactional(ScriptAction.DropTable));
            Assert.IsTrue(ScriptTransactionRules.IsTransactional(ScriptAction.AlterProcedure));
        }

        [TestMethod]
        public void APlainScriptCanRunInOneTransactionAndSaysNothingAboutWhy()
        {
            var bundle = new ScriptBundle(new[]
            {
                Statement("dbo.A", ScriptAction.AddTable, true, false),
                Statement("dbo.B", ScriptAction.AlterView, true, false)
            });

            Assert.IsTrue(bundle.CanRunInTransaction);
            Assert.AreEqual(string.Empty, bundle.TransactionDisabledReason);
        }

        [TestMethod]
        public void AScriptWithAFullTextChangeDisablesTheTransactionOptionAndExplains()
        {
            var bundle = new ScriptBundle(new[]
            {
                Statement("dbo.A", ScriptAction.AddTable, true, false),
                Statement("dbo.Search", ScriptAction.AddFullTextIndex, false, false)
            });

            Assert.IsFalse(bundle.CanRunInTransaction);
            Assert.AreEqual("One statement cannot run inside a transaction: ADD FULL TEXT INDEX",
                bundle.TransactionDisabledReason);
        }

        [TestMethod]
        public void TheBundleCountsTheStatementsTheEngineFlaggedDestructive()
        {
            var bundle = new ScriptBundle(new[]
            {
                Statement("dbo.Legacy", ScriptAction.DropTable, true, true),
                Statement("dbo.Orders", ScriptAction.AlterTable, true, true),
                Statement("dbo.New", ScriptAction.AddTable, true, false)
            });

            Assert.AreEqual(3, bundle.Count);
            Assert.AreEqual(2, bundle.DestructiveCount);
            Assert.AreEqual(2, bundle.DestructiveStatements.Count);
        }

        [TestMethod]
        public void TheStatementListShowsTheActionAndTheFirstLine()
        {
            var statement = new ScriptStatement("dbo.Legacy", ScriptAction.DropTable, "DROP TABLE [dbo].[Legacy]\r\nGO\r\n", true, true);

            Assert.AreEqual("DROP TABLE", statement.ActionName);
            Assert.AreEqual("DROP TABLE [dbo].[Legacy]", statement.FirstLine);
        }

        [TestMethod]
        public void AnAlterThatDropsAColumnIsDescribedByItsEngineAction()
        {
            var statement = new ScriptStatement("dbo.Orders", ScriptAction.AlterTable, "ALTER TABLE [dbo].[Orders] DROP COLUMN [Old]\r\nGO\r\n", true, true);

            Assert.AreEqual("ALTER TABLE", statement.ActionName);
            Assert.IsTrue(statement.IsDestructive);
            Assert.AreEqual("ALTER TABLE [dbo].[Orders] DROP COLUMN [Old]", statement.FirstLine);
        }

        [TestMethod]
        public void TheActionNamesAreReadableForTheWholeEnum()
        {
            Assert.AreEqual("DROP TABLE", ScriptTransactionRules.Description(ScriptAction.DropTable));
            Assert.AreEqual("ADD STORED PROCEDURE", ScriptTransactionRules.Description(ScriptAction.AddStoredProcedure));
            Assert.AreEqual("NONE", ScriptTransactionRules.Description(ScriptAction.None));
        }

        [TestMethod]
        public void TheBundleTextIsEveryStatementInOrder()
        {
            var bundle = new ScriptBundle(new[]
            {
                Statement("dbo.A", ScriptAction.AddTable, true, false),
                Statement("dbo.B", ScriptAction.AddTable, true, false)
            });

            Assert.AreEqual("print 'a'\r\nprint 'b'\r\n", bundle.Text);
        }

        /// <summary>
        /// Section 6.7.1 asks the preview for per-object section comments, and the words are the model's
        /// so the preview, the copy and the saved file cannot drift apart.
        /// </summary>
        [TestMethod]
        public void ASectionHeaderIsAnnotatedIntoTheTextButNotTheStatement()
        {
            var statement = Statement("dbo.DeleteScheme", ScriptAction.AlterProcedure, true, false);

            statement.SectionHeader = "ALTER dbo.DeleteScheme";

            var bundle = new ScriptBundle(new[] { statement });

            Assert.AreEqual("-- ALTER dbo.DeleteScheme\r\nprint 'e'\r\n", bundle.Text);

            // The SQL the engine sends is untouched: the comment is the tool's, not SQL Server's.
            Assert.AreEqual("print 'e'\r\n", statement.Sql);
            Assert.AreEqual("print 'e'", statement.FirstLine);
        }

        [TestMethod]
        public void AStatementWithoutASectionHeaderIsJustItsSql()
        {
            var bundle = new ScriptBundle(new[] { Statement("dbo.A", ScriptAction.AddTable, true, false) });

            Assert.AreEqual("print 'a'\r\n", bundle.Text);
        }

        [TestMethod]
        public void TheSummaryCountsStatementsAndSaysHowManyAreDestructive()
        {
            Assert.AreEqual("2 statements", new ScriptBundle(new[]
            {
                Statement("dbo.A", ScriptAction.AddTable, true, false),
                Statement("dbo.B", ScriptAction.AddTable, true, false)
            }).Summary);

            Assert.AreEqual("1 statement, 1 destructive", new ScriptBundle(new[]
            {
                Statement("dbo.C", ScriptAction.DropTable, true, true)
            }).Summary);
        }

        private static ScriptStatement Statement(string name, ScriptAction action, bool transactional, bool destructive)
        {
            return new ScriptStatement(name, action, "print '" + name.Substring(name.Length - 1).ToLowerInvariant() + "'\r\n", transactional, destructive);
        }
    }
}
