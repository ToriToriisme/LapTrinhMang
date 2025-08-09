using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    static void Main()
    {
        // Cấu hình địa chỉ và cổng
        IPAddress ip = IPAddress.Any;  // Lắng nghe tất cả địa chỉ
        int port = 5000;

        // Tạo socket TCP
        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            // Gắn socket vào IP và port
            listener.Bind(new IPEndPoint(ip, port));

            // Bắt đầu lắng nghe (tối đa 10 hàng đợi)
            listener.Listen(10);

            Console.WriteLine($"[SERVER] Đang lắng nghe tại cổng {port}...");

            while (true)
            {
                // Chấp nhận kết nối từ client
                Socket clientSocket = listener.Accept();
                Console.WriteLine("[SERVER] Client đã kết nối!");

                // Nhận dữ liệu từ client
                byte[] buffer = new byte[1024];
                int bytesReceived = clientSocket.Receive(buffer);
                string data = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                Console.WriteLine("[CLIENT]: " + data);

                // Gửi phản hồi
                string response = "Server đã nhận: " + data;
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                clientSocket.Send(responseBytes);

                // Đóng kết nối với client
                clientSocket.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi: " + ex.Message);
        }
    }
}
