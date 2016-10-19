using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using Uniso.InStat.Classes;
using Uniso.InStat.Conv;
using Uniso.InStat.Models;

namespace Uniso.InStat.Game
{
    [Serializable()]
    public class Marker : Uniso.InStat.Marker
    {
        public bool flag_hitch = false;
        public bool flag_icing = false;
        public List<Marker> flag_adding = new List<Marker>();

        public int TimeActualSrv { get; set; }

        public override int TimeActual
        {
            get
            {
                if (TimeVideo == -1)
                    return TimeActualSrv;

                return base.TimeActual;
            }
            set
            {
                base.TimeActual = value;
            }
        }

        [NonSerialized()]
        public HockeyIce game = null;

        public Marker(HockeyIce game)
            : base()
        {
            this.game = game;
        }

        public Marker(HockeyIce game, String code)
            : this(game)
        {
            var tag = code.Split(new char[] { '_' });
            if (tag.Length == 2 || tag.Length == 3)
            {
                this.ActionId = Convert.ToInt32(tag[0]);
                this.ActionType = Convert.ToInt32(tag[1]);
                if (tag.Length == 3)
                {
                    this.Win = Convert.ToInt32(tag[2]);
                }
            }
        }

        public Marker(HockeyIce game, int action_id, int action_type)
            : base(action_id, action_type)
        {
            this.game = game;
        }

        public Marker(HockeyIce game, int action_id, int action_type, Half half, int video_time)
            : base(action_id, action_type, half, video_time)
        {
            this.game = game;
        }

        [Browsable(false)]
        public override bool IsSecondary
        {
            get
            {
                return Compare(2, 9) || Compare(3, 3) || Compare(4, new int[] {10, 11});
            }
        }

        [Browsable(false)]
        [NonSerialized()]
        public Uniso.InStat.Game.HockeyIce.CheckValidMarkerException Exception;

        [Browsable(false)]
        public int NumTeam1 { get; set; }

        [Browsable(false)]
        public int NumTeam2 { get; set; }

        [Browsable(false)]
        public override bool FlagSaved
        {
            get
            {
                return Id > 0;
            }
        }

        [Browsable(false)]
        public int ActionCode
        {
            get
            {
                return ActionId * 100000 + ActionType * 100 + Win;
            }
            set
            {
                ActionId = value / 100000;
                ActionType = (value / 100) % 1000;
                Win = value % 100;
            }
        }

        [Category("1. Основные")]
        [DisplayName("10. Код действия")]
        public int ActionCodeReadOnly
        {
            get
            {
                return ActionCode;
            }
        }
        
        [TypeConverter(typeof(ActionConverter))]
        [Category("1. Основные")]
        [DisplayName("1. Действие")]
        [Browsable(true)]
        public virtual ActionEnum Action
        {
            get
            {
                return (ActionEnum)(ActionId * 100000 + ActionType * 100 + Win);
            }
            set
            {
                try
                {
                    var act = (Int32)value;

                    ActionId = act / 100000;
                    ActionType = (act % 10000) / 100;
                    Win = act % 100;
                }
                catch
                { }
            }
        }

        [TypeConverter(typeof(HalfConverter))]
        [Category("1. Основные")]
        [DisplayName("2. Период")]
        [Browsable(true)]
        public override Half Half
        {
            get
            {
                return base.Half;
            }
            set
            {
                base.Half = value;
            }
        }

        [Browsable(false)]
        public int TimeVideoReal { get; set; }

        [Category("1. Основные")]
        [DisplayName("3. Время видео")]
        [Browsable(true)]
        public override int TimeVideo
        {
            get
            {
                //if (Sync == 1 && game != null)
                    //return TimeVideoReal + game.GetSync(Half);

                return TimeVideoReal;
            }
            set
            {
                /*if (Sync == 1 && game != null)
                    TimeVideoReal = value - game.GetSync(Half);
                else*/
                    TimeVideoReal = value;
            }
        }

        [TypeConverter(typeof(PlayerConverter))]
        [Category("1. Основные")]
        [DisplayName("4. Игрок")]
        [Browsable(true)]
        public override Player Player1
        {
            get
            {
                return base.Player1;
            }
            set
            {
                base.Player1 = value;
            }
        }

        [TypeConverter(typeof(PlayerConverter))]
        [Category("1. Основные")]
        [DisplayName("5. Оппонент")]
        [Browsable(true)]
        public override Player Player2
        {
            get
            {
                return base.Player2;
            }
            set
            {
                base.Player2 = value;
            }
        }

        [TypeConverter(typeof(WinConverter))]
        [Category("1. Основные")]
        [DisplayName("6. Точность")]
        [Browsable(true)]
        public override int Win
        {
            get
            {
                return base.Win;
            }
            set
            {
                base.Win = value;
            }
        }

        [Category("1. Основные")]
        [DisplayName("7. Номер")]
        [Browsable(true)]
        public override int Num
        {
            get
            {
                return base.Num;
            }
            set
            {
                base.Num = value;
            }
        }

        [Category("2. Площадка")]
        [DisplayName("1. Точка действия X")]
        public float Point1_X
        {
            get
            {
                return Point1.X;
            }
            set
            {
                Point1 = new PointF(value, Point1.Y);
            }
        }

        [Category("2. Площадка")]
        [DisplayName("1. Точка действия Y")]
        public float Point1_Y
        {
            get
            {
                return Point1.Y;
            }
            set
            {
                Point1 = new PointF(Point1.X, value);
            }
        }

        [Category("2. Площадка")]
        [DisplayName("2. Точка назначения X")]
        public float Point2_X
        {
            get
            {
                return Point2.X;
            }
            set
            {
                Point2 = new PointF(value, Point2.Y);
            }
        }

        [Category("2. Площадка")]
        [DisplayName("2. Точка назначения Y")]
        public float Point2_Y
        {
            get
            {
                return Point2.Y;
            }
            set
            {
                Point2 = new PointF(Point2.X, value);
            }
        }

        [NonSerialized()]
        public System.Windows.Forms.ListViewItem row = null;

        public string GetNameStage(MarkersWomboCombo.FoulStageEnum stage)
        {
            switch (stage)
            {
                case MarkersWomboCombo.FoulStageEnum.None:
                    break;
                case MarkersWomboCombo.FoulStageEnum.Player0:
                    break;
                case MarkersWomboCombo.FoulStageEnum.Player1:
                    return GetNameStage(StageEnum.Player1);
                case MarkersWomboCombo.FoulStageEnum.Player2:
                    return GetNameStage(StageEnum.Player2);
                case MarkersWomboCombo.FoulStageEnum.Point0:
                    break;
                case MarkersWomboCombo.FoulStageEnum.Point1:
                    return GetNameStage(StageEnum.Point);
                default:
                    throw new ArgumentOutOfRangeException(nameof(stage), stage, null);
            }

            return "";
        }

        public String GetNameStage(StageEnum stage)
        {
            if (Compare(1, 6) && Win == 2)
            {
                if (stage == StageEnum.Player2)
                    return "КТО ПЕРЕХВАТИЛ ?";

                if (stage == StageEnum.PointAndDest)
                    return "ТОЧКА ДЕЙСТВИЯ И ГДЕ СОВЕРШЕН ПЕРЕХВАТ ?";
            }

            switch (stage)
            {
                case StageEnum.Player1:
                    return "ИГРОК";

                case StageEnum.Player2:
                    return "ОППОНЕНТ";

                case StageEnum.Point:
                    return "ТОЧКА";

                case StageEnum.Player2Gk:
                    return "ГОЛКИПЕР";

                case StageEnum.PointAndDest:
                    return "ТОЧКА ДЕЙСТВИЯ И НАЗНАЧЕНИЯ";
            }

            if (stage == StageEnum.ExtraOptions && ActionId == 12 && Num == 0)
                return "Выбор атакующих и защищающихся";

            if (stage == StageEnum.ExtraOptions && Compare(8, 1))
                return "Выбор ассистентов";

            if (stage == StageEnum.ExtraOptions && Compare(1, 1))
                return "Выбор выигравшего вбрасывание";

            if (stage == StageEnum.ExtraOptions && Compare(6, 2))
                return "Выбор игроков";

            return String.Empty;
        }

        public override string ToString()
        {
            var ss = TimeVideo / 1000L;
            var m = ss / 60L;
            var s = ss % 60L;
            var mss = (TimeVideo % 1000L) / 100;
            var tv = String.Format("{0}:{1}.{2}", m.ToString("00"), s.ToString("00"), mss.ToString("0"));

            ss = TimeActual / 1000L;
            m = ss / 60L;
            s = ss % 60L;
            mss = (TimeActual % 1000L) / 100;
            var ta = String.Format("{0}:{1}.{2}", m.ToString("00"), s.ToString("00"), mss.ToString("0"));

            try
            {
                return String.Format("{10}[ID={0}] {1}-{2}-{12} {3} T={4}/{5} P1={6} {7} P2={8} {9} U={11}",
                    Id,
                    ActionId,
                    ActionType,
                    Half != null ? Half.ToString() : "HALF NOT DEF",
                    tv,
                    ta,
                    Player1 != null ? Player1.ToString() : String.Empty,
                    Team1 != null ? Team1.ToString() : String.Empty,
                    Player2 != null ? Player2.ToString() : String.Empty,
                    Team2 != null ? Team2.ToString() : String.Empty,
                    FlagDel ? "[DEL] " : "",
                    user_id,
                    Win);
            }
            catch
            { }

            return "";
        }

        public bool Compare(int action_id, int action_type, int win)
        {
            return this.Compare(action_id, action_type) && this.Win == win;
        }

        public static void CopyMarkerData(Game.Marker copyFrom, out Game.Marker newMarker)
        {
            newMarker = new Marker(copyFrom.game)
            {
                user_id = copyFrom.user_id,
                flag_adding = copyFrom.flag_adding,
                Half = copyFrom.Half,
                ActionCode = copyFrom.ActionCode,
                ActionType = copyFrom.ActionType,
                Win = copyFrom.Win,
                TimeVideo = copyFrom.TimeVideo,
                TimeVideoReal = copyFrom.TimeVideoReal,
                TimeActual = copyFrom.TimeActual,
                TimeActualSrv = copyFrom.TimeActualSrv,
                TimeActualTotal = copyFrom.TimeActualTotal,
                Player1 =copyFrom.Player1,
                Player2 =  copyFrom.Player2,
                Point1 = copyFrom.Point1,
                Point2 = copyFrom.Point2,
                Num  = copyFrom.Num,
                NumTeam1 = copyFrom.NumTeam1,
                NumTeam2 = copyFrom.NumTeam2,
            };
        }
    }
}
