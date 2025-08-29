using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Server
{
    static async Task HandleClient(TcpClient client)
    {
        var stream = client.GetStream();
        var buffer = new byte[1024];
        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
        if (int.TryParse(message, out int num))
        {
            Console.WriteLine($"[SERVER] Received {num} from {client.Client.RemoteEndPoint}");

            await Task.Delay(num * 1000); // Delay theo số giây
            string reply = $"Done after {num} seconds\n";
            byte[] data = Encoding.UTF8.GetBytes(reply);
            await stream.WriteAsync(data, 0, data.Length);
            Console.WriteLine($"[SERVER] Sent reply for {num}");
        }
        else
        {
            Console.WriteLine("[SERVER] Invalid data");
        }

        client.Close();
    }

    static async Task Main()
    {
        TcpListener listener = new TcpListener(IPAddress.Loopback, 8888);
        listener.Start();
        Console.WriteLine("[SERVER] Listening on 127.0.0.1:8888");

        while (true)
        {
            TcpClient client = await listener.AcceptTcpClientAsync();
            _ = HandleClient(client); // chạy bất đồng bộ, không chờ
        }
    }
}