using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Uniso.InStat.Game;

namespace Uniso.InStat.Gui
{
    public partial class GoalPointForm : Form
    {
        private Point point = Point.Empty;
        private HockeyIce game = null;

        public Marker Marker { get; set; }
        
        public GoalPointForm(HockeyIce game, Marker mk)
        {
            InitializeComponent();
            Marker = mk;
            this.game = game;
        }

        public Point TransformV2F(Point pt)
        {
            return new Point(
                Convert.ToInt32(((float)pt.X / ((float)pictureBox1.Width / 2.0f) - 1) * game.GoalSize.Width * 50.0f),
                Convert.ToInt32((1 - (float)pt.Y / ((float)pictureBox1.Height)) * game.GoalSize.Height * 100.0f));
        }

        public Point TransformF2V(PointF pt)
        {
            var w2 = (float)pictureBox1.Width / 2.0f;
            var h2 = game.GoalSize.Height * 100f;
            return new Point(
                Convert.ToInt32(pt.X / (game.GoalSize.Width * 50f) * w2 + w2),
                Convert.ToInt32((1 - pt.Y / h2) * (float)pictureBox1.Height));
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Marker.Point1 = TransformV2F(e.Location);
            pictureBox1.Invalidate();
            Close();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            point = e.Location;
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            if (!Marker.Point1.IsEmpty)
            {
                var pt = TransformF2V(Marker.Point1);
                e.Graphics.FillEllipse(new SolidBrush(Color.Lime), new Rectangle(pt.X - 10, pt.Y - 10, 20, 20));
                e.Graphics.DrawEllipse(new Pen(Color.Black), new Rectangle(pt.X - 10, pt.Y - 10, 20, 20));
            }
            if (!point.IsEmpty)
            {
                var pt = point;
                e.Graphics.DrawEllipse(new Pen(Color.Black), new Rectangle(pt.X - 10, pt.Y - 10, 20, 20));
            }
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            point = Point.Empty;
            pictureBox1.Invalidate();
        }
    }
}
