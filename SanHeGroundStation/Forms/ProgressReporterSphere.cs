using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SanHeGroundStation.Forms.ProgressReporterSphereUsing;

namespace SanHeGroundStation.Forms
{
    public partial class ProgressReporterSphere : ProgressReporterDialogue
    {
        public Sphere sphere1;
        public Sphere sphere2;
        private Label label1;
        private Button button1;


        public bool autoaccept = true;

        public ProgressReporterSphere()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.sphere1 = new SanHeGroundStation.Forms.ProgressReporterSphereUsing.Sphere();
            this.sphere2 = new SanHeGroundStation.Forms.ProgressReporterSphereUsing.Sphere();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(527, 402);
            // 
            // sphere1
            // 
            this.sphere1.BackColor = System.Drawing.Color.Black;
            this.sphere1.Location = new System.Drawing.Point(31, 127);
            this.sphere1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.sphere1.Name = "sphere1";
            this.sphere1.rotatewithdata = true;
            this.sphere1.Size = new System.Drawing.Size(263, 263);
            this.sphere1.TabIndex = 6;
            this.sphere1.VSync = false;
            // 
            // sphere2
            // 
            this.sphere2.BackColor = System.Drawing.Color.Black;
            this.sphere2.Location = new System.Drawing.Point(339, 129);
            this.sphere2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.sphere2.Name = "sphere2";
            this.sphere2.rotatewithdata = true;
            this.sphere2.Size = new System.Drawing.Size(263, 263);
            this.sphere2.TabIndex = 7;
            this.sphere2.VSync = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(310, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(221, 24);
            this.label1.TabIndex = 8;
            this.label1.Text = "该功能目前正在测试中！\r\n请在旋转结束后自行点击右侧 关闭 按钮";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(537, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "关闭";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ProgressReporterSphere
            // 
            this.ClientSize = new System.Drawing.Size(639, 436);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sphere2);
            this.Controls.Add(this.sphere1);
            this.Name = "ProgressReporterSphere";
            this.Text = "Progress";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProgressReporterSphere_FormClosing);
            this.Load += new System.EventHandler(this.ProgressReporterSphere_Load);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.sphere1, 0);
            this.Controls.SetChildIndex(this.sphere2, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.button1, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ProgressReporterSphere_Load(object sender, EventArgs e)
        {
            SanHeGroundStation.Forms.ProgressReporterSphereUsing.MagCalib.boostart = true;
        }

        private void ProgressReporterSphere_FormClosing(object sender, FormClosingEventArgs e)
        {
            SanHeGroundStation.Forms.ProgressReporterSphereUsing.MagCalib.boostart = false;
        }
    }
}
