## Caro (Gomoku) WinForms — Local 2 Players (1 màn hình)

Ứng dụng Caro cho 2 người chơi trên cùng một máy tính và một màn hình. Mỗi lượt bấm chuột sẽ lần lượt đặt quân `X` rồi `O`. Trò chơi tự động kiểm tra 5 quân liên tiếp để xác định người thắng.

### Cách chạy (Windows App)

- Mở solution/project và chạy `CaroWinApp` (F5 trong Visual Studio hoặc `dotnet run --project "LapTrinhMang/Cuối-Kỳ/CaroWinApp"`).
- Giao diện gồm bàn cờ, nhãn trạng thái lượt chơi và nút "New Game".
- Người chơi lần lượt nhấp vào ô trống để đặt quân:
  - `X` đi trước.
  - Khi có 5 quân liên tiếp (ngang/dọc/chéo), trò chơi thông báo người thắng.
  - Nhấn "New Game" để bắt đầu ván mới.

### Thiết lập trước trận (Setup)

- Khi khởi động, hộp thoại Setup xuất hiện để nhập tên người chơi và chọn chế độ:
  - Local (không giờ)
  - Timed 15 minutes (đồng hồ đếm lùi 15 phút)
  - Vs Computer (không giờ)
  - Lưu ý: Tên của cả hai người chơi là bắt buộc và phải khác nhau. Nếu hủy Setup, ứng dụng sẽ thoát.

### Chế độ/Modes

- Local: hai người chơi trên cùng một máy, không giới hạn thời gian.
- Timed 15 minutes: một đồng hồ 15 phút cho toàn ván; hết giờ thì người còn lại thắng.
- Vs Computer: đấu với máy (AI cơ bản), không giới hạn thời gian.

### Đổi chế độ ngay trong trận

- Có thể đổi chế độ bất kỳ lúc nào bằng hộp chọn "Mode" trên màn hình chính (không cần thoát game):
  - Chuyển sang Timed 15m sẽ khởi tạo đồng hồ 15 phút ngay lập tức.
  - Chuyển sang Local/Vs Computer sẽ dừng đồng hồ nếu đang chạy.
  - Bàn cờ và lượt hiện tại được giữ nguyên khi đổi chế độ.

### Bộ đếm nước đi và Bảng điểm

- Bộ đếm nước đi: nhãn "X moves" và "O moves" tăng sau mỗi lần đặt quân hợp lệ, và được đặt lại về 0 khi bấm "New Game".
- Bảng điểm (theo phiên chơi):
  - "X W-L" và "O W-L" được cập nhật khi có người thắng (bao gồm thắng do đối thủ hết giờ ở chế độ Timed 15m).
  - Điểm thắng/thua được giữ trong suốt phiên chạy ứng dụng; bấm "New Game" chỉ bắt đầu ván mới nhưng không xóa điểm.

### Điều khiển và Luật

- Nhấp chuột trái vào ô trống để đặt quân của người đang đến lượt.
- `X` và `O` thay phiên nhau; không thể đánh đè lên ô đã có quân.
- Thắng khi có 5 quân liên tiếp theo hàng ngang, dọc hoặc chéo.

### Telemetry cho Wireshark

- Ứng dụng phát gói UDP cục bộ để tiện quan sát với Wireshark.
- Mặc định gửi đến `127.0.0.1:9999` với thông điệp dạng văn bản:
  - `MOVE r c X` hoặc `MOVE r c O`
  - `RESET`
- Gợi ý lọc: dùng bộ lọc `udp.port == 9999` trong Wireshark.

### Xây dựng và chạy thủ công

- Yêu cầu .NET 6+ (hoặc phiên bản tương thích với project).
- Chạy bằng CLI:
  - `dotnet build` (tại thư mục gốc repo hoặc thư mục project)
  - `dotnet run --project "LapTrinhMang/Cuối-Kỳ/CaroWinApp"`

### Ghi chú

- Dự án đã được đơn giản hóa thành chế độ local 2 người chơi trên 1 màn hình; các tính năng mạng (Host/Join/Chat) đã được loại bỏ khỏi ứng dụng Windows.
- Nếu cần chơi qua mạng hoặc chạy console, có thể tham khảo project `CaroConsole` và tự mở rộng thêm.

