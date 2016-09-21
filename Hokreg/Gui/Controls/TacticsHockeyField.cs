using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Uniso.InStat.Game;

namespace Uniso.InStat.Gui.Controls
{
    public partial class TacticsHockeyField : UserControl
    {
        #region Private Fields

        private int _tacticsCount;
        
        #endregion

        [Browsable(true)]
        public HockeyIce Game { get; set; }

        [Browsable(true)]
        [Category("Дополнительные свойства")]
        [Description(@"Количество тактических групп в командной тактике.")]
        [DisplayName(@"Количество тактик")]
        // ReSharper disable once ConvertToAutoProperty
        public int TacticsCount
        {
            get { return _tacticsCount; }
            set
            {
                if (_tacticsCount == value)
                {
                    return;
                }

                _tacticsCount = value;

            }
        }
        


        public TacticsHockeyField()
        {
            InitializeComponent();

            lblSelectedPlayer.Text = String.Empty;
            
            
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), ClientRectangle);


        }

        private void lblSelectedPlayer_TextChanged(object sender, EventArgs e)
        {
            lblSelectedPlayer.Visible = !string.IsNullOrEmpty(lblSelectedPlayer.Text);
        }
    }


    public class TacticsHockeyFieldModel
    {
        #region Private fields

        private uint _tacticsCount;
        private TeamTacticsViewModel _teamOne;
        private TeamTacticsViewModel _teamTwo;
        private uint _goalKeepersCount;

        #endregion

        /// <summary>
        /// Количество пятероок игроков
        /// </summary>
        public uint TacticsCount
        {
            get { return _tacticsCount; }
            set { _tacticsCount = value; }
        }

        /// <summary>
        /// Коилчество вратарей
        /// </summary>
        public uint GoalKeepersCount
        {
            get { return _goalKeepersCount; }
            set
            {
                _goalKeepersCount = value;
            }
        }

        /// <summary>
        /// Первая команда.
        /// </summary>
        public TeamTacticsViewModel TeamOne
        {
            get { return _teamOne; }
            set
            {
                _teamOne = value;
            }
        }

        /// <summary>
        /// Вторая команда.
        /// </summary>
        public TeamTacticsViewModel TeamTwo
        {
            get { return _teamTwo; }
            set { _teamTwo = value; }
        }
    }

    
    [Serializable()]
    public class TeamTacticsViewModel
    {
        public Color WearColor { get; set; }
        


        
    }


}
