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
    public partial class ConnForm : Form
    {
        public Action ConnClick;
        public ConnForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        }
        //初始化端口名字和波特率
        private void ConnForm_Load(object sender, EventArgs e)
        {
            this.cbSerialPorts.DataSource = SerialPort.GetPortNames();
            //this.cbSerialPorts.Text = "COM3";
            int[] baudRates = { 1200, 2400, 4800, 9600, 19200, 38400, 57600, 111100, 115200, 500000, 921600, 1500000 };
            this.cbBaudRate.DataSource = baudRates;
            this.cbBaudRate.Text = baudRates[8].ToString();
        }

        private void btnConnection_Click(object sender, EventArgs e)
        {
            Global.baudRate = Convert.ToInt32(cbBaudRate.Text);
            Global.portsName = cbSerialPorts.Text.Trim();
            ConnClick();
        }

        private void cbSerialPorts_DropDown(object sender, EventArgs e)
        {
            this.cbSerialPorts.DataSource = SerialPort.GetPortNames();
            //this.cbSerialPorts.Text = "COM3";
        }


    }
}
