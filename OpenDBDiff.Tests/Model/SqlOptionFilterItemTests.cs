using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Options;

namespace OpenDBDiff.Tests.Model.Tests
{
    [TestClass]
    public class SqlOptionFilterItemTests
    {
        private static bool IsUserMatch(string pattern, string name, ObjectType objectType = ObjectType.User)
        {
            var item = new Mock<ISchemaBase>();
            item.SetupGet(i => i.ObjectType).Returns(objectType);
            item.SetupGet(i => i.Name).Returns(name);

            return new SqlOptionFilterItem(ObjectType.User, pattern).IsMatch(item.Object);
        }

        [TestMethod]
        public void IsMatch_DomainWildcard_MatchesAllUsersOfThatDomain()
        {
            Assert.IsTrue(IsUserMatch(@"CONTOSO\*", @"CONTOSO\user1"));
            Assert.IsTrue(IsUserMatch(@"contoso\*", @"CONTOSO\user1"));
            Assert.IsFalse(IsUserMatch(@"CONTOSO\*", @"OTHER\user1"));
            Assert.IsFalse(IsUserMatch(@"CONTOSO\*", "CONTOSO.user1"));
        }

        [TestMethod]
        public void IsMatch_WithoutWildcards_ComparesWholeName()
        {
            Assert.IsTrue(IsUserMatch(@"CONTOSO\user1", @"CONTOSO\user1"));
            Assert.IsFalse(IsUserMatch(@"CONTOSO\user1", @"CONTOSO\user12"));
        }

        [TestMethod]
        public void IsMatch_RegexCharacters_AreMatchedLiterally()
        {
            Assert.IsTrue(IsUserMatch("app.*", "app.reader"));
            Assert.IsFalse(IsUserMatch("app.*", "appxreader"));
            Assert.IsTrue(IsUserMatch("user[1]*", "user[1]x"));
            Assert.IsTrue(IsUserMatch("user?", "user1"));
        }

        [TestMethod]
        public void IsMatch_EscapedWildcard_IsMatchedLiterally()
        {
            Assert.IsTrue(IsUserMatch("a~*b", "a*b"));
            Assert.IsFalse(IsUserMatch("a~*b", "axb"));
        }

        [TestMethod]
        public void IsMatch_OtherObjectType_DoesNotMatch()
        {
            Assert.IsFalse(IsUserMatch(@"CONTOSO\*", @"CONTOSO\user1", ObjectType.Table));
        }
    }
}
