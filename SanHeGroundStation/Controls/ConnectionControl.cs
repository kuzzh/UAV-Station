using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace SanHeGroundStation.Controls
{
    public partial class Connection : UserControl
    {
        public string[] ports;
        public bool IsOpen=false;
        /// <summary>
        /// 连接的端口的名字
        /// </summary>
        public string portsName;
        /// <summary>
        /// 连接的波特率
        /// </summary>
        public int baudRate;
        public Connection()
        {
            InitializeComponent();
        }
        private void Connection_Load(object sender, EventArgs e)
        {
            ports = SerialPort.GetPortNames();
            this.cbSerialPorts.DataSource = ports;
            this.cbSerialPorts.Text = "COM4";
            int[] baudRate = { 1200,2400,4800,9600,19200,38400,57600,111100,115200,500000,921600,1500000};
            this.cbBaudRate.DataSource = baudRate;
            this.cbBaudRate.Text = baudRate[8].ToString();
        }
        /// <summary>
        /// 获取选择的串口名字
        /// </summary>
        /// <returns></returns>
        public string GetSelectPortsName()
        {
            return this.cbSerialPorts.Text;
        }
        /// <summary>
        /// 获取选择的波特率
        /// </summary>
        /// <returns></returns>
        public int GetSelectBaudRate()
        {
            return Convert.ToInt32(this.cbBaudRate.Text);
        }

        /// <summary>
        /// 当单击的时候 加载COM串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSerialPorts_Click(object sender, EventArgs e)
        {
            cbSerialPorts.DataSource = null;
            ports = SerialPort.GetPortNames();
            cbSerialPorts.DataSource = ports;
        }

        public void SetPortAndBaudEnable(bool isEnable)
        {
            this.cbSerialPorts.Enabled = isEnable;
            this.cbBaudRate.Enabled = isEnable;
        }
    }
}
