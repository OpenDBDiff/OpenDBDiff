using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBDiff.Schema.SQLServer.Generates.Options;
using DBDiff.Schema.SQLServer.Generates.Model;
using System.Data.SqlClient;

namespace DBDiff.Schema.SQLServer.Generates.Generates
{
    public class GenerateUsers
    {
        private Generate root;

        public GenerateUsers(Generate root)
        {
            this.root = root;
        }

        private static string GetSQL()
        {
            string sql = "select is_fixed_role, type, ISNULL(suser_sname(sid),'') AS Login,Name,principal_id, ISNULL(default_schema_name,'') AS default_schema_name from sys.database_principals ";
            sql += "WHERE type IN ('S','U','A','R') ";
            sql += "ORDER BY Name";
            return sql;
        }

        public void Fill(Database database, string connectioString)
        {
            string type;
            if ((database.Options.Ignore.FilterUsers) || (database.Options.Ignore.FilterRoles))
            {
                using (SqlConnection conn = new SqlConnection(connectioString))
                {
                    using (SqlCommand command = new SqlCommand(GetSQL(), conn))
                    {
                        conn.Open();
                        command.CommandTimeout = 0;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                type = reader["type"].ToString();
                                if (database.Options.Ignore.FilterUsers && (type.Equals("S") || type.Equals("U")))
                                {
                                    User item = new User(database);
                                    item.Id = (int)reader["principal_id"];
                                    item.Name = reader["name"].ToString();
                                    item.Login = reader["Login"].ToString();
                                    item.Owner = reader["default_schema_name"].ToString();
                                    database.Users.Add(item);
                                }
                                if (database.Options.Ignore.FilterRoles && (type.Equals("A") || type.Equals("R")))
                                {
                                    Role item = new Role(database);
                                    item.Id = (int)reader["principal_id"];
                                    item.Name = reader["name"].ToString();
                                    item.Owner = reader["default_schema_name"].ToString();
                                    item.Password = "";
                                    item.IsSystem = (Boolean)reader["is_fixed_role"];
                                    if (type.Equals("A"))
                                        item.Type = Role.RoleTypeEnum.ApplicationRole;
                                    else
                                        item.Type = Role.RoleTypeEnum.DatabaseRole;
                                    database.Roles.Add(item);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
