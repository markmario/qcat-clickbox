namespace ClickBox.CreateAccounts
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Text;

    using Microsoft.Azure.WebJobs;

    using Newtonsoft.Json.Linq;

    internal class Program
    {
        private static string _mandrillKey;
        private static NameValueCollection _config;

        #region Methods

        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        private static void Main()
        {
            _config = System.Configuration.ConfigurationManager.AppSettings;
            var host = new JobHost(new JobHostConfiguration(SetStorageAccountConnectionString()));

            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }

        private static string SetStorageAccountConnectionString()
        {
            var runtime = _config["Runtime"];
            if (runtime == "debug")
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
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

        #endregion
    }
}