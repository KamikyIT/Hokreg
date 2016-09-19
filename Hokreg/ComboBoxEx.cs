using System.ComponentModel;
using System.Windows.Forms;

namespace Uniso.InStat
{
    public partial class ComboBoxEx : ComboBox
    {
        public ComboBoxEx()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.Selectable, false);

            KeyDown += myCombo_KeyDown;
            KeyUp += myCombo_KeyDown;
            KeyPress += ComboBoxEx_KeyPress;
        }

        void ComboBoxEx_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void myCombo_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        [DefaultValue(true)]
        public bool Focusable
        {
            get { return GetStyle(ControlStyles.Selectable); }
            set { SetStyle(ControlStyles.Selectable, value); }
        }
    }
}
