using Microsoft.Data.SqlClient;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model;
using System;

namespace OpenDBDiff.SqlServer.Schema.Generates
{
    public class GeneratePermissions
    {
        private Generate root;

        public GeneratePermissions(Generate root)
        {
            this.root = root;
        }

        private static string GetSQL()
        {
            return SQLQueries.SQLQueryFactory.Get("GetPermissions");
        }

        private static string GetPrincipalType(string type)
        {
            if (type.Equals("R")) return "ROLE";
            if (type.Equals("A")) return "APPLICATION ROLE";
            return "USER";
        }

        /// <summary>
        /// True when a name filter excludes the database principal, so permissions granted to or on it are not compared.
        /// </summary>
        public static bool IsPrincipalExcluded(Database database, string name, string type)
        {
            ISchemaBase principal;
            if (type.Equals("R") || type.Equals("A"))
                principal = new Role(database) { Name = name };
            else
                principal = new User(database) { Name = name };
            return !database.Options.Filters.IsItemIncluded(principal);
        }

        /// <summary>
        /// Reads the permissions; must run after the other objects are read, so permissions on filtered objects are skipped.
        /// </summary>
        public void Fill(Database database, string connectionString)
        {
            if (database.Options.Ignore.FilterPermissions)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(GetSQL(), conn))
                    {
                        conn.Open();
                        command.CommandTimeout = 0;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Permission item = new Permission(database);
                                item.Class = (Permission.SecurableClass)Convert.ToInt32(reader["class"]);
                                item.PermissionName = reader["permission_name"].ToString();
                                item.State = reader["state"].ToString();
                                item.Grantee = reader["grantee_name"].ToString();
                                item.ColumnName = reader["column_name"].ToString();
                                string schema = reader["securable_schema"].ToString();
                                string name = reader["securable_name"].ToString();
                                switch (item.Class)
                                {
                                    case Permission.SecurableClass.Object:
                                    case Permission.SecurableClass.Type:
                                    case Permission.SecurableClass.XmlSchemaCollection:
                                        item.Owner = schema;
                                        item.SecurableName = "[" + schema + "].[" + name + "]";
                                        break;
                                    case Permission.SecurableClass.Schema:
                                        item.Owner = schema;
                                        item.SecurableName = "[" + name + "]";
                                        break;
                                    case Permission.SecurableClass.DatabasePrincipal:
                                        item.SecurablePrincipalType = GetPrincipalType(reader["securable_principal_type"].ToString());
                                        item.SecurableName = "[" + name + "]";
                                        break;
                                    case Permission.SecurableClass.Assembly:
                                        item.SecurableName = "[" + name + "]";
                                        break;
                                }
                                // Skip permissions on objects that are not compared, such as excluded or filtered objects.
                                if (item.Class == Permission.SecurableClass.Object && database.Find(item.SecurableName) == null)
                                    continue;
                                if (IsPrincipalExcluded(database, item.Grantee, reader["grantee_type"].ToString()))
                                    continue;
                                if (item.Class == Permission.SecurableClass.DatabasePrincipal && IsPrincipalExcluded(database, name, reader["securable_principal_type"].ToString()))
                                    continue;
                                item.SetName();
                                // The same id for the same permission in both databases, unique among the permissions.
                                item.Id = -1 - (item.FullName.ToUpperInvariant().GetHashCode() & 0x7FFFFFFF);
                                database.Permissions.Add(item);
                            }
                        }
                    }
                }
            }
        }
    }
}
