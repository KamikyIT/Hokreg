using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uniso.InStat.Game.Hockey;

namespace Uniso.InStat
{
    public class SelectedMarker
    {
        public String Name { get; set; }
        public Func<Game.Marker, bool> Rule { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
