using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("=== DEMO BẤT ĐỒNG BỘ (async/await) ===");
        Console.WriteLine("1) FakeTask demo (sync vs async)");
        Console.WriteLine("2) TCP Echo server demo (test bằng Packet Sender)\n");

        // --- Phần 1: FakeTask (như trước) ---
        await RunFakeTaskDemo();

        // --- Phần 2: TCP Echo Server ---
        Console.WriteLine("\n--- TCP ECHO SERVER ---");
        Console.WriteLine("Server đang lắng nghe tại 127.0.0.1:5000");
        Console.WriteLine("Mở Packet Sender -> tạo gói TCP tới 127.0.0.1:5000 -> nhập message -> Send.");
        Console.WriteLine("Mỗi message gửi sẽ được server trả về dạng ECHO: <MESSAGE>.\n");

        await RunTcpServer(); // chạy vô hạn
    }

    // ===================== DEMO 1: FAKE TASK =====================
    static async Task RunFakeTaskDemo()
    {
        int taskCount = 5;

        Console.WriteLine("\n>>> Chạy tuần tự (Sync)");
        var sw1 = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 1; i <= taskCount; i++)
        {
            var result = await FakeTask(i);
            Console.WriteLine(result);
        }
        sw1.Stop();
        Console.WriteLine($"Tổng thời gian (sync): {sw1.ElapsedMilliseconds} ms\n");

        Console.WriteLine(">>> Chạy bất đồng bộ (Async)");
        var sw2 = System.Diagnostics.Stopwatch.StartNew();
        var tasks = new Task<string>[taskCount];
        for (int i = 1; i <= taskCount; i++)
            tasks[i - 1] = FakeTask(i);

        var results = await Task.WhenAll(tasks);
        foreach (var r in results)
            Console.WriteLine(r);
        sw2.Stop();
        Console.WriteLine($"Tổng thời gian (async): {sw2.ElapsedMilliseconds} ms\n");
    }

    static async Task<string> FakeTask(int id)
    {
        await Task.Delay(1000); // giả lập IO chờ
        return $"Task {id} hoàn thành lúc {DateTime.Now:HH:mm:ss.fff}";
    }

    // ===================== DEMO 2: TCP SERVER =====================
    static async Task RunTcpServer()
    {
        var listener = new TcpListener(IPAddress.Loopback, 5000);
        listener.Start();

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            _ = HandleClientAsync(client); // xử lý song song, không chặn vòng lặp
        }
    }

    static async Task HandleClientAsync(TcpClient client)
    {
        Console.WriteLine($"[+] Client kết nối: {client.Client.RemoteEndPoint}");
        using var stream = client.GetStream();
        var buffer = new byte[1024];

        try
        {
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break; // client đóng

                var msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Nhận: {msg.Trim()} từ {client.Client.RemoteEndPoint}");

                // Giả lập IO chậm nếu message chứa chữ "slow"
                if (msg.Contains("slow", StringComparison.OrdinalIgnoreCase))
                    await Task.Delay(2000);

                var resp = Encoding.UTF8.GetBytes("ECHO: " + msg.ToUpper());
                await stream.WriteAsync(resp, 0, resp.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi: {ex.Message}");
        }
        finally
        {
            client.Close();
            Console.WriteLine($"[-] Client ngắt kết nối");
        }
    }
}
