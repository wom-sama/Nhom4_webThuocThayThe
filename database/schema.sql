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

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622114019_VietnameseSeedContent'
)
BEGIN
    UPDATE [Categories] SET [Name] = N'Giảm đau hạ sốt', [Description] = N'Thuốc điều trị đau, sốt thông thường.' WHERE [Id] = 1;
    UPDATE [Categories] SET [Name] = N'Kháng sinh', [Description] = N'Thuốc cần kê đơn và cần xác nhận chuyên môn.' WHERE [Id] = 2;
    UPDATE [Categories] SET [Name] = N'Tiêu hóa', [Description] = N'Thuốc hỗ trợ hệ tiêu hóa.' WHERE [Id] = 3;
    UPDATE [Categories] SET [Name] = N'Dị ứng', [Description] = N'Thuốc hỗ trợ triệu chứng dị ứng.' WHERE [Id] = 4;

    UPDATE [DosageForms] SET [Name] = N'Viên nén' WHERE [Id] = 1;
    UPDATE [DosageForms] SET [Name] = N'Viên nang' WHERE [Id] = 2;
    UPDATE [DosageForms] SET [Name] = N'Viên sủi' WHERE [Id] = 4;
    UPDATE [Units] SET [Name] = N'Viên' WHERE [Id] = 1;
    UPDATE [Units] SET [Name] = N'Hộp' WHERE [Id] = 2;
    UPDATE [Manufacturers] SET [Country] = N'Việt Nam' WHERE [Id] IN (1, 3, 4);

    UPDATE [ActiveIngredients] SET [Warning] = N'Thận trọng với bệnh gan.' WHERE [Id] = 1;
    UPDATE [ActiveIngredients] SET [Warning] = N'Thận trọng với đau dạ dày.' WHERE [Id] = 2;
    UPDATE [ActiveIngredients] SET [Warning] = N'Kháng sinh cần kê đơn.' WHERE [Id] = 3;
    UPDATE [ActiveIngredients] SET [Warning] = N'Có thể gây buồn ngủ.' WHERE [Id] = 4;

    UPDATE [Drugs] SET [Description] = N'Thuốc giảm đau hạ sốt chứa paracetamol.', [Usage] = N'Dùng theo hướng dẫn của dược sĩ.', [Contraindications] = N'Quá mẫn với paracetamol.' WHERE [Id] = 1;
    UPDATE [Drugs] SET [Description] = N'Thuốc giảm đau hạ sốt thay thế cùng hoạt chất.', [Usage] = N'Dùng theo hướng dẫn của dược sĩ.', [Contraindications] = N'Quá mẫn với paracetamol.' WHERE [Id] = 2;
    UPDATE [Drugs] SET [Description] = N'Thuốc giảm đau kháng viêm không steroid.', [Usage] = N'Dùng sau ăn.', [Contraindications] = N'Loét dạ dày tiến triển.' WHERE [Id] = 3;
    UPDATE [Drugs] SET [Description] = N'Kháng sinh beta-lactam.', [Usage] = N'Dùng theo đơn bác sĩ.', [Contraindications] = N'Dị ứng penicillin.' WHERE [Id] = 4;
    UPDATE [Drugs] SET [Description] = N'Thuốc giảm đau hạ sốt dạng viên sủi.', [Usage] = N'Hòa tan trong nước trước khi dùng.', [Contraindications] = N'Quá mẫn với paracetamol.' WHERE [Id] = 5;
    UPDATE [Drugs] SET [Description] = N'Thuốc giảm đau hạ sốt hàm lượng cao hơn.', [Usage] = N'Dùng theo hướng dẫn của dược sĩ.', [Contraindications] = N'Thận trọng với người bệnh gan.' WHERE [Id] = 6;
    UPDATE [Drugs] SET [Description] = N'Thuốc kháng histamine hỗ trợ triệu chứng dị ứng.', [Usage] = N'Dùng một lần mỗi ngày theo hướng dẫn.', [Contraindications] = N'Quá mẫn với cetirizine.' WHERE [Id] = 7;

    UPDATE [Warehouses] SET [Name] = N'Kho trung tâm', [Address] = N'Quận 1' WHERE [Id] = 1;
    UPDATE [Warehouses] SET [Name] = N'Quầy bán lẻ', [Address] = N'Nhà thuốc số 1' WHERE [Id] = 2;
    UPDATE [PatientSafetyProfiles] SET [DisplayName] = N'Người dùng mặc định', [ClinicalNote] = N'Dị ứng với nhóm NSAID, ưu tiên cảnh báo ibuprofen.' WHERE [Email] = N'user@nhom4.local';
    UPDATE [PatientSafetyProfiles] SET [DisplayName] = N'Hồ sơ kiểm thử dược sĩ', [ClinicalNote] = N'Hồ sơ mẫu dùng để kiểm tra cảnh báo paracetamol.' WHERE [Email] = N'duocsi@nhom4.local';

    UPDATE [ExternalDataSources] SET [MappingStatus] = N'Sẵn sàng ánh xạ', [Purpose] = N'Thông tin thuốc, hoạt chất, chỉ định và tương tác.' WHERE [Id] = 1;
    UPDATE [ExternalDataSources] SET [MappingStatus] = N'Đang đánh giá', [Purpose] = N'Đối chiếu hợp chất, CID và cấu trúc hóa học.' WHERE [Id] = 2;
    UPDATE [ExternalDataSources] SET [MappingStatus] = N'Đã ánh xạ mẫu', [Purpose] = N'Phân loại nhóm điều trị và hỗ trợ lọc ứng viên thay thế.' WHERE [Id] = 3;

    UPDATE [ExpertReviews] SET [Reviewer] = N'Chưa gán', [Note] = N'Cùng hoạt chất, cùng hàm lượng, còn hàng.' WHERE [Id] = 1;
    UPDATE [ExpertReviews] SET [Reviewer] = N'Chuyên gia mẫu', [Note] = N'Cùng hoạt chất và hàm lượng nhưng khác dạng bào chế.' WHERE [Id] = 2;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622114019_VietnameseSeedContent'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260622114019_VietnameseSeedContent', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623133411_UserAccountsAndLibraryPersistence'
)
BEGIN
    CREATE TABLE [RegisteredUserAccounts] (
        [Email] nvarchar(450) NOT NULL,
        [DisplayName] nvarchar(max) NOT NULL,
        [Role] nvarchar(450) NOT NULL,
        [PasswordSalt] nvarchar(max) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [IsLocked] bit NOT NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_RegisteredUserAccounts] PRIMARY KEY ([Email])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623133411_UserAccountsAndLibraryPersistence'
)
BEGIN
    CREATE TABLE [SavedDrugs] (
        [Id] int NOT NULL IDENTITY,
        [UserEmail] nvarchar(450) NOT NULL,
        [DrugId] int NOT NULL,
        [SavedAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_SavedDrugs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SavedDrugs_Drugs_DrugId] FOREIGN KEY ([DrugId]) REFERENCES [Drugs] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623133411_UserAccountsAndLibraryPersistence'
)
BEGIN
    CREATE TABLE [UserSearchHistories] (
        [Id] int NOT NULL IDENTITY,
        [UserEmail] nvarchar(450) NOT NULL,
        [Keyword] nvarchar(max) NULL,
        [CategoryId] int NULL,
        [CategoryName] nvarchar(max) NULL,
        [ResultCount] int NOT NULL,
        [SearchedAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_UserSearchHistories] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623133411_UserAccountsAndLibraryPersistence'
)
BEGIN
    CREATE INDEX [IX_RegisteredUserAccounts_Role] ON [RegisteredUserAccounts] ([Role]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623133411_UserAccountsAndLibraryPersistence'
)
BEGIN
    CREATE INDEX [IX_SavedDrugs_DrugId] ON [SavedDrugs] ([DrugId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623133411_UserAccountsAndLibraryPersistence'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SavedDrugs_UserEmail_DrugId] ON [SavedDrugs] ([UserEmail], [DrugId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623133411_UserAccountsAndLibraryPersistence'
)
BEGIN
    CREATE INDEX [IX_UserSearchHistories_UserEmail_SearchedAt] ON [UserSearchHistories] ([UserEmail], [SearchedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623133411_UserAccountsAndLibraryPersistence'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260623133411_UserAccountsAndLibraryPersistence', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623170727_AddDatabaseForeignKeyConstraints'
)
BEGIN
    CREATE INDEX [IX_UserSearchHistories_CategoryId] ON [UserSearchHistories] ([CategoryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623170727_AddDatabaseForeignKeyConstraints'
)
BEGIN
    CREATE INDEX [IX_Drugs_CategoryId] ON [Drugs] ([CategoryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623170727_AddDatabaseForeignKeyConstraints'
)
BEGIN
    CREATE INDEX [IX_Drugs_DosageFormId] ON [Drugs] ([DosageFormId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623170727_AddDatabaseForeignKeyConstraints'
)
BEGIN
    CREATE INDEX [IX_Drugs_ManufacturerId] ON [Drugs] ([ManufacturerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623170727_AddDatabaseForeignKeyConstraints'
)
BEGIN
    CREATE INDEX [IX_Drugs_UnitId] ON [Drugs] ([UnitId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623170727_AddDatabaseForeignKeyConstraints'
)
BEGIN
    ALTER TABLE [Drugs] ADD CONSTRAINT [FK_Drugs_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623170727_AddDatabaseForeignKeyConstraints'
)
BEGIN
    ALTER TABLE [Drugs] ADD CONSTRAINT [FK_Drugs_DosageForms_DosageFormId] FOREIGN KEY ([DosageFormId]) REFERENCES [DosageForms] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623170727_AddDatabaseForeignKeyConstraints'
)
BEGIN
    ALTER TABLE [Drugs] ADD CONSTRAINT [FK_Drugs_Manufacturers_ManufacturerId] FOREIGN KEY ([ManufacturerId]) REFERENCES [Manufacturers] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623170727_AddDatabaseForeignKeyConstraints'
)
BEGIN
    ALTER TABLE [Drugs] ADD CONSTRAINT [FK_Drugs_Units_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [Units] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623170727_AddDatabaseForeignKeyConstraints'
)
BEGIN
    ALTER TABLE [UserSearchHistories] ADD CONSTRAINT [FK_UserSearchHistories_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE SET NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623170727_AddDatabaseForeignKeyConstraints'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260623170727_AddDatabaseForeignKeyConstraints', N'10.0.9');
END;

COMMIT;
GO

