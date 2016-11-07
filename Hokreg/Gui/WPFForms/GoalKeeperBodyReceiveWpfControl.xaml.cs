using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Логика взаимодействия для GoalKeeperBodyReceiveWpfControl.xaml
    /// </summary>
    public partial class GoalKeeperBodyReceiveWpfControl : UserControl
    {
        public GoalKeeperBodyReceiveWpfControl()
        {
            InitializeComponent();

            Result = GoalKeeperBodyEnum.None;

            all_ellipses = new List<Ellipse>
            {
                this.left_shoulder,
                this.right_shoulder,
                this.left_shit,
                this.right_shit,
                this.body,
                this.trap,
                this.podyshka,
                this.right_top_shield,
                this.right_mid_shield,
                this.right_bot_shield,
                this.left_top_shield,
                this.left_mid_shield,
                this.left_bot_shield,
                this.stick_top,
                this.stick_bot
            };

            MainGrid.Background =
                new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.GoalKeeperBody.GetHbitmap(),
                    IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));


            //< Grid.Background >
            //    < ImageBrush ImageSource = "../../Resources/GoalKeeperBody.png" />

            // </ Grid.Background >

        }

        public GoalKeeperBodyEnum Result;

        private List<Ellipse> all_ellipses;

        private void Ellipse_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var ellipse = sender as Ellipse;

            if (ellipse == null)
            {
                return;
            }

            var ellipseName = ellipse.Name;

            Ellipse selected_ellipse = null;
            switch (ellipseName)
            {
                case "right_shoulder":
                    selected_ellipse = right_shoulder;
                    Result = GoalKeeperBodyEnum.RightShoulder;
                    break;
                case "left_shoulder":
                    selected_ellipse = left_shoulder;
                    Result = GoalKeeperBodyEnum.LeftShoulder;
                    break;
                case "trap":
                    selected_ellipse = trap;
                    Result = GoalKeeperBodyEnum.Trap;
                    break;
                case "stick_top":
                    selected_ellipse = stick_top;
                    Result = GoalKeeperBodyEnum.StickTop;
                    break;
                case "body":
                    selected_ellipse = body;
                    Result = GoalKeeperBodyEnum.Body;
                    break;
                case "podyshka":
                    selected_ellipse = podyshka;
                    Result = GoalKeeperBodyEnum.Podyshka;
                    break;
                case "right_shit":
                    selected_ellipse = right_shit;
                    Result = GoalKeeperBodyEnum.RightShit;
                    break;
                case "left_shit":
                    selected_ellipse = left_shit;
                    Result = GoalKeeperBodyEnum.LeftShit;
                    break;
                case "right_top_shield":
                    selected_ellipse = right_top_shield;
                    Result = GoalKeeperBodyEnum.RightTopShield;
                    break;
                case "left_top_shield":
                    selected_ellipse = left_top_shield;
                    Result = GoalKeeperBodyEnum.LeftTopShield;
                    break;
                case "right_mid_shield":
                    selected_ellipse = right_mid_shield;
                    Result = GoalKeeperBodyEnum.RightMidShield;
                    break;
                case "left_mid_shield":
                    selected_ellipse = left_mid_shield;
                    Result = GoalKeeperBodyEnum.LeftMidShield;
                    break;
                case "right_bot_shield":
                    selected_ellipse = right_bot_shield;
                    Result = GoalKeeperBodyEnum.RightBotShield;
                    break;
                case "left_bot_shield":
                    selected_ellipse = left_bot_shield;
                    Result = GoalKeeperBodyEnum.LeftBotShield;
                    break;
                case "stick_bot":
                    selected_ellipse = stick_bot;
                    Result = GoalKeeperBodyEnum.StickBot;
                    break;
                default:
                    break;
            }

            if (selected_ellipse == null)
            {
                return;
            }

            SetEllipsesActivities(selected_ellipse);
        }

        private void SetEllipsesActivities(Ellipse selectedEllipse)
        {
            foreach (var ellipse in all_ellipses)
            {
                SetActiveEllipse(ellipse, Equals(ellipse, selectedEllipse));
            }
        }

        private void SetActiveEllipse(Ellipse ellipse, bool active)
        {
            if (active)
            {
                ellipse.Fill = new SolidColorBrush(
                    new Color()
                    {
                        A = 0xFF,
                        R = 0xFF,
                        G = 0xFF,
                        B = 0x0,
                    });
            }
            else
            {
                ellipse.Fill = new SolidColorBrush(
                    new Color()
                    {
                        A = 0xFF,
                        R = 0xC0,
                        G = 0xC0,
                        B = 0xC0,
                    });

                
            }
        }

        private void Ellipse_OnMouseEnter(object sender, MouseEventArgs e)
        {
            var ellipse = sender as Ellipse;

            if (ellipse == null)
            {
                return;
            }

            var ellipseName = ellipse.Name;

            Ellipse ellipse_mouse_over = null;
            var ellipse_name_over = GoalKeeperBodyEnum.None;

            switch (ellipseName)
            {
                case "right_shoulder":
                    ellipse_mouse_over = right_shoulder;
                    ellipse_name_over = GoalKeeperBodyEnum.RightShoulder;
                    break;
                case "left_shoulder":
                    ellipse_mouse_over = left_shoulder;
                    ellipse_name_over = GoalKeeperBodyEnum.LeftShoulder;
                    break;
                case "trap":
                    ellipse_mouse_over = trap;
                    ellipse_name_over = GoalKeeperBodyEnum.Trap;
                    break;
                case "stick_top":
                    ellipse_mouse_over = stick_top;
                    ellipse_name_over = GoalKeeperBodyEnum.StickTop;
                    break;
                case "body":
                    ellipse_mouse_over = body;
                    ellipse_name_over = GoalKeeperBodyEnum.Body;
                    break;
                case "podyshka":
                    ellipse_mouse_over = podyshka;
                    ellipse_name_over = GoalKeeperBodyEnum.Podyshka;
                    break;
                case "right_shit":
                    ellipse_mouse_over = right_shit;
                    ellipse_name_over = GoalKeeperBodyEnum.RightShit;
                    break;
                case "left_shit":
                    ellipse_mouse_over = left_shit;
                    ellipse_name_over = GoalKeeperBodyEnum.LeftShit;
                    break;
                case "right_top_shield":
                    ellipse_mouse_over = right_top_shield;
                    ellipse_name_over = GoalKeeperBodyEnum.RightTopShield;
                    break;
                case "left_top_shield":
                    ellipse_mouse_over = left_top_shield;
                    ellipse_name_over = GoalKeeperBodyEnum.LeftTopShield;
                    break;
                case "right_mid_shield":
                    ellipse_mouse_over = right_mid_shield;
                    ellipse_name_over = GoalKeeperBodyEnum.RightMidShield;
                    break;
                case "left_mid_shield":
                    ellipse_mouse_over = left_mid_shield;
                    ellipse_name_over = GoalKeeperBodyEnum.LeftMidShield;
                    break;
                case "right_bot_shield":
                    ellipse_mouse_over = right_bot_shield;
                    ellipse_name_over = GoalKeeperBodyEnum.RightBotShield;
                    break;
                case "left_bot_shield":
                    ellipse_mouse_over = left_bot_shield;
                    ellipse_name_over = GoalKeeperBodyEnum.LeftBotShield;
                    break;
                case "stick_bot":
                    ellipse_mouse_over = stick_bot;
                    ellipse_name_over = GoalKeeperBodyEnum.StickBot;
                    break;
                default:
                    break;
            }

            try
            {
                TextBlockMouseOver.Text = ConvertToString(ellipse_name_over);
                    //((XmlEnumAttribute)typeof(GoalKeeperBodyEnum).GetMember(ellipse_name_over.ToString())[0].GetCustomAttributes(typeof(XmlEnumAttribute), false)[0]).Name;
            }
            catch (Exception)
            {
                TextBlockMouseOver.Text = string.Empty;
            }

        }

        public static string ConvertToString(Enum e)
        {
            // Get the Type of the enum
            Type t = e.GetType();

            // Get the FieldInfo for the member field with the enums name
            FieldInfo info = t.GetField(e.ToString("G"));

            // Check to see if the XmlEnumAttribute is defined on this field
            if (!info.IsDefined(typeof(XmlEnumAttribute), false))
            {
                // If no XmlEnumAttribute then return the string version of the enum.
                return e.ToString("G");
            }

            // Get the XmlEnumAttribute
            object[] o = info.GetCustomAttributes(typeof(XmlEnumAttribute), false);
            XmlEnumAttribute att = (XmlEnumAttribute)o[0];
            return att.Name;
        }

        private void Ellipse_OnMouseLeave(object sender, MouseEventArgs e)
        {
            TextBlockMouseOver.Text = string.Empty;
        }

        // TODO: Сбросить отображение.
        public void ResetDisplay()
        {
            

        }

        // TODO: Изобразить выбранный
        public void ForseSetValue(GoalKeeperBodyEnum value)
        {
            

        }
    }

    public enum GoalKeeperBodyEnum
    {
        [XmlEnum("Ничего")]
        None, 

        [XmlEnum("Голова")]
        Golova = 1,

        [XmlEnum("Правое плечо")]
        RightShoulder = 3,
        [XmlEnum("Левое плечо")]
        LeftShoulder = 4,

        [XmlEnum("Ловушка")]
        Trap = 5,
        [XmlEnum("Подушка")]
        Podyshka = 9,

        [XmlEnum("Правая рука")]
        RightHand = 6,
        [XmlEnum("Левая рука")]
        LeftHand = 8,

        [XmlEnum("Тело")]
        Body = 7,

        [XmlEnum("Верх правого щитка")]
        RightTopShield = 10,
        [XmlEnum("Середина правого щитка")]
        RightMidShield = 12,
        [XmlEnum("Низ правого щитка")]
        RightBotShield = 13,


        [XmlEnum("Верх левого щитка")]
        LeftTopShield = 11,
        [XmlEnum("Середина левого щитка")]
        LeftMidShield = 14,
        [XmlEnum("Низ левого щитка")]
        LeftBotShield = 16,

        [XmlEnum("Клюшка(основная часть)")]
        StickTop = 2,
        [XmlEnum("Клюшка(нижняя часть)")]
        StickBot = 15,

        [XmlEnum("Правая зона")]
        RightShit = 17,
        [XmlEnum("Левая зона")]
        LeftShit = 18,
    }
}
