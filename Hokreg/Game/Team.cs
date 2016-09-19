using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniso.InStat.Game
{
    [Serializable()]
    public class Team : Uniso.InStat.Team
    {
        public Team()
        {
            TeamColorsKind = new Dictionary<int, TeamColors>();
            var game = new HockeyIce(HockeyIce.GameTypeEnum.Euro, HockeyIce.KIND_1);
            for (var i = 0; i < 6; i++)
            {
                var t = game.DefaultTactics.Clone();
                t.Name = "5ka-" + i.ToString();
                t.NameActionType = i;
                if (i > 2)
                {
                    var p = t.GetPlace(game, 21);
                    t.Places.Remove(p);
                }

                Tactics.Add(i, t);
            }
        }

        public List<Marker> GetCurrentTacticsMarker(HockeyIce game, Half half, int time, List<int> tactics_num_list)
        {
            if (tactics_num_list.Count == 0)
                tactics_num_list.AddRange(Tactics.Keys);

            var res = new List<Marker>();
            foreach (var i in tactics_num_list)
            {
                if (!Tactics.ContainsKey(i))
                    continue;

                foreach (var place in Tactics[i].Places)
                {
                    if (place.Player == null && i > 0 && tactics_num_list.Count == Tactics.Count)
                        continue;

                    var mk = new Marker(game, 16, place.GetCode() + 100 * i);
                    mk.Half = half;
                    mk.TimeVideo = time;
                    if (place.Player != null)
                    {
                        mk.Player1 = place.Player;
                        mk.Num = place.Player.Number;
                    }
                    else
                        mk.Team1 = this;
                    res.Add(mk);
                }
            }

            return res;
        }
    }
}
