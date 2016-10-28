using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Uniso.InStat.Game;
using Uniso.InStat.Gui.WPF_Forms.MVVM;
using Uniso.InStat.Models;
using Uniso.InStat.StreamPlayer;

namespace Uniso.InStat.Gui.WPFForms
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Логика взаимодействия для WPFMainFormControl.xaml
    /// </summary>
    public partial class WPFMainFormControl : UserControl
    {

        private WpfMainFormViewModel viewModel;
        private DispatcherTimer timer;


        public WPFMainFormControl()
        {
            InitializeComponent();



            //viewModel = new WpfMainFormViewModel(MediaElement, timer);
            viewModel = new WpfMainFormViewModel(streamVideoPlayerWpf, this);

            this.DataContext = viewModel;
        }

        

        private void TestClickCommand(object sender, RoutedEventArgs e)
        {
            var p = 5;
        }

        private void VideoRateSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (viewModel == null) return;
            this.viewModel.VideoRateSliderShanged(sender as Slider);
        }


        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (viewModel == null) return;
            viewModel.HandleKeyDown(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.streamVideoPlayerWpf.MediaPlayer_PlayStateChange();
        }


    }


    public class MarkersListModel
    {
        public List<MarkerWithProperty> Markerks;

        public void InsertMarkerk(MarkerWithProperty mk)
        {
            this.Markerks.Add(mk);

            SortMarkers();
        }

        private void SortMarkers()
        {
            this.Markerks.Sort(CompareMarkersTime);
        }

        private int CompareMarkersTime(Game.Marker ma, Game.Marker mb)
        {
            if (ma.TimeVideo > mb.TimeVideo)
            {
                return 1;
            }

            if (ma.TimeVideo < mb.TimeVideo)
            {
                return -1;
            }

            if (ma.TimeActual > mb.TimeActual)
            {
                return 1;
            }

            if (ma.TimeActual < mb.TimeActual)
            {
                return -1;
            }

            return 0;
        }
    }


    public class MarkerWithProperty : Game.Marker
    {
        private string _actionString;

        #region Constructor

        public MarkerWithProperty(HockeyIce game) : base(game)
        {
        }

        public MarkerWithProperty(HockeyIce game, string action_code) : base(game, action_code)
        {
        }

        public MarkerWithProperty(HockeyIce game, int action_id, int action_type) : base(game, action_id, action_type)
        {
        }

        public MarkerWithProperty(HockeyIce game, int action_id, int action_type, Half half, int video_time) : base(game, action_id, action_type, half, video_time)
        {
        }

        #endregion
        
        public IEnumerable<MarkerPropertyField> PropFields { get; set; }

        public string ActionString
        {
            get
            {
                this._actionString = MarkersWomboCombo.ActionString(this.ActionCode);

                return this._actionString;
            }
        }

        public string TimeString
        {
            get { return string.Format("{0} / {1}", this.TimeVideo, this.TimeActual); }
        }

        public List<StageEnum> GetStages()
        {
            return MarkersWomboCombo.GetMarkersStage(this); ;
        }

        public string Period1
        {
            get { return this.Half.Index.ToString(); }
        }
    }

    public  class MarkerPropertyField
    {
        public MarkerkPropertyFieldEnum PropertyFieldEnum;

        public object Value;
    }


        /*
        Property_ID	Свойства(типы)
        1	Тип Броска
        2	Обзор Вратаря
        3	Стойка Вратаря
        4	Начало полета шайбы
	
        5	Куда принял шайбу
        6	Шайба коснулась вратаря
	
        7	Рука
        8	Точка X в воротах
        9	Точка Y в воротах
        10	Нарушения
        11	Голевая( 1й ассистент)
        12	Голевая( 2й ассистент)
        */
    public enum MarkerkPropertyFieldEnum
    {
        //1	Тип Броска
        ThrownType = 1,
        //2	Обзор Вратаря
        GoalkeeperView,
        //3	Стойка Вратаря
        GoalkeeperStand,
        //4	Начало полета шайбы
        StartWashersFly,
        //5	Куда принял шайбу
        WhereGetWashers,
        //6	Шайба коснулась вратаря
        WashersTouchGoalkeper,
        //7	Рука
        Hand,
        //8	Точка X в воротах
        PointGoalX,
        //9	Точка Y в воротах
        PointGoalY,
        //10	Нарушения
        FoulType,
        // ReSharper disable once InconsistentNaming
        //11	Голевая( 1й ассистент)
        Goal1stAssistant,
        // ReSharper disable once InconsistentNaming
        //12	Голевая( 2й ассистент)
        Goal2ndAssistant,
    }

}
