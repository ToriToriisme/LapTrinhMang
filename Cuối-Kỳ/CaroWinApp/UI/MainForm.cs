using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaroWinApp.UI
{
    public partial class MainForm : Form
    {
        private const int BoardSize = 15;
        private const int CellSize = 40;
        private readonly char[,] board = new char[BoardSize, BoardSize];
        private bool isXTurn = true;
        private bool gameOver = false;

        private string playerXName = string.Empty;
        private string playerOName = string.Empty;
        private GameMode currentMode = GameMode.Local;

        private System.Windows.Forms.Timer? gameTimer;
        private TimeSpan remaining = TimeSpan.Zero;

        private bool vsComputer = false;
        private char humanChar = 'X';
        private readonly Random rng = new Random();

        private UdpClient? udpTelemetry;
        private int telemetryPort = 9999;

        private int xMoves = 0;
        private int oMoves = 0;
        private int xWins = 0;
        private int xLosses = 0;
        private int oWins = 0;
        private int oLosses = 0;

        public MainForm()
        {
            InitializeComponent();
            DoubleBuffered = true;
            UpdateStatus();
            UpdateCounters();
            UpdateScores();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ShowSetupDialog();
        }

        private void ShowSetupDialog()
        {
            using var dlg = new SetupForm();
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                playerXName = dlg.PlayerX.Trim();
                playerOName = dlg.PlayerO.Trim();
                currentMode = dlg.SelectedMode;
                vsComputer = currentMode == GameMode.VsComputer;
                humanChar = 'X';

                if (cboMode != null)
                {
                    switch (currentMode)
                    {
                        case GameMode.Local: cboMode.SelectedIndex = 0; break;
                        case GameMode.Timed15: cboMode.SelectedIndex = 1; break;
                        case GameMode.VsComputer: cboMode.SelectedIndex = 2; break;
                    }
                }

                ResetBoard();
                SetupMode();
            }
            else
            {
                Close();
            }
        }

        private void SetupMode()
        {
            StopTimer();
            if (currentMode == GameMode.Timed15)
            {
                remaining = TimeSpan.FromMinutes(15);
                gameTimer = new System.Windows.Forms.Timer { Interval = 1000 };
                gameTimer.Tick += (s, e) =>
                {
                    if (gameOver) { StopTimer(); return; }
                    remaining = remaining.Subtract(TimeSpan.FromSeconds(1));
                    if (remaining.TotalSeconds <= 0)
                    {
                        gameOver = true;
                        lblStatus.Text = $"Time up! {(isXTurn ? playerOName : playerXName)} wins.";
                        if (isXTurn)
                        {
                            // X to move, time up => O wins
                            oWins++; xLosses++;
                        }
                        else
                        {
                            xWins++; oLosses++;
                        }
                        UpdateScores();
                        StopTimer();
                    }
                    else
                    {
                        UpdateStatus();
                    }
                };
                gameTimer.Start();
            }
            vsComputer = currentMode == GameMode.VsComputer;
            UpdateStatus();
            InitTelemetry();
        }

        private void StopTimer()
        {
            if (gameTimer != null)
            {
                try { gameTimer.Stop(); } catch { }
                gameTimer.Dispose();
                gameTimer = null;
            }
        }

        private void InitTelemetry()
        {
            try
            {
                udpTelemetry?.Dispose();
                udpTelemetry = new UdpClient();
            }
            catch { }
        }

        private async Task SendTelemetryAsync(string message)
        {
            try
            {
                if (udpTelemetry == null) return;
                var data = Encoding.UTF8.GetBytes(message);
                await udpTelemetry.SendAsync(data, data.Length, "127.0.0.1", telemetryPort);
            }
            catch { }
        }

        private void UpdateStatus()
        {
            string turnName = isXTurn ? playerXName : playerOName;
            string baseText = gameOver ? "Game over." : $"Turn: {(isXTurn ? 'X' : 'O')} ({turnName})";
            if (currentMode == GameMode.Timed15 && !gameOver)
            {
                baseText += $"  |  Time left: {remaining:mm\\:ss}";
            }
            lblStatus.Text = baseText;
        }

        private void UpdateCounters()
        {
            lblXMoves.Text = $"X moves: {xMoves}";
            lblOMoves.Text = $"O moves: {oMoves}";
        }

        private void UpdateScores()
        {
            lblXScore.Text = $"X W-L: {xWins}-{xLosses}";
            lblOScore.Text = $"O W-L: {oWins}-{oLosses}";
        }

        private void cboMode_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cboMode.SelectedIndex == 0) currentMode = GameMode.Local;
            else if (cboMode.SelectedIndex == 1) currentMode = GameMode.Timed15;
            else if (cboMode.SelectedIndex == 2) currentMode = GameMode.VsComputer;
            SetupMode();
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

        private async void panelBoard_MouseClick(object? sender, MouseEventArgs e)
        {
            if (gameOver) return;
            int c = e.X / CellSize;
            int r = e.Y / CellSize;
            if (r < 0 || r >= BoardSize || c < 0 || c >= BoardSize) return;
            if (board[r, c] != '\0') return;

            char ch = isXTurn ? 'X' : 'O';
            PlaceLocal(r, c, ch);
            await SendTelemetryAsync($"MOVE {r} {c} {ch}");

            if (currentMode == GameMode.VsComputer && !gameOver && !isXTurn)
            {
                await Task.Delay(200);
                var (ar, ac) = ChooseAiMove();
                if (ar >= 0)
                {
                    PlaceLocal(ar, ac, 'O');
                    await SendTelemetryAsync($"MOVE {ar} {ac} O");
                }
            }
        }

        private void PlaceLocal(int r, int c, char ch)
        {
            board[r, c] = ch;
            if (ch == 'X') xMoves++; else oMoves++;
            UpdateCounters();
            panelBoard.Invalidate();
            if (CheckWin(r, c, ch))
            {
                gameOver = true;
                string winnerName = ch == 'X' ? playerXName : playerOName;
                lblStatus.Text = $"Player {ch} ({winnerName}) wins!";
                if (ch == 'X') { xWins++; oLosses++; } else { oWins++; xLosses++; }
                UpdateScores();
                StopTimer();
                return;
            }
            isXTurn = !isXTurn;
            UpdateStatus();
        }

        private (int r, int c) ChooseAiMove()
        {
            if (board[BoardSize / 2, BoardSize / 2] == '\0') return (BoardSize / 2, BoardSize / 2);
            var empties = new List<(int r, int c)>();
            for (int r = 0; r < BoardSize; r++)
                for (int c = 0; c < BoardSize; c++)
                    if (board[r, c] == '\0') empties.Add((r, c));
            if (empties.Count == 0) return (-1, -1);
            return empties[rng.Next(empties.Count)];
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

        private async void btnReset_Click(object? sender, EventArgs e)
        {
            ResetBoard();
            SetupMode();
            await SendTelemetryAsync("RESET");
        }

        private void ResetBoard()
        {
            Array.Clear(board, 0, board.Length);
            panelBoard.Invalidate();
            gameOver = false;
            isXTurn = true;
            xMoves = 0; oMoves = 0; UpdateCounters();
            UpdateStatus();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            try { udpTelemetry?.Dispose(); } catch { }
        }
    }

    public enum GameMode
    {
        Local,
        Timed15,
        VsComputer
    }
}


