using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
using AxMediaPlayer;
using Microsoft.Win32;
using Uniso.InStat.Annotations;
using Uniso.InStat.Classes;
using Uniso.InStat.Gui.WPFForms;
using Uniso.InStat.StreamPlayer;


namespace Uniso.InStat.Gui.WPF_Forms.MVVM
{
    public class WpfMainFormViewModel: INotifyPropertyChanged
    {
        #region Members

        // ReSharper disable once InconsistentNaming
        private MainFormModel model;
        private string _videoFileName;
        private ICommand _openVideoFileCommand = null;
        private bool _canOpenVideoFile;
        private ICommand _mouseDownCommand;
        private bool _mediaElementPlaying;
        private DispatcherTimer timer;
        private StreamVideoPlayerWpf streamVideoPlayerWpf;
        private float _dirtyTime;
        private string _dirtyTimeString;
        private WPFMainFormControl wPFMainFormControl;
        private ICommand _registerMarker;

        #endregion

        #region Constructor

        public WpfMainFormViewModel()
        {
            model = new MainFormModel();
            _canOpenVideoFile = true;
        }

        public WpfMainFormViewModel(StreamVideoPlayerWpf streamVideoPlayerWpf): this()
        {
            this.streamVideoPlayerWpf = streamVideoPlayerWpf;

            timer = new DispatcherTimer();

            timer.Tick += TimerOnTick;

            timer.Start();
        }

        public WpfMainFormViewModel(StreamVideoPlayerWpf streamVideoPlayerWpf, WPFMainFormControl wPFMainFormControl) : this(streamVideoPlayerWpf)
        {
            this.wPFMainFormControl = wPFMainFormControl;
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            DirtyTime = streamVideoPlayerWpf.GetCurrentSceneMiliseconds();

            wPFMainFormControl.DirtyTimeTextBlock.Text = DirtyTimeString;
        }

        #endregion


        #region Fields

        public string VideoFileName
        {
            get
            {
                return model.VideoFileName;
            }
            set
            {
                if (model.VideoFileName != value)
                {
                    model.VideoFileName = value;


                    this._mediaElementPlaying = true;

                    if (File.Exists(model.VideoFileName))
                    {
                        this.streamVideoPlayerWpf.OpenUrl(model.VideoFileName);
                    }

                    // ReSharper disable once UseNameofExpression
                    OnPropertyChanged(@"VideoFileName");
                }

            }
        }

        public float DirtyTime
        {
            get { return _dirtyTime; }
            set
            {
                if (Math.Abs(_dirtyTime - value) < 0.00001f) return;
                _dirtyTime = value;

                DirtyTimeString = _dirtyTime.ToString();

                OnPropertyChanged(@"DirtyTime");
            }
        }

        public string DirtyTimeString
        {
            get
            {
                return _dirtyTimeString;
            }
            set
            {
                if (value == _dirtyTimeString) return;
                _dirtyTimeString = value;
                OnPropertyChanged(@"DirtyTimeString");
            }
        }

        #endregion

        #region Commands

        public ICommand OpenVideoFileCommand
        {
            get
            {
                return _openVideoFileCommand ?? (_openVideoFileCommand = new CommandHandler(() =>
                           {
                               var ofd = new OpenFileDialog
                               {
                                   Filter = "Видео *.mp4|*.mp4",
                                   Multiselect = false,
                                   CheckFileExists = true
                               };

                               if (ofd.ShowDialog() == true)
                               {
                                   this.VideoFileName = ofd.FileName;
                               }
                           },
                           true));
            }
            set { _openVideoFileCommand = value; }
        }

        public ICommand RegisterMarker
        {
            get
            {
                if (_registerMarker == null)
                {
                    _registerMarker = new CommandHandler(
                        (object o) =>
                        {
                            var p = 5;
                        }, true
                        );
                }
                return _registerMarker;
            }
            set { _registerMarker = value; }
        }

        #endregion

        #region Methods

        public void HandleKeyDown(KeyEventArgs e)
        {
            /* ... */
            var p = 5;


            //var ex = e.Key.ToString();

            //if (mediaElement.HasVideo)
            //{
            //    mediaElement.LoadedBehavior = MediaState.Manual;
            //    if (ex == Options.G.Hotkey_PauseResume.ToString())
            //    {
            //        PlayPauseMediaPlayer();
            //    }
            //}
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public void VideoRateSliderShanged(Slider slider)
        {
            
        }
    }

    public class MainFormModel
    {
        public string VideoFileName { get; set; }


    }


    public class CommandHandler : ICommand
    {
        private Action _action;
        private bool _canExecute;
        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }


    
}
