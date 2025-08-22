using System.Windows.Forms;

namespace CaroWinApp.UI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel panelBoard;
        private Button btnHost;
        private Button btnJoin;
        private TextBox txtPeerIp;
        private TextBox txtPort;
        private Label lblIp;
        private Label lblPort;
        private ListBox lstLog;
        private Button btnReset;
        private TextBox txtChat;
        private Button btnSendChat;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            panelBoard = new Panel();
            btnHost = new Button();
            btnJoin = new Button();
            txtPeerIp = new TextBox();
            txtPort = new TextBox();
            lblIp = new Label();
            lblPort = new Label();
            lstLog = new ListBox();
            btnReset = new Button();

            SuspendLayout();

            panelBoard.Location = new System.Drawing.Point(12, 12);
            panelBoard.Name = "panelBoard";
            panelBoard.Size = new System.Drawing.Size(600, 600);
            panelBoard.TabIndex = 0;
            panelBoard.Paint += panelBoard_Paint;
            panelBoard.MouseClick += panelBoard_MouseClick;

            lblIp.AutoSize = true;
            lblIp.Location = new System.Drawing.Point(630, 15);
            lblIp.Text = "Peer IP:";

            txtPeerIp.Location = new System.Drawing.Point(690, 12);
            txtPeerIp.Size = new System.Drawing.Size(180, 23);
            txtPeerIp.Text = "127.0.0.1";

            lblPort.AutoSize = true;
            lblPort.Location = new System.Drawing.Point(630, 50);
            lblPort.Text = "Port:";

            txtPort.Location = new System.Drawing.Point(690, 47);
            txtPort.Size = new System.Drawing.Size(80, 23);
            txtPort.Text = "9090";

            btnHost.Location = new System.Drawing.Point(630, 85);
            btnHost.Size = new System.Drawing.Size(100, 30);
            btnHost.Text = "Host";
            btnHost.Click += btnHost_Click;

            btnJoin.Location = new System.Drawing.Point(770, 85);
            btnJoin.Size = new System.Drawing.Size(100, 30);
            btnJoin.Text = "Join";
            btnJoin.Click += btnJoin_Click;

            btnReset.Location = new System.Drawing.Point(630, 130);
            btnReset.Size = new System.Drawing.Size(240, 30);
            btnReset.Text = "Reset Game";
            btnReset.Click += btnReset_Click;

            lstLog.Location = new System.Drawing.Point(630, 180);
            lstLog.Size = new System.Drawing.Size(240, 380);

            txtChat.Location = new System.Drawing.Point(630, 570);
            txtChat.Size = new System.Drawing.Size(180, 23);

            btnSendChat.Location = new System.Drawing.Point(820, 568);
            btnSendChat.Size = new System.Drawing.Size(50, 27);
            btnSendChat.Text = "Send";
            btnSendChat.Click += btnSendChat_Click;

            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(890, 624);
            Controls.Add(panelBoard);
            Controls.Add(lblIp);
            Controls.Add(txtPeerIp);
            Controls.Add(lblPort);
            Controls.Add(txtPort);
            Controls.Add(btnHost);
            Controls.Add(btnJoin);
            Controls.Add(btnReset);
            Controls.Add(lstLog);
            Controls.Add(txtChat);
            Controls.Add(btnSendChat);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Caro (TCP)";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}


