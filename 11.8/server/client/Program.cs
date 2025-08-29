using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Client
{
    static async Task SendNumber(int num)
    {
        using (TcpClient client = new TcpClient())
        {
            await client.ConnectAsync("127.0.0.1", 8888);
            var stream = client.GetStream();

            Console.WriteLine($"[CLIENT] Sending {num}");
            byte[] data = Encoding.UTF8.GetBytes(num.ToString());
            await stream.WriteAsync(data, 0, data.Length);

            var buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string reply = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
            Console.WriteLine($"[CLIENT] Got reply: {reply}");
        }
    }

    static async Task Main()
    {
        // Gửi 2 yêu cầu gần như đồng thời
        await Task.WhenAll(
            SendNumber(10),
            SendNumber(2)
        );
    }
}