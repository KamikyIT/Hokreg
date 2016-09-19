using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Uniso.InStat.Gui.Forms
{
    public partial class MediaInputForm : Form
    {
        private Uniso.InStat.Gui.UISync sync = null;

        public int SourceType = 1;
        public int Offset = 0;
        public String AudioName = String.Empty;
        public String VideoName = String.Empty;
        int match_id; int half;
        
        public MediaInputForm(int match_id, int half)
        {
            InitializeComponent();
            this.match_id = match_id;
            this.half = half;
        }

        private void UpdateUI()
        {
            sync.Execute(() =>
            {
                radioButton1.Text = String.Format("http://satellite.instatfootball.tv/{2}{0}/{1}", match_id + 10000000, half, "");

                switch (SourceType)
                {
                    case 0:
                        VideoName = radioButton1.Text;
                        break;

                    case 1:
                        VideoName = textBox2.Text;
                        break;
                }

                button1.Enabled = SourceType == 1;
                textBox2.Enabled = SourceType == 1;

                button2.Enabled = (SourceType == 0) || (SourceType == 1 && File.Exists(VideoName));

            });
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var open = new OpenFileDialog();
            if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                textBox2.Text = open.FileName;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            var rb = (RadioButton)sender;
            if (rb.Checked)
            {
                SourceType = Convert.ToInt32(rb.Tag);
                UpdateUI();
            }
        }

        private void MediaInputForm_Load(object sender, EventArgs e)
        {
            sync = new Gui.UISync(this);
            
            UpdateUI();
        }

        private int old_code = 0;
        public void ShowStatus(String msg, int code)
        {
            sync.Execute(() =>
            {
                if (code != old_code)
                {
                    switch (code)
                    {
                        case 0:
                            toolStripStatusLabel1.BackColor = SystemColors.Control;
                            toolStripStatusLabel1.ForeColor = Color.Black;
                            break;
                        case 1:
                            toolStripStatusLabel1.BackColor = Color.Red;
                            toolStripStatusLabel1.ForeColor = Color.White;
                            break;
                    }
                    old_code = code;
                }

                if (msg.Length > 300)
                    msg = msg.Substring(0, 300);

                toolStripStatusLabel1.Text = msg;
            });
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }
    }
}
