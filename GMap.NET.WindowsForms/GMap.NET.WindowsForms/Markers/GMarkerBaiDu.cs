using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.Text;

namespace System.Windows.Forms.GMap.NET.WindowsForms.Markers
{
    using System.Drawing;
    using System.Collections.Generic;

#if !PocketPC
    using System.Windows.Forms.Properties;
    using System;
    using System.Runtime.Serialization;
#else
   using GMap.NET.WindowsMobile.Properties;
#endif

    public enum GMarkerBaiDuType
    {
        none = 0,
        arrow,
        blue,
        blue_small,
        blue_dot,
        blue_pushpin,
        brown_small,
        gray_small,
        green,
        green_small,
        green_dot,
        green_pushpin,
        green_big_go,
        yellow,
        yellow_small,
        yellow_dot,
        yellow_big_pause,
        yellow_pushpin,
        lightblue,
        lightblue_dot,
        lightblue_pushpin,
        orange,
        orange_small,
        orange_dot,
        pink,
        pink_dot,
        pink_pushpin,
        purple,
        purple_small,
        purple_dot,
        purple_pushpin,
        red,
        red_small,
        red_dot,
        red_pushpin,
        red_big_stop,
        black_small,
        white_small,
    }

#if !PocketPC
    [Serializable]
    public class GMarkerBaiDu : GMapMarker, ISerializable, IDeserializationCallback
#else
   public class GMarkerBaiDu : GMapMarker
#endif
    {
        public float? Bearing;
        Bitmap Bitmap;
        Bitmap BitmapShadow;

        static Bitmap arrowshadow;
        static Bitmap msmarker_shadow;
        static Bitmap shadow_small;
        static Bitmap pushpin_shadow;

        public readonly GMarkerBaiDuType Type;

        public GMarkerBaiDu(PointLatLng p, GMarkerBaiDuType type): base(p)
        {
            this.Type = type;

            if (type != GMarkerBaiDuType.none)
            {
                LoadBitmap();
            }
        }

        void LoadBitmap()
        {
            Bitmap = GetIcon(Type.ToString());
            Size = new System.Drawing.Size(Bitmap.Width, Bitmap.Height);

            switch (Type)
            {
                case GMarkerBaiDuType.arrow:
                    {
                        Offset = new Point(-11, -Size.Height);

                        if (arrowshadow == null)
                        {
                            arrowshadow = Resources.arrowshadow;
                        }
                        BitmapShadow = arrowshadow;
                    }
                    break;

                case GMarkerBaiDuType.blue:
                case GMarkerBaiDuType.blue_dot:
                case GMarkerBaiDuType.green:
                case GMarkerBaiDuType.green_dot:
                case GMarkerBaiDuType.yellow:
                case GMarkerBaiDuType.yellow_dot:
                case GMarkerBaiDuType.lightblue:
                case GMarkerBaiDuType.lightblue_dot:
                case GMarkerBaiDuType.orange:
                case GMarkerBaiDuType.orange_dot:
                case GMarkerBaiDuType.pink:
                case GMarkerBaiDuType.pink_dot:
                case GMarkerBaiDuType.purple:
                case GMarkerBaiDuType.purple_dot:
                case GMarkerBaiDuType.red:
                case GMarkerBaiDuType.red_dot:
                    {
                        Offset = new Point(-Size.Width / 2 + 1, -Size.Height + 1);

                        if (msmarker_shadow == null)
                        {
                            msmarker_shadow = Resources.msmarker_shadow;
                        }
                        BitmapShadow = msmarker_shadow;
                    }
                    break;

                case GMarkerBaiDuType.black_small:
                case GMarkerBaiDuType.blue_small:
                case GMarkerBaiDuType.brown_small:
                case GMarkerBaiDuType.gray_small:
                case GMarkerBaiDuType.green_small:
                case GMarkerBaiDuType.yellow_small:
                case GMarkerBaiDuType.orange_small:
                case GMarkerBaiDuType.purple_small:
                case GMarkerBaiDuType.red_small:
                case GMarkerBaiDuType.white_small:
                    {
                        Offset = new Point(-Size.Width / 2, -Size.Height + 1);

                        if (shadow_small == null)
                        {
                            shadow_small = Resources.shadow_small;
                        }
                        BitmapShadow = shadow_small;
                    }
                    break;

                case GMarkerBaiDuType.green_big_go:
                case GMarkerBaiDuType.yellow_big_pause:
                case GMarkerBaiDuType.red_big_stop:
                    {
                        Offset = new Point(-Size.Width / 2, -Size.Height + 1);
                        if (msmarker_shadow == null)
                        {
                            msmarker_shadow = Resources.msmarker_shadow;
                        }
                        BitmapShadow = msmarker_shadow;
                    }
                    break;

                case GMarkerBaiDuType.blue_pushpin:
                case GMarkerBaiDuType.green_pushpin:
                case GMarkerBaiDuType.yellow_pushpin:
                case GMarkerBaiDuType.lightblue_pushpin:
                case GMarkerBaiDuType.pink_pushpin:
                case GMarkerBaiDuType.purple_pushpin:
                case GMarkerBaiDuType.red_pushpin:
                    {
                        Offset = new Point(-9, -Size.Height + 1);

                        if (pushpin_shadow == null)
                        {
                            pushpin_shadow = Resources.pushpin_shadow;
                        }
                        BitmapShadow = pushpin_shadow;
                    }
                    break;
            }
        }

        /// <summary>
        /// marker using manual bitmap, NonSerialized
        /// </summary>
        /// <param name="p"></param>
        /// <param name="Bitmap"></param>
        public GMarkerBaiDu(PointLatLng p, Bitmap Bitmap)
            : base(p)
        {
            this.Bitmap = Bitmap;
            Size = new System.Drawing.Size(Bitmap.Width, Bitmap.Height);
            Offset = new Point(-Size.Width / 2, -Size.Height);
        }

        static readonly Dictionary<string, Bitmap> iconCache = new Dictionary<string, Bitmap>();

        internal static Bitmap GetIcon(string name)
        {
            Bitmap ret;
            if (!iconCache.TryGetValue(name, out ret))
            {
                ret = Resources.ResourceManager.GetObject(name, Resources.Culture) as Bitmap;
                iconCache.Add(name, ret);
            }
            return ret;
        }

        static readonly Point[] Arrow = new Point[] { new Point(-7, 7), new Point(0, -22), new Point(7, 7), new Point(0, 2) };

        public override void OnRender(Graphics g)
        {
#if !PocketPC
            //if(!Bearing.HasValue)
            {
                if (BitmapShadow != null)
                {
                    g.DrawImage(BitmapShadow, LocalPosition.X, LocalPosition.Y, BitmapShadow.Width, BitmapShadow.Height);
                }
            }

            //if(Bearing.HasValue)
            //{
            //   g.RotateTransform(Bearing.Value - Overlay.Control.Bearing);
            //   g.FillPolygon(Brushes.Red, Arrow);
            //}

            //if(!Bearing.HasValue)
            {
                g.DrawImage(Bitmap, LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height);
            }
#else
         if(BitmapShadow != null)
         {
            DrawImageUnscaled(g, BitmapShadow, LocalPosition.X, LocalPosition.Y);
         }
         DrawImageUnscaled(g, Bitmap, LocalPosition.X, LocalPosition.Y);
#endif
        }

        public override void Dispose()
        {
            if (Bitmap != null)
            {
                if (!iconCache.ContainsValue(Bitmap))
                {
                    Bitmap.Dispose();
                    Bitmap = null;
                }
            }

            base.Dispose();
        }

#if !PocketPC

        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("type", this.Type);
            info.AddValue("Bearing", this.Bearing);

            base.GetObjectData(info, context);
        }

        protected GMarkerBaiDu(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Type = Extensions.GetStruct<GMarkerBaiDuType>(info, "type", GMarkerBaiDuType.none);
            this.Bearing = Extensions.GetStruct<float>(info, "Bearing", null);
        }

        #endregion

        #region IDeserializationCallback Members

        public void OnDeserialization(object sender)
        {
            if (Type != GMarkerBaiDuType.none)
            {
                LoadBitmap();
            }
        }

        #endregion

#endif
    }
}
