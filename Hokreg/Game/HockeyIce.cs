using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Uniso.InStat.Classes;
using Uniso.InStat.Conv;

namespace Uniso.InStat.Game
{
    public class HockeyIce : GameBase
    {
        public enum RoleEnum
        {
            Single = 0,
            Ttd = 1,
            Substitutions = 2,
            AdvTtd = 3,
            Online = 4,
        }

        public enum GameTypeEnum
        { 
            Euro,
            // ReSharper disable once InconsistentNaming
            NHL,
        }

        public Half Half { get; set; }
        public int Time { get; set; }

        public GameTypeEnum GameType { get; set; }
      
        public static ActionConverter convAction = new ActionConverter();
        public static User User { get; set; }
        public static RoleEnum Role { get; set; }
        public Dictionary<int, int> SyncTime { get; set; }

        public bool UpdateSync(Half half)
        {
            InStat.Marker mk189 = null;
            lock (Markers)
                mk189 = Markers.FirstOrDefault(o => !o.FlagDel && o.Half.Index == Half.Index && o.Compare(18, 9));

            if (mk189 == null)
            {
                if (SyncTime.ContainsKey(half.Index))
                {
                    SyncTime.Remove(half.Index);
                    return true;
                }
            }

            if (mk189 != null)
            {
                if (!SyncTime.ContainsKey(half.Index))
                {
                    SyncTime.Add(half.Index, mk189.TimeVideo);
                    return true;
                }
                else
                {
                    var res = SyncTime[half.Index] != mk189.TimeVideo;
                    SyncTime[half.Index] = mk189.TimeVideo;
                    return res;
                }
            }
            
            return false;
        }

        public int GetSync(Half half)
        {
            if (!SyncTime.ContainsKey(half.Index))
                return 0;

            return SyncTime[half.Index];
        }

        public override int Id
        {
            get
            {
                return 5;
            }
        }

        public Uniso.InStat.Marker LastBullet(int video_second)
        {
            try
            {
                lock (Markers)
                    return Markers.OrderByDescending(o => o.TimeVideo).First(o => o.Half.Index == 255 && o.TimeVideo <= video_second && o.Compare(4, 6));
            }
            catch
            { }

            return null;
        }

        public Dictionary<Player, List<Uniso.InStat.Marker>> GetFineList(InStat.Team team, Half half, int time)
        {
            List<Uniso.InStat.Marker> mlist = null;

            lock (Markers)
                mlist = Markers
                .Where(o
                    => (o.Team1 == team)
                    && (o.Half.Index < Half.Index || (o.Half.Index == Half.Index && o.TimeVideo <= time))
                    && (o.Compare(5, new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }))
                    && (!o.FlagDel))
                .OrderByDescending(o => o.TimeActualTotal)
                .ToList<Uniso.InStat.Marker>();

            var ami = new List<Player>();
            var res = new Dictionary<Player, List<Uniso.InStat.Marker>>();
            foreach (Marker mk in mlist)
            {
                if (mk.Player1 != null && !ami.Contains(mk.Player1))
                {
                    ami.Add(mk.Player1);
                    if (mk.ActionType > 1)
                    {
                        ami.Add(mk.Player1);
                        res.Add(mk.Player1, GetSiblings(mk).Where(o => o.ActionId == 5 && o.Player1 != null && o.Player1 == mk.Player1 && !o.FlagDel).ToList<Uniso.InStat.Marker>());
                    }
                }
            }

            return res;
        }

        public int GetFineTime(List<InStat.Marker> mklist)
        {
            var ft = 0;
            var mkf = mklist[0];
            var res = GetFineList(mkf.Team1, mkf.Half, mkf.TimeVideo);
            var time = Int32.MaxValue;
            InStat.Marker first_small_fine = null;

            foreach (var mkl in res.Values)
            { 
                foreach (var mki in mkl.Where(o => o.Compare(5, new int[] { 2, 3, 4, 5 })))
                {
                    var sibl2 = GetSiblings(mki);
                    if (mki.TimeActualTotal < time && !sibl2.Exists(o => o.ActionId == 14 && o.Player2 != mki.Player1))
                    {
                        first_small_fine = mki;
                        time = mki.TimeActualTotal;
                    }
                }
            }

            foreach (Marker mk in mklist)
            {
                var ft0 = GetFineTime(mk);
                if (mk.Compare(5, new int[] { 2, 3 }) && first_small_fine == mk)
                {
                    //Выход, если был гол
                    lock (Markers)
                    {
                        //Goal
                        if (Markers.Any(o => o.Compare(8, 1)
                            && o.TimeActualTotal > mk.TimeActualTotal
                            && o.TimeActualTotal < mk.TimeActualTotal + ft0
                            && o.Team2 == mk.Team1))
                        {
                            var sibl2 = GetSiblings(mk);
                            if (!sibl2.Exists(o => o.ActionId == 14 && o.Player2 != mklist[0].Player1))
                            {
                                var mkg = Markers.First(o => o.Compare(8, 1)
                                    && o.TimeActualTotal > mk.TimeActualTotal
                                    && o.TimeActualTotal < mk.TimeActualTotal + ft0
                                    && o.Team2 == mk.Team1);

                                ft0 = mkg.TimeActualTotal - mk.TimeActualTotal;
                            }
                        }
                    }
                }

                ft += ft0;
            }

            return ft;
        }

        public int GetFineTime(Uniso.InStat.Marker mk)
        {
            var fine_time = 0;

            if (mk.Compare(5, new int[] {2, 3}))
                fine_time = 2 * 60000;
            if (mk.Compare(5, 4))
                fine_time = 4 * 60000;
            if (mk.Compare(5, 5))
                fine_time = 5 * 60000;
            if (mk.Compare(5, 6))
                fine_time = 10 * 60000;
            if (mk.Compare(5, 7))
                fine_time = 200 * 60000;
            if (mk.Compare(5, 8))
                fine_time = 200 * 60000;

            return fine_time;
        }

        public override void Union(List<InStat.Marker> local, List<InStat.Marker> srv)
        {
            var res = new List<InStat.Marker>();
            res.AddRange(local.Where(o => !o.FlagSaved));

            if (srv == null)
                res.AddRange(local);
            else
            {
                foreach (var mks in srv)
                {
                    if (local.Exists(o => o.Id == mks.Id))
                    {
                        var mkl = local.First(o => o.Id == mks.Id);
                        if (mkl.FlagUpdate || mkl.FlagDel)
                        {
                            res.Add(mkl);
                            continue;
                        }
                    }
                    res.Add(mks);
                }
            }

            lock (Markers)
            {
                Markers.Clear();
                Markers.AddRange(res);

                foreach (Marker mk in Markers)
                {
                    if (mk.user_id == 0 && User != null)
                        mk.user_id = User.Id;
                }

                RecalcActualTime(Markers, null);
            }

            //Markers.SortBy();
        }

        public class TacticsTime : List<Marker>
        {
            public Half Half { get; set; }
            public int Time { get; set; }
            public int Num { get; set; }

            public TacticsTime(Half half, int time, int num)
            {
                Half = half;
                Time = time;
                Num = num;
            }
        }

        public override void Insert(Uniso.InStat.Marker mk)
        {
            lock (Markers)
            {
                if (Markers.IndexOf(mk) >= 0)
                    return;
            }

            if (mk.user_id == 0)
                mk.user_id = User.Id;

            var sibl = GetSiblings(mk);

            if (mk.ActionId == 14 && mk.player1_id > 0 && mk.player2_id > 0)
            {
                if (mk.team2_id > 0 && sibl.Any(o => !o.FlagDel && o.Compare(mk.ActionId, mk.ActionType) && o.team2_id > 0 && o.team2_id == mk.team2_id))
                {
                    foreach (Marker mki in sibl.Where(o => !o.FlagDel && o.Compare(mk.ActionId, mk.ActionType) && o.team2_id > 0 && o.team2_id == mk.team2_id))
                    {
                        if (mk.player1_id != mki.player2_id)
                        {
                            mk.Player2 = mki.Player2;
                            lock (Markers)
                            {
                                Log.Write("REMOVE (LOCK CH0) " + mki);

                                mki.FlagDel = true;
                                if (!mki.FlagSaved)
                                    Markers.Remove(mki);

                                Log.Write("REMOVE (LOCK CH1) " + mki);
                            }
                        }
                    }
                }

                if (sibl.Any(o => !o.FlagDel
                    && o.ActionId == 14
                    && ((o.player1_id > 0 && o.player1_id == mk.player1_id)
                     || (o.player1_id > 0 && o.player1_id == mk.player2_id)
                     || (o.player2_id > 0 && o.player2_id == mk.player1_id)
                     || (o.player2_id > 0 && o.player2_id == mk.player2_id))))
                {
                    System.Windows.Forms.MessageBox.Show(String.Format("Невозможно вставить замену {0} на {1}\nт.к. такая замена присутствует в это же время", mk.Player1, mk.Player2),
                        "ERROR", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }

                if (mk.Player1 != null && mk.Player2 != null && mk.Team1.Id != mk.Team2.Id)
                {
                    Log.Write("INSERT NEW ERROR " + mk);
                    return;
                }

                if (mk.Player1 == mk.Player2)
                {
                    Log.Write("INSERT NEW ERROR " + mk);
                    return;
                }
            }

            Log.Write("INSERT NEW " + mk);

            lock (Markers)
            {
                Markers.Add(mk);
                RecalcActualTime(Markers, null);
            }

            Log.Write("INSERT NEW 2" + mk);

            if (HockeyIce.Role == HockeyIce.RoleEnum.AdvTtd)
                RecalcMarkers(mk.Half);

            Log.Write("INSERT NEW 3 RECALC MARKER");
        }

        public bool IsInsertPickUp(InStat.Marker mk)
        {
            var prevm = GetPrevousMarkers(mk.Half, mk.TimeVideo)
                .Where(o => !o.IsSecondary).ToList<InStat.Marker>();
            return IsInsertPickUp(mk, prevm);
        }

        public bool IsInsertPickUp(InStat.Marker mk, List<InStat.Marker> prevm)
        {
            if (!mk.Compare(2, new int[] { 5, 9, 11 }) && !mk.Compare(1, new int[] { 1, 2 }) && mk.ActionId != 3
                    && mk.ActionId != 18 && mk.ActionId != 16 && mk.ActionId != 14 && mk.ActionId != 9
                    && mk.ActionId != 12 && mk.ActionId != 5 && mk.ActionId != 6 && mk.ActionId != 4
                    && mk.ActionId != 11 && mk.ActionId != 13 && mk.ActionId != 8)
            {
                var sibl = GetSiblings(mk);
                if (sibl.Count > 1
                    && sibl.Any(o => o.Compare(2, new int[] { 1, 10 })) 
                    && sibl.Any(o => o.Compare(1, new int[] { 3, 4, 5, })))
                    return false;

                if (prevm.Count == 0)
                    return false;

                if (prevm.Count == 1 && prevm.Any(o => o.Compare(2, new int[] { 4, 5 }) && o.player1_id == mk.player1_id))
                    return false;

                    /*если в маркере предыдущего действия не задействован игрок, выполняющий текущее действие
                    Если предыдущее действе - не Вбрасывание, КП, ПП, ОП или Наброс игрока той же команды, что совершает действие
                    Если текущее действие - ни подбор, ни прием, ни единоборство, ни перехват, ни перехват неудачный, ни фол,Подбор в борьбе
                    */
                    {
                        /*bool r1 = (!prevm.Any(o => o.player1_id == mk.player1_id || o.player2_id == mk.player1_id) || prevm.Any(o => o.team1_id != mk.team1_id));
                        if (prevm.Any(o => o.Compare(2, 2) && o.Win == 2))
                            r1 = (!prevm.Any(o => o.player1_id == mk.player1_id || o.player2_id == mk.player1_id) || prevm.Any(o => o.team2_id != mk.team1_id)); ;
                        */
                        var r1 = !prevm.Any(o => o.player1_id == mk.player1_id || o.player2_id == mk.player1_id);
                        var r2 = !prevm.Any(o => o.Compare(1, new int[] { 3, 4, 5, 7, 9 }) && o.team1_id == mk.team1_id);
                        var r3 = !mk.Compare(2, new int[] { 4, 5, 1, 10, 7, 9, 6 }) && !mk.Compare(3, 8);
                        return r1 && r2 && r3;
                    }
            }

            return false;
        }

        public class EditMrk
        {
            public Game.Marker G { get; set; }
        }

        public EditMrk editMarker = new EditMrk();

        public void RecalcMarkers(Half half)
        {
            lock (Markers)
            {
                //Простановка продолжения ведения и точек назначения
                Player last_p = null;
                foreach (Marker mki in Markers.Where(o => !o.FlagDel && o.Half.Index == half.Index).OrderBy(o => o.TimeVideo))
                {
                    lock (editMarker)
                        if (mki == editMarker.G)
                            continue;

                    if (mki.Compare(2, 3))
                    {
                        if (mki.Win < 2)
                        {
                            if (last_p != mki.Player1)
                            {
                                if (mki.Win != 0)
                                {
                                    mki.Win = 0;
                                    mki.FlagUpdate = true;
                                }
                            }
                            else
                            {
                                if (mki.Win != 1)
                                {
                                    mki.Win = 1;
                                    mki.FlagUpdate = true;
                                }
                            }

                            last_p = mki.Player1;
                        }

                        var mkn = Markers.Where(o => o.Half.Index == mki.Half.Index && o.TimeVideo > mki.TimeVideo).OrderBy(o => o.TimeVideo).FirstOrDefault();
                        if (mkn != null)
                        {
                            if (mkn.team1_id > 0)
                            {
                                if (mkn.team1_id == mki.team1_id)
                                {
                                    if (!mkn.Point1.Equals(mki.Point2))
                                    {
                                        mki.Point2 = mkn.Point1;
                                        mki.FlagUpdate = true;
                                    }
                                }
                                else
                                {
                                    var pt = new PointF(FieldSize.Width - mkn.Point1.X, FieldSize.Height - mkn.Point1.Y);
                                    if (!pt.Equals(mki.Point2))
                                    {
                                        mki.Point2 = pt;
                                        mki.FlagUpdate = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        last_p = null;
                    }
                }

                var order = new List<InStat.Marker>();

                var time = Int32.MaxValue;
                foreach (Marker mki in Markers.Where(o => !o.FlagDel && o.Half.Index == half.Index
                    && (o.Compare(1, new int[] { 3, 4, 5, 6, 7, 8, 9, 10 })
                    || o.Compare(2, new int[] { 1, 2, 3, 4, 5, 6, 8, 10 })
                    || (o.Compare(2, 7) && o.Win < 2)
                    || o.Compare(3, 8)
                    || o.Compare(4, new int[] { 1, 2, 3, 4, 5 })
                    || o.Compare(8, new int[] { 1 })))
                    .OrderByDescending(o => o.TimeVideo))
                {
                    lock (editMarker)
                        if (mki == editMarker.G)
                            continue;

                    if (mki.TimeVideo >= time)
                        continue;

                    InStat.Marker mk = null;
                    var sibl = GetSiblings(mki.Half, mki.TimeVideo);

                    if (sibl.Any(o => o.Compare(2, 1)))
                        mk = sibl.First(o => o.Compare(2, 1));

                    if (mk == null)
                    {
                        if (sibl.Count == 1)
                            mk = sibl.First();

                        if (mk == null && sibl.Any(o => !o.Compare(2, new int[] { 4, 5 })))
                            mk = sibl.First(o => !o.Compare(2, new int[] { 4, 5 }));
                    }

                    if (mk == null)
                        continue;

                    order.Add(mk);
                    time = mk.TimeVideo;
                }

                //Пересчет оппонентов и точек назначения
                foreach (Marker mki in order)
                {
                    lock (editMarker)
                        if (mki == editMarker.G)
                            continue;

                    var mlist = Markers
                            .Where(o
                                => (o.Half.Index == half.Index && o.TimeVideo < mki.TimeVideo)
                                && !o.FlagDel
                                && (
                                   o.Compare(1, new int[] { 3, 4, 5, 6, 7, 8, 9, 10 })
                                || o.Compare(2, new int[] { 1, 2, 3, 6, 8, 10 })
                                || (o.Compare(2, 7) && o.Win < 2)
                                || o.Compare(3, 8)
                                || o.Compare(4, new int[] { 1, 2, 3, 4, 5 })
                                || o.Compare(8, new int[] { 1 }))
                                )
                            .OrderBy(o => o.Half.Index)
                            .ThenBy(o => o.TimeVideo)
                            .ToList<Uniso.InStat.Marker>();

                    var prev = new List<InStat.Marker>();
                    if (mlist.Count > 0)
                    {
                        var mk = mlist[mlist.Count - 1];
                        prev.AddRange(mlist.Where(o => o.Half == mk.Half && o.TimeVideo == mk.TimeVideo).ToList<Uniso.InStat.Marker>());
                    }

                    if (prev.Count > 0)
                    {
                        if (prev.Any(o => o.Compare(1, new int[] { 3, 4, 5, 7, 8, 9, 10 })))
                        {
                            var mkp = prev.First(o => o.Compare(1, new int[] { 3, 4, 5, 7, 8, 9, 10 }));

                            if (mkp.player2_id == 0 && mkp.Win == 2 && !mkp.Point2.IsEmpty)
                                continue;

                            if (mki.Compare(3, 8))
                            {
                                mkp.Win = 2;
                                mkp.Player2 = null;
                                continue;
                            }

                            //Если взаимодействие двух игроков
                            if (mki.Compare(2, new int[] { 1, 2, 6, 8, 10 }) && mki.Player1 != null)
                            {
                                Player opp = null;
                                if (mki.team1_id == mkp.team1_id)
                                    opp = mki.Player1;
                                if (mki.team2_id == mkp.team1_id)
                                    opp = mki.Player2;

                                if (opp == null)
                                {
                                    mki.FlagDel = true;
                                    continue;
                                }

                                if (mkp.Player2 != opp)
                                {
                                    mkp.Player2 = opp;
                                    mkp.FlagUpdate = true;
                                }

                                /*if (mkp.Win != 1)
                                {
                                    mkp.Win = 1;
                                    mkp.FlagUpdate = true;
                                }*/

                                //Проверка точности
                                if (mkp.Player1 != null && mkp.Player2 != null && !mkp.Compare(1, 10))
                                {
                                    //Неточная
                                    if (mkp.Player1.Team.Id != mkp.Player2.Team.Id && mkp.Win != 2)
                                    {
                                        mkp.Win = 2;
                                        mkp.FlagUpdate = true;
                                    }

                                    //Точная
                                    if (mkp.Player1.Team.Id == mkp.Player2.Team.Id && mkp.Win != 1)
                                    {
                                        mkp.Win = 1;
                                        mkp.FlagUpdate = true;
                                    }
                                }
                            }

                            //Если текущая - передача или Ведение,Прием,Подбор,Перехват или броски иили гол
                            if (mki.Compare(1, new int[] { 3, 4, 5, 6, 7, 8, 9, 10 })
                                || mki.Compare(2, new int[] { 3, 4, 5, 7 })
                                || mki.Compare(4, new int[] { 1, 2, 3, 4, 5 })
                                || mki.Compare(8, 1))
                            {
                                //Сравнение игроков и оппонентов
                                if (mkp.player2_id != mki.player1_id)
                                {
                                    mkp.Player2 = mki.Player1;
                                    mkp.FlagUpdate = true;
                                }

                                //Точки
                                if (!mkp.Point2.Equals(mki.Point1))
                                {
                                    mkp.Point2 = mki.Point1;
                                    mkp.FlagUpdate = true;
                                }

                                //Проверка отсутствия точности
                                if ((mkp.Player1 != null && mkp.Player2 == null) && mkp.Win > 0)
                                {
                                    mkp.Win = 0;
                                    mkp.FlagUpdate = true;
                                }

                                //Проверка точности
                                if (mkp.Player1 != null && mkp.Player2 != null && !mkp.Compare(1, 10))
                                {
                                    //Неточная
                                    if (mkp.Player1.Team.Id != mkp.Player2.Team.Id && mkp.Win != 2)
                                    {
                                        mkp.Win = 2;
                                        mkp.FlagUpdate = true;
                                    }

                                    //Точная
                                    if (mkp.Player1.Team.Id == mkp.Player2.Team.Id && mkp.Win != 1)
                                    {
                                        mkp.Win = 1;
                                        mkp.FlagUpdate = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void GetScoreBullet(int video_second_bullet, out int team1_score, out int team2_score)
        {
            team1_score = 0;
            team2_score = 0;

            List<Uniso.InStat.Marker> mlist = null;
            lock (Markers)
                mlist = Markers
                    .Where(o
                        => (o.Half.Index == 255 && o.TimeVideo <= video_second_bullet + 100)
                        && !o.FlagDel
                        && o.Compare(8, 1))
                    .OrderBy(o => o.Half.Index)
                    .ThenBy(o => o.TimeVideo)
                    .ThenBy(o => o.Id)
                    .ToList<Uniso.InStat.Marker>();

            team1_score = mlist.Count(o => (o.Team1 != null && o.Team1.Id == Match.Team1Native.Id));
            team2_score = mlist.Count(o => (o.Team2 != null && o.Team1.Id == Match.Team2Native.Id));
        }

        public override void GetScore(Half half, int video_second, out int team1_score, out int team2_score)
        {
            team1_score = 0;
            team2_score = 0;

            Uniso.InStat.Marker mkend = null;
            List<Uniso.InStat.Marker> mlist = null;
            lock (Markers)
            {
                mlist = Markers
                    .Where(o
                        => (o.Half.Index < half.Index || (o.Half.Index == half.Index && o.TimeVideo <= video_second + 100))
                        && o.Half.Index != 255
                        && !o.FlagDel
                        && o.Compare(8, 1))
                    .OrderBy(o => o.Half.Index)
                    .ThenBy(o => o.TimeVideo)
                    .ThenBy(o => o.Id)
                    .ToList<Uniso.InStat.Marker>();

                if (Markers.Exists(o => o.Compare(18, 4)))
                    mkend = Markers.First(o => o.Compare(18, 4));
            }

            team1_score = GetScoreTeam(mlist.Where(o => (o.Team1 != null && o.Team1.Id == Match.Team1Native.Id)).ToList<Uniso.InStat.Marker>());
            team2_score = GetScoreTeam(mlist.Where(o => (o.Team1 != null && o.Team1.Id == Match.Team2Native.Id)).ToList<Uniso.InStat.Marker>());

            if (mkend != null && mkend.Half.Index == half.Index && video_second >= mkend.TimeVideo)
            { 
                int team1_score_bullet, team2_score_bullet;
                GetScoreBullet(video_second, out team1_score_bullet, out team2_score_bullet);
                if (team1_score_bullet > team2_score_bullet)
                    team1_score++;
                if (team1_score_bullet < team2_score_bullet)
                    team2_score++;
            }
        }

        private static int GetScoreTeam(List<Uniso.InStat.Marker> list)
        {
            return list.Count(o => o.Compare(8, 1));
        }

        public StageEnum GetNextStage(InStat.Marker mk, List<StageEnum> canceled)
        {
            var list = new List<Uniso.InStat.Marker>();
            list.Add(mk);
            list.AddRange(((Game.Marker)mk).flag_adding);

            return GetNextStage(list, canceled);
        }

        public override StageEnum GetNextStage(List<InStat.Marker> list, List<StageEnum> canceled)
        {
            var stages = GetStages(list);
            
            if (stages.Contains(StageEnum.Player1) && list[0].Player1 == null)
                return StageEnum.Player1;

            if (stages.Contains(StageEnum.Player2) && list[0].Player2 == null && !canceled.Contains(StageEnum.Player2))
                return StageEnum.Player2;

            if (stages.Contains(StageEnum.Player2Gk) && list[0].Player2 == null)
                return StageEnum.Player2Gk;

            if (stages.Contains(StageEnum.Point) && list[0].Point1.IsEmpty)
                return StageEnum.Point;

            if (stages.Contains(StageEnum.PointAndDest) && (list[0].Point1.IsEmpty || list[0].Point2.IsEmpty))
                return StageEnum.PointAndDest;

            if (stages.Contains(StageEnum.ExtraOptions))
            {
                try
                {
                    if ((list[0].Compare(3, 1) 
                        || list[0].Compare(8, 1) 
                        || list[0].Compare(6, 2) 
                        || list[0].Compare(12) 
                        || list[0].Compare(2, 11)) 
                        && !list[0].ExtraOptionsExists)
                        return StageEnum.ExtraOptions;
                }
                catch
                { }
            }

            return StageEnum.CreateMarker;
        }

        public override List<StageEnum> GetStages(List<InStat.Marker> list)
        {
            var res = new List<StageEnum>();

            //Игрок
            if (list.Exists(o 
                => o.ActionId != 18 
                && !o.Compare(3, 8) 
                && !o.Compare(6, 2) 
                && !o.Compare(2, 11) 
                && o.ActionId != 12))
                res.Add(StageEnum.Player1);

            var pen_no_opp = false;
            var pen = list.FirstOrDefault(o => o.Compare(3, 1));
            if (pen != null && pen.Compare(3, 1))
            {
                if (GetSiblings(pen).Any(o => o.Compare(9, new int[] { 8, 11 })))
                {
                    pen_no_opp = true;
                }
            }

            //Оппонент
            if (!pen_no_opp 
                && list.Exists(o => o.Compare(1, new int[] { 1/*, 3, 4, 5, 6, 7, 8*/ })
                || (o.Compare(1, 6) && o.Win == 2)
                || o.Compare(2, new int[] { 1, 2, 6, 9, 10 })
                || o.Compare(3, new int[] { 1, 2 })
                || o.Compare(4, 3)
                || o.Compare(6, 1)
                || o.ActionId == 14))
                res.Add(StageEnum.Player2);

            //Оппонент - ГК
            if (list.Exists(o => o.Compare(4, new int[] { 1, 2, 4, 5, 6 })
                || o.ActionId == 8))
                res.Add(StageEnum.Player2Gk);

            //Только точка действия
            if (list.Exists(o => (o.Compare(1, new int[] { 1, 2, 3, 4, 5, 7, 8, 9 }) && o.Win == 0)
                || (o.Compare(1, 6) && o.Win != 2)
                || o.Compare(2, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })
                || o.Compare(3, new int[] { 1, 2 })
                || o.Compare(6, 1)
                || o.Compare(7, 1)
                || o.Compare(4)
                || o.Compare(2, 11)
                || o.Compare(8, 1)))
                res.Add(StageEnum.Point);

            //Точка действия и назначения
            if (list.Exists(o => o.Compare(4, new int[] { 2, 3, 4, 5 })
                || (o.Compare(1, 6) && o.Win == 2)
                || (o.Compare(1, new int[] { 1, 2, 3, 4, 5, 7, 8, 9 }) && o.Win == 2)
                || (o.Compare(1, new int[] { 3, 4, 5, 7 }) && ((Game.Marker)o).flag_icing)
                || (o.Compare(1, 6) && ((Game.Marker)o).flag_icing)
                || (o.Compare(3, 9))))
            {
                res.Remove(StageEnum.Point);
                res.Add(StageEnum.PointAndDest);
            }

            //Дополнительные опции
            if (list.Exists(o => o.Compare(3, 1))
                || list.Exists(o => o.ActionId == 12)
                || list.Exists(o => o.Compare(8, 1))
                || list.Exists(o => o.Compare(2, 11))
                || list.Exists(o => o.Compare(6, 2)))
                res.Add(StageEnum.ExtraOptions);

            return res;
        }

        public override List<Uniso.InStat.Marker> GetPrevousMarkers(Half half, int time)
        {
            var res = new List<Uniso.InStat.Marker>();
            IOrderedEnumerable<Uniso.InStat.Marker> mlist = null;
            lock (Markers)
            {
                mlist = Markers
                    .Where(o
                        => (o.Half.Index < half.Index || (o.Half.Index == half.Index && o.TimeVideo < time))
                        && !o.FlagDel
                        && (o.ActionId != 18 && o.ActionId != 16 && o.ActionId != 9 && o.ActionId != 5 && o.ActionId != 14 && o.ActionId != 12 && o.ActionId != 6 && !o.Compare(2, 9)))
                    .OrderBy(o => o.Half.Index)
                    .ThenBy(o => o.TimeVideo)
                    .ThenBy(o => o.Id);

                if (mlist.Count() > 0)
                {
                    var mk = mlist.LastOrDefault();
                    res.AddRange(mlist.Where(o => o.Half == mk.Half && o.TimeVideo == mk.TimeVideo).ToList<Uniso.InStat.Marker>());
                }
            }

            return res;
        }

        public override List<Uniso.InStat.Marker> GetNextMarkers(Half half, int time)
        {
            IOrderedEnumerable<Uniso.InStat.Marker> mlist = null;
            lock (Markers)
            {
                mlist = Markers
                    .Where(o
                        => (o.Half.Index == half.Index && o.TimeVideo > time)
                        && !o.FlagDel
                        && (o.ActionId != 18 && o.ActionId != 16 && o.ActionId != 9 && o.ActionId != 5 && o.ActionId != 14 && o.ActionId != 12 && o.ActionId != 6))
                    .OrderByDescending(o => o.TimeVideo)
                    .ThenBy(o => o.Id);
            }

            var res = new List<Uniso.InStat.Marker>();
            if (mlist.Count() > 0)
            {
                var mk = mlist.Last();
                res.AddRange(mlist.Where(o => o.Half == mk.Half && o.TimeVideo == mk.TimeVideo).ToList<Uniso.InStat.Marker>());
            }

            return res;
        }

        public List<Uniso.InStat.Marker> GetPrevousMarkersHalf(Half half, int time, bool and_current)
        {
            List<Uniso.InStat.Marker> mlist = null;

            if (and_current)
                time += 50;

            lock (Markers)
                mlist = Markers
                    .Where(o
                        => (o.Half.Index == half.Index && o.TimeVideo < time)
                        && !o.FlagDel
                        && !o.Compare(new int[] { 18, 9, 5, 14, 12 }))
                    .OrderBy(o => o.Half.Index)
                    .ThenBy(o => o.TimeVideo)
                    .ThenBy(o => o.Id)
                    .ToList<Uniso.InStat.Marker>();

            var res = new List<Uniso.InStat.Marker>();
            if (mlist.Count > 0)
            {
                var mk = mlist[mlist.Count - 1];
                res.AddRange(mlist.Where(o => o.Half == mk.Half && o.TimeVideo == mk.TimeVideo).ToList<Uniso.InStat.Marker>());
            }

            return res;
        }

        public class TacticsDataException : Exception
        {
            public TacticsDataException(String msg)
                : base(msg)
            { }
        }

        public void CheckValidTactics(Half half, int ms)
        {
            CheckValidTactics(Match.Team1, half, ms);
            CheckValidTactics(Match.Team2, half, ms);
        }

        public void CheckValidTactics(Uniso.InStat.Team tm, Half half, int ms)
        {
            List<Uniso.InStat.Marker> finePlayers;
            List<Place> finePlaces;
            var t = GetTactics(tm, half, ms, out finePlayers, out finePlaces);
            if (t.GetPlayers().Count == half.MaxPlayersNum)
                throw new TacticsDataException(String.Format("В команде {0} не установлены игроки", tm.Name));
        }

        public InStat.Marker GetLastMarkerAttack(Half half, int video_time)
        {
            lock (Markers)
            {
                if (Markers.Where(o => !o.FlagDel && o.Half.Index == half.Index && o.TimeVideo < video_time).Any(o => o.ActionId == 12))
                    return Markers
                        .Where(o => !o.FlagDel && o.Half.Index == half.Index && o.TimeVideo < video_time)
                        .OrderByDescending(o => o.TimeVideo)
                        .First(o => o.ActionId == 12);
            }

            return null;
        }

        public InStat.Marker GetNextMarkerAttack(Half half, int video_time)
        {
            lock (Markers)
            {
                if (Markers.Where(o => !o.FlagDel && o.Half.Index == half.Index && o.TimeVideo > video_time).Any(o => o.ActionId == 12))
                    return Markers
                        .Where(o => !o.FlagDel && o.Half.Index == half.Index && o.TimeVideo > video_time)
                        .OrderBy(o => o.TimeVideo)
                        .First(o => o.ActionId == 12);
            }

            return null;
        }

        public InStat.Marker GetLastMarkerChange(Half half, int video_time, Player player)
        {
            lock (Markers)
                return Markers
                    .Where(o => !o.FlagDel && o.Half.Index == half.Index && o.TimeVideo <= video_time)
                    .OrderByDescending(o => o.TimeVideo)
                    .First(o => o.ActionId == 14 && o.Player1 != null && o.Player1.Id == player.Id);
        }


        public Tactics GetTactics(Uniso.InStat.Team tm, Half half, int ms, out List<Uniso.InStat.Marker> finePlayers, out List<Place> finePlaces)
        {
            finePlayers = new List<Uniso.InStat.Marker>();
            finePlaces = new List<Place>();

            List<Uniso.InStat.Marker> mlist = null;
            lock (Markers)
                mlist = Markers
                .Where(o
                    => (o.Team1 == tm || o.Team2 == tm)
                    && (o.Half.Index == half.Index && o.TimeVideo <= ms)
                    && (o.ActionId == 14)
                    && (!o.FlagDel))
                .OrderByDescending(o => o.Half.Index).ThenByDescending(o => o.TimeVideo).ThenByDescending(o => o.Id)
                .ToList<Uniso.InStat.Marker>();

            Tactics t = null;
            if (half.Index <= 3)
            {
                t = TacticsPresetList[0].Clone();
            }
            else
            {
                switch (half.MaxPlayersNum)
                {
                    case 5 + 1:
                        t = TacticsPresetList[0].Clone();
                        break;

                    case 4 + 1:
                        t = TacticsPresetList[1].Clone();
                        break;

                    case 3 + 1:
                        t = TacticsPresetList[2].Clone();
                        break;

                    default:
                        t = TacticsPresetList[0].Clone();
                        break;
                }
            }

            t.Name = "0";

            foreach (var place in t.Places)
            {
                var place_code = place.GetCode();

                if (mlist.Exists(o => o.Compare(14, place_code) && (o.Player1 != null || o.Player2 != null)))// && o.Player1 != null))
                {
                    var mk = mlist.First(o => o.Compare(14, place_code));// && o.Player1 != null);
                    var pt = t.GetPlace(this, place_code);
                    if (pt != null)
                    {
                        pt.Player = mk.Player1;
                    }
                }               
            }

            var actual_time_total = GetActuialTime(half, ms, true);

            //Штрафы
            lock (Markers)
                mlist = Markers
                .Where(o
                    => (o.Team1 == tm)
                    && (o.TimeActualTotal <= actual_time_total)
                    && (o.Compare(5, new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }))
                    && (!o.FlagDel))
                .OrderByDescending(o => o.TimeActualTotal)
                .ToList<Uniso.InStat.Marker>();

            var res = new Dictionary<Player, List<Uniso.InStat.Marker>>();
            foreach (Marker mk in mlist.Where(o => o.Compare(5, new int[] { 2, 3, 4, 5 }) && o.Team1 != null))
            {
                InStat.Marker mknext = null;
                if (mlist.Any(o => o.ActionId == 5 && o.player1_id == mk.player1_id))
                    mknext = mlist.First(o => o.ActionId == 5 && o.player1_id == mk.player1_id);

                if (mk.Player1 != null && !res.ContainsKey(mk.Player1) && (mknext == null || mknext.Compare(5, new int[] { 2, 3, 4, 5 })))
                {
                    res.Add(mk.Player1, GetSiblings(mk)
                        .Where(o => o.ActionId == 5 && o.Player1 == mk.Player1)
                        .ToList<Uniso.InStat.Marker>());
                }
            }
            
            //Коррекция расстановки
            foreach (var player in res.Keys)
            {
                var mklist = res[player];
                if (mklist.Count == 0)
                    continue;

                try
                {
                    var mkf = mklist.First();
                    var mkf_act_time = mkf.TimeActualTotal;

                    lock (Markers)
                    {
                        if (Markers.Where(o => o.TimeActualTotal >= mkf.TimeActualTotal)
                            .OrderBy(o => o.TimeActualTotal)
                            .Any(o => IsStopTimeMarker(o)))
                        {
                            var mks = Markers.Where(o => o.TimeActualTotal >= mkf.TimeActualTotal)
                                .OrderBy(o => o.TimeActualTotal)
                                .First(o => IsStopTimeMarker(o));

                            mkf_act_time = mks.TimeActualTotal;
                        }
                    }

                    var ft = GetFineTime(mklist);

                    var fine_end = mkf_act_time + ft;

                    if (fine_end > actual_time_total)
                    {
                        foreach (Marker mk in mklist)
                        {
                            if (!finePlayers.Contains(mk))
                            {
                                //TRUE если ушел с площадки
                                mk.Tag = GetSiblings(mk).Any(o => o.ActionId == 14 && o.player2_id == mk.player1_id);
                                finePlayers.Add(mk);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteException(ex);
                }
            }

            foreach (var place in t.Places)
            {
                if (place.Player != null && finePlayers.Exists(o => o.Player1 == place.Player && ((bool)o.Tag)))
                {
                    place.Player = null;
                }
            }

            return t;
        }

        public Dictionary<int, TacticsTime> GetTacticsTime(Team tm, Half half, int ms)
        {
            var res = new Dictionary<int, TacticsTime>();

            if (half == null)
                return res;

            List<Uniso.InStat.Marker> mlist = null;
            lock (Markers)
                mlist = Markers
                .Where(o
                    => (o.Team1 == tm)
                    && (o.Half.Index < half.Index || (o.Half.Index == half.Index/* && o.TimeVideo <= ms*/))
                    && (o.ActionId == 16)
                    && (!o.FlagDel))
                .OrderBy(o => o.Half.Index)
                .ThenBy(o => o.TimeVideo)
                .ThenBy(o => o.Id)
                .ToList<Uniso.InStat.Marker>();

            var ttlist = new List<TacticsTime>();
            foreach (Marker mk in mlist)
            {
                var num = mk.ActionType / 100;
                if (!ttlist.Exists(o => o.Half.Index == mk.Half.Index && o.Time == mk.TimeVideo && o.Num == num))
                    ttlist.Add(new TacticsTime(mk.Half, mk.TimeVideo, num));

                try
                {
                    var tt = ttlist.First(o => o.Half == mk.Half && o.Time == mk.TimeVideo && o.Num == num);
                    tt.Add(mk);
                }
                catch
                { }
            }
            
            if (ttlist.Count > 0)
            {
                var num_max = ttlist.Max(o => o.Num);
                for (var n = 0; n <= num_max; n++)
                {
                    if (ttlist.Exists(o => o.Num == n))
                    {
                        var ttl = ttlist
                            .OrderBy(o => o.Half.Index)
                            .ThenBy(o => o.Time)
                            .Last(o => o.Num == n);

                        res.Add(n, ttl);
                    }
                }
            }

            return res;
        }

        public override bool IsStopTimeMarker(Uniso.InStat.Marker mk)
        {
            return mk.Compare(3, 8)
                || mk.Compare(4, 5)
                || mk.Compare(6, 2)
                || mk.Compare(18, new int[] {3, 4})
                || mk.Compare(8, 1);
        }

        public override bool IsRestoreTimeMarker(Uniso.InStat.Marker mk)
        {
            return mk.Compare(1, 1);
        }

        public List<Uniso.InStat.Marker> GetFoulEmpty(Half half, int video_second)
        {
            lock (Markers)
                return Markers
                    .Where(o => !o.FlagDel
                        && o.Half.Index == half.Index
                        && o.TimeVideo <= video_second
                        && o.Compare(3, 1)
                        && o.Player1 == null)
                    .OrderByDescending(o => o.TimeVideo)
                    .ToList<Uniso.InStat.Marker>();
        }

        public int GetActuialTime(Half half, int video_second)
        {
            return GetActuialTime(half, video_second, false);
        }

        public override int GetActuialTime(Half half, int video_second, bool total)
        {
            if (half == null)
                return 0;

            List<Uniso.InStat.Marker> list = null;
            lock (Markers)
                list = Markers
                    .Where(o => !o.FlagDel 
                        && o.Half.Index == half.Index)
                 .OrderBy(o => o.Half.Index)
                 .ThenBy(o => o.TimeVideo)
                 .ToList<Uniso.InStat.Marker>();

            var offset = 0;
            Marker rest = null;
            foreach (Marker mk in list)
            {
                if (mk.TimeVideo > video_second)
                    break;

                if (IsRestoreTimeMarker(mk))
                {
                    rest = mk;
                }

                if (rest == null)
                {
                    mk.TimeActual = offset;
                    continue;
                }

                if (IsStopTimeMarker(mk))
                {
                    offset = mk.TimeActual;
                    rest = null;
                }
            }

            if (rest != null)
            {
                offset = video_second - rest.TimeVideo + offset;
            }

            if (total)
            {
                var offset_half = 0;
                foreach (var h in HalfList)
                {
                    if (h.Index == half.Index)
                        break;

                    offset_half += h.Length;
                }

                offset += offset_half;
            }

            return offset;
        }

        public override void RecalcActualTime(List<Uniso.InStat.Marker> mkl, Half half)
        {
            //вынос, удар мимо, выход мяча, гол, фол, автогол
            var list = mkl
                .Where(o => !o.FlagDel && o.ActionId != 16)
                .OrderBy(o => o.Half.Index)
                .ThenBy(o => o.TimeVideo)
                .ToList<Uniso.InStat.Marker>();

            var hnum = 0;
            var offset = 0;
            Marker rest = null;
            var offset_half = 0;

            foreach (Marker mk in list)
            {
                if (mk.TimeVideo == -1)
                    continue;

                if (hnum != mk.Half.Index)
                {
                    hnum = mk.Half.Index;
                    offset = 0;
                    rest = null;

                    offset_half = 0;
                    foreach (var h in HalfList)
                    {
                        if (h.Index == hnum)
                            break;

                        offset_half += h.Length;
                    }
                }

                if (IsRestoreTimeMarker(mk))
                {
                    rest = mk;
                }

                if (rest == null)
                {
                    if (!mk.FlagGuiUpdate)
                    {
                        var u = mk.TimeActual != offset;
                        if (u)
                            mk.FlagGuiUpdate = true;
                    }

                    mk.TimeActual = offset;
                    mk.TimeActualTotal = offset + offset_half;
                        
                    continue;
                }

                if (!mk.FlagGuiUpdate)
                {
                    var u = mk.TimeActual != (mk.TimeVideo - rest.TimeVideo + offset);
                    if (u)
                        mk.FlagGuiUpdate = true;
                }

                mk.TimeActual = mk.TimeVideo - rest.TimeVideo + offset;
                mk.TimeActualTotal = mk.TimeActual + offset_half;

                if (IsStopTimeMarker(mk))
                {
                    offset = mk.TimeActual;
                    rest = null;
                }
            }

            foreach (Marker mk in list)
            {
                if (mk.TimeVideo == -1)
                    continue;

                if (mk.TimeActual < 0)
                {
                    if (!mk.FlagGuiUpdate)
                    {
                        var u = mk.TimeActual != 0;
                        if (u)
                            mk.FlagGuiUpdate = true;
                    }

                    mk.TimeActual = 0;
                    mk.TimeActualTotal = 0;
                }
            }
        }

        public class CheckValidMarkerException : Exception
        {
            public enum CheckValidLevelEnum
            { 
                WARNING,
                CRITICAL
            }

            public CheckValidLevelEnum CheckValidLevel { get; set; }
            public int Code { get; set; }

            public CheckValidMarkerException(String msg, CheckValidLevelEnum level)
                : base(msg)
            {
                CheckValidLevel = level;
            }

            public CheckValidMarkerException(String msg, CheckValidLevelEnum level, int code)
                : base(msg)
            {
                CheckValidLevel = level;
                Code = code;
            }
        }

        public override void CheckValid(Uniso.InStat.Marker mk)
        {
            if (mk.FlagDel)
                return;

            if (mk.TimeVideo == -1)
                return;

            //if (mk.Compare(3, 8))
                //return;

            if (mk.Half.Index <= 4)
            {
                if (IsStopTimeMarker(mk) && !mk.Compare(18, new int[] { 3, 4 }))
                {
                    var mkl = GetLastControlTimeMarker(mk.Half, mk.TimeVideo - 100);

                    if (mkl == null)
                        throw new CheckValidMarkerException("Нет маркера вбрасывания",
                            CheckValidMarkerException.CheckValidLevelEnum.WARNING);

                    if (IsStopTimeMarker(mkl))
                        throw new CheckValidMarkerException("Второй подряд маркер остановки чистого времени!",
                            CheckValidMarkerException.CheckValidLevelEnum.WARNING);
                }

                if (IsRestoreTimeMarker(mk))
                {
                    var mkl = GetLastControlTimeMarker(mk.Half, mk.TimeVideo - 100);

                    if (mkl != null && IsRestoreTimeMarker(mkl))
                        throw new CheckValidMarkerException("Второй подряд маркер вбрасывания!",
                            CheckValidMarkerException.CheckValidLevelEnum.WARNING);
                }
            }

            //Атаки
            if (mk.ActionId == 12)
            {
                //начало атаки
                if (mk.ActionType > 1)
                {
                    if (mk.Team1 == null)
                        throw new CheckValidMarkerException("В маркере не указана атакующая команда!", 
                            CheckValidMarkerException.CheckValidLevelEnum.WARNING);

                    if (mk.Team2 == null)
                        throw new CheckValidMarkerException("В маркере не указана обороняющаяся команда!", 
                            CheckValidMarkerException.CheckValidLevelEnum.WARNING);

                    /*InStat.Marker att = GetLastMarkerAttack(mk.Half, mk.TimeVideo);
                    if (att != null && att.ActionType > 1)
                        throw new CheckValidMarkerException("Второй подряд маркер начала атаки! Завершите предыдущую атаку.", 
                            CheckValidMarkerException.CheckValidLevelEnum.CRITICAL);

                    InStat.Marker att2 = GetNextMarkerAttack(mk.Half, mk.TimeVideo);
                    if (att2 == null || att2.ActionType > 1)
                        throw new CheckValidMarkerException("Эта атака не завершена!", 
                            CheckValidMarkerException.CheckValidLevelEnum.WARNING);*/
                }
                else
                {
                    var att = GetLastMarkerAttack(mk.Half, mk.TimeVideo);
                    if (att == null || att.ActionType == 1)
                        throw new CheckValidMarkerException("Не найден маркер начала атаки!", 
                            CheckValidMarkerException.CheckValidLevelEnum.CRITICAL);
                }
            }

            //Замены
            if (mk.ActionId == 14)
            {
                if (mk.Num == 1)
                {
                    throw new CheckValidMarkerException("Начат эпизод замен. Его необходимо завершить!",
                        CheckValidMarkerException.CheckValidLevelEnum.WARNING);
                }
                else
                {
                    if (mk.player1_id == 0 && mk.player2_id == 0)
                        throw new CheckValidMarkerException("В маркере не указан ни игрок, ни оппонент!",
                            CheckValidMarkerException.CheckValidLevelEnum.CRITICAL);

                    if (mk.player1_id == mk.player2_id)
                        throw new CheckValidMarkerException("В маркере не указан один и тот же игрок!",
                            CheckValidMarkerException.CheckValidLevelEnum.CRITICAL);

                    if (mk.team1_id != 0 && mk.team2_id != 0 && mk.team1_id != mk.team2_id)
                        throw new CheckValidMarkerException("В маркере не указанs игроки из разных команд!",
                            CheckValidMarkerException.CheckValidLevelEnum.CRITICAL);

                    InStat.Marker mk0 = null;
                        
                    lock (Markers)
                        mk0 = Markers
                         .Where(o => !o.FlagDel && o.TimeVideo > -1 && o.ActionId == 14 && o.Num == 1 && o.Half.Index == mk.Half.Index && o.TimeVideo < mk.TimeVideo)
                         .OrderBy(o => o.TimeVideo).FirstOrDefault();

                    var time_min = mk0 != null ? mk0.TimeVideo : 0;

                    if (mk.player2_id > 0)
                    {
                        lock (Markers)
                        {
                            //Смененный
                            var ami = Markers
                                .Where(o => !o.FlagDel && o.TimeVideo > -1 && o.ActionId == 14
                                    && (mk.player2_id == o.player1_id || mk.player2_id == o.player2_id)
                                    && o.Half.Index == mk.Half.Index                                    
                                    && o.TimeVideo < mk.TimeVideo)
                                    .OrderByDescending(o => o.TimeVideo);

                            if (ami.Count() == 0)
                                throw new CheckValidMarkerException("Не найден предыдущий маркер. Ошибка в расстановке!",
                                    CheckValidMarkerException.CheckValidLevelEnum.CRITICAL);

                            var amii = ami.ToList<InStat.Marker>();

                            var mkp = ami.First();

                            if (mk.player1_id > 0 && mkp.player2_id == mk.player2_id)
                            {
                                //Проверка пары
                                var pair_check = true;
                                if (ami.Any(o => o.Half == mkp.Half && o.TimeVideo == mkp.TimeVideo && o != mkp))
                                {
                                    var mkp2 = ami.First(o => o.Half == mkp.Half && o.TimeVideo == mkp.TimeVideo && o != mkp);
                                    pair_check = mkp2.player2_id == mk.player2_id;
                                }

                                if (pair_check)
                                    throw new CheckValidMarkerException(String.Format("Игрок {2} уже был сменен в маркере ID={0} {1}!",
                                        mkp.Id, Utils.TimeFormat(mkp.TimeVideo), mk.Player2), CheckValidMarkerException.CheckValidLevelEnum.CRITICAL);
                            }

                            //Выходящий
                            if (mk.player1_id > 0)
                            {
                                ami = Markers
                                    .Where(o => !o.FlagDel && o.TimeVideo > -1 && o.ActionId == 14
                                        && (o.team1_id == mk.team2_id || o.team2_id == mk.team2_id)
                                        && (mk.player1_id == o.player1_id || mk.player1_id == o.player2_id)
                                        && o.Half.Index == mk.Half.Index
                                        && o.TimeVideo > time_min
                                        && o.TimeVideo < mk.TimeVideo)
                                        .OrderByDescending(o => o.TimeVideo);

                                amii = ami.ToList<InStat.Marker>();

                                if (ami.Count() > 0)
                                {
                                    mkp = ami.First();

                                    if (mkp.player1_id == mk.player1_id)
                                        throw new CheckValidMarkerException(String.Format("Игрок {2} уже был выпущен в маркере ID={0} {1}!",
                                            mkp.Id, Utils.TimeFormat(mkp.TimeVideo), mk.Player1), CheckValidMarkerException.CheckValidLevelEnum.CRITICAL);
                                }
                            }

                            var sibl = GetSiblings(mk.Half, mk.TimeVideo);
                            sibl.Remove(mk);

                            if (mk.player1_id > 0)
                            {
                                if (sibl.Any(o => o.ActionId == 14
                                    && (o.player1_id == mk.player1_id || o.player2_id == mk.player1_id)))
                                    throw new CheckValidMarkerException(String.Format("Выходящий игрок {2} одновременно и в замене ID={0} {1}!",
                                        mkp.Id, Utils.TimeFormat(mkp.TimeVideo), mk.Player1), CheckValidMarkerException.CheckValidLevelEnum.CRITICAL);
                            }

                            if (mk.player2_id > 0)
                            {
                                if (sibl.Any(o => o.ActionId == 14
                                    && (o.player1_id == mk.player2_id || o.player2_id == mk.player2_id)))
                                    throw new CheckValidMarkerException(String.Format("Уходящий игрок {2} одновременно и в замене ID={0} {1}!",
                                        mkp.Id, Utils.TimeFormat(mkp.TimeVideo), mk.Player2), CheckValidMarkerException.CheckValidLevelEnum.CRITICAL);
                            }
                        }
                    }
                }
            }
            else
            {
                if ((mk.Compare(1, new int[] { 3, 4, 5, 6, 7, 8 }) 
                    || mk.Compare(2) || mk.Compare(7, 1) || mk.Compare(8, 1)
                    || mk.Compare(3, new int[] { 1, 9 }) 
                    || mk.Compare(4, new int[] { 1, 2, 3, 4, 5, 6 })) 
                    && mk.Point1.IsEmpty)
                    throw new CheckValidMarkerException("В маркере нет точки действия!",
                        CheckValidMarkerException.CheckValidLevelEnum.WARNING, 2);

                if (mk.ActionId != 10)
                {
                    var prev = GetPrevousMarkersHalf(mk.Half, mk.TimeVideo, false);
                    var sibl = GetSiblings(mk);

                    if (IsInsertPickUp(mk))
                    {
                        if (!sibl.Any(o => o.Compare(2, 5)))
                            throw new CheckValidMarkerException("Перед маркером должен быть подбор!",
                                CheckValidMarkerException.CheckValidLevelEnum.WARNING, 1);
                    }
                    else
                    {
                        if (!mk.Compare(2, new int[] { 5, 9 }) && sibl.Any(o => o.Compare(2, 5)))
                            throw new CheckValidMarkerException("Перед маркером не должено быть подбора!",
                                CheckValidMarkerException.CheckValidLevelEnum.WARNING, 0);
                    }
                }
                
                //Единоборство
                if (mk.Compare(2, new int[] { 1, 10 }))
                {
                    if (mk.team1_id == 0)
                        throw new CheckValidMarkerException("Не указан игрок, выигравший единоборство!",
                            CheckValidMarkerException.CheckValidLevelEnum.CRITICAL, 0);

                    if (mk.team2_id == 0)
                        throw new CheckValidMarkerException("Не указан игрок, проигравший единоборство!",
                            CheckValidMarkerException.CheckValidLevelEnum.CRITICAL, 0);

                    if (mk.team1_id > 0 && mk.team2_id > 0 && mk.team2_id == mk.team1_id)
                        throw new CheckValidMarkerException("В маркере указаны игроки из одной команды!",
                            CheckValidMarkerException.CheckValidLevelEnum.CRITICAL, 0);
                }

                if (mk.Compare(1, 1))
                {
                    if (mk.team1_id > 0 && mk.team2_id > 0 && mk.team2_id == mk.team1_id)
                        throw new CheckValidMarkerException("В маркере указаны игроки из одной команды!",
                            CheckValidMarkerException.CheckValidLevelEnum.CRITICAL, 0);
                }

                //Проверка удаления при штрафе
                if (mk.ActionId == 5 && mk.ActionType > 1 && mk.Num == 0)
                {
                    lock (Markers)
                    {
                        if (!Markers.Any(o => 
                               !o.FlagDel
                            && o.Half.Index == mk.Half.Index 
                            && o.TimeVideo > mk.TimeVideo - 100
                            && o.TimeVideo < mk.TimeVideo + 100 
                            && o.ActionId == 14 
                            && o.player2_id == mk.player1_id))
                        {
                            throw new CheckValidMarkerException("Штраф был с удалением, но маркер удаления не найден", 
                                CheckValidMarkerException.CheckValidLevelEnum.WARNING);
                        }
                    }
                }

                //Проверка штрафа
                if (!mk.Compare(3, 1) && mk.ActionId != 16 && mk.ActionId != 5 && mk.ActionId != 9 && mk.Half.Index < 255)
                {
                    List<Uniso.InStat.Marker> finePlayers;
                    List<Place> finePlaces;

                    if (mk.Player1 != null)
                    {
                        var t = GetTactics(mk.Team1, mk.Half, mk.TimeVideo - 50, out finePlayers, out finePlaces);
                        if (finePlayers.Any(o => o.player1_id == mk.Player1.Id))
                            throw new CheckValidMarkerException(String.Format("Игрок, указанный в маркере ID={0} {1} оштрафован!",
                                mk.Id, Utils.TimeFormat(mk.TimeVideo)), CheckValidMarkerException.CheckValidLevelEnum.WARNING);

                        if (!t.GetPlayers().Any(o => o.Id == mk.Player1.Id))
                            throw new CheckValidMarkerException(String.Format("Игрок, указанный в маркере ID={0} {1} отсутствует в текущей расстановке!", 
                                mk.Id, Utils.TimeFormat(mk.TimeVideo)), CheckValidMarkerException.CheckValidLevelEnum.WARNING);
                    }

                    if (mk.Player2 != null)
                    {
                        var t = GetTactics(mk.Team2, mk.Half, mk.TimeVideo - 50, out finePlayers, out finePlaces);
                        if (finePlayers.Any(o => o.player1_id == mk.Player2.Id))
                            throw new CheckValidMarkerException(String.Format("Оппонент, указанный в маркере ID={0} {1} оштрафован!", 
                                mk.Id, Utils.TimeFormat(mk.TimeVideo)), CheckValidMarkerException.CheckValidLevelEnum.WARNING);

                        if (!t.GetPlayers().Any(o => o.Id == mk.Player2.Id))
                            throw new CheckValidMarkerException(String.Format("Оппонент, указанный в маркере ID={0} {1} отсутствует в текущей расстановке!", 
                                mk.Id, Utils.TimeFormat(mk.TimeVideo)), CheckValidMarkerException.CheckValidLevelEnum.WARNING);
                    }
                }

                //Проверка точности передач
                if (mk.Compare(1, new int[] { 3, 4, 5, 7, 8, 9 }))
                {
                    if (mk.team2_id == 0 && mk.Win < 2)
                        throw new CheckValidMarkerException(String.Format("В маркере отсутствует оппонент!",
                            mk.Id, Utils.TimeFormat(mk.TimeVideo)), CheckValidMarkerException.CheckValidLevelEnum.WARNING);

                    if (mk.Win == 0)
                        throw new CheckValidMarkerException(String.Format("В маркере не указана точность!", 
                            mk.Id, Utils.TimeFormat(mk.TimeVideo)), CheckValidMarkerException.CheckValidLevelEnum.WARNING);

                    if (mk.Win == 1 && mk.team1_id != mk.team2_id)
                        throw new CheckValidMarkerException(String.Format("Маркер должен быть не точный!",
                            mk.Id, Utils.TimeFormat(mk.TimeVideo)), CheckValidMarkerException.CheckValidLevelEnum.WARNING);

                    if (mk.Win == 2 && mk.team1_id == mk.team2_id)
                        throw new CheckValidMarkerException(String.Format("Маркер должен быть точный!",
                            mk.Id, Utils.TimeFormat(mk.TimeVideo)), CheckValidMarkerException.CheckValidLevelEnum.WARNING);
                }

                if (mk.Compare(18, new int[] { 2, 8, 5, 7 }) && mk.Half.Index <= 4)
                {
                    lock (Markers)
                    {
                        if (!Markers.Any(o => o.Half.Index == mk.Half.Index - 1 && o.Compare(18, 3)))
                        {
                            throw new CheckValidMarkerException("Не установлени маркер окончания предыдущего периода!", 
                                CheckValidMarkerException.CheckValidLevelEnum.WARNING);
                        }
                    }
                }

                if (Options.G.Game_FindActualTimeForHalfFinally 
                    && mk.Half.Index <= 3 && mk.Compare(18, new int[] { 3, 4 }) 
                    && (mk.TimeActual > mk.Half.Length + 200 || mk.TimeActual < mk.Half.Length - 200))
                    throw new CheckValidMarkerException("У маркера окончания периода неверное время!", 
                        CheckValidMarkerException.CheckValidLevelEnum.CRITICAL);

            }

            if (mk.ActionId == 18 || mk.ActionId == 16 || mk.ActionId == 14 || mk.ActionId == 12 || mk.Compare(3, 8))
                return;

            if (mk.Player1 == null)
                throw new CheckValidMarkerException("В маркере не указан игрок!", 
                    CheckValidMarkerException.CheckValidLevelEnum.WARNING);
        }

        public List<PointF> GetDumpInPoints(RectangleF rc)
        {
            var ptlist = new List<PointF>();

            var center_x = FieldSize.Width / 2.0f;
            var center_y = FieldSize.Height / 2.0f;

            ptlist.Add(new PointF(center_x, center_y));

            var left = 4.0f;
            if (GameType == GameTypeEnum.NHL)
                left = 3.35f;

            var right = FieldSize.Width - left;

            if (GameType == GameTypeEnum.Euro)
            {
                ptlist.Add(new PointF(10.0f, 8.0f));
                ptlist.Add(new PointF(10.0f, FieldSize.Height - 8.0f));
                ptlist.Add(new PointF(FieldSize.Width - 10.0f, 8.0f));
                ptlist.Add(new PointF(FieldSize.Width - 10.0f, FieldSize.Height - 8.0f));
            }

            if (GameType == GameTypeEnum.NHL)
            {
                ptlist.Add(new PointF(left + 6.10f, center_y - 6.71f));
                ptlist.Add(new PointF(left + 6.10f, center_y + 6.71f));
                ptlist.Add(new PointF(right - 6.10f, center_y - 6.71f));
                ptlist.Add(new PointF(right - 6.10f, center_y + 6.71f));
            }

            var circlex1 = left + 18.86f + 1.5f;
            var circlex2 = right - 18.86f - 1.5f;

            if (GameType == GameTypeEnum.NHL)
            {
                circlex1 = left + 19.51f + 1.52f;
                circlex2 = right - 19.51f - 1.52f;
            }

            ptlist.Add(new PointF(circlex1, center_y - 6.71f));
            ptlist.Add(new PointF(circlex1, center_y + 6.71f));
            ptlist.Add(new PointF(circlex2, center_y - 6.71f));
            ptlist.Add(new PointF(circlex2, center_y + 6.71f));

            return ptlist;
        }

        public override PointF TransformScreenToBase(RectangleF rc, Point pt, SizeF sz, Uniso.InStat.Marker mk, bool mirror)
        {
            var ptf = base.TransformScreenToBase(rc, pt, sz, mk, mirror);
            if (mk != null && mk.Compare(1, 1))
            {
                var ptlist = GetDumpInPoints(rc);
                ptf = ptlist.OrderBy(o => Math.Sqrt(Math.Pow(o.X - ptf.X, 2) + Math.Pow(o.Y - ptf.Y, 2))).First();
            }

            return ptf;
        }

        public override String GetZone(PointF pt, bool mirror)
        {
            pt = new PointF(pt.X, FieldSize.Height - pt.Y);
            var rc = new RectangleF(0, 0, FieldSize.Width, FieldSize.Height);
            var rcc = new RectangleF(4.0f, 0, FieldSize.Width - 8.0f, FieldSize.Height);

            if (pt.X < rcc.X)
                return "1G";

            if (pt.X > rcc.Right)
                return "3G";

            double dx = rcc.Width / 3.0f;
            double dy = rcc.Height / 3.0f;

            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    if (pt.X - rcc.Left >= dx * x && pt.X - rcc.Left <= dx * (x + 1) && pt.Y >= dy * y && pt.Y <= dy * (y + 1))
                    {
                        var h = (mirror ? 3 - x : x + 1);
                        var w = String.Empty;
                        switch (y)
                        {
                            case 0:
                                w = "L";
                                break;
                            case 1:
                                w = "C";
                                break;
                            case 2:
                                w = "R";
                                break;
                        }

                        return h.ToString() + w;
                    }
                }
            }

            return String.Empty;
        }

        public override void DrawField(Gdi.GDICompatible gdi, System.Drawing.RectangleF rc)
        {
            var dx = rc.Width / FieldSize.Width;
            var dy = rc.Height / FieldSize.Height;
            var c = new Point(Convert.ToInt32(rc.X + rc.Width / 2.0f), Convert.ToInt32(rc.Y + rc.Height / 2.0f));
            var rci = new Rectangle(
                Convert.ToInt32(rc.Left), 
                Convert.ToInt32(rc.Top), 
                Convert.ToInt32(rc.Width), 
                Convert.ToInt32(rc.Height));

            gdi.Pen.Width = 1;
            gdi.Brush.Style = Gdi.BrushStyle.bsSolid;

            gdi.Pen.Color = 0x00000000;
            gdi.Brush.Color = 0x00ffffff;
            gdi.RoundRect(rci, Convert.ToInt32(15f * dx), Convert.ToInt32(15f * dx));
            
            gdi.Brush.Style = Gdi.BrushStyle.bsClear;
            gdi.Pen.Color = 0x000000ff;
            gdi.Pen.Width = 3;

            //Центральная линия
            gdi.MoveTo(c.X, rci.Y);
            gdi.LineTo(c.X, rci.Bottom);

            var goal_line = GameType == GameTypeEnum.Euro ? 4.0f : 3.35f;

            var left = rci.X + ConvToV(goal_line, dx);
            var right = rci.Right - ConvToV(goal_line, dx);

            var blue_line = ConvToV(GameType == GameTypeEnum.Euro ? 18.86f : 19.51f, dx);

            //Линия ворот
            gdi.MoveTo(left, rci.Y + ConvToV(1.0f, dy));
            gdi.LineTo(left, rci.Bottom - ConvToV(1.0f, dy));
            gdi.MoveTo(right, rci.Y + ConvToV(1.0f, dy));
            gdi.LineTo(right, rci.Bottom - ConvToV(1.0f, dy));

            gdi.Pen.Color = 0x00ff0000;
            gdi.Pen.Width = 2;

            //синие линии
            gdi.MoveTo(left + blue_line, rci.Y);
            gdi.LineTo(left + blue_line, rci.Bottom);
            gdi.MoveTo(right - blue_line, rci.Y);
            gdi.LineTo(right - blue_line, rci.Bottom);

            gdi.Pen.Width = 2;

            var circlex1 = rci.X + ConvToV(10.0f, dx);
            var circlex2 = rci.Right - ConvToV(10.0f, dx);

            var circley1 = rci.Top + ConvToV(8.0f, dy);
            var circley2 = rci.Bottom - ConvToV(8.0f, dy);

            if (GameType == GameTypeEnum.NHL)
            {
                circlex1 = left + ConvToV(6.10f, dx);
                circlex2 = right - ConvToV(6.10f, dx);

                circley1 = c.Y + ConvToV(6.71f, dy);
                circley2 = c.Y - ConvToV(6.71f, dy);
            }

            DrawDumpInZone(gdi, new Point(circlex1, circley1), dx, dy, false, false);
            DrawDumpInZone(gdi, new Point(circlex1, circley2), dx, dy, false, false);
            DrawDumpInZone(gdi, new Point(circlex2, circley1), dx, dy, false, false);
            DrawDumpInZone(gdi, new Point(circlex2, circley2), dx, dy, false, false);

            DrawDumpInZone(gdi, c, dx, dy, true, false);

            circlex1 = left + blue_line + ConvToV(1.5f, dx);
            circlex2 = right - blue_line - ConvToV(1.5f, dx);

            if (GameType == GameTypeEnum.NHL)
            {
                circlex1 = left + blue_line + ConvToV(1.52f, dx);
                circlex2 = right - blue_line - ConvToV(1.52f, dx);
            }

            DrawDumpInZone(gdi, new Point(circlex1, circley1), dx, dy, false, true);
            DrawDumpInZone(gdi, new Point(circlex1, circley2), dx, dy, false, true);
            DrawDumpInZone(gdi, new Point(circlex2, circley1), dx, dy, false, true);
            DrawDumpInZone(gdi, new Point(circlex2, circley2), dx, dy, false, true);

            gdi.Pen.Width = 1;
            gdi.Pen.Color = 0x000000ff;
            gdi.Brush.Color = 0x00ff0000;

            var rcg1 = GetGoalRect(rc, 0);
            var rcg1i = new Rectangle(Convert.ToInt32(rcg1.X), Convert.ToInt32(rcg1.Y), Convert.ToInt32(rcg1.Width), Convert.ToInt32(rcg1.Height));
            gdi.Rectangle(rcg1i);

            var goalzone = new Rectangle(Convert.ToInt32(rcg1i.X),
                Convert.ToInt32(rcg1i.Y + rcg1i.Height / 2 - ConvToV(1.22f, dy)), 
                Convert.ToInt32(ConvToV(1.22f, dx)), Convert.ToInt32(ConvToV(2.44f, dx)));

            gdi.Rectangle(goalzone);

            var rcg2 = GetGoalRect(rc, 1);
            var rcg2i = new Rectangle(Convert.ToInt32(rcg2.X), Convert.ToInt32(rcg2.Y), Convert.ToInt32(rcg2.Width), Convert.ToInt32(rcg2.Height));
            gdi.Rectangle(rcg2i);

            goalzone = new Rectangle(Convert.ToInt32(rcg2i.X),
                Convert.ToInt32(rcg2i.Y + rcg2i.Height / 2 - ConvToV(1.22f, dy)),
                Convert.ToInt32(ConvToV(1.22f, dx)), Convert.ToInt32(ConvToV(2.44f, dx)));

            gdi.Rectangle(goalzone);

            gdi.Pen.Color = 0x00000000;
        }

        private void DrawDumpInZone(Gdi.GDICompatible gdi, Point pt, float dx, float dy, bool inCenter, bool woCircle)
        {
            var color = inCenter ? 0x00ff0000 : 0x000000ff;

            var d = GameType == GameTypeEnum.NHL ? 4.5f : 4.57f;

            var rx = ConvToV(d, dx);
            var ry = ConvToV(d, dy);
            var rxc = ConvToV(1.0f, dx);
            var ryc = ConvToV(1.0f, dy);
            var rcr = new Rectangle(pt.X - rx, pt.Y - ry, rx + rx, ry + ry);
            var rcc = new Rectangle(pt.X - rxc, pt.Y - ryc, rxc + rxc, ryc + ryc);

            gdi.Pen.Width = 1;
            if (!woCircle)
            {
                gdi.Pen.Color = color;
                gdi.Brush.Style = Gdi.BrushStyle.bsClear;
                gdi.Ellipse(rcr);
            }
            gdi.Brush.Style = Gdi.BrushStyle.bsSolid;
            gdi.Pen.Color = 0x00ffffff;
            gdi.Brush.Color = color;
            gdi.Ellipse(rcc);
        }

        private int ConvToV(float meters, float dx)
        {
            return Convert.ToInt32(meters * dx);
        }

        public override void DrawField(System.Drawing.Graphics g, System.Drawing.RectangleF rc)
        {
            
        }

        public override RectangleF GetGoalRect(RectangleF rc, int index)
        {
            var dx = rc.Width / FieldSize.Width;
            var dy = rc.Height / FieldSize.Height;

            var goal_line = GameType == GameTypeEnum.Euro ? 4.0f : 3.35f;

            var x1 = ConvToV(goal_line, dx);

            var rc3 = new RectangleF(0.0f, 0.0f, GoalSize.Width / 2.0f * dx, GoalSize.Width * dy);
            rc3 = new RectangleF(rc.X + x1, rc3.Top + rc.Y + (rc.Height - rc3.Height) / 2, rc3.Width, rc3.Height);

            if (index == 0)
                return rc3;

            return new RectangleF(rc.Right - x1 - rc3.Width, rc3.Top, rc3.Width, rc3.Height);
        }

        public override InStat.Marker CreateMarkerBegin(Half half, int video_second)
        {
            return new Game.Marker(this, 18, half.ActionType)
            {
                Half = half,
                TimeVideo = video_second
            };
        }
        

        public override void Remove(InStat.Marker mk)
        {
            base.Remove(mk);

            if (mk.Compare(18, 9))
            {
                var sync_ch = UpdateSync(Half);
                if (sync_ch)
                {
                    lock (Markers)
                        foreach (Marker mki in Markers.Where(o => !o.FlagDel && o.Half.Index == Half.Index && o.Sync == 1))
                            mki.FlagGuiUpdate = true;
                }
            }

            lock (Markers)
                RecalcActualTime(Markers, null);
        }

        public void RefreshOvertimes()
        {
            lock (Markers)
            {
                if (Markers.Exists(o => o.Compare(18, 5) && o.Half.Index == 4))
                {
                    var mkh = Markers.First(o => o.Compare(18, 5) && o.Half.Index == 4);
                    var maxplayers = mkh.Num % 10000;

                    var length = mkh.Num / 10000;
                    if (length == 0 || maxplayers == 0)
                    {
                        if (length == 0)
                            length = 5;

                        if (maxplayers == 0)
                            maxplayers = 4;

                        mkh.Num = length * 10000 + maxplayers;
                        mkh.FlagUpdate = true;
                    }

                    foreach (var half in HalfList.Where(o => o.Index >= 4 && o.Index < 255))
                    {
                        half.MaxPlayersNum = maxplayers + 1;
                        half.Periods[0].Length = length * 60000;
                    }
                }
            }
        }

        public MatchKind Kind { get; set; }
        public static MatchKind KIND_1 = new MatchKind { Id = 1, Name = "Регулярный чемпионат" };
        public static MatchKind KIND_2 = new MatchKind { Id = 2, Name = "Play-off" };

        public HockeyIce(GameTypeEnum gt, MatchKind kind)
        {
            SyncTime = new Dictionary<int, int>();

            Kind = kind;
            GameType = gt;

            Name = "Хоккей с шайбой";
            IsSetScreenPoints = true;
            IsEvaluateStrikes = true;
            MaxPlayersNum = 6;
            MaxPlayersChanges = 0;
            GoalSize = new System.Drawing.SizeF(1.83f, 1.22f);
            FieldSize = new System.Drawing.SizeF(60.0f, 30.0f);

            MapA = new List<Amplua>()
            { 
                AMPLUA_GK,
                AMPLUA_Z,
                AMPLUA_F,
            };

            MapP = new List<Position>() 
            { 
                POSITION_L,
                POSITION_C,
                POSITION_R,
            };

            HalfList = new List<Half>();

            HalfList.Add(
                new Half(new List<Period> { new Period { Index=1, Length = 20 * 60000 } })
            {
                Index = 1,
                ActionType = 1,
                Name = "Period 1",
                MaxPlayersNum = 6,
                MinPlayersNum = 4,
            });

            HalfList.Add(
                new Half(new List<Period> { new Period { Index = 2, Length = 20 * 60000 } })
            {
                Index = 2,
                ActionType = 2,
                Name = "Period 2",
                MaxPlayersNum = 6,
                MinPlayersNum = 4,
            });

            HalfList.Add(
                new Half(new List<Period> { new Period { Index = 3, Length = 20 * 60000 } })
            {
                Index = 3,
                ActionType = 8,
                Name = "Period 3",
                MaxPlayersNum = 6,
                MinPlayersNum = 4,
            });

            for (var i = 0; i < 15; i++)
            {
                HalfList.Add(new Half(
                    new List<Period> { new Period { Index = 4, Length = (kind.Id == 1 ? 5 : 20) * 60000 } })
                {
                    Index = 4 + i,
                    ActionType = 5,
                    Name = String.Format("Overtime #{0}", i + 1),
                    MaxPlayersNum = kind.Id == 1 ? 5 : 6,
                    MinPlayersNum = 4,
                });
            }

            HalfList.Add(new Half(
                new List<Period> { new Period { Index = 5, Length = 0 } })
            {
                Index = 255,
                ActionType = 7,
                Name = "Penalty",
                MaxPlayersNum = 0,
                MinPlayersNum = 0,
            }); 

            TacticsPresetList = new List<Tactics>();
            TacticsPresetList.Add(
                new Tactics(
                    "Default",
                    this,
                    1,
                    new List<Place>()
                            { 
                                new Place { Amplua = AMPLUA_GK, Position = POSITION_C },
                        
                                new Place { Amplua = AMPLUA_Z, Position = POSITION_L },
                                new Place { Amplua = AMPLUA_Z, Position = POSITION_R },

                                new Place { Amplua = AMPLUA_F, Position = POSITION_L },
                                new Place { Amplua = AMPLUA_F, Position = POSITION_C },
                                new Place { Amplua = AMPLUA_F, Position = POSITION_R },
                            }));

            TacticsPresetList.Add(
                new Tactics(
                    "Overtime4",
                    this,
                    2,
                    new List<Place>()
                            { 
                                new Place { Amplua = AMPLUA_GK, Position = POSITION_C },
                        
                                new Place { Amplua = AMPLUA_Z, Position = POSITION_L },
                                new Place { Amplua = AMPLUA_Z, Position = POSITION_R },

                                new Place { Amplua = AMPLUA_F, Position = POSITION_L },
                                new Place { Amplua = AMPLUA_F, Position = POSITION_R },
                            }));

            TacticsPresetList.Add(
                new Tactics(
                    "Overtime3",
                    this,
                    3,
                    new List<Place>()
                            { 
                                new Place { Amplua = AMPLUA_GK, Position = POSITION_C },
                        
                                new Place { Amplua = AMPLUA_F, Position = POSITION_L },
                                new Place { Amplua = AMPLUA_F, Position = POSITION_C },
                                new Place { Amplua = AMPLUA_F, Position = POSITION_R },
                            }));

            DefaultTactics = TacticsPresetList[0];
        }

        public override List<InStat.Marker> CreateTacticsMarkers(Half h, int second)
        {
            return new List<InStat.Marker>();
        }

        public override void FillTactics(List<InStat.Marker> ami)
        {
            
        }

        public static Position POSITION_L = new Position { Id = 1, Name = "Левый", NameShort = "Л" };
        public static Position POSITION_C = new Position { Id = 2, Name = "Центр", NameShort = "Ц" };
        public static Position POSITION_R = new Position { Id = 3, Name = "Правый", NameShort = "П" };

        public static Amplua AMPLUA_GK = new Amplua { Id = 1, Name = "Вратарь", NameShort = "ГК" };
        public static Amplua AMPLUA_Z = new Amplua { Id = 2, Name = "Защитник", NameShort = "З" };
        public static Amplua AMPLUA_F = new Amplua { Id = 3, Name = "Нападающий", NameShort = "Ф" };
    }
}
