using System.Windows.Forms;

namespace Uniso.InStat.Gui.Controls
{
    public partial class TransparentPanel : Panel
    {
        public TransparentPanel()
        {
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var createParams = base.CreateParams;
                createParams.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
                return createParams;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }
    }
}
