using System;
using System.Windows.Forms;

namespace Uniso.InStat.Gui.Forms
{
    public partial class CorrectTimeForm : Form
    {
        public int CorrectTime { get; set; }

        public CorrectTimeForm(int current_actual_time)
        {
            InitializeComponent();

            var ts = TimeSpan.FromMilliseconds(current_actual_time);
            numericUpDown1.Value = Convert.ToInt32(Math.Floor(ts.TotalMinutes));
            numericUpDown2.Value = Convert.ToInt32(ts.Seconds);
            numericUpDown3.Value = Convert.ToInt32(ts.Milliseconds) / 100;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            CorrectTime = Convert.ToInt32(numericUpDown1.Value) * 60000 + Convert.ToInt32(numericUpDown2.Value) * 1000 + Convert.ToInt32(numericUpDown3.Value) * 100;
        }
    }
}
