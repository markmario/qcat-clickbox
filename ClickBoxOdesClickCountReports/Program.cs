using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace ClickBoxOdesClickCountReports
{
    using System.Collections.Specialized;
    using System.IO;

    using Newtonsoft.Json.Linq;

    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        private static string _mandrillKey;
        private static NameValueCollection _config;
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            _config = System.Configuration.ConfigurationManager.AppSettings;
            var hostConfig = new JobHostConfiguration(
                SetStorageAccountConnectionString());
            hostConfig.UseTimers();
            var host = new JobHost(hostConfig);
            
            // The following code will invoke a function called ManualTrigger and 
            // pass in data (value in this case) to the function
            //host.Call(typeof(Functions).GetMethod("ManualTrigger"), new { value = 20 });
            host.RunAndBlock();
        }

        private static string SetStorageAccountConnectionString()
        {
            var runtime = _config["Runtime"];
            if (runtime == "debug")
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var dbPath = Path.Combine(
                    appDataPath,
                    _config["DropBoxDb"]);
                var lines = File.ReadAllLines(dbPath);
                var dbBase64Text = Convert.FromBase64String(lines[1]);

                string filepath;
                string mandrill;
                filepath = Encoding.ASCII.GetString(dbBase64Text)
                           + _config["AzureDevConnection"];

                mandrill = Encoding.ASCII.GetString(dbBase64Text)
                           + _config["MandrillKey"];

                var conJson = JObject.Parse(File.ReadAllText(filepath));
                var constring = conJson["azure"].ToString();

                var _mandrillKeyJson = JObject.Parse(File.ReadAllText(mandrill));
                _mandrillKey = _mandrillKeyJson["mandrill"].ToString();

                return constring;
            }

            return System.Configuration
                .ConfigurationManager
                .ConnectionStrings["AzureProdConnection"].ToString();
        }

        public static string MandrillKey
        {
            get
            {
                var runtime = _config["Runtime"];
                if (runtime == "debug")
                {
                    return _mandrillKey;
                }
                return System.Configuration.
                    ConfigurationManager.
                    ConnectionStrings["MandrillKey"].ToString();
            }
        }
    }
}
