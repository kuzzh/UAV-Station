namespace SanHeGroundStation.Forms
{
    partial class GeoFence
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
            this.btnSave = new System.Windows.Forms.Button();
            this.fence_RTL = new System.Windows.Forms.NumericUpDown();
            this.fence_Radius = new System.Windows.Forms.NumericUpDown();
            this.fence_Alt = new System.Windows.Forms.NumericUpDown();
            this.fence_Action = new System.Windows.Forms.ComboBox();
            this.fence_type = new System.Windows.Forms.ComboBox();
            this.fence_Check = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.fence_RTL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fence_Radius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fence_Alt)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSave.Location = new System.Drawing.Point(76, 204);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(77, 32);
            this.btnSave.TabIndex = 50;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // fence_RTL
            // 
            this.fence_RTL.Location = new System.Drawing.Point(94, 175);
            this.fence_RTL.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.fence_RTL.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.fence_RTL.Name = "fence_RTL";
            this.fence_RTL.Size = new System.Drawing.Size(120, 21);
            this.fence_RTL.TabIndex = 49;
            this.fence_RTL.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // fence_Radius
            // 
            this.fence_Radius.Location = new System.Drawing.Point(94, 142);
            this.fence_Radius.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.fence_Radius.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.fence_Radius.Name = "fence_Radius";
            this.fence_Radius.Size = new System.Drawing.Size(120, 21);
            this.fence_Radius.TabIndex = 48;
            this.fence_Radius.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // fence_Alt
            // 
            this.fence_Alt.Location = new System.Drawing.Point(94, 109);
            this.fence_Alt.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.fence_Alt.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.fence_Alt.Name = "fence_Alt";
            this.fence_Alt.Size = new System.Drawing.Size(120, 21);
            this.fence_Alt.TabIndex = 47;
            this.fence_Alt.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // fence_Action
            // 
            this.fence_Action.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fence_Action.FormattingEnabled = true;
            this.fence_Action.Location = new System.Drawing.Point(94, 77);
            this.fence_Action.Name = "fence_Action";
            this.fence_Action.Size = new System.Drawing.Size(121, 20);
            this.fence_Action.TabIndex = 46;
            // 
            // fence_type
            // 
            this.fence_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fence_type.FormattingEnabled = true;
            this.fence_type.Location = new System.Drawing.Point(94, 45);
            this.fence_type.Name = "fence_type";
            this.fence_type.Size = new System.Drawing.Size(121, 20);
            this.fence_type.TabIndex = 45;
            // 
            // fence_Check
            // 
            this.fence_Check.AutoSize = true;
            this.fence_Check.Location = new System.Drawing.Point(94, 17);
            this.fence_Check.Name = "fence_Check";
            this.fence_Check.Size = new System.Drawing.Size(48, 16);
            this.fence_Check.TabIndex = 44;
            this.fence_Check.Text = "启用";
            this.fence_Check.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 184);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 12);
            this.label6.TabIndex = 43;
            this.label6.Text = "RTL高度：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 151);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 42;
            this.label5.Text = "最大半径：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 41;
            this.label4.Text = "最大高度：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 40;
            this.label3.Text = "动作：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 39;
            this.label2.Text = "类型：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 38;
            this.label1.Text = "启用：";
            // 
            // GeoFence
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 246);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.fence_RTL);
            this.Controls.Add(this.fence_Radius);
            this.Controls.Add(this.fence_Alt);
            this.Controls.Add(this.fence_Action);
            this.Controls.Add(this.fence_type);
            this.Controls.Add(this.fence_Check);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.ImeMode = System.Windows.Forms.ImeMode.On;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GeoFence";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "设置电子围栏";
            ((System.ComponentModel.ISupportInitialize)(this.fence_RTL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fence_Radius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fence_Alt)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.NumericUpDown fence_RTL;
        private System.Windows.Forms.NumericUpDown fence_Radius;
        private System.Windows.Forms.NumericUpDown fence_Alt;
        private System.Windows.Forms.ComboBox fence_Action;
        private System.Windows.Forms.ComboBox fence_type;
        private System.Windows.Forms.CheckBox fence_Check;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}