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
    public partial class GeoFence : Form
    {
        public MavLinkInterface mavLinkInterface=new MavLinkInterface ();
        public GeoFence()
        {
            InitializeComponent();
            fence_type.DataSource = new string[] { "无", "高度", "圆形", "高度+圆形" };
            fence_Action.DataSource = new string[] { "仅报告", "返航或者降落" };
        }
        //设置电子围栏
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!Global.isConn)
            {
                MessageBox.Show("串口没有连接!");
                return;
            }
            //设置是否选中
            if (fence_Check.Checked == true)
            {
                SetGeoFence("FENCE_ENABLE", 1, 2);
            }
            else
            {
                SetGeoFence("FENCE_ENABLE", 0, 2);
            }
            //设置type类型
            switch (fence_type.Text)
            {
                case "无":
                    SetGeoFence("FENCE_TYPE", 0, 2);
                    break;
                case "高度":
                    SetGeoFence("FENCE_TYPE", 1, 2);
                    break;
                case "圆形":
                    SetGeoFence("FENCE_TYPE", 2, 2);
                    break;
                case "高度+圆形":
                    SetGeoFence("FENCE_TYPE", 3, 2);
                    break;
                default:
                    break;

            }
            //设置Action
            switch (fence_Action.Text)
            {
                case "仅报告":
                    SetGeoFence("FENCE_ACTION", 0, 2);
                    break;
                case "返航或者降落":
                    SetGeoFence("FENCE_ACTION", 1, 2);
                    break;
                default:
                    break;
            }
            //设置高度
            SetGeoFence("FENCE_ALT_MAX", (float)fence_Alt.Value, 9);

            //设置半径
            SetGeoFence("FENCE_RADIUS", (float)fence_Radius.Value, 9);

            //设置RLT
            SetGeoFence("RTL_ALT", (float)fence_RTL.Value, 4);
            this.Close();
            this.Dispose();
        }
    }
}
