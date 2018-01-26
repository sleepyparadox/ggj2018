using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SleepyHttp
{
    public class Config
    {
        public string HostPath { get; private set; }

        const string ConfigText = "SleepyHttp.Config.txt";
        const string DefaultWebsiteName = "ggj2018";

        public static Config Load()
        {
            var config = new Config();

            if(File.Exists(ConfigText))
            {
                foreach (var line in File.ReadAllLines(ConfigText))
                {
                    if (line.Contains('\t') == false || line.StartsWith("//"))
                        continue;

                    var cells = line.Split('\t');
                    if (cells.Length < 2)
                        continue;

                    if (cells[0].ToLower() == "hostpath")
                        config.HostPath = cells[1];
                }
            }

            if (string.IsNullOrEmpty(config.HostPath))
                config.HostPath = GetHostPath(DefaultWebsiteName);

            return config;
        }

        static string GetHostPath(string name)
        {
            var hostName = Dns.GetHostName();
            var myIp = Dns.GetHostEntry(hostName).AddressList.First(a => a.AddressFamily == AddressFamily.InterNetwork);
            return string.Format("http://{0}/{1}/", myIp, name);
        }
    }
}
