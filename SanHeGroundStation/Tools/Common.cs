﻿using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SanHeGroundStation.Tools
{
    /// <summary>
    /// used to override the drawing of the waypoint box bounding
    /// </summary>
    [Serializable]
    public class GMapMarkerRect : GMapMarker
    {
        public Pen Pen = new Pen(Brushes.White, 2);

        public Color Color { get { return Pen.Color; } set { if (!initcolor.HasValue) initcolor = value; Pen.Color = value; } }

        Color? initcolor = null;

        public GMapMarker InnerMarker;

        public int wprad = 0;

        public void ResetColor()
        {
            if (initcolor.HasValue)
                Color = initcolor.Value;
            else
                Color = Color.White;
        }

        public GMapMarkerRect(PointLatLng p)
            : base(p)
        {
            Pen.DashStyle = DashStyle.Dash;

            // do not forget set Size of the marker
            // if so, you shall have no event on it ;}
            Size = new System.Drawing.Size(50, 50);
            Offset = new System.Drawing.Point(-Size.Width / 2, -Size.Height / 2 - 20);
        }

        public override void OnRender(Graphics g)
        {
            base.OnRender(g);

            if (wprad == 0 || Overlay.Control == null)
                return;

            // if we have drawn it, then keep that color
            if (!initcolor.HasValue)
                Color = Color.White;

            // undo autochange in mouse over
            //if (Pen.Color == Color.Blue)
            //  Pen.Color = Color.White;

            double width = (Overlay.Control.MapProvider.Projection.GetDistance(Overlay.Control.FromLocalToLatLng(0, 0), Overlay.Control.FromLocalToLatLng(Overlay.Control.Width, 0)) * 1000.0);
            double height = (Overlay.Control.MapProvider.Projection.GetDistance(Overlay.Control.FromLocalToLatLng(0, 0), Overlay.Control.FromLocalToLatLng(Overlay.Control.Height, 0)) * 1000.0);
            double m2pixelwidth = Overlay.Control.Width / width;
            double m2pixelheight = Overlay.Control.Height / height;

            GPoint loc = new GPoint((int)(LocalPosition.X - (m2pixelwidth * wprad * 2)), LocalPosition.Y);// MainMap.FromLatLngToLocal(wpradposition);

            if (m2pixelheight > 0.5)
                g.DrawArc(Pen, new System.Drawing.Rectangle(LocalPosition.X - Offset.X - (int)(Math.Abs(loc.X - LocalPosition.X) / 2), LocalPosition.Y - Offset.Y - (int)Math.Abs(loc.X - LocalPosition.X) / 2, (int)Math.Abs(loc.X - LocalPosition.X), (int)Math.Abs(loc.X - LocalPosition.X)), 0, 360);

        }
    }

    [Serializable]
    public class GMapMarkerWP : GMarkerGoogle
    {
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        public string wpno = "";
        public bool selected = false;

        public GMapMarkerWP(PointLatLng p, string wpno): base(p, GMarkerGoogleType.green)
        {
            this.wpno = wpno;
        }

        public override void OnRender(Graphics g)
        {
            if (selected)
            {
                g.FillEllipse(Brushes.Red, new Rectangle(this.LocalPosition, this.Size));
                g.DrawArc(Pens.Red, new Rectangle(this.LocalPosition, this.Size), 0, 360);
            }
            base.OnRender(g);
            var midw = LocalPosition.X + 10;
            var midh = LocalPosition.Y + 3;
            var txtsize = TextRenderer.MeasureText(wpno, SystemFonts.DefaultFont);
            if (txtsize.Width > 15)
                midw -= 4;
            g.DrawString(wpno, SystemFonts.DefaultFont, Brushes.Black, new PointF(midw, midh));
        }
    }
}

 
   
