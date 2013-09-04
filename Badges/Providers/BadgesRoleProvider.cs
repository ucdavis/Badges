using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Configuration;
using System.Web.Security;
using Dapper;

namespace Badges.Providers
{
    public class BadgesRoleProvider : RoleProvider
    {
        private static DbConnection GetConnection(string connectionString = null)
        {
            //If connection string is null, use the default sql ce connection
            if (connectionString == null)
            {
                connectionString = WebConfigurationManager.ConnectionStrings["MainDb"].ConnectionString;
            }

            var connection = new SqlConnection(connectionString);
            connection.Open();

            return connection;
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            using (var conn = GetConnection())
            {
                var result = conn.Query<int>(
                    @"select count(USER_ID) from Permissions inner join Users on Permissions.User_Id = Users.Id 
                        where Identifier = @username and Role_Id = @rolename",
                    new {username, rolename = roleName});

                return result.Single() > 0; //return trus if more than zero users with the given username are associated with that role
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            using (var conn = GetConnection())
            {
                var result =
                    conn.Query<string>(
                        "select Role_Id from Permissions inner join Users on Permissions.User_Id = Users.Id where Identifier = @username",
                        new { username });

                return result.ToArray();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new InvalidOperationException("Cannot create roles through the role provider");
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new InvalidOperationException("Cannot delete roles through the role provider");
        }

        public override bool RoleExists(string roleName)
        {
            using (var conn = GetConnection())
            {
                var result = conn.Query<int>("select count(Id) from Roles where Id = @rolename",
                                             new { rolename = roleName });

                return result.Single() > 0;
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            using (var conn = GetConnection())
            {
                foreach (string username in usernames)
                {
                    foreach (string rolename in roleNames)
                    {
                        conn.Execute(@"insert into Permissions (User_Id, Role_Id) 
                                        select Users.Id, @rolename 
                                        from Users
                                        where Identifier = @username",
                                     new {username, rolename});

                    }
                }
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            using (var conn = GetConnection())
            {
                foreach (string username in usernames)
                {
                    foreach (string rolename in roleNames)
                    {
                        conn.Execute(@"delete Permissions
                         from Permissions perm
                         inner join Users on Users.ID = perm.User_id
	                        where Identifier = @username and perm.Role_id = @rolename",
                                     new { username, rolename });

                    }
                }
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            using (var conn = GetConnection())
            {
                var result =
                    conn.Query<string>(
                        "select Identifier from Permissions inner join Users on Permissions.User_Id = Users.Id where Role_Id = @rolename",
                        new { rolename = roleName });

                return result.ToArray();
            }
        }

        public override string[] GetAllRoles()
        {
            using (var conn = GetConnection())
            {
                var result = conn.Query<string>("select Id from Roles");

                return result.ToArray();
            }
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            using (var conn = GetConnection())
            {
                var result =
                    conn.Query<string>(
                        "select User_Id from Permissions inner join Users on Permissions.User_Id = Users.Id where Role_Id = @rolename and Identifier like %@username%",
                        new { rolename = roleName, username = usernameToMatch });

                return result.ToArray();
            }
        }

        public override string ApplicationName
        {
            get { return "Badges"; }
            set { throw new InvalidOperationException("You are not allowed to set the application name"); }
        }
    }
}