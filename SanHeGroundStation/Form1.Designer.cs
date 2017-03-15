namespace SanHeGroundStation
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.hocy_Curve1 = new hocylan_Curve.hocy_Curve();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(600, 500);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(606, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // hocy_Curve1
            // 
            this.hocy_Curve1.AxisColor = System.Drawing.Color.Blue;
            this.hocy_Curve1.AxisTextColor = System.Drawing.Color.Black;
            this.hocy_Curve1.BgColor = System.Drawing.Color.DeepSkyBlue;
            this.hocy_Curve1.BorderColor = System.Drawing.Color.Black;
            this.hocy_Curve1.CurveColor = System.Drawing.Color.Red;
            this.hocy_Curve1.GriddingColor = System.Drawing.Color.Blue;
            this.hocy_Curve1.height = 400;
            this.hocy_Curve1.Keys = new string[] {
        "1",
        "2",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8",
        "9",
        "10",
        "11",
        "12",
        "13",
        "14",
        "15",
        "16",
        "17",
        "18",
        "19",
        "20",
        "21",
        "22",
        "23",
        "24",
        "25",
        "26",
        "27",
        "28",
        "29",
        "30",
        "31"};
            this.hocy_Curve1.LineColor = new System.Drawing.Color[] {
        System.Drawing.Color.Yellow,
        System.Drawing.Color.Red,
        System.Drawing.Color.White,
        System.Drawing.Color.Green,
        System.Drawing.Color.Orange,
        System.Drawing.Color.BlueViolet};
            this.hocy_Curve1.LineValueAll = null;
            this.hocy_Curve1.Location = new System.Drawing.Point(606, 53);
            this.hocy_Curve1.Name = "hocy_Curve1";
            this.hocy_Curve1.Size = new System.Drawing.Size(534, 422);
            this.hocy_Curve1.SliceColor = System.Drawing.Color.Black;
            this.hocy_Curve1.SliceTextColor = System.Drawing.Color.Black;
            this.hocy_Curve1.TabIndex = 2;
            this.hocy_Curve1.Tension = 0F;
            this.hocy_Curve1.TextColor = System.Drawing.Color.Black;
            this.hocy_Curve1.Title = "曲线图";
            this.hocy_Curve1.width = 624;
            this.hocy_Curve1.XAxisText = "X轴说明文字";
            this.hocy_Curve1.XInScaleNum = 1;
            this.hocy_Curve1.XPointScaleNum = 15;
            this.hocy_Curve1.XSlice = 29F;
            this.hocy_Curve1.XUnit = "";
            this.hocy_Curve1.YAxisText = "Y轴说明文字";
            this.hocy_Curve1.YInSliceNum = 5;
            this.hocy_Curve1.YPointScaleNum = 5;
            this.hocy_Curve1.YSlice = 59F;
            this.hocy_Curve1.YSliceBegin = 10F;
            this.hocy_Curve1.YSliceEnd = 200F;
            this.hocy_Curve1.YSliceValue = 40F;
            this.hocy_Curve1.YUnit = "KV";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(705, 11);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1152, 487);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.hocy_Curve1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button1;
        private hocylan_Curve.hocy_Curve hocy_Curve1;
        private System.Windows.Forms.Button button2;
    }
}