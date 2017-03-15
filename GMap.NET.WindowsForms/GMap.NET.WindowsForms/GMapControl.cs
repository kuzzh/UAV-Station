using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.Internals;
using GMap.NET.ObjectModel;
using System.Diagnostics;
using System.Drawing.Text;
using GMap.NET.MapProviders;


namespace GMap.NET.WindowsForms
{
#if !PocketPC
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Collections.Generic;
    using GMap.NET.Projections;
#else
   using OpenNETCF.ComponentModel;
#endif

    /// <summary>
    /// GMap.NET控制Windows窗体
    /// </summary>   
    public partial class GMapControl : UserControl, Interface
    {
#if !PocketPC
        /// <summary>
        /// 在标志点击后会触发
        /// </summary>
        public event MarkerClick OnMarkerClick;

        /// <summary>
        /// 多边形点击后触发
        /// </summary>
        public event PolygonClick OnPolygonClick;

        /// <summary>
        /// 路线点击后触发
        /// </summary>
        public event RouteClick OnRouteClick;

        /// <summary>
        /// 发生在鼠标进入航线区域
        /// </summary>
        public event RouteEnter OnRouteEnter;

        /// <summary>
        /// 发生在鼠标离开路线区域
        /// </summary>
        public event RouteLeave OnRouteLeave;

        /// <summary>
        /// 当鼠标选择更改时发生
        /// </summary>        
        public event SelectionChange OnSelectionChange;
#endif

        /// <summary>
        /// 发生在鼠标进入标志区
        /// </summary>
        public event MarkerEnter OnMarkerEnter;

        /// <summary>
        /// 发生在鼠标离开标志区
        /// </summary>
        public event MarkerLeave OnMarkerLeave;

        /// <summary>
        /// 发生在鼠标进入多边形区域
        /// </summary>
        public event PolygonEnter OnPolygonEnter;

        /// <summary>
        /// 发生在鼠标离开多边形区域
        /// </summary>
        public event PolygonLeave OnPolygonLeave;

        /// <summary>
        /// 叠加的名单，应该是线程安全的
        /// </summary>
        public readonly ObservableCollectionThreadSafe<GMapOverlay> Overlays = new ObservableCollectionThreadSafe<GMapOverlay>();

        /// <summary>
        /// 最大变焦
        /// </summary>         
        [Category("GMap.NET")]
        [Description("maximum zoom level of map")]
        public int MaxZoom
        {
            get
            {
                return Core.maxZoom;
            }
            set
            {
                Core.maxZoom = value;
            }
        }

        /// <summary>
        /// 最小变焦
        /// </summary>      
        [Category("GMap.NET")]
        [Description("minimum zoom level of map")]
        public int MinZoom
        {
            get
            {
                return Core.minZoom;
            }
            set
            {
                Core.minZoom = value;
            }
        }

        /// <summary>
        /// 地图缩放类型鼠标滚轮
        /// </summary>
        [Category("GMap.NET")]
        [Description("map zooming type for mouse wheel")]
        public MouseWheelZoomType MouseWheelZoomType
        {
            get
            {
                return Core.MouseWheelZoomType;
            }
            set
            {
                Core.MouseWheelZoomType = value;
            }
        }

        /// <summary>
        /// text on empty tiles
        /// </summary>
        public string EmptyTileText = "We are sorry, but we don't\nhave imagery at this zoom\nlevel for this region.";

        /// <summary>
        /// pen for empty tile borders
        /// </summary>
#if !PocketPC
        public Pen EmptyTileBorders = new Pen(Brushes.White, 1);
#else
      public Pen EmptyTileBorders = new Pen(Color.White, 1);
#endif

        public bool ShowCenter = true;

        /// <summary>
        /// pen for scale info
        /// </summary>
#if !PocketPC
        public Pen ScalePen = new Pen(Brushes.Blue, 1);
        public Pen CenterPen = new Pen(Brushes.Red, 1);
#else
      public Pen ScalePen = new Pen(Color.Blue, 1);
      public Pen CenterPen = new Pen(Color.Red, 1);
#endif

#if !PocketPC
        /// <summary>
        /// 区域选择笔
        /// </summary>
        public Pen SelectionPen = new Pen(Brushes.Blue, 2);

        Brush SelectedAreaFill = new SolidBrush(Color.FromArgb(33, Color.RoyalBlue));
        Color selectedAreaFillColor = Color.FromArgb(33, Color.RoyalBlue);

        /// <summary>
        /// 选定区域的背景
        /// </summary>
        [Category("GMap.NET")]
        [Description("background color od the selected area")]
        public Color SelectedAreaFillColor
        {
            get
            {
                return selectedAreaFillColor;
            }
            set
            {
                if (selectedAreaFillColor != value)
                {
                    selectedAreaFillColor = value;

                    if (SelectedAreaFill != null)
                    {
                        SelectedAreaFill.Dispose();
                        SelectedAreaFill = null;
                    }
                    SelectedAreaFill = new SolidBrush(selectedAreaFillColor);
                }
            }
        }

        HelperLineOptions helperLineOption = HelperLineOptions.DontShow;

        /// <summary>
        /// 在鼠标指针位置画线 
        /// </summary>
        [Browsable(false)]
        public HelperLineOptions HelperLineOption
        {
            get
            {
                return helperLineOption;
            }
            set
            {
                helperLineOption = value;
                renderHelperLine = (helperLineOption == HelperLineOptions.ShowAlways);
                if (Core.IsStarted)
                {
                    Invalidate();
                }
            }
        }

        public Pen HelperLinePen = new Pen(Color.Blue, 1);
        /// <summary>
        /// 渲染辅助线
        /// </summary>
        bool renderHelperLine = false;
        #region 监测按键

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (HelperLineOption == HelperLineOptions.ShowOnModifierKey)
            {
                renderHelperLine = (e.Modifiers == Keys.Shift || e.Modifiers == Keys.Alt);
                if (renderHelperLine)
                {
                    Invalidate();
                }
            }
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (HelperLineOption == HelperLineOptions.ShowOnModifierKey)
            {
                renderHelperLine = (e.Modifiers == Keys.Shift || e.Modifiers == Keys.Alt);
                if (!renderHelperLine)
                {
                    Invalidate();
                }
            }
            base.OnKeyUp(e);
        }
        #endregion
#endif

        Brush EmptytileBrush = new SolidBrush(Color.Navy);
        Color emptyTileColor = Color.Navy;

        /// <summary>
        /// 空瓦片的背景颜色
        /// </summary>
        [Category("GMap.NET")]
        [Description("background color of the empty tile")]
        public Color EmptyTileColor
        {
            get
            {
                return emptyTileColor;
            }
            set
            {
                if (emptyTileColor != value)
                {
                    emptyTileColor = value;

                    if (EmptytileBrush != null)
                    {
                        EmptytileBrush.Dispose();
                        EmptytileBrush = null;
                    }
                    EmptytileBrush = new SolidBrush(emptyTileColor);
                }
            }
        }

#if PocketPC
      readonly Brush TileGridLinesTextBrush = new SolidBrush(Color.Red);
      readonly Brush TileGridMissingTextBrush = new SolidBrush(Color.White);
      readonly Brush CopyrightBrush = new SolidBrush(Color.Navy);
#endif

        /// <summary>
        /// 是否显示地图比例尺信息
        /// </summary>
        public bool MapScaleInfoEnabled = false;

        /// <summary>
        /// 是否使用叫低级的图片填充没有的地图
        /// enables filling empty tiles using lower level images
        /// </summary>
        public bool FillEmptyTiles = true;

        /// <summary>
        /// 是否禁用Alt键进行选择 只能用鼠标移动选择
        /// if true, selects area just by holding mouse and moving
        /// </summary>
        public bool DisableAltForSelection = false;

        /// <summary>
        /// 重试次数来获得地图片
        /// retry count to get tile 
        /// </summary>
        [Browsable(false)]
        public int RetryLoadTile
        {
            get
            {
                return Core.RetryLoadTile;
            }
            set
            {
                Core.RetryLoadTile = value;
            }
        }

        /// <summary>
        /// 有多少地图瓦片在内存中保存着
        /// how many levels of tiles are staying decompresed in memory
        /// </summary>
        [Browsable(false)]
        public int LevelsKeepInMemmory
        {
            get
            {
                return Core.LevelsKeepInMemmory;
            }

            set
            {
                Core.LevelsKeepInMemmory = value;
            }
        }

        /// <summary>
        ///地图拖动按钮
        /// </summary>
        [Category("GMap.NET")]
        public MouseButtons DragButton = MouseButtons.Left;

        private bool showTileGridLines = false;

        /// <summary>
        /// 显示网格线
        /// </summary>
        [Category("GMap.NET")]
        [Description("shows tile gridlines")]
        public bool ShowTileGridLines
        {
            get
            {
                return showTileGridLines;
            }
            set
            {
                showTileGridLines = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 在地图中选中的地区
        /// </summary>
        private RectLatLng selectedArea;

        [Browsable(false)]
        public RectLatLng SelectedArea
        {
            get
            {
                return selectedArea;
            }
            set
            {
                selectedArea = value;

                if (Core.IsStarted)
                {
                    Invalidate();
                }
            }
        }

        /// <summary>
        ///地图的边界
        /// </summary>
        public RectLatLng? BoundsOfMap = null;

        /// <summary>
        /// 是否支持集成DoubleBuffer在Windows Mobile上运行
        /// enables integrated DoubleBuffer for running on windows mobile
        /// </summary>
#if !PocketPC
        public bool ForceDoubleBuffer = false;
        readonly bool MobileMode = false;
#else
      readonly bool ForceDoubleBuffer = true;
#endif

        /// <summary>
        /// 立即停止标志/路由/多边形失效;调用刷新执行单一的刷新和重置失效状态
        /// stops immediate marker/route/polygon invalidations;
        /// call Refresh to perform single refresh and reset invalidation state
        /// </summary>
        public bool HoldInvalidation = false;

        #region 刷新
        /// <summary>
        /// call this to stop HoldInvalidation and perform single forced instant refresh 
        /// </summary>
        public override void Refresh()
        {
            HoldInvalidation = false;

            lock (Core.invalidationLock)
            {
                Core.lastInvalidation = DateTime.Now;
            }

            base.Refresh();
        }
        #endregion

#if !DESIGN
        /// <summary>
        /// enque built-in thread safe invalidation
        /// </summary>
        public new void Invalidate()
        {
            if (Core.Refresh != null)
            {
                Core.Refresh.Set();
            }
        }
#endif

#if !PocketPC
        private bool _GrayScale = false;

        [Category("GMap.NET")]
        public bool GrayScaleMode
        {
            get
            {
                return _GrayScale;
            }
            set
            {
                _GrayScale = value;
                ColorMatrix = (value == true ? ColorMatrixs.GrayScale : null);
            }
        }

        private bool _Negative = false;

        [Category("GMap.NET")]
        public bool NegativeMode
        {
            get
            {
                return _Negative;
            }
            set
            {
                _Negative = value;
                ColorMatrix = (value == true ? ColorMatrixs.Negative : null);
            }
        }

        ColorMatrix colorMatrix;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public ColorMatrix ColorMatrix
        {
            get
            {
                return colorMatrix;
            }
            set
            {
                colorMatrix = value;
                if (GMapProvider.TileImageProxy != null && GMapProvider.TileImageProxy is GMapImageProxy)
                {
                    (GMapProvider.TileImageProxy as GMapImageProxy).ColorMatrix = value;
                    if (Core.IsStarted)
                    {
                        ReloadMap();
                    }
                }
            }
        }
#endif

        // internal stuff
        internal readonly Core Core = new Core();

        internal readonly Font CopyrightFont = new Font(FontFamily.GenericSansSerif, 7, FontStyle.Regular);
#if !PocketPC
        internal readonly Font MissingDataFont = new Font(FontFamily.GenericSansSerif, 11, FontStyle.Bold);
#else
      internal readonly Font MissingDataFont = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular);
#endif
        Font ScaleFont = new Font(FontFamily.GenericSansSerif, 5, FontStyle.Italic);
        internal readonly StringFormat CenterFormat = new StringFormat();
        internal readonly StringFormat BottomFormat = new StringFormat();
#if !PocketPC
        readonly ImageAttributes TileFlipXYAttributes = new ImageAttributes();
#endif
        double zoomReal;
        Bitmap backBuffer;
        Graphics gxOff;

        #region 构造函数
#if !DESIGN
        /// <summary>
        /// 构造函数
        /// </summary>
        public GMapControl()
        {
#if !PocketPC
            if (!IsDesignerHosted)
#endif
            {
#if !PocketPC
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                this.SetStyle(ControlStyles.UserPaint, true);
                this.SetStyle(ControlStyles.Opaque, true);
                ResizeRedraw = true;

                TileFlipXYAttributes.SetWrapMode(WrapMode.TileFlipXY);

                // only one mode will be active, to get mixed mode create new ColorMatrix
                GrayScaleMode = GrayScaleMode;
                NegativeMode = NegativeMode;
#endif
                Core.SystemType = "WindowsForms";

                RenderMode = RenderMode.GDI_PLUS;

                CenterFormat.Alignment = StringAlignment.Center;
                CenterFormat.LineAlignment = StringAlignment.Center;

                BottomFormat.Alignment = StringAlignment.Center;

#if !PocketPC
                BottomFormat.LineAlignment = StringAlignment.Far;
#endif

                if (GMaps.Instance.IsRunningOnMono)
                {
                    // no imports to move pointer
                    MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
                }

                Overlays.CollectionChanged += new NotifyCollectionChangedEventHandler(Overlays_CollectionChanged);
            }
        }

#endif
        #endregion

        #region 注释掉的
        //        static GMapControl()
        //        {
        //#if !PocketPC
        //            if (!IsDesignerHosted)
        //#endif
        //            {
        //                GMapImageProxy.Enable();
        //#if !PocketPC
        //                GMaps.Instance.SQLitePing();
        //#endif
        //            }
        //        } 
        #endregion

        void Overlays_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GMapOverlay obj in e.NewItems)
                {
                    if (obj != null)
                    {
                        obj.Control = this;
                    }
                }

                if (Core.IsStarted && !HoldInvalidation)
                {
                    Invalidate();
                }
            }
        }

        void invalidatorEngage(object sender, ProgressChangedEventArgs e)
        {
            base.Invalidate();
        }

        #region 在地图上拖动/缩放更新对象
        /// <summary>
        /// 在地图上拖动/缩放更新对象
        /// </summary>
        internal void ForceUpdateOverlays()
        {
            try
            {
                HoldInvalidation = true;

                foreach (GMapOverlay o in Overlays)
                {
                    if (o.IsVisibile)
                    {
                        o.ForceUpdate();
                    }
                }
            }
            finally
            {
                Refresh();
            }
        }
        #endregion

        #region 绘制地图的GDI+
        /// <summary>
        /// 绘制地图的GDI+
        /// </summary>
        /// <param name="g"></param>
        void DrawMap(Graphics g)
        {
            if (Core.updatingBounds || MapProvider == EmptyProvider.Instance || MapProvider == null)
            {
                Debug.WriteLine("Core.updatingBounds");
                return;
            }

            Core.tileDrawingListLock.AcquireReaderLock();
            Core.Matrix.EnterReadLock();
            try
            {
                foreach (var tilePoint in Core.tileDrawingList)
                {
                    {
                        Core.tileRect.Location = tilePoint.PosPixel;
                        if (ForceDoubleBuffer)
                        {
#if !PocketPC
                            if (MobileMode)
                            {
                                Core.tileRect.Offset(Core.renderOffset);
                            }
#else
                     Core.tileRect.Offset(Core.renderOffset);
#endif
                        }
                        Core.tileRect.OffsetNegative(Core.compensationOffset);

                        //if(Core.currentRegion.IntersectsWith(Core.tileRect) || IsRotated)
                        {
                            bool found = false;

                            Tile t = Core.Matrix.GetTileWithNoLock(Core.Zoom, tilePoint.PosXY);
                            if (t.NotEmpty)
                            {
                                // render tile
                                {
                                    foreach (GMapImage img in t.Overlays)
                                    {
                                        if (img != null && img.Img != null)
                                        {
                                            if (!found)
                                                found = true;

                                            if (!img.IsParent)
                                            {
#if !PocketPC
                                                if (!MapRenderTransform.HasValue)
                                                {
                                                    g.DrawImage(img.Img, Core.tileRect.X, Core.tileRect.Y, Core.tileRect.Width, Core.tileRect.Height);
                                                }
                                                else
                                                {
                                                    g.DrawImage(img.Img, new Rectangle((int)Core.tileRect.X, (int)Core.tileRect.Y, (int)Core.tileRect.Width, (int)Core.tileRect.Height), 0, 0, Core.tileRect.Width, Core.tileRect.Height, GraphicsUnit.Pixel, TileFlipXYAttributes);
                                                }
#else
                                    g.DrawImage(img.Img, (int) Core.tileRect.X, (int) Core.tileRect.Y);
#endif
                                            }
#if !PocketPC
                                            else
                                            {
                                                // TODO: move calculations to loader thread
                                                System.Drawing.RectangleF srcRect = new System.Drawing.RectangleF((float)(img.Xoff * (img.Img.Width / img.Ix)), (float)(img.Yoff * (img.Img.Height / img.Ix)), (img.Img.Width / img.Ix), (img.Img.Height / img.Ix));
                                                System.Drawing.Rectangle dst = new System.Drawing.Rectangle((int)Core.tileRect.X, (int)Core.tileRect.Y, (int)Core.tileRect.Width, (int)Core.tileRect.Height);

                                                g.DrawImage(img.Img, dst, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, TileFlipXYAttributes);
                                            }
#endif
                                        }
                                    }
                                }
                            }
#if !PocketPC
                            else if (FillEmptyTiles && MapProvider.Projection is MercatorProjection)
                            {
                                #region -- fill empty lines --
                                int zoomOffset = 1;
                                Tile parentTile = Tile.Empty;
                                long Ix = 0;

                                while (!parentTile.NotEmpty && zoomOffset < Core.Zoom && zoomOffset <= LevelsKeepInMemmory)
                                {
                                    Ix = (long)Math.Pow(2, zoomOffset);
                                    parentTile = Core.Matrix.GetTileWithNoLock(Core.Zoom - zoomOffset++, new GPoint((int)(tilePoint.PosXY.X / Ix), (int)(tilePoint.PosXY.Y / Ix)));
                                }

                                if (parentTile.NotEmpty)
                                {
                                    long Xoff = Math.Abs(tilePoint.PosXY.X - (parentTile.Pos.X * Ix));
                                    long Yoff = Math.Abs(tilePoint.PosXY.Y - (parentTile.Pos.Y * Ix));

                                    // render tile 
                                    {
                                        foreach (GMapImage img in parentTile.Overlays)
                                        {
                                            if (img != null && img.Img != null && !img.IsParent)
                                            {
                                                if (!found)
                                                    found = true;

                                                System.Drawing.RectangleF srcRect = new System.Drawing.RectangleF((float)(Xoff * (img.Img.Width / Ix)), (float)(Yoff * (img.Img.Height / Ix)), (img.Img.Width / Ix), (img.Img.Height / Ix));
                                                System.Drawing.Rectangle dst = new System.Drawing.Rectangle((int)Core.tileRect.X, (int)Core.tileRect.Y, (int)Core.tileRect.Width, (int)Core.tileRect.Height);

                                                g.DrawImage(img.Img, dst, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, TileFlipXYAttributes);
                                                g.FillRectangle(SelectedAreaFill, dst);
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
#endif
                            // add text if tile is missing
                            if (!found)
                            {
                                lock (Core.FailedLoads)
                                {
                                    var lt = new LoadTask(tilePoint.PosXY, Core.Zoom);
                                    if (Core.FailedLoads.ContainsKey(lt))
                                    {
                                        var ex = Core.FailedLoads[lt];
#if !PocketPC
                                        g.FillRectangle(EmptytileBrush, new RectangleF(Core.tileRect.X, Core.tileRect.Y, Core.tileRect.Width, Core.tileRect.Height));

                                        g.DrawString("Exception: " + ex.Message, MissingDataFont, Brushes.Red, new RectangleF(Core.tileRect.X + 11, Core.tileRect.Y + 11, Core.tileRect.Width - 11, Core.tileRect.Height - 11));

                                        g.DrawString(EmptyTileText, MissingDataFont, Brushes.Blue, new RectangleF(Core.tileRect.X, Core.tileRect.Y, Core.tileRect.Width, Core.tileRect.Height), CenterFormat);

#else
                              g.FillRectangle(EmptytileBrush, new System.Drawing.Rectangle((int) Core.tileRect.X, (int) Core.tileRect.Y, (int) Core.tileRect.Width, (int) Core.tileRect.Height));

                              g.DrawString("Exception: " + ex.Message, MissingDataFont, TileGridMissingTextBrush, new RectangleF(Core.tileRect.X + 11, Core.tileRect.Y + 11, Core.tileRect.Width - 11, Core.tileRect.Height - 11));

                              g.DrawString(EmptyTileText, MissingDataFont, TileGridMissingTextBrush, new RectangleF(Core.tileRect.X, Core.tileRect.Y + Core.tileRect.Width / 2 + (ShowTileGridLines ? 11 : -22), Core.tileRect.Width, Core.tileRect.Height), BottomFormat);
#endif

                                        g.DrawRectangle(EmptyTileBorders, (int)Core.tileRect.X, (int)Core.tileRect.Y, (int)Core.tileRect.Width, (int)Core.tileRect.Height);
                                    }
                                }
                            }

                            if (ShowTileGridLines)
                            {
                                g.DrawRectangle(EmptyTileBorders, (int)Core.tileRect.X, (int)Core.tileRect.Y, (int)Core.tileRect.Width, (int)Core.tileRect.Height);
                                {
#if !PocketPC
                                    g.DrawString((tilePoint.PosXY == Core.centerTileXYLocation ? "CENTER: " : "TILE: ") + tilePoint, MissingDataFont, Brushes.Red, new RectangleF(Core.tileRect.X, Core.tileRect.Y, Core.tileRect.Width, Core.tileRect.Height), CenterFormat);
#else
                           g.DrawString((tilePoint.PosXY == Core.centerTileXYLocation ? "" : "TILE: ") + tilePoint, MissingDataFont, TileGridLinesTextBrush, new RectangleF(Core.tileRect.X, Core.tileRect.Y, Core.tileRect.Width, Core.tileRect.Height), CenterFormat);
#endif
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                Core.Matrix.LeaveReadLock();
                Core.tileDrawingListLock.ReleaseReaderLock();
            }
        }
        #endregion

        #region 更新标记局部位置
        /// <summary>
        /// 更新标记局部位置
        /// </summary>
        /// <param name="marker"></param>
        public void UpdateMarkerLocalPosition(GMapMarker marker)
        {
            GPoint p = FromLatLngToLocal(marker.Position);
            {
#if !PocketPC
                if (!MobileMode)
                {
                    p.OffsetNegative(Core.renderOffset);
                }
#endif

                var f = new System.Drawing.Point((int)(p.X + marker.Offset.X), (int)(p.Y + marker.Offset.Y));
                marker.LocalPosition = f;
            }
        }
        #endregion

        #region 更新路线局部位置
        /// <summary>
        /// 更新路线局部位置
        /// </summary>
        /// <param name="route"></param>
        public void UpdateRouteLocalPosition(GMapRoute route)
        {
            route.LocalPoints.Clear();

            for (int i = 0; i < route.Points.Count; i++)
            {
                PointLatLng pg = route.Points[i];
                GPoint p = FromLatLngToLocal(pg);

#if !PocketPC
                if (!MobileMode)
                {
                    p.OffsetNegative(Core.renderOffset);
                }
#endif

                //            if(IsRotated)
                //            {
                //#if !PocketPC
                //               System.Drawing.Point[] tt = new System.Drawing.Point[] { new System.Drawing.Point(p.X, p.Y) };
                //               rotationMatrix.TransformPoints(tt);
                //               var f = tt[0];

                //               p.X = f.X;
                //               p.Y = f.Y;
                //#endif
                //            }

                route.LocalPoints.Add(p);
            }
#if !PocketPC
            route.UpdateGraphicsPath();
#endif
        }
        #endregion

        #region 更新多边形局部位置
        /// <summary>
        /// 更新多边形局部位置
        /// </summary>
        /// <param name="polygon"></param>
        public void UpdatePolygonLocalPosition(GMapPolygon polygon)
        {
            polygon.LocalPoints.Clear();

            for (int i = 0; i < polygon.Points.Count; i++)
            {
                PointLatLng pg = polygon.Points[i];
                GPoint p = FromLatLngToLocal(pg);

#if !PocketPC
                if (!MobileMode)
                {
                    p.OffsetNegative(Core.renderOffset);
                }
#endif


                polygon.LocalPoints.Add(p);
            }
#if !PocketPC
            unchecked
            {
                try
                {
                    polygon.UpdateGraphicsPath();
                }
                catch
                {

                }
            }
#endif
        }
        #endregion

        #region  设置最大的变焦 适应矩形
        /// <summary>
        /// 设置最大的变焦 适应矩形
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public bool SetZoomToFitRect(RectLatLng rect)
        {
            if (lazyEvents)
            {
                lazySetZoomToFitRect = rect;
            }
            else
            {
                int maxZoom = Core.GetMaxZoomToFitRect(rect);
                if (maxZoom > 0)
                {
                    PointLatLng center = new PointLatLng(rect.Lat - (rect.HeightLat / 2), rect.Lng + (rect.WidthLng / 2));
                    Position = center;

                    if (maxZoom > MaxZoom)
                    {
                        maxZoom = MaxZoom;
                    }

                    if ((int)Zoom != maxZoom)
                    {
                        Zoom = maxZoom;
                    }

                    return true;
                }
            }

            return false;
        }
        #endregion

        RectLatLng? lazySetZoomToFitRect = null;
        bool lazyEvents = true;

        #region 设置为最大缩放以适应所有标记和中心他们的地图
        /// <summary>
        /// 设置为最大缩放以适应所有标记和中心他们的地图
        /// </summary>
        /// <param name="overlayId">叠加ID或空检查所有</param>
        /// <returns></returns>
        public bool ZoomAndCenterMarkers(string overlayId)
        {
            RectLatLng? rect = GetRectOfAllMarkers(overlayId);
            if (rect.HasValue)
            {
                return SetZoomToFitRect(rect.Value);
            }

            return false;
        }
        #endregion

        #region 缩放和中心的所有航线
        /// <summary>
        /// 缩放和中心的所有航线
        /// </summary>
        /// <param name="overlayId">叠加ID或空检查所有</param>
        /// <returns></returns>
        public bool ZoomAndCenterRoutes(string overlayId)
        {
            RectLatLng? rect = GetRectOfAllRoutes(overlayId);
            if (rect.HasValue)
            {
                return SetZoomToFitRect(rect.Value);
            }

            return false;
        }
        #endregion

        #region 变焦和中心路线
        /// <summary>
        /// 变焦和中心路线
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public bool ZoomAndCenterRoute(MapRoute route)
        {
            RectLatLng? rect = GetRectOfRoute(route);
            if (rect.HasValue)
            {
                return SetZoomToFitRect(rect.Value);
            }

            return false;
        }
        #endregion

        #region 得到矩形内的所有对象
        /// <summary>
        /// 得到矩形内的所有对象
        /// </summary>
        /// <param name="overlayId">叠加ID或空检查所有</param>
        /// <returns></returns>
        public RectLatLng? GetRectOfAllMarkers(string overlayId)
        {
            RectLatLng? ret = null;

            double left = double.MaxValue;
            double top = double.MinValue;
            double right = double.MinValue;
            double bottom = double.MaxValue;

            foreach (GMapOverlay o in Overlays)
            {
                if (overlayId == null || o.Id == overlayId)
                {
                    if (o.IsVisibile && o.Markers.Count > 0)
                    {
                        foreach (GMapMarker m in o.Markers)
                        {
                            if (m.IsVisible)
                            {
                                // left
                                if (m.Position.Lng < left)
                                {
                                    left = m.Position.Lng;
                                }

                                // top
                                if (m.Position.Lat > top)
                                {
                                    top = m.Position.Lat;
                                }

                                // right
                                if (m.Position.Lng > right)
                                {
                                    right = m.Position.Lng;
                                }

                                // bottom
                                if (m.Position.Lat < bottom)
                                {
                                    bottom = m.Position.Lat;
                                }
                            }
                        }
                    }
                }
            }

            if (left != double.MaxValue && right != double.MinValue && top != double.MinValue && bottom != double.MaxValue)
            {
                ret = RectLatLng.FromLTRB(left, top, right, bottom);
            }

            return ret;
        }
        #endregion

        #region 得到矩形内的所有对象
        /// <summary>
        /// 得到矩形内的所有对象
        /// </summary>
        /// <param name="overlayId">叠加ID或空检查所有</param>
        /// <returns></returns>
        public RectLatLng? GetRectOfAllRoutes(string overlayId)
        {
            RectLatLng? ret = null;

            double left = double.MaxValue;
            double top = double.MinValue;
            double right = double.MinValue;
            double bottom = double.MaxValue;

            foreach (GMapOverlay o in Overlays)
            {
                if (overlayId == null || o.Id == overlayId)
                {
                    if (o.IsVisibile && o.Routes.Count > 0)
                    {
                        foreach (GMapRoute route in o.Routes)
                        {
                            if (route.IsVisible && route.From.HasValue && route.To.HasValue)
                            {
                                foreach (PointLatLng p in route.Points)
                                {
                                    // left
                                    if (p.Lng < left)
                                    {
                                        left = p.Lng;
                                    }

                                    // top
                                    if (p.Lat > top)
                                    {
                                        top = p.Lat;
                                    }

                                    // right
                                    if (p.Lng > right)
                                    {
                                        right = p.Lng;
                                    }

                                    // bottom
                                    if (p.Lat < bottom)
                                    {
                                        bottom = p.Lat;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (left != double.MaxValue && right != double.MinValue && top != double.MinValue && bottom != double.MaxValue)
            {
                ret = RectLatLng.FromLTRB(left, top, right, bottom);
            }

            return ret;
        }
        #endregion

        #region 获取途径的矩形
        /// <summary>
        /// 获取途径的矩形
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public RectLatLng? GetRectOfRoute(MapRoute route)
        {
            RectLatLng? ret = null;

            double left = double.MaxValue;
            double top = double.MinValue;
            double right = double.MinValue;
            double bottom = double.MaxValue;

            if (route.From.HasValue && route.To.HasValue)
            {
                foreach (PointLatLng p in route.Points)
                {
                    // left
                    if (p.Lng < left)
                    {
                        left = p.Lng;
                    }

                    // top
                    if (p.Lat > top)
                    {
                        top = p.Lat;
                    }

                    // right
                    if (p.Lng > right)
                    {
                        right = p.Lng;
                    }

                    // bottom
                    if (p.Lat < bottom)
                    {
                        bottom = p.Lat;
                    }
                }
                ret = RectLatLng.FromLTRB(left, top, right, bottom);
            }
            return ret;
        }
        #endregion

        #region 获取当前视图的图像

#if !PocketPC
        /// <summary>
        /// 获取当前视图的图像
        /// </summary>
        /// <returns></returns>
        public Image ToImage()
        {
            Image ret = null;
            try
            {
                using (Bitmap bitmap = new Bitmap(Width, Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        using (Graphics gg = this.CreateGraphics())
                        {
#if !PocketPC
                            g.CopyFromScreen(PointToScreen(new System.Drawing.Point()).X, PointToScreen(new System.Drawing.Point()).Y, 0, 0, new System.Drawing.Size(Width, Height));
#else
                     throw new NotImplementedException("Not implemeted for PocketPC");
#endif
                        }
                    }

                    // 将图像转换为PNG
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bitmap.Save(ms, ImageFormat.Png);
#if !PocketPC
                        ret = Image.FromStream(ms);
#else
                  throw new NotImplementedException("Not implemeted for PocketPC");
#endif
                    }
                }
            }
            catch
            {
                ret = null;
            }
            return ret;
        }
#endif
        #endregion

        #region 像素偏移位置
        /// <summary>
        /// 像素偏移位置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Offset(int x, int y)
        {
            if (IsHandleCreated)
            {
                // need to fix in rotated mode usinf rotationMatrix
                // ...
                Core.DragOffset(new GPoint(x, y));

                ForceUpdateOverlays();
            }
        }

        #endregion

        #region UserControl Events

#if !PocketPC
        public readonly static bool IsDesignerHosted = LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsDesignerHosted)
            {
                //MethodInvoker m = delegate
                //{
                //   Thread.Sleep(444);

                //OnSizeChanged(null);

                if (lazyEvents)
                {
                    lazyEvents = false;

                    if (lazySetZoomToFitRect.HasValue)
                    {
                        SetZoomToFitRect(lazySetZoomToFitRect.Value);
                        lazySetZoomToFitRect = null;
                    }
                }
                Core.OnMapOpen().ProgressChanged += new ProgressChangedEventHandler(invalidatorEngage);
                ForceUpdateOverlays();
                //};
                //this.BeginInvoke(m);
            }
        }
#else
      //delegate void MethodInvoker();
      bool IsHandleCreated = false;

      protected override void OnPaintBackground(PaintEventArgs e)
      {
         if(!IsHandleCreated)
         {
            IsHandleCreated = true;

            if(lazyEvents)
            {
               lazyEvents = false;

               if(lazySetZoomToFitRect.HasValue)
               {
                  SetZoomToFitRect(lazySetZoomToFitRect.Value);
                  lazySetZoomToFitRect = null;
               }
            }

            Core.OnMapOpen().ProgressChanged += new ProgressChangedEventHandler(invalidatorEngage);
            ForceUpdateOverlays();
         }
      }
#endif

#if !PocketPC
        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (!IsDesignerHosted)
            {
                var f = ParentForm;
                if (f != null)
                {
                    while (f.ParentForm != null)
                    {
                        f = f.ParentForm;
                    }

                    if (f != null)
                    {
                        f.FormClosing += new FormClosingEventHandler(ParentForm_FormClosing);
                    }
                }
            }
        }

        void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown || e.CloseReason == CloseReason.TaskManagerClosing)
            {
                Manager.CancelTileCaching();
            }
        }
#endif

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Core.OnMapClose();

                Overlays.CollectionChanged -= new NotifyCollectionChangedEventHandler(Overlays_CollectionChanged);

                foreach (var o in Overlays)
                {
                    o.Dispose();
                }
                Overlays.Clear();

                ScaleFont.Dispose();
                ScalePen.Dispose();
                CenterFormat.Dispose();
                CenterPen.Dispose();
                BottomFormat.Dispose();
                CopyrightFont.Dispose();
                EmptyTileBorders.Dispose();
                EmptytileBrush.Dispose();

#if !PocketPC
                SelectedAreaFill.Dispose();
                SelectionPen.Dispose();
#endif
                if (backBuffer != null)
                {
                    backBuffer.Dispose();
                    backBuffer = null;
                }

                if (gxOff != null)
                {
                    gxOff.Dispose();
                    gxOff = null;
                }
            }
            base.Dispose(disposing);
        }

        PointLatLng selectionStart;
        PointLatLng selectionEnd;

#if !PocketPC
        float? MapRenderTransform = null;
#endif

        public Color EmptyMapBackground = Color.WhiteSmoke;

#if !DESIGN
        protected override void OnPaint(PaintEventArgs e)
        {
            if (ForceDoubleBuffer)
            {
                #region -- manual buffer --
                if (gxOff != null && backBuffer != null)
                {
                    // render white background
                    gxOff.Clear(EmptyMapBackground);

#if !PocketPC
                    if (MapRenderTransform.HasValue)
                    {
                        if (!MobileMode)
                        {
                            var center = new GPoint(Width / 2, Height / 2);
                            var delta = center;
                            delta.OffsetNegative(Core.renderOffset);
                            var pos = center;
                            pos.OffsetNegative(delta);

                            gxOff.ScaleTransform(MapRenderTransform.Value, MapRenderTransform.Value, MatrixOrder.Append);
                            gxOff.TranslateTransform(pos.X, pos.Y, MatrixOrder.Append);

                            DrawMap(gxOff);
                            gxOff.ResetTransform();

                            gxOff.TranslateTransform(pos.X, pos.Y, MatrixOrder.Append);
                        }
                        else
                        {
                            DrawMap(gxOff);
                            gxOff.ResetTransform();
                        }
                        OnPaintOverlays(gxOff);
                    }
                    else
#endif
                    {
#if !PocketPC
                        if (!MobileMode)
                        {
                            gxOff.TranslateTransform(Core.renderOffset.X, Core.renderOffset.Y);
                        }
#endif
                        DrawMap(gxOff);
                        OnPaintOverlays(gxOff);
                    }

                    e.Graphics.DrawImage(backBuffer, 0, 0);
                }
                #endregion
            }
            else
            {
                e.Graphics.Clear(EmptyMapBackground);

#if !PocketPC
                if (MapRenderTransform.HasValue)
                {
                    if (!MobileMode)
                    {
                        var center = new GPoint(Width / 2, Height / 2);
                        var delta = center;
                        delta.OffsetNegative(Core.renderOffset);
                        var pos = center;
                        pos.OffsetNegative(delta);

                        e.Graphics.ScaleTransform(MapRenderTransform.Value, MapRenderTransform.Value, MatrixOrder.Append);
                        e.Graphics.TranslateTransform(pos.X, pos.Y, MatrixOrder.Append);

                        DrawMap(e.Graphics);
                        e.Graphics.ResetTransform();

                        e.Graphics.TranslateTransform(pos.X, pos.Y, MatrixOrder.Append);
                    }
                    else
                    {
                        DrawMap(e.Graphics);
                        e.Graphics.ResetTransform();
                    }
                    OnPaintOverlays(e.Graphics);
                }
                else
#endif
                {
#if !PocketPC
                    if (IsRotated)
                    {
                        #region -- rotation --

                        e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                        e.Graphics.TranslateTransform((float)(Core.Width / 2.0), (float)(Core.Height / 2.0));
                        e.Graphics.RotateTransform(-Bearing);
                        e.Graphics.TranslateTransform((float)(-Core.Width / 2.0), (float)(-Core.Height / 2.0));

                        e.Graphics.TranslateTransform(Core.renderOffset.X, Core.renderOffset.Y);

                        DrawMap(e.Graphics);
                        OnPaintOverlays(e.Graphics);

                        #endregion
                    }
                    else
#endif
                    {
#if !PocketPC
                        if (!MobileMode)
                        {
                            e.Graphics.TranslateTransform(Core.renderOffset.X, Core.renderOffset.Y);
                        }
#endif
                        DrawMap(e.Graphics);
                        OnPaintOverlays(e.Graphics);
                    }
                }
            }

            base.OnPaint(e);
        }
#endif

#if !PocketPC
        readonly Matrix rotationMatrix = new Matrix();
        readonly Matrix rotationMatrixInvert = new Matrix();

        /// <summary>
        /// updates rotation matrix
        /// </summary>
        void UpdateRotationMatrix()
        {
            PointF center = new PointF(Core.Width / 2, Core.Height / 2);

            rotationMatrix.Reset();
            rotationMatrix.RotateAt(-Bearing, center);

            rotationMatrixInvert.Reset();
            rotationMatrixInvert.RotateAt(-Bearing, center);
            rotationMatrixInvert.Invert();
        }

        /// <summary>
        /// returs true if map bearing is not zero
        /// </summary>    
        [Browsable(false)]
        public bool IsRotated
        {
            get
            {
                return Core.IsRotated;
            }
        }

        /// <summary>
        /// bearing for rotation of the map
        /// </summary>
        [Category("GMap.NET")]
        public float Bearing
        {
            get
            {
                return Core.bearing;
            }
            set
            {
                //if(Core.bearing != value)
                //{
                //   bool resize = Core.bearing == 0;
                //   Core.bearing = value;

                //   //if(VirtualSizeEnabled)
                //   //{
                //   //   c.X += (Width - Core.vWidth) / 2;
                //   //   c.Y += (Height - Core.vHeight) / 2;
                //   //}

                //   UpdateRotationMatrix();

                //   if(value != 0 && value % 360 != 0)
                //   {
                //      Core.IsRotated = true;

                //      if(Core.tileRectBearing.Size == Core.tileRect.Size)
                //      {
                //         Core.tileRectBearing = Core.tileRect;
                //         Core.tileRectBearing.Inflate(1, 1);
                //      }
                //   }
                //   else
                //   {
                //      Core.IsRotated = false;
                //      Core.tileRectBearing = Core.tileRect;
                //   }

                //   if(resize)
                //   {
                //      Core.OnMapSizeChanged(Width, Height);
                //   }

                //   if(!HoldInvalidation && Core.IsStarted)
                //   {
                //      ForceUpdateOverlays();
                //   }
                //}
            }
        }
#endif

        /// <summary>
        /// override, to render something more
        /// </summary>
        /// <param name="g"></param>
        protected virtual void OnPaintOverlays(Graphics g)
        {
#if !PocketPC
            g.SmoothingMode = SmoothingMode.HighQuality;
#endif
            foreach (GMapOverlay o in Overlays)
            {
                if (o.IsVisibile)
                {
                    o.OnRender(g);
                }
            }

            // center in virtual spcace...
#if DEBUG
            g.DrawLine(ScalePen, -20, 0, 20, 0);
            g.DrawLine(ScalePen, 0, -20, 0, 20);

#if PocketPC
         g.DrawString("debug build", CopyrightFont, CopyrightBrush, 2, CopyrightFont.Size);
#else
            g.DrawString("debug build", CopyrightFont, Brushes.Blue, 2, CopyrightFont.Height);
#endif

#endif

#if !PocketPC

            if (!MobileMode)
            {
                g.ResetTransform();
            }

            if (!SelectedArea.IsEmpty)
            {
                GPoint p1 = FromLatLngToLocal(SelectedArea.LocationTopLeft);
                GPoint p2 = FromLatLngToLocal(SelectedArea.LocationRightBottom);

                long x1 = p1.X;
                long y1 = p1.Y;
                long x2 = p2.X;
                long y2 = p2.Y;

                g.DrawRectangle(SelectionPen, x1, y1, x2 - x1, y2 - y1);
                g.FillRectangle(SelectedAreaFill, x1, y1, x2 - x1, y2 - y1);
            }

            if (renderHelperLine)
            {
                var p = PointToClient(Form.MousePosition);

                g.DrawLine(HelperLinePen, p.X, 0, p.X, Height);
                g.DrawLine(HelperLinePen, 0, p.Y, Width, p.Y);
            }
#endif
            if (ShowCenter)
            {
                g.DrawLine(CenterPen, Width / 2 - 5, Height / 2, Width / 2 + 5, Height / 2);
                g.DrawLine(CenterPen, Width / 2, Height / 2 - 5, Width / 2, Height / 2 + 5);
            }

            #region -- copyright --

            if (!string.IsNullOrEmpty(Core.provider.Copyright))
            {
#if !PocketPC
                g.DrawString(Core.provider.Copyright, CopyrightFont, Brushes.Navy, 3, Height - CopyrightFont.Height - 5);
#else
            g.DrawString(Core.provider.Copyright, CopyrightFont, CopyrightBrush, 3, Height - CopyrightFont.Size - 15);
#endif
            }

            #endregion

            #region -- draw scale --
#if !PocketPC
            if (MapScaleInfoEnabled)
            {
                if (Width > Core.pxRes5000km)
                {
                    g.DrawRectangle(ScalePen, 10, 10, Core.pxRes5000km, 10);
                    g.DrawString("5000Km", ScaleFont, Brushes.Blue, Core.pxRes5000km + 10, 11);
                }
                if (Width > Core.pxRes1000km)
                {
                    g.DrawRectangle(ScalePen, 10, 10, Core.pxRes1000km, 10);
                    g.DrawString("1000Km", ScaleFont, Brushes.Blue, Core.pxRes1000km + 10, 11);
                }
                if (Width > Core.pxRes100km && Zoom > 2)
                {
                    g.DrawRectangle(ScalePen, 10, 10, Core.pxRes100km, 10);
                    g.DrawString("100Km", ScaleFont, Brushes.Blue, Core.pxRes100km + 10, 11);
                }
                if (Width > Core.pxRes10km && Zoom > 5)
                {
                    g.DrawRectangle(ScalePen, 10, 10, Core.pxRes10km, 10);
                    g.DrawString("10Km", ScaleFont, Brushes.Blue, Core.pxRes10km + 10, 11);
                }
                if (Width > Core.pxRes1000m && Zoom >= 10)
                {
                    g.DrawRectangle(ScalePen, 10, 10, Core.pxRes1000m, 10);
                    g.DrawString("1000m", ScaleFont, Brushes.Blue, Core.pxRes1000m + 10, 11);
                }
                if (Width > Core.pxRes100m && Zoom > 11)
                {
                    g.DrawRectangle(ScalePen, 10, 10, Core.pxRes100m, 10);
                    g.DrawString("100m", ScaleFont, Brushes.Blue, Core.pxRes100m + 9, 11);
                }
            }
#endif
            #endregion
        }

#if !PocketPC

        /// <summary>
        /// shrinks map area, useful just for testing
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool VirtualSizeEnabled
        {
            get
            {
                return Core.VirtualSizeEnabled;
            }
            set
            {
                Core.VirtualSizeEnabled = value;
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
#else
      protected override void OnResize(EventArgs e)
      {
         base.OnResize(e);
#endif
            if (Width == 0 || Height == 0)
            {
                Debug.WriteLine("minimized");
                return;
            }

            if (Width == Core.Width && Height == Core.Height)
            {
                Debug.WriteLine("maximized");
                return;
            }

#if !PocketPC
            if (!IsDesignerHosted)
#endif
            {
                if (ForceDoubleBuffer)
                {
                    if (backBuffer != null)
                    {
                        backBuffer.Dispose();
                        backBuffer = null;
                    }
                    if (gxOff != null)
                    {
                        gxOff.Dispose();
                        gxOff = null;
                    }

                    backBuffer = new Bitmap(Width, Height);
                    gxOff = Graphics.FromImage(backBuffer);
                }

#if !PocketPC
                if (VirtualSizeEnabled)
                {
                    Core.OnMapSizeChanged(Core.vWidth, Core.vHeight);
                }
                else
#endif
                {
                    Core.OnMapSizeChanged(Width, Height);
                }
                //Core.currentRegion = new GRect(-50, -50, Core.Width + 50, Core.Height + 50);

                if (Visible && IsHandleCreated && Core.IsStarted)
                {
#if !PocketPC
                    if (IsRotated)
                    {
                        UpdateRotationMatrix();
                    }
#endif
                    try
                    {
                        ForceUpdateOverlays();
                    }
                    catch (System.OverflowException ex) { System.Diagnostics.Debug.WriteLine(ex); }
                }
            }
        }

        bool isSelected = false;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!IsMouseOverMarker)
            {
#if !PocketPC
                if (e.Button == DragButton && CanDragMap)
#else
            if(CanDragMap)
#endif
                {
#if !PocketPC
                    Core.mouseDown = ApplyRotationInversion(e.X, e.Y);
#else
               Core.mouseDown = new GPoint(e.X, e.Y);
#endif
                    this.Invalidate();
                }
                else if (!isSelected)
                {
                    isSelected = true;
                    SelectedArea = RectLatLng.Empty;
                    selectionEnd = PointLatLng.Empty;
                    selectionStart = FromLocalToLatLng(e.X, e.Y);
                }
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (isSelected)
            {
                isSelected = false;
            }

            if (Core.IsDragging)
            {
                if (isDragging)
                {
                    isDragging = false;
                    Debug.WriteLine("IsDragging = " + isDragging);
#if !PocketPC
                    this.Cursor = cursorBefore;
                    cursorBefore = null;
#endif
                }
                Core.EndDrag();

                if (BoundsOfMap.HasValue && !BoundsOfMap.Value.Contains(Position))
                {
                    if (Core.LastLocationInBounds.HasValue)
                    {
                        Position = Core.LastLocationInBounds.Value;
                    }
                }
            }
            else
            {
#if !PocketPC
                if (e.Button == DragButton)
                {
                    Core.mouseDown = GPoint.Empty;
                }

                if (!selectionEnd.IsEmpty && !selectionStart.IsEmpty)
                {
                    bool zoomtofit = false;

                    if (!SelectedArea.IsEmpty && Form.ModifierKeys == Keys.Shift)
                    {
                        zoomtofit = SetZoomToFitRect(SelectedArea);
                    }

                    if (OnSelectionChange != null)
                    {
                        OnSelectionChange(SelectedArea, zoomtofit);
                    }
                }
                else
                {
                    Invalidate();
                }
#endif
            }
        }

#if !PocketPC
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (!Core.IsDragging)
            {
                for (int i = Overlays.Count - 1; i >= 0; i--)
                {
                    GMapOverlay o = Overlays[i];
                    if (o != null && o.IsVisibile)
                    {
                        foreach (GMapMarker m in o.Markers)
                        {
                            if (m.IsVisible && m.IsHitTestVisible)
                            {
                                #region -- check --

                                if ((MobileMode && m.LocalArea.Contains(e.X, e.Y)) || (!MobileMode && m.LocalAreaInControlSpace.Contains(e.X, e.Y)))
                                {
                                    if (OnMarkerClick != null)
                                    {
                                        OnMarkerClick(m, e);
                                    }
                                    break;
                                }

                                #endregion
                            }
                        }

                        foreach (GMapRoute m in o.Routes)
                        {
                            if (m.IsVisible && m.IsHitTestVisible)
                            {
                                #region -- check --

                                GPoint rp = new GPoint(e.X, e.Y);
#if !PocketPC
                                if (!MobileMode)
                                {
                                    rp.OffsetNegative(Core.renderOffset);
                                }
#endif
                                if (m.IsInside((int)rp.X, (int)rp.Y))
                                {
                                    if (OnRouteClick != null)
                                    {
                                        OnRouteClick(m, e);
                                    }
                                    break;
                                }
                                #endregion
                            }
                        }

                        foreach (GMapPolygon m in o.Polygons)
                        {
                            if (m.IsVisible && m.IsHitTestVisible)
                            {
                                #region -- check --
                                if (m.IsInside(FromLocalToLatLng(e.X, e.Y)))
                                {
                                    if (OnPolygonClick != null)
                                    {
                                        OnPolygonClick(m, e);
                                    }
                                    break;
                                }
                                #endregion
                            }
                        }
                    }
                }
            }

            //m_mousepos = e.Location;
            //if(HelperLineOption == HelperLineOptions.ShowAlways)
            //{
            //   base.Invalidate();
            //}

            base.OnMouseClick(e);
        }
#endif
#if !PocketPC
        /// <summary>
        /// apply transformation if in rotation mode
        /// </summary>
        GPoint ApplyRotationInversion(int x, int y)
        {
            GPoint ret = new GPoint(x, y);

            if (IsRotated)
            {

                System.Drawing.Point[] tt = new System.Drawing.Point[] { new System.Drawing.Point(x, y) };
                rotationMatrixInvert.TransformPoints(tt);
                var f = tt[0];

                ret.X = f.X;
                ret.Y = f.Y;
            }

            return ret;
        }

        /// <summary>
        /// apply transformation if in rotation mode
        /// </summary>
        GPoint ApplyRotation(int x, int y)
        {
            GPoint ret = new GPoint(x, y);

            if (IsRotated)
            {
                System.Drawing.Point[] tt = new System.Drawing.Point[] { new System.Drawing.Point(x, y) };
                rotationMatrix.TransformPoints(tt);
                var f = tt[0];

                ret.X = f.X;
                ret.Y = f.Y;
            }

            return ret;
        }

        Cursor cursorBefore = Cursors.Default;
#endif

        /// <summary>
        /// Gets the width and height of a rectangle centered on the point the mouse
        /// button was pressed, within which a drag operation will not begin.
        /// </summary>
#if !PocketPC
        public Size DragSize = SystemInformation.DragSize;
#else
      public Size DragSize = new Size(4, 4);
#endif

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!Core.IsDragging && !Core.mouseDown.IsEmpty)
            {
#if PocketPC
            GPoint p = new GPoint(e.X, e.Y);
#else
                GPoint p = ApplyRotationInversion(e.X, e.Y);
#endif
                if (Math.Abs(p.X - Core.mouseDown.X) * 2 >= DragSize.Width || Math.Abs(p.Y - Core.mouseDown.Y) * 2 >= DragSize.Height)
                {
                    Core.BeginDrag(Core.mouseDown);
                }
            }

            if (Core.IsDragging)
            {
                if (!isDragging)
                {
                    isDragging = true;
                    Debug.WriteLine("IsDragging = " + isDragging);

#if !PocketPC
                    cursorBefore = this.Cursor;
                    this.Cursor = Cursors.SizeAll;
#endif
                }

                if (BoundsOfMap.HasValue && !BoundsOfMap.Value.Contains(Position))
                {
                    // ...
                }
                else
                {
#if !PocketPC
                    Core.mouseCurrent = ApplyRotationInversion(e.X, e.Y);
#else
               Core.mouseCurrent = new GPoint(e.X, e.Y);
#endif
                    Core.Drag(Core.mouseCurrent);

#if !PocketPC
                    if (MobileMode)
                    {
                        ForceUpdateOverlays();
                    }
#else
               ForceUpdateOverlays();
#endif

                    base.Invalidate();
                }
            }
            else
            {
#if !PocketPC
                if (isSelected && !selectionStart.IsEmpty && (Form.ModifierKeys == Keys.Alt || Form.ModifierKeys == Keys.Shift || DisableAltForSelection))
                {
                    selectionEnd = FromLocalToLatLng(e.X, e.Y);
                    {
                        GMap.NET.PointLatLng p1 = selectionStart;
                        GMap.NET.PointLatLng p2 = selectionEnd;

                        double x1 = Math.Min(p1.Lng, p2.Lng);
                        double y1 = Math.Max(p1.Lat, p2.Lat);
                        double x2 = Math.Max(p1.Lng, p2.Lng);
                        double y2 = Math.Min(p1.Lat, p2.Lat);

                        SelectedArea = new RectLatLng(y1, x1, x2 - x1, y1 - y2);
                    }
                }
                else
#endif
                    if (Core.mouseDown.IsEmpty)
                    {
                        for (int i = Overlays.Count - 1; i >= 0; i--)
                        {
                            GMapOverlay o = Overlays[i];
                            if (o != null && o.IsVisibile)
                            {
                                foreach (GMapMarker m in o.Markers)
                                {
                                    if (m.IsVisible && m.IsHitTestVisible)
                                    {
                                        #region -- check --
#if !PocketPC
                                        if ((MobileMode && m.LocalArea.Contains(e.X, e.Y)) || (!MobileMode && m.LocalAreaInControlSpace.Contains(e.X, e.Y)))
#else
                           if(m.LocalArea.Contains(e.X, e.Y))
#endif
                                        {
                                            if (!m.IsMouseOver)
                                            {
#if !PocketPC
                                                SetCursorHandOnEnter();
#endif
                                                m.IsMouseOver = true;
                                                IsMouseOverMarker = true;

                                                if (OnMarkerEnter != null)
                                                {
                                                    OnMarkerEnter(m);
                                                }

                                                Invalidate();
                                            }
                                        }
                                        else if (m.IsMouseOver)
                                        {
                                            m.IsMouseOver = false;
                                            IsMouseOverMarker = false;
#if !PocketPC
                                            RestoreCursorOnLeave();
#endif
                                            if (OnMarkerLeave != null)
                                            {
                                                OnMarkerLeave(m);
                                            }

                                            Invalidate();
                                        }
                                        #endregion
                                    }
                                }

#if !PocketPC
                                foreach (GMapRoute m in o.Routes)
                                {
                                    if (m.IsVisible && m.IsHitTestVisible)
                                    {
                                        #region -- check --

                                        GPoint rp = new GPoint(e.X, e.Y);
#if !PocketPC
                                        if (!MobileMode)
                                        {
                                            rp.OffsetNegative(Core.renderOffset);
                                        }
#endif
                                        if (m.IsInside((int)rp.X, (int)rp.Y))
                                        {
                                            if (!m.IsMouseOver)
                                            {
#if !PocketPC
                                                SetCursorHandOnEnter();
#endif
                                                m.IsMouseOver = true;
                                                IsMouseOverRoute = true;

                                                if (OnRouteEnter != null)
                                                {
                                                    OnRouteEnter(m);
                                                }

                                                Invalidate();
                                            }
                                        }
                                        else
                                        {
                                            if (m.IsMouseOver)
                                            {
                                                m.IsMouseOver = false;
                                                IsMouseOverRoute = false;
#if !PocketPC
                                                RestoreCursorOnLeave();
#endif
                                                if (OnRouteLeave != null)
                                                {
                                                    OnRouteLeave(m);
                                                }

                                                Invalidate();
                                            }
                                        }
                                        #endregion
                                    }
                                }
#endif

                                foreach (GMapPolygon m in o.Polygons)
                                {
                                    if (m.IsVisible && m.IsHitTestVisible)
                                    {
                                        #region -- check --
#if !PocketPC
                                        GPoint rp = new GPoint(e.X, e.Y);

                                        if (!MobileMode)
                                        {
                                            rp.OffsetNegative(Core.renderOffset);
                                        }

                                        if (m.IsInsideLocal((int)rp.X, (int)rp.Y))
#else
                              if (m.IsInside(FromLocalToLatLng(e.X, e.Y)))
#endif
                                        {
                                            if (!m.IsMouseOver)
                                            {
#if !PocketPC
                                                SetCursorHandOnEnter();
#endif
                                                m.IsMouseOver = true;
                                                IsMouseOverPolygon = true;

                                                if (OnPolygonEnter != null)
                                                {
                                                    OnPolygonEnter(m);
                                                }

                                                Invalidate();
                                            }
                                        }
                                        else
                                        {
                                            if (m.IsMouseOver)
                                            {
                                                m.IsMouseOver = false;
                                                IsMouseOverPolygon = false;
#if !PocketPC
                                                RestoreCursorOnLeave();
#endif
                                                if (OnPolygonLeave != null)
                                                {
                                                    OnPolygonLeave(m);
                                                }

                                                Invalidate();
                                            }
                                        }
                                        #endregion
                                    }
                                }
                            }
                        }
                    }

#if !PocketPC
                if (renderHelperLine)
                {
                    base.Invalidate();
                }
#endif
            }

            base.OnMouseMove(e);
        }

#if !PocketPC

        internal void RestoreCursorOnLeave()
        {
            if (overObjectCount <= 0 && cursorBefore != null)
            {
                overObjectCount = 0;
                this.Cursor = this.cursorBefore;
                cursorBefore = null;
            }
        }

        internal void SetCursorHandOnEnter()
        {
            if (overObjectCount <= 0 && Cursor != Cursors.Hand)
            {
                overObjectCount = 0;
                cursorBefore = this.Cursor;
                this.Cursor = Cursors.Hand;
            }
        }

        /// <summary>
        /// 防止聚焦地图，如果鼠标进入它的面积
        /// </summary>
        public bool DisableFocusOnMouseEnter = false;

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (!DisableFocusOnMouseEnter)
            {
                Focus();
            }
            mouseIn = true;
        }

        bool mouseIn = false;

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            mouseIn = false;
        }

        /// <summary>
        /// reverses MouseWheel zooming direction
        /// </summary>
        public bool InvertedMouseWheelZooming = false;

        /// <summary>
        /// lets you zoom by MouseWheel even when pointer is in area of marker
        /// </summary>
        public bool IgnoreMarkerOnMouseWheel = false;

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (mouseIn && (!IsMouseOverMarker || IgnoreMarkerOnMouseWheel) && !Core.IsDragging)
            {
                if (Core.mouseLastZoom.X != e.X && Core.mouseLastZoom.Y != e.Y)
                {
                    if (MouseWheelZoomType == MouseWheelZoomType.MousePositionAndCenter)
                    {
                        Core.position = FromLocalToLatLng(e.X, e.Y);
                    }
                    else if (MouseWheelZoomType == MouseWheelZoomType.ViewCenter)
                    {
                        Core.position = FromLocalToLatLng((int)Width / 2, (int)Height / 2);
                    }
                    else if (MouseWheelZoomType == MouseWheelZoomType.MousePositionWithoutCenter)
                    {
                        Core.position = FromLocalToLatLng(e.X, e.Y);
                    }

                    Core.mouseLastZoom.X = e.X;
                    Core.mouseLastZoom.Y = e.Y;
                }

                // set mouse position to map center
                if (MouseWheelZoomType != MouseWheelZoomType.MousePositionWithoutCenter)
                {
                    if (!GMaps.Instance.IsRunningOnMono)
                    {
                        System.Drawing.Point p = PointToScreen(new System.Drawing.Point(Width / 2, Height / 2));
                        Stuff.SetCursorPos((int)p.X, (int)p.Y);
                    }
                }

                Core.MouseWheelZooming = true;

                if (e.Delta > 0)
                {
                    if (!InvertedMouseWheelZooming)
                    {
                        Zoom = ((int)Zoom) + 1;
                    }
                    else
                    {
                        Zoom = ((int)(Zoom + 0.99)) - 1;
                    }
                }
                else if (e.Delta < 0)
                {
                    if (!InvertedMouseWheelZooming)
                    {
                        Zoom = ((int)(Zoom + 0.99)) - 1;
                    }
                    else
                    {
                        Zoom = ((int)Zoom) + 1;
                    }
                }

                Core.MouseWheelZooming = false;
            }
        }
#endif
        #endregion

        #region IGControl Members

        /// <summary>
        /// Call it to empty tile cache & reload tiles
        /// </summary>
        public void ReloadMap()
        {
            Core.ReloadMap();
        }

        /// <summary>
        /// 当前设置使用位置关键字
        /// set current position using keywords
        /// </summary>
        /// <param name="keys"></param>
        /// <returns>true if successfull</returns>
        public GeoCoderStatusCode SetPositionByKeywords(string keys)
        {
            GeoCoderStatusCode status = GeoCoderStatusCode.Unknow;

            GeocodingProvider gp = MapProvider as GeocodingProvider;
            if (gp == null)
            {
                gp = GMapProviders.OpenStreetMap as GeocodingProvider;
            }

            if (gp != null)
            {
                var pt = gp.GetPoint(keys, out status);
                if (status == GeoCoderStatusCode.G_GEO_SUCCESS && pt.HasValue)
                {
                    Position = pt.Value;
                }
            }

            return status;
        }

        /// <summary>
        /// 从本地控制被世界坐标的坐标
        /// gets world coordinate from local control coordinate 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public PointLatLng FromLocalToLatLng(int x, int y)
        {
#if !PocketPC
            if (MapRenderTransform.HasValue)
            {
                //var xx = (int)(Core.renderOffset.X + ((x - Core.renderOffset.X) / MapRenderTransform.Value));
                //var yy = (int)(Core.renderOffset.Y + ((y - Core.renderOffset.Y) / MapRenderTransform.Value));

                //PointF center = new PointF(Core.Width / 2, Core.Height / 2);

                //Matrix m = new Matrix();
                //m.Translate(-Core.renderOffset.X, -Core.renderOffset.Y);
                //m.Scale(MapRenderTransform.Value, MapRenderTransform.Value);

                //System.Drawing.Point[] tt = new System.Drawing.Point[] { new System.Drawing.Point(x, y) };
                //m.TransformPoints(tt);
                //var z = tt[0];

                //

                x = (int)(Core.renderOffset.X + ((x - Core.renderOffset.X) / MapRenderTransform.Value));
                y = (int)(Core.renderOffset.Y + ((y - Core.renderOffset.Y) / MapRenderTransform.Value));
            }

            if (IsRotated)
            {
                System.Drawing.Point[] tt = new System.Drawing.Point[] { new System.Drawing.Point(x, y) };
                rotationMatrixInvert.TransformPoints(tt);
                var f = tt[0];

                if (VirtualSizeEnabled)
                {
                    f.X += (Width - Core.vWidth) / 2;
                    f.Y += (Height - Core.vHeight) / 2;
                }

                x = f.X;
                y = f.Y;
            }
#endif
            return Core.FromLocalToLatLng(x, y);
        }



        /// <summary>
        /// 得到来自世界局部坐标坐标
        /// gets local coordinate from world coordinate
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public GPoint FromLatLngToLocal(PointLatLng point)
        {
            GPoint ret = Core.FromLatLngToLocal(point);

#if !PocketPC
            if (MapRenderTransform.HasValue)
            {
                ret.X = (int)(Core.renderOffset.X + ((Core.renderOffset.X - ret.X) * -MapRenderTransform.Value));
                ret.Y = (int)(Core.renderOffset.Y + ((Core.renderOffset.Y - ret.Y) * -MapRenderTransform.Value));
            }

            if (IsRotated)
            {
                System.Drawing.Point[] tt = new System.Drawing.Point[] { new System.Drawing.Point((int)ret.X, (int)ret.Y) };
                rotationMatrix.TransformPoints(tt);
                var f = tt[0];

                if (VirtualSizeEnabled)
                {
                    f.X += (Width - Core.vWidth) / 2;
                    f.Y += (Height - Core.vHeight) / 2;
                }

                ret.X = f.X;
                ret.Y = f.Y;
            }

#endif
            return ret;
        }

#if !PocketPC

        /// <summary>
        /// 显示地图数据库导出对话框
        /// shows map db export dialog
        /// </summary>
        /// <returns></returns>
        public bool ShowExportDialog()
        {
            using (FileDialog dlg = new SaveFileDialog())
            {
                dlg.CheckPathExists = true;
                dlg.CheckFileExists = false;
                dlg.AddExtension = true;
                dlg.DefaultExt = "gmdb";
                dlg.ValidateNames = true;
                dlg.Title = "GMap.NET: Export map to db, if file exsist only new data will be added";
                dlg.FileName = "DataExp";
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dlg.Filter = "GMap.NET DB files (*.gmdb)|*.gmdb";
                dlg.FilterIndex = 1;
                dlg.RestoreDirectory = true;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    bool ok = GMaps.Instance.ExportToGMDB(dlg.FileName);
                    if (ok)
                    {
                        MessageBox.Show("Complete!", "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed!", "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    return ok;
                }
            }

            return false;
        }

        /// <summary>
        /// 显示地图dbimport对话框
        /// shows map dbimport dialog
        /// </summary>
        /// <returns></returns>
        public bool ShowImportDialog()
        {
            using (FileDialog dlg = new OpenFileDialog())
            {
                dlg.CheckPathExists = true;
                dlg.CheckFileExists = false;
                dlg.AddExtension = true;
                dlg.DefaultExt = "gmdb";
                dlg.ValidateNames = true;
                dlg.Title = "GMap.NET: Import to db, only new data will be added";
                dlg.FileName = "DataImport";
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dlg.Filter = "GMap.NET DB files (*.gmdb)|*.gmdb";
                dlg.FilterIndex = 1;
                dlg.RestoreDirectory = true;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    bool ok = GMaps.Instance.ImportFromGMDB(dlg.FileName);
                    if (ok)
                    {
                        MessageBox.Show("Complete!", "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ReloadMap();
                    }
                    else
                    {
                        MessageBox.Show("Failed!", "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    return ok;
                }
            }

            return false;
        }
#endif

        private ScaleModes scaleMode = ScaleModes.Integer;

        [Category("GMap.NET")]
        [Description("map scale type")]
        public ScaleModes ScaleMode
        {
            get
            {
                return scaleMode;
            }
            set
            {
                scaleMode = value;
            }
        }

        [Category("GMap.NET"), DefaultValue(0)]
        public double Zoom
        {
            get
            {
                return zoomReal;
            }
            set
            {
                if (zoomReal != value)
                {
                    Debug.WriteLine("ZoomPropertyChanged: " + zoomReal + " -> " + value);

                    if (value > MaxZoom)
                    {
                        zoomReal = MaxZoom;
                    }
                    else if (value < MinZoom)
                    {
                        zoomReal = MinZoom;
                    }
                    else
                    {
                        zoomReal = value;
                    }

#if !PocketPC
                    double remainder = value % 1;
                    if (ScaleMode == ScaleModes.Fractional && remainder != 0)
                    {
                        float scaleValue = (float)Math.Pow(2d, remainder);
                        {
                            MapRenderTransform = scaleValue;
                        }

                        ZoomStep = Convert.ToInt32(value - remainder);
                    }
                    else
#endif
                    {
#if !PocketPC
                        MapRenderTransform = null;
#endif
                        ZoomStep = (int)Math.Floor(value);
                        //zoomReal = ZoomStep;
                    }

                    if (Core.IsStarted && !IsDragging)
                    {
                        ForceUpdateOverlays();
                    }
                }
            }
        }

        /// <summary>
        /// 地图缩放级别
        /// map zoom level
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal int ZoomStep
        {
            get
            {
                return Core.Zoom;
            }
            set
            {
                if (value > MaxZoom)
                {
                    Core.Zoom = MaxZoom;
                }
                else if (value < MinZoom)
                {
                    Core.Zoom = MinZoom;
                }
                else
                {
                    Core.Zoom = value;
                }
            }
        }

        /// <summary>
        /// 当前地图的中心位置
        /// current map center position
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public PointLatLng Position
        {
            get
            {
                return Core.Position;
            }
            set
            {
                Core.Position = value;

                if (Core.IsStarted)
                {
                    ForceUpdateOverlays();
                }
            }
        }

        /// <summary>
        /// 在像素坐标当前位置
        /// current position in pixel coordinates
        /// </summary>
        [Browsable(false)]
        public GPoint PositionPixel
        {
            get
            {
                return Core.PositionPixel;
            }
        }

        /// <summary>
        /// 缓存的位置
        /// location of cache
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public string CacheLocation
        {
            get
            {
#if !DESIGN
                return CacheLocator.Location;
#else
            return string.Empty;
#endif
            }
            set
            {
#if !DESIGN
                CacheLocator.Location = value;
#endif
            }
        }

        bool isDragging = false;

        /// <summary>
        /// 是用户拖动地图
        /// is user dragging map
        /// </summary>
        [Browsable(false)]
        public bool IsDragging
        {
            get
            {
                return isDragging;
            }
        }

        bool isMouseOverMarker;
        internal int overObjectCount = 0;

        /// <summary>
        /// 鼠标标记
        /// is mouse over marker
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsMouseOverMarker
        {
            get
            {
                return isMouseOverMarker;
            }
            internal set
            {
                isMouseOverMarker = value;
                overObjectCount += value ? 1 : -1;
            }
        }

        bool isMouseOverRoute;

        /// <summary>
        /// 是鼠标路线
        /// is mouse over route
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsMouseOverRoute
        {
            get
            {
                return isMouseOverRoute;
            }
            internal set
            {
                isMouseOverRoute = value;
                overObjectCount += value ? 1 : -1;
            }
        }

        bool isMouseOverPolygon;

        /// <summary>
        /// 是鼠标悬停多边形
        /// is mouse over polygon
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsMouseOverPolygon
        {
            get
            {
                return isMouseOverPolygon;
            }
            internal set
            {
                isMouseOverPolygon = value;
                overObjectCount += value ? 1 : -1;
            }
        }

        /// <summary>
        /// 获取当前地图视图顶部/左坐标，宽度在经度，高度纬度
        /// gets current map view top/left coordinate, width in Lng, height in Lat
        /// </summary>
        [Browsable(false)]
        public RectLatLng ViewArea
        {
            get
            {
                return Core.ViewArea;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GMapProvider MapProvider
        {
            get
            {
                return Core.Provider;
            }
            set
            {
                if (Core.Provider == null || !Core.Provider.Equals(value))
                {
                    Debug.WriteLine("MapType: " + Core.Provider.Name + " -> " + value.Name);

                    RectLatLng viewarea = SelectedArea;
                    if (viewarea != RectLatLng.Empty)
                    {
                        Position = new PointLatLng(viewarea.Lat - viewarea.HeightLat / 2, viewarea.Lng + viewarea.WidthLng / 2);
                    }
                    else
                    {
                        viewarea = ViewArea;
                    }

                    Core.Provider = value;

                    if (Core.IsStarted)
                    {
                        if (Core.zoomToArea)
                        {
                            // restore zoomrect as close as possible
                            if (viewarea != RectLatLng.Empty && viewarea != ViewArea)
                            {
                                int bestZoom = Core.GetMaxZoomToFitRect(viewarea);
                                if (bestZoom > 0 && Zoom != bestZoom)
                                {
                                    Zoom = bestZoom;
                                }
                            }
                        }
                        else
                        {
                            ForceUpdateOverlays();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 航线是否启用
        /// </summary>
        [Category("GMap.NET")]
        public bool RoutesEnabled
        {
            get
            {
                return Core.RoutesEnabled;
            }
            set
            {
                Core.RoutesEnabled = value;
            }
        }

        /// <summary>
        /// 是否启用多边形
        /// </summary>
        [Category("GMap.NET")]
        public bool PolygonsEnabled
        {
            get
            {
                return Core.PolygonsEnabled;
            }
            set
            {
                Core.PolygonsEnabled = value;
            }
        }

        /// <summary>
        /// 是否启用标志
        /// </summary>
        [Category("GMap.NET")]
        public bool MarkersEnabled
        {
            get
            {
                return Core.MarkersEnabled;
            }
            set
            {
                Core.MarkersEnabled = value;
            }
        }

        /// <summary>
        ///用户是否可以拖动地图
        /// </summary>
        [Category("GMap.NET")]
        public bool CanDragMap
        {
            get
            {
                return Core.CanDragMap;
            }
            set
            {
                Core.CanDragMap = value;
            }
        }

        /// <summary>
        /// 地图渲染模式
        /// </summary>
        [Browsable(false)]
        public RenderMode RenderMode
        {
            get
            {
                return Core.RenderMode;
            }
            internal set
            {
                Core.RenderMode = value;
            }
        }

        /// <summary>
        ///获取地图管理器
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GMaps Manager
        {
            get
            {
                return GMaps.Instance;
            }
        }

        #endregion

        #region 关于地图加载的事件 IGControl event Members

        /// <summary>
        /// 当前位置变化发生
        /// </summary>
        public event PositionChanged OnPositionChanged
        {
            add
            {
                Core.OnCurrentPositionChanged += value;
            }
            remove
            {
                Core.OnCurrentPositionChanged -= value;
            }
        }

        /// <summary>
        /// 当地图片加载完成
        /// occurs when tile set load is complete
        /// </summary>
        public event TileLoadComplete OnTileLoadComplete
        {
            add
            {
                Core.OnTileLoadComplete += value;
            }
            remove
            {
                Core.OnTileLoadComplete -= value;
            }
        }

        /// <summary>
        /// 当地图片开始加载的时候
        /// occurs when tile set is starting to load
        /// </summary>
        public event TileLoadStart OnTileLoadStart
        {
            add
            {
                Core.OnTileLoadStart += value;
            }
            remove
            {
                Core.OnTileLoadStart -= value;
            }
        }

        /// <summary>
        /// 发生在地图上拖动
        /// occurs on map drag
        /// </summary>
        public event MapDrag OnMapDrag
        {
            add
            {
                Core.OnMapDrag += value;
            }
            remove
            {
                Core.OnMapDrag -= value;
            }
        }

        /// <summary>
        /// 发生在地图缩放改
        /// occurs on map zoom changed
        /// </summary>
        public event MapZoomChanged OnMapZoomChanged
        {
            add
            {
                Core.OnMapZoomChanged += value;
            }
            remove
            {
                Core.OnMapZoomChanged -= value;
            }
        }

        /// <summary>
        /// 发生在地图类型改变
        /// occures on map type changed
        /// </summary>
        public event MapTypeChanged OnMapTypeChanged
        {
            add
            {
                Core.OnMapTypeChanged += value;
            }
            remove
            {
                Core.OnMapTypeChanged -= value;
            }
        }

        /// <summary>
        /// 发生在显示空的地图片
        /// occurs on empty tile displayed
        /// </summary>
        public event EmptyTileError OnEmptyTileError
        {
            add
            {
                Core.OnEmptyTileError += value;
            }
            remove
            {
                Core.OnEmptyTileError -= value;
            }
        }

        #endregion

        #region 序列化
#if !PocketPC

        static readonly BinaryFormatter BinaryFormatter = new BinaryFormatter();

        /// <summary>
        /// 序列化覆盖。
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void SerializeOverlays(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            // Create an array from the overlays
            GMapOverlay[] overlayArray = new GMapOverlay[this.Overlays.Count];
            this.Overlays.CopyTo(overlayArray, 0);

            // 序列化覆盖
            BinaryFormatter.Serialize(stream, overlayArray);
        }

        /// <summary>
        /// 反序列化覆盖。
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void DeserializeOverlays(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            // De-serialize the overlays
            GMapOverlay[] overlayArray = BinaryFormatter.Deserialize(stream) as GMapOverlay[];

            // Populate the collection of overlays.
            foreach (GMapOverlay overlay in overlayArray)
            {
                overlay.Control = this;
                this.Overlays.Add(overlay);
            }

            this.ForceUpdateOverlays();
        }

#endif
    }
        #endregion

    public enum ScaleModes
    {
        /// <summary>
        /// no scaling
        /// </summary>
        Integer,

#if !PocketPC
        /// <summary>
        /// 扩展到分数的水平，目前的版本不处理物体位置正确，
        /// http://greatmaps.codeplex.com/workitem/16046
        /// </summary>
        Fractional,
#endif
    }
    //掌上电脑
#if !PocketPC
    /// <summary>
    /// 助手行选项
    /// </summary>
    public enum HelperLineOptions
    {
        /// <summary>
        /// 不显示
        /// </summary>
        DontShow = 0,
        /// <summary>
        /// 一直显示
        /// </summary>
        ShowAlways = 1,
        /// <summary>
        /// 显示在键修改
        /// </summary>
        ShowOnModifierKey = 2
    }

    public delegate void SelectionChange(RectLatLng Selection, bool ZoomToFit);
#endif
}
