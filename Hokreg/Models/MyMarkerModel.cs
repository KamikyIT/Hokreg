using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniso.InStat.Models
{
    public class MyMarkerModel
    {
        public int id;

        public int action_id;
        public int action_type;
        public int win;

        public string name;
        public string name_eng;

        public bool player1_required;
        public bool player2_required;

        public bool point1_required;
        public bool point2_required;

        public MyMarkerModel(int id, int action_id, int action_type, int win, string name, string name_eng, bool p1, bool p2, bool t1, bool t2)
        {
            this.id = id;

            this.action_id = action_id;
            this.action_type = action_type;
            this.win = win;

            this.name = name;
            this.name_eng = name_eng;

            this.player1_required = p1;
            this.player2_required = p2;

            this.point1_required = t1;
            this.point2_required = t2;
        }

        public bool Compare(int actionId, int actionType, int iwIn)
        {
            return this.action_id == actionId && this.action_type == actionType && this.win == iwIn;
        }
    }
}
