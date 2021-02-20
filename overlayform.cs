using System;
using System.Drawing;
using System.Management;
using System.Windows.Forms;

namespace Brightness_Stress_Test
{
    public partial class overlayform : Form
    {
        public overlayform()
        {
            InitializeComponent();
        }
        
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //empty implementation
            this.BackColor = Color.LimeGreen;
            this.TransparencyKey = Color.LimeGreen;
            
        }

        private void CheckBattery()
        {
            ManagementClass wmi = new ManagementClass("Win32_Battery");
            var allBatteries = wmi.GetInstances();
            foreach (var battery in allBatteries)
            {
                int batteryLevel = Convert.ToInt32(battery["EstimatedChargeRemaining"]);
                var msg = $"Battery Level: {batteryLevel}%";
                label1.Text = msg;
            }
        }
        
        private void overlayform_Load(object sender, EventArgs e)
        {
            label1.Text = "Battery Level: NaN";
            timer1.Tick += __timer_Tick;
            timer1.Interval = 1000 * 10;
            timer1.Enabled = true;
            timer1.Start();

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(100, 100);
            this.TopMost = true;
        }

        private void __timer_Tick(object sender, EventArgs e)
        {
            CheckBattery();
        }
    }
}