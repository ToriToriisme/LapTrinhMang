using System.Windows.Forms;

namespace CaroWinApp.UI
{
    partial class SetupForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtPlayerX;
        private TextBox txtPlayerO;
        private Label lblX;
        private Label lblO;
        private GroupBox grpMode;
        private RadioButton rdoLocal;
        private RadioButton rdoTimed15;
        private RadioButton rdoVsComputer;
        private Button btnOk;
        private Button btnCancel;

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
            txtPlayerX = new TextBox();
            txtPlayerO = new TextBox();
            lblX = new Label();
            lblO = new Label();
            grpMode = new GroupBox();
            rdoLocal = new RadioButton();
            rdoTimed15 = new RadioButton();
            rdoVsComputer = new RadioButton();
            btnOk = new Button();
            btnCancel = new Button();

            SuspendLayout();

            lblX.AutoSize = true;
            lblX.Location = new System.Drawing.Point(12, 15);
            lblX.Text = "Player X:";
            txtPlayerX.Location = new System.Drawing.Point(85, 12);
            txtPlayerX.Size = new System.Drawing.Size(200, 23);

            lblO.AutoSize = true;
            lblO.Location = new System.Drawing.Point(12, 50);
            lblO.Text = "Player O:";
            txtPlayerO.Location = new System.Drawing.Point(85, 47);
            txtPlayerO.Size = new System.Drawing.Size(200, 23);

            grpMode.Location = new System.Drawing.Point(12, 85);
            grpMode.Size = new System.Drawing.Size(273, 100);
            grpMode.Text = "Mode";

            rdoLocal.Location = new System.Drawing.Point(10, 22);
            rdoLocal.Text = "Local (no timer)";
            rdoLocal.AutoSize = true;
            rdoLocal.Checked = true;

            rdoTimed15.Location = new System.Drawing.Point(10, 45);
            rdoTimed15.Text = "Timed 15 minutes";
            rdoTimed15.AutoSize = true;

            rdoVsComputer.Location = new System.Drawing.Point(10, 68);
            rdoVsComputer.Text = "Vs Computer (no timer)";
            rdoVsComputer.AutoSize = true;

            grpMode.Controls.Add(rdoLocal);
            grpMode.Controls.Add(rdoTimed15);
            grpMode.Controls.Add(rdoVsComputer);

            btnOk.Location = new System.Drawing.Point(129, 195);
            btnOk.Size = new System.Drawing.Size(75, 30);
            btnOk.Text = "OK";
            btnOk.Click += btnOk_Click;

            btnCancel.Location = new System.Drawing.Point(210, 195);
            btnCancel.Size = new System.Drawing.Size(75, 30);
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;

            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(297, 235);
            Controls.Add(lblX);
            Controls.Add(txtPlayerX);
            Controls.Add(lblO);
            Controls.Add(txtPlayerO);
            Controls.Add(grpMode);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SetupForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Game Setup";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
