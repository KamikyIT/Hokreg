using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
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
        private ICommand _registerMarker123;
        private ICommand _registerMarkerCommnd;
        private ObservableCollection<MarkerWithProperty> _markerWithPropertiesCollection;
        private MarkerWithProperty _selectedMarkerFromDataGrid;


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

            _registerMarkerCommnd = new CommandHandlerWithParam(RegisterMarkerMethod, CanExecute);

            timer = new DispatcherTimer();

            timer.Tick += TimerOnTick;

            timer.Start();

            _markerWithPropertiesCollection = new ObservableCollection<MarkerWithProperty>();

            Test();
        }

        private void Test()
        {
            for (int i = 0; i < 10; i++)
            {
                var mk = new MarkerWithProperty(null)
                {
                    ActionCode = 300100,
                    Point1 = new PointF(1f, 10f),
                    Player1 = new Player()
                    {
                        Name = "aaa",
                    },
                    Player2 = new Player()
                    {
                        Name = "bbb",
                    },
                    Point2 = new PointF(5f, 5f),
                    Id = i,
                    Half = new Half(new List<Period>()
                    {
                        new Period()
                        {
                            Index = 1,
                            Length = 100,
                        },
                    }),
                    
                };

                MarkerWithPropertiesCollection.Add(mk);
            }
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

        public ObservableCollection<MarkerWithProperty> MarkerWithPropertiesCollection
        {
            get { return this._markerWithPropertiesCollection; }
        }

        public MarkerWithProperty SelectedMarkerFromDataGrid
        {
            get { return _selectedMarkerFromDataGrid; }
            set
            {
                if (_selectedMarkerFromDataGrid == value)
                    return;

                _selectedMarkerFromDataGrid = value;
                OnPropertyChanged(@"SelectedMarkerFromDataGrid");


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


        private bool CanExecute(object o)
        {
            var p = 5;
            return true;
        }

        private void RegisterMarkerMethod(object o)
        {
            var p = 5;
        }

        public ICommand RegisterMarker123
        {
            get
            {
                if (_registerMarker123 == null)
                {
                    _registerMarker123 = new CommandHandler(
                        () =>
                        {
                            var p = 5;
                        }, true
                        );
                }
                return _registerMarker123;
            }
            set { _registerMarker123 = value; }
        }


        private void RegisterMarker(object action_code)
        {
            var p = 5;
        }

        // ReSharper disable once ConvertToAutoProperty
        public ICommand RegisterMarkerCommand
        {
            // ReSharper disable once ConvertPropertyToExpressionBody
            get { return _registerMarkerCommnd; }
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

    public class CommandHandlerWithParam : ICommand
    {
        public delegate bool CanExecuteDelegate(object obj);

        private Action<object> _action;
        private CanExecuteDelegate _canExecute;

        public CommandHandlerWithParam(Action<object> method, CanExecuteDelegate canExecute)
        {
            this._action = method;
            this._canExecute = canExecute;
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }


}
