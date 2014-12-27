using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserInterface
{
    public partial class WelcomeScreen : Form
    {
        private Timer timer;

        public WelcomeScreen()
        {
            InitializeComponent();
        }

        private void WelcomeScreen_Shown(object sender, EventArgs e)
        {
            timer = new Timer();
            timer.Interval = 3000;
            timer.Start();
            timer.Tick += onTimeElapsedEvent;
        }

        private void onTimeElapsedEvent(object sender, EventArgs args)
        {
            timer.Stop();
            ApplicationWindow applicationWindow = new ApplicationWindow();
            applicationWindow.Show();
            this.Hide();
        }
    }
}
