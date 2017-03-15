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
    public partial class SetDefaultAltForm : Form
    {
        public SetDefaultAltForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.txtAlt.Text = Global.defaultAlt.ToString();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(txtAlt.Text==""||!Helper.IsDouble(txtAlt.Text.Trim()))
            {
                MessageBox.Show("请填入正确的数字!");
                return;
            }
            Global.defaultAlt = Convert.ToDouble(txtAlt.Text.Trim());
            this.Close();
        }

        private void txtAlt_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
