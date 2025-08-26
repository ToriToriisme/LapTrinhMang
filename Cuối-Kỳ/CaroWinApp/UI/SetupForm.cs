using System;
using System.Windows.Forms;

namespace CaroWinApp.UI
{
    public partial class SetupForm : Form
    {
        public string PlayerX => txtPlayerX.Text;
        public string PlayerO => txtPlayerO.Text;
        public GameMode SelectedMode
        {
            get
            {
                if (rdoTimed15.Checked) return GameMode.Timed15;
                if (rdoVsComputer.Checked) return GameMode.VsComputer;
                return GameMode.Local;
            }
        }

        public SetupForm()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object? sender, EventArgs e)
        {
            var x = (txtPlayerX.Text ?? string.Empty).Trim();
            var o = (txtPlayerO.Text ?? string.Empty).Trim();
            if (x.Length == 0 || o.Length == 0)
            {
                MessageBox.Show(this, "Both player names are required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.Equals(x, o, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(this, "Player names must be different.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
