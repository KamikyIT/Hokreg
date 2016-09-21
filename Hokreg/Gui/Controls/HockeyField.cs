using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Gdi;
using ShellApi;
using Uniso.InStat.Classes;
using Uniso.InStat.Game;

namespace Uniso.InStat.Gui.Controls
{
    public partial class HockeyField : UserControl
    {
        public GDICompatible gdi = null;
        public HockeyIce Game { get; set; }
        public bool IsShowSpares { get; set; }

        public event EventHandler PaintCompatible;
        public Point omPoint = Point.Empty;
        private Place omPlace = null;
        private Amplua omAmplua = null;
        private Tactics omTactics = null;
        private Team omTeam = null;
        public PointF point = PointF.Empty;

        public int PlaceSize 
        {
            get
            {
                var coef = HockeyGui.Mode == HockeyGui.ModeEnum.EditTactics ? 5 : 4;
                return ((Height - 20) / coef);
            }
        }      
        
        public event EventHandler ChangedPlayers;
        public event HockeyGui.ChangedPlaceEventHandler ChangedPlace;
        public event HockeyGui.SelectedPlayerEventHandler SelectedPlayer;
        public event HockeyGui.RestorePlayerEventHandler RestorePlayer;
        public event HockeyGui.SelectedPointEventHandler SelectedPoint;
        public event HockeyGui.SelectedPointAndDestEventHandler SelectedPointAndDest;
        public event HockeyGui.SelectedManyPlayerEventHandler SelectedManyPlayer;

        public HockeyField()
        {
            InitializeComponent();

            IsShowSpares = true;

            gdi = new GDICompatible(Handle.ToInt32());
            gdi.InitContext();

            Paint += new System.Windows.Forms.PaintEventHandler(DoPaint);
            SizeChanged += new System.EventHandler(DoSizeChanged);
            MouseMove += DoMouseMove;
            MouseUp += HockeyField_MouseUp;
            MouseDown += HockeyField_MouseDown;
            MouseLeave += HockeyField_MouseLeave;
        }

        void HockeyField_MouseLeave(object sender, EventArgs e)
        {
            if (gdi != null)
                gdi.InvalidateRect();
        }

        private void DoMouseMove(object sender, MouseEventArgs e)
        {
            if (Game.Match == null)
                return;

            omPoint = e.Location;

            omTeam = SeekTeam(omPoint);
            omTactics = SeekTactics(omTeam, omPoint);
            omAmplua = SeekAmplua(omPoint);
            omPlace = SeekPlace(omPoint);

            gdi.InvalidateRect();
        }

        private Team SeekTeam(Point pt)
        {
            float dh, dw;
            var rcf = GetFieldRect(out dw, out dh);
            if (pt.X > rcf.Left + rcf.Width / 2)
                return Game.Match.Team2;

            return Game.Match.Team1;
        }

        private Tactics SeekTactics(Uniso.InStat.Team tm, Point pt)
        {
            if (tm == null || tm.Tactics == null)
                return null;

            for (var i = 0; i < tm.Tactics.Count; i++)
            {
                if (HockeyGui.Mode == HockeyGui.ModeEnum.EditTactics && i == 0)
                    continue;

                var t = tm.Tactics[i];

                if (GetRect(tm, t).Contains(pt))
                    return t;
            }

            return null;
        }

        private Amplua SeekAmplua(Point pt)
        {
            var amplua = SeekAmplua((Uniso.InStat.Game.Team)Game.Match.Team1, pt);
            if (amplua != null)
                return amplua;

            return SeekAmplua((Uniso.InStat.Game.Team)Game.Match.Team2, pt);
        }

        private Amplua SeekAmplua(Uniso.InStat.Game.Team tm, Point pt)
        {
            if (tm.Tactics == null || tm.Tactics.Count == 0)
                return null;

            for (var i = 0; i < tm.Tactics.Count; i++)
            {
                if (HockeyGui.Mode == HockeyGui.ModeEnum.EditTactics && i == 0)
                    continue;

                var t = tm.Tactics[i];
                foreach (var a in Game.MapA)
                    if (GetRect(tm, t, a).Contains(pt))
                        return a;
            }

            return null;
        }

        private Place SeekPlace(Point pt)
        {
            var place = SeekPlace((Uniso.InStat.Game.Team)Game.Match.Team1, pt);
            if (place != null)
                return place;

            return SeekPlace((Uniso.InStat.Game.Team)Game.Match.Team2, pt);
        }

        private Place SeekPlace(Uniso.InStat.Game.Team tm, Point pt)
        {
            if (tm.Tactics == null || tm.Tactics.Count == 0)
                return null;

            if (HockeyGui.Mode == HockeyGui.ModeEnum.EditTactics)
            {
                for (var i = 1; i < tm.Tactics.Count; i++)
                {
                    var t = tm.Tactics[i];

                    foreach (var p in t.Places)
                    {
                        var rci = GetRect(tm, t, p);
                        if (rci.Contains(pt))
                            return p;
                    }
                }
            }
            else
            {
                var t = tm.Tactics[0];

                foreach (var p in t.Places)
                {
                    var rci = GetRect(tm, t, p);
                    if (rci.Contains(pt))
                        return p;
                }
            }

            return null;
        }

        private Rectangle GetRect(Team tm, Tactics t)
        {
            float dh, dw;
            var rcf = GetFieldRect(out dw, out dh);

            var index = Game.Match.Team2 == tm ? 1 : 0;

            var dist = 70;
            var res = new Rectangle(0, (int)rcf.Top, 13 + PlaceSize, (int)rcf.Height);


            for (var i = 1; i < tm.Tactics.Count; i++)
            {
                if (tm.Tactics[i] == t)
                {
                    if (index == 0)
                        res.Offset((int)rcf.Left + (int)rcf.Width / 2 - dist * i, 0);
                    else
                        res.Offset((int)rcf.Left + (int)rcf.Width / 2 + (dist * i) - (dist / 4 * 3), 0);

                    return res;
                }
            }

            //Основная тактика
            if (tm.Tactics[0] == t)
            {
                if (index == 0)
                    return new Rectangle((int)rcf.Left, (int)rcf.Top, (int)rcf.Width / 2, (int)rcf.Height);

                return new Rectangle((int)(rcf.Left + rcf.Width / 2), (int)rcf.Top, (int)rcf.Width / 2, (int)rcf.Height);
            }

            return Rectangle.Empty;
        }

        private Rectangle GetRect(Team tm, Tactics t, Place p)
        {
            var res = _GetRect(tm, t, p);
            if (!res.IsEmpty)
            {
                if (tm.Tactics[0] == t && HockeyGui.Mode != HockeyGui.ModeEnum.EditTactics)
                {
                    var res1 = new Rectangle(res.Left + res.Width / 2 - PlaceSize / 2 + 1, res.Top + res.Height / 2 - PlaceSize / 2, PlaceSize, PlaceSize);
                    return res1;
                }

                if (Game.Match.Team2 == tm)
                    return new Rectangle(res.Left, res.Top + res.Height / 2 - PlaceSize / 2, PlaceSize, PlaceSize);

                return new Rectangle(res.Left + res.Width - PlaceSize, res.Top + res.Height / 2 - PlaceSize / 2, PlaceSize, PlaceSize);
            }

            return Rectangle.Empty;
        }

        private Rectangle _GetRect(Team tm, Tactics t, Place p)
        {
            float dh, dw;
            var rcf = GetFieldRect(out dw, out dh);

            var index = Game.Match.Team2 == tm ? 1 : 0;

            //Основная тактика
            if (tm.Tactics[0] == t)
            {
                Rectangle rct;
                if (index == 0)
                {
                    rct = new Rectangle((int)rcf.Left, (int)rcf.Top, (int)rcf.Width / 2, (int)rcf.Height);
                    var dx = rct.Width / Game.MapA.Count;
                    var dy = rct.Height / Game.MapP.Count;
                    var res1 = new Rectangle(rct.Left + (p.Amplua.Id - 1) * dx, rct.Top + (p.Position.Id - 1) * dy, dx, dy);
                    if (p.Amplua.Id == 2)
                    {
                        var offset = (dy / 3) * (p.Position.Id == 1 ? 1 : -1);
                        res1.Offset(0, offset);
                    }
                    if (p.Amplua.Id == 1 && p.Player != null && !p.Player.IsGk)
                        res1.Offset(dx, 0);

                    return res1;
                }
                else
                {
                    rct = new Rectangle((int)(rcf.Left + rcf.Width / 2), (int)rcf.Top, (int)rcf.Width / 2, (int)rcf.Height);
                    var dx = rct.Width / Game.MapA.Count;
                    var dy = rct.Height / Game.MapP.Count;
                    var res2 = new Rectangle(rct.Left + (3 - p.Amplua.Id) * dx, rct.Top + (3 - p.Position.Id) * dy, dx, dy);
                    if (p.Amplua.Id == 2)
                    {
                        var offset = (dy / 3) * (p.Position.Id == 1 ? -1 : 1);
                        res2.Offset(0, offset);
                    }
                    if (p.Amplua.Id == 1 && p.Player != null && !p.Player.IsGk)
                        res2.Offset(-dx, 0);
                    return res2;
                }
            }

            var rcti = GetRect(tm, t);
            var h = rcti.Height / 6;

            var ih = t.Places.Count - t.Places.IndexOf(p) - 1;
            var xh = 0;
            if (p.Amplua.Id == 1)
            {
                xh = 10;
            }

            if (index == 0)
            {
                if (p.Amplua.Id == 3)
                {
                    ih = p.Position.Id - 1;
                }
                if (p.Amplua.Id == 2)
                {
                    ih = 2 + (p.Position.Id == 1 ? 1 : 2);
                }
            }

            return new Rectangle(rcti.Left, rcti.Top + h * ih + xh, rcti.Width, h);
        }

        private Rectangle GetRect(Team tm, Tactics t, Amplua a)
        {
            float dh, dw;
            var rcf = GetFieldRect(out dw, out dh);

            var index = Game.Match.Team2 == tm ? 1 : 0;
            Rectangle rct;

            //Основная тактика
            if (tm.Tactics[0] == t)
            {
                if (index == 0)
                {
                    rct = new Rectangle((int)rcf.Left, (int)rcf.Top, (int)rcf.Width / 2, (int)rcf.Height);
                    var dx = rct.Width / Game.MapA.Count;
                    var dy = rct.Height / Game.MapP.Count;
                    return new Rectangle(rct.Left + (a.Id - 1) * dx, rct.Top, dx, rct.Height);
                }
                else
                {
                    rct = new Rectangle((int)(rcf.Left + rcf.Width / 2), (int)rcf.Top, (int)rcf.Width / 2, (int)rcf.Height);
                    var dx = rct.Width / Game.MapA.Count;
                    var dy = rct.Height / Game.MapP.Count;
                    return new Rectangle(rct.Left + (3 - a.Id) * dx, rct.Top, dx, rct.Height);
                }
            }

            rct = GetRect(tm, t);
            var w = rct.Width;

            if (index == 0)
            {
                if (a.Id == HockeyIce.AMPLUA_F.Id)
                    rct = new Rectangle(rct.Left, rct.Top, rct.Width - PlaceSize, rct.Height / 2 - 3);

                if (a.Id == HockeyIce.AMPLUA_Z.Id)
                    rct = new Rectangle(rct.Left, rct.Top + rct.Height / 2, rct.Width - PlaceSize, rct.Height / 3 - 3);

                if (a.Id == HockeyIce.AMPLUA_GK.Id)
                    rct = new Rectangle(rct.Left, rct.Top + rct.Height / 6 * 5, rct.Width - PlaceSize, rct.Height / 6 - 3);
            }
            else
            {
                if (a.Id == HockeyIce.AMPLUA_F.Id)
                    rct = new Rectangle(rct.Left + PlaceSize, rct.Top, rct.Width - PlaceSize, rct.Height / 2 - 3);

                if (a.Id == HockeyIce.AMPLUA_Z.Id)
                    rct = new Rectangle(rct.Left + PlaceSize, rct.Top + rct.Height / 2, rct.Width - PlaceSize, rct.Height / 3 - 3);

                if (a.Id == HockeyIce.AMPLUA_GK.Id)
                    rct = new Rectangle(rct.Left + PlaceSize, rct.Top + rct.Height / 6 * 5, rct.Width - PlaceSize, rct.Height / 6 - 3);
            }

            return new Rectangle(rct.Left, rct.Top + PlaceSize / 2, rct.Width, rct.Height - PlaceSize);
        }

        private RectangleF GetFieldRect(out float dw, out float dh)
        {
            var dv = HockeyGui.Mode == HockeyGui.ModeEnum.SelectPoint || HockeyGui.Mode == HockeyGui.ModeEnum.SelectPointAndDest
                ? 30 : 10;

            dh = Height - dv;

            if (IsShowSpares)
            {
                dw = Game.FieldSize.Width * dh / Game.FieldSize.Height;
            }
            else
            {
                dw = Width - dv;
            }

            return new RectangleF((Width - dw) / 2.0f, dv / 2, dw, dh);
        }

        // TODO: 9	Тактическая схема	Указывать точку на тактической расстановке, обозначающую координаты последнего зарегистрированного маркера "Х"	Не выполнено
        private void DoPaint(object sender, PaintEventArgs e)
        {
            try
            {
                
                gdi.BeforePaint();

                if (Game == null)
                    return;

                //Field
                float dh, dw;
                var rcf = GetFieldRect(out dw, out dh);

                var dx = rcf.Width / Game.FieldSize.Width;
                var dy = rcf.Height / Game.FieldSize.Height;
                var c = new Point(Convert.ToInt32(rcf.X + rcf.Width / 2.0f), Convert.ToInt32(rcf.Y + rcf.Height / 2.0f));
                var rci = new Rectangle(
                    Convert.ToInt32(rcf.Left),
                    Convert.ToInt32(rcf.Top),
                    Convert.ToInt32(rcf.Width),
                    Convert.ToInt32(rcf.Height));

                if (Game.Match != null)
                {
                    var mirror = false;
                    if (HockeyGui.Marker != null)
                    {
                        if (HockeyGui.Mode == HockeyGui.ModeEnum.SelectPointAndDest)
                        {
                            mirror = point.IsEmpty ? HockeyGui.Marker.Team1 == Game.Match.Team2 : HockeyGui.Marker.Team2 == Game.Match.Team2;
                        }
                        else
                        {
                            mirror = HockeyGui.Marker.Team1 == Game.Match.Team2;
                        }
                    }

                    var ptf = Game.TransformScreenToBase(rcf, omPoint, Game.FieldSize, HockeyGui.Marker, mirror);

                    if (HockeyGui.Mode == HockeyGui.ModeEnum.SelectPoint || HockeyGui.Mode == HockeyGui.ModeEnum.SelectPointAndDest)
                    {
                        gdi.Pen.Width = 1;
                        gdi.Pen.Color = 0x00000000;
                        gdi.Brush.Style = BrushStyle.bsClear;
                        gdi.Rectangle(ClientRectangle);
                        gdi.Brush.Style = BrushStyle.bsSolid;

                        gdi.Pen.Width = 1;
                        gdi.Brush.Style = Gdi.BrushStyle.bsSolid;

                        gdi.Pen.Color = 0x00000000;
                        gdi.Brush.Color = 0x00ffffff;
                        gdi.RoundRect(rci, Convert.ToInt32(15f * dx), Convert.ToInt32(15f * dx));

                        gdi.Pen.Color = 0x000000ff;
                        gdi.Pen.Width = 3;

                        //Центральная линия
                        gdi.MoveTo(c.X, rci.Y);
                        gdi.LineTo(c.X, rci.Bottom);

                        Game.DrawField(gdi, rcf);

                        gdi.Font.Name = "Tahoma";
                        gdi.Font.Size = 9;
                        gdi.Font.Color = 0x00000000;
                        gdi.SetBkMode(GDIBkModes.TRANSPARENT);
                        var zone = Game.GetZone(ptf, false);
                        gdi.TextOut((int)rcf.X, 1, String.Format("X={0} Y={1} Zone={2}", ptf.X.ToString("0.00"), ptf.Y.ToString("0.00"), zone));
                        gdi.SetBkMode(GDIBkModes.BKMODE_LAST);

                        gdi.Pen.Color = 0x0000ff00;
                        var vvv = Game.TransformBaseToScreen(rcf, ptf, Game.FieldSize, mirror);

                        if (HockeyGui.Mode == HockeyGui.ModeEnum.SelectPointAndDest && !point.IsEmpty)
                        {
                            mirror = HockeyGui.Marker.Team1 == Game.Match.Team2;
                            var vvv2 = Game.TransformBaseToScreen(rcf, point, Game.FieldSize, mirror);
                            gdi.MoveTo((int)vvv.X, (int)vvv.Y);
                            gdi.LineTo((int)vvv2.X, (int)vvv2.Y);
                        }

                        gdi.MoveTo((int)rcf.X, (int)vvv.Y);
                        gdi.LineTo((int)rcf.Right, (int)vvv.Y);
                        gdi.MoveTo((int)vvv.X, (int)rcf.Y);
                        gdi.LineTo((int)vvv.X, (int)rcf.Bottom);
                    }
                    else
                    {
                        var ctrl = SystemColors.Control;
                        gdi.Pen.Style = PenStyle.psClear;
                        gdi.Brush.Color = 0x00f0f0f0;
                        gdi.Rectangle(new Rectangle(0, 0, Width + 1, Height + 1));
                        gdi.Pen.Style = PenStyle.psSolid;
                    }

                    var primaryShow = HockeyGui.Mode != HockeyGui.ModeEnum.SelectPoint
                        && HockeyGui.Mode != HockeyGui.ModeEnum.SelectPointAndDest
                        && HockeyGui.Mode != HockeyGui.ModeEnum.EditTactics;

                    DrawTactics(gdi, Game.Match.Team1, primaryShow, IsShowSpares);
                    DrawTactics(gdi, Game.Match.Team2, primaryShow, IsShowSpares);
                }
                else
                {
                    gdi.Brush.Color = 0x00eeeeee;
                    gdi.Pen.Style = PenStyle.psClear;

                    var rc = ClientRectangle;
                    rc = new Rectangle(rc.Left, rc.Top, rc.Width + 1, rc.Height + 1);

                    gdi.Rectangle(rc);

                    gdi.Font.Name = "Segoe UI Semibold";
                    gdi.Font.Size = -22;
                    gdi.Font.Color = 0x00808080;
                    gdi.DrawTextCenter("ЗАГРУЗИТЕ ДАННЫЕ ПО МАТЧУ", rc);
                }

                if (PaintCompatible != null)
                    PaintCompatible(sender, e);
            }
            catch
            { }
            finally
            {
                gdi.AfterPaint();
            }
        }

        public bool IsTacticsStartValid
        {
            get
            {
                return Game != null && Game.Match != null
                    && Game.Match.Team1 != null && Game.Match.Team2 != null
                    && Game.Match.Team1.Tactics.Count > 0 && Game.Match.Team2.Tactics.Count > 0
                    && Game.Match.Team1.Tactics[1].IsValid && Game.Match.Team2.Tactics[1].IsValid;
            }
        }

        public void ClearMousePosition()
        {
            omPoint = Point.Empty;
            omAmplua = null;
            omPlace = null;
            omTactics = null;
            omTeam = null;
            gdi.InvalidateRect();
        }

        public void RefreshRgn()
        {
            if (HockeyIce.Role == HockeyIce.RoleEnum.AdvTtd)
            {
                Region = null;
                return;
            }

            if (Game.Match == null || Game.Match.Team1 == null || Game.Match.Team2 == null)
                return;

            if (HockeyGui.Mode == HockeyGui.ModeEnum.SelectPlayer || HockeyGui.Mode == HockeyGui.ModeEnum.SelectManyPlayers || HockeyGui.Mode == HockeyGui.ModeEnum.View)
            {
                var path =
                    new System.Drawing.Drawing2D.GraphicsPath(System.Drawing.Drawing2D.FillMode.Winding);

                foreach (var place in Game.Match.Team1.Tactics[0].Places)
                {
                    lock (Game.Match.Team1.FinePlaces)
                        if (Game.Match.Team1.FinePlaces.Exists(o => o.Compare(place)))
                            continue;

                    var rc = GetRect(Game.Match.Team1, Game.Match.Team1.Tactics[0], place);
                    rc = new Rectangle(rc.Left + 3, rc.Top + 3, rc.Width - 7, rc.Height - 7);
                    path.AddEllipse(rc);
                }

                foreach (var place in Game.Match.Team2.Tactics[0].Places)
                {
                    lock (Game.Match.Team2.FinePlaces)
                        if (Game.Match.Team2.FinePlaces.Exists(o => o.Compare(place)))
                            continue;

                    var rc = GetRect(Game.Match.Team2, Game.Match.Team2.Tactics[0], place);
                    rc = new Rectangle(rc.Left + 3, rc.Top + 3, rc.Width - 7, rc.Height - 7);
                    path.AddEllipse(rc);
                }

                Region = new Region(path);
            }
            else
                Region = null;
        }

        private void DrawTactics(GDICompatible gdi, Team tm, bool primaryShow, bool spareShow)
        {
            float dw, dh;
            var rcf = GetFieldRect(out dw, out dh);
            
            var index = Game.Match.Team2 == tm ? 1 : 0;
            for (var i = 0; i < tm.Tactics.Count; i++)
            {
                if (!primaryShow && i == 0)
                    continue;

                if (!spareShow && i > 0)
                    continue;

                var t = tm.Tactics[i];

                if (i > 0)
                {
                    gdi.Brush.Style = BrushStyle.bsClear;

                    foreach (var a in Game.MapA)
                    {
                        if (a.Id == 1)
                            continue;

                        var select = false;// a == omAmplua && omPlace == null && omTactics != null && omTactics.NameActionType == t.NameActionType && HockeyGui.Mode != HockeyGui.ModeEnum.SelectPoint;

                        gdi.Pen.Color = select ? 0x000000ff : 0x00000000;
                        gdi.Pen.Width = select ? 4 : 2;

                        var rca = GetRect(tm, t, a);
                        rca = new Rectangle(rca.Left, rca.Top, rca.Width, rca.Height);

                        if (index == 0)
                        {
                            gdi.MoveTo(rca.Right + 3, rca.Top);
                            gdi.LineTo(rca.Left, rca.Top);
                            gdi.LineTo(rca.Left, rca.Bottom);
                            gdi.LineTo(rca.Right + 3, rca.Bottom);
                        }
                        else
                        {
                            gdi.MoveTo(rca.Left - 3, rca.Top);
                            gdi.LineTo(rca.Right, rca.Top);
                            gdi.LineTo(rca.Right, rca.Bottom);
                            gdi.LineTo(rca.Left - 3, rca.Bottom);
                        }
                        

                        // TODO: Вот тут вопрос чзётм
                        if (omTactics != null && omTactics.NameActionType == t.NameActionType && omPlace == null && omAmplua == null && false)
                        {
                            var rct = GetRect(omTeam, omTactics);

                            gdi.Pen.Color = 0x000000ff;// HockeyGui.TransformColor(tm.Color.SelfColor1);
                            gdi.Pen.Width = 4;

                            if (index == 0)
                            {
                                rct = new Rectangle(rct.Left, rct.Top + PlaceSize / 2, rct.Width - PlaceSize, rct.Height - PlaceSize * 2);
                                gdi.MoveTo(rct.Right + 3, rct.Top);
                                gdi.LineTo(rct.Left, rct.Top);
                                gdi.LineTo(rct.Left, rct.Bottom);
                                gdi.LineTo(rct.Right + 3, rct.Bottom);
                            }
                            else
                            {
                                rct = new Rectangle(rct.Left + PlaceSize, rct.Top + PlaceSize / 2, rct.Width - PlaceSize, rct.Height - PlaceSize * 2);
                                gdi.MoveTo(rct.Left - 3, rct.Top);
                                gdi.LineTo(rct.Right, rct.Top);
                                gdi.LineTo(rct.Right, rct.Bottom);
                                gdi.LineTo(rct.Left - 3, rct.Bottom);
                            }
                        }
                    }
                    gdi.Brush.Style = BrushStyle.bsSolid;
                }

                gdi.Pen.Width = 1;
                gdi.Pen.Color = 0x00000000;

                foreach (var place in t.Places)
                {
                    if (Game.Half != null && Game.Half.Index >= Game.HalfList[3].Index && Game.Half.Index < 255 && place.GetCode() == 23 && Game.Half.MaxPlayersNum == 5)
                        continue;

                    var selectedPlace = (place.Compare(omPlace) && omTeam == tm && omTactics != null && omTactics.NameActionType == t.NameActionType && HockeyGui.Mode != HockeyGui.ModeEnum.SelectPoint);
                    var selectedAll = (place.Amplua == omAmplua && omTeam == tm && omPlace == null && omTactics != null && omTactics.NameActionType == t.NameActionType && HockeyGui.Mode != HockeyGui.ModeEnum.SelectPoint);

                    var rc = GetRect(tm, t, place);

                    if (spareShow)
                        rc = new Rectangle(rc.Left + 5, rc.Top + 5, rc.Width - 10, rc.Height - 10);

                    gdi.Pen.Style = PenStyle.psSolid;
                    gdi.Pen.Width = 2;
                    gdi.Pen.Color = i >= 0 ? HockeyGui.TransformColor(tm.Color.SelfColor1) : 0x00000000;
                    gdi.Brush.Color = HockeyGui.TransformColor(tm.Color.SelfColor1);

                    if (selectedPlace)
                        gdi.Pen.Color = 0x000080ff;

                    if (place.Player != null 
                        && HockeyGui.ChangedPlayersList.Exists(
                            o => (place.Compare(o.Place1) && t.NameActionType == o.Tactics1.NameActionType) 
                            || place.Compare(o.Place2) && t.NameActionType == o.Tactics2.NameActionType))
                        gdi.Pen.Color = 0x00000000;

                    if (place.Player != null && HockeyGui.Marker != null && (HockeyGui.Marker.Player1 == place.Player || HockeyGui.Marker.Player2 == place.Player))
                        gdi.Pen.Color = 0x0000ff00;

                    if (place.Player != null && HockeyGui.selectedManyPlayers.IndexOf(place.Player) >= 0)
                        gdi.Pen.Color = 0x0000ffff;

                    var block_place = false;

                    lock (tm.FinePlaces)
                    lock (tm.FinePlayers)
                    {
                        if (tm.FinePlaces.Exists(o => o.Compare(place)) || (place.Player != null && tm.FinePlayers.Exists(o => o.Player1 == place.Player && ((bool)o.Tag))))
                        {
                            block_place = true;
                        }
                    }

                    if (!block_place && HockeyGui.Marker != null)
                    {
                        block_place =
                            //Вбрасывание - блокирование ГК
                            (HockeyGui.Marker.Compare(1, 1) && place.Amplua.Id == 1)
                            //Вбрасывание - блокирование своей команды
                            || (HockeyGui.Marker.Compare(1, 1) && HockeyGui.Marker.Player1 != null && (HockeyGui.Marker.Team1 == tm && HockeyGui.Marker.Player1 != place.Player))
                            //Бросок заблок - блоктрование своей команды
                            || (HockeyGui.Marker.Compare(4, 3) && HockeyGui.Marker.Player1 != null && (HockeyGui.Marker.Team1 == tm && HockeyGui.Marker.Player1 != place.Player))
                            //Фиксация шайбы вратарем - блокирование полевого
                            || (HockeyGui.Marker.Compare(3, 6) && place.Amplua.Id > 1)
                            //Фиксация шайбы игроком - блокирование ГК
                            || (HockeyGui.Marker.Compare(3, 7) && place.Amplua.Id == 1)
                            //Гол асистенты - блокирование команды соперника
                            || (HockeyGui.Marker.Compare(8, 1) && HockeyGui.Marker.Team1 != null && HockeyGui.Marker.Team1 != tm)
                            //Буллит - команда нарушителя
                            || (HockeyGui.Marker.Compare(4, 6) && HockeyGui.Marker.Team1 == tm)
                            //Сэйв - вратари
                            || (HockeyGui.Marker.Compare(7, 1) && (place.Player == null || (place.Player != null && !place.Player.IsGk)));

                        if (!block_place && HockeyGui.Marker.Compare(2, 9))
                        {
                            if (place.Player != null && place.Player.IsGk)
                                block_place = true;
                            else
                            {
                                var sibl = Game.GetSiblings(HockeyGui.Marker);
                                if (sibl.Count > 0 && sibl[0].Player1 != null)
                                {
                                    var mki = sibl[0];
                                    block_place = sibl[0].Player1.Team == tm;
                                }
                            }
                        }
                    }

                    if (block_place)
                    { 
                        gdi.Brush.Color = 0x00b0b0b0;
                        gdi.Pen.Color = 0x00808080;
                    }

                    var rc222 = rc;
                    if (HockeyGui.Mode != HockeyGui.ModeEnum.EditTactics) 
                        rc222 = new Rectangle(rc.Left + 5, rc.Top + 5, rc.Width - 10, rc.Height - 10);

                    gdi.Ellipse(rc222);
                    
                    var p = place.Player;
                    var place_changed = false;
                    if (HockeyGui.ChangedPlayersList.Exists(o => place.Compare(o.Place2)))
                    {
                        p = HockeyGui.ChangedPlayersList.First(o => place.Compare(o.Place2)).Place1.Player;
                        place_changed = true;

                        gdi.Pen.Color = 0x0000ff00;
                        gdi.Brush.Style = BrushStyle.bsClear;
                        gdi.Ellipse(rc);
                        gdi.Brush.Style = BrushStyle.bsSolid;
                        gdi.Pen.Color = 0x00000000;
                    }

                    gdi.Pen.Width = 1;
                    gdi.Pen.Color = 0x00000000;

                    lock (tm.FinePlaces)
                    lock (tm.FinePlayers)
                    {
                        if (!place_changed  
                            && HockeyGui.ChangedPlayersList.Exists(o => o.Place1 != null && o.Place2 == null && o.Place1.Player != null && o.Place1.Player.Team == omTeam)
                            && place.Compare(omPlace) && i == 0
                            && !tm.FinePlaces.Exists(o => o.Compare(omPlace)))
                        {
                            var cpp = HockeyGui.ChangedPlayersList.First(o => o.Place1 != null && o.Place2 == null && o.Place1.Player != null && o.Place1.Player.Team == omTeam);
                            if (cpp.Place1.Player != null && cpp.Place1.Player.Team == tm && !HockeyGui.IsPlaying(cpp.Place1.Player)
                                && ((cpp.Place1.Amplua.Id == 1 && place.Amplua.Id == 1) || cpp.Place1.Amplua.Id > 1))
                                p = cpp.Place1.Player;
                        }
                    }

                    if (p != null)
                    {
                        gdi.Font.Color = HockeyGui.FontColor(tm.Color.NumberColor);
                        gdi.Font.Name = "Arial";
                        if (rc.Width > 22)
                            gdi.Font.Size = 8 + (rc.Width - 22) / 2;
                        else
                            gdi.Font.Size = 8;

                        var num = p.Number.ToString();

                        if (num.Length >= 3)
                            gdi.Font.Size -= 4;

                        SIZE sz;
                        gdi.GetTextExtent(num, out sz);

                        var rct1 = new Rectangle(rc.Left + (rc.Width - sz.cx) / 2,
                                rc.Top + (rc.Height - sz.cy) / 2,
                                sz.cx, sz.cy);

                        gdi.SetBkMode(Gdi.GDIBkModes.TRANSPARENT);
                        gdi.TextOut(rct1.X, rct1.Y, num);

                        if (selectedPlace && p != null)
                        {
                            gdi.Font.Color = selectedPlace || selectedAll ? 0x00000000 : 0x00808080;
                            gdi.Font.Size = 8;

                            if (spareShow)
                                gdi.TextOut(rc.X, 1, p.ToString());
                            else
                                gdi.TextOut(10, 1, p.ToString());
                        }

                        gdi.SetBkMode(Gdi.GDIBkModes.BKMODE_LAST);
                    }
                }
            }

            if (IsShowSpares)
            {
                var w = Convert.ToInt32(rcf.Width / 2.0f);
                var h = Convert.ToInt32(rcf.Top + 5.0f * rcf.Height / 6.0f) + 3;
                gdi.Pen.Style = PenStyle.psSolid;
                gdi.Pen.Color = 0x00ff0000;

                if (Game.Match.Team1 == tm)
                {
                    gdi.MoveTo((int)rcf.Left - 5 + w, h);
                    gdi.LineTo((int)rcf.Left - 5 - 4 * 60 + w, h);
                }
                else
                {
                    gdi.MoveTo((int)rcf.Right + 5 - w, h);
                    gdi.LineTo((int)rcf.Right + 5 + 4 * 60 - w, h);
                }
            }
        }
        
        public void DoSizeChanged(object sender, EventArgs e)
        {
            gdi.UpdateContext();
        }

        private Place miPlace = null;

        private void contextMenuStrip1_SelectPlayer(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem)sender;
            if (mi.Tag is Player && miPlace != null && omTeam != null)
            {
                var player = (Player)mi.Tag;
                for (var i = 0; i < omTeam.Tactics.Count; i++)
                {
                    var t = omTeam.Tactics[i];
                    foreach (var p in t.Places)
                    {
                        if (p.Player != null && p.Player.Id == player.Id)
                        {
                            p.Player = null;
                        }
                    }
                }

                miPlace.Player = player;

                if (ChangedPlace != null)
                    ChangedPlace(this, new HockeyGui.ChangedPlaceEventArgs { Team = omTeam, Tactics = omTactics, Place = miPlace });

                gdi.InvalidateRect();
            }
        }

        private void HockeyField_MouseDown(object sender, MouseEventArgs e)
        {
            if (Keyboard.IsKeyDown(Keys.ShiftKey) && HockeyGui.ChangedPlayersList.Count == 0)
            {
                ShellApi.User32.ReleaseCapture();
                ShellApi.User32.SendMessage(this.Handle.ToInt32(), (int)ShellApi.Msgs.WM_SYSCOMMAND, 0xf012, 0);
            }

            if (placeChangeNumber != null)
            {
                CheckEnterNumber();
                placeChangeNumber = null;
                textBox1.Visible = false;
            }

            contextMenuStrip1.Items.Clear();

            if (Game.Match == null)
                return;

            if (e.Button == System.Windows.Forms.MouseButtons.Right
                && HockeyGui.Mode == HockeyGui.ModeEnum.EditTactics
                && omTactics != null && omTeam != null
                && omTactics.NameActionType > 0)
            {
                if (omTeam != null && omPlace != null)
                {
                    miPlace = omPlace;
                    foreach (var p in omTeam.Players.Where(o => (o.IsGk && omPlace.Amplua.Id == 1) || (!o.IsGk && omPlace.Amplua.Id > 1)).OrderBy(o => o.Number))
                    {
                        if (omPlace.Player == p)
                            continue;

                        var mi = new ToolStripMenuItem(p.ToString(), null, contextMenuStrip1_SelectPlayer) { Tag = p };
                        contextMenuStrip1.Items.Add(mi);
                    }
                }
            }
        }

        // TODO: 9	Тактическая схема	Координату точки определять в if-е, потом сохранять.
        private void HockeyField_MouseUp(object sender, MouseEventArgs e)
        {
            if (HockeyGui.Mode == HockeyGui.ModeEnum.EditTactics)
                return;

            if (Game.Match == null)
                return;

            if (e.Button == System.Windows.Forms.MouseButtons.Left && HockeyGui.Mode == HockeyGui.ModeEnum.View)
            {
                if (omPlace != null && omTactics != null && omTeam != null)
                {
                    if (omTeam.FinePlaces.Exists(o => o.Compare(omPlace)))
                        return;

                    if (omPlace.Player != null && omTeam.FinePlayers.Exists(o => o.Id == omPlace.Player.Id && ((bool)o.Tag)))
                        return;

                    if (omTeam.FinePlayers.Exists(o => omPlace.Player != null && o.Id == omPlace.Player.Id && ((bool)o.Tag)))
                        return;

                    if (HockeyGui.ChangedPlayersList.Count > 0
                        && HockeyGui.ChangedPlayersList[0].Place1.Player != null
                        && HockeyGui.ChangedPlayersList[0].Place1.Player.Team != omTeam)
                        return;

                    if (omTactics.NameActionType == 0 && HockeyGui.ChangedPlayersList.Exists(o => o.Place2 == null))
                    {
                        var cpp = HockeyGui.ChangedPlayersList.First(o => o.Place2 == null);
                        if (cpp.Place1 != null
                            && (/*(cpp.Place1.Amplua.Id == 1 && omPlace.Amplua.Id > 1) || */(HockeyGui.IsPlaying(cpp.Place1.Player))))
                            return;

                        cpp.Place2 = omPlace;
                        cpp.Tactics2 = omTactics;
                        gdi.InvalidateRect();
                    }
                    else
                    {
                        if (HockeyGui.ChangedPlayersList.Exists(o => o.Place1.Compare(omPlace)))
                        {
                            var cpp = HockeyGui.ChangedPlayersList.First(o => o.Place1.Compare(omPlace));
                            HockeyGui.ChangedPlayersList.Remove(cpp);
                        }
                        else
                        {
                            if (omTactics.NameActionType == 0)
                                return;
                            
                            HockeyGui.ChangedPlayersList.Add(new HockeyGui.ChangedPlayersPair { Place1 = omPlace, Tactics1 = omTactics });
                        }

                        gdi.InvalidateRect();
                    }
                }

                if (omPlace == null)
                {
                    HockeyGui.ChangedPlayersList.Clear();
                }

                if (ChangedPlayers != null)
                    ChangedPlayers(this, EventArgs.Empty);

                RefreshRgn();
            }

            if (HockeyGui.Marker != null)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left && HockeyGui.Mode == HockeyGui.ModeEnum.SelectPoint)
                {
                    float dw, dh;
                    var rcf = GetFieldRect(out dw, out dh);
                    var mirror = HockeyGui.Marker.Team1 != null && Game.Match.Team2 == HockeyGui.Marker.Team1;
                    var ptf = Game.TransformScreenToBase(rcf, omPoint, Game.FieldSize, HockeyGui.Marker, mirror);
                    if (SelectedPoint != null)
                        SelectedPoint(this, new HockeyGui.SelectedPointEventArgs { Point = ptf });

                    return;
                }

                if (e.Button == System.Windows.Forms.MouseButtons.Left && HockeyGui.Mode == HockeyGui.ModeEnum.SelectPointAndDest && HockeyGui.Marker.Team1 != null)
                {
                    float dw, dh;
                    var rcf = GetFieldRect(out dw, out dh);

                    if (point.IsEmpty)
                    {
                        point = Game.TransformScreenToBase(rcf, omPoint, Game.FieldSize, HockeyGui.Marker, Game.Match.Team2 == HockeyGui.Marker.Team1);
                    }
                    else
                    {
                        var ptf = Game.TransformScreenToBase(rcf, omPoint, Game.FieldSize, HockeyGui.Marker, Game.Match.Team2 == HockeyGui.Marker.Team1);

                        if (SelectedPointAndDest != null)
                            SelectedPointAndDest(this, new HockeyGui.SelectedPointAndDestEventArgs { Point1 = point, Point2 = ptf });
                    }

                    return;
                }

                if (e.Button == System.Windows.Forms.MouseButtons.Left
                    && HockeyGui.Mode == HockeyGui.ModeEnum.SelectManyPlayers
                    && omPlace != null && omPlace.Player != null
                    && HockeyGui.Marker.ActionId == 12)
                {
                    if (HockeyGui.selectedManyPlayers.IndexOf(omPlace.Player) >= 0)
                        HockeyGui.selectedManyPlayers.Remove(omPlace.Player);
                    else
                    {
                        if (omPlace.Player.IsGk)
                            return;

                        //Определяем команды атака - оборона
                        if (HockeyGui.selectedManyPlayers.Count == 0)
                        {
                            HockeyGui.Marker.Team1 = omPlace.Player.Team;
                            HockeyGui.Marker.Team2 = Game.Match.Opponent(omPlace.Player.Team);
                        }

                        var count = HockeyGui.selectedManyPlayers.Count(o => o.Team == omPlace.Player.Team);

                        //Если в команде больше 3 игроков - отмена
                        if (HockeyGui.Marker.Team1 == omPlace.Player.Team && count >= 3)
                            return;

                        if (HockeyGui.Marker.Team2 == omPlace.Player.Team && count >= 3)
                            return;

                        HockeyGui.selectedManyPlayers.Add(omPlace.Player);
                    }

                    gdi.InvalidateRect();
                    return;
                }

                if (e.Button == System.Windows.Forms.MouseButtons.Left
                    && HockeyGui.Mode == HockeyGui.ModeEnum.SelectManyPlayers 
                    && omPlace != null && omPlace.Player != null
                    && HockeyGui.Marker.Compare(8, 1)
                    && HockeyGui.Marker.Team1 != null
                    && omTeam == HockeyGui.Marker.Team1)
                {
                    if (HockeyGui.Marker.Team1 != omTeam)
                        return;

                    if (HockeyGui.selectedManyPlayers.IndexOf(omPlace.Player) < 0)
                        HockeyGui.selectedManyPlayers.Add(omPlace.Player);
                    else
                        HockeyGui.selectedManyPlayers.Remove(omPlace.Player);

                    gdi.InvalidateRect();

                    if (HockeyGui.selectedManyPlayers.Count >= 2)
                        CompleteEnter();

                    return;
                }

                if (e.Button == System.Windows.Forms.MouseButtons.Left
                    && HockeyGui.Mode == HockeyGui.ModeEnum.SelectManyPlayers
                    && omPlace != null && omPlace.Player != null
                    && HockeyGui.Marker.Compare(6, 2))
                {
                    if (HockeyGui.selectedManyPlayers.IndexOf(omPlace.Player) < 0)
                        HockeyGui.selectedManyPlayers.Add(omPlace.Player);
                    else
                        HockeyGui.selectedManyPlayers.Remove(omPlace.Player);

                    gdi.InvalidateRect();
                    return;
                }

                if (e.Button == System.Windows.Forms.MouseButtons.Left
                    && HockeyGui.Mode == HockeyGui.ModeEnum.SelectManyPlayers
                    && omPlace != null && omPlace.Player != null
                    && HockeyGui.Marker.Compare(2, 11))
                {
                    if (HockeyGui.selectedManyPlayers.IndexOf(omPlace.Player) < 0)
                        HockeyGui.selectedManyPlayers.Add(omPlace.Player);
                    else
                        HockeyGui.selectedManyPlayers.Remove(omPlace.Player);

                    gdi.InvalidateRect();
                    return;
                }

                if (e.Button == System.Windows.Forms.MouseButtons.Left && HockeyGui.Mode == HockeyGui.ModeEnum.SelectPlayer && omPlace != null && omPlace.Player != null)
                {
                    var fine = omTeam != null && omTactics.NameActionType == 0;
                    if (HockeyGui.Marker.ActionId == 5)
                        fine = ((HockeyGui.Marker.Player1 == null && omTactics.NameActionType == 0) 
                            || (HockeyGui.Marker.Player1 != null && omTactics.NameActionType > 0));

                    //Вбрасывание - блокирование ГК
                    if ((HockeyGui.Marker.Compare(1, 1) && omPlace.Amplua.Id == 1)
                    //Фиксация шайбы вратарем - блокирование полевого
                    || (HockeyGui.Marker.Compare(3, 6) && omPlace.Amplua.Id > 1)
                    //Фиксация шайбы игроком - блокирование ГК
                    || (HockeyGui.Marker.Compare(3, 7) && omPlace.Amplua.Id == 1)
                    //Гол асистенты - блокирование команды соперника
                    || (HockeyGui.Marker.Compare(8, 1) && HockeyGui.Marker.Team1 != null && HockeyGui.Marker.Team1 != omTeam)
                    //Буллит - команда нарушителя
                    || (HockeyGui.Marker.Compare(4, 6) && HockeyGui.Marker.Team1 == omTeam)
                            //Сэйв - вратари
                    || (HockeyGui.Marker.Compare(7, 1) && (omPlace.Player == null || (omPlace.Player != null && !omPlace.Player.IsGk))))
                        return;

                    if (fine)
                    {
                        if (SelectedPlayer != null)
                            SelectedPlayer(this, new HockeyGui.SelectedPlayerEventArgs { Player = omPlace.Player, Place = omPlace, Tactics = omTactics });

                        return;
                    }
                }
            }
        }

        public bool Visible
        {
            get { return base.Visible; }
            set
            {
                base.Visible = value;
                if (value)
                    RefreshRgn();
            }
        }

        private Place placeChangeNumber = null;

        private Rectangle GetRectNumber(Place place)
        {
            var rcp = GetRect(omTeam, omTactics, place);

            var ps = rcp.Height / 5 * 4;
            var offps = (rcp.Height - ps) / 2;
            var rcc = new Rectangle(rcp.Left + offps, rcp.Top + offps, ps, ps);
            if (place.Player == null)
            {
                rcc.Offset((rcp.Width - ps) / 2 - offps, 0);
            }
            return rcc;
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            CheckEnterNumber();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                textBox1.Visible = false;
                placeChangeNumber = null;
            }

            if (e.KeyCode == Keys.Enter)
                CheckEnterNumber();
        }

        private void CheckEnterNumber()
        {
            if (!textBox1.Visible)
                return;

            textBox1.Visible = false;
            var num = 0;
            if (placeChangeNumber != null && placeChangeNumber.Player != null
                && Int32.TryParse(textBox1.Text.Trim(), out num))
            {
                if (num >= 1 && num <= 999)
                {
                    placeChangeNumber.Player.Number = num;
                    if (ChangedPlace != null)
                        ChangedPlace(this, new HockeyGui.ChangedPlaceEventArgs { Place = placeChangeNumber, Tactics = omTactics, Team = omTeam });
                }
            }
        }

        private void HockeyField_DoubleClick(object sender, EventArgs e)
        {
            if (HockeyGui.Mode == HockeyGui.ModeEnum.EditTactics)
            {
                placeChangeNumber = omPlace;
                if (placeChangeNumber != null && placeChangeNumber.Player != null)
                {
                    var rc = GetRectNumber(placeChangeNumber);
                    textBox1.Location = rc.Location;
                    textBox1.Size = rc.Size;
                    textBox1.Visible = true;
                    textBox1.Text = placeChangeNumber.Player.Number.ToString();
                }
                return;
            }

            if (omTactics != null && omTeam != null && omTactics.NameActionType > 0)
            {
                lock (omTeam.FinePlayers)
                    lock (omTeam.FinePlaces)
                {
                    //Замена одного
                    if (omTactics != null && omPlace != null)
                    {
                        try
                        {
                            if (omTeam.FinePlaces.Exists(o => o.Compare(omPlace)))
                                return;

                            if (omPlace.Player != null && omTeam.FinePlayers.Exists(o => o.Player1 == omPlace.Player && ((bool)o.Tag)))
                                return;

                            if (omTeam.Tactics[0].Places.Exists(o => o.Amplua == omPlace.Amplua && o.Position == omPlace.Position))
                            {
                                var p = omTeam.Tactics[0].Places.First(o => o.Amplua == omPlace.Amplua && o.Position == omPlace.Position);

                                if (omTeam.FinePlaces.Exists(o => p.Compare(o)))
                                    return;

                                HockeyGui.ChangedPlayersList.Add(new HockeyGui.ChangedPlayersPair { Place1 = omPlace, Place2 = p });
                            }

                            gdi.InvalidateRect();

                            if (ChangedPlayers != null)
                                ChangedPlayers(this, EventArgs.Empty);
                        }
                        catch
                        { }
                    }

                    if (omAmplua != null && omPlace == null)
                    {
                        try
                        {
                            var places = omTactics.Places.Where(o => o.Amplua.Id == omAmplua.Id).ToList<Place>();
                            if (places.Count(o => o.Player == null) > 0)
                                return;

                            foreach (var place in places)
                            {
                                if (omTeam.FinePlaces.Exists(o => place.Compare(o)))
                                    continue;

                                if (place.Player != null && omTeam.FinePlayers.Exists(o => o.Player1 == place.Player && ((bool)o.Tag)))
                                    continue;

                                if (omTeam.Tactics[0].Places.Exists(o => o.Amplua == place.Amplua && o.Position == place.Position))
                                {
                                    var p = omTeam.Tactics[0].Places.First(o => o.Amplua == place.Amplua && o.Position == place.Position);

                                    if (omTeam.FinePlaces.Exists(o => place.Compare(o)))
                                        continue;

                                    HockeyGui.ChangedPlayersList.Add(new HockeyGui.ChangedPlayersPair { Place1 = place, Place2 = p });
                                }
                            }

                            gdi.InvalidateRect();

                            if (ChangedPlayers != null)
                                ChangedPlayers(this, EventArgs.Empty);
                        }
                        catch
                        { }
                    }

                    if (omTactics != null && omAmplua == null && omPlace == null)
                    {
                        try
                        {
                            var places = omTactics.Places.Where(o => o.Amplua.Id > 1).ToList<Place>();
                            if (places.Count(o => o.Player == null) > 0)
                                return;

                            foreach (var place in places)
                            {
                                if (omTeam.FinePlaces.Exists(o => o.Compare(place)))
                                    continue;

                                if (place.Player != null && omTeam.FinePlayers.Exists(o => o.Player1 == place.Player && ((bool)o.Tag)))
                                    continue;

                                if (omTeam.Tactics[0].Places.Exists(o => o.Amplua == place.Amplua && o.Position == place.Position))
                                {
                                    var p = omTeam.Tactics[0].Places.First(o => o.Amplua == place.Amplua && o.Position == place.Position);

                                    if (omTeam.FinePlaces.Exists(o => o.Compare(place)))
                                        continue;

                                    HockeyGui.ChangedPlayersList.Add(new HockeyGui.ChangedPlayersPair { Place1 = place, Place2 = p });
                                }
                            }

                            gdi.InvalidateRect();

                            if (ChangedPlayers != null)
                                ChangedPlayers(this, EventArgs.Empty);
                        }
                        catch
                        { }
                    }
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CompleteEnter();
        }

        public void CompleteEnter()
        {
            if (HockeyGui.Mode == HockeyGui.ModeEnum.SelectManyPlayers)
            {
                if (SelectedManyPlayer != null)
                    SelectedManyPlayer(this, new HockeyGui.SelectedManyPlayerEventArgs { Players = HockeyGui.selectedManyPlayers });
            }
        }
    }
}
