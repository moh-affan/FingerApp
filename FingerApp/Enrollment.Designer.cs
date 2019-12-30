namespace FingerApp
{
    partial class Enrollment
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
            this.picFinger = new System.Windows.Forms.PictureBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picFinger)).BeginInit();
            this.SuspendLayout();
            // 
            // picFinger
            // 
            this.picFinger.BackColor = System.Drawing.SystemColors.ControlDark;
            this.picFinger.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picFinger.Location = new System.Drawing.Point(13, 40);
            this.picFinger.Name = "picFinger";
            this.picFinger.Size = new System.Drawing.Size(282, 282);
            this.picFinger.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picFinger.TabIndex = 0;
            this.picFinger.TabStop = false;
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.txtLog.Location = new System.Drawing.Point(13, 347);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(282, 106);
            this.txtLog.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(198, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Tempelkan Sidik Jari Anda 4 kali";
            // 
            // Enrollment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 465);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.picFinger);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Enrollment";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Enrollment";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Enrollment_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picFinger)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picFinger;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label label1;
    }
}

