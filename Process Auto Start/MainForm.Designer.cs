namespace Process_Auto_Start
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnTarget = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnLaunch = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lblTarget = new System.Windows.Forms.Label();
            this.lblLaunch = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cbxAutoClose = new System.Windows.Forms.CheckBox();
            this.cbxAutoStart = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnTarget
            // 
            this.btnTarget.Location = new System.Drawing.Point(13, 13);
            this.btnTarget.Name = "btnTarget";
            this.btnTarget.Size = new System.Drawing.Size(75, 23);
            this.btnTarget.TabIndex = 0;
            this.btnTarget.Text = "Open";
            this.btnTarget.UseVisualStyleBackColor = true;
            this.btnTarget.Click += new System.EventHandler(this.btnTarget_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(94, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Process to target:";
            // 
            // btnLaunch
            // 
            this.btnLaunch.Location = new System.Drawing.Point(13, 43);
            this.btnLaunch.Name = "btnLaunch";
            this.btnLaunch.Size = new System.Drawing.Size(75, 23);
            this.btnLaunch.TabIndex = 2;
            this.btnLaunch.Text = "Open";
            this.btnLaunch.UseVisualStyleBackColor = true;
            this.btnLaunch.Click += new System.EventHandler(this.btnLaunch_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(94, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Process to launch:";
            // 
            // lblTarget
            // 
            this.lblTarget.AutoSize = true;
            this.lblTarget.Location = new System.Drawing.Point(198, 18);
            this.lblTarget.Name = "lblTarget";
            this.lblTarget.Size = new System.Drawing.Size(0, 13);
            this.lblTarget.TabIndex = 4;
            // 
            // lblLaunch
            // 
            this.lblLaunch.AutoSize = true;
            this.lblLaunch.Location = new System.Drawing.Point(198, 48);
            this.lblLaunch.Name = "lblLaunch";
            this.lblLaunch.Size = new System.Drawing.Size(0, 13);
            this.lblLaunch.TabIndex = 5;
            // 
            // btnStart
            // 
            this.btnStart.Enabled = false;
            this.btnStart.Location = new System.Drawing.Point(13, 73);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(95, 73);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 7;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(176, 78);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(81, 13);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Waiting to start.";
            // 
            // cbxAutoClose
            // 
            this.cbxAutoClose.Checked = true;
            this.cbxAutoClose.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxAutoClose.Location = new System.Drawing.Point(309, 43);
            this.cbxAutoClose.Margin = new System.Windows.Forms.Padding(0);
            this.cbxAutoClose.Name = "cbxAutoClose";
            this.cbxAutoClose.Size = new System.Drawing.Size(76, 24);
            this.cbxAutoClose.TabIndex = 9;
            this.cbxAutoClose.Text = "Auto close";
            this.cbxAutoClose.UseVisualStyleBackColor = true;
            this.cbxAutoClose.CheckedChanged += new System.EventHandler(this.cbxAutoClose_CheckedChanged);
            // 
            // cbxAutoStart
            // 
            this.cbxAutoStart.Location = new System.Drawing.Point(309, 13);
            this.cbxAutoStart.Name = "cbxAutoStart";
            this.cbxAutoStart.Size = new System.Drawing.Size(76, 24);
            this.cbxAutoStart.TabIndex = 10;
            this.cbxAutoStart.Text = "Auto Start";
            this.cbxAutoStart.UseVisualStyleBackColor = true;
            this.cbxAutoStart.CheckedChanged += new System.EventHandler(this.cbxAutoStart_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(384, 112);
            this.Controls.Add(this.cbxAutoStart);
            this.Controls.Add(this.cbxAutoClose);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.lblLaunch);
            this.Controls.Add(this.lblTarget);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnLaunch);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnTarget);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Process Auto Start";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnTarget;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLaunch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTarget;
        private System.Windows.Forms.Label lblLaunch;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.CheckBox cbxAutoClose;
        private System.Windows.Forms.CheckBox cbxAutoStart;
    }
}

