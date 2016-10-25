using System;
using System.Windows;
using System.Windows.Threading;

namespace Uniso.InStat.Gui.WPFForms
{
    public class Render
    {
        public static void DoAction(Action a)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Render, a);
            }
            catch
            { }
        }
    }
}