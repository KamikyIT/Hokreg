using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace Uniso.InStat.Gui.WPFForms
{
    /// <summary>
    /// Логика взаимодействия для GoalKeeperStateWpfControl.xaml
    /// </summary>
    public partial class GoalKeeperStateWpfControl : UserControl
    {
        public GoalKeeperStateWpfControl()
        {
            InitializeComponent();

            allEllipses = new List<Ellipse>
            {
                this.nad_blin_ellipse,
                this.nad_trap_ellipse,
                this.pod_blin_ellipse,
                this.pod_trap_ellipse,
                this.v_telo_ellipse,
                this.v_dom_ellipse
            };

            ResultState = GateWayThrowEnum.none;

            MainGrid.Background =
                new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.goalkeeperstate.GetHbitmap(),
                    IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));
            
        }

        public GateWayThrowEnum ResultState { get; set; }

        private void SetEllipsesActivities(Ellipse selected_ellipse)
        {
            if (selected_ellipse == null)
            {
                return;
            }

            foreach (var ellipse in allEllipses)
            {
                SetActiveEllipse(ellipse, Equals(ellipse, selected_ellipse));
            }
        }


        private void SetActiveEllipse(Ellipse ellipse, bool active)
        {
            if (active)
            {
                ellipse.Fill = new SolidColorBrush(new Color()
                {
                    A = 0xFF,
                    R = 0xFF,
                    G = 0xFF,
                    B = 0x0,
                });

                
            }
            else
            {
                ellipse.Fill = new SolidColorBrush(new Color()
                {
                    A = 0xFF,
                    R = 0xC0,
                    G = 0xC0,
                    B = 0xC0,
                });

            }
        }
        
        private List<Ellipse> allEllipses;

        private void Ellipse_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var ellipse = (sender as Viewbox);

            if (ellipse == null)
            {
                return;
            }

            var ellipseName = ellipse.Name;

            Viewbox active_viewbox = null;
            switch (ellipseName)
            {
                case "nad_blin":
                    active_viewbox = nad_blin;
                    ResultState = GateWayThrowEnum.nad_blin;
                    break;
                case "nad_trap":
                    active_viewbox = nad_trap;
                    ResultState = GateWayThrowEnum.nad_trap;
                    break;
                case "v_telo":
                    active_viewbox = v_telo;
                    ResultState = GateWayThrowEnum.v_telo;
                    break;
                case "pod_blin":
                    active_viewbox = pod_blin;
                    ResultState = GateWayThrowEnum.pod_blin;
                    break;
                case "pod_trap":
                    active_viewbox = pod_trap;
                    ResultState = GateWayThrowEnum.pod_trap;
                    break;
                case "v_dom":
                    active_viewbox = v_dom;
                    ResultState = GateWayThrowEnum.v_dom;
                    break;
                default:
                    break;
            }

            var view_grid = (active_viewbox.Child as Grid);

            var actives = view_grid.Children.OfType<Ellipse>();

            var active_ellipse = actives.FirstOrDefault();

            if (active_ellipse != null)
            {
                this.SetEllipsesActivities(active_ellipse);
            }
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum GateWayThrowEnum
    {
        none,
        nad_blin,
        nad_trap,
        v_telo,
        pod_blin,
        pod_trap,
        v_dom,
    }

    
}
