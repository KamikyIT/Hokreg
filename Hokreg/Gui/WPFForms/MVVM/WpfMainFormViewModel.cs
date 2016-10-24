using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private MediaElement mediaElement;
        private bool _mediaElementPlaying;
        private DispatcherTimer timer;

        #endregion

        #region Constructor

        public WpfMainFormViewModel()
        {
            model = new MainFormModel();
            _canOpenVideoFile = true;
        }

        public WpfMainFormViewModel(MediaElement mediaElement): this()
        {
            this.mediaElement = mediaElement;
        }

        public WpfMainFormViewModel(MediaElement mediaElement, DispatcherTimer timer) : this(mediaElement)
        {
            this.timer = timer;

            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += TimerOnTick;
            timer.Start();

            //dispatcherTimer.Tick += dispatcherTimer_Tick;
            //dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            //dispatcherTimer.Start();
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

                    // ReSharper disable once UseNameofExpression
                    OnPropertyChanged(@"VideoFileName");
                }

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

        

        #endregion

        #region Methods

        public void HandleKeyDown(KeyEventArgs e)
        {
            /* ... */
            var p = 5;


            var ex = e.Key.ToString();

            if (mediaElement.HasVideo)
            {
                mediaElement.LoadedBehavior = MediaState.Manual;

                if (ex == Options.G.Hotkey_PauseResume.ToString())
                {
                    PlayPauseMediaPlayer();
                }


            }



            //if (e.KeyData == Options.G.Hotkey_PauseResume)
            //{
            //    if (vlcStreamPlayer1.Mode != StreamPlayer.PlayerMode.Stop)
            //    {
            //        vlcStreamPlayer1.Mode =
            //            vlcStreamPlayer1.Mode == StreamPlayer.PlayerMode.Play
            //            ? StreamPlayer.PlayerMode.Pause
            //            : StreamPlayer.PlayerMode.Play;
            //    }

            //    return;
            //}
        }

        private void PlayPauseMediaPlayer()
        {
            if (_mediaElementPlaying)
            {
                mediaElement.Pause();
                _mediaElementPlaying = false;
            }
            else
            {
                mediaElement.Play();
                _mediaElementPlaying = true;
            }
        }
        
        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            if (mediaElement.HasVideo == false)
            {
                return;
            }

             mediaElement.Clock.CurrentTime
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
