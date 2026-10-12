using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Generates;
using OpenDBDiff.SqlServer.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Options;
using System.Collections.Generic;

namespace OpenDBDiff.Tests.Model.Tests
{
    [TestClass]
    public class PermissionTests
    {
        private static Database CreateDatabase()
        {
            Database database = new Database();
            database.Options = new SqlOption();
            database.Info = new DatabaseInfo();
            return database;
        }

        private static Permission CreatePermission(Database database, string state)
        {
            Permission permission = new Permission(database)
            {
                Class = Permission.SecurableClass.Object,
                PermissionName = "EXECUTE",
                State = state,
                Owner = "dbo",
                SecurableName = "[dbo].[MyProc]",
                ColumnName = "",
                Grantee = "MyUser",
            };
            permission.SetName();
            return permission;
        }

        private static string ToSqlDiff(Permission permission)
        {
            return permission.ToSqlDiff(new List<ISchemaBase>()).ToSQL();
        }

        [TestMethod]
        public void SetName_ObjectPermission_ShouldIdentifyPermissionWithoutState()
        {
            Permission permission = CreatePermission(CreateDatabase(), "G");

            Assert.AreEqual("EXECUTE ON OBJECT::[dbo].[MyProc] TO [MyUser]", permission.FullName);
        }

        [TestMethod]
        public void ToSql_ShouldScriptTheState()
        {
            Database database = CreateDatabase();

            Assert.AreEqual("GRANT EXECUTE ON OBJECT::[dbo].[MyProc] TO [MyUser]\r\nGO\r\n", CreatePermission(database, "G").ToSql());
            Assert.AreEqual("GRANT EXECUTE ON OBJECT::[dbo].[MyProc] TO [MyUser] WITH GRANT OPTION\r\nGO\r\n", CreatePermission(database, "W").ToSql());
            Assert.AreEqual("DENY EXECUTE ON OBJECT::[dbo].[MyProc] TO [MyUser]\r\nGO\r\n", CreatePermission(database, "D").ToSql());
        }

        [TestMethod]
        public void ToSql_OtherSecurables_ShouldUseTheirClass()
        {
            Database database = CreateDatabase();
            Permission column = CreatePermission(database, "G");
            column.PermissionName = "SELECT";
            column.SecurableName = "[dbo].[MyTable]";
            column.ColumnName = "Name";
            Permission schema = new Permission(database) { Class = Permission.SecurableClass.Schema, PermissionName = "SELECT", State = "G", SecurableName = "[Sales]", Grantee = "MyRole" };
            Permission onDatabase = new Permission(database) { Class = Permission.SecurableClass.Database, PermissionName = "CONNECT", State = "G", Grantee = "MyUser" };
            Permission onUser = new Permission(database) { Class = Permission.SecurableClass.DatabasePrincipal, PermissionName = "IMPERSONATE", State = "G", SecurablePrincipalType = "USER", SecurableName = "[Other]", Grantee = "MyUser" };

            StringAssert.StartsWith(column.ToSql(), "GRANT SELECT ON OBJECT::[dbo].[MyTable] ([Name]) TO [MyUser]");
            StringAssert.StartsWith(schema.ToSql(), "GRANT SELECT ON SCHEMA::[Sales] TO [MyRole]");
            StringAssert.StartsWith(onDatabase.ToSql(), "GRANT CONNECT TO [MyUser]");
            StringAssert.StartsWith(onUser.ToSql(), "GRANT IMPERSONATE ON USER::[Other] TO [MyUser]");
        }

        [TestMethod]
        public void ToSqlDiff_Create_ShouldGrant()
        {
            Permission permission = CreatePermission(CreateDatabase(), "G");
            permission.Status = ObjectStatus.Create;

            Assert.AreEqual("GRANT EXECUTE ON OBJECT::[dbo].[MyProc] TO [MyUser]\r\nGO\r\n", ToSqlDiff(permission));
        }

        [TestMethod]
        public void ToSqlDiff_Drop_ShouldRevoke()
        {
            Permission permission = CreatePermission(CreateDatabase(), "W");
            permission.Status = ObjectStatus.Drop;

            Assert.AreEqual("REVOKE EXECUTE ON OBJECT::[dbo].[MyProc] FROM [MyUser] CASCADE\r\nGO\r\n", ToSqlDiff(permission));
        }

        [TestMethod]
        public void ToSqlDiff_DropWithDroppedGrantee_ShouldScriptNothing()
        {
            Database database = CreateDatabase();
            User user = new User(database) { Name = "MyUser", Login = "", Owner = "dbo" };
            database.Users.Add(user);
            user.Status = ObjectStatus.Drop;
            Permission permission = CreatePermission(database, "G");
            permission.Status = ObjectStatus.Drop;

            Assert.AreEqual("", ToSqlDiff(permission));
        }

        [TestMethod]
        public void ToSqlDiff_GrantOptionToGrant_ShouldRevokeGrantOption()
        {
            Permission permission = CreatePermission(CreateDatabase(), "G");
            permission.PreviousState = "W";
            permission.Status = ObjectStatus.Alter;

            Assert.AreEqual("REVOKE GRANT OPTION FOR EXECUTE ON OBJECT::[dbo].[MyProc] FROM [MyUser] CASCADE\r\nGO\r\n", ToSqlDiff(permission));
        }

        [TestMethod]
        public void ToSqlDiff_DenyToGrant_ShouldGrant()
        {
            Permission permission = CreatePermission(CreateDatabase(), "G");
            permission.PreviousState = "D";
            permission.Status = ObjectStatus.Alter;

            Assert.AreEqual("GRANT EXECUTE ON OBJECT::[dbo].[MyProc] TO [MyUser]\r\nGO\r\n", ToSqlDiff(permission));
        }

        [TestMethod]
        public void ToSqlDiff_UnchangedWithRecreatedGrantee_ShouldGrantAgain()
        {
            Database database = CreateDatabase();
            User user = new User(database) { Name = "MyUser", Login = "", Owner = "dbo" };
            database.Users.Add(user);
            Permission permission = CreatePermission(database, "G");
            user.Status = ObjectStatus.Alter;

            Assert.AreEqual("GRANT EXECUTE ON OBJECT::[dbo].[MyProc] TO [MyUser]\r\nGO\r\n", ToSqlDiff(permission));
        }

        [TestMethod]
        public void IsPrincipalExcluded_UserWildcardFilter_ShouldExcludeMatchingUsersOnly()
        {
            Database database = CreateDatabase();
            database.Options.Filters.Items.Add(new SqlOptionFilterItem(ObjectType.User, "Temp*"));

            Assert.IsTrue(GeneratePermissions.IsPrincipalExcluded(database, "TempUser1", "U"));
            Assert.IsFalse(GeneratePermissions.IsPrincipalExcluded(database, "AppUser", "S"));
            Assert.IsFalse(GeneratePermissions.IsPrincipalExcluded(database, "TempRole1", "R"));
        }

        [TestMethod]
        public void IsPrincipalExcluded_RoleFilter_ShouldExcludeRoles()
        {
            Database database = CreateDatabase();
            database.Options.Filters.Items.Add(new SqlOptionFilterItem(ObjectType.Role, "Reporting*"));

            Assert.IsTrue(GeneratePermissions.IsPrincipalExcluded(database, "ReportingReaders", "R"));
            Assert.IsFalse(GeneratePermissions.IsPrincipalExcluded(database, "ReportingUser", "U"));
        }

        [TestMethod]
        public void ToSqlDiff_Unchanged_ShouldScriptNothing()
        {
            Permission permission = CreatePermission(CreateDatabase(), "G");

            Assert.AreEqual("", ToSqlDiff(permission));
        }
    }
}
