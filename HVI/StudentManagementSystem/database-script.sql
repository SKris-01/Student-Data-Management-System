IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Role] nvarchar(50) NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);

CREATE TABLE [tblDepartment] (
    [DepartmentId] int NOT NULL IDENTITY,
    [DepartmentName] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_tblDepartment] PRIMARY KEY ([DepartmentId])
);

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [tblStudent] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Username] nvarchar(50) NOT NULL,
    [Role] nvarchar(50) NOT NULL DEFAULT N'Student',
    [Course] nvarchar(100) NOT NULL,
    [Semester] int NOT NULL,
    [CGPA] decimal(3,2) NOT NULL,
    [DOB] datetime2 NOT NULL,
    [Hometown] nvarchar(100) NOT NULL,
    [Password] nvarchar(255) NOT NULL,
    [DepartmentId] int NOT NULL,
    CONSTRAINT [PK_tblStudent] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_tblStudent_tblDepartment_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [tblDepartment] ([DepartmentId]) ON DELETE NO ACTION
);

CREATE TABLE [tblAdmin] (
    [AdminId] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Username] nvarchar(50) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [Role] nvarchar(10) NOT NULL DEFAULT N'Admin',
    [PhoneNumber] nvarchar(15) NULL,
    [OriginalStudentId] int NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] datetime2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_tblAdmin] PRIMARY KEY ([AdminId]),
    CONSTRAINT [FK_tblAdmin_tblStudent_OriginalStudentId] FOREIGN KEY ([OriginalStudentId]) REFERENCES [tblStudent] ([Id]) ON DELETE SET NULL
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'DepartmentId', N'DepartmentName') AND [object_id] = OBJECT_ID(N'[tblDepartment]'))
    SET IDENTITY_INSERT [tblDepartment] ON;
INSERT INTO [tblDepartment] ([DepartmentId], [DepartmentName])
VALUES (1, N'Computer Science'),
(2, N'Information Technology'),
(3, N'Electronics Engineering'),
(4, N'Mechanical Engineering'),
(5, N'Civil Engineering'),
(6, N'Administration');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'DepartmentId', N'DepartmentName') AND [object_id] = OBJECT_ID(N'[tblDepartment]'))
    SET IDENTITY_INSERT [tblDepartment] OFF;

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

CREATE INDEX [IX_tblStudent_DepartmentId] ON [tblStudent] ([DepartmentId]);

CREATE UNIQUE INDEX [IX_tblAdmin_Username] ON [tblAdmin] ([Username]);
CREATE INDEX [IX_tblAdmin_Role] ON [tblAdmin] ([Role]);
CREATE INDEX [IX_tblAdmin_OriginalStudentId] ON [tblAdmin] ([OriginalStudentId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250806190403_InitialCreate', N'9.0.8');

COMMIT;
GO

