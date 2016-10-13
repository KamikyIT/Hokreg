using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
