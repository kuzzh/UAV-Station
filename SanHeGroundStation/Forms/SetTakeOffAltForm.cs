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
    public partial class SetTakeOffAltForm : Form
    {
        public SetTakeOffAltForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!Helper.IsDouble(this.txtTakeOffAlt.Text.Trim()))
            {
                MessageBox.Show("输入高度错误!");
                return;
            }
            else if (Convert.ToSingle(txtTakeOffAlt.Text.Trim()) <= 0)
            {
                MessageBox.Show("输入高度无效!");
                return;
            }
            else
            {
                Global.takeOffAlt = Convert.ToSingle(txtTakeOffAlt.Text.Trim());
                this.Close();
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
