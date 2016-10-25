using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Globalization;

namespace Uniso.InStat
{
    public class Ffprobe
    {
        public int Duration { get; set; }
        public float SAR { get; set; }
        public float PAR { get; set; }
        public float DAR { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        private List<Dictionary<String, String>> dict = new List<Dictionary<string, string>>();
        private Dictionary<String, String> tmp = null;

        public Ffprobe(String fileName)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");

            Process proc = new Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.ErrorDialog = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardError = true;

            bool camera = fileName.Contains("-f dshow");

            if (camera)
                proc.StartInfo.Arguments = fileName + " -show_streams";
            else
                proc.StartInfo.Arguments = " -i \"" + fileName + "\" -show_streams";

            proc.StartInfo.FileName = @".\ffmpeg\bin\ffprobe.exe";

            proc.OutputDataReceived += HandleMediaPlayerOutputDataReceived;
            proc.ErrorDataReceived += HandleMediaPlayerErrorDataReceived;

            proc.Start();

            proc.BeginErrorReadLine();
            proc.BeginOutputReadLine();

            proc.WaitForExit();

            foreach (Dictionary<String, String> d in dict)
            {
                if (d.ContainsKey("codec_type") && d["codec_type"].Contains("video"))
                {
                    if (d.ContainsKey("width"))
                        Width = Int32.Parse(d["width"]);
                    if (d.ContainsKey("height"))
                        Height = Int32.Parse(d["height"]);
                    if (!camera && d.ContainsKey("duration"))
                        Duration = Convert.ToInt32(Single.Parse(d["duration"]) * 1000.0f);
                    if (d.ContainsKey("display_aspect_ratio"))
                    {
                        String[] s = d["display_aspect_ratio"].Split(':');
                        if (s.Length == 2)
                            DAR = Single.Parse(s[0]) / Single.Parse(s[1]);
                    }
                }
            }

            if (Width > 0 && Height > 0)
                SAR = (float)Math.Max(Width, Height) / (float)Math.Min(Width, Height);

            //PAR = DAR/SAR.
            if (DAR > 0 && SAR > 0)
                PAR = DAR / SAR;
        }

        private void HandleMediaPlayerOutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                String value = e.Data.ToString();
                //Log.Write("O >> " + value);

                if (value.Contains("[STREAM]"))
                {
                    tmp = new Dictionary<string, string>();
                    dict.Add(tmp);
                    return;
                }
                if (value.Contains("[/STREAM]"))
                {
                    tmp = null;
                    return;
                }
                if (tmp != null && value.Contains("="))
                {
                    String[] str = value.Split(new String[] { "=" }, StringSplitOptions.None);
                    if (str.Length == 2 && !tmp.ContainsKey(str[0]))
                        tmp.Add(str[0], str[1]);
                    return;
                }
            }
        }

        private void HandleMediaPlayerErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                String value = e.Data.ToString();
                //Log.Write("E >> " + value);
            }
        }
    }
}
