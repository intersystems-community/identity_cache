using InterSystems.Data.CacheClient;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace InterSystems.AspNet.Identity.Cache
{
    /// <summary>
    /// Default initializer that uses Cache database and enables ASP.NET Identity to work with it.
    /// </summary>
    class IdentityDbInitializer : CreateDatabaseIfNotExists<IdentityDbContext>
    {
        #region Constants

        private const string AspNetUsers = "DBO.AspNetUsers";
        private const string AspNetRoles = "DBO.AspNetRoles";
        private const string AspNetUserRoles = "DBO.AspNetUserRoles";
        private const string AspNetUserClaims = "DBO.AspNetUserClaims";
        private const string AspNetUserLogins = "DBO.AspNetUserLogins";

        private const string UserNameIndexQuery = "CREATE UNIQUE INDEX UserNameIndex ON DBO.AspNetUsers(UserName)";
        private const string RoleNameIndexQuery = "CREATE UNIQUE INDEX RoleNameIndex ON DBO.AspNetRoles(Name)";
        private readonly string CheckUserNameIndexQuery = string.Format("SELECT Count(*) FROM %Dictionary.CompiledIndex WHERE Name='UserNameIndex' AND Origin='{0}'", AspNetUsers);
        private readonly string CheckRoleNameIndexQuery = string.Format("SELECT Count(*) FROM %Dictionary.CompiledIndex WHERE Name='RoleNameIndex' AND Origin='{0}'", AspNetRoles);
        
        private const string ExistingTablesQuery = @"SELECT id 
                                                     FROM %Dictionary.CompiledClass 
                                                     WHERE SqlTableName='AspNetUsers' 
                                                     OR SqlTableName='AspNetRoles' 
                                                     OR SqlTableName='AspNetUserRoles' 
                                                     OR SqlTableName='AspNetUserClaims' 
                                                     OR SqlTableName='AspNetUserLogins'";

        private const string AspNetUsersQuery = @"CREATE TABLE DBO.AspNetUsers (Id nvarchar(128) NOT NULL PRIMARY KEY, 
                                                     Email nvarchar(256),
                                                     EmailConfirmed bit NOT NULL, 
                                                     PasswordHash nvarchar(MAX), 
                                                     SecurityStamp nvarchar(MAX), 
                                                     PhoneNumber nvarchar(MAX), 
                                                     PhoneNumberConfirmed bit NOT NULL, 
                                                     TwoFactorEnabled bit NOT NULL, 
                                                     LockoutEndDateUtc datetime, 
                                                     LockoutEnabled bit NOT NULL, 
                                                     AccessFailedCount int NOT NULL, 
                                                     UserName nvarchar(256) NOT NULL UNIQUE)";

        private const string AspNetRolesQuery = @"CREATE TABLE DBO.AspNetRoles (Id nvarchar(128) NOT NULL PRIMARY KEY, 
                                                     Name nvarchar(256))";

        private const string AspNetUserRolesQuery = @"CREATE TABLE DBO.AspNetUserRoles (UserId nvarchar(128) NOT NULL, 
                                                     RoleId nvarchar(128), 
                                                     CONSTRAINT PK_AspNetUserRoles PRIMARY KEY CLUSTERED (UserId), 
                                                     FOREIGN KEY (UserId) REFERENCES DBO.AspNetUsers(Id), 
                                                     FOREIGN KEY (RoleId) REFERENCES DBO.AspNetRoles(Id))";

        private const string AspNetUserClaimsQuery = @"CREATE TABLE DBO.AspNetUserClaims (Id int NOT NULL, 
                                                     UserId nvarchar(128) NOT NULL, 
                                                     ClaimType nvarchar(MAX), 
                                                     ClaimValue nvarchar(MAX), 
                                                     CONSTRAINT PK_AspNetUserClaims PRIMARY KEY CLUSTERED (Id), 
                                                     FOREIGN KEY (UserId) REFERENCES DBO.AspNetUsers(Id))";

        private const string AspNetUserLoginsQuery = @"CREATE TABLE DBO.AspNetUserLogins (LoginProvider NVARCHAR(128) NOT NULL, 
                                                     ProviderKey NVARCHAR(128) NOT NULL, 
                                                     UserId NVARCHAR(128) NOT NULL, 
                                                     CONSTRAINT PK_AspNetUserLogins PRIMARY KEY CLUSTERED (LoginProvider, ProviderKey, UserId), 
                                                     FOREIGN KEY (UserId) REFERENCES DBO.AspNetUsers(Id))";

        #endregion

        #region Helpers

        private List<string> GetExistingTables(CacheConnection connection)
        {
            var tables = new List<string>();
            using (var command = new CacheCommand(ExistingTablesQuery, connection))
            using (var tablesReader = command.ExecuteReader())
            {
                while (tablesReader.Read())
                    tables.Add(tablesReader[0].ToString());
            }

            return tables;
        }

        private string GetTableQuery(string name)
        {
            switch (name)
            {
                case AspNetUsers:
                    return AspNetUsersQuery;
                case AspNetRoles:
                    return AspNetRolesQuery;
                case AspNetUserRoles:
                    return AspNetUserRolesQuery;
                case AspNetUserClaims:
                    return AspNetUserClaimsQuery;
                case AspNetUserLogins:
                    return AspNetUserLoginsQuery;
                default:
                    throw new ArgumentException("Unexpected table name!");
            }
        }

        private void CreateTableIfNotExists(List<string> tables, string tableName, CacheConnection connection)
        {
            if (tables.Count(x => x == tableName) == 0)
                using (var command = new CacheCommand(GetTableQuery(tableName), connection))
                    command.ExecuteScalar();
        }

        private void CreateIndexesIfNotExist(CacheConnection connection)
        {
            using (CacheCommand checkUserIndex = new CacheCommand(CheckUserNameIndexQuery, connection), 
                                checkRoleIndex = new CacheCommand(CheckRoleNameIndexQuery, connection))
            {
                if ((int)checkUserIndex.ExecuteScalar() == 0)
                    using (var createUserIndex = new CacheCommand(UserNameIndexQuery, connection))
                        createUserIndex.ExecuteNonQuery();

                if ((int)checkRoleIndex.ExecuteScalar() == 0)
                    using (var createRoleIndex = new CacheCommand(RoleNameIndexQuery, connection))
                        createRoleIndex.ExecuteNonQuery();
            }
        }

        private CacheConnection BuildConnection(IdentityDbContext context)
        {
            var connection = new CacheConnection();
            connection.ConnectionString = context.Database.Connection.ConnectionString;
            connection.Open();

            return connection;
        }

        #endregion

        /// <summary>
        /// Provides needed initializations for the database context.
        /// </summary>
        /// <param name="context"></param>
        public override void InitializeDatabase(IdentityDbContext context)
        {
            using (var connection = BuildConnection(context))
            {
                var tables = GetExistingTables(connection);

                CreateTableIfNotExists(tables, AspNetUsers, connection);
                CreateTableIfNotExists(tables, AspNetRoles, connection);
                CreateTableIfNotExists(tables, AspNetUserRoles, connection);
                CreateTableIfNotExists(tables, AspNetUserClaims, connection);
                CreateTableIfNotExists(tables, AspNetUserLogins, connection);
                CreateIndexesIfNotExist(connection);
            }
        }
    }
}