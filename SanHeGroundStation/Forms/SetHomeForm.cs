using SanHeGroundStation.Tools;
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
    public partial class SetHomeForm : Form
    {
        public SetHomeForm()
        {
            InitializeComponent();
            txtAlt.Text = Global.home.Alt.ToString();
            //txtLat.Text = Global.home.Lat.ToString();
            //txtLng.Text = Global.home.Lng.ToString();

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtAlt.Text == "" || !Helper.IsDouble(txtAlt.Text.Trim()))
            {
                MessageBox.Show("请填入正确的数字!");
                return;
            }
            Global.home.Alt = Convert.ToDouble(txtAlt.Text.Trim());
            //Global.home.Lng = Convert.ToDouble(txtLng.Text.Trim());
            //Global.home.Lat = Convert.ToDouble(txtLat.Text.Trim());
            this.Close();
        }
    }
}
