using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Uniso.InStat.Annotations;
using Uniso.InStat.PlayerTypes;

namespace Uniso.InStat.Gui.WPFForms
{
    public partial class EditBrosokForm : Form
    {
        

        public EditBrosokForm(string videoFileName, List<Game.Marker> brosokMarkers)
        {
            InitializeComponent();

            goalKeeperStateWpfControl = new GoalKeeperStateWpfControl();
            this.elementHost1.Child = goalKeeperStateWpfControl;

            goalKeeperBodyReceiveWpfControl = new GoalKeeperBodyReceiveWpfControl();
            elementHost2.Child = goalKeeperBodyReceiveWpfControl;

            hockeyGatesWpfControl = new HockeyGatesWpfControl();
            elementHost3.Child = hockeyGatesWpfControl;

            video_view_model = new VideoViewModel(videoFileName, this.streamPlayer1);

            this.brosok_markers = brosokMarkers;
            
            FillMarkersListBox(brosokMarkers);

            StartSetTagsAndFillControlsDict();

        }


        private Dictionary<object, Button> allButtonControls = new Dictionary<object, Button>();

        

        private void StartSetTagsAndFillControlsDict()
        {
            this.btnBrosokType1.Tag = (TypeBrosokEnum) (1);
            this.btnBrosokType2.Tag = (TypeBrosokEnum) (2);
            this.btnBrosokType3.Tag = (TypeBrosokEnum) (3);
            this.btnBrosokType4.Tag = (TypeBrosokEnum) (4);
            this.btnBrosokType5.Tag = (TypeBrosokEnum) (5);
            this.btnBrosokType6.Tag = (TypeBrosokEnum) (6);

            this.btnGoalkeeperView1.Tag = (GoalkeeperViewEnum) (1);
            this.btnGoalkeeperView2.Tag = (GoalkeeperViewEnum) (2);
            this.btnGoalkeeperView3.Tag = (GoalkeeperViewEnum) (3);

            this.btnGoalKeeperStanding1.Tag = (GoalKeeperStandingEnum) (1);
            this.btnGoalKeeperStanding2.Tag = (GoalKeeperStandingEnum) (2);
            this.btnGoalKeeperStanding3.Tag = (GoalKeeperStandingEnum) (3);
            this.btnGoalKeeperStanding4.Tag = (GoalKeeperStandingEnum) (4);

            this.btnGoalFixation1.Tag = (GoalFixationEnum) (1);
            this.btnGoalFixation2.Tag = (GoalFixationEnum) (2);
            this.btnGoalFixation3.Tag = (GoalFixationEnum) (3);
            this.btnGoalFixation4.Tag = (GoalFixationEnum) (4);

            allButtonControls.Add(btnBrosokType1.Tag, btnBrosokType1);
            allButtonControls.Add(btnBrosokType2.Tag, btnBrosokType2);
            allButtonControls.Add(btnBrosokType3.Tag, btnBrosokType3);
            allButtonControls.Add(btnBrosokType4.Tag, btnBrosokType4);
            allButtonControls.Add(btnBrosokType5.Tag, btnBrosokType5);
            allButtonControls.Add(btnBrosokType6.Tag, btnBrosokType6);

            allButtonControls.Add(btnGoalkeeperView1.Tag, btnGoalkeeperView1);
            allButtonControls.Add(btnGoalkeeperView2.Tag, btnGoalkeeperView2);
            allButtonControls.Add(btnGoalkeeperView3.Tag, btnGoalkeeperView3);
            
            allButtonControls.Add(btnGoalKeeperStanding1.Tag, btnGoalKeeperStanding1);
            allButtonControls.Add(btnGoalKeeperStanding2.Tag, btnGoalKeeperStanding2);
            allButtonControls.Add(btnGoalKeeperStanding3.Tag, btnGoalKeeperStanding3);
            allButtonControls.Add(btnGoalKeeperStanding4.Tag, btnGoalKeeperStanding4);

            allButtonControls.Add(btnGoalFixation1.Tag, btnGoalFixation1);
            allButtonControls.Add(btnGoalFixation2.Tag, btnGoalFixation2);
            allButtonControls.Add(btnGoalFixation3.Tag, btnGoalFixation3);
            allButtonControls.Add(btnGoalFixation4.Tag, btnGoalFixation4);

        }

        private void FillMarkersListBox(List<Game.Marker> brosokMarkers)
        {
            this.markersListBox.Items.Clear();

            if (brosokMarkers == null)
            {
                return;
            }

            brosokMarkers.ForEach(m => markersListBox.Items.Add(new MarkerForListView(m)));
        }

        private BrosokModel current_model
        {
            get { return _currentModel; }
            set
            {
                if (value == null)
                {
                    _currentModel = null;
                    return;
                }

                if (_currentModel == value)
                {
                    return;
                }

                _currentModel = value;

                DisplaySelectedCurrentModel();
            }
        }

        private void DisplaySelectedCurrentModel()
        {
            ResetDisplayProperties();

            if (this.current_model == null)
            {
                return;
            }


            if (this.current_model.BrosokType.HasValue)
            {
                if (allButtonControls.ContainsKey(this.current_model.BrosokType.Value))
                {
                    SetSelectedButtonColor(allButtonControls[this.current_model.BrosokType.Value], true);
                }
            }

            if (this.current_model.GoalkeeperView.HasValue)
            {
                if (allButtonControls.ContainsKey(this.current_model.GoalkeeperView.Value))
                {
                    SetSelectedButtonColor(allButtonControls[this.current_model.GoalkeeperView.Value], true);
                }
            }

            if (this.current_model.GoalKeeperStanding.HasValue)
            {
                if (allButtonControls.ContainsKey(this.current_model.GoalKeeperStanding.Value))
                {
                    SetSelectedButtonColor(allButtonControls[this.current_model.GoalKeeperStanding.Value], true);
                }
            }

            if (this.current_model.GoalFixation.HasValue)
            {
                if (allButtonControls.ContainsKey(this.current_model.GoalFixation.Value))
                {
                    SetSelectedButtonColor(allButtonControls[this.current_model.GoalFixation.Value], true);
                }
            }

            if (this.current_model.StartFlyTime.HasValue)
            {
                this.lblMomentBroska.Text = this.current_model.StartFlyTime.Value.ToString();
            }

            if (this.current_model.EndFlyTime.HasValue)
            {
                this.lblMomentBroska.Text = this.current_model.EndFlyTime.Value.ToString();
            }

            if (this.current_model.GateWayThrow.HasValue)
            {
                this.goalKeeperStateWpfControl.ForseSetValue(this.current_model.GateWayThrow.Value);
            }

            if (this.current_model.GoalKeeperBody.HasValue)
            {
                this.goalKeeperBodyReceiveWpfControl.ForseSetValue(this.current_model.GoalKeeperBody.Value);
            }

            if (this.current_model.PointInGates.HasValue)
            {
                this.hockeyGatesWpfControl.ForseSetValue(this.current_model.PointInGates.Value);
            }
        }

        private void ResetDisplayProperties()
        {
            SetSelectedButtonColor(btnBrosokType1, false);
            SetSelectedButtonColor(btnBrosokType2, false);
            SetSelectedButtonColor(btnBrosokType3, false);
            SetSelectedButtonColor(btnBrosokType4, false);
            SetSelectedButtonColor(btnBrosokType5, false);
            SetSelectedButtonColor(btnBrosokType6, false);

            SetSelectedButtonColor(btnGoalkeeperView1, false);
            SetSelectedButtonColor(btnGoalkeeperView2, false);
            SetSelectedButtonColor(btnGoalkeeperView3, false);

            SetSelectedButtonColor(btnGoalKeeperStanding1, false);
            SetSelectedButtonColor(btnGoalKeeperStanding2, false);
            SetSelectedButtonColor(btnGoalKeeperStanding3, false);
            SetSelectedButtonColor(btnGoalKeeperStanding4, false);

            SetSelectedButtonColor(btnGoalFixation1, false);
            SetSelectedButtonColor(btnGoalFixation2, false);
            SetSelectedButtonColor(btnGoalFixation3, false);
            SetSelectedButtonColor(btnGoalFixation4, false);

            goalKeeperStateWpfControl.ResetDisplay();
            goalKeeperBodyReceiveWpfControl.ResetDisplay();
            hockeyGatesWpfControl.ResetDisplay();

            lblMomentBroska.Text = @"00:00.0";
            lblMomentOtskoka.Text = @"00:00.0";
        }

        private void SetSelectedButtonColor(Button btn, bool selected)
        {
            btn.BackColor = selected ? Color.LightSkyBlue : SystemColors.Control;
        }

        private List<Game.Marker> brosok_markers;

        private VideoViewModel video_view_model;
        private BrosokModel _currentModel;

        private class MarkerForListView
        {
            public MarkerForListView(Game.Marker marker)
            {
                this.marker = marker;
            }

            public int TimeVideo => this.marker.TimeVideo;

            public Game.Marker marker;

            public override string ToString()
            {
                return string.Format("{0} / {1}", marker.TimeActual, marker.TimeVideo);
            }
        }

        private class VideoViewModel
        {
            private PlayerTypes.StreamPlayer streamPlayer1;

            public string videoFileName { get; private set; }

            public float speed { get; set; }

            public float volume { get; set; }

            public VideoViewModel(string videoUri, PlayerTypes.StreamPlayer streamPlayer1)
            {
                this.videoFileName = videoUri;

                this.streamPlayer1 = streamPlayer1;

                this.streamPlayer1.Start(this.videoFileName);
            }
        }

        private void markersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.current_model = (BrosokModel)markersListBox.Items[markersListBox.SelectedIndex];
        }

        private void markersListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            // https://msdn.microsoft.com/ru-ru/library/system.windows.forms.listbox.drawmode(v=vs.110).aspx

            e.DrawBackground();

            var myBrush = Brushes.Black;

            switch (e.Index)
            {
                case 0:
                    myBrush = Brushes.Red;
                    break;
                case 1:
                    myBrush = Brushes.Orange;
                    break;
                case 3:
                    myBrush = Brushes.Purple;
                    break;
            }

            e.Graphics.DrawString(markersListBox.Items[e.Index].ToString(), e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);

            e.DrawFocusRectangle();            
        }


        private GoalKeeperStateWpfControl goalKeeperStateWpfControl;
        private GoalKeeperBodyReceiveWpfControl goalKeeperBodyReceiveWpfControl;
        private HockeyGatesWpfControl hockeyGatesWpfControl;
    }

    public class BrosokModel
    {
        public TypeBrosokEnum? BrosokType { get; set; }

        public GoalkeeperViewEnum? GoalkeeperView { get; set; }

        public GoalKeeperStandingEnum? GoalKeeperStanding { get; set; }

        public GoalFixationEnum? GoalFixation { get; set; }

        public int? StartFlyTime { get; set; }

        public int? EndFlyTime { get; set; }

        public GateWayThrowEnum? GateWayThrow { get; set; }

        public GoalKeeperBodyEnum? GoalKeeperBody { get; set; }

        

        public PointF? PointInGates { get; set; }


        public bool CheckValuesAreSet()
        {
            return
                BrosokType.HasValue && GoalkeeperView.HasValue && GoalKeeperStanding.HasValue &&
                GoalFixation.HasValue && StartFlyTime.HasValue && EndFlyTime.HasValue && GateWayThrow.HasValue &&
                PointInGates.HasValue;
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum TypeBrosokEnum
    {
        [XmlEnum("ЩЕЛЧОК")]
        shelchok = 1,

        [XmlEnum("ЩЕЛЧОК С РАЗМАХОМ")]
        shelchok_s_razmaxom,

        [XmlEnum("КИСТЕВОЙ")]
        kistevoy,

        [XmlEnum("С НЕУДОБНОЙ РУКИ")]
        s_neydobnoy_ryki,

        [XmlEnum("ПОДСТАВЛЕННАЯ КЛЮШКА")]
        podstavlennaya_klyshka,

        [XmlEnum("БРОСОК С БЛИЗКОЙ ДИСТАНЦИИ(ДОБИВАНИЕ)")]
        brosok_s_blizkoy_distancii,
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum GoalkeeperViewEnum
    {
        [XmlEnum("ОТКРЫТЫЙ")]
        open = 1,
        [XmlEnum("ЗАКРЫТЫЙ")]
        close,
        [XmlEnum("БРОСОК С ОБМАННЫМ ДВИЖЕНИЕМ")]
        tricky,
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum GoalKeeperStandingEnum
    {
        [XmlEnum("ВЫСОКАЯ СТОЙКА")]
        high_stand = 1,
        [XmlEnum("НИЗКАЯ СТОЙКА")]
        low_stand,
        [XmlEnum("В СПЛИТЕ")]
        in_split,
        [XmlEnum("ЛЕЖИТ")]
        laying,
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum GoalFixationEnum
    {
        [XmlEnum("ЗАФИКСИРОВАЛ СРАЗУ")]
        zafiksiroval_srazy = 1,
        [XmlEnum("ЗАФИКСИРОВАЛ С ОТСКОКА")]
        zafiksirovan_s_otskoka,
        [XmlEnum("КОНТРОЛИРУЕМЫЙ ОТСКОК")]
        kontroliryemy_otskok,
        [XmlEnum("НЕКОНТРОЛИРУЕМЫЙ ОТСКОК")]
        ne_kontroliryemy_otskok,
    }

}
