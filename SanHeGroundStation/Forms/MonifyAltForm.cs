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
    public partial class MonifyAltForm : Form
    {
        public MonifyAltForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        
        }
     
        private void btnOK_Click_1(object sender, EventArgs e)
        {

            if (txtAlt.Text == "" || !Helper.IsDouble(txtAlt.Text.Trim()))
            {
                MessageBox.Show("请填入正确的数字!");
                return;
            }
            Global.monifyAlt = Convert.ToDouble(txtAlt.Text.Trim());
            this.Close();
        }
    }
}
