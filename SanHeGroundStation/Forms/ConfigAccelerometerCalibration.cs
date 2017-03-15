using MAVLink;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SanHeGroundStation.Forms
{
    public partial class ConfigAccelerometerCalibration : Form
    {
        public ConfigAccelerometerCalibration()
        {
            InitializeComponent();
            if (lbAccel.Text.Contains("左") || lbAccel.Text.Contains("右") || lbAccel.Text.Contains("上") || lbAccel.Text.Contains("下") || lbAccel.Text.Contains("反") || lbAccel.Text.Contains("水平"))
            {
                btnCalibrationAccel.Text = "完成时点击";
            }
        }

        int result = 0;
        bool alCalibrationLevel = false;
        public Func<bool> btnCalibrationLevelEvent;
        public Func<int, bool> btnCalibrationAccelEvent;
        public Action btnCalibrationClose;

        public void setCalibrationAccelInfo(string info)
        {
            if (info.ToLower().Contains("calibration failed"))
            {
                pictureBox1.Image = SanHeGroundStation.Properties.Resources.calibration01;
                btnCalibrationAccel.Text = "完成";
                lbAccel.Text = "加速度计校准失败！";
                Global.CalibrationAccelInfo = "";
                Global.alCalibrationLevel = false;
            }
            else if (info.ToLower().Contains("calibration successful"))
            {
                pictureBox1.Image = SanHeGroundStation.Properties.Resources.calibration01;
                btnCalibrationAccel.Text = "完成";
                lbAccel.Text = "加速度计校准成功！";
                Global.CalibrationAccelInfo = "";
                Global.alCalibrationLevel = false;
            }
            else if (info.ToLower().Contains("left"))
            {
                pictureBox1.Image = SanHeGroundStation.Properties.Resources.calibration07;
                lbAccel.Text = "请左放置您的自驾仪！";
                btnCalibrationAccel.Text = "完成时点击";
                Global.CalibrationAccelInfo = "请左放置您的自驾仪！";
            }
            else if (info.ToLower().Contains("right"))
            {
                pictureBox1.Image = SanHeGroundStation.Properties.Resources.calibration05;
                lbAccel.Text = "请右放置您的自驾仪！";
                btnCalibrationAccel.Text = "完成时点击";
                Global.CalibrationAccelInfo = "请右放置您的自驾仪！";
            }
            else if (info.ToLower().Contains("down"))
            {
                pictureBox1.Image = SanHeGroundStation.Properties.Resources.calibration04;
                lbAccel.Text = "请向下放置您的自驾仪！";
                btnCalibrationAccel.Text = "完成时点击";
                Global.CalibrationAccelInfo = "请向下放置您的自驾仪！";
            }
            else if (info.ToLower().Contains("up"))
            {
                pictureBox1.Image = SanHeGroundStation.Properties.Resources.calibration06;
                lbAccel.Text = "请向上放置您的自驾仪！";
                btnCalibrationAccel.Text = "完成时点击";
                Global.CalibrationAccelInfo = "请向上放置您的自驾仪！";
            }
            else if (info.ToLower().Contains("back"))
            {
                pictureBox1.Image = SanHeGroundStation.Properties.Resources.calibration03;
                lbAccel.Text = "请反向放置您的自驾仪！";
                btnCalibrationAccel.Text = "完成时点击";
                Global.CalibrationAccelInfo = "请反向放置您的自驾仪！";
            }
            else if (info.ToLower().Contains("place vehicle level and press any key"))
            {
                if (Convert.ToInt32(btnCalibrationLevel.Tag) != 1)
                {
                    pictureBox1.Image = SanHeGroundStation.Properties.Resources.calibration01;
                    lbAccel.Text = "请水平放置您的自驾仪！";
                    btnCalibrationAccel.Text = "完成时点击";
                    Global.CalibrationAccelInfo = "请水平放置您的自驾仪！";
                }
            }
            lbAccel.Visible = true;
        }
        /// <summary>
        /// 水平校准
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCalibrationLevel_Click(object sender, EventArgs e)
        {
            if (btnCalibrationLevel.Text == "完成")
            {
                return;
            }
            if (!Global.isConn)
            {
                MessageBox.Show("请检查串口是否连接！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            btnCalibrationLevel.Tag = 1;
            Global.alCalibrationLevel = btnCalibrationLevelEvent();
            btnCalibrationLevel.Text = "完成";
        }
        /// <summary>
        /// 加速度计校准
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCalibrationAccel_Click(object sender, EventArgs e)
        {
           
            if (btnCalibrationAccel.Text == "完成")
            {
                return;
            }
            if (!Global.isConn)
            {
                MessageBox.Show("请检查串口是否连接！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            result++;
            btnCalibrationAccelEvent(result);
            if (Global.alCalibrationLevel)
            {
                btnCalibrationAccel.Text = "完成时点击";
            }
        }

     







    }
}
