using System;
using System.Diagnostics;

namespace Uniso.InStat.MediaInfo
{
    public class MediaInfo
    {
        private static String mediaInfoExe = ".\\mediainfo\\MediaInfo.exe";
        private static Process proc = null;

        public static String GetValue(String inFile, String valueName)
        {
            return GetValue(inFile, valueName, "Video");
        }

        public static String GetValue(String inFile, String valueName, String sect)
        {
            value = String.Empty;

            proc = new Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.ErrorDialog = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardError = true;

            var com = " --Inform=" + sect + ";%" + valueName + "% \"" + inFile + "\"";
            proc.StartInfo.Arguments = com;

            proc.StartInfo.FileName = mediaInfoExe;

            proc.OutputDataReceived += HandleMediaPlayerOutputDataReceived;
            proc.ErrorDataReceived += HandleMediaPlayerErrorDataReceived;

            proc.Start();

            Log.Write(proc.StartInfo.FileName);
            Log.Write(proc.StartInfo.Arguments);

            proc.BeginErrorReadLine();
            proc.BeginOutputReadLine();

            proc.WaitForExit();

            return value;
        }

        private static String value = String.Empty;

        private static void HandleMediaPlayerOutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                value = e.Data.ToString();
                Log.Write("OUTPUT >> " + value);
            }
        }

        private static void HandleMediaPlayerErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                value = e.Data.ToString();
                Log.Write("ERROR >> " + value);
            }
        }
    }
}
