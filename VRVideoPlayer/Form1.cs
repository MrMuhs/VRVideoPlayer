using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Timers;
using System.Runtime.Serialization;

namespace VRVideoPlayer
{
    public partial class VRVideoPlayer : Form
    {
        class playlistItem
        {
            public playlistItem(string url, int x, int y, int w, int h)
            {
                this.url = url;
                this.x = x;
                this.y = y;
                this.w = w;
                this.h = h;
            }
            public string url;
            public int x;
            public int y;
            public int w;
            public int h;
        }

        Form2 player = new Form2();
        List<playlistItem> playlist = new List<playlistItem>();
        int playlistIndex;
        String[] playlistFileSeperator = { "/__/" };
        private GlobalKeyboardHook _globalKeyboardHook;
        int displayId;

        public VRVideoPlayer()
        {
            InitializeComponent();

            displayId = 0;

            player.axWindowsMediaPlayer1.PlayStateChange += AxWindowsMediaPlayer1_PlayStateChange;
            player.Show();

            _globalKeyboardHook = new GlobalKeyboardHook();
            _globalKeyboardHook.KeyboardPressed += onKeyPressed;

            loadPlaylist();
            playlistIndex = -1;

            playNext();
        }

        public void Dispose()
        {
            _globalKeyboardHook?.Dispose();
            base.Dispose();
        }

        private void AxWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            //Console.WriteLine("State=" + player.axWindowsMediaPlayer1.playState + " thread=" + Thread.CurrentThread.ManagedThreadId);

            if (player.axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsStopped)
            {
                System.Timers.Timer aTimer = new System.Timers.Timer(1);
                // Hook up the Elapsed event for the timer. 
                aTimer.Elapsed += timerCallback;
                aTimer.AutoReset = false;
                aTimer.Enabled = true;
            }
            if (player.axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                showPlaylist();
                alignPlayer();
            }
        }

        private void timerCallback(Object source, ElapsedEventArgs e)
        {
            //Console.WriteLine("Timer...");
            playNext();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            //openFileDialog1.Filter = "(*.mp4) | *.mp4";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                player.play(openFileDialog1.FileName);
                alignPlayer();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //axVLCPlugin21.playlist.play();
        }

        private void playNext()
        {
            if (playlist.Count <= 0)
                return;

            playlistIndex++;
            if (playlistIndex >= playlist.Count)
                playlistIndex = 0;

            player.play(playlist.ElementAt(playlistIndex).url);
        }

        private void playPrevious()
        {
            if (playlist.Count <= 0)
                return;

            playlistIndex--;
            if (playlistIndex < 0)
                playlistIndex = playlist.Count - 1;

            player.play(playlist.ElementAt(playlistIndex).url);
        }

        private void alignPlayer()
        {
            if (System.Windows.Forms.Screen.AllScreens.Length >= 2)
            {
                int displayX = System.Windows.Forms.Screen.AllScreens[displayId].Bounds.X;
                int displayY = System.Windows.Forms.Screen.AllScreens[displayId].Bounds.Y;
                int displayW = System.Windows.Forms.Screen.AllScreens[displayId].Bounds.Width;
                int displayH = System.Windows.Forms.Screen.AllScreens[displayId].Bounds.Height;

                try
                {
                    player.Location = new Point(displayX + playlist.ElementAt(playlistIndex).x, playlist.ElementAt(playlistIndex).y);
                    player.Size = new Size(playlist.ElementAt(playlistIndex).w, playlist.ElementAt(playlistIndex).h);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("alignPlayer() catched");
                }
            }
            else
            {
                // fuck off
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Console.WriteLine("button2_Click_1 thread=" + Thread.CurrentThread.ManagedThreadId);
            playNext();
        }

        private void showPlaylist()
        {
            textBox1.Clear();
            for (int i = 0; i < playlist.Count; i++)
            {
                textBox1.Text += i + " " + playlist.ElementAt(i).url + " " + playlist.ElementAt(i).x + " " + playlist.ElementAt(i).y + " " + playlist.ElementAt(i).w + " " + playlist.ElementAt(i).h;
                if (i == playlistIndex)
                {
                    textBox1.Text += " (playing)";
                }
                textBox1.Text += "\r\n";
            }
        }

        private void loadPlaylist()
        {
            // Read the file and display it line by line.  
            System.IO.StreamReader file = new System.IO.StreamReader(@"playlist.txt", Encoding.Default);
            string line;
            int counter = 0;
            while ((line = file.ReadLine()) != null)
            {
                Console.WriteLine("Line of file=" + line);

                string[] parts = line.Split(playlistFileSeperator, StringSplitOptions.None);
                if (parts.Length == 5)
                {
                    try
                    {
                        playlist.Add(new playlistItem(parts.ElementAt(0), int.Parse(parts.ElementAt(1)), int.Parse(parts.ElementAt(2)), int.Parse(parts.ElementAt(3)), int.Parse(parts.ElementAt(4))));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Something went wrong parsing");
                    }
                }
                else
                {
                    Console.WriteLine("Line is shit, Length=" + parts.Length);
                }
                counter++;
            }
            file.Close();

            showPlaylist();
        }

        private void storePlaylist()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"playlist.txt", false, Encoding.Default);
            for (int i = 0; i < playlist.Count; i++)
            {
                file.WriteLine(playlist.ElementAt(i).url + playlistFileSeperator[0] +
                    playlist.ElementAt(i).x + playlistFileSeperator[0] +
                    playlist.ElementAt(i).y + playlistFileSeperator[0] + 
                    playlist.ElementAt(i).w + playlistFileSeperator[0] +
                    playlist.ElementAt(i).h);
            }
            file.Close();
        }

        // https://stackoverflow.com/questions/604410/global-keyboard-capture-in-c-sharp-application
        private void onKeyPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            if(e.KeyboardData.VirtualCode == Keys.Up.GetHashCode() && e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                //Console.WriteLine("PRESSED UP");
                playlist.ElementAt(playlistIndex).y++;
                alignPlayer();
                showPlaylist();
            }
            else if (e.KeyboardData.VirtualCode == Keys.Down.GetHashCode() && e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                //Console.WriteLine("PRESSED DOWN");
                playlist.ElementAt(playlistIndex).y--;
                alignPlayer();
                showPlaylist();
            }
            else if (e.KeyboardData.VirtualCode == Keys.Left.GetHashCode() && e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                //Console.WriteLine("PRESSED LEFT");
                playlist.ElementAt(playlistIndex).x--;
                alignPlayer();
                showPlaylist();
            }
            else if (e.KeyboardData.VirtualCode == Keys.Right.GetHashCode() && e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                //Console.WriteLine("PRESSED RIGHT");
                playlist.ElementAt(playlistIndex).x++;
                alignPlayer();
                showPlaylist();
            }
            else if (e.KeyboardData.VirtualCode == Keys.Q.GetHashCode() && e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                //Console.WriteLine("PRESSED Q");
                playlist.ElementAt(playlistIndex).w++;
                alignPlayer();
                showPlaylist();
            }
            else if (e.KeyboardData.VirtualCode == Keys.A.GetHashCode() && e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                //Console.WriteLine("PRESSED A");
                playlist.ElementAt(playlistIndex).w--;
                alignPlayer();
                showPlaylist();
            }
            else if (e.KeyboardData.VirtualCode == Keys.W.GetHashCode() && e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                //Console.WriteLine("PRESSED W");
                playlist.ElementAt(playlistIndex).h++;
                alignPlayer();
                showPlaylist();
            }
            else if (e.KeyboardData.VirtualCode == Keys.S.GetHashCode() && e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                //Console.WriteLine("PRESSED S");
                playlist.ElementAt(playlistIndex).h--;
                alignPlayer();
                showPlaylist();
            }
            else if (e.KeyboardData.VirtualCode == Keys.Escape.GetHashCode() && e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                //Console.WriteLine("PRESSED Escape");
                storePlaylist();
                Close();
            }
            else if (e.KeyboardData.VirtualCode == Keys.X.GetHashCode() && e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                //Console.WriteLine("PRESSED X");
                playPrevious();
            }
            else if (e.KeyboardData.VirtualCode == Keys.Y.GetHashCode() && e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                //Console.WriteLine("PRESSED Y");
                playNext();
            }
            else if (e.KeyboardData.VirtualCode == Keys.Space.GetHashCode() && e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                //Console.WriteLine("PRESSED Space");
                player.togglePlayPause();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            displayId = 0;
            alignPlayer();
        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            displayId = 1;
            alignPlayer();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            playPrevious();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            playNext();
        }
    }
}
