using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniso.InStat
{
    public class Utils
    {
        public static String TimeFormat(long ms)
        {
            var sign = ms >= 0 ? "" : "-";
            ms = Math.Abs(ms);
            var ss = ms / 1000L;
            var m = ss / 60L;
            var s = ss % 60L;
            var mss = (ms % 1000L) / 100;
            return String.Format("{3}{0}:{1}.{2}", m.ToString("00"), s.ToString("00"), mss.ToString("0"), sign);
        }
    }
}
