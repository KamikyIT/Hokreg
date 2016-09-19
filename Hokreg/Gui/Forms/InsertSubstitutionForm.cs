using System;
using System.Linq;
using System.Windows.Forms;
using Uniso.InStat.Game;

namespace Uniso.InStat.Gui.Forms
{
    public partial class InsertSubstitutionForm : Form
    {
        private HockeyIce game;

        public Player Player1 { get; set; }
        public Player Player2 { get; set; }

        public InsertSubstitutionForm(HockeyIce game)
        {
            InitializeComponent();
            this.game = game;

            var exists = game.Match.Team1.Tactics[0].GetPlayers().OrderBy(o => o.Number)
                .Concat(game.Match.Team2.Tactics[0].GetPlayers().OrderBy(o => o.Number)).ToList<Player>();

            var exists_no = game.Match.Team1.Players.OrderBy(o => o.Number)
                .Concat(game.Match.Team2.Players.OrderBy(o => o.Number)).Where(o => exists.IndexOf(o) < 0).ToList<Player>();

            comboBox2.Items.Add("-НЕ ВЫБРАНО-");
            comboBox2.Items.AddRange(exists.ToArray<Player>());
            comboBox2.SelectedIndex = 0;

            comboBox1.Items.Add("-НЕ ВЫБРАНО-");
            comboBox1.Items.AddRange(exists_no.ToArray<Player>());
            comboBox1.SelectedIndex = 0;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Close();
        }

        private void UpdateUI()
        {
            var error = false;
            if (Player1 != null && Player2 != null)
            {
                error = Player1.Team.Id != Player2.Team.Id;
                if (error)
                {
                    label8.Visible = true;
                    label8.Text = "Выбраны игроки из разных команд";
                }
                else
                {
                    label8.Visible = false;
                    label8.Text = "";
                }
            }

            button1.Enabled = Player2 != null && !error;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem is Player)
                Player2 = comboBox2.SelectedItem as Player;
            else
                Player2 = null;

            UpdateUI();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem is Player)
                Player1 = comboBox1.SelectedItem as Player;
            else
                Player1 = null;

            UpdateUI();
        }
    }
}
