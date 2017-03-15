using MAVLink;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SanHeGroundStation.Forms
{
    public partial class InitForm : Form
    {
        public MavLinkInterface mavLinkInterface = new MavLinkInterface();
        private readonly System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();//定义时钟
        SerialPort sp = new SerialPort();
        private readonly double pwm = 0;
        [Flags]
        public enum SimpleMode      //飞行模式枚举
        {
            None = 0,
            Simple1 = 1,
            Simple2 = 2,
            Simple3 = 4,
            Simple4 = 8,
            Simple5 = 16,
            Simple6 = 32
        }
        public InitForm()
        {

            InitializeComponent();
            Init();
        }

        /// <summary>
        /// 初始化界面
        /// </summary>
        private void Init()
        {

            //绑定数据源
            Activate();

        }

        /// <summary>
        /// 绑定数据源
        /// </summary>
        public void Activate()
        {
            #region 绑定数据源

            string[] Source = new string[] { "自稳", "定高", "定点", "任务", "环绕", "返航", "降落", "引导" };
            ComboBox[] flightmodes = new ComboBox[] { FlightMode1, FlightMode2, FlightMode3, FlightMode4, FlightMode5, FlightMode6 };
            foreach (ComboBox temp in flightmodes)
            {
                temp.Items.AddRange(Source);
            }
            CurrentMode.Text = FlightMode1.Text.Trim();
            CurrentPWM.Text = pwm.ToString();
            #endregion

            #region 给不同飞行模式设置初始值

            try
            {
                string mode1 = IdToMode(int.Parse(Global.mavStatus.paraDic["FLTMODE1"].ToString()));
                string mode2 = IdToMode(int.Parse(Global.mavStatus.paraDic["FLTMODE2"].ToString()));
                string mode3 = IdToMode(int.Parse(Global.mavStatus.paraDic["FLTMODE3"].ToString()));
                string mode4 = IdToMode(int.Parse(Global.mavStatus.paraDic["FLTMODE4"].ToString()));
                string mode5 = IdToMode(int.Parse(Global.mavStatus.paraDic["FLTMODE5"].ToString()));
                string mode6 = IdToMode(int.Parse(Global.mavStatus.paraDic["FLTMODE6"].ToString()));
                FlightMode1.SelectedItem = mode1;
                FlightMode2.SelectedItem = mode2;
                FlightMode3.SelectedItem = mode3;
                FlightMode4.SelectedItem = mode4;
                FlightMode5.SelectedItem = mode5;
                FlightMode6.SelectedItem = mode6;
            }
            catch
            {
                string mode1 = "自稳";
                string mode2 = "定高";
                string mode3 = "任务";
                string mode4 = "环绕";
                string mode5 = "返航";
                string mode6 = "降落";
                FlightMode1.SelectedItem = mode1;
                FlightMode2.SelectedItem = mode2;
                FlightMode3.SelectedItem = mode3;
                FlightMode4.SelectedItem = mode4;
                FlightMode5.SelectedItem = mode5;
                FlightMode6.SelectedItem = mode6;
            }
            if (Global.mavStatus.paraDic.ContainsKey("SIMPLE"))
            {
                var simple = int.Parse(Global.mavStatus.paraDic["SIMPLE"].ToString());
                Cb_simple1.Checked = ((simple >> 0 & 1) == 1);
                Cb_simple2.Checked = ((simple >> 1 & 1) == 1);
                Cb_simple3.Checked = ((simple >> 2 & 1) == 1);
                Cb_simple4.Checked = ((simple >> 3 & 1) == 1);
                Cb_simple5.Checked = ((simple >> 4 & 1) == 1);
                Cb_simple6.Checked = ((simple >> 5 & 1) == 1);
            }
            if (Global.mavStatus.paraDic.ContainsKey("SUPER_SIMPLE"))
            {
                var simple = int.Parse(Global.mavStatus.paraDic["SUPER_SIMPLE"].ToString());
                Cb_ss1.Checked = ((simple >> 0 & 1) == 1);
                Cb_ss2.Checked = ((simple >> 1 & 1) == 1);
                Cb_ss3.Checked = ((simple >> 2 & 1) == 1);
                Cb_ss4.Checked = ((simple >> 3 & 1) == 1);
                Cb_ss5.Checked = ((simple >> 4 & 1) == 1);
                Cb_ss6.Checked = ((simple >> 5 & 1) == 1);
            }
            #endregion

            timer.Tick += timer_Tick;
            timer.Enabled = true;
            timer.Interval = 100;
            timer.Start();
        }

        public void Dactivate()
        {
            timer.Stop();
        }

        /// <summary>
        /// 根据PWM的不同，设置不同的飞行模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            #region 注释
            /*
            switch (Judge_PWM(pwm))
            {
                case 1:
                    //设置飞行模式
                    MavLink.mavlink_Set_Mode mode = SetMode(FlightMode1.Text.Trim());
                    AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.SET_MODE, mode);
                    Thread.Sleep(10);
                    AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.SET_MODE, mode);
                    //显示当前的飞行模式
                    CurrentMode.Text = FlightMode1.Text.Trim();
                    CurrentPWM.Text = pwm.ToString();
                    break;
                case 2:
                    //设置飞行模式
                    MavLink.mavlink_Set_Mode mode2 = SetMode(FlightMode2.Text.Trim());
                    AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.SET_MODE, mode2);
                    Thread.Sleep(10);
                    AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.SET_MODE, mode2);
                    //显示当前的飞行模式
                    CurrentMode.Text = FlightMode2.Text.Trim();
                    CurrentPWM.Text = pwm.ToString();
                    break;
                case 3:
                    //设置飞行模式
                    MavLink.mavlink_Set_Mode mode3 = SetMode(FlightMode3.Text.Trim());
                    AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.SET_MODE, mode3);
                    Thread.Sleep(10);
                    AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.SET_MODE, mode3);
                    //显示当前的飞行模式
                    CurrentMode.Text = FlightMode3.Text.Trim();
                    CurrentPWM.Text = pwm.ToString();
                    break;
                case 4:
                    //设置飞行模式
                    MavLink.mavlink_Set_Mode mode4 = SetMode(FlightMode4.Text.Trim());
                    AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.SET_MODE, mode4);
                    Thread.Sleep(10);
                    AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.SET_MODE, mode4);
                    //显示当前的飞行模式
                    CurrentMode.Text = FlightMode4.Text.Trim();
                    CurrentPWM.Text = pwm.ToString();
                    break;
                case 5:
                    //设置飞行模式
                    MavLink.mavlink_Set_Mode mode5 = SetMode(FlightMode5.Text.Trim());
                    AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.SET_MODE, mode5);
                    Thread.Sleep(10);
                    AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.SET_MODE, mode5);
                    //显示当前的飞行模式
                    CurrentMode.Text = FlightMode5.Text.Trim();
                    CurrentPWM.Text = pwm.ToString();
                    break;
                case 6:
                    //设置飞行模式
                    MavLink.mavlink_Set_Mode mode6 = SetMode(FlightMode6.Text.Trim());
                    AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.SET_MODE, mode6);
                    Thread.Sleep(10);
                    AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.SET_MODE, mode6);
                    //显示当前的飞行模式
                    CurrentMode.Text = FlightMode6.Text.Trim();
                    CurrentPWM.Text = pwm.ToString();
                    break;
            }
             */
            #endregion

            if (pwm <= 1230)
            {
                CurrentMode.Text = FlightMode1.Text.Trim();
                CurrentPWM.Text = pwm.ToString();
            }
            else if (pwm > 1230 && pwm <= 1360)
            {
                CurrentMode.Text = FlightMode2.Text.Trim();
                CurrentPWM.Text = pwm.ToString();
            }
            else if (pwm > 1360 && pwm <= 1490)
            {
                CurrentMode.Text = FlightMode3.Text.Trim();
                CurrentPWM.Text = pwm.ToString();
            }
            else if (pwm > 1490 && pwm <= 1620)
            {
                CurrentMode.Text = FlightMode4.Text.Trim();
                CurrentPWM.Text = pwm.ToString();
            }
            else if (pwm > 1620 && pwm <= 1749)
            {
                CurrentMode.Text = FlightMode5.Text.Trim();
                CurrentPWM.Text = pwm.ToString();
            }
            else if (pwm > 1749)
            {
                CurrentMode.Text = FlightMode6.Text.Trim();
                CurrentPWM.Text = pwm.ToString();
            }
        }

       

        //保存飞行模式
        private void btn_SaveModes_Click(object sender, EventArgs e)
        {
            try
            {
                SetParam(FlightMode1, "FLTMODE1");
                SetParam(FlightMode2, "FLTMODE2");
                SetParam(FlightMode3, "FLTMODE3");
                SetParam(FlightMode4, "FLTMODE4");
                SetParam(FlightMode5, "FLTMODE5");
                SetParam(FlightMode6, "FLTMODE6");
                var value = (float)(Cb_simple1.Checked ? (int)SimpleMode.Simple1 : 0) +
                    (Cb_simple2.Checked ? (int)SimpleMode.Simple2 : 0) +
                    (Cb_simple3.Checked ? (int)SimpleMode.Simple3 : 0) +
                    (Cb_simple4.Checked ? (int)SimpleMode.Simple4 : 0) +
                    (Cb_simple5.Checked ? (int)SimpleMode.Simple5 : 0) +
                    (Cb_simple6.Checked ? (int)SimpleMode.Simple6 : 0);
                SetGeoFence("SIMPLE", value, 2);
                value = (float)(Cb_ss1.Checked ? (int)SimpleMode.Simple1 : 0) +
                    (Cb_ss2.Checked ? (int)SimpleMode.Simple2 : 0) +
                    (Cb_ss3.Checked ? (int)SimpleMode.Simple3 : 0) +
                    (Cb_ss4.Checked ? (int)SimpleMode.Simple4 : 0) +
                    (Cb_ss5.Checked ? (int)SimpleMode.Simple5 : 0) +
                    (Cb_ss6.Checked ? (int)SimpleMode.Simple6 : 0);
                SetGeoFence("SUPER_SIMPLE", value, 2);
                btn_SaveModes.Text = "已保存";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private bool SetParam(Control item, string paraname)
        {
            try
            {
                switch (item.Text)
                {
                    //Stabilize 0
                    case "自稳":
                        SetGeoFence(paraname, 0, 2);
                        break;
                    //Altitude 2 
                    case "定高":
                        SetGeoFence(paraname, 2, 2);
                        break;
                    //PosHold 16
                    case "定点":
                        SetGeoFence(paraname, 16, 2);
                        break;
                    //Auto 3
                    case "任务":
                        SetGeoFence(paraname, 3, 2);
                        break;
                    //Circle 7
                    case "环绕":
                        SetGeoFence(paraname, 7, 2);
                        break;
                    //RTL 6
                    case "返航":
                        SetGeoFence(paraname, 6, 2);
                        break;
                    //Land 9
                    case "降落":
                        SetGeoFence(paraname, 9, 2);
                        break;
                    //Guided 4
                    case "引导":
                        SetGeoFence(paraname, 4, 2);
                        break;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string IdToMode(int n)
        {
            switch (n)
            {
                case 0:
                    return "自稳";
                    break;
                case 2:
                    return "定高";
                    break;
                case 3:
                    return "任务";
                    break;
                case 4:
                    return "引导";
                    break;
                case 6:
                    return "返航";
                    break;
                case 7:
                    return "环绕";
                    break;
                case 9:
                    return "降落";
                    break;
                case 16:
                    return "定点";
                    break;
                default:
                    return "";
                    break;
            }
        }

        private void SetGeoFence(string paramname, float value, byte num)
        {
            MavLink.mavlink_Param_Set param_set = new MavLink.mavlink_Param_Set();
            param_set.param_value = value;
            param_set.target_system = Global.sysID;
            param_set.target_component = Global.compID;
            byte[] temp = Encoding.ASCII.GetBytes(paramname);
            Array.Resize(ref temp, 16);
            param_set.param_id = temp;
            param_set.param_type = num;//参数值类型
            mavLinkInterface.AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.PARAM_SET, param_set);
        }

        private void InitForm_Load(object sender, EventArgs e)
        {

        }

        private void Mode_Changed(object sender, EventArgs e)
        {
            var sender2 = (Control)sender;
            var txt = sender2.Text;
            if (txt.Contains("引导") || txt.Contains("环绕"))
            {
                var number = sender2.Name.Substring(sender2.Name.Length - 1);
                setEnable("Cb_simple" + number, false);
                setEnable("Cb_ss" + number, false);
            }
            else
            {
                var number = sender2.Name.Substring(sender2.Name.Length - 1);
                setEnable("Cb_simple" + number, true);
                setEnable("Cb_ss" + number, true);
            }
        }

        private void setEnable(string name, bool judge)
        {
            var item = Controls.Find(name, true);
            if (item.Length > 0)
            {
                item[0].Enabled = judge;
            }
        }

      

       

       
       

        

        

      

    }
}
