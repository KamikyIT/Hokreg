using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Uniso.InStat
{
    public partial class FoulTypeForm : Form
    {
        public Game.Marker Marker { get; set; }

        public Game.Marker Result1 { get; set; }
        public Game.Marker Result2 { get; set; }

        public FoulTypeForm(Marker mk)
        {
            InitializeComponent();

            Marker = (Game.Marker)mk;

            Result1 = new Game.Marker(Marker.game, 9, 1) { Half = mk.Half, TimeVideo = mk.TimeVideo, Player1 = mk.Player1 };
            Result2 = null;

            groupBox1.Text = PlayerNameFormat(mk.Player1);
            groupBox2.Text = PlayerNameFormat(mk.Player2);
        }

        private String PlayerNameFormat(Player player)
        {
            return String.Format("[{0}] {1} {2} - {3}",
                player.Number,
                player.Name,
                player.LastName,
                player.Team.Name);
        }

        //Marker 1
        private void radioButton13_CheckedChanged(object sender, EventArgs e)
        {
            var rb = (RadioButton)sender;
            var action_type = Convert.ToInt32(rb.Tag);
            Result1 = new Game.Marker(Marker.game, 9, action_type) { Half = Marker.Half, TimeVideo = Marker.TimeVideo, Player1 = Marker.Player1 };
        }

        //Marker 2
        private void radioButton14_CheckedChanged(object sender, EventArgs e)
        {
            var rb = (RadioButton)sender;
            var action_type = Convert.ToInt32(rb.Tag);
            Result2 = action_type > 0
                    ? new Game.Marker(Marker.game, 9, action_type) { Half = Marker.Half, TimeVideo = Marker.TimeVideo, Player1 = Marker.Player2 }
                    : null;
        }
    }
}
