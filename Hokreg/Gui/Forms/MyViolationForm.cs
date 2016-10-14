using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Uniso.InStat.Gui.Forms
{
    public partial class MyViolationForm : Form
    {
        private Game.Marker marker = null;
        private Player foulPlayer = null;

        private MyViolationForm()
        {
            InitializeComponent();
        }

        public MyViolationForm(Game.Marker marker, Player player): this()
        {
            this.marker = marker;
            this.foulPlayer = player;

            var hasP2 = marker.player2_id > 0;

            rb2P.Checked = hasP2;
            rb1P.Checked = !hasP2;
            rb0P.Checked = false;

            foulPlayer = player;
            checkBox1.Enabled = hasP2 && foulPlayer == marker.Player1;
            
            foulPlayersCount = FoulTypeEnum.None;

            UpdateUI();

            ResetFoulTypeChecks();
        }

        private void UpdateUI()
        {
            rb1.Enabled = foulPlayersCount != FoulTypeEnum.None;
            rb2.Enabled = foulPlayersCount == FoulTypeEnum.TwoPlayers;
            rb3.Enabled = foulPlayersCount == FoulTypeEnum.TwoPlayers;
            rb4.Enabled = foulPlayersCount == FoulTypeEnum.TwoPlayers || foulPlayersCount == FoulTypeEnum.SoloPLayer;
            rb5.Enabled = foulPlayersCount == FoulTypeEnum.TwoPlayers;
            rb6.Enabled = foulPlayersCount == FoulTypeEnum.TwoPlayers;
            rb7.Enabled = foulPlayersCount == FoulTypeEnum.TwoPlayers;
            rb8.Enabled = foulPlayersCount == FoulTypeEnum.NoPlayer;
            rb9.Enabled = foulPlayersCount == FoulTypeEnum.TwoPlayers;
            rb10.Enabled = foulPlayersCount == FoulTypeEnum.TwoPlayers;
            rb11.Enabled = foulPlayersCount == FoulTypeEnum.SoloPLayer;
            rb12.Enabled = foulPlayersCount == FoulTypeEnum.SoloPLayer || foulPlayersCount == FoulTypeEnum.NoPlayer;
            rb13.Enabled = foulPlayersCount == FoulTypeEnum.TwoPlayers;
            rb14.Enabled = foulPlayersCount == FoulTypeEnum.SoloPLayer;

            button1.Enabled = foul > 0;
        }

        private void ResetFoulTypeChecks()
        {
            this.foul = 0;

            rb1.Checked = false;
            rb2.Checked = false;
            rb3.Checked = false;
            rb4.Checked = false;
            rb5.Checked = false;
            rb6.Checked = false;
            rb7.Checked = false;
            rb8.Checked = false;
            rb9.Checked = false;
            rb10.Checked = false;
            rb11.Checked = false;
            rb12.Checked = false;
            rb13.Checked = false;
            rb14.Checked = false;
        }

        private void radioButtonFoulPlayersCount_CheckedChanged(object sender, EventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                var tag = radioButton.Tag.ToString();

                this.foulPlayersCount = (FoulTypeEnum) int.Parse(tag);

                UpdateUI();

                ResetFoulTypeChecks();
            }
        }

        private void radioButtonFoulType_CheckedChanged(object sender, EventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                var tag = radioButton.Tag.ToString();

                this.foul = int.Parse(tag);
            }
        }

        public int foul { get; set; }

        public bool IsPair { get { return this.checkBox1.Checked; } }

        public FoulTypeEnum foulPlayersCount { get; set; }

        public Game.Marker Result()
        {
            var mk = foulPlayersCount > 0?
                new Game.Marker(marker.game, 9, foul)
                {
                    Half = marker.Half,
                    TimeVideo = marker.TimeVideo,
                    Player1 = foulPlayer
                }:
                null;

            return mk;
        }

        public enum FoulTypeEnum
        {
            TwoPlayers = 2,
            SoloPLayer = 1,
            NoPlayer = 0,
            None = -1,
        }
    }


}
