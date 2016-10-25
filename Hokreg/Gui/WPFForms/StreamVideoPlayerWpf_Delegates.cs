using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uniso.InStat.StreamPlayer
{
    public partial class StreamVideoPlayerWpf
    {
        public enum PlayerMode
        {
            Stop,
            Play,
            Pause
        }
        
        public enum MediaTypeEnum
        {
            FLAT_FILE,
            NETWORK_STREAM,
            LOCAL_STREAM
        }

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
            public float Speed { get; set; }
        }
    
        //EVENTS
        public event DownloadPlaylistResultHandler DownloadPlaylistResult;
        public event DownloadSceneResultHandler DownloadSceneResult;
        public event PositionEventHandler PositionChanged;
        public event EventHandler PlayStateChange;
        public event EventHandler MediaBuffering;
        public event EventHandler MediaModeChanged;
        public event EventHandler FfprobeDetected;

        public delegate void PositionEventHandler(object sender, PositionEventArgs e);

        public class PositionEventArgs : EventArgs
        {
            public long Position { get; set; }
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
    }
}
