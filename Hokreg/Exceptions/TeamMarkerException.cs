using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniso.InStat
{
    public class TeamMarkerException : Exception
    {
        public TeamMarkerException(String msg)
            : base(msg)
        {
        }
    }
}
