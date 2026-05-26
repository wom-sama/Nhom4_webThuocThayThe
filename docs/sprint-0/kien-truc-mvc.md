# Kien Truc MVC Va Domain Sprint 1

## Muc Tieu Kien Truc

Kien truc sprint 1 uu tien mot monolith ASP.NET Core MVC de de demo va cham diem hoc phan, nhung van tach lop de co the mo rong sang database, API va AI service trong cac sprint sau.

## Layers

```text
Controllers -> Services -> Data/InMemoryStore -> Domain Models
Views       -> ViewModels -> Controllers
```

| Layer | Trach nhiem |
|---|---|
| Controllers | Dieu phoi request, authorization, validation co ban |
| Services | Xu ly nghiep vu: auth, permission, search, catalog, inventory |
| Data | Luu tru tam thoi bang in-memory seed data cho sprint 1 |
| Models | Entity/domain model co ban |
| ViewModels | Model rieng cho form va man hinh |
| Views | Razor UI MVC |

## Module Sprint 1

### Auth va RBAC

- Cookie authentication.
- Tai khoan mau theo vai tro: Admin, Duoc si, Chuyen gia, Nguoi dung.
- Policies: `AdminOnly`, `CatalogManager`, `InventoryManager`, `AuthenticatedUser`.

### Drug Catalog

Entity chinh:

- `Drug`
- `DrugCategory`
- `DosageForm`
- `Unit`
- `Manufacturer`
- `ActiveIngredient`
- `DrugActiveIngredient`

### Inventory

Entity chinh:

- `Warehouse`
- `DrugBatch`
- `StockSummary`

Ton kho duoc tinh bang tong so luong cac lo con han va thuoc dang hoat dong.

### Search

Search service tim theo:

- Ten thuoc.
- Hoat chat.
- Danh muc.
- Nha san xuat.

Ket qua hien thi:

- Ten thuoc.
- Ham luong.
- Dang bao che.
- Gia.
- Ton kho.
- Trang thai con/hit hang.

## Quy Uoc Git

- Branch theo issue: `N4WTT-10-login-rbac`.
- Commit co Jira key: `N4WTT-10 Add login and RBAC`.
- PR phai co Jira key trong title.
- Build/test truoc khi merge.

## Ghi Chu Mo Rong Sprint Sau

- Sprint 2 se them recommendation, AI score va safety rules.
- Data layer hien tai co the thay bang EF Core DbContext khi chot database.
- Services da tach interface de giam thay doi controller khi thay storage.
