using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model.Util;
using OpenDBDiff.Tests.Utils;
using System;

namespace OpenDBDiff.Tests.Model.Tests
{
    [TestClass]
    public class FormatCodeTests
    {
        private readonly ResourceFileExtractor extractor;

        public FormatCodeTests()
        {
            this.extractor = new ResourceFileExtractor(".SqlSnippets.Triggers.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FindAndNormalizeCreate_BrokenThrowsException()
        {
            var triggerItemMock = new Mock<ISchemaBase>();
            triggerItemMock.Setup(x => x.FullName).Returns("[dbo].[iutrgTriggerName]");

            FormatCode.FindAndNormalizeCreate(triggerItemMock.Object, extractor.ReadFileFromResource("broken.sql"));
        }

        [TestMethod]
        public void FindAndNormalizeCreate_NoBracket_ShouldNormalize()
        {
            var triggerItemMock = new Mock<ISchemaBase>();
            triggerItemMock.Setup(x => x.FullName).Returns("[dbo].[iutrgTriggerName]");

            var a = FormatCode.FindAndNormalizeCreate(triggerItemMock.Object, extractor.ReadFileFromResource("no-brackets.sql"));

            var normalized = extractor.ReadFileFromResource("normalized.sql");
            Assert.AreEqual(normalized, a.Body);
            Assert.AreEqual(0, a.FindPosition);
        }

        [TestMethod]
        public void FindAndNormalizeCreate_NoOwner_ShouldNormalize()
        {
            var triggerItemMock = new Mock<ISchemaBase>();
            triggerItemMock.Setup(x => x.FullName).Returns("[dbo].[iutrgTriggerName]");

            var a = FormatCode.FindAndNormalizeCreate(triggerItemMock.Object, extractor.ReadFileFromResource("no-owner.sql"));

            var normalized = extractor.ReadFileFromResource("normalized.sql");
            Assert.AreEqual(normalized, a.Body);
            Assert.AreEqual(0, a.FindPosition);
        }

        [TestMethod]
        public void FindAndNormalizeCreate_NormalizedSql_ShouldKeepSourceText()
        {
            var triggerItemMock = new Mock<ISchemaBase>();
            triggerItemMock.Setup(x => x.FullName).Returns("[dbo].[iutrgTriggerName]");

            var normalized = extractor.ReadFileFromResource("normalized.sql");
            var a = FormatCode.FindAndNormalizeCreate(triggerItemMock.Object, normalized);

            Assert.AreEqual(normalized, a.Body);
            Assert.AreEqual(0, a.FindPosition);
        }

        [TestMethod]
        public void FindAndNormalizeCreate_WithComment_ShouldNormalize()
        {
            var triggerItemMock = new Mock<ISchemaBase>();
            triggerItemMock.Setup(x => x.FullName).Returns("[dbo].[iutrgTriggerName]");

            var a = FormatCode.FindAndNormalizeCreate(triggerItemMock.Object, extractor.ReadFileFromResource("with-comments.sql"));

            var normalized = extractor.ReadFileFromResource("normalized.sql");
            Assert.AreEqual(62, a.FindPosition);
            Assert.AreEqual(normalized, a.Body.Substring(a.FindPosition));
        }

        [TestMethod]
        public void FindCreate_BrokenReturnsNull()
        {
            var findResult = FormatCode.FindCreate(extractor.ReadFileFromResource("broken.sql"));
            Assert.IsNull(findResult);
        }

        [TestMethod]
        public void FindCreate_NoBracket_ShouldFindTheCreatePositions()
        {
            var findResult = FormatCode.FindCreate(extractor.ReadFileFromResource("no-brackets.sql"));

            Assert.AreEqual(0, findResult.CreateBeginPosition);
            Assert.AreEqual(13, findResult.TypeEndPosition);
            Assert.AreEqual(30, findResult.NameEndPosition);
        }

        [TestMethod]
        public void FindCreate_NoOwner_ShouldFindTheCreatePositions()
        {
            var findResult = FormatCode.FindCreate(extractor.ReadFileFromResource("no-owner.sql"));

            Assert.AreEqual(0, findResult.CreateBeginPosition);
            Assert.AreEqual(13, findResult.TypeEndPosition);
            Assert.AreEqual(32, findResult.NameEndPosition);
        }

        [TestMethod]
        public void FindCreate_Normalized_ShouldFindTheCreatePositions()
        {
            var findResult = FormatCode.FindCreate(extractor.ReadFileFromResource("normalized.sql"));

            Assert.AreEqual(0, findResult.CreateBeginPosition);
            Assert.AreEqual(13, findResult.TypeEndPosition);
            Assert.AreEqual(38, findResult.NameEndPosition);
        }

        [TestMethod]
        public void FindCreate_WithComment_ShouldFindTheCreatePositions()
        {
            var findResult = FormatCode.FindCreate(extractor.ReadFileFromResource("with-comments.sql"));

            Assert.AreEqual(62, findResult.CreateBeginPosition);
            Assert.AreEqual(75, findResult.TypeEndPosition);
            Assert.AreEqual(100, findResult.NameEndPosition);
        }
    }
}
