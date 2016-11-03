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

            this.elementHost1.Child = new GoalKeeperStateWpfControl();

            elementHost2.Child = new GoalKeeperBodyReceiveWpfControl();

            elementHost3.Child = new HockeyGatesWpfControl();

            video_view_model = new VideoViewModel(videoFileName, this.streamPlayer1);

            this.brosok_markers = brosokMarkers;
            
            FillMarkersListBox(brosokMarkers);

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
        shelchok,

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
        open,
        [XmlEnum("ЗАКРЫТЫЙ")]
        close,
        [XmlEnum("БРОСОК С ОБМАННЫМ ДВИЖЕНИЕМ")]
        tricky,
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum GoalKeeperStandingEnum
    {
        [XmlEnum("ВЫСОКАЯ СТОЙКА")]
        high_stand,
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
        zafiksiroval_srazy,
        [XmlEnum("ЗАФИКСИРОВАЛ С ОТСКОКА")]
        zafiksirovan_s_otskoka,
        [XmlEnum("КОНТРОЛИРУЕМЫЙ ОТСКОК")]
        kontroliryemy_otskok,
        [XmlEnum("НЕКОНТРОЛИРУЕМЫЙ ОТСКОК")]
        ne_kontroliryemy_otskok,
    }

}
