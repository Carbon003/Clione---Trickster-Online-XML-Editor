namespace Clione___Trickster_Online_XML_Editor
{
    partial class AboutForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Timer fadeTimer;

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            fadeTimer = new System.Windows.Forms.Timer(components);
            pictureBox1 = new PictureBox();
            lblInfo = new Label();
            btnOK = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // fadeTimer
            // 
            fadeTimer.Enabled = true;
            fadeTimer.Interval = 15;
            fadeTimer.Tick += FadeTimer_Tick;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(64, 64);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.Font = new Font("Segoe Print", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblInfo.ForeColor = Color.Salmon;
            lblInfo.Location = new Point(82, 9);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(295, 78);
            lblInfo.TabIndex = 1;
            lblInfo.Text = "Clione – Trickster Online XML Editor\nVersion 0.1\nBy Carbon – 2025-05-11";
            // 
            // btnOK
            // 
            btnOK.BackColor = Color.LightYellow;
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(295, 90);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(75, 23);
            btnOK.TabIndex = 2;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = false;
            // 
            // AboutForm
            // 
            AcceptButton = btnOK;
            BackColor = Color.Azure;
            ClientSize = new Size(382, 121);
            Controls.Add(pictureBox1);
            Controls.Add(lblInfo);
            Controls.Add(btnOK);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            Opacity = 0D;
            StartPosition = FormStartPosition.CenterParent;
            Text = "About Clione";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
