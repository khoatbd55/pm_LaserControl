﻿namespace LaserControl
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
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnFind2 = new DevExpress.XtraEditors.SimpleButton();
            this.btnFind = new DevExpress.XtraEditors.SimpleButton();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.btnGetInfos = new DevExpress.XtraEditors.SimpleButton();
            this.btnReadPos = new DevExpress.XtraEditors.SimpleButton();
            this.btnOpen = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.listLog = new System.Windows.Forms.ListBox();
            this.panelControl = new DevExpress.XtraEditors.PanelControl();
            this.btnReadVersion = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl)).BeginInit();
            this.panelControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Appearance.BackColor = System.Drawing.Color.Silver;
            this.panelControl1.Appearance.Options.UseBackColor = true;
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.panelControl);
            this.panelControl1.Controls.Add(this.btnFind2);
            this.panelControl1.Controls.Add(this.btnFind);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(6);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1321, 224);
            this.panelControl1.TabIndex = 0;
            // 
            // btnFind2
            // 
            this.btnFind2.Location = new System.Drawing.Point(232, 12);
            this.btnFind2.Name = "btnFind2";
            this.btnFind2.Size = new System.Drawing.Size(164, 61);
            this.btnFind2.TabIndex = 5;
            this.btnFind2.Text = "Find2";
            this.btnFind2.Click += new System.EventHandler(this.btnFind2_Click);
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(24, 12);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(164, 61);
            this.btnFind.TabIndex = 4;
            this.btnFind.Text = "Find";
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(220, 22);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(164, 61);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnGetInfos
            // 
            this.btnGetInfos.Location = new System.Drawing.Point(653, 22);
            this.btnGetInfos.Name = "btnGetInfos";
            this.btnGetInfos.Size = new System.Drawing.Size(164, 61);
            this.btnGetInfos.TabIndex = 2;
            this.btnGetInfos.Text = "Get Infos";
            this.btnGetInfos.Click += new System.EventHandler(this.btnGetInfos_Click);
            // 
            // btnReadPos
            // 
            this.btnReadPos.Location = new System.Drawing.Point(445, 22);
            this.btnReadPos.Name = "btnReadPos";
            this.btnReadPos.Size = new System.Drawing.Size(164, 61);
            this.btnReadPos.TabIndex = 1;
            this.btnReadPos.Text = "Read Pos";
            this.btnReadPos.Click += new System.EventHandler(this.btnReadPos_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(12, 22);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(164, 61);
            this.btnOpen.TabIndex = 0;
            this.btnOpen.Text = "Open";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // panelControl2
            // 
            this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl2.Controls.Add(this.listLog);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl2.Location = new System.Drawing.Point(0, 224);
            this.panelControl2.Margin = new System.Windows.Forms.Padding(4);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(1321, 477);
            this.panelControl2.TabIndex = 1;
            // 
            // listLog
            // 
            this.listLog.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.listLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.listLog.FormattingEnabled = true;
            this.listLog.ItemHeight = 25;
            this.listLog.Location = new System.Drawing.Point(0, 0);
            this.listLog.Name = "listLog";
            this.listLog.Size = new System.Drawing.Size(1321, 477);
            this.listLog.TabIndex = 0;
            // 
            // panelControl
            // 
            this.panelControl.Controls.Add(this.btnReadVersion);
            this.panelControl.Controls.Add(this.btnClose);
            this.panelControl.Controls.Add(this.btnGetInfos);
            this.panelControl.Controls.Add(this.btnReadPos);
            this.panelControl.Controls.Add(this.btnOpen);
            this.panelControl.Enabled = false;
            this.panelControl.Location = new System.Drawing.Point(12, 102);
            this.panelControl.Name = "panelControl";
            this.panelControl.Size = new System.Drawing.Size(1297, 110);
            this.panelControl.TabIndex = 6;
            // 
            // btnReadVersion
            // 
            this.btnReadVersion.Location = new System.Drawing.Point(839, 22);
            this.btnReadVersion.Name = "btnReadVersion";
            this.btnReadVersion.Size = new System.Drawing.Size(164, 61);
            this.btnReadVersion.TabIndex = 4;
            this.btnReadVersion.Text = "Read Version";
            this.btnReadVersion.Click += new System.EventHandler(this.btnReadVersion_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1321, 701);
            this.Controls.Add(this.panelControl2);
            this.Controls.Add(this.panelControl1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl)).EndInit();
            this.panelControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private System.Windows.Forms.ListBox listLog;
        private DevExpress.XtraEditors.SimpleButton btnGetInfos;
        private DevExpress.XtraEditors.SimpleButton btnReadPos;
        private DevExpress.XtraEditors.SimpleButton btnOpen;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraEditors.SimpleButton btnFind;
        private DevExpress.XtraEditors.SimpleButton btnFind2;
        private DevExpress.XtraEditors.PanelControl panelControl;
        private DevExpress.XtraEditors.SimpleButton btnReadVersion;
    }
}

