# Tài khoản thử nghiệm theo vai trò

Tài liệu này chỉ dùng cho môi trường phát triển và kiểm thử nội bộ. Không dùng các tài khoản này trên production, không đưa thông tin đăng nhập vào giao diện, ảnh chụp công khai, Jira hoặc nhật ký ứng dụng.

Ứng dụng chỉ nạp bảng dưới đây khi chạy trong `Development` hoặc `Testing`. Môi trường `Production`
phải nhận bộ tài khoản đã băm từ `Authentication__EncodedAccounts`; thiếu cấu hình thì ứng dụng dừng
khởi động. Mật khẩu production được xoay vòng và chỉ lưu trong
`D:\OneDrive\Desktop\n4wttProductionAccounts.txt`, là file ngoài repository.

| Vai trò | Email | Mật khẩu | Area sau đăng nhập |
|---|---|---|---|
| Quản trị viên | `admin@nhom4.local` | `Admin@123` | `/Admin` |
| Dược sĩ | `duocsi@nhom4.local` | `Duocsi@123` | `/Pharmacist` |
| Chuyên gia | `chuyengia@nhom4.local` | `Chuyengia@123` | `/Expert` |
| Người dùng | `user@nhom4.local` | `User@123` | `/User` |

## Nguyên tắc

- Thay tài khoản in-memory bằng kho danh tính phù hợp trước khi phát hành thực tế.
- Cấp vai trò ở phía máy chủ; màn hình đăng nhập không cho người dùng tự chọn vai trò.
- Đổi hoặc vô hiệu hóa toàn bộ tài khoản mẫu khi chuyển sang môi trường production.
- Kiểm tra chéo để tài khoản chỉ truy cập Area được cấp và nhận `403` hoặc chuyển hướng an toàn với Area khác.
