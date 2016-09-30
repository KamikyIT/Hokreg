using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Uniso.InStat.Game
{
    public class MyMarker
    {
        public int Action_Id { get; set; }

        public int Action_Type { get; set; }

        public int Win { get; set; }

        public string Name { get; set; }

        public string Name_Eng { get; set; }

        public MyPlayer Player1 { get; set; }

        public MyPlayer Player2 { get; set; }

        public  PointF Point1 { get; set; }

        public  PointF Point2 { get; set; }

        public bool StopTime { get; set; }

        public List<MyMarker> Flags_Addings { get; set; }

        public string Description { get; set; }


        public MyMarker(int action_id, int action_type, int win = 0)
        {
            this.Action_Id = action_id;
            this.Action_Type = action_type;
            this.Win = win;
        }

        public bool Compare(int action_id)
        {
            return this.Action_Id == action_id;
        }

        public bool Compare(int action_id, int action_type)
        {
            return this.Action_Id == action_id && Action_Type == action_type;
        }

        public bool Compare(int action_id, int action_type, int win)
        {
            return Compare(action_id, action_type) && this.Win == win;
        }

        public bool Compare(int action_id, int[] action_types)
        {
            if (action_types == null)
            {
                throw new ArgumentNullException("action_types == null");
            }
            if (action_types.Length == 0)
            {
                throw new ArgumentNullException("action_types.Length == 0");
            }

            return this.Action_Id == action_id && action_types.Contains(this.Action_Type);
        }

        public int Time { get; set; }

        public int FactTime { get; set; }




    }

    public class MyPlayer
    {
        
    }
}
