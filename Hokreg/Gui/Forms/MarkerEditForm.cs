using System;
using System.Windows.Forms;

namespace Uniso.InStat.Gui.Forms
{
    public partial class MarkerEditForm : Form
    {
        public MarkerEditForm()
        {
            InitializeComponent();
        }

        public void Edit(Marker mk)
        {
            Uniso.Log.Write("Редактирование маркера BEGIN\n" + mk.ToString());
            propertyGrid1.SelectedObject = mk;
        }

        private void MarkerEditForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Uniso.Log.Write("Редактирование маркера END\n" + propertyGrid1.SelectedObject.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
