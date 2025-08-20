using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Client
{
    static readonly string[] choices = { "rock", "paper", "scissors" };

    public static void Run()
    {
        try
        {
            // Cấu hình server IP và port
            int port = 5050;

            using TcpClient client = new TcpClient(AddressFamily.InterNetwork);
            client.Connect(IPAddress.Loopback, port);
            using NetworkStream stream = client.GetStream();
            using StreamReader reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            using StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };

            Console.Write("Nhập tên bạn (Người chơi 2): ");
            string playerName = Console.ReadLine()?.Trim() ?? "Người chơi 2";

            // Gửi HELLO để server biết tên
            writer.WriteLine($"HELLO|{playerName}");

            int p1Wins = 0, p2Wins = 0, draws = 0;

            while (true)
            {
                // Chờ server yêu cầu lựa chọn
                string? cmd = reader.ReadLine();
                if (cmd == null) break;

                if (string.Equals(cmd, "ASK_CHOICE", StringComparison.OrdinalIgnoreCase))
                {
                    string playerChoice = GetValidChoice(playerName);
                    writer.WriteLine($"CHOICE|{playerChoice}");
                }
                else if (cmd.StartsWith("RESULT|", StringComparison.OrdinalIgnoreCase))
                {
                    string[] parts = cmd.Split('|');
                    // RESULT|p1Choice|p2Choice|result|p1Wins|p2Wins|draws
                    if (parts.Length >= 7)
                    {
                        string p1Choice = parts[1];
                        string p2Choice = parts[2];
                        string result = parts[3];
                        p1Wins = int.Parse(parts[4]);
                        p2Wins = int.Parse(parts[5]);
                        draws = int.Parse(parts[6]);

                        Console.WriteLine($"\nMáy chủ đã chọn: {p1Choice}");
                        Console.WriteLine($"Bạn đã chọn: {p2Choice}");
                        Console.WriteLine(result);
                        Console.WriteLine($"Tỉ số: Server {p1Wins} - {p2Wins} {playerName} (Hòa: {draws})");
                    }
                }
                else if (string.Equals(cmd, "REMATCH_REQ", StringComparison.OrdinalIgnoreCase))
                {
                    bool wants = AskYesNo("Chơi lại? (y/n): ");
                    writer.WriteLine(wants ? "REMATCH|Y" : "REMATCH|N");
                }
                else if (string.Equals(cmd, "REMATCH_OK", StringComparison.OrdinalIgnoreCase))
                {
                    // vòng tiếp theo
                    continue;
                }
                else if (string.Equals(cmd, "BYE", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Kết thúc trò chơi.");
                    break;
                }
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Không thể kết nối tới server. Hãy chắc chắn server đang chạy. Chi tiết: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi: " + ex.Message);
        }
    }

    static string GetValidChoice(string playerName)
    {
        while (true)
        {
            Console.Write($"{playerName}, hãy nhập lựa chọn của bạn (rock, paper, scissors): ");
            string? input = Console.ReadLine()?.Trim().ToLower();
            if (Array.IndexOf(choices, input ?? "") != -1)
                return input!;
            Console.WriteLine("Lựa chọn không hợp lệ! Vui lòng chọn rock, paper hoặc scissors.");
        }
    }

    static bool AskYesNo(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? ans = Console.ReadLine()?.Trim().ToLowerInvariant();
            if (ans == "y" || ans == "yes") return true;
            if (ans == "n" || ans == "no") return false;
            Console.WriteLine("Vui lòng nhập y/n.");
        }
    }
}
