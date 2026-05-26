# SRS Rut Gon - Web De Xuat Thuoc Thay The

## Muc Tieu

Xay dung website ho tro nguoi dung va duoc si tra cuu thuoc, kiem tra ton kho va de xuat thuoc thay the an toan khi thuoc chinh khong co san.

He thong khong thay the chi dinh cua bac si. Cac de xuat co rui ro cao phai hien thi canh bao ro va can xac nhan chuyen mon.

## Actor

| Actor | Vai tro |
|---|---|
| Khach mua thuoc | Tra cuu va xem thong tin thay the co ban |
| Nguoi dung tieu chuan | Dang nhap, tra cuu, xem goi y, luu lich su |
| Duoc si | Tu van, xac nhan canh bao, quan ly thuoc va kho |
| Chuyen gia y te | Danh gia ket qua AI va quy tac thay the |
| Quan tri vien | Quan ly tai khoan, danh muc, cau hinh, bao cao va backup |
| API ton kho nha thuoc | Cung cap ton kho theo thuoc/kho |
| Nguon du lieu duoc chuan | Cung cap du lieu thuoc, benh, target va bang chung |
| AI Model Service | Tinh AI Score va diem thanh phan |

## Pham Vi Sprint 0 va Sprint 1

Sprint 0 tap trung vao phan tich, repo, framework, kien truc va tai lieu thiet ke.

Sprint 1 tap trung vao nen tang co the chay duoc:

- Dang nhap bang tai khoan mau theo vai tro.
- Phan quyen Admin, Duoc si, Chuyen gia va Nguoi dung.
- Quan ly danh muc thuoc co ban.
- Quan ly kho va lo thuoc co ban.
- Tra cuu thuoc theo ten, hoat chat va danh muc.
- Hien thi ton kho tong hop.

## Yeu Cau Chuc Nang Chinh

| Ma | Nhom | Mo ta |
|---|---|---|
| FR-USER-01 | Nguoi dung | Dang nhap bang email/mat khau |
| FR-USER-02 | Nguoi dung | Phan quyen theo vai tro |
| FR-ADMIN-01 | Danh muc | CRUD thuoc va du lieu duoc ly |
| FR-ADMIN-03 | Kho | Quan ly kho, lo thuoc, so luong va han su dung |
| FR-SEARCH-01 | Tra cuu | Tim thuoc theo ten, hoat chat, danh muc |
| FR-SEARCH-02 | Ton kho | Tinh ton kho tu lo thuoc |
| FR-SEARCH-03 | Goi y | Xac dinh thuoc het hang lam dau vao cho sprint sau |

## Yeu Cau Phi Chuc Nang

- Luong chinh khong qua 3 click voi tac vu tra cuu.
- Thoi gian phan hoi tra cuu muc tieu duoi 2 giay voi du lieu mau.
- Mat khau tai khoan mau khong dung cho production.
- Phan quyen tren controller/action, khong chi an nut tren UI.
- Code tach lop theo MVC, Services, Data va Domain Models.
- Co README, convention branch/commit va test build truoc khi merge.

## Definition of Ready

- Story co actor ro rang.
- Co acceptance criteria.
- Co sprint, component va story point.
- Phu thuoc ky thuat da duoc ghi chu.

## Definition of Done

- Code build thanh cong.
- Luong chinh co UI co ban va validation.
- Controller co phan quyen phu hop.
- Du lieu mau du de demo.
- README/tai lieu cap nhat neu thay doi hanh vi.
- PR duoc review truoc khi merge vao `main`.
