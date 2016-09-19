using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Uniso.InStat.Gui
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
