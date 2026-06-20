# SQL Server Data Dictionary

## Core Catalog

| Table | Purpose | Important fields |
|---|---|---|
| `Categories` | Nhom dieu tri/danh muc thuoc | `Id`, `Name`, `Description` |
| `DosageForms` | Dang bao che | `Id`, `Name` |
| `Units` | Don vi tinh | `Id`, `Name` |
| `Manufacturers` | Nha san xuat | `Id`, `Name`, `Country` |
| `ActiveIngredients` | Hoat chat va canh bao | `Id`, `Name`, `Warning` |
| `Drugs` | Thong tin thuoc chinh | `Id`, `Name`, `Strength`, `Price`, foreign keys, `PrescriptionRequired`, `IsActive` |
| `DrugActiveIngredients` | Mapping thuoc-hoat chat | composite key `DrugId`, `ActiveIngredientId`, `Strength` |

## Inventory

| Table | Purpose | Important fields |
|---|---|---|
| `Warehouses` | Kho/quay thuoc | `Id`, `Name`, `Address` |
| `Batches` | Lo thuoc va ton kho | `Id`, `DrugId`, `WarehouseId`, `BatchNumber`, `Quantity`, `ImportedDate`, `ExpiryDate` |

## Recommendation Safety

| Table | Purpose | Important fields |
|---|---|---|
| `PatientSafetyProfiles` | Ho so canh bao nguoi dung | `Email`, `DisplayName`, `AllergyActiveIngredientIdsCsv`, `ClinicalNote` |
| `ExpertReviews` | Xac nhan ket qua de xuat | `SourceDrugId`, `RecommendedDrugId`, `Score`, `Status`, `Reviewer`, `Note`, `UpdatedAt` |

## Operations

| Table | Purpose | Important fields |
|---|---|---|
| `ExternalDataSources` | Registry nguon du lieu duoc | `Name`, `SourceUrl`, `MappingStatus`, `LastSyncDate`, `Purpose` |
| `AuditLogs` | Truy vet thao tac | `CreatedAt`, `Actor`, `Action`, `Entity`, `Detail` |
| `__EFMigrationsHistory` | Theo doi migration EF Core | `MigrationId`, `ProductVersion` |

## Story Mapping

| Jira | Database scope |
|---|---|
| `N4WTT-12`, `N4WTT-15`, `N4WTT-16` | `Drugs`, `ActiveIngredients`, `DrugActiveIngredients` |
| `N4WTT-13` | `Warehouses`, `Batches` |
| `N4WTT-18` | `PatientSafetyProfiles` |
| `N4WTT-19` | `ExpertReviews` |
| `N4WTT-20`, `N4WTT-22` | `AuditLogs` and reporting queries |
| `N4WTT-21` | `ExternalDataSources` |
| `N4WTT-145` | Entire SQL Server persistence layer |
