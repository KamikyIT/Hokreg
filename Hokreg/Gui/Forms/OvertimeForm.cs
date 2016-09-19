using System;
using System.Windows.Forms;

namespace Uniso.InStat.Gui.Forms
{
    public partial class OvertimeForm : Form
    {
        public int Length { get; set; }
        public int Count { get; set; }

        public OvertimeForm()
        {
            InitializeComponent();
            Length = 5;
            Count = 4;
        }

        public OvertimeForm(int maxplayersnum, int lengthovertime)
        {
            InitializeComponent();
            Length = lengthovertime;
            Count = maxplayersnum;

            switch (maxplayersnum)
            { 
                case 3:
                    radioButton5.Checked = true;
                    break;

                case 4:
                    radioButton3.Checked = true;
                    break;

                case 5:
                    radioButton4.Checked = true;
                    break;
            }

            if (Length == 20)
                radioButton2.Checked = true;
            else
                radioButton1.Checked = true;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Length = Convert.ToInt32(((RadioButton)sender).Tag);
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            Count = Convert.ToInt32(((RadioButton)sender).Tag);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Close();
        }
    }
}
