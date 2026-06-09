using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clione___Trickster_Online_XML_Editor
{
    public partial class RenameForm : Form
    {
        private readonly string _oldName;
        private readonly string _folder;
        public string NewName;
        private System.Windows.Forms.Timer _fadeTimer;

        public RenameForm(string oldName, string folder)
        {

            InitializeComponent();

            _oldName = oldName;
            _folder = folder;

            lblFileName.Text = _oldName;
            var fileNameWithoutExtension = _oldName.Split(".", StringSplitOptions.None);
            txtbxNewName.Text = fileNameWithoutExtension[0];

            this.Opacity = 0;

            // set up the fade timer
            _fadeTimer = new System.Windows.Forms.Timer { Interval = 15 };      // 15 ms per tick
            _fadeTimer.Tick += FadeTimer_Tick;
            _fadeTimer.Start();

            txtbxNewName.Focus();
            txtbxNewName.SelectAll();

        }



        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            if (Opacity < 1.0)
                Opacity += 0.05;   // adjust step to taste
            else
                _fadeTimer.Stop();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string newName = txtbxNewName.Text.Trim();

            // 1) Basic empty check
            if (string.IsNullOrEmpty(newName))
            {
                MessageBox.Show("Please enter a new name.", "Validation",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2) Don’t allow the .xml extension in the textbox
            if (newName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Do not include the .xml extension; it'll be added automatically.",
                                "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3) Check for any characters Windows forbids in filenames
            char[] invalid = Path.GetInvalidFileNameChars();
            var bad = newName.FirstOrDefault(c => invalid.Contains(c));
            if (bad != default(char))
            {
                MessageBox.Show($"The character '{bad}' is not allowed in filenames.",
                                "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4) (Optional) Prevent reserved names like CON, PRN, AUX, etc.
            string[] reserved = {
                "CON","PRN","AUX","NUL",
                "COM1","COM2","COM3","COM4","COM5","COM6","COM7","COM8","COM9",
                "LPT1","LPT2","LPT3","LPT4","LPT5","LPT6","LPT7","LPT8","LPT9"
                };
            if (reserved.Contains(newName, StringComparer.OrdinalIgnoreCase))
            {
                MessageBox.Show($"'{newName}' is a reserved name on Windows and cannot be used.",
                                "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 5) All good – perform the rename
            string oldPath = Path.Combine(_folder, _oldName);
            string newPath = Path.Combine(_folder, newName + ".xml");
            try
            {
                File.Move(oldPath, newPath);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Rename failed:\n{ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
