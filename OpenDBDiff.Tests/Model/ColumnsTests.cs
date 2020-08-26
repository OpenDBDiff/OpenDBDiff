using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Compare;
using OpenDBDiff.SqlServer.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Options;
using System;
using System.Collections.Generic;

namespace OpenDBDiff.Tests.Model.Tests
{
    [TestClass()]
    public class ColumnsTests
    {
        [TestMethod()]
        public void OriginHasExtraColumn_NothingSelected_ShouldDropExtraColumn()
        {
            int idStorage = 1;
            System.Func<int> getId = new Func<int>(()=>++idStorage);
            
            Database originDatabase = new Database();
            originDatabase.Info = new DatabaseInfo()
            {
                Collation = "SQL_Latin1_General_CP1_CI_AS",
            };
            originDatabase.Options = new SqlOption();
            originDatabase.Id = getId();
            Table originTable = new Table(originDatabase);
            originTable.Name = "Example";
            originTable.Id = getId();
            var originColumn1 = new Column(originTable)
            {
                Name = "Test",
                Type = "int",
                Id = getId()
        };
            var originColumn2 = new Column(originTable)
            {
                Name = "Test2",
                Type = "varchar(20)",
                Id = getId()
            };
            var originColumn3 = new Column(originTable)
            {
                Name = "Test3",
                Type = "bigint",
                Id = getId()
            };
            originTable.Columns.Add(originColumn1);
            originTable.Columns.Add(originColumn3);
            originTable.Columns.Add(originColumn2);
            originDatabase.Tables.Add(originTable);


            Database destinationDatabase = new Database();
            destinationDatabase.Info = new DatabaseInfo()
            {
                Collation = "SQL_Latin1_General_CP1_CI_AS"
            };
            destinationDatabase.Id = getId();
            destinationDatabase.Options = new SqlOption();
            Table destinationTable = new Table(destinationDatabase);
            destinationTable.Name = "Example";
            destinationTable.Id = getId();
            var destinationColumn1 = new Column(destinationTable)
            {
                Name = "Test",
                Type = "int",
                Id = getId()
            };
            var destinationColumn3 = new Column(destinationTable)
            {
                Name = "Test3",
                Type = "bigint",
                Id = getId()
            };
            destinationTable.Columns.Add(destinationColumn1);
            destinationTable.Columns.Add(destinationColumn3);
            destinationDatabase.Tables.Add(destinationTable);


            originTable.OriginalTable = (Table)originTable.Clone((Database)originTable.Parent);
            new CompareColumns().GenerateDifferences<Table>(originTable.Columns, destinationTable.Columns);

            SQLScriptList sqlList = originTable.ToSqlDiff(new List<ISchemaBase>());
            string sql = sqlList.ToSQL();
            Assert.AreEqual(originColumn2.ToSqlDrop(), sql);
        }
        [TestMethod()]
        public void OriginHasExtraColumn_NotChangedColumnSelected_ShouldBeEmptyScript()
        {
            int idStorage = 1;
            System.Func<int> getId = new Func<int>(() => ++idStorage);
            Database originDatabase = new Database();
            originDatabase.Info = new DatabaseInfo()
            {
                Collation = "SQL_Latin1_General_CP1_CI_AS"
            };
            originDatabase.Id = getId();
            originDatabase.Options = new SqlOption();
            Table originTable = new Table(originDatabase);
            originTable.Name = "Example";
            originTable.Id = getId();
            var originColumn1 = new Column(originTable)
            {
                Name = "Test",
                Type = "int",
                Id = getId()
            };
            var originColumn2 = new Column(originTable)
            {
                Name = "Test2",
                Type = "varchar(20)",
                Id = getId()
            };
            var originColumn3 = new Column(originTable)
            {
                Name = "Test3",
                Type = "bigint",
                Id = getId()
            };
            originTable.Columns.Add(originColumn1);
            originTable.Columns.Add(originColumn3);
            originTable.Columns.Add(originColumn2);
            originDatabase.Tables.Add(originTable);


            Database destinationDatabase = new Database();
            destinationDatabase.Info = new DatabaseInfo()
            {
                Collation = "SQL_Latin1_General_CP1_CI_AS"
            };
            destinationDatabase.Id = getId();
            destinationDatabase.Options = new SqlOption();
            Table destinationTable = new Table(destinationDatabase);
            destinationTable.Name = "Example";
            destinationTable.Id = getId();
            var destinationColumn1 = new Column(destinationTable)
            {
                Name = "Test",
                Type = "int",
                Id = getId()
            };
            var destinationColumn3 = new Column(destinationTable)
            {
                Name = "Test3",
                Type = "bigint",
                Id = getId()
            };
            destinationTable.Columns.Add(destinationColumn1);
            destinationTable.Columns.Add(destinationColumn3);
            destinationDatabase.Tables.Add(destinationTable);


            originTable.OriginalTable = (Table)originTable.Clone((Database)originTable.Parent);
            new CompareColumns().GenerateDifferences<Table>(originTable.Columns, destinationTable.Columns);

            SQLScriptList sqlList = originTable.ToSqlDiff(new List<ISchemaBase>() { originColumn3 });
            string sql = sqlList.ToSQL();
            Assert.AreEqual("", sql);
        }
        [TestMethod()]
        public void OriginHasExtraColumn_ExtraColumnSelected_ShouldBeDropColumnScript()
        {
            int idStorage = 1;
            System.Func<int> getId = new Func<int>(() => ++idStorage);
            Database originDatabase = new Database();
            originDatabase.Info = new DatabaseInfo()
            {
                Collation = "SQL_Latin1_General_CP1_CI_AS"
            };
            originDatabase.Id = getId();
            originDatabase.Options = new SqlOption();
            Table originTable = new Table(originDatabase);
            originTable.Name = "Example";
            originTable.Id = getId();
            var originColumn1 = new Column(originTable)
            {
                Name = "Test",
                Type = "int",
                Id = getId()
            };
            var originColumn2 = new Column(originTable)
            {
                Name = "Test2",
                Type = "varchar(20)",
                Id = getId()
            };
            var originColumn3 = new Column(originTable)
            {
                Name = "Test3",
                Type = "bigint",
                Id = getId()
            };
            originTable.Columns.Add(originColumn1);
            originTable.Columns.Add(originColumn3);
            originTable.Columns.Add(originColumn2);
            originDatabase.Tables.Add(originTable);


            Database destinationDatabase = new Database();
            destinationDatabase.Info = new DatabaseInfo()
            {
                Collation = "SQL_Latin1_General_CP1_CI_AS"
            };
            destinationDatabase.Id = getId();
            destinationDatabase.Options = new SqlOption();
            Table destinationTable = new Table(destinationDatabase);
            destinationTable.Name = "Example";
            destinationTable.Id = getId();
            var destinationColumn1 = new Column(destinationTable)
            {
                Name = "Test",
                Type = "int",
                Id = getId()
            };
            var destinationColumn3 = new Column(destinationTable)
            {
                Name = "Test3",
                Type = "bigint",
                Id = getId()
            };
            destinationTable.Columns.Add(destinationColumn1);
            destinationTable.Columns.Add(destinationColumn3);
            destinationDatabase.Tables.Add(destinationTable);


            originTable.OriginalTable = (Table)originTable.Clone((Database)originTable.Parent);
            new CompareColumns().GenerateDifferences<Table>(originTable.Columns, destinationTable.Columns);

            SQLScriptList sqlList = originTable.ToSqlDiff(new List<ISchemaBase>() { originColumn2 });
            string sql = sqlList.ToSQL();
            Assert.AreEqual(originColumn2.ToSqlDrop(), sql);
        }
    }
}
