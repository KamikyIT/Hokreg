using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Uniso.InStat.Gui.Forms
{
    public partial class ViolationForm : Form
    {
        private int foul1 = 2;
        private Game.Marker marker = null;
        private Player foulPlayer = null;

        public ViolationForm(Game.Marker marker, Player player)
        {
            InitializeComponent();
            this.marker = marker;

            radioButton1.Enabled = marker.player2_id > 0;
            radioButton2.Enabled = marker.player2_id > 0;
            radioButton3.Enabled = marker.player2_id > 0;
            radioButton4.Enabled = marker.player2_id > 0;
            radioButton5.Enabled = marker.player2_id > 0;
            radioButton6.Enabled = marker.player2_id > 0;
            radioButton7.Enabled = marker.player2_id == 0;
            radioButton8.Enabled = marker.player2_id > 0;
            radioButton9.Enabled = marker.player2_id > 0;
            radioButton10.Enabled = marker.player2_id == 0;
            radioButton11.Enabled = marker.player2_id > 0;
            radioButton12.Enabled = marker.player2_id > 0;
            radioButton13.Enabled = marker.player2_id > 0;
            radioButton14.Enabled = marker.player2_id == 0;

            if (marker.player2_id == 0)
                radioButton7.Checked = true;

            foulPlayer = player;
            checkBox1.Enabled = marker.player2_id > 0 && foulPlayer == marker.Player1;

            if (foulPlayer != null)
            {
                label1.Text = foulPlayer.ToString();
            }
            else
            {
                groupBox1.Enabled = false;
            }

            UpdateUI();
        }

        public List<Game.Marker> GetResult()
        {
            var res = new List<Game.Marker>();

            if (foul1 > 0)
                res.Add(new Game.Marker(marker.game, 9, foul1) { Half = marker.Half, TimeVideo = marker.TimeVideo, Player1 = foulPlayer });

            return res;
        }

        //Marker 1
        private void radioButton13_CheckedChanged(object sender, EventArgs e)
        {
            foul1 = Convert.ToInt32(((RadioButton)sender).Tag);
            UpdateUI();
        }

        private void UpdateUI()
        {

        }

        public bool IsPair
        {
            get { return checkBox1.Checked; }
        }
    }
}
