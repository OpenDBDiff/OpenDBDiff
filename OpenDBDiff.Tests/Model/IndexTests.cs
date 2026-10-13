using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.SqlServer.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Options;

namespace OpenDBDiff.Tests.Model.Tests
{
    [TestClass()]
    public class IndexTests
    {
        private static Index CreateIndex(string name)
        {
            Database database = new Database();
            database.Info = new DatabaseInfo();
            database.Options = new SqlOption();
            Table table = new Table(database) { Name = "Photos", Owner = "dbo" };
            database.Tables.Add(table);
            Index index = new Index(table)
            {
                Name = name,
                Owner = "dbo",
                Type = Index.IndexTypeEnum.Nonclustered
            };
            index.Columns.Add(new IndexColumn(table) { Name = "LastChanged" });
            table.Indexes.Add(index);
            return index;
        }

        [TestMethod()]
        public void ToSql_NameWithClosingBracket_EscapesBracket()
        {
            Index index = CreateIndex("IX_Photos__LastChanged [+FileDeleted]");

            StringAssert.Contains(index.ToSql(), "INDEX [IX_Photos__LastChanged [+FileDeleted]]] ON [dbo].[Photos]");
            StringAssert.Contains(index.ToSqlDrop(), "DROP INDEX [IX_Photos__LastChanged [+FileDeleted]]] ON [dbo].[Photos]");
        }

        [TestMethod()]
        public void ToSql_NameWithPlus_KeepsName()
        {
            Index index = CreateIndex("IX_Photos__LastChanged+FileDeleted");

            StringAssert.Contains(index.ToSql(), "INDEX [IX_Photos__LastChanged+FileDeleted] ON [dbo].[Photos]");
        }
    }
}
