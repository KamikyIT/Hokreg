using System;
using System.Windows.Forms;
using Uniso.InStat.Classes;

namespace Uniso.InStat.Gui.Forms
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
