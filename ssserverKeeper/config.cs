using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ssserverKeeper
{
    public class Config
    {
        public string name { get; private set; }
        public int port { get; set; }
        public string password { get; set; }
        public string method { get; set; }

        public Config(bool changePort = false)
        {
            this.name = Program.SERVER_NAME;
            this.port = changePort ? (new Random()).Next(8300, 8400) : 8388;
            this.password = (new Random()).Next(999999).ToString("D6");
            this.method = "aes-256-cfb";

        }
    }
}
