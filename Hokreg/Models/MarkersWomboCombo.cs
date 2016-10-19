using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Uniso.InStat.Gui.Controls;
using Uniso.InStat.Gui.Forms;

namespace Uniso.InStat.Models
{
    public static class MarkersWomboCombo
    {
        /// <summary>
        /// Правило создания дочернего(Marker.flag_adding) маркера после создания исходного маркера.
        /// Короче, когда сразу несколько маркеров создается, вот.
        /// </summary>
        public enum AddChildMarkerRule
        {
            /// <summary>
            /// Не определено мною, оставить то, что было.
            /// </summary>
            None = 0,
            /// <summary>
            /// Просто. Время доп.маркера = время исходного маркера.
            /// </summary>
            //TODO: Пока что не добавляю. оставлю
            //Simple,
            /// <summary>
            /// Время доп.маркера = время исходного + 100 мс.
            /// </summary>
            Add100ms,
        }

        /// <summary>
        /// Возможно ли добавить к prevMarker дополнительный extraMarker.
        /// </summary>
        /// <param name="prevMarker">Исходный маркер, в который можно добавить новый.</param>
        /// <param name="extraMarker">Новый маркер.</param>
        /// <returns>Возвращает true - можно, false - нельзя.</returns>
        public static bool CheckPrevMarkerNeedsExtraMarker(Game.Marker prevMarker, Game.Marker extraMarker)
        {
            #region null and empty marker check

            if (prevMarker == null || extraMarker == null)
            {
                return false;
            }

            if (prevMarker.Compare(0, 0, 0))
            {
                return false;
            }

            #endregion


            #region Перехват 200700

            // Перехват. 200700
            // возможен с:
            // Пас по борту         100300
            // Пас                  100400
            // ОП                   100500
            // Вброс                100800
            // Выброс(+)            100601
            if (prevMarker.Compare(2, 7, 0))
            {
                var res = extraMarker.Compare(1, 3, 0) || extraMarker.Compare(1, 4, 0) ||
                          extraMarker.Compare(1, 5, 0) || extraMarker.Compare(1, 8, 0) ||
                          extraMarker.Compare(1, 6, 1);

                return res;
            }

            #endregion


            #region Единоборство. 200100

            // Единоборство. 200100
            // возможен с:
            // Пас по борту         100300
            // Пас                  100400
            // ОП                   100500
            // Вброс                100800
            // Выброс(+)            100601

            if (prevMarker.Compare(2, 1, 0))
            {
                var res = extraMarker.Compare(1, 3, 0) || extraMarker.Compare(1, 4, 0) ||
                          extraMarker.Compare(1, 5, 0) || extraMarker.Compare(1, 8, 0) ||
                          extraMarker.Compare(1, 6, 1);

                return res;
            }

            #endregion


            #region Отбор 200600

            // Отбор. 200600
            // возможен с:
            // Пас по борту         100300
            // Пас                  100400
            // ОП                   100500
            // Вброс                100800
            // Выброс(+)            100601

            if (prevMarker.Compare(2, 6, 0))
            {
                var res = extraMarker.Compare(1, 3, 0) || extraMarker.Compare(1, 4, 0) ||
                          extraMarker.Compare(1, 5, 0) || extraMarker.Compare(1, 8, 0) ||
                          extraMarker.Compare(1, 6, 1);

                return res;
            }

            #endregion

            #region Отбитый бросок

            // Отбитый бросок               400200
            // возможен с флагами:
            // Фиксация шайбы вратарем      300600
            // Отбивание шайбы вратарем     100900

            if (prevMarker.Compare(4, 2, 0))
            {
                var res = extraMarker.Compare(3, 6, 0) || extraMarker.Compare(1, 9, 0);

                return res;
            }

            #endregion


            return false;
        }

        /// <summary>
        /// Добавить или Убрать в prevMarker дополнительный newMarker. Возможность добавлять СТРОГО один дополнительный маркер.
        /// </summary>
        /// <param name="prevMarker">Исходный маркер, в который можно добавить новый.</param>
        /// <param name="newMarker">Новый маркер.</param>
        public static void AddSingleNewExtraMarker(Game.Marker prevMarker, Game.Marker newMarker)
        {
            if (prevMarker == null || newMarker == null)
            {
                return;
            }

            var alreadyAddedMarker =
                prevMarker.flag_adding.FirstOrDefault(
                    o => o.Compare(newMarker.ActionId, newMarker.ActionType, newMarker.Win));

            // Если уже есть такой маркер, то выпиливаем его.
            if (alreadyAddedMarker != null)
            {
                prevMarker.flag_adding.Remove(alreadyAddedMarker);
            }
            // Иначе очищаем списокдоп.маркеров и запихивает новый маркер.
            else
            {
                prevMarker.flag_adding.Clear();
                prevMarker.flag_adding.Add(newMarker);
            }
        }

        /// <summary>
        /// Получить правило создания дополнительного маркера childMarker в маркере sourceMarker.
        /// </summary>
        /// <param name="sourceMarker">Родительский маркер.</param>
        /// <param name="childMarker">Дочерний маркер в составе родительского.</param>
        /// <returns>Правило создания дополнительного маркер.</returns>
        public static AddChildMarkerRule CheckRuleForeExtraMarker(Game.Marker sourceMarker, Game.Marker childMarker)
        {
            #region Перехват 200700

            // Перехват. 200700
            // добавляется 100 мс.
            // возможен с:
            // Пас по борту         100300
            // Пас                  100400
            // ОП                   100500
            // Вброс                100800
            // Выброс(+)            100601
            if (sourceMarker.Compare(2, 7, 0))
            {
                var isMyRule = childMarker.Compare(1, 3, 0) || childMarker.Compare(1, 4, 0) ||
                          childMarker.Compare(1, 5, 0) || childMarker.Compare(1, 8, 0) ||
                          childMarker.Compare(1, 6, 1);

                return isMyRule ? AddChildMarkerRule.Add100ms : AddChildMarkerRule.None;
            }

            #endregion


            #region Единоборство 200100

            // Единоборство. 200100
            // добавляется 100 мс.
            // возможен с:
            // Пас по борту         100300
            // Пас                  100400
            // ОП                   100500
            // Вброс                100800
            // Выброс(+)            100601
            if (sourceMarker.Compare(2, 1, 0))
            {
                var isMyRule = childMarker.Compare(1, 3, 0) || childMarker.Compare(1, 4, 0) ||
                               childMarker.Compare(1, 5, 0) || childMarker.Compare(1, 8, 0) ||
                               childMarker.Compare(1, 6, 1);

                return isMyRule ? AddChildMarkerRule.Add100ms : AddChildMarkerRule.None;
            }

            #endregion


            #region Единоборство 200600

            // Отбор. 200600
            // добавляется 100 мс.
            // возможен с:
            // Пас по борту         100300
            // Пас                  100400
            // ОП                   100500
            // Вброс                100800
            // Выброс(+)            100601
            if (sourceMarker.Compare(2, 6, 0))
            {
                var isMyRule = childMarker.Compare(1, 3, 0) || childMarker.Compare(1, 4, 0) ||
                               childMarker.Compare(1, 5, 0) || childMarker.Compare(1, 8, 0) ||
                               childMarker.Compare(1, 6, 1);

                return isMyRule ? AddChildMarkerRule.Add100ms : AddChildMarkerRule.None;
            }

            #endregion

            return AddChildMarkerRule.None;
        }

        /// <summary>
        /// Получить список действий для маркеров.
        /// </summary>
        /// <param name="markers">Список маркеров.</param>
        /// <returns>Возвращает список действий, если список маркеров есть. Иначе null.</returns>
        public static List<StageEnum> GetMarkersStage(List<Marker> markers)
        {
            return null;

            // TODO: вечнопроблемы.

            if (markers.Count == 1)
            {
                return GetMarkersStage((Game.Marker)markers[0]);
            }


            return null;

        }

        /// <summary>
        /// Получить список действий для маркера.
        /// </summary>
        /// <param name="marker">Список маркеров.</param>
        /// <returns>Возвращает список действий, если маркер есть. Иначе null.</returns>
        public static List<StageEnum> GetMarkersStage(Game.Marker marker)
        {
            var action_id = marker.ActionId;
            var action_type = marker.ActionType;
            var win = marker.Win;

            var markerModel = markerModels.FirstOrDefault(x => x.Compare(action_id, action_type, win));

            if (markerModel != null)
            {
                return GetMyMarkerStage(markerModel);
            }

            return null;

        }

        private static List<StageEnum> GetMyMarkerStage(MyMarkerModel markerModel)
        {

            var res = new List<StageEnum>();

            if (markerModel.player1_required)
            {
                res.Add(StageEnum.Player1);
            }


            if (markerModel.player2_required)
            {
                res.Add(StageEnum.Player2);
            }

            if (markerModel.point1_required)
            {
                if (markerModel.point2_required)
                {
                    res.Add(StageEnum.PointAndDest);
                }
                else
                {
                    res.Add(StageEnum.Point);
                }
            }

            return res;
        }

        static MarkersWomboCombo()
        {

            ParseFoulStages();

            ParseMarkerRules();

        }


        /// <summary>
        /// Распарсить правила и требуемые действия(StageEnum) для маркеров фолов.
        /// </summary>
        private static void ParseFoulStages()
        {
            FoulStagesDictionary = new Dictionary<int, FoulStageRulesModel>();

            var lines = Properties.Resources.foul_marker_rules.Split('\n');

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var parts = line.Split(new char[] {'\r', '\t', '\n'});

                // 1		Прочее												p0	p1	p2  t0  t1

                if (parts.Length < 3)
                {
                    continue;
                }

                var action_type = int.Parse(parts[0]);

                var name = parts[2];

                var p0 = parts.Any(x => x == "p0");
                var p1 = parts.Any(x => x == "p1");
                var p2 = parts.Any(x => x == "p2");

                var t0 = parts.Any(x => x == "t0");
                var t1 = parts.Any(x => x == "t1");

                FoulStagesDictionary.Add(action_type, new FoulStageRulesModel(action_type, name, p0, p1, p2, t0, t1));
            }


        }

        public class FoulStageRulesModel
        {
            public int ActionType;

            public string Name;

            public bool Player0;
            public bool Player1;
            public bool Player2;

            public bool Point0;
            public bool Point1;

            public FoulStageRulesModel(int action_type, string name, bool p0, bool p1, bool p2, bool t0, bool t1)
            {
                this.ActionType = action_type;
                this.Name = name;
                this.Player0 = p0;
                this.Player1 = p1;
                this.Player2 = p2;

                this.Point0 = t0;
                this.Point1 = t1;
            }

            /// <summary>
            /// Получить НОВЫЙ ОБЪЕКТ список вообще всех возможных действий для заполнения данного маркера.
            /// Далее из полученного можно удалять, по ситуации.
            /// </summary>
            /// <returns></returns>
            public List<FoulStageEnum> GetPossibleStages()
            {
                var res = new List<FoulStageEnum>();

                // Если без игроков.
                if (Player0 && !Player1 && !Player2)
                    res.Add(FoulStageEnum.Player0);

                if (this.Player1)
                    res.Add(FoulStageEnum.Player1);

                if (this.Player2)
                {

                    res.Add(FoulStageEnum.Player1);
                    res.Add(FoulStageEnum.Player2);
                }

                if (Point0)
                    res.Add(FoulStageEnum.Point0);

                if (Point1)
                    res.Add(FoulStageEnum.Point1);

                return res;
            }

            public static FoulStageEnum Convert(StageEnum st)
            {
                switch (st)
                {
                    case StageEnum.Standard:
                        break;
                    case StageEnum.Body:
                        break;
                    case StageEnum.Player1:
                        return FoulStageEnum.Player1;
                        break;
                    case StageEnum.Player2:
                        return FoulStageEnum.Player2;
                        break;
                    case StageEnum.Player2Gk:
                        break;
                    case StageEnum.Point:
                        return FoulStageEnum.Point1;
                        break;
                    case StageEnum.PointAndDest:
                        break;
                    case StageEnum.Marking:
                        break;
                    case StageEnum.ScreenPosition:
                        break;
                    case StageEnum.GoalLocation:
                        break;
                    case StageEnum.ExtraOptions:
                        break;
                    case StageEnum.CreateMarker:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(st), st, null);
                }

                return FoulStageEnum.None;
            }

            internal static HockeyGui.ModeEnum Convert(FoulStageEnum foulStageEnum)
            {
                switch (foulStageEnum)
                {
                    case FoulStageEnum.None:
                        return HockeyGui.ModeEnum.View;
                        break;
                    case FoulStageEnum.Player0:
                        return HockeyGui.ModeEnum.View;
                        break;
                    case FoulStageEnum.Player1:
                        return HockeyGui.ModeEnum.SelectPlayer;
                        break;
                    case FoulStageEnum.Player2:
                        return HockeyGui.ModeEnum.SelectPlayer;
                        break;
                    case FoulStageEnum.Point0:
                        return HockeyGui.ModeEnum.View;
                        break;
                    case FoulStageEnum.Point1:
                        return HockeyGui.ModeEnum.SelectPoint;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(foulStageEnum), foulStageEnum, null);
                }
            }
        }

        public enum FoulStageEnum
        {
            None,

            Player0,
            Player1,
            Player2,

            Point0,
            Point1,
        }

        /// <summary>
        /// Распарсить правила создания маркеров из файла "marker_rules.txt" ресурсов.
        /// </summary>
        private static void ParseMarkerRules()
        {
            var marker_rules = Properties.Resources.marker_rules;

            markerModels = new List<MyMarkerModel>();

            var lines = marker_rules.Split('\n');

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var parts = line.Split(new string[] { ",\t" }, StringSplitOptions.None);

                if (parts.Length < 10)
                {
                    continue;
                }

                var i = 0;

                var include = parts[i];
                i++;
                if (include != "x")
                {
                    continue;
                }

                var idstr = parts[i];
                i++;
                var id = 0;
                if (int.TryParse(idstr, out id) == false)
                {
                    continue;
                }

                var action_id_str = parts[i];
                i++;
                var action_id = 0;
                if (int.TryParse(action_id_str, out action_id) == false)
                {
                    continue;
                }

                var action_type_str = parts[i];
                i++;
                var action_type = 0;
                if (int.TryParse(action_type_str, out action_type) == false)
                {
                    continue;
                }

                var win_str = parts[i];
                i++;
                var win = 0;
                if (int.TryParse(win_str, out win) == false)
                {
                    if (string.IsNullOrEmpty(win_str) == false)
                    {
                        continue;
                    }
                }

                var name = parts[i];
                i++;

                var name_eng = parts[i];
                i++;

                //player	opponent	point	dest
                var player = parts[i] == "v" ? true : false;
                i++;

                var opponent = parts[i] == "v" ? true : false;
                i++;

                var point = parts[i] == "v" ? true : false;
                i++;

                var dest = parts[i] == "v" ? true : false;
                i++;



                var markerModel = new MyMarkerModel(id, action_id, action_type, win, name, name_eng, player, opponent, point, dest);

                markerModels.Add(markerModel);

                //x,	100601,	1,	6,	1,	Выброс(+),	,	v,	v,	,	,	,	,	Выброс удачный, ,   ,
            }
        }

        private static List<MyMarkerModel> markerModels;

        /// <summary>
        /// Добавить новый ИЛИ убрать предыдущее вхождение нового маркера ИЛИ переключить на новый дочерний маркер в prevMarker дополнительный newMarker. Возможность добавлять СТРОГО один дополнительный маркер.
        /// </summary>
        /// <param name="marker">Исходный маркер, в который можно добавить новый.</param>
        /// <param name="extraMarker">Новый дочерний маркер.</param>
        public static void XorAddSingleNewExtraMarker(Game.Marker marker, Game.Marker extraMarker)
        {
            // Если еще не пустой.
            if (marker.flag_adding.Any())
            {
                // Проверяем, если пытаемся добавить тот же самый, то вырубаем его.
                if (marker.flag_adding.Any(x => x.Compare(extraMarker.ActionId, extraMarker.ActionType, extraMarker.Win)))
                {
                    marker.flag_adding.Clear();
                }
                // Иначе в нем был какой-то другой.
                else
                {
                    marker.flag_adding.Clear();

                    marker.flag_adding.Add(extraMarker);
                }
            }
            // Иначе, если пустой.
            else
            {
                marker.flag_adding.Add(extraMarker);
            }

        }

        internal static List<FoulStageEnum> GetFoulMarkerPossibleStages(FoulMarkerModel foulMarkerModel)
        {
            return GetFoulMarkerPossibleStages(foulMarkerModel.ActionType);
        }

        internal static List<FoulStageEnum> GetFoulMarkerPossibleStages(int action_type)
        {
            var foulStageRules = FoulStagesDictionary.FirstOrDefault(x => x.Value.ActionType == action_type).Value;

            return foulStageRules.GetPossibleStages();
        }


        private static Dictionary<int, FoulStageRulesModel> FoulStagesDictionary { get; set; }
    }


    public enum EditFoulMarkerActionEnum
    {
        None = 0x0,
        Player0,
        Player1,
        Player2,
        Point0,
        Point1,
    }

    public class FoulMarkerModel
    {
        public int ActionType;

        public MyViolationForm.FoulTypeEnum FoulPlayersCount;

        public bool Pair;

        internal List<MarkersWomboCombo.FoulStageEnum> FoulStages;

        public FoulMarkerModel()
        {
            
        }

        private FoulMarkerModel(int foul, MyViolationForm.FoulTypeEnum foulCounTypeEnum, bool pair)
        {
            this.ActionType = foul;

            this.FoulPlayersCount = foulCounTypeEnum;

            this.Pair = pair;
        }

        //public void SetStages(IEnumerable<StageEnum> stages)
        //{
        //    Stages = new List<StageEnum>(stages);
        //}

        public List<Game.Marker> GenerateMarkers(Game.Marker copyMarkerInfo)
        {
            var res = new List<Game.Marker>();

            if (Pair)
            {

                Game.Marker mk1 = null;
                Game.Marker.CopyMarkerData(copyMarkerInfo, out mk1);

                mk1.ActionId = 9;
                mk1.ActionType = this.ActionType;
                mk1.Win = 0;

                res.Add(mk1);

                // Другие игроки, поменялись местами!
                Game.Marker mk2 = null;
                Game.Marker.CopyMarkerData(copyMarkerInfo, out mk2);

                mk2.ActionId = 9;
                mk2.ActionType = this.ActionType;
                mk2.Win = 0;
                // меняем игроков.
                var player = mk2.Player1;
                mk2.Player1 = mk2.Player2;
                mk2.Player2 = player;

                res.Add(mk2);
            }
            else
            {
                Game.Marker mk = null;
                Game.Marker.CopyMarkerData(copyMarkerInfo, out mk);

                mk.ActionId = 9;
                mk.ActionType = this.ActionType;
                mk.Win = 0;

                res.Add(mk);
            }

            return res;
        }
    }

}
