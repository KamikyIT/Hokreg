using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Uniso.InStat.Gui.Forms
{
    public partial class CheckForm : Form
    {
        public CheckForm(List<String> msg)
        {
            InitializeComponent();
            textBox1.Lines = msg.ToArray<String>();
        }
    }
}
