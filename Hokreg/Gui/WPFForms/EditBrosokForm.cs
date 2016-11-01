using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Uniso.InStat.Gui.WPFForms
{
    public partial class EditBrosokForm : Form
    {
        public EditBrosokForm()
        {
            InitializeComponent();

            this.elementHost1.Child = new GoalKeeperStateWpfControl();

            elementHost2.Child = new GoalKeeperBodyReceiveWpfControl();
        }
    }
}
