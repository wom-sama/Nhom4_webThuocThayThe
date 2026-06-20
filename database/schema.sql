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
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620134440_InitialSqlServer'
)
BEGIN
    CREATE TABLE [ActiveIngredients] (
        [Id] int NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Warning] nvarchar(max) NULL,
        CONSTRAINT [PK_ActiveIngredients] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620134440_InitialSqlServer'
)
BEGIN
    CREATE TABLE [AuditLogs] (
        [Id] int NOT NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [Actor] nvarchar(max) NOT NULL,
        [Action] nvarchar(max) NOT NULL,
        [Entity] nvarchar(max) NOT NULL,
        [Detail] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620134440_InitialSqlServer'
)
BEGIN
    CREATE TABLE [Categories] (
        [Id] int NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620134440_InitialSqlServer'
)
BEGIN
    CREATE TABLE [DosageForms] (
        [Id] int NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_DosageForms] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620134440_InitialSqlServer'
)
BEGIN
    CREATE TABLE [Drugs] (
        [Id] int NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Strength] nvarchar(max) NOT NULL,
        [Price] decimal(18,2) NOT NULL,
        [CategoryId] int NOT NULL,
        [DosageFormId] int NOT NULL,
        [UnitId] int NOT NULL,
        [ManufacturerId] int NOT NULL,
        [PrescriptionRequired] bit NOT NULL,
        [Description] nvarchar(max) NULL,
        [Usage] nvarchar(max) NULL,
        [Contraindications] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Drugs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620134440_InitialSqlServer'
)
BEGIN
    CREATE TABLE [ExternalDataSources] (
        [Id] int NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [SourceUrl] nvarchar(max) NOT NULL,
        [MappingStatus] nvarchar(max) NOT NULL,
        [LastSyncDate] date NULL,
        [Purpose] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_ExternalDataSources] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620134440_InitialSqlServer'
)
BEGIN
    CREATE TABLE [Manufacturers] (
        [Id] int NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Country] nvarchar(max) NULL,
        CONSTRAINT [PK_Manufacturers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620134440_InitialSqlServer'
)
BEGIN
    CREATE TABLE [PatientSafetyProfiles] (
        [Email] nvarchar(450) NOT NULL,
        [DisplayName] nvarchar(max) NOT NULL,
        [AllergyActiveIngredientIdsCsv] nvarchar(max) NOT NULL,
        [ClinicalNote] nvarchar(max) NULL,
        CONSTRAINT [PK_PatientSafetyProfiles] PRIMARY KEY ([Email])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620134440_InitialSqlServer'
)
BEGIN
    CREATE TABLE [Units] (
        [Id] int NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Units] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620134440_InitialSqlServer'
)
BEGIN
    CREATE TABLE [Warehouses] (
        [Id] int NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Address] nvarchar(max) NULL,
        CONSTRAINT [PK_Warehouses] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620134440_InitialSqlServer'
)
BEGIN
    CREATE TABLE [DrugActiveIngredients] (
        [DrugId] int NOT NULL,
        [ActiveIngredientId] int NOT NULL,
        [Strength] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_DrugActiveIngredients] PRIMARY KEY ([DrugId], [ActiveIngredientId]),
        CONSTRAINT [FK_DrugActiveIngredients_ActiveIngredients_ActiveIngredientId] FOREIGN KEY ([ActiveIngredientId]) REFERENCES [ActiveIngredients] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DrugActiveIngredients_Drugs_DrugId] FOREIGN KEY ([DrugId]) REFERENCES [Drugs] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620134440_InitialSqlServer'
)
BEGIN
    CREATE TABLE [ExpertReviews] (
        [Id] int NOT NULL,
        [SourceDrugId] int NOT NULL,
        [RecommendedDrugId] int NOT NULL,
        [Score] int NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [Reviewer] nvarchar(max) NOT NULL,
        [Note] nvarchar(max) NOT NULL,
        [UpdatedAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_ExpertReviews] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ExpertReviews_Drugs_RecommendedDrugId] FOREIGN KEY ([RecommendedDrugId]) REFERENCES [Drugs] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ExpertReviews_Drugs_SourceDrugId] FOREIGN KEY ([SourceDrugId]) REFERENCES [Drugs] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620134440_InitialSqlServer'
)
BEGIN
    CREATE TABLE [Batches] (
        [Id] int NOT NULL,
        [DrugId] int NOT NULL,
        [WarehouseId] int NOT NULL,
        [BatchNumber] nvarchar(max) NOT NULL,
        [Quantity] int NOT NULL,
        [ExpiryDate] date NOT NULL,
        [ImportedDate] date NOT NULL,
        CONSTRAINT [PK_Batches] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Batches_Drugs_DrugId] FOREIGN KEY ([DrugId]) REFERENCES [Drugs] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Batches_Warehouses_WarehouseId] FOREIGN KEY ([WarehouseId]) REFERENCES [Warehouses] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620134440_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_CreatedAt] ON [AuditLogs] ([CreatedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620134440_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_Batches_DrugId_ExpiryDate] ON [Batches] ([DrugId], [ExpiryDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620134440_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_Batches_WarehouseId] ON [Batches] ([WarehouseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620134440_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_DrugActiveIngredients_ActiveIngredientId] ON [DrugActiveIngredients] ([ActiveIngredientId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620134440_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_Drugs_Name] ON [Drugs] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620134440_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_ExpertReviews_RecommendedDrugId] ON [ExpertReviews] ([RecommendedDrugId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620134440_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_ExpertReviews_SourceDrugId] ON [ExpertReviews] ([SourceDrugId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620134440_InitialSqlServer'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260620134440_InitialSqlServer', N'10.0.9');
END;

COMMIT;
GO
