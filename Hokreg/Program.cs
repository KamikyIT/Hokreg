using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Uniso.InStat.Game;
using Uniso.InStat.Gui;
using Uniso.Update;

namespace Uniso.InStat
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Log.IsDebug = true;
            AppInfo.SaveEntryAssemblyVersionToFile();
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

            var time = 12345;
            var s = time / 1000;
            Log.Write(String.Format("{0}:{1}.{2}", (s / 60).ToString("00"), (s % 60).ToString("00"), ((time % 1000) / 100).ToString("0")));

            Uniso.Update.Checker.Start();
            Uniso.Update.Checker.UpdateReady += new EventHandler(Ftp_UpdateReady);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var login = new Login();
            login.ShowDialog();

            if (login.User != null)
            {
                HockeyIce.User = login.User;
                Application.Run(new SelectForm());
            }
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            Uniso.Update.Checker.Stop();
            if (Uniso.Update.Checker.IsReady)
            {
                RunUpdate();
            }
        }

        private static void Ftp_UpdateReady(object sender, EventArgs e)
        {
            var txt = "Загружена новая версия!\nДля установки обновления необходимо выйти из программы.";
            MessageBox.Show(txt, "Update", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private static void RunUpdate()
        {
            AppInfo.Get();
            if (AppInfo.LastVersion != null && Checker.IsReady && !AppInfo.Setup)
            {
                Process.Start(@".\upmod.exe");
            }
        }
    }
}
