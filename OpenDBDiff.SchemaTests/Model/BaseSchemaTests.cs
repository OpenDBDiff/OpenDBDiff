using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Schema;
using OpenDBDiff.Schema.Model;

namespace OpenDBDiff.SchemaTests
{
    [TestClass]
    public class SchemaBaseTests
    {
        [TestMethod]
        public void SchemaHasANullParent_SettingStatusToRebuild_ShouldSucceed()
        {
            var schema = new MockSchema
            {
                Parent = null,
                Status = ObjectStatus.Rebuild
            };
            Assert.IsNotNull(schema);
        }

        private class MockSchema : SchemaBase
        {
            public MockSchema()
                : base("[", "]", ObjectType.CLRFunction)
            {
            }

            public override string ToSql()
            {
                throw new System.NotImplementedException();
            }

            public override string ToSqlAdd()
            {
                throw new System.NotImplementedException();
            }

            public override string ToSqlDrop()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
