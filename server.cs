using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    static readonly string[] choices = { "rock", "paper", "scissors" };

    public static void Run()
    {
        // Cấu hình địa chỉ và cổng
        IPAddress ip = IPAddress.Any;  // Lắng nghe tất cả địa chỉ
        int port = 5050;

        // Tạo socket TCP
        using Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

        try
        {
            // Gắn socket vào IP và port
            listener.Bind(new IPEndPoint(ip, port));

            // Bắt đầu lắng nghe
            listener.Listen(1);

            Console.WriteLine($"[SERVER] Đang lắng nghe tại cổng {port}...");

            while (true)
            {
                using Socket clientSocket = listener.Accept();
                Console.WriteLine("[SERVER] Client đã kết nối!");

                using NetworkStream stream = new NetworkStream(clientSocket, ownsSocket: false);
                using StreamReader reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
                using StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };

                // Người chơi 1 (máy chủ) - nhập một lần
                Console.Write("Nhập tên của bạn (Người chơi 1): ");
                string player1Name = Console.ReadLine()?.Trim() ?? "Player1";

                // Nhận tên client (HELLO|name)
                string helloLine = reader.ReadLine() ?? string.Empty;
                string player2Name = "Player2";
                if (helloLine.StartsWith("HELLO|", StringComparison.OrdinalIgnoreCase))
                {
                    player2Name = helloLine.Substring("HELLO|".Length);
                }

                int p1Wins = 0, p2Wins = 0, draws = 0;

                bool continuePlaying = true;
                while (continuePlaying)
                {
                    string player1Choice = GetValidChoice(player1Name);

                    // Yêu cầu client nhập lựa chọn
                    writer.WriteLine("ASK_CHOICE");
                    string line = reader.ReadLine() ?? string.Empty; // CHOICE|<choice>
                    string player2Choice = "rock";
                    if (line.StartsWith("CHOICE|", StringComparison.OrdinalIgnoreCase))
                        player2Choice = line.Substring("CHOICE|".Length);

                    // Công bố lựa chọn
                    Console.WriteLine($"\n{player1Name} đã chọn: {player1Choice}");
                    Console.WriteLine($"{player2Name} đã chọn: {player2Choice}");

                    // Xác định người chiến thắng + cập nhật điểm
                    string result = DetermineWinner(player1Name, player1Choice, player2Name, player2Choice);
                    if (player1Choice == player2Choice) draws++;
                    else if ((player1Choice == "rock" && player2Choice == "scissors") ||
                             (player1Choice == "paper" && player2Choice == "rock") ||
                             (player1Choice == "scissors" && player2Choice == "paper"))
                        p1Wins++;
                    else
                        p2Wins++;

                    Console.WriteLine(result);
                    Console.WriteLine($"Tỉ số: {player1Name} {p1Wins} - {p2Wins} {player2Name} (Hòa: {draws})");

                    // Gửi kết quả và bảng điểm cho client
                    writer.WriteLine($"RESULT|{player1Choice}|{player2Choice}|{result}|{p1Wins}|{p2Wins}|{draws}");

                    // Hỏi chơi lại từ client
                    writer.WriteLine("REMATCH_REQ");
                    string clientAnsLine = reader.ReadLine() ?? string.Empty; // REMATCH|Y/N
                    bool clientWants = clientAnsLine.EndsWith("Y", StringComparison.OrdinalIgnoreCase);

                    // Hỏi người chơi 1
                    bool serverWants = AskYesNo("Chơi lại? (y/n): ");

                    if (serverWants && clientWants)
                    {
                        writer.WriteLine("REMATCH_OK");
                        continue;
                    }
                    else
                    {
                        writer.WriteLine("BYE");
                        continuePlaying = false;
                    }
                }

                Console.WriteLine("Kết thúc phiên chơi với client. Đang chờ client tiếp theo...\n");
            }
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
            Console.Write($"{playerName}, nhập lựa chọn của bạn (rock, paper, scissors): ");
            string? input = Console.ReadLine()?.Trim().ToLower();
            if (Array.IndexOf(choices, input ?? "") != -1)
                return input!;
            Console.WriteLine("Lựa chọn không hợp lệ! Vui lòng chọn rock, paper, hoặc scissors.");
        }
    }

    static string DetermineWinner(string player1Name, string player1Choice, string player2Name, string player2Choice)
    {
        if (player1Choice == player2Choice)
            return "Hòa nhau!";
        bool player1Wins = (player1Choice == "rock" && player2Choice == "scissors") ||
                           (player1Choice == "paper" && player2Choice == "rock") ||
                           (player1Choice == "scissors" && player2Choice == "paper");
        if (player1Wins)
            return $"{player1Name} thắng!";
        else
            return $"{player2Name} thắng!";
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
