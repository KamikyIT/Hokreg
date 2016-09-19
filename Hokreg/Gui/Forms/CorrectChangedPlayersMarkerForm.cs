using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Uniso.InStat.Classes;
using Uniso.InStat.Game;

namespace Uniso.InStat.Gui.Forms
{
    public partial class CorrectChangedPlayersMarkerForm : Form
    {
        private HockeyIce game;
        private int limit1_time = 0;
        private int limit2_time = 60 * 60000;
        private bool changed = false;
        private bool error = false;
        private Player player1_old = null;
        private Player player2_old = null;
        private Team team = null;
        private Marker mk1 = null;
        private Marker mk2 = null;

        public Marker Marker { get; set; }
        
        public CorrectChangedPlayersMarkerForm(HockeyIce game, Marker mk, Marker mk1, Marker mk2)
        {
            InitializeComponent();

            this.game = game;
            this.Marker = mk;

            OrganizeUI();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Close();
        }

        private List<Player> GetPlayersBySelectedTime()
        {
            var res = new List<Player>();

            /*List<Uniso.InStat.Marker> finePlayers;
            List<Place> finePlaces;
            Tactics t = game.GetTactics(Marker.Team1, Marker.Half, Marker.TimeVideo, out finePlayers, out finePlaces);*/

            foreach (var id in team.Tactics.Keys)
            {
                if (id == 0)
                    continue;

                res.AddRange(team.Tactics[id].GetPlayers());
            }

            return res.OrderBy(o => o.Number).ToList<Player>();
        }

        private void OrganizeUI()
        {
            lock (game.Markers)
            { 
                team = Marker.Player2 != null
                    ? Marker.Team2 : Marker.Team1;

                if (mk1 != null)
                {
                    limit1_time = mk1.TimeVideo;
                }

                if (mk2 != null)
                {
                    limit2_time = mk2.TimeVideo;
                }
                else
                    limit2_time = Marker.TimeVideo + 5 * 60000;

                linkLabel2.Enabled = game.Time > limit1_time && game.Time < limit2_time;
                linkLabel2.Text += " [" + Utils.TimeFormat(game.Time) + "]";

                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(GetPlayersBySelectedTime().ToArray<Player>());
                comboBox1.SelectedItem = Marker.Player1;
                player1_old = Marker.Player1;

                comboBox2.Enabled = Marker.Player2 != null;
                comboBox2.Items.Clear();
                comboBox2.Items.AddRange(GetPlayersBySelectedTime().ToArray<Player>());
                comboBox2.SelectedItem = Marker.Player2;
                player2_old = Marker.Player2;

                textBox1.Text = String.Format("{0}", Utils.TimeFormat(limit1_time));
                textBox2.Text = String.Format("{0}", Utils.TimeFormat(limit2_time));
                macTrackBar1.Minimum = limit1_time + 1000;
                macTrackBar1.Buffered = limit2_time - 1000;
                macTrackBar1.Maximum = limit2_time;
                macTrackBar1.Value = Marker.TimeVideo;
                
                changed = false;
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            button1.Enabled = changed && !error;
        }

        private void macTrackBar1_ValueChanged(object sender, decimal value)
        {
            Marker.TimeVideo = Convert.ToInt32(value);
            label7.Text = String.Format("{0} / {1}", Utils.TimeFormat(Marker.TimeVideo), Utils.TimeFormat(game.GetActuialTime(Marker.Half, Marker.TimeVideo)));
            CheckMarker();

            changed = true;
            UpdateUI();  
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            macTrackBar1.Value = game.Time;
            changed = true;
            UpdateUI();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            changed = true;
            Marker.Player1 = (Player)comboBox1.SelectedItem;
            CheckMarker();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            changed = true;
            Marker.Player2 = (Player)comboBox2.SelectedItem;
            CheckMarker();
        }

        private bool checking = false;

        private void CheckMarker()
        {
            if (checking)
                return;

            try
            {
                checking = true;
                error = false;

                lock (game.Markers)
                {
                    game.CheckValid(Marker);

                    label8.Visible = false;
                    label8.Text = String.Empty;
                }
            }
            catch (Exception ex)
            {
                //error = true;
                label8.Visible = true;
                label8.Text = ex.Message;
            }
            finally
            {
                checking = false;
            }

            UpdateUI();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
