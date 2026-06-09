using System;
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
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }
        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            if (Opacity < 1.0)
                Opacity += 0.05;
            else
                fadeTimer.Stop();
        }


    }
}
