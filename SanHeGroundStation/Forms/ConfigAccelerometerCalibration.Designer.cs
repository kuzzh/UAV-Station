namespace SanHeGroundStation.Forms
{
    partial class ConfigAccelerometerCalibration
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
            this.btnCalibrationAccel = new System.Windows.Forms.Button();
            this.btnCalibrationLevel = new System.Windows.Forms.Button();
            this.lbAccel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCalibrationAccel
            // 
            this.btnCalibrationAccel.AutoSize = true;
            this.btnCalibrationAccel.Font = new System.Drawing.Font("楷体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCalibrationAccel.Location = new System.Drawing.Point(470, 85);
            this.btnCalibrationAccel.Name = "btnCalibrationAccel";
            this.btnCalibrationAccel.Size = new System.Drawing.Size(139, 30);
            this.btnCalibrationAccel.TabIndex = 7;
            this.btnCalibrationAccel.Text = "校准加速度计";
            this.btnCalibrationAccel.UseVisualStyleBackColor = true;
            this.btnCalibrationAccel.Click += new System.EventHandler(this.btnCalibrationAccel_Click);
            // 
            // btnCalibrationLevel
            // 
            this.btnCalibrationLevel.AutoSize = true;
            this.btnCalibrationLevel.Font = new System.Drawing.Font("楷体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCalibrationLevel.Location = new System.Drawing.Point(490, 231);
            this.btnCalibrationLevel.Name = "btnCalibrationLevel";
            this.btnCalibrationLevel.Size = new System.Drawing.Size(99, 30);
            this.btnCalibrationLevel.TabIndex = 6;
            this.btnCalibrationLevel.Text = "校准水平";
            this.btnCalibrationLevel.UseVisualStyleBackColor = true;
            this.btnCalibrationLevel.Click += new System.EventHandler(this.btnCalibrationLevel_Click);
            // 
            // lbAccel
            // 
            this.lbAccel.Font = new System.Drawing.Font("楷体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbAccel.Location = new System.Drawing.Point(405, 136);
            this.lbAccel.Name = "lbAccel";
            this.lbAccel.Size = new System.Drawing.Size(268, 17);
            this.lbAccel.TabIndex = 3;
            this.lbAccel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("楷体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(302, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(474, 40);
            this.label2.TabIndex = 4;
            this.label2.Text = "水平放置您的自驾仪，设置加速度计的默认最小/最大值。这会要求您将自驾仪的每一面都放置一次。";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("楷体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(302, 178);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(474, 40);
            this.label1.TabIndex = 5;
            this.label1.Text = "水平放置您的自驾仪，设置加速度计的默认偏移，这需要您将自驾仪放置在水平的平面上。";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SanHeGroundStation.Properties.Resources.calibration01;
            this.pictureBox1.Location = new System.Drawing.Point(3, 30);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(303, 208);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // ConfigAccelerometerCalibration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(772, 270);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnCalibrationAccel);
            this.Controls.Add(this.btnCalibrationLevel);
            this.Controls.Add(this.lbAccel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ConfigAccelerometerCalibration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "加速度计校准";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCalibrationAccel;
        private System.Windows.Forms.Button btnCalibrationLevel;
        private System.Windows.Forms.Label lbAccel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;

    }
}