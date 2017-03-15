namespace SanHeGroundStation.Forms
{
    partial class InitForm
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
            this.btn_SaveModes = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.Cb_ss5 = new System.Windows.Forms.CheckBox();
            this.Cb_ss6 = new System.Windows.Forms.CheckBox();
            this.Cb_simple6 = new System.Windows.Forms.CheckBox();
            this.Cb_simple5 = new System.Windows.Forms.CheckBox();
            this.Cb_ss4 = new System.Windows.Forms.CheckBox();
            this.Cb_simple3 = new System.Windows.Forms.CheckBox();
            this.Cb_ss3 = new System.Windows.Forms.CheckBox();
            this.Cb_simple4 = new System.Windows.Forms.CheckBox();
            this.Cb_ss2 = new System.Windows.Forms.CheckBox();
            this.Cb_simple2 = new System.Windows.Forms.CheckBox();
            this.Cb_ss1 = new System.Windows.Forms.CheckBox();
            this.Cb_simple1 = new System.Windows.Forms.CheckBox();
            this.FlightMode6 = new System.Windows.Forms.ComboBox();
            this.FlightMode5 = new System.Windows.Forms.ComboBox();
            this.FlightMode4 = new System.Windows.Forms.ComboBox();
            this.FlightMode3 = new System.Windows.Forms.ComboBox();
            this.FlightMode2 = new System.Windows.Forms.ComboBox();
            this.FlightMode1 = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.CurrentPWM = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.CurrentMode = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_SaveModes
            // 
            this.btn_SaveModes.Location = new System.Drawing.Point(202, 222);
            this.btn_SaveModes.Name = "btn_SaveModes";
            this.btn_SaveModes.Size = new System.Drawing.Size(123, 41);
            this.btn_SaveModes.TabIndex = 70;
            this.btn_SaveModes.Text = "保存模式";
            this.btn_SaveModes.UseVisualStyleBackColor = true;
            this.btn_SaveModes.Click += new System.EventHandler(this.btn_SaveModes_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(402, 185);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(65, 12);
            this.label14.TabIndex = 69;
            this.label14.Text = "PWM 1750 +";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(402, 161);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(95, 12);
            this.label13.TabIndex = 68;
            this.label13.Text = "PWM 1621 - 1749";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(402, 136);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(95, 12);
            this.label12.TabIndex = 67;
            this.label12.Text = "PWM 1491 - 1620";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(402, 108);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(95, 12);
            this.label11.TabIndex = 66;
            this.label11.Text = "PWM 1361 - 1490";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(402, 81);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(95, 12);
            this.label10.TabIndex = 65;
            this.label10.Text = "PWM 1231 - 1360";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(402, 59);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 12);
            this.label9.TabIndex = 64;
            this.label9.Text = "PWM 0 - 1230";
            // 
            // Cb_ss5
            // 
            this.Cb_ss5.AutoSize = true;
            this.Cb_ss5.Location = new System.Drawing.Point(302, 156);
            this.Cb_ss5.Name = "Cb_ss5";
            this.Cb_ss5.Size = new System.Drawing.Size(84, 16);
            this.Cb_ss5.TabIndex = 63;
            this.Cb_ss5.Text = "超简单模式";
            this.Cb_ss5.UseVisualStyleBackColor = true;
            // 
            // Cb_ss6
            // 
            this.Cb_ss6.AutoSize = true;
            this.Cb_ss6.Location = new System.Drawing.Point(302, 181);
            this.Cb_ss6.Name = "Cb_ss6";
            this.Cb_ss6.Size = new System.Drawing.Size(84, 16);
            this.Cb_ss6.TabIndex = 62;
            this.Cb_ss6.Text = "超简单模式";
            this.Cb_ss6.UseVisualStyleBackColor = true;
            // 
            // Cb_simple6
            // 
            this.Cb_simple6.AutoSize = true;
            this.Cb_simple6.Location = new System.Drawing.Point(202, 181);
            this.Cb_simple6.Name = "Cb_simple6";
            this.Cb_simple6.Size = new System.Drawing.Size(72, 16);
            this.Cb_simple6.TabIndex = 61;
            this.Cb_simple6.Text = "简单模式";
            this.Cb_simple6.UseVisualStyleBackColor = true;
            // 
            // Cb_simple5
            // 
            this.Cb_simple5.AutoSize = true;
            this.Cb_simple5.Location = new System.Drawing.Point(202, 156);
            this.Cb_simple5.Name = "Cb_simple5";
            this.Cb_simple5.Size = new System.Drawing.Size(72, 16);
            this.Cb_simple5.TabIndex = 60;
            this.Cb_simple5.Text = "简单模式";
            this.Cb_simple5.UseVisualStyleBackColor = true;
            // 
            // Cb_ss4
            // 
            this.Cb_ss4.AutoSize = true;
            this.Cb_ss4.Location = new System.Drawing.Point(302, 132);
            this.Cb_ss4.Name = "Cb_ss4";
            this.Cb_ss4.Size = new System.Drawing.Size(84, 16);
            this.Cb_ss4.TabIndex = 59;
            this.Cb_ss4.Text = "超简单模式";
            this.Cb_ss4.UseVisualStyleBackColor = true;
            // 
            // Cb_simple3
            // 
            this.Cb_simple3.AutoSize = true;
            this.Cb_simple3.Location = new System.Drawing.Point(202, 108);
            this.Cb_simple3.Name = "Cb_simple3";
            this.Cb_simple3.Size = new System.Drawing.Size(72, 16);
            this.Cb_simple3.TabIndex = 58;
            this.Cb_simple3.Text = "简单模式";
            this.Cb_simple3.UseVisualStyleBackColor = true;
            // 
            // Cb_ss3
            // 
            this.Cb_ss3.AutoSize = true;
            this.Cb_ss3.Location = new System.Drawing.Point(302, 107);
            this.Cb_ss3.Name = "Cb_ss3";
            this.Cb_ss3.Size = new System.Drawing.Size(84, 16);
            this.Cb_ss3.TabIndex = 57;
            this.Cb_ss3.Text = "超简单模式";
            this.Cb_ss3.UseVisualStyleBackColor = true;
            // 
            // Cb_simple4
            // 
            this.Cb_simple4.AutoSize = true;
            this.Cb_simple4.Location = new System.Drawing.Point(202, 132);
            this.Cb_simple4.Name = "Cb_simple4";
            this.Cb_simple4.Size = new System.Drawing.Size(72, 16);
            this.Cb_simple4.TabIndex = 56;
            this.Cb_simple4.Text = "简单模式";
            this.Cb_simple4.UseVisualStyleBackColor = true;
            // 
            // Cb_ss2
            // 
            this.Cb_ss2.AutoSize = true;
            this.Cb_ss2.Location = new System.Drawing.Point(302, 83);
            this.Cb_ss2.Name = "Cb_ss2";
            this.Cb_ss2.Size = new System.Drawing.Size(84, 16);
            this.Cb_ss2.TabIndex = 55;
            this.Cb_ss2.Text = "超简单模式";
            this.Cb_ss2.UseVisualStyleBackColor = true;
            // 
            // Cb_simple2
            // 
            this.Cb_simple2.AutoSize = true;
            this.Cb_simple2.Location = new System.Drawing.Point(202, 83);
            this.Cb_simple2.Name = "Cb_simple2";
            this.Cb_simple2.Size = new System.Drawing.Size(72, 16);
            this.Cb_simple2.TabIndex = 54;
            this.Cb_simple2.Text = "简单模式";
            this.Cb_simple2.UseVisualStyleBackColor = true;
            // 
            // Cb_ss1
            // 
            this.Cb_ss1.AutoSize = true;
            this.Cb_ss1.Location = new System.Drawing.Point(302, 57);
            this.Cb_ss1.Name = "Cb_ss1";
            this.Cb_ss1.Size = new System.Drawing.Size(84, 16);
            this.Cb_ss1.TabIndex = 53;
            this.Cb_ss1.Text = "超简单模式";
            this.Cb_ss1.UseVisualStyleBackColor = true;
            // 
            // Cb_simple1
            // 
            this.Cb_simple1.AutoSize = true;
            this.Cb_simple1.Location = new System.Drawing.Point(202, 57);
            this.Cb_simple1.Name = "Cb_simple1";
            this.Cb_simple1.Size = new System.Drawing.Size(72, 16);
            this.Cb_simple1.TabIndex = 52;
            this.Cb_simple1.Text = "简单模式";
            this.Cb_simple1.UseVisualStyleBackColor = true;
            // 
            // FlightMode6
            // 
            this.FlightMode6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FlightMode6.FormattingEnabled = true;
            this.FlightMode6.Location = new System.Drawing.Point(88, 177);
            this.FlightMode6.Name = "FlightMode6";
            this.FlightMode6.Size = new System.Drawing.Size(101, 20);
            this.FlightMode6.TabIndex = 51;
            this.FlightMode6.SelectedIndexChanged += new System.EventHandler(this.Mode_Changed);
            // 
            // FlightMode5
            // 
            this.FlightMode5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FlightMode5.FormattingEnabled = true;
            this.FlightMode5.Location = new System.Drawing.Point(88, 152);
            this.FlightMode5.Name = "FlightMode5";
            this.FlightMode5.Size = new System.Drawing.Size(101, 20);
            this.FlightMode5.TabIndex = 50;
            this.FlightMode5.SelectedIndexChanged += new System.EventHandler(this.Mode_Changed);
            // 
            // FlightMode4
            // 
            this.FlightMode4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FlightMode4.FormattingEnabled = true;
            this.FlightMode4.Location = new System.Drawing.Point(88, 128);
            this.FlightMode4.Name = "FlightMode4";
            this.FlightMode4.Size = new System.Drawing.Size(101, 20);
            this.FlightMode4.TabIndex = 49;
            this.FlightMode4.SelectedIndexChanged += new System.EventHandler(this.Mode_Changed);
            // 
            // FlightMode3
            // 
            this.FlightMode3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FlightMode3.FormattingEnabled = true;
            this.FlightMode3.Location = new System.Drawing.Point(88, 104);
            this.FlightMode3.Name = "FlightMode3";
            this.FlightMode3.Size = new System.Drawing.Size(101, 20);
            this.FlightMode3.TabIndex = 48;
            this.FlightMode3.SelectedIndexChanged += new System.EventHandler(this.Mode_Changed);
            // 
            // FlightMode2
            // 
            this.FlightMode2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FlightMode2.FormattingEnabled = true;
            this.FlightMode2.Location = new System.Drawing.Point(88, 81);
            this.FlightMode2.Name = "FlightMode2";
            this.FlightMode2.Size = new System.Drawing.Size(101, 20);
            this.FlightMode2.TabIndex = 47;
            this.FlightMode2.SelectedIndexChanged += new System.EventHandler(this.Mode_Changed);
            // 
            // FlightMode1
            // 
            this.FlightMode1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FlightMode1.FormattingEnabled = true;
            this.FlightMode1.Location = new System.Drawing.Point(88, 58);
            this.FlightMode1.Name = "FlightMode1";
            this.FlightMode1.Size = new System.Drawing.Size(101, 20);
            this.FlightMode1.TabIndex = 46;
            this.FlightMode1.SelectedIndexChanged += new System.EventHandler(this.Mode_Changed);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(22, 181);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 12);
            this.label8.TabIndex = 45;
            this.label8.Text = "飞行模式6";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(22, 156);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 12);
            this.label7.TabIndex = 44;
            this.label7.Text = "飞行模式5";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 132);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 12);
            this.label6.TabIndex = 43;
            this.label6.Text = "飞行模式4";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 108);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 42;
            this.label5.Text = "飞行模式3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 41;
            this.label4.Text = "飞行模式2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 40;
            this.label3.Text = "飞行模式1";
            // 
            // CurrentPWM
            // 
            this.CurrentPWM.AutoSize = true;
            this.CurrentPWM.Location = new System.Drawing.Point(91, 46);
            this.CurrentPWM.Name = "CurrentPWM";
            this.CurrentPWM.Size = new System.Drawing.Size(29, 12);
            this.CurrentPWM.TabIndex = 39;
            this.CurrentPWM.Text = "“”";
            this.CurrentPWM.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 38;
            this.label2.Text = "当前PWM ：";
            this.label2.Visible = false;
            // 
            // CurrentMode
            // 
            this.CurrentMode.AutoSize = true;
            this.CurrentMode.Location = new System.Drawing.Point(91, 20);
            this.CurrentMode.Name = "CurrentMode";
            this.CurrentMode.Size = new System.Drawing.Size(29, 12);
            this.CurrentMode.TabIndex = 37;
            this.CurrentMode.Text = "“”";
            this.CurrentMode.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 36;
            this.label1.Text = "当前模式：";
            this.label1.Visible = false;
            // 
            // InitForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 275);
            this.Controls.Add(this.btn_SaveModes);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.Cb_ss5);
            this.Controls.Add(this.Cb_ss6);
            this.Controls.Add(this.Cb_simple6);
            this.Controls.Add(this.Cb_simple5);
            this.Controls.Add(this.Cb_ss4);
            this.Controls.Add(this.Cb_simple3);
            this.Controls.Add(this.Cb_ss3);
            this.Controls.Add(this.Cb_simple4);
            this.Controls.Add(this.Cb_ss2);
            this.Controls.Add(this.Cb_simple2);
            this.Controls.Add(this.Cb_ss1);
            this.Controls.Add(this.Cb_simple1);
            this.Controls.Add(this.FlightMode6);
            this.Controls.Add(this.FlightMode5);
            this.Controls.Add(this.FlightMode4);
            this.Controls.Add(this.FlightMode3);
            this.Controls.Add(this.FlightMode2);
            this.Controls.Add(this.FlightMode1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.CurrentPWM);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CurrentMode);
            this.Controls.Add(this.label1);
            this.Name = "InitForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "飞行模式设置";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_SaveModes;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox Cb_ss5;
        private System.Windows.Forms.CheckBox Cb_ss6;
        private System.Windows.Forms.CheckBox Cb_simple6;
        private System.Windows.Forms.CheckBox Cb_simple5;
        private System.Windows.Forms.CheckBox Cb_ss4;
        private System.Windows.Forms.CheckBox Cb_simple3;
        private System.Windows.Forms.CheckBox Cb_ss3;
        private System.Windows.Forms.CheckBox Cb_simple4;
        private System.Windows.Forms.CheckBox Cb_ss2;
        private System.Windows.Forms.CheckBox Cb_simple2;
        private System.Windows.Forms.CheckBox Cb_ss1;
        private System.Windows.Forms.CheckBox Cb_simple1;
        private System.Windows.Forms.ComboBox FlightMode6;
        private System.Windows.Forms.ComboBox FlightMode5;
        private System.Windows.Forms.ComboBox FlightMode4;
        private System.Windows.Forms.ComboBox FlightMode3;
        private System.Windows.Forms.ComboBox FlightMode2;
        private System.Windows.Forms.ComboBox FlightMode1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label CurrentPWM;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label CurrentMode;
        private System.Windows.Forms.Label label1;
    }
}