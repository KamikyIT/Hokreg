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
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = Options.G;
        }

        private void OptionsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Options.G.Save();
        }
    }
}
