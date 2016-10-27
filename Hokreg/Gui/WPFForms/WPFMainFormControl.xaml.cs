using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Uniso.InStat.Gui.WPF_Forms.MVVM;
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
}
