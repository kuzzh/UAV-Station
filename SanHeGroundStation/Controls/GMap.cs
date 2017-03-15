using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET.WindowsForms;

namespace SanHeGroundStation.Controls
{
    public partial class GMap:GMapControl
    {
        public long ElapsedMilliseconds;
        public bool inOnPaint = false;
        string otherthread = "";
        int lastx = 0;
        int lasty = 0;
        public GMap(): base()
        {
            this.Text = "Map";
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            var start = DateTime.Now;

            if (inOnPaint)
            {
                Console.WriteLine("Was in onpaint Gmap th:" + System.Threading.Thread.CurrentThread.Name + " in " + otherthread);
                return;
            }

            otherthread = System.Threading.Thread.CurrentThread.Name;

            inOnPaint = true;

            try
            {
                base.OnPaint(e);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }

            inOnPaint = false;

            var end = DateTime.Now;

            System.Diagnostics.Debug.WriteLine("map draw time " + (end-start).TotalMilliseconds);
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                // try prevent alot of cpu usage
                if (e.X >= lastx - 2 && e.X <= lastx + 2 && e.Y >= lasty - 2 && e.Y <= lasty + 2)
                    return;

                lastx = e.X;
                lasty = e.Y;

                base.OnMouseMove(e);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }
    }
}
