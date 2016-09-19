using System.ComponentModel;
using System.Windows.Forms;

namespace Uniso.InStat.Gui.Controls
{
    public partial class ButtonEx : Button
    {
        public ButtonEx()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.Selectable, false);
        }

        [DefaultValue(true)]
        public bool Focusable
        {
            get { return GetStyle(ControlStyles.Selectable); }
            set { SetStyle(ControlStyles.Selectable, value); }
        }
    }
}
