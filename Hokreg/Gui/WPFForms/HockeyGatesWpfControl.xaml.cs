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
            var canvasWidth = (int)this.CanvasGates.RenderSize.Width;
            var canvasHeight = (int)this.CanvasGates.RenderSize.Height;

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
            if (e.ChangedButton == MouseButton.Left)
            {
                var canvasWidth = (int)this.CanvasGates.RenderSize.Width;
                var canvasHeight = (int)this.CanvasGates.RenderSize.Height;

                Result = ConvertToGatesPoint(e.GetPosition(CanvasGates), canvasWidth, canvasHeight);

                Canvas.SetTop(SelectedPointElipse, e.GetPosition(CanvasGates).Y - this.SelectedPointElipse.Height / 2f);
                Canvas.SetLeft(SelectedPointElipse, e.GetPosition(CanvasGates).X - this.SelectedPointElipse.Width / 2f);

                SelectedPointElipse.Visibility = Visibility.Visible;
            }
        }

        private PointF ConvertToGatesPoint(System.Windows.Point point, int width, int height)
        {
            double xp1 = width - point.X;
            double yp = height - point.Y;
            double xp = width / 2f - xp1;

            double koefX = GoalSize.Width/width;
            double koefY = GoalSize.Height / height;

            double x = (double)(xp*koefX);
            double y = (double)(yp*koefY);

            return new PointF((float) x, (float) y);
        }

        private PointF ConvertFromGatesPoint(PointF point, int width, int height)
        {
            double koefX = GoalSize.Width / width;
            double koefY = GoalSize.Height / height;

            double x = point.X/koefX + width / 2f;

            double y = height - point.Y / koefY;
            
            return new PointF((float) x, (float) y);
        }


        private string DisplayStringResult(PointF point)
        {
            var x = point.X;
            var y = point.Y;

            return string.Format("{{ {0} , {1} }}", x.ToString("0.00"), y.ToString("0.00"));
        }
        
        public void ResetDisplay()
        {
            this.Result = null;

            SelectedPointElipse.Visibility = Visibility.Hidden;
        }

        public void ForseSetValue(PointF value)
        {
            Result = value;

            var canvasWidth = (int)(this.CanvasGates.RenderSize).Width;
            var canvasHeight = (int)(this.CanvasGates.RenderSize).Height;

            var point = ConvertFromGatesPoint(Result.Value, canvasWidth, canvasHeight);

            Canvas.SetTop(SelectedPointElipse, point.Y - this.SelectedPointElipse.Height / 2);
            Canvas.SetLeft(SelectedPointElipse, point.X - this.SelectedPointElipse.Width / 2);

            SelectedPointElipse.Visibility = Visibility.Visible;

            PointTextBlock.Text = DisplayStringResult(Result.Value);
        }
    }
}
