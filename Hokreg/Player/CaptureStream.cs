using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Uniso.InStat
{
    public class CaptureStream
    {
        private static Process ffmpeg = null;
        private static CancellationTokenSource cts;

        public static void Start(String videoDevice, String audioDevice)
        {
            cts = new CancellationTokenSource();

            // Запускаем асинхронную операцию
            Task.Factory.StartNew(() =>
                DoEncode(videoDevice, audioDevice, cts.Token), 
                cts.Token);
        }

        private static void DoEncode(String videoDevice, String audioDevice, CancellationToken cancellationToken)
        {
            ClearFolder(@".\cache");

            while (true)
            {
                var dir = new DirectoryInfo(@".\cache");
                var segment_start_number = dir.GetFiles().Count(o => o.Name.Contains(".mp4"));

                ffmpeg = new Process();
                ffmpeg.StartInfo.CreateNoWindow = true;
                ffmpeg.StartInfo.UseShellExecute = false;
                ffmpeg.StartInfo.ErrorDialog = false;
                ffmpeg.StartInfo.RedirectStandardOutput = true;
                ffmpeg.StartInfo.RedirectStandardInput = true;
                ffmpeg.StartInfo.RedirectStandardError = true;

                ffmpeg.StartInfo.Arguments = GetCommandLine(videoDevice, audioDevice, ".\\cache", Quality.p720, segment_start_number);
                ffmpeg.StartInfo.FileName = @".\ffmpeg\bin\ffmpeg.exe";

                ffmpeg.ErrorDataReceived += ffmpeg_ErrorDataReceived;
                ffmpeg.OutputDataReceived += ffmpeg_OutputDataReceived;

                ffmpeg.Start();
                ffmpeg.BeginErrorReadLine();
                ffmpeg.BeginOutputReadLine();

                ffmpeg.WaitForExit();
                
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private static void ffmpeg_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Log.Write("OUTPUT: " + e.Data);
        }

        private static void ffmpeg_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Log.Write("ERROR: " + e.Data);
        }

        private static System.Drawing.Point GetWatermarkOffset(Quality quality)
        {
            if (quality == Quality.p720)
                return new System.Drawing.Point(80, 55);

            if (quality == Quality.p400)
                return new System.Drawing.Point(45, 30);

            return new System.Drawing.Point(25, 18);
        }

        private static System.Drawing.Point GetRunStringOffset(Quality quality)
        {
            if (quality == Quality.p720)
                return new System.Drawing.Point(258, 161);

            if (quality == Quality.p400)
                return new System.Drawing.Point(53, 28);

            return new System.Drawing.Point(40, 16);
        }

        private static System.Drawing.Size GetSizeByQuality(Quality quality)
        {
            switch (quality)
            {
                case Quality.p720:
                    return new System.Drawing.Size(1280, 720);
                case Quality.p400:
                    return new System.Drawing.Size(704, 400);
            }

            return new System.Drawing.Size(416, 240);
        }

        private static String GetVideoParametersString(Quality quality)
        {
            var sz = GetSizeByQuality(quality);

            var video_bitrate = "2200k";
            switch (quality)
            {
                case Quality.p400:
                    video_bitrate = "1200k";
                    break;

                case Quality.p240:
                    video_bitrate = "550k";
                    break;
            }

            return " -sn -r 25"
                 + " -acodec libvo_aacenc"
                 + " -ar 22050 -ac 2 -ab 96k"
                 + " -refs 5"
                 + " -vcodec libx264"
                 + " -pix_fmt yuv420p"
                 + " -aspect " + sz.Width + ":" + sz.Height +
                 String.Format(" -b:v {0} -minrate {1} -maxrate {2} -bufsize {3}", video_bitrate, video_bitrate, video_bitrate, video_bitrate);
        }

        private static String GetCommandLine(
            String videoDevice,
            String audioDevice,
            String outPath,
            Quality quality,
            int segment_start_number)
        {
            var yadif = "yadif,";
            var conv = String.Format("0:0", 0);
            var con_filter = String.Empty;

            var res = String.Format("-f dshow -rtbufsize 2004000k -i video=\"{0}\"", videoDevice);
            res += String.Format(" -f dshow -i audio=\"{0}\"", audioDevice);
            res += String.Format(" -i \".\\logo\\logo{0}.png\"", quality.ToString());
            res += " -filter_complex \"" + con_filter;

            var source_par = 1.0;

            var dev_str = String.Format("-f dshow -i video=\"{0}\"", videoDevice);

            var ffprobe = new Ffprobe(dev_str);
            var source_height = ffprobe.Height;
            var source_width = ffprobe.Width;
            
            var target_full = GetSizeByQuality(quality);

            var source_dar = (double)source_width / (double)source_height * source_par;
            var target_dar = (double)target_full.Width / (double)target_full.Height;

            var pad_x = 0;
            var pad_y = 0;
            double target_inner_height = 0;
            double target_inner_width = 0;
            var scale = 0.0;

            if (source_dar < target_dar)
            {
                target_inner_height = target_full.Height;
                scale = (double)target_inner_height / (double)source_height;
                target_inner_width = (double)source_width * scale * source_par;

                if (target_full.Width / 2 == (double)target_full.Width / 2.0)
                {
                    target_inner_width = Math.Round(target_inner_width / 2.0) * 2.0;
                }
                else
                {
                    target_inner_width = Math.Round((target_inner_width - 1) / 2.0) * 2.0 + 1;
                }

                if (target_inner_width < target_full.Width)
                {
                    pad_x = Convert.ToInt32((target_full.Width - target_inner_width) / 2);
                    pad_y = 0;
                }
                else
                {
                    target_inner_width = target_full.Width;
                    pad_x = 0;
                    pad_y = 0;
                }
            }
            else
            {
                target_inner_width = target_full.Width;
                scale = (double)target_inner_width / (double)source_width;
                target_inner_height = Convert.ToInt32((double)source_height * scale / source_par);
                if (target_full.Height / 2 == (double)target_full.Height / 2.0)
                {
                    target_inner_height = Math.Round(target_inner_height / 2.0) * 2.0;
                }
                else
                {
                    target_inner_height = Math.Round((target_inner_height - 1) / 2.0) * 2.0 + 1;
                }

                if (target_inner_height < target_full.Height)
                {
                    //Необходимо добавить поля сверху и снизу
                    //Расчитать ширину каждого поля:
                    pad_x = 0;
                    pad_y = Convert.ToInt32((target_full.Height - target_inner_height) / 2);
                }
                else
                {
                    //Поля не нужны
                    target_inner_height = target_full.Height;
                    pad_x = 0;
                    pad_y = 0;
                }
            }

            var scale_width = Convert.ToInt32(target_inner_width == source_width ? 0 : target_inner_width);
            var scale_height = Convert.ToInt32(target_inner_height == source_height ? 0 : target_inner_height);
            var pad_width = (target_full.Width == target_inner_width ? 0 : target_full.Width);
            var pad_height = (target_full.Height == target_inner_height ? 0 : target_full.Height);

            res += String.Format("[{0}]{1}setsar=1/1", conv, yadif);

            if (scale_width != 0 || scale_height != 0)
                res += ",scale=" + scale_width + ":" + scale_height;
            if (pad_x != 0 || pad_y != 0)
                res += ",pad=" + pad_width + ":" + pad_height + ":" + pad_x + ":" + pad_y + ":black";
            res += "[conv]; ";

            var wmo = GetWatermarkOffset(quality);
            res += String.Format("[conv][{2}:v]overlay={0}:main_h-overlay_h-{1} [mvid]", wmo.X, wmo.Y, 2);

            res += "\"";

            res += GetVideoParametersString(quality);
            res += " -x264opts keyint=25:" +
                    "b-pyramid=normal:no-mixed-refs=1:no-8x8dct=1:no-fast-pskip=" +
                    " -fpre .\\ffmpeg\\presets\\libx264-slow.ffpreset" +
                    " -threads 0 -profile:v baseline";

            res += " -f mp4";
            res += " -metadata title=\"InStat Football video\"";
            res += " -map_chapters -1";
            res += " -map \"[mvid\"]";
            res += " -map 1:0";
            res += " -sn";
            res += String.Format(" -f segment -segment_list \"{0}\\out.m3u8\" -segment_time 5 -segment_start_number {1} -segment_format mp4 \"{0}\\out%06d.mp4",
                outPath, segment_start_number);

            return res;
        }

        public static void Stop()
        {
            if (ffmpeg != null)
                ffmpeg.Kill();

            ffmpeg = null;

            // Запрашиваем отмену операции
            if (cts != null)
                cts.Cancel();
        }

        public static void ClearFolder(String dir)
        {
            var didir = new DirectoryInfo(dir);
            var fis = didir.GetFiles();
            foreach (var fi in fis)
            {
                fi.Delete();
            }
            var dis = didir.GetDirectories();
            foreach (var di in dis)
            {
                ClearFolder(di.FullName);
                Directory.Delete(di.FullName);
            }
        }
    }
}
