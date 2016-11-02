using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Uniso.InStat.Gui.WPFForms
{
    /// <summary>
    /// Логика взаимодействия для HockeyGatesWpfControl.xaml
    /// </summary>
    public partial class HockeyGatesWpfControl : UserControl
    {

        public PointF? Result;

        public HockeyGatesWpfControl()
        {
            InitializeComponent();

            GoalSize = new SizeF(1.83f, 1.22f);

            CanvasGates.Background =
                    new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.HockeyGateway.GetHbitmap(),
                    IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));
        }

        public SizeF GoalSize { get; set; }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var point = e.GetPosition(sender as IInputElement);
            var canvasWidth = (int) ((sender as Canvas).RenderSize).Width;
            var canvasHeight = (int) ((sender as Canvas).RenderSize).Height;

            var outPoint = ConvertToGatesPoint(point, canvasWidth, canvasHeight);


            PointTextBlock.Text = DisplayStringResult(outPoint);
        }

        private void Canvas_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (Result.HasValue)
            {
                PointTextBlock.Text = DisplayStringResult(Result.Value);
            }
            else
            {
                PointTextBlock.Text = @"Введите значение";
            }
        }

        private void Canvas_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var canvasWidth = (int)((sender as Canvas).RenderSize).Width;
            var canvasHeight = (int)((sender as Canvas).RenderSize).Height;

            Result = ConvertToGatesPoint(e.GetPosition(CanvasGates), canvasWidth, canvasHeight);

            Canvas.SetTop(SelectedPointElipse, e.GetPosition(CanvasGates).Y - this.SelectedPointElipse.Height / 2);
            Canvas.SetLeft(SelectedPointElipse, e.GetPosition(CanvasGates).X - this.SelectedPointElipse.Width / 2);

            SelectedPointElipse.Visibility = Visibility.Visible;
        }

        private PointF ConvertToGatesPoint(System.Windows.Point point, int width, int height)
        {
            var xp1 = width - point.X;
            var yp = height - point.Y;
            var xp = width / 2 - xp1;

            var koefX = GoalSize.Width/width;
            var koefY = GoalSize.Width/width;

            var x = (float) (xp*koefX);
            var y = (float) (yp*koefY);

            return new PointF(x, y);
        }


        private string DisplayStringResult(PointF point)
        {
            var x = point.X;
            var y = point.Y;

            return string.Format("{{ {0} , {1} }}", x.ToString("0.00"), y.ToString("0.00"));
        }
    }
}
