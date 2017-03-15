using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SanHeGroundStation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] month = new string[12] { "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月" };
            float[] d = new float[12] { 20.5F, 60, 10.8F, 15.6F, 30, 70.9F, 50.3F, 30.7F, 70, 50.4F, 30.8F, 20 };
            //float[] d = new float[12];
            //画图初始化   
            Bitmap bMap = new Bitmap(500, 500);
            Graphics gph = Graphics.FromImage(bMap);
            gph.Clear(Color.White);

            PointF cPt = new PointF(40, 420);//中心点   
            PointF[] xPt = new PointF[3]{
             new   PointF(cPt.Y+15,cPt.Y),
             new   PointF(cPt.Y,cPt.Y-8),
             new   PointF(cPt.Y,cPt.Y+8)};//X轴三角形   
            PointF[] yPt = new PointF[3]{
             new   PointF(cPt.X,cPt.X-15),
             new   PointF(cPt.X+8,cPt.X),
             new   PointF(cPt.X-8,cPt.X)};//Y轴三角形   
            gph.DrawString("郑州轻工业学院产品月生产量图表", new Font("宋体", 14),
             Brushes.Black, new PointF(cPt.X + 80, cPt.X));//图表标题   
            //画X轴   
            gph.DrawLine(Pens.Black, cPt.X, cPt.Y, cPt.Y, cPt.Y);
            gph.DrawPolygon(Pens.Black, xPt);
            gph.FillPolygon(new SolidBrush(Color.Black), xPt);
            gph.DrawString("月份", new Font("宋体", 12),
             Brushes.Black, new PointF(cPt.Y + 12, cPt.Y - 10));
            //画Y轴   
            gph.DrawLine(Pens.Black, cPt.X, cPt.Y, cPt.X, cPt.X);
            gph.DrawPolygon(Pens.Black, yPt);
            gph.FillPolygon(new SolidBrush(Color.Black), yPt);
            gph.DrawString("单位(万)", new Font("宋体", 12), Brushes.Black, new PointF(6, 7));
            for (int i = 1; i <= 12; i++)
            {
                //画Y轴刻度   
                if (i < 11)
                {
                    gph.DrawString((i * 10).ToString(), new Font("宋体", 11), Brushes.Black,
                     new PointF(cPt.X - 30, cPt.Y - i * 30 - 6));
                    gph.DrawLine(Pens.Black, cPt.X - 3, cPt.Y - i * 30, cPt.X, cPt.Y - i * 30);
                }
                //画X轴项目   
                gph.DrawString(month[i - 1].Substring(0, 1), new Font("宋体", 11), Brushes.Black,
                 new PointF(cPt.X + i * 30 - 5, cPt.Y + 5));
                gph.DrawString(month[i - 1].Substring(1, 1), new Font("宋体", 11),
                 Brushes.Black, new PointF(cPt.X + i * 30 - 5, cPt.Y + 20));
                if (month[i - 1].Length > 2)
                    gph.DrawString(month[i - 1].Substring(2, 1), new Font("宋体", 11),
                     Brushes.Black, new PointF(cPt.X + i * 30 - 5, cPt.Y + 35));
                //画点   
                gph.DrawEllipse(Pens.Black, cPt.X + i * 30 - 1.5F, cPt.Y - d[i - 1] * 3 - 1.5F, 3, 3);
                gph.FillEllipse(new SolidBrush(Color.Black), cPt.X + i * 30 - 1.5F, cPt.Y - d[i - 1] * 3 + 1.5F, 3, 3);
                //画数值   
                gph.DrawString(d[i - 1].ToString(), new Font("宋体", 11), Brushes.Black,
                 new PointF(cPt.X + i * 30, cPt.Y - d[i - 1] * 3));
                //画折线   
                if (i > 1)
                    gph.DrawLine(Pens.Red, cPt.X + (i - 1) * 30, cPt.Y - d[i - 2] * 3, cPt.X + i * 30, cPt.Y - d[i - 1] * 3);
            }
            //显示在pictureBox1控件中
            this.pictureBox1.Image = bMap;
            //保存输出图片   
            //bMap.Save("C:\\Users\\FH\\Desktoptest.bmp");  
        }

        private void button2_Click(object sender, EventArgs e)
        {
            float[][] m_LineValue = { new float[] { 0.88f, 3.0f, 3.90f, 0f, 2f, 10f, 1f, 5.0f, 10.88f, 3.0f, 2.90f, 0.8f, 0f, 1.90f, 0.9f }};

            //float[][] m_LineValue = { new float[] { 0.88f, 3.0f, 3.90f, 0f, 2f, 10f, 1f, 5.0f, 10.88f, 3.0f, 2.90f, 0.8f, 0f, 1.90f, 0.9f }, new float[] { 1.8f, 1.0f, 9.90f, 10f, 2f, 1f, 4f, 3.0f, 2.88f, 7.0f, 6.90f, 9.8f, 7f, 9.90f, 4.9f }, new float[] { 8.88f, 10.0f, 1.90f, 0.6f, 1.7f, 7f, 6f, 10.0f, 8.88f, 1.0f, 1.90f, 1.8f, 3f, 0.90f, 1.9f }, };
            hocy_Curve1.YSliceValue = 1;
            hocy_Curve1.YSliceBegin = -5;
            hocy_Curve1.YSliceEnd = 12;
            hocy_Curve1.XPointScaleNum = 20;
            hocy_Curve1.LineValueAll = m_LineValue;
            hocy_Curve1.Invalidate();
        }

        
    }
}
