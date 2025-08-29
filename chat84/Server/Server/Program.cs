using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    private static List<IPEndPoint> clients = new List<IPEndPoint>();
    private const int Port = 12345;

    static async Task Main(string[] args)
    {
        // Tạo một UdpClient và gán cho nó một cổng.
        using (var server = new UdpClient(Port))
        {
            Console.WriteLine($"Server đang lắng nghe trên cổng {Port}...");

            while (true)
            {
                try
                {
                    // Nhận gói tin một cách không đồng bộ
                    var receivedResults = await server.ReceiveAsync();
                    byte[] receivedBytes = receivedResults.Buffer;
                    IPEndPoint sender = receivedResults.RemoteEndPoint;

                    string message = Encoding.UTF8.GetString(receivedBytes);
                    Console.WriteLine($"Nhận từ {sender}: {message}");

                    // Thêm máy khách mới vào danh sách
                    if (!clients.Contains(sender))
                    {
                        clients.Add(sender);
                        Console.WriteLine($"Máy khách mới kết nối từ {sender}");
                    }

                    // Phát lại tin nhắn cho tất cả các máy khách khác
                    foreach (var client in clients)
                    {
                        if (!client.Equals(sender))
                        {
                            await server.SendAsync(receivedBytes, receivedBytes.Length, client);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Lỗi: {e.Message}");
                }
            }
        }
    }
}