using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
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
