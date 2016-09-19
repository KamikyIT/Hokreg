using System;

namespace Uniso.InStat.Game
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
