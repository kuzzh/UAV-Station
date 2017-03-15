using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace SanHeGroundStation.Controls
{
    public partial class HUD : UserControl
    {
        Graphics g;
        Pen greenPen_1 = new Pen(Color.Green, 1);
        Pen greenPen_2 = new Pen(Color.Green, 2);
        Pen yellowPen = new Pen(Color.FromArgb(178, 173, 3), 1);
        Pen whitePen = new Pen(Color.White, 2);
        Pen redPen = new Pen(Color.Red);
        Pen greenPen_3 = new Pen(Color.FromArgb(38, 198, 1), 2);
        Font font = new Font("Arial", 10, FontStyle.Regular);
        Font yellowFont = new Font("Arial", 10, FontStyle.Bold);
        SolidBrush greenBrush = new SolidBrush(Color.FromArgb(98, 133, 98));
        SolidBrush yellowBrush = new SolidBrush(Color.FromArgb(178, 173, 3));
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        public float roll = 0;
        public float speed = 0;
        public float alt = 0;
        public float pitch = 0;
        public float yaw = 0;
        Bitmap bmp = new Bitmap(12, 12);
        public string mode=null;
        public HUD()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.BackColor = Color.FromArgb(8, 18, 8);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.DesignMode)
            {
                e.Graphics.Clear(this.BackColor);
                e.Graphics.Flush();
            }
            if (g == null && (bmp.Width != this.Width || bmp.Height != this.Height))
            {
                bmp = new Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                //bmp.MakeTransparent();
                g = Graphics.FromImage(bmp);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.CompositingMode = CompositingMode.SourceOver;
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                
            }
            //g.InterpolationMode = InterpolationMode.Bilinear;
            g.Clear(Color.Transparent);
            yellowPen.Color = Color.Yellow;
            int longLineStartX = 37;
            int lineLength = 20;
            int lineEndX = longLineStartX + lineLength;
            int shortLineStartX = 47;
            int stringX = longLineStartX - 30;

            #region 画速度
            //画速度
            g.ResetTransform();
            g.TranslateTransform(0, this.Height / 2);
            Point[] sanJiaoXing = new Point[3];
            sanJiaoXing[0] = new Point(25, 8);
            sanJiaoXing[1] = new Point(35, 0);
            sanJiaoXing[2] = new Point(25, -8);
            g.DrawPolygon(greenPen_1, sanJiaoXing);
            for (int i = (int)speed - 10; i <= (int)speed + 10; i++)
            {

                if (i > speed)//在上面
                {
                    if (i % 5 == 0)
                    {
                        g.DrawLine(greenPen_2, longLineStartX, -Math.Abs(speed - i) * 10, lineEndX, -Math.Abs(speed - i) * 10);
                        g.DrawString(i.ToString().PadLeft(2), font, greenBrush, new PointF(stringX, -Math.Abs(speed - i) * 10 - 5));
                    }
                    else
                    {
                        g.DrawLine(greenPen_1, shortLineStartX, -Math.Abs(speed - i) * 10, lineEndX, -Math.Abs(speed - i) * 10);
                    }
                }
                else//在下面
                {
                    if (i % 5 == 0)
                    {
                        g.DrawLine(greenPen_2, longLineStartX, Math.Abs(speed - i) * 10, lineEndX, Math.Abs(speed - i) * 10);
                        g.DrawString(i.ToString().PadLeft(2), font, greenBrush, new PointF(stringX, Math.Abs(speed - i) * 10 - 5));
                    }
                    else
                    {
                        g.DrawLine(greenPen_1, shortLineStartX, Math.Abs(speed - i) * 10, lineEndX, Math.Abs(speed - i) * 10);
                    }
                }
            }
            g.DrawString(("飞行模式："+mode).ToString(), yellowFont, yellowBrush, new PointF(stringX - 0, 150));

           // g.DrawString(("当前时间：" + DateTime.Now.ToString("HH:mm:ss")).ToString(), yellowFont, yellowBrush, new PointF(stringX + 250, 150));
            g.ResetTransform();
            g.TranslateTransform(this.Width, this.Height / 2);
            #endregion;

            #region 画高度
            //画高度
            sanJiaoXing[0] = new Point(-25, 8);
            sanJiaoXing[1] = new Point(-35, 0);
            sanJiaoXing[2] = new Point(-25, -8);
            g.DrawPolygon(greenPen_1, sanJiaoXing);
            longLineStartX = -37;
            lineEndX = longLineStartX - lineLength;
            shortLineStartX = longLineStartX - 10;
            stringX = longLineStartX + 15;
            for (int i = (int)alt - 10; i <= (int)alt + 10; i++)
            {
                if (i > alt)//在上面
                {
                    if (i % 5 == 0)//画长线
                    {
                        g.DrawLine(greenPen_2, longLineStartX, -Math.Abs(alt - i) * 10, lineEndX, -Math.Abs(alt - i) * 10);
                        g.DrawString(i.ToString().PadLeft(2), font, greenBrush, new PointF(stringX, -Math.Abs(alt - i) * 10 - 5));
                    }
                    else
                    {
                        g.DrawLine(greenPen_1, shortLineStartX, -Math.Abs(alt - i) * 10, lineEndX, -Math.Abs(alt - i) * 10);
                    }
                }
                else
                {
                    if (i % 5 == 0)
                    {
                        g.DrawLine(greenPen_2, longLineStartX, Math.Abs(alt - i) * 10, lineEndX, Math.Abs(alt - i) * 10);
                        g.DrawString(i.ToString().PadLeft(2), font, greenBrush, new PointF(stringX, Math.Abs(alt - i) * 10 - 5));
                    }
                    else
                    {
                        g.DrawLine(greenPen_1, shortLineStartX, Math.Abs(alt - i) * 10, lineEndX, Math.Abs(alt - i) * 10);
                    }
                }
            }
           // g.DrawString("高度指示器".ToString(), yellowFont, yellowBrush, new PointF(lineEndX - 0, -175));
            #endregion

            #region 画表盘
            //画表盘
            int bianChang;
            if (this.Width > this.Height)
            {
                bianChang = (int)(this.Height / 1.5);
            }
            else
                bianChang = (int)(this.Width / 1.5);
            g.TranslateTransform(this.Width / 2, this.Height / 2);
            int[] array = new int[] { -60, -45, -30, -20, -10, 0, 10, 20, 30, 45, 60 };
            foreach (int a in array)
            {
                g.ResetTransform();
                g.TranslateTransform(this.Width / 2, this.Height / 2);
                g.RotateTransform(a - roll);
                g.DrawString(Math.Abs(a).ToString("0").PadLeft(2), font, whiteBrush, new PointF(0, -bianChang / 2 - 20));
                g.DrawLine(new Pen(Color.White, 2), 0, -bianChang / 2 - 4, 0, -bianChang / 2);
            }
            g.ResetTransform();
            g.TranslateTransform(this.Width / 2, this.Height / 2);
            RectangleF recatangle = new RectangleF(-bianChang / 2, -bianChang / 2, bianChang, bianChang);
            g.DrawArc(whitePen, recatangle, 180 + 29 - roll, 121);
            g.ResetTransform();
            //画表盘上的小三角形
            g.TranslateTransform(this.Width / 2, this.Height / 2);
            sanJiaoXing[0] = new Point(0, -bianChang / 2 + 2);
            sanJiaoXing[1] = new Point(-5, -bianChang / 2 + 10);
            sanJiaoXing[2] = new Point(5, -bianChang / 2 + 10);
            g.DrawPolygon(greenPen_3, sanJiaoXing);
            #endregion

            #region 画俯仰度
            //画俯仰度
            g.ResetTransform();
            g.TranslateTransform(this.Width / 2, this.Height / 2);
            sanJiaoXing[1] = new Point(0, 0);
            sanJiaoXing[2] = new Point(-20, 10);
            sanJiaoXing[0] = new Point(20, 10);
            g.DrawLines(redPen, sanJiaoXing);
            for (int i = (int)pitch - 5; i <= (int)pitch + 5; i++)
            {
                if (i > pitch)//在上面
                {
                    if (i % 10 == 0)
                    {
                        //白长线
                        g.DrawLine(greenPen_1, -20, (pitch - i) * 20, -120, (pitch - i) * 20);
                        g.DrawLine(greenPen_1, 20, (pitch - i) * 20, 120, (pitch - i) * 20);
                        g.DrawString(i.ToString().PadLeft(2), font, greenBrush, -120, (pitch - i) * 20);
                        g.DrawString(i.ToString().PadLeft(2), font, greenBrush, 120, (pitch - i) * 20);
                    }
                    else if (i % 5 == 0)
                    {
                        //白短线
                        g.DrawLine(greenPen_1, -20, (pitch - i) * 20, -80, (pitch - i) * 20);
                        g.DrawLine(greenPen_1, 20, (pitch - i) * 20, 80, (pitch - i) * 20);
                        g.DrawString(i.ToString().PadLeft(2), font, greenBrush, -80, (pitch - i) * 20);
                        g.DrawString(i.ToString().PadLeft(2), font, greenBrush, 80, (pitch - i) * 20);
                    }
                }
                else//在下面
                {
                    if (i % 10 == 0)
                    {
                        //白长线
                        g.DrawLine(greenPen_1, -20, (pitch - i) * 20, -120, (pitch - i) * 20);
                        g.DrawLine(greenPen_1, 20, (pitch - i) * 20, 120, (pitch - i) * 20);
                        g.DrawString(i.ToString().PadLeft(2), font, greenBrush, -120, (pitch - i) * 20);
                        g.DrawString(i.ToString().PadLeft(2), font, greenBrush, 120, (pitch - i) * 20);
                    }
                    else if (i % 5 == 0)
                    {
                        //白短线
                        g.DrawLine(greenPen_1, -20, (pitch - i) * 20, -80, (pitch - i) * 20);
                        g.DrawLine(greenPen_1, 20, (pitch - i) * 20, 80, (pitch - i) * 20);
                        g.DrawString(i.ToString().PadLeft(2), font, greenBrush, -80, (pitch - i) * 20);
                        g.DrawString(i.ToString().PadLeft(2), font, greenBrush, 80, (pitch - i) * 20);
                    }
                }
            }
            #endregion

            #region 画航向
            //画航向
            g.ResetTransform();
            g.TranslateTransform(this.Width / 2, 0);
            g.DrawLine(greenPen_3, -this.Width / 2, 20, this.Width / 2, 20);
            int distance = this.Width / 120;
            for (int i = (int)yaw - 60; i <= (int)yaw + 60; i++)
            {
                if (i % 15 == 0)
                {
                    g.DrawLine(greenPen_3, distance * (i - yaw), 13, distance * (i - yaw), 20);
                    g.DrawString(i.ToString(), font, greenBrush, new PointF(distance * (i - yaw) - 5, 20));
                }
                else if (i % 5 == 0)
                {
                    g.DrawLine(greenPen_3, distance * (i - yaw), 16, distance * (i - yaw), 20);
                }
            }
            //画当前指向标记
            Rectangle marker = new Rectangle(-10, 0, 20, 35);
            g.FillRectangle(new SolidBrush(Color.FromArgb(80, Color.White)), marker);
            g.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(80, Color.White)), 2), marker);
            #endregion

            //Graphics g = this.CreateGraphics();
            //Rectangle rect = new Rectangle(10, 10, 50, 50);//定义矩形,参数为起点横纵坐标以及其长和宽
            //SolidBrush b1 = new SolidBrush(Color.White);//定义单色画刷
            //g.DrawString("事件。。。", new Font("宋体", 10), b1, this.Width / 2, this.Height / 2);

            e.Graphics.DrawImage(bmp, 0, 0);
           

        }
    }
}
