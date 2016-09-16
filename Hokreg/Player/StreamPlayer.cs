using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Net;
using System.Threading.Tasks;

namespace Uniso.InStat.Players
{
    public partial class StreamPlayer : System.Windows.Forms.Control, IDisposable
    {
        //EVENTS
        public event DownloadPlaylistResultHandler DownloadPlaylistResult;
        public event DownloadSceneResultHandler DownloadSceneResult;
        public event PositionEventHandler PositionChanged;
        public event EventHandler MediaBuffering;
        public event EventHandler MediaModeChanged;
        public event MouseEventHandler PointOnScreenRegister;
        public event EventHandler CanvasResized;
        public event DrawFrameEventHandler DrawFrame;
        public event MouseEventHandler MouseMovePlayer;

        public StreamPlayer()
        {
            InitializeComponent();
            InitializePlayer();
        }

        public StreamPlayer(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            InitializePlayer();
        }

        private void InitializePlayer()
        {
            BackColor = Color.Black;
            BufferTime = 1000;
            CacheDirectory = @".\cache\";
            UISync.Init(this);
        }
        
        private void RecreatePlayer()
        {
            currentScene = -1;
            openScene = false;
            closed = true;
            needPosition = 0;
            DurationTotal = 0;
            DurationUpload = 0;

            NewInstancePlayer();
        }

        public void NewInstancePlayer()
        {
            if (mediaElement1 != null)
            {
                if (!mediaElement1.IsDisposed)
                    mediaElement1.Dispose();

                Controls.Remove(mediaElement1);
            }

            mediaElement1 = new AxMediaPlayer.FootballPlayer();
            mediaElement1.MouseMoveEvent += mediaElement2_MouseMoveEvent;
            mediaElement1.MouseUpEvent += mediaElement2_MouseUpEvent;
            mediaElement1.PlayStateChange += mediaElement1_PlayStateChange;
            mediaElement1.Location = new System.Drawing.Point(0, 0);
            mediaElement1.Size = new System.Drawing.Size(400, 300);

            Controls.Add(mediaElement1);

            if (mediaElement2 != null)
            {
                if (!mediaElement2.IsDisposed)
                    mediaElement2.Dispose();

                Controls.Remove(mediaElement2);
            }

            mediaElement2 = new AxMediaPlayer.FootballPlayer();
            mediaElement2.MouseMoveEvent += mediaElement2_MouseMoveEvent;
            mediaElement2.MouseUpEvent += mediaElement2_MouseUpEvent;
            mediaElement2.PlayStateChange += mediaElement1_PlayStateChange;
            mediaElement2.Location = new System.Drawing.Point(0, 0);
            mediaElement2.Size = new System.Drawing.Size(400, 300);

            Controls.Add(mediaElement2);
        }

        void mediaElement2_MouseMoveEvent(object sender, AxMediaPlayer._MediaPlayerEvents_MouseMoveEvent e)
        {
            if (MouseMovePlayer != null)
                MouseMovePlayer(this, new MouseEventArgs(System.Windows.Forms.MouseButtons.None, 1, e.x, e.y, 0));
        }

        #region EVENTS

        public delegate void PositionEventHandler(object sender, PositionEventArgs e);

        public class PositionEventArgs : EventArgs
        {
            public int Position { get; set; }
        }

        public delegate void DrawFrameEventHandler(object sender, DrawFrameEventArgs e);

        public class DrawFrameEventArgs : EventArgs
        {
            public Gdi.GDICompatible Gdi { get; set; }
        }

        private class UISync
        {
            public static bool canExec = false;
            private static ISynchronizeInvoke Sync;

            public static void Init(ISynchronizeInvoke sync)
            {
                Sync = sync;
            }

            public static void Execute(Action action)
            {
                try
                {
                    Sync.Invoke(action, null);
                }
                catch
                { }
            }
        }

        #endregion

        //MEMBERS
        private bool buffering = false;

        //PROPERTIES
        public int BufferTime { get; set; }
        public String CacheDirectory { get; set; }

        //private AxMediaPlayer.FootballPlayer mediaPlayer1 = null;
        private AxMediaPlayer.FootballPlayer mediaElement1 = null;
        private AxMediaPlayer.FootballPlayer mediaElement2 = null;

        [Browsable(false)]
        public Rectangle GetVideoRect()
        {
            if (mediaElement1 == null)
                return ClientRectangle;

            return new Rectangle(mediaElement1.Left, mediaElement1.Top, mediaElement1.Width, mediaElement1.Height);
        }

        private long needPosition = 0L;

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

        public long GetPositinLong(int num, long offset)
        {
            return upscs[num].Time0 + offset;
        }

        public bool GetSceneAndOffset(long pos, out int num, out long offset)
        {
            num = 0;
            offset = 0;
            var ami = upscs.Where(o => o.Time0 <= pos).ToList<Scene>();
            if (ami.Count > 0)
            {
                var sc = ami[ami.Count - 1];
                num = sc.Index;
                offset = pos - sc.Time0;
                return true;
            }

            return false;
        }

        private int position = 0;

        [Browsable(false)]
        public int Position
        {
            get
            {
                return position;
            }
            set
            {
                try
                {
                    if (Mode == PlayerMode.Stop)
                        return;

                    if (value >= DurationUpload)
                    {
                        needPosition = value;
                        return;
                    }

                    position = value;
                    var num = 0;
                    long offset = 0;

                    if (GetSceneAndOffset(value, out num, out offset))
                    {
                        if (currentScene == num)
                        {
                            MediaElement.CurrentPosition = (double)offset / 1000.0;
                            if (Mode == PlayerMode.Pause)
                            {
                                MediaElement.Play();
                                MediaElement.Pause();
                            }
                        }
                        else
                        {
                            Open(num, offset, Mode != PlayerMode.Play);
                        }
                    }
                }
                catch
                { }
            }
        }

        private double rate = 1.0;

        public double Rate
        {
            get
            {
                return rate;
            }
            set
            {
                rate = value;
                if (mediaElement1 != null)
                    mediaElement1.Rate = rate;

                if (mediaElement2 != null)
                    mediaElement2.Rate = rate;
            }
        }

        private int volume = -4000;

        [Browsable(false)]
        public int Volume
        {
            get
            {
                if (MediaElement != null)
                    return MediaElement.Volume;

                return 0;
            }
            set
            {
                try
                {
                    volume = value;
                    MediaElement.Volume = value;
                    
                }
                catch
                { }
            }
        }

        void mediaElement2_MouseUpEvent(object sender, AxMediaPlayer._MediaPlayerEvents_MouseUpEvent e)
        {

        }

        private int currentScene = -1;
        public int CurrentScene
        {
            get { return currentScene; }
        }

        private bool isSwitchingFragments = false;
        private bool closed = true;
        private int _num = 0;
        private long _pos = 0;
        private bool _set_on_pause = false;
        private bool openScene = false;

        void mediaElement1_PlayStateChange(object sender, AxMediaPlayer._MediaPlayerEvents_PlayStateChangeEvent e)
        {
            var player = (AxMediaPlayer.FootballPlayer)sender;

            var ps0 = (MediaPlayer.MPPlayStateConstants)e.oldState;
            var ps1 = (MediaPlayer.MPPlayStateConstants)e.newState;

            if (ps0 == MediaPlayer.MPPlayStateConstants.mpWaiting && ps1 == MediaPlayer.MPPlayStateConstants.mpStopped && MediaElement == player)
            {
                Open(currentScene + 1, 0, false);
            }

            if (ps0 == MediaPlayer.MPPlayStateConstants.mpWaiting && ps1 == MediaPlayer.MPPlayStateConstants.mpPlaying && MediaElement != player)
            {
                player.Pause();
                player.CurrentPosition = 0;
            }

            if (player == MediaElement)
            {
                if (ps1 == MediaPlayer.MPPlayStateConstants.mpPlaying)
                    closed = false;

                if (ps1 == MediaPlayer.MPPlayStateConstants.mpPlaying && _set_on_pause)
                {
                    _set_on_pause = false;

                    MediaElement.Pause();
                }

                if (MediaElement.PlayState == MediaPlayer.MPPlayStateConstants.mpPlaying && mode != PlayerMode.Play)
                {
                    mode = PlayerMode.Play;
                    if (MediaModeChanged != null)
                        MediaModeChanged(this, EventArgs.Empty);
                }

                if (MediaElement.PlayState == MediaPlayer.MPPlayStateConstants.mpPaused && mode != PlayerMode.Pause)
                {
                    mode = PlayerMode.Pause;
                    if (MediaModeChanged != null)
                        MediaModeChanged(this, EventArgs.Empty);
                }
            }
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

            UISync.Execute(() => StartScene());
        }

        public String CurrentSceneFileName { get; set; }

        public float GetCurrentSceneSecond()
        {
            return (float)MediaElement.CurrentPosition * 1000.0f;
        }

        private bool curr_player = false;

        private AxMediaPlayer.FootballPlayer MediaElement
        {
            get { return curr_player ? mediaElement1 : mediaElement2; }
        }

        private AxMediaPlayer.FootballPlayer MediaElementShadow
        {
            get { return curr_player ? mediaElement2 : mediaElement1; }
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

                    curr_player = !curr_player;

                    if (String.IsNullOrEmpty(MediaElement.FileName) || !MediaElement.FileName.Contains(sc.Name))
                    {
                        MediaElement.Open(CurrentSceneFileName);
                        MediaElement.Play();
                    }
                    else
                    {
                        MediaElement.CurrentPosition = 0;
                        MediaElement.Play();
                    }

                    MediaElement.Volume = volume;
                    MediaElement.BringToFront();
                    MediaElement.AudioStream = 0;

                    buffering = false;
                    if (MediaBuffering != null)
                        MediaBuffering(this, EventArgs.Empty);

                    needPosition = 0L;

                    PreloadScene(sc);

                    if (_pos > 0)
                        MediaElement.CurrentPosition = (double)_pos / 1000.0;

                    if (_set_on_pause)
                        if (Mode == PlayerMode.Pause)
                            MediaElement.Pause();

                    if (ffprobe == null || MediaType == MediaTypeEnum.FLAT_FILE)
                        ffprobe = new InStat.Ffprobe(CurrentSceneFileName);
                }
                catch (Exception ex)
                {
                    Log.WriteException(ex);
                }
                finally
                {
                    currentScene = _num;
                    openScene = false;
                }
            }
        }

        private void PreloadScene(Scene sc)
        {
            lock (scenes)
            {
                if (MediaType != MediaTypeEnum.FLAT_FILE && scenes.Exists(o => o.Index == sc.Index + 1))
                {
                    var next = scenes.First(o => o.Index == sc.Index + 1);

                    if (String.IsNullOrEmpty(MediaElement.FileName) || !MediaElement.FileName.Contains(next.Name))
                    {
                        var fn = String.Empty;

                        switch (MediaType)
                        {
                            case MediaTypeEnum.LOCAL_STREAM:
                                fn = new FileInfo(Uri).Directory.FullName + @"\" + next.Name;
                                break;

                            case MediaTypeEnum.NETWORK_STREAM:
                                fn = CacheDirectory + next.Name;
                                break;
                        }

                        MediaElementShadow.Open(fn);
                    }
                }
            }
        }

        public Size VideoSize { get; set; }

        public enum MediaTypeEnum
        {
            FLAT_FILE,
            NETWORK_STREAM,
            LOCAL_STREAM,
        }

        public MediaTypeEnum MediaType { get; set; }

        public void OpenUrl(String url)
        {
            OpenUrl(url, 0);
        }

        public void OpenUrl(String url, int offsetScene1)
        {
            Start(url, offsetScene1);
            buffering = true;
            if (MediaBuffering != null)
                MediaBuffering(this, EventArgs.Empty);

            //vlcPlayerControl1.Volume = 0;
        }

        protected void ResizeCanvas()
        {
            UISync.Execute(() =>
            {
                if (Ffprobe != null)
                {
                    var vs = new Size(Ffprobe.Width, Ffprobe.Height);
                    var dxv = (Double)vs.Width / (Double)vs.Height;
                    var dxc = (Double)(this.Width / 1) / (Double)this.Height;
                    if (dxv < dxc)
                    {
                        mediaElement1.Height = this.Height;
                        mediaElement1.Width = Convert.ToInt32(dxv * this.Height) / 1;
                    }
                    else
                    {
                        mediaElement1.Width = this.Width / 1;
                        mediaElement1.Height = Convert.ToInt32(this.Width / dxv);
                    }
                    mediaElement1.Left = (this.Width - mediaElement1.Width) / 2;
                    mediaElement1.Top = (this.Height - mediaElement1.Height) / 2;

                    mediaElement2.Left = mediaElement1.Left;// +Width / 2;
                    mediaElement2.Top = mediaElement1.Top;
                    mediaElement2.Width = mediaElement1.Width;
                    mediaElement2.Height = mediaElement1.Height;

                    if (CanvasResized != null)
                        CanvasResized(this, EventArgs.Empty);
                }
            });
        }

        public enum PlayerMode
        {
            Stop,
            Play,
            Pause
        }

        private PlayerMode mode = PlayerMode.Stop;

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
                                
                                ffprobe = null;
                                Stop();

                                mode = value;

                                buffering = false;
                                if (MediaBuffering != null)
                                    MediaBuffering(this, EventArgs.Empty);

                                if (mediaElement1 != null)
                                {
                                    mediaElement1.Stop();
                                    mediaElement1.Dispose();
                                }

                                if (mediaElement2 != null)
                                {
                                    mediaElement2.Stop();
                                    mediaElement2.Dispose();
                                }

                                position = 0;

                                break;

                            case PlayerMode.Play:
                                if (!buffering)
                                    MediaElement.Play();
                                mode = value;
                                break;

                            case PlayerMode.Pause:
                                MediaElement.Pause();
                                mode = value;
                                break;
                        }

                        if (MediaModeChanged != null)
                            MediaModeChanged(this, EventArgs.Empty);
                    }
                    catch
                    { }
                }
            }
        }

        public void ToggleMute()
        {

        }

        private void VLCStreamPlayer_Resize(object sender, EventArgs e)
        {
            ResizeCanvas();
        }

        private Thread thread = null;
        private List<Scene> scenes = new List<Scene>();
        private List<Scene> upscs = new List<Scene>();

        [Serializable()]
        public class Scene
        {
            public long Time0 { get; set; }
            public int Len { get; set; }
            public String Name { get; set; }
            public bool Uploaded { get; set; }
            public bool Used { get; set; }
            public int Index { get; set; }
            public long Size { get; set; }
            public float Rate { get; set; }
            public Exception DownloadException = null;
        }

        public delegate void DownloadPlaylistResultHandler(object sender, DownloadPlaylistResultEventAgrs e);
        public class DownloadPlaylistResultEventAgrs : EventArgs
        {
            public Exception ErrorCode { get; set; }
        }

        public delegate void DownloadSceneResultHandler(object sender, DownloadSceneResultEventAgrs e);
        public class DownloadSceneResultEventAgrs : EventArgs
        {
            public Exception ErrorCode { get; set; }
            public Scene Scene { get; set; }
        }

        public event EventHandler FfprobeDetected;

        private String uri = null;
        public String Uri
        {
            get
            {
                return uri;
            }
        }

        public int DurationUpload { get; set; }
        public int DurationTotal { get; set; }

        private Ffprobe ffprobe = null;
        public Ffprobe Ffprobe
        {
            get { return ffprobe; }
        }

        public int NumScene1 { get; set; }
        public int NumScene2 { get; set; }

        public void Start(String uri)
        {
            Start(uri, 0, Int32.MaxValue);
        }

        public void Start(String uri, int numScene1)
        {
            Start(uri, numScene1, Int32.MaxValue);
        }

        public void Start(String uri, int numScene1, int numScene2)
        {
            NumScene1 = numScene1;
            NumScene2 = numScene2;

            RecreatePlayer();

            Stop();

            this.uri = uri;

            if (!Directory.Exists(CacheDirectory))
            {
                Directory.CreateDirectory(CacheDirectory);
            }
            else
            {
                ClearFolder(CacheDirectory);
            }

            MediaType = MediaTypeEnum.FLAT_FILE;

            if (uri.Contains(".m3u8"))
                MediaType = MediaTypeEnum.LOCAL_STREAM;

            if (uri.Contains("http://"))
                MediaType = MediaTypeEnum.NETWORK_STREAM;

            thread = new Thread(DoStart);
            thread.IsBackground = true;
            thread.Start();
        }

        private List<Scene> CreateAndExecTasks(List<Scene> upList, String baseUrl, int max_tasks)
        {
            var tasks = new List<Task>();
            var res = new List<Scene>();

            foreach (var sc in upList.OrderBy(o => o.Time0))
            {
                res.Add(sc);
                if (--max_tasks <= 0)
                    break;
            }

            var max_try = 3;
            do
            {
                foreach (var sc in res.Where(o => !o.Uploaded))
                {
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            var sw = new Stopwatch();
                            sw.Start();

                            Log.Write("Downloading " + (baseUrl + sc.Name) + " => " + (CacheDirectory + sc.Name));
                            var wc = new WebClient();
                            wc.DownloadFile(baseUrl + sc.Name, CacheDirectory + sc.Name);
                            sw.Stop();

                            if (File.Exists(CacheDirectory + sc.Name))
                            {
                                float t = sw.ElapsedMilliseconds;
                                var sz = (new FileInfo(CacheDirectory + sc.Name)).Length;

                                sc.Size = sz;
                                sc.Rate = sz / (t / 1000.0f);

                                sc.Uploaded = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (DownloadSceneResult != null)
                                DownloadSceneResult(this, new DownloadSceneResultEventAgrs { ErrorCode = ex, Scene = sc });

                            sc.DownloadException = ex;
                        }
                    }));
                }

                Task.WaitAll(tasks.ToArray<Task>());

                if (--max_try <= 0)
                    break;
            }
            while (res.Any(o => !o.Uploaded) && max_try > 0);

            return res;
        }

        private bool done = false;
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
                                    DownloadPlaylistResult(this, new DownloadPlaylistResultEventAgrs { ErrorCode = ex });

                                Thread.Sleep(1000);
                                continue;
                            }

                            if (DownloadPlaylistResult != null)
                                DownloadPlaylistResult(this, new DownloadPlaylistResultEventAgrs { ErrorCode = null });

                            ParseOut(CacheDirectory + "_out.m3u8");

                            if (done)
                                return;

                            Scene last = null;
                            var upList = new List<Scene>();

                            lock (scenes)
                            {
                                DurationTotal = scenes.Sum(o => o.Len);
                                upList = scenes.Where(o => !o.Uploaded).ToList<Scene>();
                                if (scenes.Count > 0)
                                    last = scenes[0];
                            }

                            if (done)
                                return;

                            if (upList.Count > 0)
                            {
                                var tasks = CreateAndExecTasks(upList, baseUrl, 3);

                                var exeption_flag = false;
                                foreach (var sc in tasks)
                                {
                                    //Если была ошибка или на предыдущем моменте облом
                                    if (sc.DownloadException != null || exeption_flag)
                                    {
                                        exeption_flag = true;
                                        sc.DownloadException = null;
                                        sc.Uploaded = false;
                                        continue;
                                    }

                                    lock (scenes)
                                        lock (upscs)
                                        {
                                            upscs.Clear();
                                            upscs.AddRange(scenes.Where(o => o.Uploaded));
                                            DurationUpload = upscs.Sum(o => o.Len);
                                        }

                                    if (DownloadSceneResult != null)
                                        DownloadSceneResult(this, new DownloadSceneResultEventAgrs { ErrorCode = null, Scene = sc });

                                    if (ffprobe == null && last != null)
                                    {
                                        ffprobe = new Ffprobe(CacheDirectory + last.Name);
                                        if (ffprobe.Width == 0)
                                            ffprobe = null;

                                        if (ffprobe != null)
                                        {
                                            VideoSize = new Size(ffprobe.Width, ffprobe.Height);
                                            ResizeCanvas();
                                            if (FfprobeDetected != null)
                                                FfprobeDetected(this, EventArgs.Empty);
                                        }
                                    }

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

                                        var shadow_fn = String.Empty;
                                        float sc_curr_t = 0;

                                        UISync.Execute(() =>
                                        {
                                            try
                                            {
                                                sc_curr_t = GetCurrentSceneSecond();
                                                shadow_fn = MediaElementShadow != null && !String.IsNullOrEmpty(MediaElementShadow.FileName)
                                                    ? MediaElementShadow.FileName
                                                    : String.Empty;
                                            }
                                            catch
                                            { }
                                        });

                                        if (currentScene >= 0 && currentScene + 1 == sc.Index && sc_curr_t < 8000 && !shadow_fn.Contains(sc.Name))
                                        {
                                            Scene sc_curr = null;
                                            lock (scenes)
                                                if (scenes.Any(o => o.Index == currentScene))
                                                    sc_curr = scenes.First(o => o.Index == currentScene);

                                            if (sc_curr != null)
                                                UISync.Execute(() =>
                                                {
                                                    PreloadScene(sc_curr);
                                                });
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

                    case MediaTypeEnum.LOCAL_STREAM:
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
                                    last = scenes[0];
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
                                        if (DownloadSceneResult != null)
                                            DownloadSceneResult(this, new DownloadSceneResultEventAgrs { ErrorCode = ex, Scene = sc });

                                        break;
                                    }

                                    if (done)
                                        return;

                                    if (File.Exists(CacheDirectory + sc.Name))
                                    {
                                        var t = (float)(DateTime.Now.ToFileTime() - t0) / (float)10000;
                                        var sz = (new FileInfo(CacheDirectory + sc.Name)).Length;

                                        sc.Uploaded = true;
                                        sc.Size = sz;

                                        lock (scenes)
                                            lock (upscs)
                                            {
                                                upscs.Clear();
                                                upscs.AddRange(scenes.Where(o => o.Uploaded));
                                                DurationUpload = upscs.Sum(o => o.Len);
                                            }

                                        if (DownloadSceneResult != null)
                                            DownloadSceneResult(this, new DownloadSceneResultEventAgrs { ErrorCode = null, Scene = sc });
                                    }
                                    else
                                        break;

                                    if (done)
                                        return;

                                    if (ffprobe == null && last != null)
                                    {
                                        ffprobe = new Ffprobe(CacheDirectory + last.Name);
                                        if (ffprobe.Width == 0)
                                            ffprobe = null;

                                        if (ffprobe != null)
                                        {
                                            VideoSize = new Size(ffprobe.Width, ffprobe.Height);
                                            ResizeCanvas();
                                            if (FfprobeDetected != null)
                                                FfprobeDetected(this, EventArgs.Empty);
                                        }
                                    }

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

                    case MediaTypeEnum.FLAT_FILE:
                        ffprobe = new Ffprobe(Uri);
                        if (ffprobe != null)
                        {
                            var sc = new Scene();
                            sc.Name = Uri;
                            sc.Index = 0;
                            sc.Len = Convert.ToInt32(ffprobe.Duration);
                            sc.Uploaded = true;

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

                            VideoSize = new Size(ffprobe.Width, ffprobe.Height);
                            ResizeCanvas();
                            if (FfprobeDetected != null)
                                FfprobeDetected(this, EventArgs.Empty);

                            Open(0, 0, false);

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
                        break;
                }
            }
            catch (ThreadInterruptedException e)
            { }
        }

        private void ParseOut(String fileName)
        {
            try
            {
                var fi = new FileInfo(fileName);

                //String[] lines = File.ReadAllLines(fi.FullName);
                using (var sr = new StreamReader(File.Open(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    String line = null;
                    String extInf = null;
                    while ((line = sr.ReadLine()) != null)
                    //foreach (String line in lines)
                    {
                        if (line.Contains("#EXT-X-ENDLIST"))
                        {
                            //return true;
                        }

                        if (String.IsNullOrEmpty(extInf) && line.Contains("#EXTINF:"))
                        {
                            extInf = line;
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(extInf) && line.Contains("out"))
                            {
                                var num = 0;
                                var numStr = line.Replace("out", "").Replace(".mp4", "").Replace(".ts", "");
                                var len = 0.0f;
                                var lenStr = extInf.Replace("#EXTINF:", "").Replace(",", "");
                                if (Int32.TryParse(numStr, out num))
                                {
                                    lock (scenes)
                                    {
                                        if (!scenes.Exists(o => o.Index == num) && Single.TryParse(lenStr, out len)/* && num >= NumScene1 && num <= NumScene2*/)
                                        {
                                            var t = 0;
                                            foreach (var s in scenes)
                                                t += s.Len;

                                            scenes.Add(new Scene
                                            {
                                                Index = num,
                                                Len = Convert.ToInt32(len * 1000),
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
            { }
        }

        private void ClearFolder(String dir)
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
            ffprobe = null;
            done = true;

            if (thread != null && thread.IsAlive)
                thread.Interrupt();
        }

        private bool IsPlayerLive
        {
            get
            {
                return MediaElement != null
                    && (MediaElement.PlayState == MediaPlayer.MPPlayStateConstants.mpPaused
                    || MediaElement.PlayState == MediaPlayer.MPPlayStateConstants.mpPlaying);
            }
        }

        long old_pos = 0L;
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            try
            {
                if (!IsPlayerLive && !Buffering)
                    return;

                lock (upscs)
                {
                    if (MediaElement != null && currentScene >= 0 && currentScene < upscs.Count)
                    {
                        var sc = upscs[currentScene];
                        var p = (int)sc.Time0;
                        if (MediaElement != null)
                            p += Convert.ToInt32(MediaElement.CurrentPosition * 1000.0);
                        if (p != position)
                            position = p;
                    }
                }

                if (old_pos != position)
                {
                    if (MediaElement.PlayState == MediaPlayer.MPPlayStateConstants.mpPlaying && position > DurationUpload - BufferTime)
                    {
                        MediaElement.Pause();

                        buffering = true;

                        if (MediaBuffering != null)
                            MediaBuffering(this, EventArgs.Empty);
                    }

                    if (PositionChanged != null)
                        PositionChanged(this, new PositionEventArgs { Position = position });

                    old_pos = position;
                }
                /*else
                    if (vlcPlayerControl1.State == DZ.MediaPlayer.Vlc.WindowsForms.VlcPlayerControlState.Playing)
                    {
                        pch++;
                        if (pch >= 6)
                        {
                            pch = 0;
                            ReopenPlayer();
                        }
                    }*/
            }
            catch
            { }
            finally
            {
                timer1.Enabled = true;
            }
        }
    }
}
