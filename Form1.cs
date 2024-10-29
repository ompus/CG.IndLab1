using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndLab1.CG
{
    public partial class Form1 : Form
    {
        Bitmap bmp;
        Graphics g;
        List<Point> list = new List<Point>();
        HashSet<(Point, Point)> liveEdges = new HashSet<(Point, Point)>();
        public Form1()
        {
            InitializeComponent();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bmp;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            g = Graphics.FromImage(pictureBox1.Image);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Delaunay();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ClearPictureBox();
            list.Clear();
        }

        // Очистка холста
        private void ClearPictureBox()
        {
            g.Clear(pictureBox1.BackColor);
            pictureBox1.Image = bmp;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            g.FillEllipse(Brushes.Red, e.X - 3, e.Y - 3, 6, 6);
            list.Add(new Point(e.X, e.Y));
            pictureBox1.Image = bmp;
        }

        private void Delaunay()
        {
            Point start = new Point(int.MaxValue, int.MaxValue);
            Point newPoint = new Point(int.MaxValue, int.MaxValue);
            for (int i = 0; i < list.Count(); ++i)
            {
                if (list[i].X < start.X)
                {
                    start.X = list[i].X;
                    start.Y = list[i].Y;
                }
                if (list[i].X == start.X && list[i].Y > start.Y)
                {
                    start.X = list[i].X;
                    start.Y = list[i].Y;
                }
            }

            double cos = int.MinValue;
            for (int i = 0; i < list.Count(); ++i)
            {
                if (list[i].X != start.X && list[i].Y != start.Y)
                {
                    double coss = (start.Y -list[i].Y) / 
                        Math.Sqrt(Math.Pow(list[i].Y - start.Y, 2) + Math.Pow(list[i].X - start.X, 2));
                    if (coss > cos)
                    {
                        newPoint.X = list[i].X;
                        newPoint.Y = list[i].Y;
                        cos = coss;
                    }
                }
            }

            var pen = new Pen(Color.Yellow, 2);
            g.DrawLine(pen, start, newPoint);
            pen.Dispose();
            liveEdges.Add((start, newPoint));

            while (liveEdges.Count != 0)
            {
                double min = double.MaxValue;
                int index = -1;
                Point st = liveEdges.Last().Item1;
                Point fin = liveEdges.Last().Item2;
                double minD = 0.0;

                for (int i = 0; i < list.Count(); ++i)
                {
                    if (list[i].X != st.X && list[i].Y != st.Y && list[i].X != fin.X && list[i].Y != fin.Y && 
                        ((fin.X - st.X)*(st.Y - fin.Y)*((list[i].X - st.X) / (double)(fin.X - st.X) - (list[i].Y - st.Y) / (double)(fin.Y - st.Y)) > 0))
                    {
                        Point a = new Point((list[i].X + st.X) / 2, (list[i].Y + st.Y) / 2);
                        Point b = new Point((list[i].X + fin.X) / 2, (list[i].Y + fin.Y) / 2);
                        double l = (double)(list[i].X - st.X) / (list[i].Y - st.Y);
                        double k = (double)(fin.X - list[i].X) / (-list[i].Y + fin.Y);
                        double x = (k* b.X - l * a.X - a.Y + b.Y) / (k - l);
                        double y = Math.Abs(l*(x - a.X) - a.Y);
                        minD = Math.Sqrt(Math.Pow(x - st.X, 2) + Math.Pow(y - st.Y, 2)) +
                                            Math.Sqrt(Math.Pow(x - fin.X, 2) + Math.Pow(y - fin.Y, 2)) +
                                            Math.Sqrt(Math.Pow(x - list[i].X, 2) + Math.Pow(y - list[i].Y, 2));
                        if ((fin.X - st.X)*(st.Y - fin.Y)*((x - st.X) / (double)(fin.X - st.X) - (y - st.Y) / (double)(fin.Y - st.Y)) < 0)
                            minD = -minD;
                        if (minD < min)
                        {
                            min = minD;
                            index = i;
                        }
                    }
                }
                liveEdges.Remove(liveEdges.Last());
                var pen12 = new Pen(Color.Black, 3);
                g.DrawLine(pen12, st, fin);
                pen12.Dispose();
                
                if (index != -1)
                {
                    if (!liveEdges.Contains((st, list[index])) && !liveEdges.Contains((list[index], st)))
                    {
                        var pen1 = new Pen(Color.Gray, 2);
                        g.DrawLine(pen1, st, list[index]);
                        pen1.Dispose();
                        liveEdges.Add((st, list[index]));
                    }

                    else
                    {
                        liveEdges.Remove((st, list[index]));
                        liveEdges.Remove((list[index], st));
                        var pen1 = new Pen(Color.Black, 3);
                        g.DrawLine(pen1, st, list[index]);
                        pen1.Dispose();
                    }
                        

                    if (!liveEdges.Contains((list[index], fin))&& !liveEdges.Contains((fin, list[index])))
                    {
                        var pen1 = new Pen(Color.Green, 1);
                        g.DrawLine(pen1, list[index], fin);
                        pen1.Dispose();
                        liveEdges.Add((list[index], fin));
                    }

                    else
                    {
                        liveEdges.Remove((list[index], fin));
                        liveEdges.Remove((fin, list[index]));
                        var pen1 = new Pen(Color.Black, 3);
                        g.DrawLine(pen1, fin, list[index]);
                        pen1.Dispose();
                    }
                                         
                }
                pictureBox1.Image = bmp;
            }
        }
    }
}