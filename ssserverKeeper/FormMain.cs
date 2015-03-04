using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ssserverKeeper
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();

            dispConfig(null);
            dispIP(null);
            dispTime(null);
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel label = sender as LinkLabel;
            if (label == null || e == null || e.Button != System.Windows.Forms.MouseButtons.Left)
                return;
            if (label.Text.ToLower().IndexOf("shadowsocks") >= 0)
                Process.Start("https://github.com/shadowsocks/shadowsocks/");
            else if (label.Text.ToLower().IndexOf("tsunamiproxy") >= 0)
                Process.Start("https://github.com/Ecbruck/TsunamiProxy");
            else if (label.Text.ToLower().IndexOf("xuhai") >= 0)
                Process.Start("mailto:xuhai515@foxmail.com");
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            dispConfig(Program.config);
            dispIP(Program.publicIP);
            dispTime(Program.startTime);
        }

        private void dispConfig(Config config)
        {
            if (config == null)
                foreach (Label l in new Label[] { label10, label11, label12, label13 })
                    l.Text = "-";
            else
            {
                label10.Text = config.method;
                label11.Text = config.password;
                label12.Text = config.port.ToString();
                label13.Text = config.name;
            }
        }
        private void dispIP(string strIP)
        {
            if (string.IsNullOrWhiteSpace(strIP))
                label9.Text = "-";
            else
                label9.Text = strIP;
        }
        private void dispTime(Nullable<DateTime> startTime)
        {
            if (startTime.HasValue)
            {
                TimeSpan duration = DateTime.Now - startTime.Value;
                if (duration.Days > 0)
                    label8.Text = duration.ToString("d'天 'h':'mm':'ss");
                else
                    label8.Text = duration.ToString("h':'mm':'ss");
            }
            else
                label8.Text = "-";
        }

    }
}
