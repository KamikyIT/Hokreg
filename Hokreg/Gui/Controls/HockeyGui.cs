using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Uniso.InStat.Game;

namespace Uniso.InStat.Gui.Controls
{
    public class HockeyGui : UserControl
    {
        public enum ModeEnum
        {
            View,
            EditTactics,
            SelectPlayer,
            SelectManyPlayers,
            SelectPoint,
            SelectPointAndDest,
        }

        private static ModeEnum mode = ModeEnum.View;
        public static ModeEnum Mode
        {
            get { return mode; }
            set
            {
                if (mode != value)
                {
                    if (HF != null)
                        HF.point = PointF.Empty;
                    
                    mode = value;
                    
                    if (ManyPlayersCompleteA != null)
                        ManyPlayersCompleteA.Visible = mode == ModeEnum.SelectManyPlayers && HockeyGui.Marker != null && HockeyGui.Marker.ActionId == 12;
                    if (ManyPlayersCompleteB != null)
                        ManyPlayersCompleteB.Visible = mode == ModeEnum.SelectManyPlayers && HockeyGui.Marker != null && HockeyGui.Marker.ActionId != 12;

                    HockeyGui.selectedManyPlayers.Clear();

                    InvalidateRect();                    
                }

                if (HF != null)
                    HF.RefreshRgn();
            }
        }

        public static void InvalidateRect()
        {
            if (HF != null)
                HF.gdi.InvalidateRect();

            if (SF1 != null)
                SF1.gdi.InvalidateRect();

            if (SF2 != null)
                SF2.gdi.InvalidateRect();
        }

        //public static void SetMode(ModeEnum mode, Marker mk, string labelInviteText = null)
        public static void SetMode(ModeEnum mode, Marker mk)
        {
            
            HockeyGui.Marker = mk;
            HockeyGui.Mode = mode;

            if (HF != null && mode == ModeEnum.SelectPointAndDest)
                HF.point = mk.Point1;

            //if (string.IsNullOrEmpty(labelInviteText) == false)
            //{
            //    HockeyGui.LabelInviteAction.Text = labelInviteText;
            //    HockeyGui.LabelInviteAction.Visible = true;
            //}
            //else
            //{
            //    HockeyGui.LabelInviteAction.Visible = false;
            //}
            

            InvalidateRect();
        }

        [Serializable()]
        public class ChangedPlayersPair
        {
            public Tactics Tactics1 { get; set; }
            public Tactics Tactics2 { get; set; }
            public Place Place1 { get; set; }
            public Place Place2 { get; set; }
        }

        public class ChangedPlaceEventArgs : EventArgs
        {
            public Team Team { get; set; }
            public Tactics Tactics { get; set; }
            public Place Place { get; set; }
        }
        public delegate void ChangedPlaceEventHandler(object sender, ChangedPlaceEventArgs e);

        public class SelectedPlayerEventArgs : EventArgs
        {
            public Tactics Tactics { get; set; }
            public Place Place { get; set; }
            public Player Player { get; set; }
        }
        public delegate void SelectedPlayerEventHandler(object sender, SelectedPlayerEventArgs e);

        public class RestorePlayerEventArgs : EventArgs
        {
            public Tactics Tactics { get; set; }
            public Place Place { get; set; }
            public Player Player { get; set; }
        }
        public delegate void RestorePlayerEventHandler(object sender, RestorePlayerEventArgs e);
        
        public class SelectedPointEventArgs : EventArgs
        {
            public PointF Point { get; set; }
        }
        public delegate void SelectedPointEventHandler(object sender, SelectedPointEventArgs e);
        
        public class SelectedPointAndDestEventArgs : EventArgs
        {
            public PointF Point1 { get; set; }
            public PointF Point2 { get; set; }
        }
        public delegate void SelectedPointAndDestEventHandler(object sender, SelectedPointAndDestEventArgs e);
        
        public class SelectedManyPlayerEventArgs : EventArgs
        {
            public List<Player> Players { get; set; }
        }
        public delegate void SelectedManyPlayerEventHandler(object sender, SelectedManyPlayerEventArgs e);

        public static HockeyField HF { get; set; }
        //public static Label LabelInviteAction { get; set; }
        public static SpareField SF1 { get; set; }
        public static SpareField SF2 { get; set; }
        public static LinkLabel ManyPlayersCompleteA { get; set; }
        public static LinkLabel ManyPlayersCompleteB { get; set; }

        public static List<Player> selectedManyPlayers = new List<Player>();
        public static List<ChangedPlayersPair> ChangedPlayersList = new List<ChangedPlayersPair>();
        public static List<Player> RestorePlayers = new List<Player>();

        private static Marker marker = null;

        public static Marker Marker
        {
            get { return marker; }
            set
            {
                marker = value;
                if (marker == null)
                {
                    selectedManyPlayers.Clear();
                    if (HF != null)
                        HF.point = Point.Empty;
                }

                InvalidateRect();
            }
        }

        public static bool IsFine(HockeyIce game, Player p)
        {
            lock (p.Team.FinePlayers)
                return p != null && p.Team.FinePlayers.Exists(o => o != null && o.Id == p.Id && ((bool)o.Tag));
        }

        public static bool IsPlaying(Player p)
        {
            return p != null && p.Team.Tactics[0].Places.Exists(o => o.Player != null && o.Player.Id == p.Id);
        }

        public static bool IsPlaying(Team tm, Place p)
        {
            return p.Player != null && tm.Tactics[0].Places.Exists(o => o.Player != null && o.Player.Id == p.Player.Id);
        }

        public static int FontColor(Color color)
        {
            return color.R + color.G * 256 + color.B * 65536;

            if (color.GetBrightness() > 0.5f)
                return 0x00000000;

            return 0x00ffffff;
        }

        public static int TransformColor(Color color)
        {
            if (color.B > 240 && color.G > 240 && color.R > 240)
                return 0x00dddddd;

            return (color.B << 16) + (color.G << 8) + color.R;
        }

        public static void SetInviteLabel(ModeEnum newmode, Game.Marker mk, string text, Label inviteLabel)
        {
            HockeyGui.SetMode(newmode, mk);

            if (mk != null)
            {
                inviteLabel.Visible = true;

                inviteLabel.Text = text;
            }
            else
            {
                inviteLabel.Visible = false;
            }


        }
    }
}
