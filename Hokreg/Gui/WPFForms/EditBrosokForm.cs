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

namespace Uniso.InStat.Gui.WPFForms
{
    public partial class EditBrosokForm : Form
    {
        public BrosokModel CurrentModel;

        public EditBrosokForm()
        {
            InitializeComponent();

            this.elementHost1.Child = new GoalKeeperStateWpfControl();

            elementHost2.Child = new GoalKeeperBodyReceiveWpfControl();

            elementHost3.Child = new HockeyGatesWpfControl();
        }
    }

    public class BrosokModel
    {
        public TypeBrosokEnum BrosokType { get; set; }

        public GoalkeeperViewEnum GoalkeeperView { get; set; }

        public GoalKeeperStandingEnum GoalKeeperStanding { get; set; }

        public GoalFixationEnum GoalFixation { get; set; }

        public int StartFlyTime { get; set; }

        public int EndFlyTime { get; set; }

        public GateWayThrowEnum GateWayThrow { get; set; }

        public PointF PointInGates { get; set; }

        public GoalKeeperBodyEnum GoalKeeperBody { get; set; }
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
