using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRVideoPlayer
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            //no fucking bar this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            axWindowsMediaPlayer1.stretchToFit = true;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            
        }

        public void play(string url, double pos = 0.0)
        {
            //if(axWindowsMediaPlayer1.InvokeRequired)
            //{
            //    //Console.WriteLine("Invoce what the fuck play: " + url);
            //    axWindowsMediaPlayer1.URL = url;
            //    axWindowsMediaPlayer1.Ctlcontrols.play();
            //}
            //else
            //{
                Console.WriteLine("now play: " + url);
                axWindowsMediaPlayer1.URL = url;
                axWindowsMediaPlayer1.Ctlcontrols.currentPosition = pos;
                axWindowsMediaPlayer1.Ctlcontrols.play();
            //}
        }

        public void togglePlayPause()
        {
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                axWindowsMediaPlayer1.Ctlcontrols.pause();
            }
            else
            {
                axWindowsMediaPlayer1.PerformLayout();
                play(axWindowsMediaPlayer1.URL, axWindowsMediaPlayer1.Ctlcontrols.currentPosition);
            }
        }

        private void Form2_SizeChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Form2_SizeChanged");
            axWindowsMediaPlayer1.Size = new Size(this.Width, this.Height);
            //axWindowsMediaPlayer1.Update();
            axWindowsMediaPlayer1.PerformLayout();
        }
    }
}
