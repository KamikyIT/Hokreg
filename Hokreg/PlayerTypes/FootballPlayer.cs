using System.Windows.Forms;

namespace Uniso.InStat.PlayerTypes
{
    class FootballPlayer : AxMediaPlayer.AxMediaPlayer
    {
        public event PaintEventHandler PaintOver;

        protected override void AttachInterfaces()
        {
            base.AttachInterfaces();
            
            if (GetOcx() is MediaPlayer.IMediaPlayer2)
            {
                var m = (MediaPlayer.IMediaPlayer2)GetOcx();
                m.ShowAudioControls = false;
                m.ShowPositionControls = false;
                m.ShowGotoBar = false;
                m.ShowControls = false;
            }

            //WindowlessVideo = value;
            //SetStyle(System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, value);
        }

        /*protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // WM_PAINT
            if (m.Msg == 0x0F && PaintOver != null)
            {
                
            }
        }*/
    }
}
