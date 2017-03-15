﻿using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SanHeGroundStation.Forms.ProgressReporterSphereUsing
{
    partial class ProgressReporterDialogue
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
            this.components = new System.ComponentModel.Container();
            this.lblProgressMessage = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.imgWarning = new System.Windows.Forms.PictureBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.progressBar1 = new SanHeGroundStation.Forms.ProgressReporterSphereUsing.MyProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.imgWarning)).BeginInit();
            this.SuspendLayout();
            // 
            // lblProgressMessage
            // 
            this.lblProgressMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProgressMessage.Location = new System.Drawing.Point(13, 13);
            this.lblProgressMessage.Name = "lblProgressMessage";
            this.lblProgressMessage.Size = new System.Drawing.Size(275, 74);
            this.lblProgressMessage.TabIndex = 2;
            this.lblProgressMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(240, 90);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(47, 12);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "详情...";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // imgWarning
            // 
            this.imgWarning.Image = global::SanHeGroundStation.Properties.Resources.iconWarning48;
            this.imgWarning.Location = new System.Drawing.Point(13, 22);
            this.imgWarning.Name = "imgWarning";
            this.imgWarning.Size = new System.Drawing.Size(48, 48);
            this.imgWarning.TabIndex = 1;
            this.imgWarning.TabStop = false;
            this.imgWarning.Visible = false;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(213, 109);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "关闭";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(132, 109);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.BackColor = System.Drawing.Color.Transparent;
            this.progressBar1.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(167)))), ((int)(((byte)(42)))));
            this.progressBar1.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(139)))), ((int)(((byte)(26)))));
            this.progressBar1.Location = new System.Drawing.Point(11, 90);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(171)))), ((int)(((byte)(112)))));
            this.progressBar1.Size = new System.Drawing.Size(261, 13);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 3;
            this.progressBar1.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(54)))), ((int)(((byte)(8)))));
            // 
            // ProgressReporterDialogue
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(306, 144);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.imgWarning);
            this.Controls.Add(this.lblProgressMessage);
            this.Controls.Add(this.progressBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressReporterDialogue";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "进程";
            this.Load += new System.EventHandler(this.ProgressReporterDialogue_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imgWarning)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MyProgressBar progressBar1;
        private System.Windows.Forms.Label lblProgressMessage;
        private PictureBox imgWarning;
        private LinkLabel linkLabel1;
        private Timer timer1;
        private Button btnClose;
        public Button btnCancel;
    }
}