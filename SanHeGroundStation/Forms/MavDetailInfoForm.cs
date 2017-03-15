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
    public partial class MavDetailInfoForm : Form
    {
        public MavDetailInfoForm()
        {
            InitializeComponent();
            timerReflush.Start();
        }

        private void timerReflush_Tick(object sender, EventArgs e)
        {
            // 0 心跳包
            this.txtAutopilot.Text = Global.mavStatus.heart_Pack.autopilot.ToString();
            this.txtSystemModel.Text = Global.mavStatus.heart_Pack.base_mode.ToString();
            this.txtSystemState.Text = Global.mavStatus.heart_Pack.system_status.ToString();
            this.txtType.Text = Global.mavStatus.heart_Pack.type.ToString();
            this.txtUserModel.Text = Global.mavStatus.heart_Pack.custom_mode.ToString();
            this.txtVersion.Text = Global.mavStatus.heart_Pack.mavlink_version.ToString();
            // 1 系统状态
            this.textBox1.Text = Global.mavStatus.sys_Status.onboard_control_sensors_present.ToString();
            this.textBox2.Text = Global.mavStatus.sys_Status.onboard_control_sensors_enabled.ToString();
            this.textBox3.Text = Global.mavStatus.sys_Status.onboard_control_sensors_health.ToString();
            this.textBox4.Text = Global.mavStatus.sys_Status.load.ToString();
            this.textBox5.Text = Global.mavStatus.sys_Status.voltage_battery.ToString();
            this.textBox6.Text = Global.mavStatus.sys_Status.current_battery.ToString();
            this.textBox7.Text = Global.mavStatus.sys_Status.battery_remaining.ToString();
            this.textBox8.Text = Global.mavStatus.sys_Status.drop_rate_comm.ToString();
            this.textBox9.Text = Global.mavStatus.sys_Status.errors_comm.ToString();
            this.textBox10.Text = Global.mavStatus.sys_Status.errors_count1.ToString();
            this.textBox11.Text = Global.mavStatus.sys_Status.errors_count2.ToString();
            this.textBox12.Text = Global.mavStatus.sys_Status.errors_count3.ToString();
            this.textBox13.Text = Global.mavStatus.sys_Status.errors_count4.ToString();
            // 2 系统时间
            this.textBox14.Text = Global.mavStatus.system_Time.time_unix_usec.ToString();
            this.textBox15.Text = Global.mavStatus.system_Time.time_boot_ms.ToString();
            // 30 姿态信息
            this.textBox16.Text = Global.mavStatus.attitude.time_boot_ms.ToString();
            this.textBox17.Text = Global.mavStatus.attitude.roll.ToString();
            this.textBox18.Text = Global.mavStatus.attitude.pitch.ToString();
            this.textBox22.Text = Global.mavStatus.attitude.yaw.ToString();
            this.textBox24.Text = Global.mavStatus.attitude.rollspeed.ToString();
            this.textBox20.Text = Global.mavStatus.attitude.pitchspeed.ToString();
            this.textBox21.Text = Global.mavStatus.attitude.yawspeed.ToString();

            // 24 GPS信息
            this.textBox27.Text = Global.mavStatus.gps_Row.time_usec.ToString();
            this.textBox29.Text = Global.mavStatus.gps_Row.fix_type.ToString();
            this.textBox25.Text = (Global.mavStatus.gps_Row.lat/Math.Pow(10,7)).ToString();
            this.textBox26.Text = (Global.mavStatus.gps_Row.lng / Math.Pow(10, 7)).ToString();
            this.textBox28.Text = Global.mavStatus.gps_Row.alt.ToString();
            this.textBox32.Text = Global.mavStatus.gps_Row.eph.ToString();
            this.textBox34.Text = Global.mavStatus.gps_Row.epv.ToString();
            this.textBox30.Text = Global.mavStatus.gps_Row.vel.ToString();
            this.textBox31.Text = Global.mavStatus.gps_Row.cog.ToString();
            this.textBox33.Text = Global.mavStatus.gps_Row.satellites_visible.ToString();
            // 62 位置导航输出 
            this.textBox38.Text = Global.mavStatus.nav_Controller_Output.nav_roll.ToString();
            this.textBox43.Text = Global.mavStatus.nav_Controller_Output.nav_pitch.ToString();
            this.textBox46.Text = Global.mavStatus.nav_Controller_Output.nav_bearing.ToString();
            this.textBox47.Text = Global.mavStatus.nav_Controller_Output.target_bearing.ToString();
            this.textBox48.Text = Global.mavStatus.nav_Controller_Output.wp_dist.ToString();
            this.textBox49.Text = Global.mavStatus.nav_Controller_Output.alt_error.ToString();
            this.textBox50.Text = Global.mavStatus.nav_Controller_Output.aspd_error.ToString();
            this.textBox51.Text = Global.mavStatus.nav_Controller_Output.xtrack_error.ToString();

            // 33 位置信息
            this.textBox41.Text = Global.mavStatus.global_Position.time_boot_ms.ToString();
            this.textBox39.Text = Global.mavStatus.global_Position.lat.ToString();
            this.textBox44.Text = Global.mavStatus.global_Position.lon.ToString();
            this.textBox35.Text = Global.mavStatus.global_Position.alt.ToString();
            this.textBox37.Text = (Global.mavStatus.global_Position.relative_alt / 1000).ToString();
            this.textBox42.Text = Global.mavStatus.global_Position.vx.ToString();
            this.textBox40.Text = Global.mavStatus.global_Position.vy.ToString();
            this.textBox45.Text = Global.mavStatus.global_Position.vz.ToString();
            this.textBox36.Text = Global.mavStatus.global_Position.hdg.ToString();

            //150传感器的偏移量
            this.Accel_X.Text = Global.mavStatus.sensor_offset.accel_cal_x.ToString();
            this.Accel_Y.Text = Global.mavStatus.sensor_offset.accel_cal_y.ToString();
            this.Accel_Z.Text = Global.mavStatus.sensor_offset.accel_cal_z.ToString();


            //gps信息
            //this.txtgpslng.Text = Global.mavStatus.gps_Row.lng.ToString();
            //this.txtgpslat.Text = Global.mavStatus.gps_Row.lat.ToString();
            //this.txtgpsalt.Text = Global.mavStatus.gps_Row.alt.ToString();
            
            this.txtgpslng.Text =( Global.mavStatus.gps_Row.lng/Math.Pow(10, 7)).ToString();
            this.txtgpslat.Text = (Global.mavStatus.gps_Row.lat / Math.Pow(10, 7)).ToString();
            this.txtgpsalt.Text = (Global.mavStatus.gps_Row.alt / Math.Pow(10, 7)).ToString();
             
      
        }

        private void MavDetailInfoForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerReflush.Stop();
        }

      
    }
}
