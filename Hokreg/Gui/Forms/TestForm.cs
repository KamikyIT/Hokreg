using System;
using System.Windows.Forms;
using Uniso.InStat.PlayerTypes;

namespace Uniso.InStat.Gui.Forms
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            streamPlayer1.PositionChanged += streamPlayer1_PositionChanged;
            streamPlayer1.MediaBuffering += streamPlayer1_MediaBuffering;
            //streamPlayer1.OpenUrl(@"d:\MOVIES\368283_1.mp4");
            streamPlayer1.OpenUrl(@"d:\Projects\cs\M3U8Sender\release\cache\4\4\raw\_out000001.mp4");
        }

        void streamPlayer1_MediaBuffering(object sender, EventArgs e)
        {
            label1.Visible = streamPlayer1.Buffering;
        }

        void streamPlayer1_PositionChanged(object sender, StreamPlayer.PositionEventArgs e)
        {
            try
            {
                trackBar1.Maximum = streamPlayer1.DurationTotal;
                trackBar1.Value = streamPlayer1.Position;
                label2.Text = TimeSpan.FromMilliseconds(streamPlayer1.Position).ToString();
            }
            catch
            { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (streamPlayer1.Mode == StreamPlayer.PlayerMode.Play)
                streamPlayer1.Mode = StreamPlayer.PlayerMode.Pause;
            else
                streamPlayer1.Mode = StreamPlayer.PlayerMode.Play;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            streamPlayer1.Position = trackBar1.Value;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            streamPlayer1.Volume = trackBar2.Value;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            //streamPlayer1.Rate = (float)trackBar3.Value / 10.0f;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
