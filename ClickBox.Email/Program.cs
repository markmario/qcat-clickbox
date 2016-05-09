namespace ClickBox.Email
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json.Linq;

    internal class Program
    {
        private static string _mandrillKey;
        private static NameValueCollection _config  = System.Configuration.ConfigurationManager.AppSettings;

        #region Methods

        static Program()
        {
            _mandrillKey = default(string);
            SetStorageAccountConnectionString();
        }

        Program(bool withoutStorage)
        {
            _mandrillKey = default(string);
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

        #endregion
    }
}
