namespace TechBlog.NewsManager.Tests.IntegrationTests.Fixtures
{
    public static class IntegrationTestsHelper
    {
        public const string ApiKeyName = "X-API-KEY";
        public const string ApiKeyValue = "e82c255a-7a1c-47b4-b9f2-8e7a2bf5336c";

        public const string JournalistEmail = "journalist@email.com";
        public const string JournalistName = "Journalist Example";

        public const string ReaderEmail = "reader@email.com";
        public const string ReaderName = "Reader Example";

        public const string FakePassword = "P@ssW0rD12";

        public const string JwtIssuer = "TechBlog_DEV";
        public const string JwtAudience = "TechBlog_DEV";
        public const string JwtKey = "b8ce5d82-50cb-4e4f-90a6-07e8e7841987";

        public const string BlogNewTitle = "Fake blog new";
        public const string BlogNewBody = "Fake body";
        public const string BlogNewDescription = "Fake description";
        public const string BlogNewTags = "fake;new";
    }

    public static class DatabaseScript
    {
        public const string CreateBlogNew = @"
            INSERT INTO [BlogNew]
            ([Id],[Title],[Description],[Body],[Tags],[Enabled],[BlogUserId],[CreatedAt],[LastUpdateAt])
            VALUES
            (@Id,@Title,@Description,@Body,@Tags,@Enabled,@BlogUserId,@CreatedAt,@LastUpdateAt)
        ";

        public const string CreateDatabase = @"
            CREATE TABLE IF NOT EXISTS ""__EFMigrationsHistory"" (
                ""MigrationId"" TEXT NOT NULL CONSTRAINT ""PK___EFMigrationsHistory"" PRIMARY KEY,
                ""ProductVersion"" TEXT NOT NULL
            );
            
            BEGIN TRANSACTION;
            
            CREATE TABLE IF NOT EXISTS ""AspNetRoles"" (
                ""Id"" varchar(300) NOT NULL CONSTRAINT ""PK_AspNetRoles"" PRIMARY KEY,
                ""Name"" varchar(300) NULL,
                ""NormalizedName"" varchar(300) NULL,
                ""ConcurrencyStamp"" varchar(300) NULL
            );
            
            CREATE TABLE IF NOT EXISTS ""AspNetUsers"" (
                ""Id"" varchar(300) NOT NULL CONSTRAINT ""PK_AspNetUsers"" PRIMARY KEY,
                ""Name"" varchar(300) NOT NULL,
                ""BlogUserType"" TEXT NOT NULL,
                ""Enabled"" INTEGER NOT NULL DEFAULT 1,
                ""CreatedAt"" TEXT NOT NULL DEFAULT '2023-11-02 00:24:23.0959662',
                ""LastUpdateAt"" TEXT NOT NULL DEFAULT '2023-11-02 00:24:23.0959879',
                ""UserName"" varchar(300) NOT NULL,
                ""NormalizedUserName"" varchar(300) NULL,
                ""Email"" varchar(300) NOT NULL,
                ""NormalizedEmail"" varchar(300) NULL,
                ""EmailConfirmed"" INTEGER NOT NULL DEFAULT 1,
                ""PasswordHash"" varchar(300) NULL,
                ""SecurityStamp"" varchar(300) NULL,
                ""ConcurrencyStamp"" varchar(300) NULL,
                ""PhoneNumber"" varchar(300) NULL,
                ""PhoneNumberConfirmed"" INTEGER NOT NULL,
                ""TwoFactorEnabled"" INTEGER NOT NULL,
                ""LockoutEnd"" TEXT NULL,
                ""LockoutEnabled"" INTEGER NOT NULL,
                ""AccessFailedCount"" INTEGER NOT NULL
            );
            
            CREATE TABLE IF NOT EXISTS ""AspNetRoleClaims"" (
                ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_AspNetRoleClaims"" PRIMARY KEY AUTOINCREMENT,
                ""RoleId"" varchar(300) NOT NULL,
                ""ClaimType"" varchar(300) NULL,
                ""ClaimValue"" varchar(300) NULL,
                CONSTRAINT ""FK_AspNetRoleClaims_AspNetRoles_RoleId"" FOREIGN KEY (""RoleId"") REFERENCES ""AspNetRoles"" (""Id"") ON DELETE CASCADE
            );
            
            CREATE TABLE IF NOT EXISTS ""AspNetUserClaims"" (
                ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_AspNetUserClaims"" PRIMARY KEY AUTOINCREMENT,
                ""UserId"" varchar(300) NOT NULL,
                ""ClaimType"" varchar(300) NULL,
                ""ClaimValue"" varchar(300) NULL,
                CONSTRAINT ""FK_AspNetUserClaims_AspNetUsers_UserId"" FOREIGN KEY (""UserId"") REFERENCES ""AspNetUsers"" (""Id"") ON DELETE CASCADE
            );
            
            CREATE TABLE IF NOT EXISTS ""AspNetUserLogins"" (
                ""LoginProvider"" varchar(300) NOT NULL,
                ""ProviderKey"" varchar(300) NOT NULL,
                ""ProviderDisplayName"" varchar(300) NULL,
                ""UserId"" varchar(300) NOT NULL,
                CONSTRAINT ""PK_AspNetUserLogins"" PRIMARY KEY (""LoginProvider"", ""ProviderKey""),
                CONSTRAINT ""FK_AspNetUserLogins_AspNetUsers_UserId"" FOREIGN KEY (""UserId"") REFERENCES ""AspNetUsers"" (""Id"") ON DELETE CASCADE
            );
            
            CREATE TABLE IF NOT EXISTS ""AspNetUserRoles"" (
                ""UserId"" varchar(300) NOT NULL,
                ""RoleId"" varchar(300) NOT NULL,
                CONSTRAINT ""PK_AspNetUserRoles"" PRIMARY KEY (""UserId"", ""RoleId""),
                CONSTRAINT ""FK_AspNetUserRoles_AspNetRoles_RoleId"" FOREIGN KEY (""RoleId"") REFERENCES ""AspNetRoles"" (""Id"") ON DELETE CASCADE,
                CONSTRAINT ""FK_AspNetUserRoles_AspNetUsers_UserId"" FOREIGN KEY (""UserId"") REFERENCES ""AspNetUsers"" (""Id"") ON DELETE CASCADE
            );
            
            CREATE TABLE IF NOT EXISTS ""AspNetUserTokens"" (
                ""UserId"" varchar(300) NOT NULL,
                ""LoginProvider"" varchar(300) NOT NULL,
                ""Name"" varchar(300) NOT NULL,
                ""Value"" varchar(300) NULL,
                CONSTRAINT ""PK_AspNetUserTokens"" PRIMARY KEY (""UserId"", ""LoginProvider"", ""Name""),
                CONSTRAINT ""FK_AspNetUserTokens_AspNetUsers_UserId"" FOREIGN KEY (""UserId"") REFERENCES ""AspNetUsers"" (""Id"") ON DELETE CASCADE
            );
            
            CREATE TABLE IF NOT EXISTS ""BlogNew"" (
                ""Id"" TEXT NOT NULL CONSTRAINT ""PK_BlogNew"" PRIMARY KEY,
                ""Title"" varchar(300) NOT NULL,
                ""Description"" varchar(300) NOT NULL,
                ""Body"" varchar(300) NOT NULL,
                ""Tags"" varchar(300) NULL,
                ""Enabled"" INTEGER NOT NULL DEFAULT 1,
                ""BlogUserId"" varchar(300) NOT NULL,
                ""CreatedAt"" TEXT NOT NULL DEFAULT '2023-11-02 00:24:23.0962701',
                ""LastUpdateAt"" TEXT NOT NULL DEFAULT '2023-11-02 00:24:23.0962965',
                CONSTRAINT ""FK_BlogNew_AspNetUsers_BlogUserId"" FOREIGN KEY (""BlogUserId"") REFERENCES ""AspNetUsers"" (""Id"")
            );
            
            CREATE INDEX IF NOT EXISTS ""IX_AspNetRoleClaims_RoleId"" ON ""AspNetRoleClaims"" (""RoleId"");
            
            CREATE UNIQUE INDEX IF NOT EXISTS ""RoleNameIndex"" ON ""AspNetRoles"" (""NormalizedName"");
            
            CREATE INDEX IF NOT EXISTS ""IX_AspNetUserClaims_UserId"" ON ""AspNetUserClaims"" (""UserId"");
            
            CREATE INDEX IF NOT EXISTS ""IX_AspNetUserLogins_UserId"" ON ""AspNetUserLogins"" (""UserId"");
            
            CREATE INDEX IF NOT EXISTS ""IX_AspNetUserRoles_RoleId"" ON ""AspNetUserRoles"" (""RoleId"");
            
            CREATE INDEX IF NOT EXISTS ""EmailIndex"" ON ""AspNetUsers"" (""NormalizedEmail"");
            
            CREATE UNIQUE INDEX IF NOT EXISTS ""UserNameIndex"" ON ""AspNetUsers"" (""NormalizedUserName"");
            
            CREATE INDEX IF NOT EXISTS ""IX_BlogNew_BlogUserId"" ON ""BlogNew"" (""BlogUserId"");
                        
            COMMIT;
            ";
    }
}
