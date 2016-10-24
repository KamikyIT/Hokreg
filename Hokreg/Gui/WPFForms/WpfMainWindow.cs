using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Uniso.InStat.Gui.WPF_Forms
{
    public partial class WpfMainWindow : Form
    {
        public WpfMainWindow()
        {
            InitializeComponent();
        }

        private void WpfMainWindow_Resize(object sender, EventArgs e)
        {
            this.fullWPFelement.Size = this.Size;
        }
    }
}
