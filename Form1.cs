using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

namespace Brightness_Stress_Test
{
    public partial class Form1 : Form
    {
        private ulong last_access;
        private ulong log_every_seconds = 300;
        private Process proce;
        private overlayform overlay;
        private System.Timers.Timer ttimer1;
        private System.Timers.Timer ttimer2;

        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        
        public Form1()
        {
            InitializeComponent();
            button2.Enabled = false;
        }
        
        private void init_timer()
        {
            if(File.Exists(Path.Combine(AssemblyDirectory, "log.txt")))
                File.Delete(Path.Combine(AssemblyDirectory, "log.txt"));
            int ss = Int32.Parse(textBox1.Text);
            ttimer1 = new System.Timers.Timer {Interval = 1000 * 60 * ss};
            ttimer1.Elapsed += _timer_Tick;
            ttimer1.Start();
            init_timer_seconds_ticker();
        }
        
        // run something per iteration (currently every 10 sec)
        private void init_timer_seconds_ticker()
        {
            last_access = UnixTimestamp.Now;
            ttimer2 = new System.Timers.Timer {Interval = 10000};
            ttimer2.Elapsed += _timer_seconds_Tick;
            ttimer2.Start();
        }

        private void _timer_seconds_Tick(object sender, EventArgs e)
        {
            //run that something here
            int steps = Int32.Parse(textBox2.Text);
            compare_timestamps();
            do_brightness_steps(100, 1, steps);
            do_brightness_steps(1, 100, steps);
        }

        private void compare_timestamps()
        {
            ulong current_access = UnixTimestamp.Now;
            //testing 30 sec, later will be 300 seconds ie. 5 minutes
            if ((current_access - last_access) > log_every_seconds)
            {
                File.AppendAllText(Path.Combine(AssemblyDirectory,"log.txt"),
                    UnixTimestamp.UnixTimestampToDateTime(current_access).ToUniversalTime().ToString("u")
                    + Environment.NewLine);
                last_access = UnixTimestamp.Now;
            }

        }

        private void set_brightness(int val)
        {
            try {
                ManagementScope s = new ManagementScope("root\\WMI");
                SelectQuery q = new SelectQuery("WmiMonitorBrightnessMethods");
                ManagementObjectSearcher mos = new ManagementObjectSearcher(s, q);
                ManagementObjectCollection moc = mos.Get();
                foreach (var managementBaseObject in moc)
                {
                    var o = (ManagementObject) managementBaseObject;
                    o.InvokeMethod("WmiSetBrightness", new object[] { UInt32.MaxValue, val });
                }
                moc.Dispose();
                mos.Dispose();
            }catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        private void do_brightness_steps(int from, int to, int steps)
        {
            decimal _waittime = Math.Round((decimal) 1000 / steps);
            int waittime = Decimal.ToInt32(_waittime);
            decimal _incr = Math.Round((decimal) 100 / steps);
            int incr = Decimal.ToInt32(_incr);
            // check do we do increments or decrements
            if (to > from)
            {
                for (int i = from; i < to; i += incr)
                {
                    set_brightness(i);
                    Thread.Sleep(waittime);
                } 
            }
            else
            {
                for (int i = from; i > to; i -= incr)
                {
                    set_brightness(i);
                    Thread.Sleep(waittime);
                } 
            }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            stop_timers();
            MessageBox.Show(@"Time's up!", @"Timeout reached", MessageBoxButtons.OK, MessageBoxIcon.Information);
            button2.PerformClick();
        }

        private void stop_timers()
        {
            ttimer1.Stop();
            ttimer2.Stop();
        }

        private void start_actions()
        {
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = true;
            start_video();
            init_timer();
            start_overlay();
        }

        private void start_overlay()
        {
            Thread.Sleep(20000);
            overlay = new overlayform();
            overlay.Show();
        }
        
        private void stop_overlay()
        {
            overlay?.Close();
        }

        private void stop_actions()
        {
            stop_video();
            stop_overlay();
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            button1.Enabled = true;
            stop_timers();
            set_brightness(100);
            button2.Enabled = false;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            start_actions();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            stop_actions();
        }

        private void stop_video()
        {
            proce?.Close();
        }
        
        private void start_video()
        {
            if (
                File.Exists(Path.Combine(AssemblyDirectory, "mplayer.exe"))
                && File.Exists(Path.Combine(AssemblyDirectory, "video.mp4")))
            {
                proce = new Process
                {
                    StartInfo =
                    {
                        FileName = Path.Combine(AssemblyDirectory, "mplayer.exe"),
                        Arguments = $"-fs -loop 0 \"{Path.Combine(AssemblyDirectory,"video.mp4")}\"",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    }
                };
                proce.Start();
            }
        }
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Escape))
            {
                button2.PerformClick();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            
        }

    }
    
    public static class UnixTimestamp
    {
        private static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static DateTime UnixTimestampToDateTime(ulong timestamp)
        {
            return UnixEpoch.AddSeconds(timestamp);
        }
 
        public static ulong Now
        {
            get
            {
                return (ulong)(DateTime.UtcNow - UnixEpoch).TotalSeconds;
            }
        }
    }
    
    
}