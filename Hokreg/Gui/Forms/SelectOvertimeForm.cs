using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Uniso.InStat.Game;

namespace Uniso.InStat.Gui.Forms
{
    public partial class SelectOvertimeForm : Form
    {
        private HockeyIce game = null;
        public Half Half { get; set; }
        public int MaxPlayersNum { get; set; }
        public int OvertimeLength { get; set; }

        public SelectOvertimeForm(HockeyIce game)
        {
            InitializeComponent();
            this.game = game;

            Half = game.Half;
            MaxPlayersNum = 4;
            OvertimeLength = 5;

            if (game.Markers.Exists(o => o.Compare(18, 5) && o.Half.Index == 4))
            {
                var mkh = game.Markers.First(o => o.Compare(18, 5) && o.Half.Index == 4);
                MaxPlayersNum = mkh.Num % 10000;
                OvertimeLength = mkh.Num / 10000;
            }

            UpdateUI();
        }

        private void UpdateUI()
        {
            var ami = game.Markers.Where(o => o.Compare(18, 5)).ToList<Marker>();
            FormatButton(button1, ami);
            FormatButton(button2, ami);
            FormatButton(button3, ami);
            FormatButton(button4, ami);
            FormatButton(button5, ami);
            FormatButton(button6, ami);
            FormatButton(button7, ami);
            FormatButton(button8, ami);
            FormatButton(button9, ami);
            FormatButton(button10, ami);
            FormatButton(button11, ami);
            FormatButton(button12, ami);
            FormatButton(button13, ami);
            FormatButton(button14, ami);
            FormatButton(button15, ami);

            button16.Enabled = game.Markers.Exists(o => o.Compare(18, 5) && o.Half.Index == 4);
        }

        private void FormatButton(Button btn, List<Marker> ami)
        {
            var tag = Convert.ToInt32(btn.Tag);
            btn.BackColor = ami.Any(o => o.Half.Index == tag)
                ? Color.Yellow
                : Color.White;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var ami = game.Markers.Where(o => o.Compare(18, 5)).ToList<Marker>();
            var tag = Convert.ToInt32(((Button)sender).Tag);
            var half = game.HalfList.FirstOrDefault(o => o.Index == tag);

            if (half == null)
                return;

            //Если первая установка, то вывести 
            if (ami.Count == 0 && half.Index == 4)
            {
                var form = new OvertimeForm(MaxPlayersNum, OvertimeLength);
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    OvertimeLength = form.Length;
                    MaxPlayersNum = form.Count;
                }
            }

            Half = half;

            Close();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (game.Markers.Exists(o => o.Compare(18, 5) && o.Half.Index == 4))
            {
                var mkh = game.Markers.First(o => o.Compare(18, 5) && o.Half.Index == 4);
                var maxplayers = mkh.Num % 10000;
                var length = mkh.Num / 10000;

                var form = new OvertimeForm(maxplayers, length);
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    OvertimeLength = form.Length;
                    MaxPlayersNum = form.Count;

                    Close();
                }
            }
        }
    }
}
