using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MAVLink;
using SanHeGroundStation.Controls;
using SanHeGroundStation.Forms;
using SanHeGroundStation.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace SanHeGroundStation
{

    //解析数据帧委托
    public delegate void AnalyzeMavLinkFrame(byte[] mavLinkFrame_2);
    public partial class MainForm : Form
    {
        GMapOverlay gMapOverlay = new GMapOverlay("mark");//多边形图层
        GMapOverlay gMapOverlay_Rouite = new GMapOverlay("rouite");//路线图层
        public GMapMarker gMapMarker;
        public GMapPolygon gMapPolygon = null;
        public GMapRoute gMapRoute = null;
        public GMapMarkerWP wp = null;//普通的标记点
        public GMapMarkerWP wp1 = null;//home点的标记点
        public GMapMarkerWP landWP = null;//着陆点标记
        public List<GMapMarkerWP> wpsList = new List<GMapMarkerWP>();//标记点集合
        private List<PointLatLng> pointsList=new List<PointLatLng>();//标记点位置集合--经度维度信息
        private List<PointLatLng> drawRouiteList = new List<PointLatLng>();//记录轨迹
        private List<GMapMarker> RouiteMarkList = new List<GMapMarker>();//轨迹上的标记点
        private GMapMarker mark = null;
        public PointLatLngAlt pointlatlngalt = null;
        public List<PointLatLngAlt> pointlatlngaltList = new List<PointLatLngAlt>();//标记点集合--经度维度高度信息
        SerialPort sp = new SerialPort();
        public AnalyzeMavLinkFrame analyzeMavLinkFrame;
        public MainForm()
        {
            command_Ack.result = 255;
            //mission_request.seq = 255;
            mission_Ack.type = 255;
            mission_count.count = 255;
            //mission_iteam.seq = 255;
            InitializeComponent();
            cbMoShi.DataSource = new string[] { "自稳", "定高", "定点", "任务", "环绕", "返航", "降落", "引导" };
            this.SetHome.Visible = false;

           
            

        }

        private void setBattery()
        {
            if (labYuLiang.Text!="")
            {
                this.battery1.BackColor = Color.Green;
            }
        }


       // StreamWriter sw1 = new StreamWriter(new FileStream(@"C:\Users\Administrator\Desktop\正确的帧.txt", FileMode.Create));
        //StreamWriter sw2 = new StreamWriter(new FileStream(@"C:\Users\WangZheng\Desktop\全部的帧.txt", FileMode.Create));
        List<byte> mavDataList = new List<byte>();
        MAVLink.MavLink.MavLinkFrame_2 dataFrame_2;
        byte[] mavLinkFrame = null;
        MavStatus mavStatus = new MavStatus();
        byte[] oldHeartPack = new byte[17];
        int bufferNum;
        public double Alt;
        public bool isFirstClick = true;//判断是不是第一次点击 地图
        private bool isMouseLeftDown = false;//鼠标左键是否按下
        private bool isDragingMarker = false;//是否正在拖动航点
        private bool isDragingMap = false;//是否在拖动地图
        private bool isDrawRouite = false;//是否化路线
        private int markerNum = 0;//当前标记的个数
        GMapMarkerWP mouseOnMarkerWP = null;//鼠标下的航点
        private int dragingMarkerIndex;//正在拖动标记的标记的索引
        private int deleteingMarkerIndex;//正在删除标记的标记的索引
        private bool IsChick=false;//检查航点规划是否被选中过

        /// <summary>
        /// 检查航点规划建是否被选中
        /// </summary>
        /// <returns></returns>
        int ClickNum = 0;//记录单击次数

        /// <summary>
        /// 进入航点规划界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 命令ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ClickNum++;
            if(ClickNum%2==0)
            {
                //this.命令ToolStripMenuItem.BackColor = System.Drawing.Color.Transparent;//点击偶数次数时表示是离开编辑模式，颜色恢复
                this.pelPoint.Visible = false;
                this.label20.Visible = false;
                this.label22.Visible = false;
                this.button1.Visible = false;
                this.button2.Visible = false;
                this.button3.Visible = false;
                this.button4.Visible = false;
                this.button5.Visible = false;
                this.button6.Visible = false;
                //this.SetHome.Visible = false;
            }
            else if (ClickNum % 2 == 1)
            {
                //this.命令ToolStripMenuItem.BackColor = System.Drawing.Color.LightBlue;///点击奇数次数时表示进入编辑模式，颜色改变作为提醒
                this.label20.Visible = true;
                this.label22.Visible = true;
                this.button1.Visible = true;
                this.button2.Visible = true;
                this.button3.Visible = true;
                this.button4.Visible = true;
                this.button5.Visible = true;
                this.button6.Visible = true;
                this.pelPoint.Visible = true;
                //this.SetHome.Visible = true;
            }
        }

       
        

        #region 鼠标相关事件

        private void MainMap_MouseDown(object sender, MouseEventArgs e)
        {
                if (e.Button == MouseButtons.Left)
                {
                    isMouseLeftDown = true;
                    isDragingMarker = false;//不是正在拖动航点
                    isDragingMap = false;//不是正在拖动地图
                    return;
                }

        }
        private void MainMap_MouseMove(object sender, MouseEventArgs e)
        {
            /*------判断拖动地图-----------------------------判断拖动标记----------
             * 1、isMouseLeftDown=true               1、isMouseLetDown=true
             * 2、mouseOnMarkerWP=null鼠标下的航点   2、mouseOnMarkerWP！=null
             * 3、isDargingMarker=false              3、isDragingMap=false
             */

            this.label3.Text = MainMap.FromLocalToLatLng(e.X, e.Y).Lat.ToString();
            this.label2.Text = MainMap.FromLocalToLatLng(e.X, e.Y).Lng.ToString();
            if (isMouseLeftDown && mouseOnMarkerWP == null)//按下鼠标移动 并且变量Marker是空 -----拖动地图
            {
                isDragingMap = true;
                isDragingMarker = false;
                return;
            }
            if (isMouseLeftDown && mouseOnMarkerWP != null)//拖动图标
            {
                if (pointsList.Contains(mouseOnMarkerWP.Position))
                {
                    dragingMarkerIndex = pointsList.IndexOf(mouseOnMarkerWP.Position);//将移动的这个点的索引记录下来
                }
                mouseOnMarkerWP.Position = MainMap.FromLocalToLatLng(e.X, e.Y);


                isDragingMarker = true;
                isDragingMap = false;
            }
        }
        private void MainMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && ModifierKeys == Keys.Alt)//区域缓存
            {
                RectLatLng area = MainMap.SelectedArea;
                if (!area.IsEmpty)
                {
                    DialogResult res = MessageBox.Show("当前处在的层数 = " + (int)MainMap.Zoom + " !请确认缓存.", "提示", MessageBoxButtons.YesNo);

                    if (res == DialogResult.Yes)
                    {
                        for (int i = 1; i <= MainMap.MaxZoom; i++)
                        {
                            TilePrefetcher obj = new TilePrefetcher();
                            obj.ShowCompleteMessage = false;
                            obj.Start(area, i, MainMap.MapProvider, 100, 0);

                            if (obj.UserAborted)
                                break;
                        }
                    }

                }

                return;
            }

            if (e.Button == MouseButtons.Right)
            {
                return;
            }
            else
            {
                if (isDrawRouite)//模拟飞行轨迹
                {
                    DrawRouite(new PointLatLngAlt(MainMap.FromLocalToLatLng(e.X, e.Y).Lat, MainMap.FromLocalToLatLng(e.X, e.Y).Lng, Global.defaultAlt));
                    //mark = new GMarkerGoogle(MainMap.FromLocalToLatLng(e.X, e.Y), GMarkerGoogleType.orange);//可以选择飞机图片替换
                    //mark.ToolTipText = string.Format("高度:{0},经度:{1},维度:{2}", Global.defaultAlt, MainMap.FromLocalToLatLng(e.X, e.Y).Lng, MainMap.FromLocalToLatLng(e.X, e.Y).Lat);
                    //mark.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                    //RouiteMarkList.Add(mark);
                    //drawRouiteList.Add(mark.Position);
                    //if (RouiteMarkList.Count > 1)//不是第一次点击
                    //{
                    //    setDistance(drawRouiteList);
                    //    gMapOverlay_Rouite.Markers.Remove(RouiteMarkList[0]);//删除路线层第一个点
                    //    gMapOverlay_Rouite.Routes.Remove(gMapRoute);//删除路线层上的路线
                    //    RouiteMarkList.Remove(RouiteMarkList[0]);//移除点集合
                    //}
                    //gMapRoute.Points.Clear();//删除路线上的点
                    //gMapRoute.Points.AddRange(drawRouiteList);//从新更新
                    //gMapOverlay_Rouite.Markers.Add(mark);
                    //gMapOverlay_Rouite.Routes.Add(gMapRoute);
                    //MainMap.UpdateRouteLocalPosition(gMapRoute);
                    //MainMap.Refresh();
                    return;
                }
                isMouseLeftDown = false;

                ///航点设置是否已选中，选中执行
                if (IsChick )
                {

                    if (isDragingMap)
                    {
                        //拖动地图 什么都不做
                    }
                    else if (isFirstClick)//左键第一次点击 画多边形
                    {
                        markerNum++;
                        isFirstClick = false;
                        //pointsList = new List<PointLatLng>();
                        gMapPolygon = new GMapPolygon(pointsList, "my polygon");
                        gMapPolygon.Fill = new SolidBrush(Color.FromArgb(20, Color.Blue));
                        gMapPolygon.Stroke = new Pen(Color.Green, 2);
                        gMapPolygon.IsHitTestVisible = false;
                        wp = new GMapMarkerWP(MainMap.FromLocalToLatLng(e.X, e.Y), markerNum.ToString());
                        SetPointLatLngAlt(MainMap.FromLocalToLatLng(e.X, e.Y), Global.defaultAlt);//设置纬度经度高度
                        wp.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                        wp.ToolTipText = string.Format("高度:{0},经度:{1},维度:{2}", Global.defaultAlt, wp.Position.Lng, wp.Position.Lat);
                        wpsList.Add(wp);//添加到标记点 集合中
                        pointsList.Add(wp.Position);//将标记点添加到 点PointsList中
                        gMapPolygon.Points.Clear();
                        gMapPolygon.Points.AddRange(pointsList);//将 点集合添加到多边形中
                        gMapOverlay.Markers.Add(wp);//将点添加到第二层
                        gMapOverlay.Polygons.Add(gMapPolygon);//将多边形添加到第二层
                        this.MainMap.Position = this.MainMap.Position;
                        MainMap.Refresh();
                        pointWriteDGV(MavLink.MAV_CMD.WAYPOINT, new PointLatLngAlt(wp.Position, Global.defaultAlt));
                    }
                    else if (!isDragingMarker && !isDragingMap && !isFirstClick)//左键不是第一次点击 不是拖动 画多边形
                    {
                        markerNum++;
                        wp = new GMapMarkerWP(MainMap.FromLocalToLatLng(e.X, e.Y), markerNum.ToString());
                        SetPointLatLngAlt(MainMap.FromLocalToLatLng(e.X, e.Y), Global.defaultAlt);//设置纬度经度高度
                        wp.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                        wp.ToolTipText = string.Format("高度:{0},经度:{1},维度:{2}", Global.defaultAlt, wp.Position.Lng, wp.Position.Lat);
                        pointsList.Add(wp.Position);//将标记点添加到 点List中
                        setDistance(pointsList);//调用测量距离的方法
                        wpsList.Add(wp);
                        gMapPolygon.Points.Clear();
                        gMapOverlay.Polygons.Clear();
                        gMapPolygon.Points.AddRange(pointsList);//将 点集合添加到多边形中
                        MainMap.UpdatePolygonLocalPosition(gMapPolygon);//更新多边形的位置
                        gMapOverlay.Markers.Add(wp);//将点添加到第二层
                        gMapOverlay.Polygons.Add(gMapPolygon);//将多边形添加到第二层
                        if (MainMap.Overlays.Count == 0)//如果点击了删除全部航点的时候 那么Overlay=0了 再也添加不上点了
                        {
                            MainMap.Overlays.Add(gMapOverlay);//将第二层添加到第三层
                        }
                        this.MainMap.Position = this.MainMap.Position;
                        MainMap.Refresh();
                        pointWriteDGV(MavLink.MAV_CMD.WAYPOINT, new PointLatLngAlt(wp.Position, Global.defaultAlt));
                    }
                    else if (isDragingMarker)
                    {
                        if (pointsList.Count > 0)
                        {
                            pointsList[dragingMarkerIndex] = mouseOnMarkerWP.Position;
                            wpsList[dragingMarkerIndex].ToolTipText = string.Format("高度:{0},经度:{1},维度:{2}", pointlatlngaltList[dragingMarkerIndex].Alt, mouseOnMarkerWP.Position.Lng, mouseOnMarkerWP.Position.Lat);
                            setDistance(pointsList);//调用测量距离的方法
                            gMapPolygon.Points.Clear();
                            gMapPolygon.Points.AddRange(pointsList);//将 点集合添加到多边形中
                            this.MainMap.Position = this.MainMap.Position;
                            MainMap.Refresh();
                            dgvPoints.Rows[dragingMarkerIndex].Cells[1].Value = pointsList[dragingMarkerIndex].Lng.ToString();
                            dgvPoints.Rows[dragingMarkerIndex].Cells[2].Value = pointsList[dragingMarkerIndex].Lat.ToString();
                        }
                    }

                }
                else
               {
                   if (!isDragingMap)
                   {
                       MessageBox.Show("须先进入航点编辑模式才能创建航点！", "提示!", MessageBoxButtons.OK);
                   } 
                }
                isDragingMarker = false;
                isDragingMap = false;
            }
        }
        private void MainMap_OnMarkerEnter(GMapMarker item)
        {
            if (isDragingMarker || isDragingMap)
            {
                return;
            }
            mouseOnMarkerWP = item as GMapMarkerWP;
            deleteingMarkerIndex = wpsList.IndexOf(mouseOnMarkerWP);
        }
        private void MainMap_OnMarkerLeave(GMapMarker item)
        {
            if (!isMouseLeftDown)//没有按左键 离开标记
            {
                mouseOnMarkerWP = null;
            }
        }
        #endregion

        #region 右键菜单事件
        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsChick)
            {
                if (mouseOnMarkerWP == null || !wpsList.Contains(mouseOnMarkerWP))
                {
                    MessageBox.Show("请选择标记！", "错误", MessageBoxButtons.OK);
                    return;
                 }
                 pointlatlngaltList.Remove(pointlatlngaltList[deleteingMarkerIndex]);//从经度 维度 高度中移除
                 pointsList.Remove(mouseOnMarkerWP.Position);//从点集合中移除
                 setDistance(pointsList);//实时更新距离的测量
                 gMapOverlay.Markers.Remove(mouseOnMarkerWP);//从第二层 移除 点
                 gMapOverlay.Polygons.Remove(gMapPolygon);//从第二层 移除 多边形
                 MainMap.Overlays.Remove(gMapOverlay);//从第三层移除第二层
                 wpsList.Remove(wpsList[deleteingMarkerIndex]);
                 for (int i = deleteingMarkerIndex; i < wpsList.Count; i++)
                 {
                        wpsList[i].wpno = (Convert.ToInt32(wpsList[i].wpno) - 1).ToString();
                 }
                 gMapPolygon.Points.Clear();
                 gMapPolygon.Points.AddRange(pointsList);

                  MainMap.UpdatePolygonLocalPosition(gMapPolygon);//更新多边形的位置
                 gMapOverlay.Polygons.Add(gMapPolygon);//将多边形 添加到第二层
                 MainMap.Overlays.Add(gMapOverlay);// 将第二层加到 第三层
                 this.MainMap.Position = this.MainMap.Position;
                 MainMap.Refresh();
                 markerNum--;
                 dgvPoints.Rows.RemoveAt(deleteingMarkerIndex);
            }
        }
        private void DeleteAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsChick)
            {
                if (pointsList == null)
                {
                    return;
                }
                pointsList.Clear();//经纬度集合清空
                setDistance(pointsList);
                pointlatlngaltList.Clear();//经纬度高度的集合清空
                gMapOverlay.Markers.Clear();
                gMapOverlay.Polygons.Clear();
                //MainMap.Overlays.Clear();//没有必要把地图上的所有层都清出
                wpsList.Clear();
                markerNum = 0;
                dgvPoints.Rows.Clear();
            }
        }
        private void ModifyAlttoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mouseOnMarkerWP == null || !wpsList.Contains(mouseOnMarkerWP))
            {
                MessageBox.Show("请先选择标记!");
                return;
            }
            MonifyAltForm monifyform = new MonifyAltForm();
            monifyform.ShowDialog();

            pointlatlngaltList[deleteingMarkerIndex].Alt = Global.monifyAlt;//只修改该点的高度
            wpsList[deleteingMarkerIndex].ToolTipText = string.Format("高度:{0},经度:{1},纬度:{2}", Global.monifyAlt, wpsList[deleteingMarkerIndex].Position.Lng, wpsList[deleteingMarkerIndex].Position.Lat);//修改这点的提示
            dgvPoints.Rows[deleteingMarkerIndex].Cells[3].Value = Global.monifyAlt.ToString();
        }
        private void PrefetchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RectLatLng area = MainMap.SelectedArea;
            if (area.IsEmpty)
            {
                DialogResult res = MessageBox.Show("缓存屏幕上的区域1-18层", "提示", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    area = MainMap.ViewArea;
                }
                return;
            }

            if (!area.IsEmpty)
            {
                DialogResult res = MessageBox.Show("当前处在的层数 = " + (int)MainMap.Zoom + " !", "提示", MessageBoxButtons.YesNo);

                if (res == DialogResult.Yes)
                {
                    for (int i = 1; i <= MainMap.MaxZoom; i++)
                    {
                        TilePrefetcher obj = new TilePrefetcher();
                        obj.ShowCompleteMessage = false;
                        obj.Start(area, i, MainMap.MapProvider, 100, 0);

                        if (obj.UserAborted)
                            break;
                    }
                }

            }
            else
            {
                MessageBox.Show("请按住ALT键和鼠标右键选择区域！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }
        }

        private void AreaPrefetchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("请按住ALT键和鼠标右键选择区域！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        /// <summary>
        /// 右键菜单起飞
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TakeOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!beforeSendCommand())
            {
                return;
            }
            SetTakeOffAltForm setTakeOffAltForm = new SetTakeOffAltForm();
            setTakeOffAltForm.ShowDialog();
            int rowIndex = this.dgvPoints.Rows.Add();
            dgvPoints.Rows[rowIndex].Cells[0].Value = MavLink.MAV_CMD.TAKEOFF.ToString();
            dgvPoints.Rows[rowIndex].Cells[1].Value = mavStatus.gps_Row.lat.ToString();
            dgvPoints.Rows[rowIndex].Cells[2].Value = mavStatus.gps_Row.lng.ToString();
            dgvPoints.Rows[rowIndex].Cells[3].Value = Global.takeOffAlt.ToString();
            dgvPoints.Rows[rowIndex].Cells[4].Value = "0";
           // PointLatLng point = new PointLatLng(0, 0);

            //pointWriteDGV(MavLink.MAV_CMD.TAKEOFF, new PointLatLngAlt((float)0, (float)0, Global.defaultAlt));
            //try
            //{
            //    DoCommand(MavLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, Global.takeOffAlt);
            //}
            //catch
            //{
            //    MessageBox.Show("起飞错误!", "错误");
            //}

        }
        /// <summary>
        /// 右键菜单降落
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void landToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!beforeSendCommand())
            {
                return;
            }
            SetLandAltForm setLandAltForm = new SetLandAltForm();
            setLandAltForm.ShowDialog();
            markerNum++;
            landWP = new GMapMarkerWP(new PointLatLng(Convert.ToDouble(this.label3.Text), Convert.ToDouble(this.label2.Text)), markerNum.ToString());
            SetPointLatLngAlt(new PointLatLng(Convert.ToDouble(this.label3.Text), Convert.ToDouble(this.label2.Text)), Global.landAlt);//设置纬度经度高度
            landWP.ToolTipMode = MarkerTooltipMode.OnMouseOver;
            landWP.ToolTipText = string.Format("高度:{0},经度:{1},维度:{2}", Global.landAlt, landWP.Position.Lng, landWP.Position.Lat);

            if (pointsList == null)
            {
                pointsList = new List<PointLatLng>();
            }
            pointsList.Add(landWP.Position);//将标记点添加到 点List中
            setDistance(pointsList);//调用测量距离的方法
            wpsList.Add(landWP);
            if (gMapPolygon == null)
            {
                gMapPolygon = new GMapPolygon(pointsList, "my polygon");
            }
            if (gMapOverlay == null)
            {
                gMapOverlay = new GMapOverlay();
            }
            gMapPolygon.Points.Clear();
            gMapOverlay.Polygons.Clear();
            gMapPolygon.Points.AddRange(pointsList);//将 点集合添加到多边形中
            MainMap.UpdatePolygonLocalPosition(gMapPolygon);//更新多边形的位置
            gMapOverlay.Markers.Add(landWP);//将点添加到第二层
            gMapOverlay.Polygons.Add(gMapPolygon);//将多边形添加到第二层
            if (MainMap.Overlays.Count == 0)//如果点击了删除全部航点的时候 那么Overlay=0了 再也添加不上点了
            {
                MainMap.Overlays.Add(gMapOverlay);//将第二层添加到第三层
            }
            this.MainMap.Position = this.MainMap.Position;
            MainMap.Refresh();
            pointWriteDGV(MavLink.MAV_CMD.LAND, new PointLatLngAlt(landWP.Position.Lng, landWP.Position.Lat, Global.landAlt));

            //DoCommand(MavLink.MAV_CMD.LAND, 0, 0, 0, 0, (float)landWP.Position.Lat, (float)landWP.Position.Lng, (float)Global.landAlt);//降落语句
        }
    
        /// <summary>
        /// 右键菜单返航
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void goHomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!beforeSendCommand())
            {
                return;
            }
            else if (MessageBox.Show("你想要回到起飞点?", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                //DoCommand(MavLink.MAV_CMD.RETURN_TO_LAUNCH, 0, 0, 1, 0, 0, 0, 0);//这个是param3 = 1 源码上发现的 需要验证
                pointWriteDGV(MavLink.MAV_CMD.RETURN_TO_LAUNCH, new PointLatLngAlt(mavStatus.gps_Row.lat, mavStatus.gps_Row.lng ,mavStatus.gps_Row.alt));

            }
            
        }
        #endregion

        #region 瓦片加载相关事件
        private void MainMap_OnTileLoadStart()
        {
            MethodInvoker m = delegate()
            {
                //label60.Text = "状态: loading tiles...";
            };
            try
            {
                BeginInvoke(m);
            }
            catch
            {
            }
        }
        private void MainMap_OnTileLoadComplete(long ElapsedMilliseconds)
        {
            MainMap.ElapsedMilliseconds = ElapsedMilliseconds;

            MethodInvoker m = delegate()
            {
                //label61.Text = "Menu, last load in " + MainMap.ElapsedMilliseconds + "ms";

                //label62.Text = string.Format(CultureInfo.InvariantCulture, "{0:0.00} MB of {1:0.00} MB", MainMap.Manager.MemoryCache.Size, MainMap.Manager.MemoryCache.Capacity);
            };
            try
            {
                BeginInvoke(m);
            }
            catch
            {
            }
        }
        private void OnTileCacheStart()
        {
            if (!IsDisposed)
            {
                MethodInvoker m = delegate
                {
                    //label63.Text = "saving tiles...";
                };
                Invoke(m);
            }
        }
        private void OnTileCacheProgress(int left)
        {
            if (!IsDisposed)
            {
                MethodInvoker m = delegate
                {
                    //label64.Text = left + " tile to save...";
                };
                Invoke(m);
            }
        }
        private void OnTileCacheComplete()
        {
            Debug.WriteLine("地图缓冲完成！");
            long size = 0;
            int db = 0;
            try
            {
                DirectoryInfo di = new DirectoryInfo(MainMap.CacheLocation);
                var dbs = di.GetFiles("*.gmdb", SearchOption.AllDirectories);
                foreach (var d in dbs)
                {
                    size += d.Length;
                    db++;
                }
            }
            catch
            {
            }

            if (!IsDisposed)
            {
                MethodInvoker m = delegate
                {
                    //label65.Text = string.Format(CultureInfo.InvariantCulture, "{0} db in {1:00} MB", db, size / (1024.0 * 1024.0));
                    //label66.Text = "all tiles saved!";
                };

                if (!IsDisposed)
                {
                    try
                    {
                        Invoke(m);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        #endregion

        bool isSuo = true;

        #region 接受和解析数据
        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            bufferNum = sp.BytesToRead;
            byte[] buffer = new byte[bufferNum];
            //读取数据缓冲区的所有数据
            sp.Read(buffer, 0, bufferNum);
            mavDataList.AddRange(buffer);
            //for (int i = 0; i < mavDataList.Count; i++)
            //{
            //    sw2.Write(string.Format("{0,-4}", mavDataList[i].ToString() + " "));
            //}
            while (mavDataList.Count > 2)//
            {
                //第一个是254 且 长度够一个数据帧 (不一定是一个正确帧)
                if (mavDataList[0] == 254 && mavDataList.Count >= 8 + mavDataList[1])
                {
                    byte dataLength = mavDataList[1];
                    mavLinkFrame = new byte[8 + dataLength];
                    mavLinkFrame = mavDataList.GetRange(0, 8 + dataLength).ToArray();
                    //dataFrame = new MavLink.MavLinkFrame(mavLinkFrame);
                    dataFrame_2 = new MavLink.MavLinkFrame_2(mavLinkFrame);
                    ushort checksum = MavlinkCRC.crc_calculate(Helper.StructToByteArray_2(dataFrame_2), 6 + dataLength);
                    checksum = MavlinkCRC.crc_accumulate(MavlinkCRC.MAVLINK_MESSAGE_CRCS[mavLinkFrame[5]], checksum);
                    byte ck_a = (byte)(checksum & 0xFF);
                    byte ck_b = (byte)(checksum >> 8);
                    //254是数据部分的254  不是头
                    if (mavLinkFrame[mavLinkFrame.Length - 2] != ck_a || mavLinkFrame[mavLinkFrame.Length - 1] != ck_b)
                    {
                        mavDataList.RemoveAt(0);
                        continue;
                    }
                    else//正确的帧
                    {
                        analyzeMavLinkFrame = new AnalyzeMavLinkFrame(UpdateMavStatus);
                        analyzeMavLinkFrame(mavLinkFrame);
                        //for (int i = 0; i < mavLinkFrame.Length; i++)
                        //{
                        //    sw1.Write(string.Format("{0,-4}", mavDataList[i].ToString() + " "));
                        //}
                        //sw1.WriteLine();
                        //移除这个帧
                        mavDataList.RemoveRange(0, 8 + dataLength);
                    }
                }
                //第一个是254 但 长度不够一个数据帧
                else if (mavDataList[0] == 254 && mavDataList.Count < 8 + mavDataList[1])
                {
                    //结束  继续接收数据
                    break;
                }
                //第一个不是 254
                else
                {
                    //将第一个移除
                    mavDataList.RemoveAt(0);
                    continue;
                }
            }

        }
        public void UpdateMavStatus(byte[] mavLinkFrame)
        {
            switch (mavLinkFrame[5])
            {
                case 0://心跳包
                    UpdateHeartPack(mavLinkFrame);
                    break;
                case 1://系统状态
                    UpdateSysStatus(mavLinkFrame);
                    break;
                case 2://系统时间
                    UpdateSystemTime(mavLinkFrame);
                    break;
                case 22://参数数值
                    UpdateParamValue(mavLinkFrame);
                    break;
                case 24://GPS信息
                    UpdateGPSRow(mavLinkFrame);
                    break;
                case 27://原始的姿态传感器信息
                    UpdateRawImuT(mavLinkFrame);
                    break;
                case 30://姿态信息
                    UpdateAttitude(mavLinkFrame);
                    break;
                case 33://位置信息
                    UpdateGlobalPosition(mavLinkFrame);
                    break;
                case 39://任务项
                    Mission_Iteam(mavLinkFrame);
                    break;
                case 40://任务下载请求
                    Mission_Request(mavLinkFrame);
                    break;
                case 44://任务计数
                    Mission_Count(mavLinkFrame);
                    break;
                case 47://任务回应
                    Mission_Ack(mavLinkFrame);
                    break;
                case 62://导航信息输出
                    UpdateControllerOutput(mavLinkFrame);
                    break;
                case 74://通常用在HUD显示上的各种数据
                    UpdateHUDInfo(mavLinkFrame);
                    break;
                case 77://命令应答
                    Command_ACK(mavLinkFrame);
                    break;
                case 116:
                    UpdateRawImu2T(mavLinkFrame);
                    break;
                case 150:
                    UpdateSensoroffset(mavLinkFrame);
                    break;
                case 253://文本信息
                    UpdateStatusText(mavLinkFrame);
                    break;
            }
        }
        /// <summary>
        /// 任务计数 解析44号消息包让其mission_count.count = 1000
        /// </summary>
        MAVLink.MavLink.mavlink_Mission_Count mission_count = new MavLink.mavlink_Mission_Count();
        /// <summary>
        /// 任务下载请求 
        /// </summary>
        MavLink.mavlink_mission_request mission_request = new MavLink.mavlink_mission_request();
        /// <summary>
        /// 任务项 解析39号消息包 mission_iteam.seq=255
        /// </summary>
        MAVLink.MavLink.mavlink_Mission_Item mission_iteam = new MavLink.mavlink_Mission_Item();
        /// <summary>
        /// 命令应答 
        /// </summary>
        MavLink.mavlink_Command_Ack command_Ack = new MavLink.mavlink_Command_Ack();
        /// <summary>
        /// 任务回应 让其初始化的时候 result=255
        /// </summary>
        MavLink.mavlink_Mission_Ack mission_Ack = new MavLink.mavlink_Mission_Ack();

        #region 更新Mav状态
        private void Mission_Count(byte[] mavLinkFrame)
        {
            mission_count.count = (ushort)Helper.Turn(mavLinkFrame, 6, 7, DataType.U_Short);
            mission_count.target_system = mavLinkFrame[8];
            mission_count.target_component = mavLinkFrame[9];
           // MessageBox.Show("航点总数：" + mission_count.count.ToString());
            cmdcount = mission_count.count;//这两个是全局变量
            index = 0;
            getWP(index);//开始要第0个
            getWP(index);//要两次增加正确的机率。


        }

        bool isOK = false;
        DateTime now;
        private void Mission_Request(byte[] mavLinkFrame)//40号解析收到的序号
        {
            mission_request.seq = (ushort)Helper.Turn(mavLinkFrame, 6, 7, DataType.U_Short);
            mission_request.target_system = mavLinkFrame[8];
            mission_request.target_component = mavLinkFrame[9];

            List<LocationWayPoint> missionWayPointList = LatlonAltToLocationWayPoints();

            MAVLink.MavLink.MAV_FRAME frame = MAVLink.MavLink.MAV_FRAME.GLOBAL_RELATIVE_ALT;
            if (mission_request.seq == 0)
            {
                SendWP(missionWayPointList[mission_request.seq], (ushort)mission_request.seq, MAVLink.MavLink.MAV_FRAME.GLOBAL, 0);
                now = DateTime.Now;
            }
            else
            {
                SendWP(missionWayPointList[mission_request.seq], (ushort)mission_request.seq, frame, 0);

                if (mission_request.seq == missionWayPointList.Count - 1)
                {
                    isOK = true;
                    MessageBox.Show("写入成功！");
                    timer1.Stop();
                }
                else if (mission_request.seq < missionWayPointList.Count - 1)
                {
                    now = DateTime.Now;
                }
            }

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!isOK)
            {
                if (now.AddMilliseconds(500) < DateTime.Now)
                {
                    //MessageBox.Show("超时!");
                }
            }
        }
        private void Mission_Iteam(byte[] mavLinkFrame)
        {
            mission_iteam.param1 = (float)Helper.Turn(mavLinkFrame, 6, 9, DataType.Float);
            mission_iteam.param2 = (float)Helper.Turn(mavLinkFrame, 10, 13, DataType.Float);
            mission_iteam.param3 = (float)Helper.Turn(mavLinkFrame, 14, 17, DataType.Float);
            mission_iteam.param4 = (float)Helper.Turn(mavLinkFrame, 18, 21, DataType.Float);
            mission_iteam.x = (float)Helper.Turn(mavLinkFrame, 22, 25, DataType.Float);
            mission_iteam.y = (float)Helper.Turn(mavLinkFrame, 26, 29, DataType.Float);
            mission_iteam.z = (float)Helper.Turn(mavLinkFrame, 30, 33, DataType.Float);
            mission_iteam.seq = (ushort)Helper.Turn(mavLinkFrame, 34, 35, DataType.U_Short);
            mission_iteam.command = (ushort)Helper.Turn(mavLinkFrame, 36, 37, DataType.U_Short);
            mission_iteam.target_system = mavLinkFrame[38];
            mission_iteam.target_component = mavLinkFrame[39];
            mission_iteam.frame = mavLinkFrame[40];
            mission_iteam.current = mavLinkFrame[41];
            mission_iteam.autocontinue = mavLinkFrame[42];

            

            if (index == mission_iteam.seq)
            {
               // MessageBox.Show(mission_iteam.seq.ToString() + "成功接收到了。");

                LocationWayPoint wps = new LocationWayPoint();
                wps.p1 = mission_iteam.param1;
                wps.p2 = mission_iteam.param2;
                wps.p3 = mission_iteam.param3;
                wps.p4 = mission_iteam.param4;
                wps.id = (byte)mission_iteam.command;
                wps.lat = mission_iteam.x;
                wps.lng = mission_iteam.y;
                wps.alt = mission_iteam.z;
                wps.options = (byte)(mission_iteam.frame);
                cmdlist.Add(wps);
                if (cmdcount > cmdlist.Count)
                {
                    index++;
                  //  MessageBox.Show("要第" + index.ToString() + "航点");
                    getWP((ushort)(index));
                }
                else
                {
                    index = 0;
                    mission_iteam.seq = 0;
                    wpslisttopointlist(cmdlist,1);
                    drawDgv(cmdlist,1);
                    //this.Invoke(new Func<pointlatlngaltList, bool>(precsslatlngalt));
                    precsslatlngalt(pointlatlngaltList);

                }
                //getWP(index + 1);
            }






        }
        private void UpdateHUDInfo(byte[] mavLinkFrame)
        {
            mavStatus.Vfr_Hud.airspeed = (float)Helper.Turn(mavLinkFrame, 6, 9, DataType.Float);
            mavStatus.Vfr_Hud.groundspeed = (float)Helper.Turn(mavLinkFrame, 10, 13, DataType.Float);
            mavStatus.Vfr_Hud.alt = (float)Helper.Turn(mavLinkFrame, 14,17, DataType.Float);
            mavStatus.Vfr_Hud.climb = (float)Helper.Turn(mavLinkFrame,18, 21, DataType.Float);
            mavStatus.Vfr_Hud.heading = (short)Helper.Turn(mavLinkFrame,22, 23, DataType.S_Short);
            mavStatus.Vfr_Hud.throttle = (ushort)Helper.Turn(mavLinkFrame,24,25, DataType.U_Short);
           
        
            try
            {
                this.Invoke(new Action(() =>
                {
                    this.hud.alt = mavStatus.Vfr_Hud.alt;
                    this.labAlt.Text = mavStatus.Vfr_Hud.alt.ToString()+"m";
                    this.txtAlt.Text = mavStatus.Vfr_Hud.alt.ToString() + "m";
                    this.labSpeed.Text = mavStatus.Vfr_Hud.groundspeed.ToString()+"m/s";
                    this.txtSpeed.Text = mavStatus.Vfr_Hud.groundspeed.ToString() + "m/s";
                    this.labShangShenSpeed.Text = mavStatus.Vfr_Hud.climb.ToString("f2")+"m/s";
                    this.txtUpSpeed.Text = mavStatus.Vfr_Hud.climb.ToString("f2") + "m/s";

                    this.hud.speed = mavStatus.Vfr_Hud.groundspeed;
                    this.hud.Refresh();
                    this.hud.Invalidate();
                }
                ));
            }
            catch
            {
            }
        }
        private void Mission_Ack(byte[] mavLinkFrame)
        {
            mission_Ack.target_system = mavLinkFrame[6];
            mission_Ack.target_component = mavLinkFrame[7];
            mission_Ack.type = mavLinkFrame[8];

        }
        private void Command_ACK(byte[] mavLinkFrame)
        {
            command_Ack.command = (ushort)Helper.Turn(mavLinkFrame, 6, 7, DataType.U_Short);
            command_Ack.result = mavLinkFrame[8];
        }
        private void UpdateParamValue(byte[] mavLinkFrame)
        {
            mavStatus.param_Value.param_value = (float)Helper.Turn(mavLinkFrame, 6, 9, DataType.Float);
            mavStatus.param_Value.param_count = (ushort)Helper.Turn(mavLinkFrame, 10, 11, DataType.U_Short);
            mavStatus.param_Value.param_index = (ushort)Helper.Turn(mavLinkFrame, 12, 13, DataType.U_Short);
            mavStatus.param_Value.param_id = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                mavStatus.param_Value.param_id[i] = mavLinkFrame[i + 14];
            }
            mavStatus.param_Value.param_type = mavLinkFrame[30];



            string paramID = System.Text.ASCIIEncoding.ASCII.GetString(mavStatus.param_Value.param_id);
            int pos = paramID.IndexOf('\0');
            if (pos != -1)
            {
                paramID = paramID.Substring(0, pos);
            }
            if (!mavStatus.paraDic.ContainsKey(paramID))//如果不包含 就将数据添加进去
            {
                mavStatus.paraDic.Add(paramID, mavStatus.param_Value.param_value);
            }
            //如果包含 但是值被更新了，就更新paramFrameDic中的值，保持最新。
            if (mavStatus.paraDic.ContainsKey(paramID) && mavStatus.paraDic[paramID] != mavStatus.param_Value.param_value)
            {
                mavStatus.paraDic[paramID] = mavStatus.param_Value.param_value;
            }
        }
        private void UpdateStatusText(byte[] mavLinkFrame)//通过这个得到飞控的详细信息
        {
            mavStatus.status_Text.severity = mavLinkFrame[6];
            mavStatus.status_Text.text = mavLinkFrame.Skip(7).ToArray();
            this.Invoke(new Action(() =>
                {
                    this.NewtxtContent.AppendText(mavStatus.status_Text.severity.ToString() + " " + Encoding.ASCII.GetString(mavStatus.status_Text.text));
                    this.NewtxtContent.AppendText("\r\n");

                    if (configAccelerometerCalibration != null)
                    {
                        configAccelerometerCalibration.setCalibrationAccelInfo(Encoding.ASCII.GetString(mavStatus.status_Text.text));
                    }
                }
                ));
        }
         private void UpdateSensoroffset(byte[] mavLinkFrame)//传感器的偏移量
        {
            mavStatus.sensor_offset.mag_declination = (float)Helper.Turn(mavLinkFrame, 6, 9, DataType.Float);
            mavStatus.sensor_offset.raw_press = (int)Helper.Turn(mavLinkFrame, 10, 13, DataType.S_Int);
            mavStatus.sensor_offset.raw_temp = (int)Helper.Turn(mavLinkFrame, 14, 17, DataType.S_Int);
            mavStatus.sensor_offset.gyro_cal_x = (float)Helper.Turn(mavLinkFrame, 18, 21, DataType.Float);
            mavStatus.sensor_offset.gyro_cal_y = (float)Helper.Turn(mavLinkFrame, 22, 25, DataType.Float);
            mavStatus.sensor_offset.gyro_cal_z = (float)Helper.Turn(mavLinkFrame, 26, 29, DataType.Float);
            mavStatus.sensor_offset.accel_cal_x = (float)Helper.Turn(mavLinkFrame, 30, 33, DataType.Float);
            mavStatus.sensor_offset.accel_cal_y = (float)Helper.Turn(mavLinkFrame, 34, 37, DataType.Float);
            mavStatus.sensor_offset.accel_cal_z = (float)Helper.Turn(mavLinkFrame, 38, 41, DataType.Float);
            mavStatus.sensor_offset.mag_ofs_x = (short)Helper.Turn(mavLinkFrame, 42, 43, DataType.S_Short);
            mavStatus.sensor_offset.mag_ofs_y = (short)Helper.Turn(mavLinkFrame, 44, 45, DataType.S_Short);
            mavStatus.sensor_offset.mag_ofs_z = (short)Helper.Turn(mavLinkFrame, 46, 47, DataType.S_Short);
            try
            {
            //    this.Invoke(new Action(() => 
            //        {
            //            this.Accel_X.Text = mavStatus.sensor_offset.accel_cal_x.ToString();
            //            this.Accel_Y.Text = mavStatus.sensor_offset.accel_cal_y.ToString();
            //            this.Accel_Z.Text = mavStatus.sensor_offset.accel_cal_z.ToString();
            //        }));
            }
            catch
            { }
           
        }

        private void UpdateControllerOutput(byte[] mavLinkFrame)
        {
            mavStatus.nav_Controller_Output.nav_roll = (float)Helper.Turn(mavLinkFrame, 6, 9, DataType.Float);
            mavStatus.nav_Controller_Output.nav_pitch = (float)Helper.Turn(mavLinkFrame, 10, 13, DataType.Float);

            mavStatus.nav_Controller_Output.alt_error = (float)Helper.Turn(mavLinkFrame, 14, 17, DataType.Float);
            mavStatus.nav_Controller_Output.aspd_error = (float)Helper.Turn(mavLinkFrame, 18, 21, DataType.Float);
            mavStatus.nav_Controller_Output.xtrack_error = (float)Helper.Turn(mavLinkFrame, 22, 25, DataType.Float);

            mavStatus.nav_Controller_Output.nav_bearing = (short)Helper.Turn(mavLinkFrame, 26, 27, DataType.S_Short);
            mavStatus.nav_Controller_Output.target_bearing = (short)Helper.Turn(mavLinkFrame, 28, 29, DataType.S_Short);
            mavStatus.nav_Controller_Output.wp_dist = (ushort)Helper.Turn(mavLinkFrame, 30, 31, DataType.U_Short);
            //try
            //{
            //    this.Invoke(new Action(() => this.labLastdistance.Text = mavStatus.nav_Controller_Output.wp_dist.ToString()));
            //}
            //catch
            //{ }
            //mavStatus.nav_Controller_Output.alt_error = (float)Helper.Turn(mavLinkFrame, 20, 23, DataType.Float);
            //mavStatus.nav_Controller_Output.aspd_error = (float)Helper.Turn(mavLinkFrame, 24, 27, DataType.Float);
            //mavStatus.nav_Controller_Output.xtrack_error = (float)Helper.Turn(mavLinkFrame, 28, 31, DataType.Float);
        }

        private void UpdateGlobalPosition(byte[] mavLinkFrame)
        {
            mavStatus.global_Position.time_boot_ms = (UInt32)Helper.Turn(mavLinkFrame, 6, 9, DataType.U_Int);
            mavStatus.global_Position.lat = (Int32)Helper.Turn(mavLinkFrame, 10, 13, DataType.S_Int);
            mavStatus.global_Position.lon = (int)Helper.Turn(mavLinkFrame, 14, 17, DataType.S_Int);
            mavStatus.global_Position.alt = (int)Helper.Turn(mavLinkFrame, 18, 21, DataType.S_Int);
            mavStatus.global_Position.relative_alt = (int)Helper.Turn(mavLinkFrame, 22, 25, DataType.S_Int);
            mavStatus.global_Position.vx = (short)Helper.Turn(mavLinkFrame, 26, 27, DataType.S_Short);
            mavStatus.global_Position.vy = (short)Helper.Turn(mavLinkFrame, 28, 29, DataType.S_Short);
            mavStatus.global_Position.vz = (short)Helper.Turn(mavLinkFrame, 30, 31, DataType.S_Short);
            mavStatus.global_Position.hdg = (ushort)Helper.Turn(mavLinkFrame, 32, 33, DataType.U_Short);
        }
        bool isOpen = true;//第一次打开 


        private void UpdateGPSRow(byte[] mavLinkFrame)
        {
            //pointsList.Add(new PointLatLng(mavStatus.gps_Row.lat / Math.Pow(10, 7), mavStatus.gps_Row.lng / Math.Pow(10, 7)));
            mavStatus.gps_Row.time_usec = (UInt64)Helper.Turn(mavLinkFrame, 6, 13, DataType.U_Int64);
            //mavStatus.gps_Row.fix_type = mavLinkFrame[14];
            mavStatus.gps_Row.lat = (Int32)Helper.Turn(mavLinkFrame, 14, 17, DataType.S_Int);
            mavStatus.gps_Row.lng = (int)Helper.Turn(mavLinkFrame, 18, 21, DataType.S_Int);
            mavStatus.gps_Row.alt = (int)Helper.Turn(mavLinkFrame, 22, 25, DataType.S_Int);
            mavStatus.gps_Row.eph = (ushort)Helper.Turn(mavLinkFrame, 26, 27, DataType.U_Short);
            mavStatus.gps_Row.epv = (ushort)Helper.Turn(mavLinkFrame, 28, 29, DataType.U_Short);
            mavStatus.gps_Row.vel = (ushort)Helper.Turn(mavLinkFrame, 30, 31, DataType.U_Short);
            mavStatus.gps_Row.cog = (ushort)Helper.Turn(mavLinkFrame, 32, 33, DataType.U_Short);

            mavStatus.gps_Row.fix_type = mavLinkFrame[34];
            mavStatus.gps_Row.satellites_visible = mavLinkFrame[35];
            try
            {
                this.Invoke(new Action(() =>
                {
                    labNum.Text = mavStatus.gps_Row.satellites_visible.ToString();
                    labJingDu.Text = mavStatus.gps_Row.eph.ToString();
                    
                }));
            }
            catch { };
            if (isOpen)
            {

                Global.nowLocation = new PointLatLng(mavStatus.gps_Row.lat / Math.Pow(10, 7), mavStatus.gps_Row.lng / Math.Pow(10, 7));
                this.Invoke(new Action(() =>
                {
                    MainMap.Position = Global.nowLocation;
                    MainMap.Zoom = 14;
                    pointsList.Add(new PointLatLng(mavStatus.gps_Row.lat / Math.Pow(10, 7), mavStatus.gps_Row.lng / Math.Pow(10, 7)));
                    DrawRouite(new PointLatLngAlt(mavStatus.gps_Row.lat / Math.Pow(10, 7), mavStatus.gps_Row.lng / Math.Pow(10, 7), mavStatus.gps_Row.alt / Math.Pow(10, 7)));//绘制行走轨迹
                    
                }));

                isOpen = false;
            }
        }

        private void UpdateRawImuT(byte[] mavLinkFrame)
        {
            mavStatus.raw_imu_t.time_usec = (UInt64)Helper.Turn(mavLinkFrame, 6, 13, DataType.U_Int64);
            mavStatus.raw_imu_t.xacc = (Int16)Helper.Turn(mavLinkFrame, 14, 15, DataType.S_Short);
            mavStatus.raw_imu_t.yacc = (Int16)Helper.Turn(mavLinkFrame, 16, 17, DataType.S_Short);
            mavStatus.raw_imu_t.zacc = (Int16)Helper.Turn(mavLinkFrame, 18, 19, DataType.S_Short);
            mavStatus.raw_imu_t.xgyro = (Int16)Helper.Turn(mavLinkFrame, 20, 21, DataType.S_Short);
            mavStatus.raw_imu_t.ygyro = (Int16)Helper.Turn(mavLinkFrame, 22, 23, DataType.S_Short);
            mavStatus.raw_imu_t.zgyro = (Int16)Helper.Turn(mavLinkFrame, 24, 25, DataType.S_Short);
            mavStatus.raw_imu_t.xmag = (Int16)Helper.Turn(mavLinkFrame, 26, 27, DataType.S_Short);
            mavStatus.raw_imu_t.ymag = (Int16)Helper.Turn(mavLinkFrame, 28, 29, DataType.S_Short);
            mavStatus.raw_imu_t.zmag = (Int16)Helper.Turn(mavLinkFrame, 30, 31, DataType.S_Short);

            float rawmx = mavStatus.raw_imu_t.xmag - (float)mavStatus.sensor_offset.mag_ofs_x;
            float rawmy = mavStatus.raw_imu_t.ymag - (float)mavStatus.sensor_offset.mag_ofs_y;
            float rawmz = mavStatus.raw_imu_t.zmag - (float)mavStatus.sensor_offset.mag_ofs_z;


            if (SanHeGroundStation.Forms.ProgressReporterSphereUsing.MagCalib.boostart)
            {
                SanHeGroundStation.Forms.ProgressReporterSphereUsing.MagCalib.comdatacompass1 = new Tuple<float, float, float>(rawmx, rawmy, rawmz);

            }


        }

         private void UpdateRawImu2T(byte[] mavLinkFrame)
        {
            

            mavStatus.raw_imu2_t.time_usec = (UInt32)Helper.Turn(mavLinkFrame, 6, 9, DataType.U_Int);
            mavStatus.raw_imu2_t.xacc = (Int16)Helper.Turn(mavLinkFrame, 10, 11, DataType.S_Short);
            mavStatus.raw_imu2_t.yacc = (Int16)Helper.Turn(mavLinkFrame, 12, 13, DataType.S_Short);
            mavStatus.raw_imu2_t.zacc = (Int16)Helper.Turn(mavLinkFrame, 14, 15, DataType.S_Short);
            mavStatus.raw_imu2_t.xgyro = (Int16)Helper.Turn(mavLinkFrame, 16, 17, DataType.S_Short);
            mavStatus.raw_imu2_t.ygyro = (Int16)Helper.Turn(mavLinkFrame, 18, 19, DataType.S_Short);
            mavStatus.raw_imu2_t.zgyro = (Int16)Helper.Turn(mavLinkFrame, 20, 21, DataType.S_Short);
            mavStatus.raw_imu2_t.xmag = (Int16)Helper.Turn(mavLinkFrame, 22, 23, DataType.S_Short);
            mavStatus.raw_imu2_t.ymag = (Int16)Helper.Turn(mavLinkFrame, 24, 25, DataType.S_Short);
            mavStatus.raw_imu2_t.zmag = (Int16)Helper.Turn(mavLinkFrame, 26, 27, DataType.S_Short);

            float rawmx = mavStatus.raw_imu2_t.xmag - (float)mavStatus.sensor_offset.mag_ofs_x;
            float rawmy = mavStatus.raw_imu2_t.ymag - (float)mavStatus.sensor_offset.mag_ofs_y;
            float rawmz = mavStatus.raw_imu2_t.zmag - (float)mavStatus.sensor_offset.mag_ofs_z;

            if (SanHeGroundStation.Forms.ProgressReporterSphereUsing.MagCalib.boostart)
            {
                SanHeGroundStation.Forms.ProgressReporterSphereUsing.MagCalib.comdatacompass2 = new Tuple<float, float, float>(rawmx, rawmy, rawmz);

            }
           


        }

        private void UpdateSystemTime(byte[] mavLinkFrame)
        {
            mavStatus.system_Time.time_boot_ms = (uint)Helper.Turn(mavLinkFrame, 14, 17, DataType.U_Int);
            mavStatus.system_Time.time_unix_usec = (UInt64)Helper.Turn(mavLinkFrame, 6, 13, DataType.U_Int64);
        }

        private void UpdateAttitude(byte[] mavLinkFrame)
        {
            mavStatus.attitude.time_boot_ms = (uint)Helper.Turn(mavLinkFrame, 6, 9, DataType.U_Int);
            mavStatus.attitude.roll = Helper.RadianToAngle((float)Helper.Turn(mavLinkFrame, 10, 13, DataType.Float));
            mavStatus.attitude.pitch = Helper.RadianToAngle((float)Helper.Turn(mavLinkFrame, 14, 17, DataType.Float));
            mavStatus.attitude.yaw = Helper.RadianToAngle((float)Helper.Turn(mavLinkFrame, 18, 21, DataType.Float));
            mavStatus.attitude.rollspeed = Helper.RadianToAngle((float)Helper.Turn(mavLinkFrame, 22, 25, DataType.Float));
            mavStatus.attitude.pitchspeed = Helper.RadianToAngle((float)Helper.Turn(mavLinkFrame, 26, 29, DataType.Float));
            mavStatus.attitude.yawspeed = Helper.RadianToAngle((Single)Helper.Turn(mavLinkFrame, 30, 33, DataType.Float));
            try
            {
                this.Invoke(new Action(() =>
                {
                    this.hud.roll = mavStatus.attitude.roll;
                    this.hud.pitch = mavStatus.attitude.pitch;
                    this.hud.yaw = mavStatus.attitude.yaw;

                    this.labHengGunJiao.Text = mavStatus.attitude.roll.ToString()+"°";
                    this.labFuYangJiao.Text = mavStatus.attitude.pitch.ToString() + "°";
                    this.txtFYJ.Text = mavStatus.attitude.pitch.ToString() + "°";
                    this.labHengGunJiaoSpeed.Text = mavStatus.attitude.rollspeed.ToString() + "°";
                    this.labFuYangJiaoSpeed.Text = mavStatus.attitude.pitchspeed.ToString() + "°";
                    this.txtGspeed.Text = mavStatus.attitude.pitchspeed.ToString() + "°";
                    this.hud.Refresh();
                    this.hud.Invalidate();
                }));

            }
            catch
            {
                return;
            }
        }
        private void UpdateSysStatus(byte[] mavLinkFrame)
        {

            mavStatus.sys_Status.onboard_control_sensors_present = (UInt32)(Helper.Turn(mavLinkFrame, 6, 9, DataType.U_Int));
            mavStatus.sys_Status.onboard_control_sensors_enabled = (UInt32)(Helper.Turn(mavLinkFrame, 10, 13, DataType.U_Int));
            mavStatus.sys_Status.onboard_control_sensors_health = (UInt32)(Helper.Turn(mavLinkFrame, 14, 17, DataType.U_Int));
            mavStatus.sys_Status.load = (ushort)(Helper.Turn(mavLinkFrame, 18, 19, DataType.U_Short));
            mavStatus.sys_Status.voltage_battery = (ushort)(Helper.Turn(mavLinkFrame, 20, 21, DataType.U_Short));
            //mavStatus.sys_Status.voltage_battery = (ushort)(Helper.Turn(mavLinkFrame, 20, 21, DataType.U_Short));
            mavStatus.sys_Status.current_battery = (short)(Helper.Turn(mavLinkFrame, 22, 23, DataType.S_Short));
         
          
            mavStatus.sys_Status.drop_rate_comm = (ushort)(Helper.Turn(mavLinkFrame, 24, 25, DataType.U_Short));
            mavStatus.sys_Status.errors_comm = (ushort)(Helper.Turn(mavLinkFrame, 26, 27, DataType.U_Short));
            mavStatus.sys_Status.errors_count1 = (ushort)(Helper.Turn(mavLinkFrame, 28, 29, DataType.U_Short));
            mavStatus.sys_Status.errors_count2 = (ushort)(Helper.Turn(mavLinkFrame, 30, 31, DataType.U_Short));
            mavStatus.sys_Status.errors_count3 = (ushort)(Helper.Turn(mavLinkFrame, 32, 33, DataType.U_Short));
            mavStatus.sys_Status.errors_count4 = (ushort)(Helper.Turn(mavLinkFrame, 34, 35, DataType.U_Short));
            mavStatus.sys_Status.battery_remaining = mavLinkFrame[36];
           
            this.Invoke(new Action(() =>
              {
                  //丢包率
                  this.labLostLv.Text = mavStatus.sys_Status.drop_rate_comm.ToString();
                  this.txtThrowPackage.Text = mavStatus.sys_Status.drop_rate_comm.ToString();
                  //电池电压
                  this.labDianYa.Text = mavStatus.sys_Status.voltage_battery.ToString();
                  //电池余量
                  this.labYuLiang.Text = mavStatus.sys_Status.battery_remaining.ToString();
                  //丢包计数
                  this.labLostNum.Text = mavStatus.sys_Status.errors_comm.ToString();
              }));
        }
        private void UpdateHeartPack(byte[] mavLinkFrame)
        {
            if (Helper.IsEqual(mavLinkFrame, oldHeartPack))
            {
                return;
            }
            else
            {
                oldHeartPack = mavLinkFrame;
                mavStatus.heart_Pack.custom_mode = (uint)(Helper.Turn(mavLinkFrame, 6, 9, DataType.U_Int));
                mavStatus.heart_Pack.type = mavLinkFrame[10];
                mavStatus.heart_Pack.autopilot = mavLinkFrame[11];
                mavStatus.heart_Pack.base_mode = mavLinkFrame[12];
                mavStatus.heart_Pack.system_status = mavLinkFrame[13];
                mavStatus.heart_Pack.mavlink_version = mavLinkFrame[14];
                Global.sysID = mavLinkFrame[3];
                Global.compID = mavLinkFrame[4];//在这里获得的
                if ((mavStatus.heart_Pack.base_mode & 0x80) == 0)
                {
                    isSuo = true;
                    try
                    {
                        this.Invoke(new Action(() => this.labSuo.Text = "上锁"));
                    }
                    catch { }
                   
                }
                else
                {
                    isSuo = false;
                    try
                    {
                        this.Invoke(new Action(() => this.labSuo.Text = "解锁"));
                    }
                    catch { }
                   
                }
                try
                {
                    switch (mavStatus.heart_Pack.custom_mode)
                    {
                        case 0:
                            this.Invoke(new Action(() => this.labState.Text = "自稳"));
                            this.Invoke(new Action(() => this.hud.mode = "自稳"));
                            break;
                        case 2:
                            this.Invoke(new Action(() => this.labState.Text = "定高"));
                            this.Invoke(new Action(() => this.hud.mode = "定高"));
                            break;
                        case 3:
                            this.Invoke(new Action(() => this.labState.Text = "任务/自动"));
                            this.Invoke(new Action(() => this.hud.mode = "任务/自动"));
                            break;
                        case 4:
                            this.Invoke(new Action(() => this.labState.Text = "引导"));
                            this.Invoke(new Action(() => this.hud.mode = "引导"));
                            break;
                        case 5:
                            this.Invoke(new Action(() => this.labState.Text = "停留"));
                            this.Invoke(new Action(() => this.hud.mode = "停留"));
                            break;
                        case 6:
                            this.Invoke(new Action(() => this.labState.Text = "返航"));
                            this.Invoke(new Action(() => this.hud.mode = "返航"));
                            break;
                        case 7:
                            this.Invoke(new Action(() => this.labState.Text = "环绕"));
                            this.Invoke(new Action(() => this.hud.mode = "环绕"));
                            break;
                        case 9:
                            this.Invoke(new Action(() => this.labState.Text = "降落"));
                            this.Invoke(new Action(() => this.hud.mode = "降落"));
                            break;
                        case 11:
                            this.Invoke(new Action(() => this.labState.Text = "漂移"));
                            this.Invoke(new Action(() => this.hud.mode = "漂移"));
                            break;
                        case 13:
                            this.Invoke(new Action(() => this.labState.Text = "运动"));
                            this.Invoke(new Action(() => this.hud.mode = "运动"));
                            break;
                        case 16:
                            this.Invoke(new Action(() => this.labState.Text = "定点"));
                            this.Invoke(new Action(() => this.hud.mode = "定点"));
                            break;
                    }
                }
                catch { }
            }
        }
        #endregion
        #endregion

        //初始化串口
        public bool InisPort(string name, int baudRate)
        {
            try
            {
                Global.serialPort = sp;
                sp.DataBits = 8;
                sp.DiscardNull = false;
                sp.DtrEnable = false;
                sp.ReceivedBytesThreshold = 1;
                sp.RtsEnable = false;
                sp.StopBits = StopBits.One;
                if (sp.IsOpen)
                {
                    sp.Close();
                }
                sp.WriteBufferSize = 2048;
                sp.BaudRate = baudRate;
                sp.PortName = name;
                sp.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
                sp.Open();
            }
            catch(Exception ex)
            {
                Global.isConn = false;
                Global.serialPort = null;

                MessageBox.Show("错误信息："+ex.Message);
               //MessageBox.Show("串口初始化失败，请重试!");
                //connForm.Close();
                return false;
            }
            return true;
        }
        //将经纬度高度添加在一起
        private void SetPointLatLngAlt(PointLatLng point, double alt)
        {
            pointlatlngalt = new PointLatLngAlt(point, alt);
            pointlatlngaltList.Add(pointlatlngalt);
        }
        //设置航线距离的方法
        private void setDistance(List<PointLatLng> pointList1)
        {
            double d = 0;

            for (int i = 0; i < pointList1.Count; i++)
            {
                if (pointList1.Count != i + 1)
                {
                    //在Helper里的方法
                    d += Helper.GetDistance(pointList1[i].Lat, pointList1[i].Lng, pointList1[i + 1].Lat, pointList1[i + 1].Lng);

                }


            }
            label67.Text = "总距离："/* + d.ToString() + "千米。"*/;
            label27.Text = d.ToString() + "千米";
            this.txtLength.Text = d.ToString() + "千米";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.CmbChuanKou.DataSource = SerialPort.GetPortNames();//获取串口
            int[] baudRates = { 1200, 2400, 4800, 9600, 19200, 38400, 57600, 111100, 115200, 500000, 921600, 1500000 };
            this.CmdBoTeLiu.DataSource = baudRates;
            this.CmdBoTeLiu.Text = baudRates[8].ToString();
            this.panel1.Expand = false;
            Global.mavStatus = this.mavStatus;
            MainMap.Manager.Mode = AccessMode.ServerAndCache;//设置默认
            MainMap.MapProvider = GMapProviders.AMap;
            MainMap.MinZoom = 0;
            MainMap.MaxZoom = 18;
            MainMap.ShowCenter = false;//不显示十字点
            MainMap.DragButton = MouseButtons.Left;//左键拖动地图
            MainMap.Zoom = 16;//当前比例
            MainMap.Position = new PointLatLng(35.1938, 113.2726);//设置地图初始位置
            MainMap.CanDragMap = true;
            //配置地图事件
            MainMap.OnTileLoadStart += new TileLoadStart(MainMap_OnTileLoadStart);
            MainMap.OnTileLoadComplete += new TileLoadComplete(MainMap_OnTileLoadComplete);
            //碎片到缓存事件
            MainMap.Manager.OnTileCacheComplete += new TileCacheComplete(OnTileCacheComplete);
            MainMap.Manager.OnTileCacheStart += new TileCacheStart(OnTileCacheStart);
            MainMap.Manager.OnTileCacheProgress += new TileCacheProgress(OnTileCacheProgress);
            //声明的图层添加到地图图层上 
            MainMap.Overlays.Add(gMapOverlay);
            MainMap.Overlays.Add(gMapOverlay_Rouite);
            //对轨迹做出设置
            gMapRoute = new GMapRoute(drawRouiteList, "line");
            gMapRoute.Stroke.Color = Color.Red;
            gMapRoute.Stroke.Width = 2;
            //设置地图的提供者和地图读取方式的对勾
            //BaiDuMap.Checked = true;
            ServerAndCache.Checked = true;
        }

        //窗体关闭的时候，将用户的数据写入配置文件
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Global.isConn)
            {

                Dictionary<string, string> config = new Dictionary<string, string>();
                config.Add("BaudRate", Global.baudRate.ToString());
                config.Add("PortsName", Global.portsName);
                config.Add("MapProvider", Global.mapName);
                config.Add("LastLat", MainMap.Position.Lat.ToString());
                config.Add("LastLng", MainMap.Position.Lng.ToString());
                config.Add("Zoom", MainMap.Zoom.ToString());
                config.Add("DefaultAlt", Global.defaultAlt.ToString());
                Helper.WriteDataToXml(config);

                //MessageBox.Show("请先关闭串口！谢谢.");
                //return;

            }
            else
                return;

            //if (Global.isConn)
            //{
            //    MessageBox.Show("请先关闭串口！谢谢.");
            //    return;
            //}
        }
        private void StartRouiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isDrawRouite = true;//化路线设置为true
        }
        private void StopRouiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isDrawRouite = false;//化路线设置为false
        }
        private void DeleteRouiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RouiteMarkList.Clear();//标记清空
            drawRouiteList.Clear();//路线清空
            gMapOverlay_Rouite.Markers.Clear();//层清空
            gMapOverlay_Rouite.Routes.Clear();//路线层清空
        }

        /// <summary>
        /// 在发送命令之前 判断系统是否准备好
        /// </summary>
        /// <returns></returns>
        private bool beforeSendCommand()
        {
            if (!Global.isConn)
            {
                MessageBox.Show("飞行器没有连接!");
                return false;
            }
            return true;
        }
        //ConnForm connForm;

        #region 点击菜单触发
        //点击连接
        private void ConnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //connForm = new ConnForm();
            //connForm.ConnClick = new Action(Conn);//现在并没有触发委托的那个事件，只有点击的时候才触发，现在是给委托绑定事件
            //connForm.ShowDialog();
        }
        private void Conn()
        {
            if(InisPort(Global.portsName, Global.baudRate))//判断初始化是否成功
            {

                bufferNum = 0;
                DateTime now = DateTime.Now;
                while (true)
                {
                    if (bufferNum > 0)
                    {

                        //connForm.label1.Text = "连接状态:连接成功！";
                        label26.Text = "已连接";
                        Global.isConn = true;
                        Application.DoEvents();
                        Thread.Sleep(1500);
                        //connForm.Close();
                        this.labIsLocked.Visible = false;
                        timer2.Interval = 1200;
                        timer2.Start();
                        break;
                    }
                    else
                    {
                        if (now.AddSeconds(3) < DateTime.Now)
                        {
                            MessageBox.Show("连接失败,请重试!", "错误", MessageBoxButtons.OK);
                            Global.isConn = false;
                            Application.DoEvents();
                            Thread.Sleep(1500);
                            break;
                        }
                    }
                }
            }
            else
            {

            }

        }
      
        //断开连接
        private void DisConnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Global.isConn)
            {
                return;
            }
            try
            {
                sp.Close();
                sp.Dispose();//注意释放  不然就只能关一次 之后不会在关闭了
                Global.isConn = false;
                bufferNum = 0;
            }
            catch
            {

            }
            
        }
        //显示飞控详细信息
        MavDetailInfoForm mavDetailInfoForm;
        private void ShowInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!beforeSendCommand())
            {
                return;
            }
            if (mavDetailInfoForm != null && mavDetailInfoForm.Created)
            {
                mavDetailInfoForm.Focus();
                return;
            }
            mavDetailInfoForm = new MavDetailInfoForm();
            mavDetailInfoForm.Show();
        }
        //确保有一个地图被选中
        private void SingleMapCheck(object sender)
        {
            BaiDuMap.Checked = false;
            BaiDuWXMap.Checked = false;
            
            GaoDeMap.Checked = false;
            GaoDeWXMap.Checked = false;
            ((ToolStripMenuItem)sender).Checked = true;
            Global.mapName = ((ToolStripMenuItem)sender).Text;
        }
        //点击选择6个地图时
        private void Map_Click(object sender, EventArgs e)
        {
            SingleMapCheck(sender);
            SelectMapProvider(Global.mapName);
        }
        //当需要改变选择的地图的时候 调用这个
        private void SelectMapProvider(string mapName)
        {
            switch (mapName)
            {
                case "百度地图":
                    MainMap.MapProvider = GMapProviders.BaiduMap;
                    break;
                case "百度卫星地图":
                    MainMap.MapProvider = GMapProviders.BaiduSateliteMap;
                    break;
                case "谷歌中国地图":
                    MainMap.MapProvider = GMapProviders.GoogleChinaMap;
                    break;
                case "谷歌中国卫星地图":
                    MainMap.MapProvider = GMapProviders.GoogleChinaSatelliteMap;
                    break;
                case "高德地图":
                    MainMap.MapProvider = GMapProviders.AMap;
                    break;
                case "高德卫星地图":
                    MainMap.MapProvider = GMapProviders.AMapStatelite;
                    break;
                default:
                    MainMap.CacheLocation = Environment.CurrentDirectory + "//GMapCache//";//缓冲位置
                    MainMap.MinZoom = 0;
                    MainMap.MaxZoom = 18;
                    MainMap.ShowCenter = false;//不显示十字点
                    MainMap.DragButton = MouseButtons.Left;//左键拖动地图
                    MainMap.Zoom = 6;//当前比例
                    MainMap.Position = new PointLatLng (35.194,113.2726);//设置地图的中心为切换地图前的位置
                    break;
            }
        }
        //导出参数
        private void ExportParams_Click(object sender, EventArgs e)
        {
            if (!beforeSendCommand())
            {
                return;
            }
            MavLink.mavlink_Param_Request_List param_Rwquest_List;
            param_Rwquest_List = new MavLink.mavlink_Param_Request_List();
            param_Rwquest_List.target_system = Global.sysID;
            param_Rwquest_List.target_component = Global.compID;
            for (int i = 0; i < 3; i++)
            {
                AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.PARAM_REQUEST_LIST, param_Rwquest_List);
            }
            using (SaveFileDialog saveParamsDialog = new SaveFileDialog())
            {
                saveParamsDialog.Filter = "文本文件(*.txt)|*.txt";
                saveParamsDialog.RestoreDirectory = true;//保存对话框是否记忆上次打开的目录
                if (saveParamsDialog.ShowDialog() == DialogResult.OK)
                {
                    string localFilePath = saveParamsDialog.FileName.ToString();//获取文件路径
                    if (localFilePath != null)
                    {
                        using (FileStream fs = (FileStream)saveParamsDialog.OpenFile())
                        {
                            using (StreamWriter sw = new StreamWriter(fs))
                            {
                                foreach (var param in mavStatus.paraDic)
                                {
                                    sw.WriteLine(param.Key + "," + param.Value);
                                }

                            }
                        }
                        MessageBox.Show("导出完成!");
                    }
                }
            }
        }
        //设置飞行默认高度
        private void SetDefaultAlt_Click(object sender, EventArgs e)
        {
            SetDefaultAltForm setAltForm = new SetDefaultAltForm();
            setAltForm.ShowDialog();
        }
        //点击3个读取模式
        private void MapReadWay_Click(object sender, EventArgs e)
        {
            SelectMapReadWay(((ToolStripMenuItem)sender).Text);
            SingleMapReadWayCheck(sender);
        }
        //选择地图的读取方式
        private void SelectMapReadWay(string mapReadWayName)
        {
            switch (mapReadWayName)
            {
                case "只从服务器读取":
                    MainMap.Manager.Mode = AccessMode.ServerOnly;
                    break;
                case "只从缓存中读取":
                    MainMap.Manager.Mode = AccessMode.CacheOnly;
                    break;
                case "从缓存和服务器读取":
                    MainMap.Manager.Mode = AccessMode.ServerAndCache;
                    break;
                default:
                    MainMap.ReloadMap();
                    break;
            }
        }
        //读取地图方式的对勾
        private void SingleMapReadWayCheck(object sender)
        {
            CacheOnly.Checked = false;
            ServerOnly.Checked = false;
            ServerAndCache.Checked = false;
            ((ToolStripMenuItem)sender).Checked = true;
            switch (((ToolStripMenuItem)sender).Text)
            {
                case "只从服务器读取":
                    Global.mapReadWay = AccessMode.ServerOnly;
                    break;
                case "只从缓存中读取":
                    Global.mapReadWay = AccessMode.CacheOnly;
                    break;
                case "从缓存和服务器读取":
                    Global.mapReadWay = AccessMode.ServerAndCache;
                    break;
            }
        }
        GeoFence geofence;
        //设置电子围栏
        private void GeoFence_Click(object sender, EventArgs e)
        {
            if (geofence != null && geofence.Created)//只允许打开一次
            {
                geofence.Focus();
                return;
            }
            geofence = new GeoFence();
            geofence.ShowDialog();
        }
        #endregion




        public bool DoCommand(MavLink.MAV_CMD action, float p1, float p2, float p3, float p4, float p5, float p6, float p7)
        {
            MavLink.mavlink_Command_Long command = new MavLink.mavlink_Command_Long();
            command.target_component = Global.compID;
            command.target_system = Global.sysID;
            command.command = (ushort)action;
            command.param1 = p1;
            command.param2 = p2;
            command.param3 = p3;
            command.param4 = p4;
            command.param5 = p5;
            command.param6 = p6;
            command.param7 = p7;
            AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.COMMAND_LONG, command);
            DateTime now = DateTime.Now;
            int num = 5;
            while (true)
            {
                if (command_Ack.result == 255 && now.AddMilliseconds(500) > DateTime.Now)//没有超时 但是 数据帧没有来 继续接受
                {
                    Application.DoEvents();
                    continue;
                }
                else if (command_Ack.result != 255)//数据帧来了
                {
                    command_Ack.result = 255;
                    return true;
                }
                else if (now.AddMilliseconds(500) < DateTime.Now)//超时了
                {
                    if (num == 0)//5次之后 
                    {
                        return false;
                    }
                    AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.COMMAND_LONG, command);
                    num--;
                    now = DateTime.Now;
                }
            }
        }


        public void AssembleAndSendFrame(byte msgID, object structData)
        {
            if (!Global.isConn)
            {
                return;
            }
            byte[] dataPacket = MavlinkUtil.StructureToByteArray(structData);
            byte[] mavLinkFrame = new byte[dataPacket.Length + 8];
            mavLinkFrame[0] = 254;
            mavLinkFrame[1] = (byte)dataPacket.Length;
            mavLinkFrame[2] = Global.seq;
            mavLinkFrame[3] = Global.sysID;
            mavLinkFrame[4] = Global.compID;
            mavLinkFrame[5] = msgID;
            dataPacket.CopyTo(mavLinkFrame, 6);
            //算校验和
            ushort checksum = MavlinkCRC.crc_calculate(mavLinkFrame, mavLinkFrame[1] + 6);
            checksum = MavlinkCRC.crc_accumulate(MavlinkCRC.MAVLINK_MESSAGE_CRCS[msgID], checksum);
            byte ck_a = (byte)(checksum & 0xFF); ///< High byte
            byte ck_b = (byte)(checksum >> 8); ///< Low byte
            mavLinkFrame[mavLinkFrame.Length - 1] = ck_b;
            mavLinkFrame[mavLinkFrame.Length - 2] = ck_a;
            sp.Write(mavLinkFrame, 0, mavLinkFrame.Length);
            if (Global.seq > 255)
            {
                Global.seq = 0;
            }
            else
            {
                Global.seq++;
            }
        }

        public void pointWriteDGV(MavLink.MAV_CMD command, PointLatLngAlt pointLatLngAlt)
        {
            int rowIndex = this.dgvPoints.Rows.Add();
            dgvPoints.Rows[rowIndex].Cells[0].Value = command.ToString();
            //返航这个地方会报错

            dgvPoints.Rows[rowIndex].Cells[1].Value = pointlatlngalt.Lng.ToString();
            dgvPoints.Rows[rowIndex].Cells[2].Value = pointlatlngalt.Lat.ToString();
            dgvPoints.Rows[rowIndex].Cells[3].Value = pointlatlngalt.Alt.ToString();
            dgvPoints.Rows[rowIndex].Cells[4].Value = "0";
        }
        //绘制DGV 的行号
        private void dgvPoints_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var dgv = sender as DataGridView;
            if (dgv != null)
            {
                System.Drawing.Rectangle rect = new Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, dgv.RowHeadersWidth - 4, e.RowBounds.Height);
                TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), dgv.RowHeadersDefaultCellStyle.Font, rect, dgv.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
                //TextRenderer.DrawText(e.Graphics, ((SplitPage.currentPage)  SplitPage.amountPerPage + e.RowIndex + 1).ToString(), dgv.RowHeadersDefaultCellStyle.Font, rect, dgv.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter  TextFormatFlags.Right);
            }
        }
        /// <summary>
        /// 发送航点的总数 
        /// </summary>
        /// <param name="wps_total">航点总数</param>
        /// <returns>返回是否发送成功</returns>
        public bool SendWpsTotal(ushort wps_total)
        {
            MavLink.mavlink_Mission_Count req = new MavLink.mavlink_Mission_Count();
            req.target_component = Global.compID;
            req.target_system = Global.sysID;
            req.count = wps_total;
            AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.MISSION_COUNT, req);



            DateTime now = DateTime.Now;
            int num = 5;//重试的次数
            while (true)
            {
                if (mission_request.seq == 255 && now.AddMilliseconds(500) > DateTime.Now)//没有应答 且 没有超时 继续接受帧
                {
                    continue;
                }
                else if (mission_request.seq == 0)//有应答
                {
                    //mission_request.seq = 255;
                    return true;
                }
                else if (now.AddMilliseconds(500) < DateTime.Now)//超时
                {
                    if (num == 0)
                    {
                        return false;
                    }
                    AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.MISSION_COUNT, req);
                    num--;
                    now = DateTime.Now;
                }
            }

        }

        //根据经纬度赚换成LocationWayPoint集合
        private List<LocationWayPoint> LatlonAltToLocationWayPoints()
        {
            LocationWayPoint temp;
            List<LocationWayPoint> missions = new List<LocationWayPoint>();
            //添加默认的home航点，
            missions.Add(new LocationWayPoint
            {
                alt = 0,
                id = 16,
                lat = 0,
                lng = 0,
                options = 0,
                p1 = 0,
                p2 = 0,
                p3 = 0,
                p4 = 0,
            });

            for (int a = 0; a < dgvPoints.RowCount-1; a++)
            {
                temp = new LocationWayPoint();
                if (dgvPoints.Rows[a].Cells[0].Value.ToString().Contains("WAYPOINT"))
                {
                    temp.id = (byte)MavLink.MAV_CMD.WAYPOINT;
                }
                if (dgvPoints.Rows[a].Cells[0].Value.ToString().Contains("TAKEOFF"))
                {
                    temp.id = (byte)MavLink.MAV_CMD.TAKEOFF;
                }
                if (dgvPoints.Rows[a].Cells[0].Value.ToString().Contains("LAND"))
                {
                    temp.id = (byte)MavLink.MAV_CMD.LAND;
                }
                if (dgvPoints.Rows[a].Cells[0].Value.ToString().Contains("RETURN_TO_LAUNCH"))
                {
                    temp.id = (byte)MavLink.MAV_CMD.RETURN_TO_LAUNCH;
                }
               
                temp.p1 = (float)0;
                temp.p2 = (float)0;
                temp.p3 = (float)0;
                temp.p4 = (float)0;

                temp.alt = (float)(Convert.ToDouble(dgvPoints.Rows[a].Cells[3].Value.ToString()));

                temp.lat = Convert.ToDouble(dgvPoints.Rows[a].Cells[2].Value);
                temp.lng = Convert.ToDouble(dgvPoints.Rows[a].Cells[1].Value);

               
                missions.Add(temp);
            }

            return missions;
        }

        /// <summary>
        /// 发送航点列表
        /// </summary>
        /// <param name="loc">航点列表</param>
        /// <param name="index">航点序号</param>
        /// <param name="frame">消息帧坐标格式</param>
        /// <param name="current"></param>
        /// <param name="autocontinue">自动执行下移航点指令</param>
        /// <returns></returns>
        public void SendWP(LocationWayPoint loc, ushort index, MAVLink.MavLink.MAV_FRAME frame, byte current = 0, byte autocontinue = 1)
        {
            MAVLink.MavLink.mavlink_Mission_Item req = new MavLink.mavlink_Mission_Item();
            req.target_system = Global.sysID;
            req.target_component = Global.compID; // MSG_NAMES.MISSION_ITEM
            req.command = loc.id;
            req.current = current;
            req.autocontinue = autocontinue;
            req.frame = (byte)frame;
            req.y = (float)(loc.lng);
            req.x = (float)(loc.lat);
            req.z = (float)(loc.alt);
            req.param1 = loc.p1;
            req.param2 = loc.p2;
            req.param3 = loc.p3;
            req.param4 = loc.p4;
            req.seq = index;
            AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.MISSION_ITEM, req);

            //return true;
            //DateTime now = DateTime.Now;
            //while (true)
            //{
            //    //sw1.WriteLine("航点开始写入");
            //    if (mission_Ack.type == 255)//返回来的命令帧没有来
            //    {
            //        if (now.AddMilliseconds(600) > DateTime.Now)//没有超时
            //        {
            //            continue;
            //        }
            //        else//超时  重新发送命令，重新计时
            //        {
            //            sw1.WriteLine("写入超市！");
            //            AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.MISSION_ITEM, req);
            //            now = DateTime.Now;//从新计时
            //        }

            //    }
            //    else if (mission_Ack.type == (byte)MavLink.MAV_MISSION_RESULT.MAV_MISSION_ACCEPTED || mission_Ack.type == (byte)MavLink.MAV_MISSION_RESULT.MAV_MISSION_INVALID_SEQUENCE)//接受
            //    {
            //        mission_Ack.type = 255;//赋值为255  以便判断这个值有没有发过来
            //        sw1.WriteLine("航点写入成功！　　　　");
            //        return true;
            //    }
            //    else
            //    {
            //        mission_Ack.type = 255;
            //        return false;
            //    }
            //}
        }

        //航点发送完毕
        public void SendWPACK()
        {
            MAVLink.MavLink.mavlink_Mission_Ack req = new MavLink.mavlink_Mission_Ack();
            req.target_system = Global.sysID;
            req.target_component = Global.compID;
            req.type = 0;
            AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.MISSION_ACK, req);
        }

        /// <summary>
        /// 写入航点(无用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WriteWayPoints_Click(object sender, EventArgs e)
        {
            //if (!beforeSendCommand())
            //{
            //    return;
            //}
            //if (pointlatlngaltList.Count > 0)//航点集合
            //{
            //    List<LocationWayPoint> missionWayPointList = LatlonAltToLocationWayPoints();

            //    ushort index = (ushort)(dgvPoints.RowCount);
               
            //    var total = SendWpsTotal(index);//发送总航点数 这里容易出现错误 一定要确保航点的总数发送正确

            //    if (!total)
            //    {
            //        //MessageBox.Show("航点总数发送失败");
            //        return;
            //    }
            //}
            //else//没有发送的点的集合
            //{
            //    MessageBox.Show("请绘制要发送的点！");
            //    return;
            //}
            //timer1.Start();
        }


        List<LocationWayPoint> ReadWayPointsList = new List<LocationWayPoint>();

        
        List<LocationWayPoint> cmdlist = new List<LocationWayPoint>();
        ushort index;
        int cmdcount;
        /// <summary>
        /// //读出航点任务(无用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReadWayPoints_Click(object sender, EventArgs e)
        {
            //if (!beforeSendCommand())
            //{
            //    return;
            //}
            //if (pointlatlngaltList.Count > 0)
            //{
            //    if (MessageBox.Show("是否清除原来的航点坐标。", "警告", MessageBoxButtons.OKCancel) != DialogResult.OK)
            //    {
            //        return;
            //    }
            //    else
            //    {
            //        if (pointsList != null)
            //        {
            //            // 清除原来的轨迹
            //            pointsList.Clear();//经纬度集合清空
            //            setDistance(pointsList);
            //            pointlatlngaltList.Clear();//经纬度高度的集合清空
            //            gMapOverlay.Markers.Clear();
            //            gMapOverlay.Polygons.Clear();
            //            //MainMap.Overlays.Clear();//没有必要把地图上的所有层都清出
            //            wpsList.Clear();
            //            markerNum = 0;
            //            dgvPoints.Rows.Clear();
            //        }
            //    }
            //}

          
           


            //ReadWayPointsList.Clear();
            //cmdlist.Clear();
            ////GetWPCount();
            ////可能要出现错误
            //if(GetWPCount()==255)//确保一定得到或者一定没有得到
            //{
            //   // MessageBox.Show("航点总数获取失败！");
            //    return;
            //}
            ////Thread.Sleep(5000);
            ////if (mission_count.count!=cmdlist.Count)
            ////{
            ////    MessageBox.Show("航点读取失败，请再次重试。");
            ////}
            ////else
            ////{
            ////    MessageBox.Show("读取航点成功！");
            ////}
            ////cmdcount = mission_count.count;
            ////if (cmdcount == 0)
            ////{
            ////    MessageBox.Show("航点总数获取失败！");
            ////    return;
            ////}
            ////MessageBox.Show("获得到航点数:" + cmdcount.ToString());

            ////index = 0;

            ////getWP(index);//这里有问题。默认一次发送成功了

            ////getWP(index);
            ////for (ushort a = 0; a < cmdcount; a++)
            ////{
            ////    var r = getWP(a);
            ////    if (!r)
            ////    {
            ////        MessageBox.Show("航点获取失败！");
            ////        return;
            ////    }
            ////}
            ////SendWPACK();
            ////wpslisttopointlist(ReadWayPointsList);
            ////precsslatlngalt(pointlatlngaltList);
        }

        public ushort GetWPCount()
        {
            MAVLink.MavLink.mavlink_Mission_Request_List req = new MavLink.mavlink_Mission_Request_List();
            req.target_system = Global.sysID;
            req.target_component = Global.compID;
            AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.MISSION_REQUEST_LIST, req);//只发了一次
            DateTime now = DateTime.Now;
            int num2 = 5;//总共发5次
            while (true)
            {
                if (mission_count.count == 255)//返回来的命令帧没有来
                {
                    if (now.AddMilliseconds(500) > DateTime.Now)//没有超时
                    {
                        continue;
                    }
                    else//超时  重新发送命令，重新计时
                    {
                        if (num2 == 0)
                        {
                            return (ushort)mission_count.count;
                        }
                        AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.MISSION_REQUEST_LIST, req);
                        num2--;
                        now = DateTime.Now;//从新计时
                    }
                }
                else//值发生变化说明收到数据
                {
                    ushort t = mission_count.count;
                    ///MessageBox.Show("或得到航点数:"+t.ToString());
                    //mission_request.seq = 1000;
                    //mission_count.count = 255;
                    return t;
                    // MessageBox.Show("航点数量发送成功！");

                }


            }


        }

        public void getWP(ushort index)
        {
            MAVLink.MavLink.mavlink_mission_request req = new MavLink.mavlink_mission_request();
            req.target_system = Global.sysID;
            req.target_component = Global.compID;
            req.seq = index;
            AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.MISSION_REQUEST, req);
            //DateTime now = DateTime.Now;

            //int num3 = 5;//总共发5次

            //while (true)
            //{
            //    //if (mission_iteam.seq == 255)//返回来的命令帧没有来
            //    //{
            //    //    //if (now.AddMilliseconds(800) > DateTime.Now)//没有超时
            //    //    //{
            //    //    //    continue;
            //    //    //}
            //    //    //else//超时  重新发送命令，重新计时
            //    //    //{
            //    //    //    if (num3 == 0)
            //    //    //    {
            //    //    //        return false;
            //    //    //    }
            //    //    //    AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.MISSION_REQUEST, req);
            //    //    //    num3--;
            //    //    //    now = DateTime.Now;//从新计时
            //    //    //}

            //    //}
            //    //else//值发生变化说明收到数据
            //    //{
            //    //    MessageBox.Show(mission_iteam.seq.ToString());
            //    //    if (index == mission_iteam.seq)
            //    //    {
            //    //        LocationWayPoint wps = new LocationWayPoint();
            //    //        wps.p1 = mission_iteam.param1;
            //    //        wps.p2 = mission_iteam.param2;
            //    //        wps.p3 = mission_iteam.param3;
            //    //        wps.p4 = mission_iteam.param4;
            //    //        wps.id = (byte)mission_iteam.command;
            //    //        wps.lat = mission_iteam.x;
            //    //        wps.lng = mission_iteam.y;
            //    //        wps.alt = mission_iteam.z;
            //    //        wps.options = (byte)(mission_iteam.frame);
            //    //        cmdlist.Add(wps);

            //    //        mission_iteam.seq = 255;
            //    //        return true;
            //    //    }
            //    //    else
            //    //    {
            //    //        MessageBox.Show("数字不匹配！");
            //    //        return false;
            //    //    }




            //    //}


            //}

        }

        public PointLatLngAlt latlngalt;
        //把获得到的航点信息转换成经纬高集合
        public void wpslisttopointlist(List<LocationWayPoint> cmd, int check)
        {
            pointlatlngaltList.Clear();
            for (int a = check; a < cmd.Count; a++)
            {
                if(cmd[a].id==22||cmd[a].id==20)
                {
                    continue ;
                }

                latlngalt = new PointLatLngAlt(cmd[a].lat, cmd[a].lng, cmd[a].alt);
                //latlngalt.Lat = cmd[a].lat;
                //latlngalt.Lng = cmd[a].lng;
                //latlngalt.Alt = cmd[a].alt;
                pointlatlngaltList.Add(latlngalt);

            }
        }

        public void drawDgv(List<LocationWayPoint> cmdlist, int check = 0)
        {
            this.Invoke
                (new Action(() =>
                    {
                        for (int i = check; i < cmdlist.Count; i++)
                        {
                            int rowIndex = this.dgvPoints.Rows.Add();
                            if (cmdlist[i].id == 16)
                            {
                                dgvPoints.Rows[rowIndex].Cells[0].Value = MavLink.MAV_CMD.WAYPOINT.ToString();
                            }
                            if (cmdlist[i].id == 21)
                            {
                                dgvPoints.Rows[rowIndex].Cells[0].Value = MavLink.MAV_CMD.LAND.ToString();
                            }
                            if (cmdlist[i].id == 22)
                            {
                                dgvPoints.Rows[rowIndex].Cells[0].Value = MavLink.MAV_CMD.TAKEOFF.ToString();

                            }
                            if (cmdlist[i].id == 20)
                            {
                                dgvPoints.Rows[rowIndex].Cells[0].Value = MavLink.MAV_CMD.RETURN_TO_LAUNCH.ToString();
                            }
                            dgvPoints.Rows[rowIndex].Cells[1].Value = cmdlist[i].lng.ToString();
                            dgvPoints.Rows[rowIndex].Cells[2].Value = cmdlist[i].lat.ToString();
                            dgvPoints.Rows[rowIndex].Cells[3].Value = cmdlist[i].alt.ToString();
                            dgvPoints.Rows[rowIndex].Cells[4].Value = "0";

                        }
                    }));
        }







        public PointLatLng pointlatlng;
        private void precsslatlngalt(List<PointLatLngAlt> pointlatlngaltList)
        {
            this.Invoke
                (new Action(() =>
                    {
                        pointsList = new List<PointLatLng>();
                        gMapPolygon = new GMapPolygon(pointsList, "my polygon");
                        gMapPolygon.Fill = new SolidBrush(Color.FromArgb(20, Color.Blue));
                        gMapPolygon.Stroke = new Pen(Color.Green, 2);
                        gMapPolygon.IsHitTestVisible = false;
                        for (int a = 0; a < pointlatlngaltList.Count; a++)
                        {
                           
                            pointlatlng = new PointLatLng(pointlatlngaltList[a].Lat, pointlatlngaltList[a].Lng);
                            //pointlatlng.Lat = pointlatlngaltList[a].Lat;
                            //pointlatlng.Lng = pointlatlngaltList[a].Lng;
                            wp = new GMapMarkerWP(pointlatlng, (a + 1).ToString());
                            wp.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                            wp.ToolTipText = string.Format("高度:{0},经度:{1},维度:{2}", Global.defaultAlt, wp.Position.Lng, wp.Position.Lat);
                            pointsList.Add(wp.Position);//将标记点添加到 点List中
                            if (pointlatlngaltList.Count > 1)
                            {
                                setDistance(pointsList);//调用测量距离的方法
                                wpsList.Add(wp);
                                gMapPolygon.Points.Clear();
                                gMapOverlay.Polygons.Clear();
                                gMapPolygon.Points.AddRange(pointsList);//将 点集合添加到多边形中
                                MainMap.UpdatePolygonLocalPosition(gMapPolygon);//更新多边形的位置
                                gMapOverlay.Markers.Add(wp);//将点添加到第二层
                                gMapOverlay.Polygons.Add(gMapPolygon);//将多边形添加到第二层
                                if (MainMap.Overlays.Count == 0)//如果点击了删除全部航点的时候 那么Overlay=0了 再也添加不上点了
                                {
                                    MainMap.Overlays.Add(gMapOverlay);//将第二层添加到第三层
                                }
                                this.MainMap.Position = this.MainMap.Position;

                            }
                            else
                            {
                                gMapPolygon.Points.Clear();
                                gMapPolygon.Points.AddRange(pointsList);//将 点集合添加到多边形中
                                gMapOverlay.Markers.Add(wp);//将点添加到第二层
                                gMapOverlay.Polygons.Add(gMapPolygon);//将多边形添加到第二层
                                this.MainMap.Position = this.MainMap.Position;
                                MainMap.Refresh();
                            }
                            MainMap.Refresh();


                        }
                        markerNum = pointlatlngaltList.Count;
                    }));
            
        }

        private void btnSetMode_Click(object sender, EventArgs e)
        {
            if (!beforeSendCommand())
            {
                return;
            }
            MavLink.mavlink_Set_Mode mode = SetMode(cbMoShi.Text.Trim());
            AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.SET_MODE, mode);
            Thread.Sleep(10);
            AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.SET_MODE, mode);

        }

        public MavLink.mavlink_Set_Mode SetMode(string MoShiString)
        {
            MAVLink.MavLink.mavlink_Set_Mode set_Mode = new MAVLink.MavLink.mavlink_Set_Mode();
            switch (MoShiString)
            {
                case "自稳":
                    set_Mode.target_system = Global.compID;
                    set_Mode.custom_mode = (UInt32)0;
                    set_Mode.base_mode = (byte)1;
                    return set_Mode;
                case "定高":
                    set_Mode.target_system = Global.compID;
                    set_Mode.custom_mode = (UInt32)2;
                    set_Mode.base_mode = (byte)1;
                    return set_Mode;
                case "定点":
                    set_Mode.target_system = Global.compID;
                    set_Mode.custom_mode = (UInt32)16;
                    set_Mode.base_mode = (byte)1;
                    return set_Mode;
                case "任务":
                    set_Mode.target_system = Global.compID;
                    set_Mode.custom_mode = (UInt32)3;
                    set_Mode.base_mode = (byte)1;
                    return set_Mode;
                case "环绕":
                    set_Mode.target_system = Global.compID;
                    set_Mode.custom_mode = (UInt32)7;
                    set_Mode.base_mode = (byte)1;
                    return set_Mode;
                case "返航":
                    set_Mode.target_system = Global.compID;
                    set_Mode.custom_mode = (UInt32)6;
                    set_Mode.base_mode = (byte)1;
                    return set_Mode;
                case "降落":
                    set_Mode.target_system = Global.compID;
                    set_Mode.custom_mode = (UInt32)9;
                    set_Mode.base_mode = (byte)1;
                    return set_Mode;
                case "引导":
                    set_Mode.target_system = Global.compID;
                    set_Mode.custom_mode = (UInt32)4;
                    set_Mode.base_mode = (byte)1;
                    return set_Mode;
                default:
                    return set_Mode;
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            //this.hud.Height = 390;
            //this.hud.Width = 353;
        }

        private void btnSuo_Click(object sender, EventArgs e)
        {
            if (!beforeSendCommand())
            {
                return;
            }
            DoCommand(MAVLink.MavLink.MAV_CMD.COMPONENT_ARM_DISARM, isSuo ? 1 : 0, 21196, 0, 0, 0, 0, 0);
        }

        private void btnSetSpeed_Click(object sender, EventArgs e)
        {
            if (!beforeSendCommand())
            {
                return;
            }

            DoCommand(MavLink.MAV_CMD.DO_CHANGE_SPEED, 1, (float)numSetSpeed.Value, 0, 0, 0, 0, 0);
        }

        private void btnHeight_Click(object sender, EventArgs e)
        {
            if (!beforeSendCommand())
            {
                return;
            }
            LocationWayPoint gotohere = new LocationWayPoint();
            gotohere.alt = (float)numFlyAlt.Value;
            gotohere.id = (byte)MavLink.MAV_CMD.WAYPOINT; //航点的ID编号命ling
            SendWP(gotohere, 0, MavLink.MAV_FRAME.GLOBAL_RELATIVE_ALT, (byte)3);
            Thread.Sleep(500);
            SendWP(gotohere, 0, MavLink.MAV_FRAME.GLOBAL_RELATIVE_ALT, (byte)3);
        }

        private void btnFly_Click(object sender, EventArgs e)
        {
            if (!beforeSendCommand())
            {
                return;
            }
            if (!Helper.IsDouble(txtTakeOffAlt.Text.Trim()))
            {
                MessageBox.Show("请输入目标高度");
                return;
            }
            Global.takeOffAlt = Convert.ToInt32(txtTakeOffAlt.Text.Trim());
            DoCommand(MavLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, Global.takeOffAlt);
        }

        private void btnInisTuoLuoYi_Click(object sender, EventArgs e)
        {
            if (!beforeSendCommand())
            {
                return;
            }
            DoCommand(MavLink.MAV_CMD.PREFLIGHT_CALIBRATION, 1, 0, 0, 0, 0, 0, 0);
            MessageBox.Show("初始化成功！", "消息",MessageBoxButtons.OK);
        }

        private void btnInisAlt_Click(object sender, EventArgs e)
        {
            if (!beforeSendCommand())
            {
                return;
            }
            DoCommand(MavLink.MAV_CMD.PREFLIGHT_CALIBRATION, 0, 0, 1, 0, 0, 0, 0);
            MessageBox.Show("初始化成功！", "消息",MessageBoxButtons.OK);
        }

        private void btnHuiFuMission_Click(object sender, EventArgs e)
        {
            if (!beforeSendCommand())
            {
                return;
            }
            if (!Helper.IsInt(txtWPIndex.Text.Trim()))
            {
                MessageBox.Show("请输入航点序号");
                return;
            }
            MavLink.mavlink_Mission_Set_Current set_Current = new MavLink.mavlink_Mission_Set_Current();
            set_Current.target_system = Global.sysID;
            set_Current.target_component = Global.compID;
            set_Current.seq = Convert.ToUInt16(txtWPIndex.Text.Trim());
            AssembleAndSendFrame((byte)MAVLink.MavLink.MAVLINK_MSG_ID.MISSION_SET_CURRENT, set_Current);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (!beforeSendCommand())
            {
                return;
            }
            MavLink.mavlink_Mission_Set_Current set_Current = new MavLink.mavlink_Mission_Set_Current();
            set_Current.target_system = Global.sysID;
            set_Current.target_component = Global.compID;
            set_Current.seq =(ushort) 0;
            AssembleAndSendFrame((byte)MAVLink.MavLink.MAVLINK_MSG_ID.MISSION_SET_CURRENT, set_Current);
        }

        //private void 显示轨迹ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    precsslatlngalt(pointlatlngaltList);
        //}

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Global.isConn)
            {
                if (DialogResult.Yes == MessageBox.Show("是否断开连接？", "提示", MessageBoxButtons.YesNo))
                {
                    try
                        {
                            Global.serialPort.Close();
                        } 
                    catch
                    {

                    }
                    Global.isConn = false;
                   

                    Dictionary<string, string> config = new Dictionary<string, string>();
                    config.Add("BaudRate", Global.baudRate.ToString());
                    config.Add("PortsName", Global.portsName);
                    config.Add("MapProvider", Global.mapName);
                    config.Add("LastLat", MainMap.Position.Lat.ToString());
                    config.Add("LastLng", MainMap.Position.Lng.ToString());
                    config.Add("Zoom", MainMap.Zoom.ToString());
                    config.Add("DefaultAlt", Global.defaultAlt.ToString());
                    Helper.WriteDataToXml(config);
                    e.Cancel = false;

                }
                else
                {
                    e.Cancel = true;
                }
            }
         
          
        }

        
       
       

        
        byte result = 0;
        public void Accel()
        {
           
                DoCommand(MavLink.MAV_CMD.PREFLIGHT_CALIBRATION, 0, 0, 0, 0, 1, 0, 0);

           
           
         
        }
        public void Accel1()
        {

            MavLink.mavlink_Command_Ack ack = new MavLink.mavlink_Command_Ack();
            ack.command = 1;
            ack.result = result;
            AssembleAndSendFrame((byte)MAVLink.MavLink.MAVLINK_MSG_ID.COMMAND_ACK, ack);
        }

        private void hud_Load(object sender, EventArgs e)
        {

        }

        private void PositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!beforeSendCommand())
            {
                return;
            }
            double Lat = (double)(Global.mavStatus.global_Position.lat / Math.Pow(10, 7));//维度
            double Lon = Global.mavStatus.global_Position.lon / Math.Pow(10, 7);//经度
            MainMap.Position = new PointLatLng { Lat = Lat, Lng = Lon };
          
            PointLatLng point = MainMap.Position;
            GMarkerGoogle marker = new GMarkerGoogle(point,GMarkerGoogleType.red);
            //marker.Tag = "P";
          
            gMapOverlay.Markers.Add(marker);
            MainMap.Overlays.Add(gMapOverlay);
            MainMap.Refresh();
        }
        ConfigAccelerometerCalibration configAccelerometerCalibration;
        /// <summary>
        /// 加速计校准
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AccelerateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            configAccelerometerCalibration = new ConfigAccelerometerCalibration();
            configAccelerometerCalibration.btnCalibrationLevelEvent = CalibrationLevelEvent;
            configAccelerometerCalibration.btnCalibrationAccelEvent = CalibrationAccelEvent;
            configAccelerometerCalibration.ShowDialog();
            Application.DoEvents();
           
        }
        private bool CalibrationLevelEvent()
        {
            DoCommand(MavLink.MAV_CMD.PREFLIGHT_CALIBRATION, 0, 0, 0, 0, 1, 0, 0);
            return true;
        }

        private bool CalibrationAccelEvent(int result)
        {
            if (!Global.alCalibrationLevel)
            {
                Global.alCalibrationLevel = true;
                DoCommand(MavLink.MAV_CMD.PREFLIGHT_CALIBRATION, 0, 0, 0, 0, 1, 0, 0);
                return true;
            }
            MavLink.mavlink_Command_Ack req = new MavLink.mavlink_Command_Ack();
            req.command = 1;
            req.result = (byte)result;


            AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.COMMAND_ACK, req);
            return true;
        }

        private void ModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //判断是否已连接APM
            if (!beforeSendCommand())
            {
                return;
            }


           Thread.Sleep(500);
            InitForm frm = new InitForm();
            frm.ShowDialog();
        }

        private void 初始设置_Click(object sender, EventArgs e)
        {
            MavLink.mavlink_Param_Request_List param_Rwquest_List;
            param_Rwquest_List = new MavLink.mavlink_Param_Request_List();
            param_Rwquest_List.target_system = Global.sysID;
            param_Rwquest_List.target_component = Global.compID;
            AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.PARAM_REQUEST_LIST, param_Rwquest_List);
            
        }
        internal string wpfilename;
        private void SaveWpsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvPoints.RowCount == 1)
            {
                MessageBox.Show("请添加航点信息后再保存！", "错误");
                return;
            }
             using (SaveFileDialog fd = new SaveFileDialog())
             {
                 fd.Filter = "Ardupilot Mission (*.txt)|*.*";
                 fd.DefaultExt = ".txt";
                 fd.FileName = wpfilename;
                 DialogResult result = fd.ShowDialog();
                 string file = fd.FileName;
                 if (file != "")
                 {
                     try 
                     {
                         StreamWriter sw = new StreamWriter(file);
                         sw.WriteLine("QGC WPL 110");
                         try
                         {
                             //sw.WriteLine("0\t1\t0\t16\t0\t0\t0\t0\t" + double.Parse(TXT_homelat.Text).ToString("0.000000", new CultureInfo("en-US")) + "\t" + double.Parse(TXT_homelng.Text).ToString("0.000000", new CultureInfo("en-US")) + "\t" + double.Parse(TXT_homealt.Text).ToString("0.000000", new CultureInfo("en-US")) + "\t1");
                         }
                         catch 
                         {
                             sw.WriteLine("0\t1\t0\t0\t0\t0\t0\t0\t0\t0\t0\t1");
                         }
                         for (int a = 0; a < dgvPoints.Rows.Count - 1; a++)
                         {
                             byte mode = (byte)(MavLink.MAV_CMD)Enum.Parse(typeof(MavLink.MAV_CMD), dgvPoints.Rows[a].Cells[0].Value.ToString());

                             sw.Write((a + 1)); // seq
                             sw.Write("\t" + 0); // current
                           
                             sw.Write("\t" + mode);
                             sw.Write("\t" + double.Parse(dgvPoints.Rows[a].Cells[1].Value.ToString()).ToString("0.000000", new CultureInfo("en-US")));
                             sw.Write("\t" + double.Parse(dgvPoints.Rows[a].Cells[2].Value.ToString()).ToString("0.000000", new CultureInfo("en-US")));
                             sw.Write("\t" + double.Parse(dgvPoints.Rows[a].Cells[3].Value.ToString()).ToString("0.000000", new CultureInfo("en-US")));
                             sw.Write("\t" + double.Parse(dgvPoints.Rows[a].Cells[4].Value.ToString()).ToString("0.000000", new CultureInfo("en-US")));
                            
                             sw.WriteLine("");
                         }
                         sw.Close();
                         MessageBox.Show("航点信息保存成功！");
                     }
                     catch
                     {

                     }
                 }
             }
        }

       

        //当鼠标浮到"文件"上时，判断当前是否连接，
        private void WenJianToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            //if (Global.isConn)
            //{
            //    ConnToolStripMenuItem.Enabled = false;
            //    DisConnToolStripMenuItem.Enabled = true;
            //}
            //else
            //{
            //    ConnToolStripMenuItem.Enabled = true;
            //    DisConnToolStripMenuItem.Enabled = false;
            //}
        }

        private void LoadWpsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteAllToolStripMenuItem_Click(sender, e);

            using (OpenFileDialog fd = new OpenFileDialog())
            {

                fd.Filter = "Ardupilot Mission (*.txt)|*.*|Shape file|*.shp";
                DialogResult result = fd.ShowDialog();
                string file = fd.FileName;
                if (File.Exists(file))
                {
                    List<LocationWayPoint> cmdlist1 = new List<LocationWayPoint>();
                    try
                    {
                        StreamReader sr = new StreamReader(file);
                        string header = sr.ReadLine();
                        if (header == null || !header.Contains("QGC WPL"))
                        {
                            MessageBox.Show("这是个无效的航点文件！");
                            return;
                        }
                        while (!sr.EndOfStream)
                        {
                            string line = sr.ReadLine();
                            string[] items = line.Split(new[] { '\t', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                            try
                            {

                                LocationWayPoint temp = new LocationWayPoint();

                                temp.id = (byte)(int)Enum.Parse(typeof(MavLink.MAV_CMD), items[2], false);
                                temp.p1 = float.Parse(items[1], new CultureInfo("en-US"));



                                temp.alt = (float)(double.Parse(items[5], new CultureInfo("en-US")));
                                temp.lat = (double.Parse(items[4], new CultureInfo("en-US")));
                                temp.lng = (double.Parse(items[3], new CultureInfo("en-US")));


                                cmdlist1.Add(temp);



                            }
                            catch { MessageBox.Show("Line invalid\n" + line); }


                        }
                        drawDgv(cmdlist1, 0);
                        wpslisttopointlist(cmdlist1, 0);

                        precsslatlngalt(pointlatlngaltList);
                    }
                    catch
                    {

                    }

                }

            }
        }
        /// <summary>
        /// 设置home点的位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SethomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (!beforeSendCommand())
            //{
            //    return;
            //}
            SetHomeForm sethomeform = new SetHomeForm();

            sethomeform.ShowDialog();
            if (wp1 == null)
            {
                wp1 = new GMapMarkerWP(new PointLatLng((float)Global.home.Lat, (float)Global.home.Lng), "H");
            }

            wp1.Position = new PointLatLng((float)Global.home.Lat, (float)Global.home.Lng);
            wp1.ToolTipMode = MarkerTooltipMode.OnMouseOver;
            wp1.ToolTipText = string.Format("高度:{0},经度:{1},维度:{2}", (float)Global.home.Alt, wp1.Position.Lng, wp1.Position.Lat);



            gMapOverlay.Markers.Add(wp1);//将点添加到第二层
            MainMap.Overlays.Add(gMapOverlay);
            this.MainMap.Position = new PointLatLng((float)Global.home.Lat, (float)Global.home.Lng);
            MainMap.Refresh();

            DoCommand(MavLink.MAV_CMD.DO_SET_HOME, 0, 0, 0, 0, (float)Global.home.Lat, (float)Global.home.Lng, (float)Global.home.Alt);
            // Changes the home location either to the current location or a specified location. |Use current (1=use current location, 0=use specified location)| Empty| Empty| Empty| Latitude| Longitude| Altitude| 

        
        }
        /// <summary>
        /// 绘制飞控的位置信息
        /// </summary>
        /// <param name="p"></param>
        private void DrawRouite(PointLatLngAlt p)
        {
            this.Invoke
              (new Action(() =>
              {

                  mark = new GMarkerGoogle(new PointLatLng(p.Lat, p.Lng), GMarkerGoogleType.orange);//可以选择飞机图片替换
                 
                  mark.ToolTipText = string.Format("高度:{0},经度:{1},维度:{2}", p.Alt, p.Lng, p.Lat);
                  mark.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                  RouiteMarkList.Add(mark);
                  drawRouiteList.Add(mark.Position);
                  if (RouiteMarkList.Count > 1)//不是第一次点击
                  {
                      setDistance(drawRouiteList);
                      gMapOverlay_Rouite.Markers.Remove(RouiteMarkList[0]);//删除路线层第一个点
                      gMapOverlay_Rouite.Routes.Remove(gMapRoute);//删除路线层上的路线
                      RouiteMarkList.Remove(RouiteMarkList[0]);//移除点集合
                  }
                  gMapRoute.Points.Clear();//删除路线上的点
                  gMapRoute.Points.AddRange(drawRouiteList);//从新更新
                  gMapOverlay_Rouite.Markers.Add(mark);
                  gMapOverlay_Rouite.Routes.Add(gMapRoute);
                  MainMap.UpdateRouteLocalPosition(gMapRoute);
                  MainMap.Refresh();
              })
              );

        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

            AboutForm a = new AboutForm();
            a.ShowDialog();
        }

        //无人机地面站使用说明
        private void UsingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Application.StartupPath + "//无人机地面站使用说明.chm";
            try
            {
                System.Diagnostics.Process.Start(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 遥控器校准
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 遥控器校准ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 引导模式sethomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetHomeForm sethomeform = new SetHomeForm();
            sethomeform.ShowDialog();
            if (wp1 == null)
            {
                wp1 = new GMapMarkerWP(new PointLatLng(Convert.ToDouble(this.label3.Text), Convert.ToDouble(this.label2.Text)), "H");
            }

            wp1.Position = new PointLatLng(Convert.ToDouble(this.label3.Text), Convert.ToDouble(this.label2.Text));
            wp1.ToolTipMode = MarkerTooltipMode.OnMouseOver;
            wp1.ToolTipText = string.Format("高度:{0},经度:{1},维度:{2}", (float)Global.home.Alt, wp1.Position.Lng, wp1.Position.Lat);



            gMapOverlay.Markers.Add(wp1);//将点添加到第二层
            MainMap.Overlays.Add(gMapOverlay);
            this.MainMap.Position = new PointLatLng(Convert.ToDouble(this.label3.Text), Convert.ToDouble(this.label2.Text));
            MainMap.Refresh();

            DoCommand(MavLink.MAV_CMD.DO_SET_HOME, 0, 0, 0, 0, (float)Convert.ToDouble(this.label3.Text), (float)Convert.ToDouble(this.label2.Text), (float)Global.home.Alt);
        }

        private void 引导模式起飞ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTakeOffAltForm setTakeOffAltForm = new SetTakeOffAltForm();
            setTakeOffAltForm.ShowDialog();
           
           // dgvPoints.Rows[rowIndex].Cells[3].Value = Global.takeOffAlt.ToString();
           

            MavLink.mavlink_Set_Mode mode = SetMode("引导");
            AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.SET_MODE, mode);
            try
            {
                DoCommand(MavLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, Global.takeOffAlt);
            }
            catch
            {
                MessageBox.Show("起飞错误!", "错误");
            }
        }

        private void 引导模式飞到这ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTakeOffAltForm setTakeOffAltForm = new SetTakeOffAltForm();
            setTakeOffAltForm.ShowDialog();
            LocationWayPoint gotohere = new LocationWayPoint();
            gotohere.id = (byte)MavLink.MAV_CMD.WAYPOINT;
            gotohere.alt = Global.takeOffAlt;
            gotohere.lng = Convert.ToDouble(this.label2.Text);
            gotohere.lat = Convert.ToDouble(this.label3.Text);

            MavLink.mavlink_Set_Mode mode = SetMode("引导");
            AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.SET_MODE, mode);

            try
            {
                SendWP(gotohere, 0, MAVLink.MavLink.MAV_FRAME.GLOBAL_RELATIVE_ALT, (byte)2);//注意
            }
            catch
            {
                MessageBox.Show("本操作失败！");
            }

        }

        private void 写入航点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!beforeSendCommand())
            {
                return;
            }
            if (pointlatlngaltList.Count > 0)//航点集合
            {
                List<LocationWayPoint> missionWayPointList = LatlonAltToLocationWayPoints();

                ushort index = (ushort)(dgvPoints.RowCount);

                var total = SendWpsTotal(index);//发送总航点数 这里容易出现错误 一定要确保航点的总数发送正确

                if (!total)
                {
                    //MessageBox.Show("航点总数发送失败");
                    return;
                }
            }
            else//没有发送的点的集合
            {
                MessageBox.Show("请绘制要发送的点！");
                return;
            }
            timer1.Start();
        }

        private void 读取航点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!beforeSendCommand())
            {
                return;
            }
            if (pointlatlngaltList.Count > 0)
            {
                if (MessageBox.Show("是否清除原来的航点坐标。", "警告", MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    return;
                }
                else
                {
                    if (pointsList != null)
                    {
                        // 清除原来的轨迹
                        pointsList.Clear();//经纬度集合清空
                        setDistance(pointsList);
                        pointlatlngaltList.Clear();//经纬度高度的集合清空
                        gMapOverlay.Markers.Clear();
                        gMapOverlay.Polygons.Clear();
                        //MainMap.Overlays.Clear();//没有必要把地图上的所有层都清出
                        wpsList.Clear();
                        markerNum = 0;
                        dgvPoints.Rows.Clear();
                    }
                }
            }
            ReadWayPointsList.Clear();
            cmdlist.Clear();
            //GetWPCount();
            //可能要出现错误
            if (GetWPCount() == 255)//确保一定得到或者一定没有得到
            {
                // MessageBox.Show("航点总数获取失败！");
                return;
            }
        }

        /// <summary>
        /// 右键菜单设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void youjianItem_Opening(object sender, CancelEventArgs e)
        {
            if (IsChick)
            {
                //youjianItem.Enabled = true;
                youjianItem.Items[0].Enabled = true;
                youjianItem.Items[1].Enabled = true;
                youjianItem.Items[2].Enabled = true;
                youjianItem.Items[8].Enabled = true;
                youjianItem.Items[9].Enabled = true;
                youjianItem.Items[10].Enabled = true;
                youjianItem.Items[11].Enabled = true;
                youjianItem.Items[12].Enabled = true;
                youjianItem.Items[13].Enabled = true;
                youjianItem.Items[14].Enabled = true;
                //youjianItem.Items[3].Enabled = true;
            }
            else
            {
               // youjianItem.Enabled = false;
                youjianItem.Items[0].Enabled = false;
                youjianItem.Items[1].Enabled = false;
                youjianItem.Items[2].Enabled = false;
                youjianItem.Items[8].Enabled = false;
                youjianItem.Items[9].Enabled = false;
                youjianItem.Items[10].Enabled = false;
                youjianItem.Items[11].Enabled = false;
                youjianItem.Items[12].Enabled = false;
                youjianItem.Items[13].Enabled = false;
                youjianItem.Items[14].Enabled = false;
                //youjianItem.Items[3].Enabled = false;
            }
                
        }

        /// <summary>
        /// 智能控制起飞
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 起飞ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!beforeSendCommand())
            {
                return;
            }
            SetTakeOffAltForm setTakeOffAltForm = new SetTakeOffAltForm();
            setTakeOffAltForm.ShowDialog();
            int rowIndex = this.dgvPoints.Rows.Add();
            dgvPoints.Rows[rowIndex].Cells[0].Value = MavLink.MAV_CMD.TAKEOFF.ToString();
            dgvPoints.Rows[rowIndex].Cells[1].Value = 0.ToString();
            dgvPoints.Rows[rowIndex].Cells[2].Value = 0.ToString();
            dgvPoints.Rows[rowIndex].Cells[3].Value = Global.takeOffAlt.ToString();
            dgvPoints.Rows[rowIndex].Cells[4].Value = "0";
        }

        /// <summary>
        /// 智能控制降落
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 降落ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!beforeSendCommand())
            {
                return;
            }
            SetLandAltForm setLandAltForm = new SetLandAltForm();
            setLandAltForm.ShowDialog();
            markerNum++;
            landWP = new GMapMarkerWP(new PointLatLng(Convert.ToDouble(this.label3.Text), Convert.ToDouble(this.label2.Text)), markerNum.ToString());
            SetPointLatLngAlt(new PointLatLng(Convert.ToDouble(this.label3.Text), Convert.ToDouble(this.label2.Text)), Global.landAlt);//设置纬度经度高度
            landWP.ToolTipMode = MarkerTooltipMode.OnMouseOver;
            landWP.ToolTipText = string.Format("高度:{0},经度:{1},维度:{2}", Global.landAlt, landWP.Position.Lng, landWP.Position.Lat);

            if (pointsList == null)
            {
                pointsList = new List<PointLatLng>();
            }
            pointsList.Add(landWP.Position);//将标记点添加到 点List中
            setDistance(pointsList);//调用测量距离的方法
            wpsList.Add(landWP);
            if (gMapPolygon == null)
            {
                gMapPolygon = new GMapPolygon(pointsList, "my polygon");
            }
            if (gMapOverlay == null)
            {
                gMapOverlay = new GMapOverlay();
            }
            gMapPolygon.Points.Clear();
            gMapOverlay.Polygons.Clear();
            gMapPolygon.Points.AddRange(pointsList);//将 点集合添加到多边形中
            MainMap.UpdatePolygonLocalPosition(gMapPolygon);//更新多边形的位置
            gMapOverlay.Markers.Add(landWP);//将点添加到第二层
            gMapOverlay.Polygons.Add(gMapPolygon);//将多边形添加到第二层
            if (MainMap.Overlays.Count == 0)//如果点击了删除全部航点的时候 那么Overlay=0了 再也添加不上点了
            {
                MainMap.Overlays.Add(gMapOverlay);//将第二层添加到第三层
            }
            this.MainMap.Position = this.MainMap.Position;
            MainMap.Refresh();
            pointWriteDGV(MavLink.MAV_CMD.LAND, new PointLatLngAlt(landWP.Position.Lng, landWP.Position.Lat, Global.landAlt));
        }
        /// <summary>
        /// 智能控制返航
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 返航ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!beforeSendCommand())
            {
                return;
            }
            else
                if(MessageBox.Show("你想要回到起飞点?", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                //DoCommand(MavLink.MAV_CMD.RETURN_TO_LAUNCH, 0, 0, 1, 0, 0, 0, 0);//这个是param3 = 1 源码上发现的 需要验证
                pointWriteDGV(MavLink.MAV_CMD.RETURN_TO_LAUNCH, new PointLatLngAlt(0, 0, 0));

            }
        }

        private void 罗盘校准ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!beforeSendCommand())
            {
                return;
            }
            SanHeGroundStation.Forms.ProgressReporterSphereUsing.MagCalib.DoGUIMagCalib();
        }

        uint systimeold = 0;
        uint systimenum = 0;
        private void timer2_Tick(object sender, EventArgs e)
        {

            if (systimenum == 0)
            {
                systimeold = Global.mavStatus.attitude.time_boot_ms;
                systimenum = 1;
            }
            else
            {
                uint systimenew = Global.mavStatus.attitude.time_boot_ms;
                if (systimeold == systimenew)
                {
                    systimenum++;
                }
                else
                {
                    systimeold = systimenew;
                    systimenum = 1;
                }
                if (systimenum >= 3)
                {
                    try
                    {
                        sp.Close();
                        sp.Dispose();//注意释放  不然就只能关一次 之后不会在关闭了
                        Global.isConn = false;
                        //SerialPort newPort = new SerialPort();
                        //if(newPort.IsOpen==true)
                        //{
                        //    newPort.Close();
                        //}
                        bufferNum = 0;
                    }
                    catch
                    {

                    }
                    label26.Text= "已断开";
                    timer2.Stop();

                }
            }
            if (!Global.isConn)
            {
                timer2.Stop();
                return;
            }



        }
        //解锁地图编辑
        private int ButtonNum = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            this.button1.Text = "退出编辑模式";
            ButtonNum++;
            if(ButtonNum%2==1)
            {
                this.label20.Text = "可编辑";
                IsChick = true;
                
            }
            else{
                this.label20.Text = "不可编辑";
                IsChick = false;
            }
            
        }
        /// <summary>
        /// 读取航点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (!beforeSendCommand())
            {
                return;
            }
            if (pointlatlngaltList.Count > 0)
            {
                if (MessageBox.Show("是否清除原来的航点坐标。", "警告", MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    return;
                }
                else
                {
                    if (pointsList != null)
                    {
                        // 清除原来的轨迹
                        pointsList.Clear();//经纬度集合清空
                        setDistance(pointsList);
                        pointlatlngaltList.Clear();//经纬度高度的集合清空
                        gMapOverlay.Markers.Clear();
                        gMapOverlay.Polygons.Clear();
                        //MainMap.Overlays.Clear();//没有必要把地图上的所有层都清出
                        wpsList.Clear();
                        markerNum = 0;
                        dgvPoints.Rows.Clear();
                    }
                }
            }
            ReadWayPointsList.Clear();
            cmdlist.Clear();
            //GetWPCount();
            //可能要出现错误
            if (GetWPCount() == 255)//确保一定得到或者一定没有得到
            {
                // MessageBox.Show("航点总数获取失败！");
                return;
            }
        }
        
        /// <summary>
        /// 写入航点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (!beforeSendCommand())
            {
                return;
            }
            if (pointlatlngaltList.Count > 0)//航点集合
            {
                List<LocationWayPoint> missionWayPointList = LatlonAltToLocationWayPoints();

                ushort index = (ushort)(dgvPoints.RowCount);

                var total = SendWpsTotal(index);//发送总航点数 这里容易出现错误 一定要确保航点的总数发送正确

                if (!total)
                {
                    //MessageBox.Show("航点总数发送失败");
                    return;
                }
            }
            else//没有发送的点的集合
            {
                MessageBox.Show("请绘制要发送的点！");
                return;
            }
            timer1.Start();
        }
        /// <summary>
        /// 保存航点信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            if (dgvPoints.RowCount == 1)
            {
                MessageBox.Show("请添加航点信息后再保存！", "错误");
                return;
            }
            using (SaveFileDialog fd = new SaveFileDialog())
            {
                fd.Filter = "Ardupilot Mission (*.txt)|*.*";
                fd.DefaultExt = ".txt";
                fd.FileName = wpfilename;
                DialogResult result = fd.ShowDialog();
                string file = fd.FileName;
                if (file != "")
                {
                    try
                    {
                        StreamWriter sw = new StreamWriter(file);
                        sw.WriteLine("QGC WPL 110");
                        try
                        {
                            //sw.WriteLine("0\t1\t0\t16\t0\t0\t0\t0\t" + double.Parse(TXT_homelat.Text).ToString("0.000000", new CultureInfo("en-US")) + "\t" + double.Parse(TXT_homelng.Text).ToString("0.000000", new CultureInfo("en-US")) + "\t" + double.Parse(TXT_homealt.Text).ToString("0.000000", new CultureInfo("en-US")) + "\t1");
                        }
                        catch
                        {
                            sw.WriteLine("0\t1\t0\t0\t0\t0\t0\t0\t0\t0\t0\t1");
                        }
                        for (int a = 0; a < dgvPoints.Rows.Count - 1; a++)
                        {
                            byte mode = (byte)(MavLink.MAV_CMD)Enum.Parse(typeof(MavLink.MAV_CMD), dgvPoints.Rows[a].Cells[0].Value.ToString());

                            sw.Write((a + 1)); // seq
                            sw.Write("\t" + 0); // current

                            sw.Write("\t" + mode);
                            sw.Write("\t" + double.Parse(dgvPoints.Rows[a].Cells[1].Value.ToString()).ToString("0.000000", new CultureInfo("en-US")));
                            sw.Write("\t" + double.Parse(dgvPoints.Rows[a].Cells[2].Value.ToString()).ToString("0.000000", new CultureInfo("en-US")));
                            sw.Write("\t" + double.Parse(dgvPoints.Rows[a].Cells[3].Value.ToString()).ToString("0.000000", new CultureInfo("en-US")));
                            sw.Write("\t" + double.Parse(dgvPoints.Rows[a].Cells[4].Value.ToString()).ToString("0.000000", new CultureInfo("en-US")));

                            sw.WriteLine("");
                        }
                        sw.Close();
                        MessageBox.Show("航点信息保存成功！");
                    }
                    catch
                    {

                    }
                }
            }
        }
        /// <summary>
        /// 打开航点文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            DeleteAllToolStripMenuItem_Click(sender, e);

            using (OpenFileDialog fd = new OpenFileDialog())
            {

                fd.Filter = "Ardupilot Mission (*.txt)|*.*|Shape file|*.shp";
                DialogResult result = fd.ShowDialog();
                string file = fd.FileName;
                if (File.Exists(file))
                {
                    List<LocationWayPoint> cmdlist1 = new List<LocationWayPoint>();
                    try
                    {
                        StreamReader sr = new StreamReader(file);
                        string header = sr.ReadLine();
                        if (header == null || !header.Contains("QGC WPL"))
                        {
                            MessageBox.Show("这是个无效的航点文件！");
                            return;
                        }
                        while (!sr.EndOfStream)
                        {
                            string line = sr.ReadLine();
                            string[] items = line.Split(new[] { '\t', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                            try
                            {

                                LocationWayPoint temp = new LocationWayPoint();

                                temp.id = (byte)(int)Enum.Parse(typeof(MavLink.MAV_CMD), items[2], false);
                                temp.p1 = float.Parse(items[1], new CultureInfo("en-US"));



                                temp.alt = (float)(double.Parse(items[5], new CultureInfo("en-US")));
                                temp.lat = (double.Parse(items[4], new CultureInfo("en-US")));
                                temp.lng = (double.Parse(items[3], new CultureInfo("en-US")));


                                cmdlist1.Add(temp);



                            }
                            catch { MessageBox.Show("Line invalid\n" + line); }


                        }
                        drawDgv(cmdlist1, 0);
                        wpslisttopointlist(cmdlist1, 0);

                        precsslatlngalt(pointlatlngaltList);
                    }
                    catch
                    {

                    }

                }

            }
        }
        /// <summary>
        /// 清除航点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (IsChick)
            {
                if (pointsList == null)
                {
                    return;
                }
                pointsList.Clear();//经纬度集合清空
                setDistance(pointsList);
                pointlatlngaltList.Clear();//经纬度高度的集合清空
                gMapOverlay.Markers.Clear();
                gMapOverlay.Polygons.Clear();
                //MainMap.Overlays.Clear();//没有必要把地图上的所有层都清出
                wpsList.Clear();
                markerNum = 0;
                dgvPoints.Rows.Clear();
                pointsList.Add(new PointLatLng(mavStatus.gps_Row.lat / Math.Pow(10, 7), mavStatus.gps_Row.lng / Math.Pow(10, 7)));
            }
        }
        //设置家
        private void button7_Click(object sender, EventArgs e)
        {
            double Homelat=0;
            double Homelng=0;
            if (!beforeSendCommand())
            {
                return;
            }
            if (textBox1.Text.Trim() != "" && textBox2.Text.Trim() != "")
            {
                if (Double.TryParse(textBox1.Text.ToString(), out Homelat) && Double.TryParse(textBox2.Text.ToString(), out Homelng))
                {

                    if (!isOpen)
                    {
                        Global.nowLocation = new PointLatLng(Homelat, Homelng);//设置初始位置
                        this.Invoke(new Action(() =>
                        {
                            MainMap.Position = Global.nowLocation;
                            MainMap.Zoom = 14;
                            pointsList.Clear();       
                            pointsList.Add( new PointLatLng(Homelat,Homelng));
                            DrawRouite(new PointLatLngAlt(Homelat, Homelng, mavStatus.gps_Row.alt / Math.Pow(10, 7)));//绘制 
                        }));

                        isOpen = false;
                    }

                }
                else
                {
                    MessageBox.Show("请输入有效数据！", "警告！", MessageBoxButtons.OK);
                }

            }
            else
            {
                MessageBox.Show("经纬度不能为空。","提示！",MessageBoxButtons.OK);
            }
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }


        public Action ConnClick;
        private void btnConn_Click(object sender, EventArgs e)
        {
            if (btnConn.Text == "连接")
            {
                Global.baudRate = Convert.ToInt32(CmdBoTeLiu.Text);
                Global.portsName = CmbChuanKou.Text.Trim(); 
                this.ConnClick = new Action(Conn);//现在并没有触发委托的那个事件，只有点击的时候才触发，现在是给委托绑定事件
                ConnClick();
            }
            else
            {
                if (!Global.isConn)
                {
                    return;
                }
                try
                {
                    sp.Close();
                    sp.Dispose();//注意释放  不然就只能关一次 之后不会在关闭了
                    Global.isConn = false;
                    bufferNum = 0;
                    label26.Text = "已断开";

                }
                catch
                {

                }

            }
        }

        private void label26_Click(object sender, EventArgs e)
        {
            
        }

        private void label26_TextChanged(object sender, EventArgs e)
        {
            if (label26.Text == "已连接")
            {
                btnConn.Text = "断开";
            }
            else
            {
                btnConn.Text = "连接";
            }
        }

        private void CmbChuanKou_DropDown(object sender, EventArgs e)
        {
            this.CmbChuanKou.DataSource = SerialPort.GetPortNames();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 设置电池电量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void labYuLiang_TextChanged(object sender, EventArgs e)
        {
            setBattery();
        }

        

       


        
       
    }
}
