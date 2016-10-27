using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Uniso.InStat.Coders;

namespace Uniso.InStat.StreamPlayer
{
    /// <summary>
    /// Логика взаимодействия для StreamVideoPlayerWpf.xaml
    /// </summary>
    public partial class StreamVideoPlayerWpf : UserControl
    {
        public StreamVideoPlayerWpf()
        {
            InitializeComponent();

            Render.Init();

            mediaElement1.Clock = null;
            mediaElement2.Clock = null;

            timer1 = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(100)};
            timer1.Tick += timer1_Tick;
            timer1.Start();

            mediaElement1.MediaEnded += mediaElement1_MediaEnded;
            mediaElement1.MediaOpened += mediaElement1_MediaOpened;
            mediaElement1.LoadedBehavior = MediaState.Manual;
            mediaElement1.ScrubbingEnabled = true;

            mediaElement2.MediaEnded += mediaElement1_MediaEnded;
            mediaElement2.MediaOpened += mediaElement1_MediaOpened;
            mediaElement2.LoadedBehavior = MediaState.Manual;
            mediaElement2.ScrubbingEnabled = true;

            InitializePlayer();
        }

        #region Fields

        private bool buffering = false;
        private DispatcherTimer timer1;
        private long needPosition = 0L;
        private PlayerMode mode = PlayerMode.Stop;
        private int currentScene = -1;
        private bool closed = true;
        private int _num = 0;
        private long _pos = 0;
        private bool _set_on_pause = false;
        private bool openScene = false;
        private bool isSwitchingFragments = false;
        private bool curr_player = true;
        private Thickness margin;
        private bool block_opened = false;
        private Thread thread = null;
        private List<Scene> scenes = new List<Scene>();
        private List<Scene> upscs = new List<Scene>();
        private Ffprobe ffprobe = null;
        private bool done = false;
        long old_pos = 0L;

        #endregion

        #region Properties

        public Size VideoSize { get; set; }
        public int BufferTime { get; set; }

        public string CacheDirectory
        {
            get { return Environment.CurrentDirectory + @"\cache\"; }
        }

        [Browsable(false)]
        public long CurrentConnectorSecond { get; set; }

        [Browsable(false)]
        public bool Buffering
        {
            get { return buffering; }
        }

        public bool IsSwitchingFragments
        {
            get { return isSwitchingFragments; }
        }

        private MediaState GetMediaState()
        {
            var hlp = typeof(MediaElement).GetField("_helper", BindingFlags.NonPublic | BindingFlags.Instance);
            var helperObject = hlp.GetValue(MediaElement);
            var stateField = helperObject.GetType()
                .GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
            var state = (MediaState) stateField.GetValue(helperObject);
            return state;
        }

        public bool GetSceneAndOffset(long pos, out int num, out long offset)
        {
            num = 0;
            offset = 0;
            List<Scene> ami = null;
            lock (upscs)
                ami = upscs.Where(o => o.Time0 < pos).ToList<Scene>();

            if (ami.Count > 0)
            {
                var sc = ami[ami.Count - 1];
                num = sc.Index;
                offset = pos - sc.Time0;
                return true;
            }

            return false;
        }

        [Browsable(false)]
        public int Position
        {
            get
            {
                if (MediaElement != null && currentScene >= 0 && currentScene < upscs.Count)
                {
                    try
                    {
                        Scene sc = null;
                        lock (upscs)
                            sc = upscs[currentScene];

                        switch (GetMediaState())
                        {
                            case MediaState.Play:
                            case MediaState.Pause:
                                return Convert.ToInt32(MediaElement.Position.TotalMilliseconds + sc.Time0);
                        }
                    }
                    catch
                    {
                    }
                }

                return 0;
            }
            set
            {
                try
                {
                    if (Mode == PlayerMode.Stop)
                        return;

                    var p = GetMediaState() == MediaState.Pause;

                    if (value >= DurationUpload)
                    {
                        needPosition = value;
                        return;
                    }

                    var num = 0;
                    long offset = 0;
                    if (GetSceneAndOffset(value, out num, out offset))
                    {
                        if (currentScene == num)
                        {
                            MediaElement.Position = TimeSpan.FromMilliseconds(offset);
                        }
                        else
                        {
                            Open(num, offset, p);
                        }
                    }
                }
                catch
                {
                }
            }
        }

        [Browsable(false)]
        public int Volume
        {
            get
            {
                if (MediaElement != null)
                    return Convert.ToInt32(MediaElement.Volume*100.0);

                return 0;
            }
            set
            {
                try
                {
                    mediaElement1.Volume = (double) value/100.0;
                    mediaElement2.Volume = mediaElement1.Volume;
                }
                catch
                {
                }
            }
        }

        public int CurrentScene
        {
            get { return currentScene; }
        }

        public MediaTypeEnum MediaType { get; set; }

        public string Uri { get; set; }
        public int DurationUpload { get; set; }
        public int DurationTotal { get; set; }

        public int NumScene1 { get; set; }
        public int NumScene2 { get; set; }

        public PlayerMode Mode
        {
            get { return mode; }
            set
            {
                if (mode != value)
                {
                    try
                    {
                        switch (value)
                        {
                            case PlayerMode.Stop:
                                if (mediaElement1 != null)
                                    mediaElement1.Close();

                                if (mediaElement2 != null)
                                    mediaElement2.Close();

                                ffprobe = null;
                                Stop();

                                mode = value;

                                buffering = false;
                                if (MediaBuffering != null)
                                    MediaBuffering(this, EventArgs.Empty);

                                break;

                            case PlayerMode.Play:
                                if (!buffering)
                                    if (GetMediaState() == MediaState.Pause)
                                        MediaElement.Play();
                                mode = value;
                                break;

                            case PlayerMode.Pause:
                                if (GetMediaState() == MediaState.Play)
                                    MediaElement.Pause();
                                mode = value;
                                break;
                        }

                        if (MediaModeChanged != null)
                            MediaModeChanged(this, EventArgs.Empty);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private MediaElement MediaElement
        {
            get { return curr_player ? mediaElement1 : mediaElement2; }
        }

        private MediaElement MediaElementShadow
        {
            get { return curr_player ? mediaElement2 : mediaElement1; }
        }

        #endregion

        private void InitializePlayer()
        {
            Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            BufferTime = 500;
        }

        private void RecreatePlayer()
        {
            currentScene = -1;
            openScene = false;
            closed = true;
            needPosition = 0;
            DurationTotal = 0;
            DurationUpload = 0;
        }

        void mediaElement1_MediaEnded(object sender, RoutedEventArgs e)
        {
            Open(currentScene + 1, 0, false);
        }

        void mediaElement1_MediaOpened(object sender, RoutedEventArgs e)
        {
            closed = false;
            //MediaElement.Visibility = System.Windows.Visibility.Visible;
            //MediaElementShadow.Visibility = System.Windows.Visibility.Hidden;
            isSwitchingFragments = false;

            var curr_pl = MediaElement.Name;

            /*if (MediaType != MediaTypeEnum.FLAT_FILE && scenes.Exists(o => o.Index == CurrentScene + 1))
            {
                block_opened = true;
                try
                {
                    Scene next = scenes.First(o => o.Index == CurrentScene + 1);
                    String fn = String.Empty;

                    switch (MediaType)
                    {
                        case MediaTypeEnum.LOCAL_STREAM:
                            fn = new FileInfo(Uri).Directory.FullName + @"\" + next.Name;
                            break;

                        case MediaTypeEnum.NETWORK_STREAM:
                            fn = CacheDirectory + next.Name;
                            break;
                    }

                    if (mediaElement1 == MediaElementShadow)
                        lab111.Content = CurrentScene + 1;
                    if (mediaElement2 == MediaElementShadow)
                        lab222.Content = CurrentScene + 1;

                    MediaElementShadow.Source = new System.Uri(fn);
                    MediaElementShadow.Position = TimeSpan.FromMilliseconds(50);
                    MediaElementShadow.Pause();
                }
                catch
                { }
                block_opened = false;
            }*/
        }

        void mediaPlayer1_PlayStateChange()
        {
            var ps1 = GetMediaState();

            if (ps1 == MediaState.Play && _set_on_pause)
            {
                _set_on_pause = false;
                MediaElement.Pause();
            }

            if (ps1 == MediaState.Play && mode != PlayerMode.Play)
            {
                mode = PlayerMode.Play;
                if (MediaModeChanged != null)
                    MediaModeChanged(this, EventArgs.Empty);
            }

            if (ps1 == MediaState.Pause && mode != PlayerMode.Pause)
            {
                mode = PlayerMode.Pause;
                if (MediaModeChanged != null)
                    MediaModeChanged(this, EventArgs.Empty);
            }

            if (PlayStateChange != null)
                PlayStateChange(this, EventArgs.Empty);
        }

        public string CurrentSceneFileName { get; set; }

        public float GetCurrentSceneSecond()
        {
            return (float) MediaElement.Position.TotalMilliseconds/1000.0f;
        }



        public void Open(int num, long position, bool set_on_pause)
        {
            if (openScene)
                return;

            lock (upscs)
            {
                if (num > upscs.Count - 1)
                {
                    buffering = true;
                    if (MediaBuffering != null)
                        MediaBuffering(this, EventArgs.Empty);

                    return;
                }
            }

            isSwitchingFragments = true;

            _num = num;
            _pos = position;
            _set_on_pause = set_on_pause;
            closed = true;

            //DoAction(StartScene);
            Render.DoAction(StartScene);
            //Render.MyDoAction(StartScene);
        }

        private void StartScene()
        {
            if (openScene)
                return;

            if (closed)
            {
                openScene = true;
                closed = false;

                try
                {
                    Scene sc = null;
                    lock (upscs)
                        sc = upscs[_num];

                    CurrentSceneFileName = sc.Name;

                    switch (MediaType)
                    {
                        case MediaTypeEnum.LOCAL_STREAM:
                            CurrentSceneFileName = new FileInfo(Uri).Directory.FullName + @"\" + sc.Name;
                            break;

                        case MediaTypeEnum.NETWORK_STREAM:
                            CurrentSceneFileName = CacheDirectory + sc.Name;
                            break;
                    }

                    //var curr_pl = MediaElement.Name;

                    curr_player = !curr_player;

                    try
                    {
                        //lock (MediaElement)
                            if (MediaElement.Source == null || !MediaElement.Source.AbsoluteUri.Contains(sc.Name))
                                MediaElement.Source = new Uri(CurrentSceneFileName);
                    }
                    catch (Exception exc)
                    {
                        lock (MediaElement)
                            MediaElement.Source = new Uri(CurrentSceneFileName);
                    }

                    MediaElement.Visibility = System.Windows.Visibility.Visible;
                    MediaElementShadow.Visibility = System.Windows.Visibility.Hidden;

                    MediaElement.Play();

                    //curr_pl = MediaElement.Name;

                    buffering = false;
                    if (MediaBuffering != null)
                        MediaBuffering(this, EventArgs.Empty);

                    needPosition = 0L;

                    if (sc.Index > 1 && MediaType != MediaTypeEnum.FLAT_FILE &&
                        scenes.Exists(o => o.Index == sc.Index + 1))
                    {
                        var next = scenes.First(o => o.Index == sc.Index + 1);
                        var fn = string.Empty;

                        switch (MediaType)
                        {
                            case MediaTypeEnum.LOCAL_STREAM:
                                fn = new FileInfo(Uri).Directory.FullName + @"\" + next.Name;
                                break;

                            case MediaTypeEnum.NETWORK_STREAM:
                                fn = CacheDirectory + next.Name;
                                break;
                        }

                        MediaElementShadow.Source = new System.Uri(fn);
                        MediaElementShadow.Position = TimeSpan.FromMilliseconds(5);
                        MediaElementShadow.Pause();
                    }

                    if (_pos > 0)
                        MediaElement.Position = TimeSpan.FromMilliseconds(_pos);

                    if (_set_on_pause)
                        if (Mode == PlayerMode.Pause)
                            MediaElement.Pause();

                    if (ffprobe == null || MediaType == MediaTypeEnum.FLAT_FILE)
                        ffprobe = new Ffprobe(CurrentSceneFileName);

                    if (ffprobe != null)
                    {
                        VideoSize = new Size(ffprobe.Width, ffprobe.Height);
                        ResizeCanvas();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception " + ex.Message);
                }
                finally
                {
                    currentScene = _num;
                    openScene = false;
                }
            }
        }

        public Thickness GetVideoMargin()
        {
            return margin;
        }

        public void OpenUrl(string url)
        {
            OpenUrl(url, 0);
        }

        public void OpenUrl(string url, int offsetScene1)
        {
            Start(url, offsetScene1);
            buffering = true;
            if (MediaBuffering != null)
                MediaBuffering(this, EventArgs.Empty);
        }

        protected void ResizeCanvas()
        {
            if (ffprobe == null)
                return;

            Render.DoAction(() =>
            {
                var vs = new Size(ffprobe.Width, ffprobe.Height);
                try
                {
                    var aw = blackRect.ActualWidth;
                    var ah = blackRect.ActualHeight;

                    var dxv = (double) vs.Width/(double) vs.Height;
                    var dxc = aw/ah;

                    double w, h;

                    if (dxv < dxc)
                    {
                        h = blackRect.ActualHeight;
                        w = dxv*blackRect.ActualHeight;
                    }
                    else
                    {
                        w = blackRect.ActualWidth;
                        h = blackRect.ActualWidth/dxv;
                    }

                    var dw = Convert.ToDouble((aw - w)/2);
                    var dh = Convert.ToDouble((ah - h)/2);

                    margin = new Thickness(dw, dh, dw, dh);
                    mediaElement1.Margin = new Thickness(dw, dh, dw, dh);
                    mediaElement2.Margin = new Thickness(dw, dh, dw, dh);
                }
                catch
                {
                    ffprobe = null;
                }
            });
        }


        public void Start(string uri)
        {
            Start(uri, 0, int.MaxValue);
        }

        public void Start(string uri, int numScene1)
        {
            Start(uri, numScene1, int.MaxValue);
        }

        public void Start(string uri, int numScene1, int numScene2)
        {
            NumScene1 = numScene1;
            NumScene2 = numScene2;

            Stop();

            RecreatePlayer();

            Uri = uri;

            if (!Directory.Exists(CacheDirectory))
                Directory.CreateDirectory(CacheDirectory);
            else
                ClearFolder(CacheDirectory);

            MediaType = MediaTypeEnum.FLAT_FILE;

            if (uri.Contains(".m3u8"))
                MediaType = MediaTypeEnum.LOCAL_STREAM;

            if (uri.Contains("http://"))
                MediaType = MediaTypeEnum.NETWORK_STREAM;

            thread = new Thread(DoStart) {IsBackground = true};
            thread.Start();
        }


        private void DoStart()
        {
            try
            {
                done = false;
                scenes.Clear();
                openScene = false;
                DurationTotal = 0;
                DurationUpload = 0;
                ffprobe = null;

                var ci = new System.Globalization.CultureInfo("en-us");
                Thread.CurrentThread.CurrentCulture = ci;

                switch (MediaType)
                {
                    case MediaTypeEnum.NETWORK_STREAM:

                        #region NETWORK_STREAM

                        var wc = new WebClient();

                        while (!done)
                        {
                            var baseUrl = Uri + "/";

                            try
                            {
                                wc.DownloadFile(baseUrl + "out.m3u8", CacheDirectory + "_out.m3u8");
                            }
                            catch (Exception ex)
                            {
                                if (DownloadPlaylistResult != null)
                                    DownloadPlaylistResult(this, new DownloadPlaylistResultEventAgrs {ErrorCode = ex});

                                Thread.Sleep(1000);

                                continue;
                            }

                            if (DownloadPlaylistResult != null)
                                DownloadPlaylistResult(this, new DownloadPlaylistResultEventAgrs {ErrorCode = null});

                            ParseOut(CacheDirectory + "_out.m3u8");

                            if (done)
                                return;

                            var upList = new List<Scene>();
                            lock (scenes)
                            {
                                DurationTotal = scenes.Sum(o => o.Len);
                                upList = scenes.Where(o => !o.Uploaded).ToList<Scene>();
                            }

                            if (done)
                                return;

                            if (upList.Count > 0)
                            {
                                foreach (var sc in upList)
                                {
                                    var t0 = DateTime.Now.ToFileTime();

                                    if (done)
                                        return;

                                    try
                                    {
                                        wc.DownloadFile(baseUrl + sc.Name, CacheDirectory + sc.Name);
                                    }
                                    catch (Exception ex)
                                    {
                                        if (DownloadSceneResult != null)
                                            DownloadSceneResult(this,
                                                new DownloadSceneResultEventAgrs {ErrorCode = ex, Scene = sc});

                                        break;
                                    }

                                    if (done)
                                        return;

                                    if (File.Exists(CacheDirectory + sc.Name))
                                    {
                                        var t = (float) (DateTime.Now.ToFileTime() - t0)/(float) 10000;
                                        var sz = (new FileInfo(CacheDirectory + sc.Name)).Length;

                                        sc.Uploaded = true;
                                        sc.Size = sz;
                                        sc.Speed = sz/(t/1000.0f); // *(8.0f / 1024.0f / 1024.0f);

                                        lock (scenes)
                                            lock (upscs)
                                            {
                                                upscs.Clear();
                                                upscs.AddRange(scenes.Where(o => o.Uploaded));
                                                DurationUpload = upscs.Sum(o => o.Len);
                                            }

                                        if (DownloadSceneResult != null)
                                            DownloadSceneResult(this,
                                                new DownloadSceneResultEventAgrs {ErrorCode = null, Scene = sc});

                                        if (!openScene)
                                        {
                                            long start_position = Position;

                                            if (needPosition > 0L)
                                            {
                                                start_position = Math.Max(needPosition, CurrentConnectorSecond);
                                                if (DurationUpload >= 500 + start_position && buffering)
                                                {
                                                    var n = 0;
                                                    long ofss = 0;
                                                    if (GetSceneAndOffset(start_position, out n, out ofss))
                                                        Open(n, ofss, false);
                                                }
                                            }
                                            else
                                            {
                                                if (DurationUpload >= 500 + start_position && buffering)
                                                {
                                                    Open(currentScene + 1, 0, false);
                                                }
                                            }

                                            if (ffprobe == null)
                                            {
                                                ffprobe = new Ffprobe(CacheDirectory + sc.Name);
                                                if (ffprobe != null)
                                                {
                                                    ResizeCanvas();
                                                    if (FfprobeDetected != null)
                                                        FfprobeDetected(this, EventArgs.Empty);
                                                }
                                            }
                                        }
                                    }
                                    else
                                        break;

                                    if (done)
                                        return;
                                }
                            }
                            else
                            {
                                if (!openScene)
                                {
                                    if (needPosition > 0L)
                                    {
                                        if (DurationUpload >= 500 + needPosition && buffering)
                                        {
                                            var n = 0;
                                            long ofss = 0;
                                            if (GetSceneAndOffset(needPosition, out n, out ofss))
                                                Open(n, ofss, false);
                                        }
                                    }
                                    else
                                    {
                                        if (DurationUpload >= 500 + Position && buffering)
                                        {
                                            Open(currentScene + 1, 0, false);
                                        }
                                    }
                                }
                            }

                            Thread.Sleep(1000);
                        }
                        break;

                        #endregion

                    case MediaTypeEnum.LOCAL_STREAM:

                        #region LOCAL_STREAM

                        while (!done)
                        {
                            var baseUrl = new FileInfo(Uri).Directory.FullName;

                            ParseOut(Uri);

                            if (done)
                                return;

                            var upList = new List<Scene>();
                            Scene last = null;
                            lock (scenes)
                            {
                                DurationTotal = scenes.Sum(o => o.Len);
                                upList = scenes.Where(o => !o.Uploaded).ToList<Scene>();
                                if (scenes.Count > 0)
                                    last = scenes[scenes.Count - 1];
                            }

                            if (ffprobe == null && last != null)
                            {
                                ffprobe = new Ffprobe(baseUrl + "\\" + last.Name);
                                if (ffprobe != null && ffprobe.Width > 0)
                                {
                                    ResizeCanvas();
                                    if (FfprobeDetected != null)
                                        FfprobeDetected(this, EventArgs.Empty);
                                }
                            }

                            if (done)
                                return;

                            if (upList.Count > 0)
                            {
                                foreach (var sc in upList)
                                {
                                    var t0 = DateTime.Now.ToFileTime();

                                    if (done)
                                        return;

                                    try
                                    {
                                        File.Copy(baseUrl + "\\" + sc.Name, CacheDirectory + sc.Name, true);
                                    }
                                    catch (Exception ex)
                                    {
                                        Render.DoAction(() =>
                                        {
                                            labelError.Visibility = Visibility.Visible;
                                            labelError.Content = ex.Message;
                                        });

                                        if (DownloadSceneResult != null)
                                            DownloadSceneResult(this,
                                                new DownloadSceneResultEventAgrs {ErrorCode = ex, Scene = sc});

                                        break;
                                    }

                                    Render.DoAction(() =>
                                    {
                                        labelError.Visibility = Visibility.Hidden;
                                        labelError.Content = "";
                                    });

                                    if (done)
                                        return;

                                    if (File.Exists(CacheDirectory + sc.Name))
                                    {
                                        var t = (float) (DateTime.Now.ToFileTime() - t0)/(float) 10000;
                                        var sz = (new FileInfo(CacheDirectory + sc.Name)).Length;

                                        sc.Uploaded = true;
                                        sc.Size = sz;
                                        sc.Speed = sz/(t/1000.0f); // *(8.0f / 1024.0f / 1024.0f);

                                        lock (scenes)
                                            lock (upscs)
                                            {
                                                upscs.Clear();
                                                upscs.AddRange(scenes.Where(o => o.Uploaded));
                                                DurationUpload = upscs.Sum(o => o.Len);
                                            }

                                        if (DownloadSceneResult != null)
                                            DownloadSceneResult(this,
                                                new DownloadSceneResultEventAgrs {ErrorCode = null, Scene = sc});
                                    }
                                    else
                                        break;

                                    if (done)
                                        return;

                                    if (!openScene)
                                    {
                                        if (needPosition > 0L)
                                        {
                                            if (DurationUpload >= 500 + needPosition && buffering)
                                            {
                                                var n = 0;
                                                long ofss = 0;
                                                if (GetSceneAndOffset(needPosition, out n, out ofss))
                                                    Open(n, ofss, false);
                                            }
                                        }
                                        else
                                        {
                                            if (DurationUpload >= 500 + Position && buffering)
                                            {
                                                Open(currentScene + 1, 0, false);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (!openScene)
                                {
                                    if (needPosition > 0L)
                                    {
                                        if (DurationUpload >= 500 + needPosition && buffering)
                                        {
                                            var n = 0;
                                            long ofss = 0;
                                            if (GetSceneAndOffset(needPosition, out n, out ofss))
                                                Open(n, ofss, false);
                                        }
                                    }
                                    else
                                    {
                                        if (DurationUpload >= 500 + Position && buffering)
                                        {
                                            Open(currentScene + 1, 0, false);
                                        }
                                    }
                                }
                            }

                            Thread.Sleep(1000);
                        }
                        break;

                        #endregion

                    case MediaTypeEnum.FLAT_FILE:

                        #region FLAT_FILE

                        ffprobe = new Ffprobe(Uri);
                        if (ffprobe != null)
                        {
                            var sc = new Scene
                            {
                                Name = Uri,
                                Index = 0,
                                Len = Convert.ToInt32(ffprobe.Duration),
                                Uploaded = true
                            };

                            lock (scenes)
                                scenes.Add(sc);

                            lock (upscs)
                            {
                                upscs.Clear();
                                upscs.Add(sc);
                                DurationUpload = upscs.Sum(o => o.Len);
                            }

                            lock (scenes)
                                DurationTotal = scenes.Sum(o => o.Len);

                            ResizeCanvas();
                            if (FfprobeDetected != null)
                                FfprobeDetected(this, EventArgs.Empty);

                            Open(0, 0, false);
                        }
                        break;

                        #endregion
                }
            }
            catch (ThreadInterruptedException e)
            {
            }
        }

        private void ParseOut(string fileName)
        {
            try
            {
                var fi = new FileInfo(fileName);

                //String[] lines = File.ReadAllLines(fi.FullName);
                using (
                    var sr =
                        new StreamReader(File.Open(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    string line = null;
                    string extInf = null;
                    while ((line = sr.ReadLine()) != null)
                        //foreach (String line in lines)
                    {
                        if (line.Contains("#EXT-X-ENDLIST"))
                        {
                            //return true;
                        }

                        if (string.IsNullOrEmpty(extInf) && line.Contains("#EXTINF:"))
                        {
                            extInf = line;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(extInf) && line.Contains("out"))
                            {
                                var num = 0;
                                var numStr = line.Replace("out", "").Replace(".mp4", "").Replace(".ts", "");
                                var len = 0.0f;
                                var lenStr = extInf.Replace("#EXTINF:", "").Replace(",", "");
                                if (int.TryParse(numStr, out num))
                                {
                                    lock (scenes)
                                    {
                                        if (!scenes.Exists(o => o.Index == num) && float.TryParse(lenStr, out len))
                                        {
                                            var t = 0;
                                            foreach (var s in scenes)
                                                t += s.Len;

                                            scenes.Add(new Scene
                                            {
                                                Index = num,
                                                Len = Convert.ToInt32(len*1000),
                                                Name = line,
                                                Time0 = t,
                                                Uploaded = false,
                                            });
                                        }
                                    }
                                }

                                extInf = null;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void ClearFolder(string dir)
        {
            try
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
            catch (Exception ex)
            {

            }
        }

        public void Stop()
        {
            done = true;

            if (thread != null && thread.IsAlive)
                thread.Interrupt();

            ffprobe = null;
        }

        private bool IsPlayerLive
        {
            get
            {
                return MediaElement != null
                       && (GetMediaState() == MediaState.Pause || GetMediaState() == MediaState.Play);
            }
        }

        public double SpeedRatio
        {
            get
            {
                if (MediaElement != null)
                    return MediaElement.SpeedRatio;

                return 1.0;
            }
            set
            {
                if (mediaElement1 != null)
                    mediaElement1.SpeedRatio = value;
                if (mediaElement2 != null)
                    mediaElement2.SpeedRatio = value;
            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!IsPlayerLive && !openScene && !Buffering)
                    return;

                mediaPlayer1_PlayStateChange();

                long pos = Position;

                lock (scenes)
                    if (MediaElement.Position.TotalMilliseconds > scenes[currentScene].Len - 300)
                    {
                        //MediaElement.Pause();
                        //Open(currentScene + 1, 0, false);
                        //return;
                    }

                if (old_pos != pos)
                {
                    if (GetMediaState() == MediaState.Play && pos > DurationUpload - 500)
                    {
                        MediaElement.Pause();

                        buffering = true;

                        if (MediaBuffering != null)
                            MediaBuffering(this, EventArgs.Empty);
                    }

                    if (PositionChanged != null)
                        PositionChanged(this, new PositionEventArgs {Position = pos});

                    old_pos = pos;
                }
            }
            catch
            {
            }
        }

        private void blackRect_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeCanvas();
        }
    }

    public class Render
    {
        public static void Init()
        {
            new System.Windows.Application();
        }

        public static void Execute(Action a)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Render, a);
            }
            catch
            { }
        }

        public static void DoAction(Action a)
        {
            Execute(a);
        }
    }
}
