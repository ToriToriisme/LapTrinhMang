using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

const int BoardSize = 15;
char[,] board = new char[BoardSize, BoardSize];

void Draw()
{
    if (!Console.IsOutputRedirected)
    {
        Console.Clear();
    }
    Console.WriteLine("   " + string.Join(" ", Enumerable.Range(0, BoardSize).Select(c => c.ToString("D2"))));
    for (int r = 0; r < BoardSize; r++)
    {
        var row = new StringBuilder();
        row.Append(r.ToString("D2")).Append(" ");
        for (int c = 0; c < BoardSize; c++)
        {
            row.Append(board[r, c] == '\0' ? '.' : board[r, c]).Append(' ');
        }
        Console.WriteLine(row.ToString());
    }
}

bool CheckWin(int r, int c, char ch)
{
    int CountDir(int rr, int cc, int dr, int dc)
    {
        int cnt = 0; rr += dr; cc += dc;
        while (rr >= 0 && rr < BoardSize && cc >= 0 && cc < BoardSize && board[rr, cc] == ch)
        { cnt++; rr += dr; cc += dc; }
        return cnt;
    }
    int[][] dirs = new[] { new[]{0,1}, new[]{1,0}, new[]{1,1}, new[]{1,-1} };
    foreach (var d in dirs)
    {
        int count = 1 + CountDir(r, c, d[0], d[1]) + CountDir(r, c, -d[0], -d[1]);
        if (count >= 5) return true;
    }
    return false;
}

bool Place(int r, int c, char ch)
{
    if (r < 0 || r >= BoardSize || c < 0 || c >= BoardSize) return false;
    if (board[r, c] != '\0') return false;
    board[r, c] = ch; return true;
}

void Reset() => Array.Clear(board, 0, board.Length);

if (args.Length == 0)
{
    Console.WriteLine("Chế độ: local | host <port> | join <ip> <port>");
    return;
}

string mode = args[0].ToLowerInvariant();
if (mode == "local")
{
    char cur = 'X';
    while (true)
    {
        Draw();
        Console.Write($"Lượt {cur}. Nhập 'r c' hoặc 'q' để thoát: ");
        var line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) continue;
        if (line.Trim().ToLower() == "q") break;
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2 && int.TryParse(parts[0], out int r) && int.TryParse(parts[1], out int c))
        {
            if (Place(r, c, cur))
            {
                if (CheckWin(r, c, cur)) { Draw(); Console.WriteLine($"{cur} thắng!"); break; }
                cur = cur == 'X' ? 'O' : 'X';
            }
        }
    }
    return;
}

if (mode == "host" && args.Length >= 2)
{
    int port = int.Parse(args[1]);
    var listener = new TcpListener(IPAddress.Loopback, port);
    listener.Start();
    Console.WriteLine($"Host trên 127.0.0.1:{port}. Đang chờ kết nối...");
    using var tcp = listener.AcceptTcpClient();
    using var stream = tcp.GetStream();
    var me = 'X'; var opp = 'O'; bool myTurn = true;
    SendRaw(stream, "HELLO HOST X\n");
    Loop(stream, me, opp, () => myTurn, v => myTurn = v);
    return;
}

if (mode == "join" && args.Length >= 3)
{
    string ip = args[1]; int port = int.Parse(args[2]);
    using var tcp = new TcpClient();
    tcp.Connect(IPAddress.Parse(ip), port);
    using var stream = tcp.GetStream();
    var me = 'O'; var opp = 'X'; bool myTurn = false;
    SendRaw(stream, "HELLO CLIENT O\n");
    Loop(stream, me, opp, () => myTurn, v => myTurn = v);
    return;
}

Console.WriteLine("Sai tham số. Dùng: local | host <port> | join <ip> <port>");
return;

void Loop(NetworkStream stream, char me, char opp, Func<bool> getTurn, Action<bool> setTurn)
{
    Reset();
    var recvTask = Task.Run(async () =>
    {
        var buf = new byte[4096]; var sb = new StringBuilder();
        while (true)
        {
            int n = await stream.ReadAsync(buf, 0, buf.Length);
            if (n <= 0) break;
            sb.Append(Encoding.UTF8.GetString(buf, 0, n));
            int idx;
            while ((idx = sb.ToString().IndexOf('\n')) >= 0)
            {
                string line = sb.ToString(0, idx).Trim();
                sb.Remove(0, idx + 1);
                if (line.Length == 0) continue;
                Handle(line);
            }
        }
    });

    while (true)
    {
        Draw();
        Console.Write(getTurn() ? $"Lượt {me}. Nhập 'r c', 'chat ...', 'reset', 'q': " : $"Đợi đối thủ ({opp})... gõ 'chat ...' hoặc 'q': ");
        var line = Console.ReadLine();
        if (line == null) continue;
        if (line.Trim().ToLower() == "q") break;
        if (line.StartsWith("chat ", StringComparison.OrdinalIgnoreCase))
        {
            Send($"CHAT {line.Substring(5)}\n");
            continue;
        }
        if (line.Trim().ToLower() == "reset")
        {
            Reset();
            Send("RESET\n");
            continue;
        }
        if (!getTurn()) continue;
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2 && int.TryParse(parts[0], out int r) && int.TryParse(parts[1], out int c))
        {
            if (Place(r, c, me))
            {
                if (CheckWin(r, c, me)) { Draw(); Console.WriteLine($"{me} thắng!"); }
                setTurn(false);
                Send($"MOVE {r} {c}\n");
            }
        }
    }

    void Handle(string msg)
    {
        if (msg.StartsWith("MOVE "))
        {
            var p = msg.Split(' ');
            int r = int.Parse(p[1]); int c = int.Parse(p[2]);
            Place(r, c, opp);
            if (CheckWin(r, c, opp)) { Draw(); Console.WriteLine($"{opp} thắng!"); }
            setTurn(true);
        }
        else if (msg == "RESET")
        {
            Reset();
        }
        else if (msg.StartsWith("CHAT "))
        {
            Console.WriteLine($"Đối thủ: {msg.Substring(5)}");
        }
        else if (msg.StartsWith("HELLO "))
        {
            // ignore
        }
    }

    void Send(string s)
    {
        var data = Encoding.UTF8.GetBytes(s.EndsWith("\n") ? s : s + "\n");
        stream.Write(data, 0, data.Length);
    }
}

void SendRaw(NetworkStream stream, string s)
{
    var data = Encoding.UTF8.GetBytes(s.EndsWith("\n") ? s : s + "\n");
    stream.Write(data, 0, data.Length);
}


