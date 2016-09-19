using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Uniso.InStat.Gui.Controls;

namespace Uniso.InStat.Gui.Forms
{
    public partial class FineForm : Form
    {
        private int type11 = 0;
        private int type12 = 0;
        private int foul1 = 2;
        private Game.Marker marker = null;
        private Player foulPlayer = null;
        private Player selectedPlayer = null;

        public bool IsPair
        {
            get { return checkBox1.Checked; }
        }

        public bool WithoutDelete
        {
            get { return checkBox2.Checked; }
            set { checkBox2.Checked = value; }
        }

        public FineForm(Game.Marker marker, Player player)
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
                LoadPlayers(false);
            }
            else
            {
                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
                groupBox3.Enabled = false;
            }

            UpdateUI();
        }

        private void LoadPlayers(bool all)
        {
            comboBox1.Items.Clear();

            all = true;

            if (all)
            {
                foreach (var id in foulPlayer.Team.Tactics.Keys)
                {
                    if (id == 0)
                        continue;

                    foreach (var p in foulPlayer.Team.Tactics[id].GetPlayers())
                    {
                        if (!p.IsGk && !foulPlayer.Team.FinePlayers.Exists(o => o.Player1 == p && ((bool)o.Tag)))
                            comboBox1.Items.Add(p);
                    }
                }
            }
            else
            {
                foreach (var p in foulPlayer.Team.Tactics[0].GetPlayers())
                {
                    if (!p.IsGk && !foulPlayer.Team.FinePlayers.Exists(o => o.Player1 == p && ((bool)o.Tag)) && HockeyGui.IsPlaying(p))
                        comboBox1.Items.Add(p);
                }
            }

            comboBox1.SelectedItem = foulPlayer;
            comboBox1.Enabled = foulPlayer.IsGk || type12 == 7;
        }

        public List<Game.Marker> GetResult()
        {
            var res = new List<Game.Marker>();

            if (type11 == 5 && type12 == 7 && foulPlayer.IsGk)
            {
                res.Add(new Game.Marker(marker.game, 5, type11) { Half = marker.Half, TimeVideo = marker.TimeVideo, Player1 = selectedPlayer, Player2 = foulPlayer });
                res.Add(new Game.Marker(marker.game, 5, type12) { Half = marker.Half, TimeVideo = marker.TimeVideo, Player1 = foulPlayer, Player2 = foulPlayer });
            }
            else if (type11 <= 5 && type12 == 7/* && !foulPlayer.IsGk*/)
            {
                res.Add(new Game.Marker(marker.game, 5, type11) { Half = marker.Half, TimeVideo = marker.TimeVideo, Player1 = selectedPlayer, Player2 = foulPlayer });
                res.Add(new Game.Marker(marker.game, 5, type12) { Half = marker.Half, TimeVideo = marker.TimeVideo, Player1 = foulPlayer, Player2 = foulPlayer });
            }
            else
            {
                if (type11 > 0)
                    res.Add(new Game.Marker(marker.game, 5, type11) { Half = marker.Half, TimeVideo = marker.TimeVideo, Player1 = selectedPlayer, Player2 = foulPlayer });

                if (type12 > 0)
                    res.Add(new Game.Marker(marker.game, 5, type12) { Half = marker.Half, TimeVideo = marker.TimeVideo, Player1 = selectedPlayer, Player2 = foulPlayer });
            }

            if (foul1 > 0)
                res.Add(new Game.Marker(marker.game, 9, foul1) { Half = marker.Half, TimeVideo = marker.TimeVideo, Player1 = foulPlayer });

            return res;
        }

        private void radioButton15_CheckedChanged(object sender, EventArgs e)
        {
            type11 = Convert.ToInt32(((RadioButton)sender).Tag);
            UpdateUI();
        }

        private void radioButton15_CheckedChanged_1(object sender, EventArgs e)
        {
            type12 = Convert.ToInt32(((RadioButton)sender).Tag);
            LoadPlayers(type12 == 7);
            UpdateUI();
        }

        //Marker 1
        private void radioButton13_CheckedChanged(object sender, EventArgs e)
        {
            foul1 = Convert.ToInt32(((RadioButton)sender).Tag);
            UpdateUI();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedPlayer = (Player)comboBox1.SelectedItem;
            UpdateUI();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void UpdateUI()
        {
            button1.Enabled = selectedPlayer != null && ((selectedPlayer.IsGk && type11 == 5) || !selectedPlayer.IsGk);
            radioButton15.BackColor = foulPlayer != null && foulPlayer.IsGk && type11 == 5
                ? Color.Yellow
                : SystemColors.ButtonFace;
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //checkBox2.Enabled = checkBox1.Checked;
        }
    }
}
