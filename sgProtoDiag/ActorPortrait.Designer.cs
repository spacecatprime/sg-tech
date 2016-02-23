namespace sgProtoDiag
{
    partial class ActorPortrait
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.portrait = new System.Windows.Forms.PictureBox();
            this.moodlets = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.portrait)).BeginInit();
            this.SuspendLayout();
            // 
            // portrait
            // 
            this.portrait.BackColor = System.Drawing.Color.Gainsboro;
            this.portrait.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.portrait.Location = new System.Drawing.Point(3, 3);
            this.portrait.Name = "portrait";
            this.portrait.Size = new System.Drawing.Size(100, 90);
            this.portrait.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.portrait.TabIndex = 0;
            this.portrait.TabStop = false;
            // 
            // moodlets
            // 
            this.moodlets.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.moodlets.Location = new System.Drawing.Point(3, 96);
            this.moodlets.Name = "moodlets";
            this.moodlets.Size = new System.Drawing.Size(100, 46);
            this.moodlets.TabIndex = 1;
            // 
            // ActorPortrait
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.moodlets);
            this.Controls.Add(this.portrait);
            this.Name = "ActorPortrait";
            this.Size = new System.Drawing.Size(159, 147);
            ((System.ComponentModel.ISupportInitialize)(this.portrait)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox portrait;
        private System.Windows.Forms.FlowLayoutPanel moodlets;
    }
}
