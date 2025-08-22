using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaroWinApp.UI
{
    public partial class MainForm : Form
    {
        private const int BoardSize = 15;
        private const int CellSize = 40;
        private readonly char[,] board = new char[BoardSize, BoardSize];
        private bool isMyTurn = false;
        private char myChar = 'X';
        private char oppChar = 'O';
        private TcpListener? listener;
        private TcpClient? client;
        private NetworkStream? stream;
        private CancellationTokenSource? cts;
        private readonly object netLock = new object();
        private const string ChatPrefix = "CHAT";

        public MainForm()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        private void Log(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(Log), message);
                return;
            }
            lstLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            lstLog.TopIndex = lstLog.Items.Count - 1;
        }

        private void panelBoard_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);
            using var gridPen = new Pen(Color.LightGray);
            for (int i = 0; i <= BoardSize; i++)
            {
                g.DrawLine(gridPen, 0, i * CellSize, BoardSize * CellSize, i * CellSize);
                g.DrawLine(gridPen, i * CellSize, 0, i * CellSize, BoardSize * CellSize);
            }
            using var xBrush = new SolidBrush(Color.DarkBlue);
            using var oBrush = new SolidBrush(Color.DarkRed);
            using var font = new Font(FontFamily.GenericSansSerif, 18, FontStyle.Bold);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            for (int r = 0; r < BoardSize; r++)
            {
                for (int c = 0; c < BoardSize; c++)
                {
                    char ch = board[r, c];
                    if (ch == '\0') continue;
                    var rect = new Rectangle(c * CellSize, r * CellSize, CellSize, CellSize);
                    g.DrawString(ch.ToString(), font, ch == 'X' ? xBrush : oBrush, rect, sf);
                }
            }
        }

        private void panelBoard_MouseClick(object? sender, MouseEventArgs e)
        {
            if (!isMyTurn || stream == null) return;
            int c = e.X / CellSize;
            int r = e.Y / CellSize;
            if (r < 0 || r >= BoardSize || c < 0 || c >= BoardSize) return;
            if (board[r, c] != '\0') return;

            PlaceAndSend(r, c, myChar);
        }

        private void PlaceAndSend(int r, int c, char ch)
        {
            board[r, c] = ch;
            Invalidate(panelBoard.Bounds);
            Log($"Move {ch} at {r},{c}");
            if (CheckWin(r, c, ch))
            {
                Log($"Player {ch} wins!");
            }
            isMyTurn = ch != myChar;

            if (ch == myChar && stream != null)
            {
                SendLine($"MOVE {r} {c}");
            }
        }

        private bool CheckWin(int r, int c, char ch)
        {
            int[][] dirs = new[] {
                new[]{0,1}, new[]{1,0}, new[]{1,1}, new[]{1,-1}
            };
            foreach (var d in dirs)
            {
                int count = 1;
                count += CountDir(r, c, d[0], d[1], ch);
                count += CountDir(r, c, -d[0], -d[1], ch);
                if (count >= 5) return true;
            }
            return false;
        }

        private int CountDir(int r, int c, int dr, int dc, char ch)
        {
            int cnt = 0;
            int rr = r + dr, cc = c + dc;
            while (rr >= 0 && rr < BoardSize && cc >= 0 && cc < BoardSize && board[rr, cc] == ch)
            {
                cnt++;
                rr += dr; cc += dc;
            }
            return cnt;
        }

        private async void btnHost_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtPort.Text, out int port)) { MessageBox.Show("Invalid port"); return; }
            await StartHostAsync(port);
        }

        private async void btnJoin_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtPort.Text, out int port)) { MessageBox.Show("Invalid port"); return; }
            await JoinAsync(txtPeerIp.Text.Trim(), port);
        }

        private void btnReset_Click(object? sender, EventArgs e)
        {
            ResetBoard();
            SendLine("RESET");
        }

        private void btnSendChat_Click(object? sender, EventArgs e)
        {
            var text = txtChat.Text.Trim();
            if (text.Length == 0) return;
            SendLine($"{ChatPrefix} {text}");
            Log($"Me: {text}");
            txtChat.Clear();
        }

        private void ResetBoard()
        {
            Array.Clear(board, 0, board.Length);
            Invalidate(panelBoard.Bounds);
            Log("Board reset");
        }

        private async Task StartHostAsync(int port)
        {
            CleanupNet();
            myChar = 'X';
            oppChar = 'O';
            isMyTurn = true;
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Log($"Hosting on 0.0.0.0:{port}");
            client = await listener.AcceptTcpClientAsync();
            stream = client.GetStream();
            Log("Client connected");
            cts = new CancellationTokenSource();
            _ = Task.Run(() => ReceiveLoop(cts.Token));
            SendLine("HELLO HOST X");
        }

        private async Task JoinAsync(string host, int port)
        {
            CleanupNet();
            myChar = 'O';
            oppChar = 'X';
            isMyTurn = false;
            client = new TcpClient();
            await client.ConnectAsync(host, port);
            stream = client.GetStream();
            Log($"Connected to {host}:{port}");
            cts = new CancellationTokenSource();
            _ = Task.Run(() => ReceiveLoop(cts.Token));
            SendLine("HELLO CLIENT O");
        }

        private void CleanupNet()
        {
            try { cts?.Cancel(); } catch { }
            try { stream?.Dispose(); } catch { }
            try { client?.Close(); } catch { }
            try { listener?.Stop(); } catch { }
            cts = null; stream = null; client = null; listener = null;
        }

        private void SendLine(string line)
        {
            try
            {
                lock (netLock)
                {
                    if (stream == null) return;
                    var data = Encoding.UTF8.GetBytes(line + "\n");
                    stream.Write(data, 0, data.Length);
                }
                Log($"-> {line}");
            }
            catch (Exception ex)
            {
                Log($"Send error: {ex.Message}");
            }
        }

        private async Task ReceiveLoop(CancellationToken token)
        {
            var buffer = new byte[4096];
            var sb = new StringBuilder();
            try
            {
                while (!token.IsCancellationRequested && stream != null)
                {
                    int n = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                    if (n <= 0) break;
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, n));
                    int idx;
                    while ((idx = sb.ToString().IndexOf('\n')) >= 0)
                    {
                        string line = sb.ToString(0, idx).Trim();
                        sb.Remove(0, idx + 1);
                        if (line.Length == 0) continue;
                        HandleLine(line);
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Log($"Receive error: {ex.Message}");
            }
            Log("Connection closed");
        }

        private void HandleLine(string line)
        {
            Log($"<- {line}");
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return;
            switch (parts[0].ToUpperInvariant())
            {
                case "MOVE":
                    if (parts.Length >= 3 && int.TryParse(parts[1], out int r) && int.TryParse(parts[2], out int c))
                    {
                        if (r >= 0 && r < BoardSize && c >= 0 && c < BoardSize && board[r, c] == '\0')
                        {
                            PlaceAndSend(r, c, oppChar);
                        }
                    }
                    break;
                case "RESET":
                    ResetBoard();
                    break;
                case "HELLO":
                    // no-op: visible in log for Wireshark/demo
                    break;
                case ChatPrefix:
                    if (parts.Length >= 2)
                    {
                        var msg = line.Substring(ChatPrefix.Length).TrimStart();
                        Log($"Peer: {msg}");
                    }
                    break;
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            CleanupNet();
        }
    }
}


