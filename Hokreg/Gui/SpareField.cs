using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gdi;
using Uniso.InStat.Game;
using ShellApi;

namespace Uniso.InStat
{
    public partial class SpareField : UserControl
    {
        public GDICompatible gdi = null;
        public int PlaceSize = 22;
        public HockeyIce Game { get; set; }
        public Team Team { get; set; }

        public Point omPoint = Point.Empty;
        private Place omPlace = null;
        private Amplua omAmplua = null;
        private Tactics omTactics = null;

        public event EventHandler ChangedPlayersBegin;
        public event EventHandler ChangedPlayersEnd;
        public event EventHandler ChangedPlayers;
        public event HockeyGui.ChangedPlaceEventHandler ChangedPlace;
        public event HockeyGui.SelectedPlayerEventHandler SelectedPlayer;
        public event HockeyGui.RestorePlayerEventHandler RestorePlayer;
        public event HockeyGui.SelectedManyPlayerEventHandler SelectedManyPlayer;

        public SpareField()
        {
            InitializeComponent();

            gdi = new GDICompatible(Handle.ToInt32());
            gdi.InitContext();

            Paint += new System.Windows.Forms.PaintEventHandler(DoPaint);
            SizeChanged += new System.EventHandler(DoSizeChanged);
            MouseMove += new MouseEventHandler(DoMouseMove);
            MouseUp += new MouseEventHandler(DoMouseUp);
            MouseDown += new MouseEventHandler(DoMouseDown);
            Click += new EventHandler(DoClick);
            DoubleClick += new EventHandler(DoDoubleClick);
        }

        public void DoSizeChanged(object sender, System.EventArgs e)
        {
            gdi.UpdateContext();
            PlaceSize = 28;
        }

        private void DoMouseMove(object sender, MouseEventArgs e)
        {
            if (Game.Match == null)
                return;

            omPoint = e.Location;

            omTactics = SeekTactics(Team, omPoint);
            omAmplua = SeekAmplua(omPoint);
            omPlace = SeekPlace(omPoint);

            gdi.InvalidateRect();
        }

        private void DoDoubleClick(object sender, EventArgs e)
        {
            if (HockeyGui.Marker != null && HockeyGui.Marker.ActionId > 0 && !HockeyGui.Marker.Compare(4, 6))
                return;

            if (omTactics != null && Team != null)
            {
                HockeyGui.ChangedPlayersList.Clear();

                lock (Team.FinePlayers)
                lock (Team.FinePlaces)
                    {
                        //Замена амплуа
                        if (omAmplua != null && omPlace == null)
                        {
                            try
                            {
                                var places = omTactics.Places.Where(o => o.Amplua.Id == omAmplua.Id).ToList<Place>();
                                if (places.Count(o => o.Player == null) > 0)
                                    return;

                                foreach (var place in places)
                                {
                                    if (Team.FinePlaces.Exists(o => o.Compare(place)))
                                        continue;

                                    if (place.Player != null && Team.FinePlayers.Exists(o => o.Player1 == place.Player))
                                        continue;

                                    if (Team.Tactics[0].Places.Exists(o => o.Amplua == place.Amplua && o.Position == place.Position))
                                    {
                                        var p = Team.Tactics[0].Places.First(o => o.Amplua == place.Amplua && o.Position == place.Position);

                                        if (Team.FinePlaces.Exists(o => o.Compare(p)))
                                            continue;

                                        if (HockeyGui.IsPlaying(Team, place))
                                            continue;

                                        HockeyGui.ChangedPlayersList.Add(new HockeyGui.ChangedPlayersPair { Place1 = place, Tactics1 = omTactics, Place2 = p, Tactics2 = Team.Tactics[0] });
                                    }
                                }

                                gdi.InvalidateRect();

                                if (ChangedPlayers != null)
                                    ChangedPlayers(this, EventArgs.Empty);
                            }
                            catch
                            { }
                        }

                        //Замена 5ки
                        if (omTactics != null && omAmplua == null && omPlace == null)
                        {
                            try
                            {
                                var places = omTactics.Places.Where(o => o.Amplua.Id > 1).ToList<Place>();
                                if (places.Count(o => o.Player == null) > 0)
                                    return;

                                foreach (var place in places)
                                {
                                    if (Team.FinePlaces.Exists(o => o.Compare(place)))
                                        continue;

                                    if (place.Player != null && Team.FinePlayers.Exists(o => o.Player1 == place.Player && ((bool)o.Tag)))
                                        continue;

                                    if (Team.Tactics[0].Places.Exists(o => o.Amplua == place.Amplua && o.Position == place.Position))
                                    {
                                        var p = Team.Tactics[0].Places.First(o => o.Amplua == place.Amplua && o.Position == place.Position);

                                        if (Team.FinePlaces.Exists(o => o.Compare(p)))
                                            continue;

                                        if (HockeyGui.IsPlaying(Team, place))
                                            continue;

                                        HockeyGui.ChangedPlayersList.Add(new HockeyGui.ChangedPlayersPair { Place1 = place, Tactics1 = omTactics, Place2 = p, Tactics2 = Team.Tactics[0] });
                                    }
                                }

                                gdi.InvalidateRect();

                                if (ChangedPlayers != null)
                                    ChangedPlayers(this, EventArgs.Empty);
                            }
                            catch
                            { }
                        }

                        //Замена одного
                        if (omTactics != null && omAmplua != null && omPlace != null)
                        {
                            try
                            {
                                if (Team.FinePlaces.Exists(o => o.Compare(omPlace)))
                                    return;

                                if (omPlace.Player != null && Team.FinePlayers.Exists(o => o.Player1 == omPlace.Player && ((bool)o.Tag)))
                                    return;

                                if (Team.Tactics[0].Places.Exists(o => o.Amplua == omPlace.Amplua && o.Position == omPlace.Position))
                                {
                                    var p = Team.Tactics[0].Places.First(o => o.Amplua == omPlace.Amplua && o.Position == omPlace.Position);

                                    if (Team.FinePlaces.Exists(o => o.Compare(p)))
                                        return;

                                    if (HockeyGui.IsPlaying(Team, omPlace))
                                        return;

                                    HockeyGui.ChangedPlayersList.Add(new HockeyGui.ChangedPlayersPair { Place1 = omPlace, Tactics1 = omTactics, Place2 = p, Tactics2 = Team.Tactics[0] });
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

        private void DoMouseUp(object sender, MouseEventArgs e)
        {
        }

        private void DoMouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = e;
        }

        private MouseEventArgs mouseDown = null;

        private void DoClick(object sender, EventArgs e)
        {
            if (mouseDown == null || Game.Match == null)
                return;

            //if (HockeyGui.Marker != null && HockeyGui.Marker.ActionId > 0 && (!HockeyGui.Marker.Compare(4, 6) || Game.Half.Index != 255))
                //return;

            if (omPlace == null || omPlace.Player == null)
                return;

            //if (Team.FinePlayers.Exists(o => o.Player1 == omPlace.Player && ((bool)o.Tag)))
                //return;

            if (mouseDown.Button == System.Windows.Forms.MouseButtons.Left
                    && HockeyGui.Mode == HockeyGui.ModeEnum.SelectManyPlayers
                    && omPlace != null && omPlace.Player != null
                    && HockeyGui.Marker.Compare(8, 1)
                    && HockeyGui.Marker.Team1 != null
                    && Team == HockeyGui.Marker.Team1)
            {
                if (HockeyGui.Marker.Team1 != Team)
                    return;

                if (HockeyGui.selectedManyPlayers.IndexOf(omPlace.Player) < 0)
                    HockeyGui.selectedManyPlayers.Add(omPlace.Player);
                else
                    HockeyGui.selectedManyPlayers.Remove(omPlace.Player);

                gdi.InvalidateRect();

                if (HockeyGui.selectedManyPlayers.Count >= 2)
                {
                    if (HockeyGui.Mode == HockeyGui.ModeEnum.SelectManyPlayers)
                    {
                        if (SelectedManyPlayer != null)
                            SelectedManyPlayer(this, new HockeyGui.SelectedManyPlayerEventArgs { Players = HockeyGui.selectedManyPlayers });
                    }
                }

                return;
            }

            if (mouseDown.Button == System.Windows.Forms.MouseButtons.Left
                && (HockeyGui.Mode == HockeyGui.ModeEnum.SelectPlayer)
                && omPlace != null && omPlace.Player != null)
            {
                if (SelectedPlayer != null)
                    SelectedPlayer(this, new HockeyGui.SelectedPlayerEventArgs { Player = omPlace.Player, Place = omPlace, Tactics = omTactics });

                return;
            }

            //Едиичный
            if (mouseDown.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (omPlace != null && omTactics != null && Team != null)
                {
                    if (omPlace.Player != null && Team.FinePlayers.Exists(o => o.Id == omPlace.Player.Id && ((bool)o.Tag)))
                        return;

                    if (Team.FinePlayers.Exists(o => omPlace.Player != null && o.Id == omPlace.Player.Id && ((bool)o.Tag)))
                        return;

                    if (HockeyGui.ChangedPlayersList.Count > 0
                        && HockeyGui.ChangedPlayersList[0].Place1.Player != null
                        && HockeyGui.ChangedPlayersList[0].Place1.Player.Team.Id != Team.Id)
                        return;

                    if (HockeyGui.ChangedPlayersList.Exists(o => o.Place2 == null))
                    {
                        var cpp = HockeyGui.ChangedPlayersList.First(o => o.Place2 == null);
                        cpp.Place2 = omPlace;
                        cpp.Tactics2 = omTactics;
                        gdi.InvalidateRect();
                    }
                    else
                    //HockeyGui.ChangedPlayersList.Clear();
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

                            if (ChangedPlayersBegin != null && HockeyGui.ChangedPlayersList.Count == 0)
                                ChangedPlayersBegin(this, EventArgs.Empty);

                            HockeyGui.ChangedPlayersList.Clear();
                            HockeyGui.ChangedPlayersList.Add(new HockeyGui.ChangedPlayersPair { Place1 = omPlace, Tactics1 = omTactics });
                        }
                        gdi.InvalidateRect();
                    }
                }

                if (omPlace == null)
                {
                    HockeyGui.ChangedPlayersList.Clear();
                }

                if (ChangedPlayersEnd != null && HockeyGui.ChangedPlayersList.Count == 0)
                    ChangedPlayersEnd(this, EventArgs.Empty);

                if (ChangedPlayers != null)
                    ChangedPlayers(this, EventArgs.Empty);
            }
        }

        private void DoPaint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            try
            {
                gdi.BeforePaint();

                if (Game == null)
                    return;

                var actual_time = 0;
                lock (Game.Markers)
                    actual_time = Game.GetActuialTime(Game.Half, Game.Time, true);

                gdi.Pen.Width = 1;
                gdi.Pen.Color = 0x00000000;
                gdi.Brush.Style = BrushStyle.bsClear;
                gdi.Rectangle(new Rectangle(ClientRectangle.Left, ClientRectangle.Top + 10, ClientRectangle.Width, ClientRectangle.Height - 10));
                gdi.Brush.Style = BrushStyle.bsSolid;

                if (Game == null)
                    return;

                var rcf = GetFieldRect();

                var index = Game.Match.Team2 == Team ? 1 : 0;
                for (var i = 1; i < Team.Tactics.Count; i++)
                {
                    var t = Team.Tactics[i];

                    if (t.GetPlayers().Count == 0)
                        continue;

                    gdi.Brush.Style = BrushStyle.bsClear;
                    RectangleF rct = GetRect(Team, t);

                    foreach (var a in Game.MapA)
                    {
                        if (a.Id == 1)
                            continue;

                        var select = a == omAmplua && omPlace == null && omTactics != null && omTactics.NameActionType == t.NameActionType;

                        gdi.Pen.Color = select ? HockeyGui.TransformColor(Team.Color.SelfColor1) : 0x00000000;
                        gdi.Pen.Width = select ? 4 : 2;

                        var rca = GetRect(Team, t, a);

                        if (a.Id == 2)
                            rca = new Rectangle(rca.Left, rca.Top + PlaceSize / 2 - 3, rca.Width, rca.Height - PlaceSize + 6);
                        
                        if (index == 0)
                        {
                            rca = new Rectangle(rca.Left + (rca.Width - PlaceSize) / 2, rca.Top + PlaceSize / 2, PlaceSize, rca.Height - PlaceSize);
                            gdi.MoveTo(rca.Right, rca.Top);
                            gdi.LineTo(rca.Left, rca.Top);
                            gdi.LineTo(rca.Left, rca.Bottom);
                            gdi.LineTo(rca.Right, rca.Bottom);
                        }
                        else
                        {
                            rca = new Rectangle(rca.Right - (rca.Width - PlaceSize) / 2 - PlaceSize, rca.Top + PlaceSize / 2, PlaceSize, rca.Height - PlaceSize);
                            gdi.MoveTo(rca.Left, rca.Top);
                            gdi.LineTo(rca.Right, rca.Top);
                            gdi.LineTo(rca.Right, rca.Bottom);
                            gdi.LineTo(rca.Left, rca.Bottom);
                        }
                    }

                    //5ka
                    var select5 = omTactics != null && omTactics.NameActionType == t.NameActionType && omPlace == null && omAmplua == null;
                    gdi.Pen.Color = select5 ? HockeyGui.TransformColor(Team.Color.SelfColor1) : 0x00000000;
                    gdi.Pen.Width = select5 ? 4 : 2;

                    if (index == 0)
                    {
                        rct = new RectangleF(rct.Left + rct.Width / 2 - PlaceSize / 2, rct.Bottom - PlaceSize - PlaceSize / 2, rct.Width / 2, PlaceSize + 6);
                        gdi.MoveTo((int)rct.Left, (int)rct.Top);
                        gdi.LineTo((int)rct.Left, (int)rct.Bottom);
                        gdi.LineTo((int)rct.Right, (int)rct.Bottom);
                        gdi.LineTo((int)rct.Right, (int)rct.Top);
                    }
                    else
                    {
                        rct = new RectangleF(rct.Right - rct.Width / 2 - PlaceSize, rct.Bottom - PlaceSize - PlaceSize / 2, rct.Width / 2, PlaceSize + 6);
                        gdi.MoveTo((int)rct.Left, (int)rct.Top);
                        gdi.LineTo((int)rct.Left, (int)rct.Bottom);
                        gdi.LineTo((int)rct.Right, (int)rct.Bottom);
                        gdi.LineTo((int)rct.Right, (int)rct.Top);
                    }

                    gdi.Brush.Style = BrushStyle.bsSolid;
                    gdi.Pen.Width = 1;
                    gdi.Pen.Color = 0x00000000;
                    gdi.Brush.Style = BrushStyle.bsSolid;

                    foreach (var place in t.Places)
                    {
                        var selectedPlace = omTactics != null && place.Compare(omPlace) && omTactics.NameActionType == t.NameActionType;
                        
                        var selectedAll = (place.Amplua == omAmplua 
                            && omPlace == null 
                            && omTactics != null 
                            && omTactics.NameActionType == t.NameActionType);

                        var rc = GetRect(Team, t, place);
                        rc = new Rectangle(rc.Left + 2, rc.Top + 2, rc.Width - 4, rc.Height - 4);

                        lock (Team.FinePlaces)
                            lock (Team.FinePlayers)
                            {
                                if (HockeyGui.ChangedPlayersList.Exists(o => o.Place1 != null && o.Place2 == null)
                                    && place.Compare(omPlace) && i == 0
                                    && !Team.FinePlaces.Exists(o => o.Compare(omPlace)))
                                {
                                    var cpp = Uniso.InStat.HockeyGui.ChangedPlayersList.First(o => o.Place1 != null && o.Place2 == null);
                                    if (cpp.Place1.Player != null && cpp.Place1.Player.Team == Team)
                                    {
                                        var rce = new Rectangle(rc.Left - PlaceSize + 8, rc.Top, rc.Width, rc.Height);
                                        gdi.Pen.Width = 2;
                                        gdi.Pen.Color = 0x00000000;
                                        gdi.Brush.Color = 0x00dddddd;
                                        gdi.Ellipse(rce);

                                        if (cpp.Place1.Player != null)
                                        {
                                            gdi.Font.Color = 0x00000000;
                                            gdi.Font.Name = "Arial";
                                            if (rce.Width > 22)
                                                gdi.Font.Size = 6 + (rce.Width - 22) / 2;
                                            else
                                                gdi.Font.Size = 8;

                                            var num = cpp.Place1.Player.Number.ToString();

                                            if (num.Length >= 3)
                                                gdi.Font.Size -= 3;

                                            SIZE sz;
                                            gdi.GetTextExtent(num, out sz);

                                            var rct1 = new Rectangle(rce.Left + (rce.Width - sz.cx) / 2,
                                                    rce.Top + (rce.Height - sz.cy) / 2,
                                                    sz.cx, sz.cy);

                                            gdi.SetBkMode(Gdi.GDIBkModes.TRANSPARENT);
                                            gdi.TextOut(rct1.X, rct1.Y, num);
                                        }
                                    }
                                }
                            }

                        gdi.Pen.Style = PenStyle.psSolid;
                        gdi.Pen.Color = 0x00ffffff;
                        gdi.Brush.Color = HockeyGui.TransformColor(Team.Color.SelfColor1);
                        gdi.Pen.Width = 2;

                        gdi.Pen.Color = i > 0 ? HockeyGui.TransformColor(Team.Color.SelfColor1) : 0x00ffffff;
                        if (selectedPlace)
                            gdi.Pen.Color = 0x00000000;

                        if (place.Player != null && HockeyGui.ChangedPlayersList.Exists(o => (place.Compare(o.Place1) && t.NameActionType == o.Tactics1.NameActionType && o.Place1.Player.Team == Team) || place.Compare(o.Place2) && t.NameActionType == o.Tactics2.NameActionType))
                            gdi.Pen.Color = 0x00000000;

                        if (place.Player != null && HockeyGui.Marker != null && (HockeyGui.Marker.Player1 == place.Player || HockeyGui.Marker.Player2 == place.Player))
                            gdi.Pen.Color = 0x0000ff00;

                        if (place.Player != null && HockeyGui.selectedManyPlayers.IndexOf(place.Player) >= 0)
                            gdi.Pen.Color = 0x0000ffff;

                        var block_place = false;// HockeyGui.Marker != null;///*HockeyGui.Marker != null && HockeyGui.Marker.ActionId > 0 &&((HockeyGui.Marker != null && !HockeyGui.Marker.Compare(4, 6)) || Game.Half.Index != 255);*/ 
                        if (!block_place)
                        {
                            block_place = HockeyGui.Marker != null 
                                && HockeyGui.Marker.Compare(4, 6) 
                                && HockeyGui.Marker.Team1 != null
                                && ((Team == HockeyGui.Marker.Team1 && HockeyGui.Marker.Player1 != place.Player) || (Team != HockeyGui.Marker.Team1 && place.Amplua.Id > 1));
                        }

                        if (!block_place)
                        {
                            lock (Team.FinePlaces)
                            lock (Team.FinePlayers)
                            {
                                if ((Team.FinePlaces.Exists(o => o.Compare(place)) && t.NameActionType == 0)
                                    || (place.Player != null && Team.FinePlayers.Exists(o => o.Player1 == place.Player && ((bool)o.Tag))))
                                {
                                    block_place = true;
                                }
                            }
                        }

                        if (block_place)
                        {
                            gdi.Brush.Color = 0x00b0b0b0;
                            gdi.Pen.Color = 0x00808080;
                        }
                        
                        if (place.Player != null && Team.Tactics[0].Places.Exists(o => o.Player != null && o.Player.Id == place.Player.Id))
                        {
                            gdi.Brush.Color = 0x00ffffff;
                        }

                        gdi.Ellipse(rc); 

                        lock (Team.FinePlayers)
                        {
                            var h_time = 0;
                            if (Game != null && Game.Half != null)
                            {
                                foreach (var h in Game.HalfList)
                                {
                                    if (h.Index == Game.Half.Index)
                                        break;

                                    h_time += Convert.ToInt32(h.Length);
                                }
                            }

                            if (place.Player != null && Team.FinePlayers.Exists(o => o.Player1 == place.Player))
                            {
                                var mklist = Team.FinePlayers.Where(o => o.Player1 == place.Player).ToList<Marker>();
                                if (mklist.Count > 0)
                                {
                                    var mk = mklist.First();

                                    if (Game.Markers.Where(o => o.TimeActualTotal >= mk.TimeActualTotal)
                                        .OrderBy(o => o.TimeActualTotal)
                                        .Any(o => Game.IsStopTimeMarker(o)))
                                    {
                                        Marker mks = null;
                                        lock (Game.Markers)
                                            mks = Game.Markers.Where(o => o.TimeActualTotal >= mk.TimeActualTotal)
                                                 .OrderBy(o => o.TimeActualTotal)
                                                 .First(o => Game.IsStopTimeMarker(o));

                                        var ft = Game.GetFineTime(mklist);//mklist.Sum(o => Game.GetFineTime(o));
                                        var fine_end = mk.TimeActualTotal + ft;

                                        var rctimer = new Rectangle(rc.Left + 2, rc.Bottom - 8, rc.Width - 4, 10);
                                        gdi.Brush.Color = 0x000000ff;
                                        gdi.Pen.Color = 0x000000ff;
                                        gdi.Pen.Width = 1;
                                        gdi.Rectangle(rctimer);

                                        if (!mklist.Exists(o => o.Compare(5, new int[] { 7, 8, 9 })))
                                        {
                                            var ft0 = actual_time - mks.TimeActualTotal;
                                            if (ft0 < 0)
                                                ft0 = 0;
                                            var ms = ft - ft0;
                                            var ss = ms / 1000;
                                            var m = ss / 60;
                                            var s = ss % 60;

                                            var tf = String.Format("{0}:{1}", m.ToString("0"), s.ToString("00"));
                                            gdi.Font.Name = "Arial";
                                            gdi.Font.Size = 6;
                                            gdi.Font.Color = 0x00ffffff;

                                            gdi.SetBkMode(GDIBkModes.TRANSPARENT);
                                            gdi.DrawTextCenter(tf, rctimer);
                                            gdi.SetBkMode(GDIBkModes.BKMODE_LAST);
                                        }
                                    }
                                }
                            }
                        }

                        /*if (!block_place)
                        {
                            lock (HockeyGui.RestorePlayers)
                                if (HockeyGui.RestorePlayers.IndexOf(place.Player) >= 0 && !HockeyGui.IsPlaying(place.Player))
                                {
                                    gdi.Pen.Width = 2;
                                    gdi.Pen.Color = 0x0000ff00;
                                    gdi.Ellipse(rc);
                                }
                        }*/

                        gdi.Font.Color = HockeyGui.TransformColor(Team.Color.NumberColor);
                        if (place.Player != null && Team.Tactics[0].Places.Exists(o => o.Player != null && o.Player.Id == place.Player.Id))
                        {
                            gdi.Brush.Color = 0x00ffffff;
                            gdi.Font.Color = 0x00000000;
                        }

                        gdi.Pen.Width = 1;
                        gdi.Pen.Color = 0x00000000;

                        if (place.Player != null)
                        {
                            //int color = HockeyGui.FontColor(Team.Color.NumberColor); //0x00000000; //Color.FromArgb(255, 255 - Team.Color.SelfColor1.R, 255 - Team.Color.SelfColor1.G, 255 - Team.Color.SelfColor1.B);
                            //if (!HockeyGui.IsPlaying(place.Player))
                                //color = HockeyGui.FontColor(Team.Color.SelfColor1);

                            //gdi.Font.Color = color;
                            gdi.Font.Name = "Arial";
                            if (rc.Width > 22)
                                gdi.Font.Size = 9 + (rc.Width - 22) / 2;
                            else
                                gdi.Font.Size = 8;

                            try
                            {
                                var num = place.Player.Number.ToString();

                                if (num.Length >= 3)
                                    gdi.Font.Size -= 3;

                                SIZE sz;
                                gdi.GetTextExtent(num, out sz);

                                var rct1 = new Rectangle(rc.Left + (rc.Width - sz.cx) / 2,
                                        rc.Top + (rc.Height - sz.cy) / 2,
                                        sz.cx, sz.cy);

                                gdi.SetBkMode(Gdi.GDIBkModes.TRANSPARENT);
                                gdi.TextOut(rct1.X, rct1.Y, num);

                                if (selectedPlace)
                                {
                                    gdi.Font.Color = selectedPlace || selectedAll ? 0x00000000 : 0x00808080;
                                    gdi.Font.Size = 8;

                                    gdi.TextOut(3, 13, place.Player.ToString());
                                }

                                gdi.SetBkMode(Gdi.GDIBkModes.BKMODE_LAST);
                            }
                            catch
                            { }
                        }
                    }                    
                }

                var y = (int)rcf.Top + GetGkBottom() - 10;

                gdi.Pen.Style = PenStyle.psSolid;
                gdi.Pen.Color = 0x00ff0000;
                gdi.Pen.Width = 1;

                gdi.MoveTo(10, y);
                gdi.LineTo(Width - 10, y);
            }
            finally
            {
                gdi.AfterPaint();
            }
        }

        private Team SeekTeam(Point pt)
        {
            var rcf = GetFieldRect();
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
                var t = tm.Tactics[i];
                if (GetRect(tm, t).Contains(pt))
                    return t;
                if (GetRect(tm, t, Game.MapA[0]).Contains(pt))
                    return t;
            }

            return null;
        }

        private Amplua SeekAmplua(Point pt)
        {
            return SeekAmplua((Uniso.InStat.Game.Team)Team, pt);
        }

        private Amplua SeekAmplua(Uniso.InStat.Game.Team tm, Point pt)
        {
            if (tm.Tactics == null || tm.Tactics.Count == 0)
                return null;

            for (var i = 0; i < tm.Tactics.Count; i++)
            {
                var t = tm.Tactics[i];
                foreach (var a in Game.MapA)
                {
                    RectangleF r = GetRect(tm, t, a);
                    if (r.Contains(pt))
                        return a;
                }
            }

            return null;
        }

        private Place SeekPlace(Point pt)
        {
            return SeekPlace((Uniso.InStat.Game.Team)Team, pt);
        }

        private Place SeekPlace(Uniso.InStat.Game.Team tm, Point pt)
        {
            if (tm.Tactics == null || tm.Tactics.Count == 0)
                return null;

            for (var i = 0; i < tm.Tactics.Count; i++)
            {
                var t = tm.Tactics[i];
                foreach (var p in t.Places)
                    if (GetRect(tm, t, p).Contains(pt))
                        return p;
            }

            return null;
        }

        private RectangleF GetFieldRect()
        {
            return new RectangleF(10, 30, Width - 20, Height);
        }

        private Rectangle GetRect(Team tm, Tactics t)
        {
            var rcf = GetFieldRect();

            var index = Game.Match.Team2 == tm ? 1 : 0;

            //
            var dist = 90;
            for (var i = 1; i < tm.Tactics.Count; i++)
            {
                if (tm.Tactics[i] == t)
                {
                    return new Rectangle((int)rcf.Left, (int)rcf.Top + (dist + 20) * (i - 1), (int)rcf.Width, dist + 10);
                }
            }

            return Rectangle.Empty;
        }

        private Rectangle GetRect(Team tm, Tactics t, Place p)
        {
            var rcf = GetFieldRect();
            RectangleF rca = GetRect(tm, t, p.Amplua);

            var dw = (int)rca.Width;
            var dh = (int)rca.Height / 3;
            var index = Game.Match.Team2 == tm ? 1 : 0;

            var res = (index == 0)
                ? new Rectangle((int)rca.Left, (int)rca.Top + dh * (p.Position.Id - 1), dw, dh)
                : new Rectangle((int)rca.Left, (int)rca.Top + dh * (3 - p.Position.Id), dw, dh);
            
            // if ((index == 0 && p.Amplua.Id == 2) || (index == 1 && p.Amplua.Id == 3))
            if (index == 0)
                res = new Rectangle(res.Right - PlaceSize, res.Top + res.Height / 2 - PlaceSize / 2, PlaceSize, PlaceSize);
            else
                res = new Rectangle(res.Left, res.Top + res.Height / 2 - PlaceSize / 2, PlaceSize, PlaceSize);

            if (p.Amplua.Id == 2)
            {
                if ((index == 0 && p.Position.Id == 1) || (index == 1 && p.Position.Id == 3))
                    res.Offset(0, +PlaceSize / 2 - 1);
                else
                    res.Offset(0, -PlaceSize / 2 + 1);
            }

            return res;
        }

        private int GetGkBottom()
        {
            var res = 0;

            for (var i = 1; i < Team.Tactics.Count; i++)
                if (Team.Tactics[i].GetPlayers().Count > 0)
                    res++;
                        
            return res * 110 + 20;
        }

        private Rectangle GetRect(Team tm, Tactics t, Amplua a)
        {
            var rcf = GetFieldRect();

            var i = 0;
            for (var ii = 0; ii < tm.Tactics.Count; ii++)
                if (tm.Tactics[ii] == t)
                    i = ii;

            var index = Game.Match.Team2 == tm ? 1 : 0;

            if (a.Id == 1)
            {
                if (i == 1 || i == 2)
                {
                    var resgk = new Rectangle(0, (int)rcf.Top + GetGkBottom(), (int)rcf.Width / 2, PlaceSize);
                    if (index == 0)
                        resgk.Offset((int)rcf.Left + (int)(rcf.Width / 2 * (2 - i)), 0);
                    else
                        resgk.Offset((int)rcf.Left + (int)(rcf.Width / 2 * (i - 1)), 0);
                    return resgk;
                }

                return Rectangle.Empty;
            }

            var rct = GetRect(tm, t);
            rct = new Rectangle(rct.Left, rct.Top, rct.Width, rct.Height - 20);

            if (i == 0)
                return Rectangle.Empty;

            if (index == 0)
            {
                var res = new Rectangle((int)rct.Left + (a.Id == 3 ? rct.Width / 2 : 0), 
                    (int)rct.Top, (int)rct.Width / 2, (int)rct.Height);
                return res;
            }
            else
            {
                var res = new Rectangle((int)rct.Left + (a.Id == 2 ? rct.Width / 2 : 0),
                    (int)rct.Top, (int)rct.Width / 2, (int)rct.Height);
                return res;
            }
        }
    }
}
