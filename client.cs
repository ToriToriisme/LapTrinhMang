using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Client
{
    static void Main(string[] args)
    {
        try
        {
            // Cấu hình server IP và port
            string serverIP = "127.0.0.1"; // IP của server (local)
            int port = 5000;

            // Tạo socket TCP
            TcpClient client = new TcpClient();
            client.Connect(serverIP, port);

            Console.WriteLine("Đã kết nối đến server!");

            // Lấy stream để gửi/nhận dữ liệu
            NetworkStream stream = client.GetStream();

            while (true)
            {
                Console.Write("Bạn: ");
                string message = Console.ReadLine();

                // Nếu người dùng gõ "exit" thì thoát
                if (message.ToLower() == "exit")
                    break;

                // Gửi dữ liệu tới server
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);

                // Nhận phản hồi từ server
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Server: " + response);
            }

            // Đóng kết nối
            stream.Close();
            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi: " + ex.Message);
        }
    }
}
