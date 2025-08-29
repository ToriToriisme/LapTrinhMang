using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    // Địa chỉ và cổng của máy chủ mà máy khách sẽ kết nối
    private const string ServerHost = "127.0.0.1";
    private const int ServerPort = 12345;

    // Đối tượng UdpClient để thực hiện việc gửi và nhận dữ liệu UDP
    private static UdpClient client;

    static async Task Main(string[] args)
    {
        client = new UdpClient();
        // Tạo một đối tượng IPEndPoint để xác định địa chỉ và cổng của máy chủ
        IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse(ServerHost), ServerPort);

        Console.WriteLine("Bạn đã tham gia chat. Gõ 'exit' để thoát.");

        // Bắt đầu một tác vụ (task) mới để xử lý việc nhận tin nhắn
        // Điều này cho phép máy khách vừa có thể gửi tin nhắn vừa có thể nhận tin nhắn
        Task receiveTask = Task.Run(ReceiveMessages);

        while (true)
        {
            // Vòng lặp để người dùng nhập tin nhắn
            Console.Write("Bạn: ");
            string message = Console.ReadLine();

            if (string.IsNullOrEmpty(message) || message.ToLower() == "exit")
            {
                break;
            }

            // Chuyển đổi chuỗi tin nhắn thành mảng byte để gửi qua UDP
            byte[] bytesToSend = Encoding.UTF8.GetBytes(message);

            // Gửi mảng byte đến máy chủ
            await client.SendAsync(bytesToSend, bytesToSend.Length, serverEndpoint);
        }

        // Đóng UdpClient khi người dùng thoát
        client.Close();
        Console.WriteLine("Bạn đã thoát chat.");
    }

    // Hàm bất đồng bộ để nhận tin nhắn từ máy chủ
    private static async Task ReceiveMessages()
    {
        try
        {
            // Vòng lặp liên tục để lắng nghe tin nhắn
            while (true)
            {
                // Nhận gói tin một cách bất đồng bộ
                var receivedResults = await client.ReceiveAsync();

                // Giải mã dữ liệu nhận được thành chuỗi
                string message = Encoding.UTF8.GetString(receivedResults.Buffer);

                // Hiển thị tin nhắn và địa chỉ của người gửi
                Console.WriteLine($"\n[{receivedResults.RemoteEndPoint}] {message}");
            }
        }
        catch (ObjectDisposedException)
        {
            // Ngoại lệ này xảy ra khi UdpClient bị đóng
            // Chúng ta có thể bỏ qua nó một cách an toàn
        }
        catch (Exception e)
        {
            Console.WriteLine($"Lỗi khi nhận tin nhắn: {e.Message}");
        }
    }
}