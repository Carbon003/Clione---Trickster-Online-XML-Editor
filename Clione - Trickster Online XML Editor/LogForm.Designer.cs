namespace Clione___Trickster_Online_XML_Editor
{
    partial class LogForm
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
            lstbxLog = new ListBox();
            SuspendLayout();
            // 
            // lstbxLog
            // 
            lstbxLog.BackColor = SystemColors.ControlDarkDark;
            lstbxLog.ForeColor = Color.Cyan;
            lstbxLog.FormattingEnabled = true;
            lstbxLog.ItemHeight = 15;
            lstbxLog.Location = new Point(12, 12);
            lstbxLog.Name = "lstbxLog";
            lstbxLog.Size = new Size(776, 424);
            lstbxLog.TabIndex = 0;
            // 
            // LogForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(800, 450);
            Controls.Add(lstbxLog);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "LogForm";
            Text = "Log";
            ResumeLayout(false);
        }

        #endregion

        private ListBox lstbxLog;
    }
}