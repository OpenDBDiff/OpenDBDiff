using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using System;
using System.Collections.Generic;

namespace OpenDBDiff.SqlServer.Schema.Model
{
    /// <summary>
    /// A GRANT, DENY or REVOKE of one permission on one securable to one database principal (a row of sys.database_permissions).
    /// </summary>
    public class Permission : SQLServerSchemaBase
    {
        /// <summary>
        /// The class of the securable, as in sys.database_permissions.class.
        /// </summary>
        public enum SecurableClass
        {
            Database = 0,
            Object = 1,
            Schema = 3,
            DatabasePrincipal = 4,
            Assembly = 5,
            Type = 6,
            XmlSchemaCollection = 10
        }

        /// <summary>
        /// Permission scripts run after every other script, so the securables and principals they refer to exist.
        /// </summary>
        private const int ScriptDeep = int.MaxValue;

        public Permission(ISchemaBase parent)
            : base(parent, ObjectType.Permission)
        {
        }

        public override ISchemaBase Clone(ISchemaBase parent)
        {
            Permission item = new Permission(parent);
            item.Id = this.Id;
            item.Name = this.Name;
            item.Owner = this.Owner;
            item.Guid = this.Guid;
            item.Class = this.Class;
            item.PermissionName = this.PermissionName;
            item.State = this.State;
            item.PreviousState = this.PreviousState;
            item.SecurableName = this.SecurableName;
            item.SecurablePrincipalType = this.SecurablePrincipalType;
            item.ColumnName = this.ColumnName;
            item.Grantee = this.Grantee;
            return item;
        }

        /// <summary>
        /// Identifies the permission independent of its state, e.g. "EXECUTE ON OBJECT::[dbo].[MyProc] TO [MyUser]".
        /// </summary>
        public override string FullName
        {
            get { return Name; }
        }

        public SecurableClass Class { get; set; }

        /// <summary>
        /// The permission, e.g. EXECUTE or SELECT.
        /// </summary>
        public string PermissionName { get; set; }

        /// <summary>
        /// sys.database_permissions.state: G = GRANT, W = GRANT WITH GRANT OPTION, D = DENY, R = REVOKE.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// The state in the database being changed, when the state differs.
        /// </summary>
        public string PreviousState { get; set; }

        /// <summary>
        /// The quoted securable, e.g. [dbo].[MyProc]. Empty for database permissions.
        /// </summary>
        public string SecurableName { get; set; }

        /// <summary>
        /// For permissions on a database principal: USER, ROLE or APPLICATION ROLE.
        /// </summary>
        public string SecurablePrincipalType { get; set; }

        /// <summary>
        /// The column for column-level permissions, otherwise empty.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// The principal that gets the permission.
        /// </summary>
        public string Grantee { get; set; }

        private string GranteeFullName
        {
            get { return "[" + Grantee + "]"; }
        }

        /// <summary>
        /// "EXECUTE ON OBJECT::[dbo].[MyProc]", or just "CONNECT" for a database permission.
        /// </summary>
        private string PermissionOn
        {
            get
            {
                string sql = PermissionName;
                switch (Class)
                {
                    case SecurableClass.Object:
                        sql += " ON OBJECT::" + SecurableName;
                        if (!String.IsNullOrEmpty(ColumnName))
                            sql += " ([" + ColumnName + "])";
                        break;
                    case SecurableClass.Schema:
                        sql += " ON SCHEMA::" + SecurableName;
                        break;
                    case SecurableClass.DatabasePrincipal:
                        sql += " ON " + SecurablePrincipalType + "::" + SecurableName;
                        break;
                    case SecurableClass.Assembly:
                        sql += " ON ASSEMBLY::" + SecurableName;
                        break;
                    case SecurableClass.Type:
                        sql += " ON TYPE::" + SecurableName;
                        break;
                    case SecurableClass.XmlSchemaCollection:
                        sql += " ON XML SCHEMA COLLECTION::" + SecurableName;
                        break;
                }
                return sql;
            }
        }

        /// <summary>
        /// Builds the name that identifies the permission; call it after setting the other properties.
        /// </summary>
        public void SetName()
        {
            Name = PermissionOn + " TO " + GranteeFullName;
        }

        public override string ToSql()
        {
            switch (State)
            {
                case "W":
                    return "GRANT " + PermissionOn + " TO " + GranteeFullName + " WITH GRANT OPTION\r\nGO\r\n";
                case "D":
                    return "DENY " + PermissionOn + " TO " + GranteeFullName + "\r\nGO\r\n";
                case "R":
                    return "REVOKE " + PermissionOn + " FROM " + GranteeFullName + "\r\nGO\r\n";
                default:
                    return "GRANT " + PermissionOn + " TO " + GranteeFullName + "\r\nGO\r\n";
            }
        }

        public override string ToSqlDrop()
        {
            // CASCADE also revokes what the grantee granted on to others; SQL Server requires it for a grant with grant option.
            return "REVOKE " + PermissionOn + " FROM " + GranteeFullName + (State == "W" ? " CASCADE" : "") + "\r\nGO\r\n";
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        private string ToSqlAlter()
        {
            if (PreviousState == "W" && State == "G")
                return "REVOKE GRANT OPTION FOR " + PermissionOn + " FROM " + GranteeFullName + " CASCADE\r\nGO\r\n";
            if (PreviousState == "W" && State == "D")
                return "DENY " + PermissionOn + " TO " + GranteeFullName + " CASCADE\r\nGO\r\n";
            return ToSql();
        }

        public override SQLScriptList ToSqlDiff(ICollection<ISchemaBase> schemas)
        {
            SQLScriptList listDiff = new SQLScriptList();
            if (this.Status != ObjectStatus.Original)
                RootParent.ActionMessage.Add(this);

            if (this.Status == ObjectStatus.Drop)
            {
                // Dropping the securable or the grantee already removes the permission, and a REVOKE on it would fail.
                if (!IsDropped(FindSecurable()) && !IsDropped(FindGrantee()) && !IsColumnDropped())
                    listDiff.Add(new SQLScript(ScriptDeep, ToSqlDrop(), 0, ScriptAction.RevokePermission));
            }
            if (this.Status == ObjectStatus.Create)
                listDiff.Add(new SQLScript(ScriptDeep, ToSql(), 0, ScriptAction.GrantPermission));
            if (this.HasState(ObjectStatus.Alter))
                listDiff.Add(new SQLScript(ScriptDeep, ToSqlAlter(), 0, ScriptAction.GrantPermission));
            if (this.Status == ObjectStatus.Original && (IsRecreated(FindSecurable()) || IsRecreated(FindGrantee())))
                // The script drops and creates the securable or grantee, which loses the permission: grant it again.
                listDiff.Add(new SQLScript(ScriptDeep, ToSql(), 0, ScriptAction.GrantPermission));
            return listDiff;
        }

        private static bool IsDropped(ISchemaBase item)
        {
            return item != null && item.Status == ObjectStatus.Drop;
        }

        private static bool IsRecreated(ISchemaBase item)
        {
            if (item == null || item.Status == ObjectStatus.Create || item.Status == ObjectStatus.Drop)
                return false;
            if (item.HasState(ObjectStatus.Rebuild))
                return true;
            // These are scripted as DROP and CREATE when they change.
            return (item.ObjectType == ObjectType.User || item.ObjectType == ObjectType.Role || item.ObjectType == ObjectType.Synonym)
                && item.HasState(ObjectStatus.Alter);
        }

        private bool IsColumnDropped()
        {
            if (Class != SecurableClass.Object || String.IsNullOrEmpty(ColumnName))
                return false;
            Table table = FindSecurable() as Table;
            if (table == null)
                return false;
            Column column = table.Columns.Find(item => item.Name.Equals(ColumnName, StringComparison.CurrentCultureIgnoreCase));
            return IsDropped(column);
        }

        private Database Database
        {
            get { return (Database)RootParent; }
        }

        private ISchemaBase FindPrincipal(string fullName)
        {
            if (Database.Users.Contains(fullName))
                return Database.Users[fullName];
            if (Database.Roles.Contains(fullName))
                return Database.Roles[fullName];
            return null;
        }

        private ISchemaBase FindGrantee()
        {
            return FindPrincipal(GranteeFullName);
        }

        private ISchemaBase FindSecurable()
        {
            switch (Class)
            {
                case SecurableClass.Object:
                    return Database.Find(SecurableName);
                case SecurableClass.Schema:
                    return Database.Schemas[SecurableName];
                case SecurableClass.DatabasePrincipal:
                    return FindPrincipal(SecurableName);
                case SecurableClass.Assembly:
                    return Database.Assemblies[SecurableName];
                case SecurableClass.Type:
                    if (Database.UserTypes.Contains(SecurableName))
                        return Database.UserTypes[SecurableName];
                    return Database.TablesTypes[SecurableName];
                case SecurableClass.XmlSchemaCollection:
                    return Database.XmlSchemas[SecurableName];
                default:
                    return null;
            }
        }

        public bool Compare(Permission obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            return this.State.Equals(obj.State);
        }
    }
}
