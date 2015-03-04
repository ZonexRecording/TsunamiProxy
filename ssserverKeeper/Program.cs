using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ssserverKeeper
{
    static class Program
    {
        public static readonly string WEBSERVICE = @"http://localhost:3000/";
        public static readonly string SERVER_NAME = "default";
        public static Config config { get; private set; }
        public static string publicIP { get; private set; }
        public static Nullable<DateTime> startTime { get; private set; }

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (Mutex mutex = new Mutex(false, "Global\\" + "A834B050-CB81-4D85-A883-96F4213BDE1A"))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                if (!mutex.WaitOne(0, false))
                    return;

                viewControl();
                Thread t = new Thread(new ThreadStart(serverControl));
                t.IsBackground = true;
                t.Start();

                Application.Run();
            }
        }

        /// <summary>
        /// 状态监视器的控制
        /// </summary>
        static void viewControl()
        {
            FormMain displayForm = null;
            Action ShowForm = () =>
            {
                if (displayForm != null)
                    displayForm.Activate();
                else
                {
                    displayForm = new FormMain();
                    displayForm.Show();
                    displayForm.FormClosed += (o, e) => displayForm = null;
                }
            };

            NotifyIcon icon = new NotifyIcon();
            icon.Icon = ssserverKeeper.Properties.Resources.ssserver;
            icon.Visible = true;
            icon.ContextMenu = new ContextMenu(new MenuItem[]{
                new MenuItem("显示", (o,e)=>ShowForm()),
                new MenuItem("退出", (o,e)=>{icon.Visible=false;Application.Exit();})
            });
            icon.MouseDoubleClick += (o, e) => { if (e.Button == MouseButtons.Left) ShowForm(); };
        }

        /// <summary>
        /// 服务功能控制
        /// </summary>
        static void serverControl()
        {
            System.Diagnostics.Process p = null;
            int countGood = 0;
            int countFail = 0;
            while(true)
            {
                if (p == null || p.HasExited)
                {
                    countGood = 0;
                    publicIP = null;
                    startTime = null;
                    if (++countFail > 3)
                    {
                        config = new Config(true);
                        countFail = 0;
                    }
                    else
                        config = new Config();
                    System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                    info.FileName = "ssserver";
                    info.Arguments = string.Format("-p {0} -k {1} -m {2}", config.port, config.password, config.method);
                    info.UseShellExecute = false;
                    info.RedirectStandardError = true;
                    info.CreateNoWindow = true;
                    p = System.Diagnostics.Process.Start(info);
                }
                else
                {
                    countFail = 0;
                    if (++countGood > 50)
                    {
                        countGood = 0;
                        // register server info to website   &   get public IP
                        System.Threading.Tasks.Task.Factory.StartNew(() => websiteRegister()).ContinueWith(t => publicIP = t.Result);
                    }
                    if (startTime == null)
                        startTime = DateTime.Now;
                }
                Thread.Sleep(1000);
            }
        }
        static string websiteRegister()
        {
            // send request
            WebRequest request = WebRequest.Create(Program.WEBSERVICE);
            request.Method = "PUT";
            byte[] buffer = Encoding.Default.GetBytes(SimpleJson.SimpleJson.SerializeObject(new
            {
                name = Program.config.name,
                port = Program.config.port,
                password = Program.config.password,
                method = Program.config.method,
            }));
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            // read response
            string ipAddress;
            using (WebResponse response = request.GetResponse())
            using (System.IO.Stream resStream = response.GetResponseStream())
            {
                var buffer2 = new byte[4096];
                int bytesRead = resStream.Read(buffer2, 0, buffer2.Length);
                ipAddress = Encoding.Default.GetString(buffer2, 0, bytesRead);
            }
            // validate ipAddress
            string ValidIpAddressRegex = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
            if(System.Text.RegularExpressions.Regex.IsMatch(ipAddress, ValidIpAddressRegex ))
                return ipAddress;
            else
                return null;
        }
    }
}
