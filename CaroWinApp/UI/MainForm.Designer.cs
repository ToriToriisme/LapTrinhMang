using System.Windows.Forms;

namespace CaroWinApp.UI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel panelBoard;
        private Button btnReset;
        private Label lblStatus;
        private ComboBox cboMode;
        private Label lblMode;
        private Label lblXMoves;
        private Label lblOMoves;
        private Label lblXScore;
        private Label lblOScore;

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
            btnReset = new Button();
            lblStatus = new Label();
            lblMode = new Label();
            cboMode = new ComboBox();
            lblXMoves = new Label();
            lblOMoves = new Label();
            lblXScore = new Label();
            lblOScore = new Label();

            SuspendLayout();

            panelBoard.Location = new System.Drawing.Point(12, 12);
            panelBoard.Name = "panelBoard";
            panelBoard.Size = new System.Drawing.Size(600, 600);
            panelBoard.TabIndex = 0;
            panelBoard.Paint += panelBoard_Paint;
            panelBoard.MouseClick += panelBoard_MouseClick;

            lblStatus.AutoSize = true;
            lblStatus.Location = new System.Drawing.Point(630, 20);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(67, 15);
            lblStatus.Text = "Turn: X";

            lblMode.AutoSize = true;
            lblMode.Location = new System.Drawing.Point(630, 55);
            lblMode.Text = "Mode:";

            cboMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cboMode.Location = new System.Drawing.Point(680, 52);
            cboMode.Size = new System.Drawing.Size(150, 23);
            cboMode.Items.AddRange(new object[] { "Local", "Timed 15m", "Vs Computer" });
            cboMode.SelectedIndexChanged += cboMode_SelectedIndexChanged;

            btnReset.Location = new System.Drawing.Point(630, 90);
            btnReset.Size = new System.Drawing.Size(200, 30);
            btnReset.Text = "New Game";
            btnReset.Click += btnReset_Click;

            lblXMoves.AutoSize = true;
            lblXMoves.Location = new System.Drawing.Point(630, 140);
            lblXMoves.Text = "X moves: 0";

            lblOMoves.AutoSize = true;
            lblOMoves.Location = new System.Drawing.Point(630, 160);
            lblOMoves.Text = "O moves: 0";

            lblXScore.AutoSize = true;
            lblXScore.Location = new System.Drawing.Point(630, 200);
            lblXScore.Text = "X W-L: 0-0";

            lblOScore.AutoSize = true;
            lblOScore.Location = new System.Drawing.Point(630, 220);
            lblOScore.Text = "O W-L: 0-0";

            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(870, 630);
            Controls.Add(panelBoard);
            Controls.Add(lblStatus);
            Controls.Add(lblMode);
            Controls.Add(cboMode);
            Controls.Add(btnReset);
            Controls.Add(lblXMoves);
            Controls.Add(lblOMoves);
            Controls.Add(lblXScore);
            Controls.Add(lblOScore);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Caro (Local 2 Players)";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}


