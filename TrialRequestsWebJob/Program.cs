using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace TrialRequestsWebJob
{
    using System.IO;

    using Newtonsoft.Json.Linq;

    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var host = new JobHost(new JobHostConfiguration(SetStorageAccountConnectionString()));
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }

        private static string SetStorageAccountConnectionString()
        {
            var runtime = System.Configuration.ConfigurationSettings.AppSettings["Runtime"];
            if (runtime == "debug")
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var dbPath = Path.Combine(appDataPath, System.Configuration.ConfigurationSettings.AppSettings["DropBoxDb"]);
                var lines = File.ReadAllLines(dbPath);
                var dbBase64Text = Convert.FromBase64String(lines[1]);

                string filepath;
                filepath = System.Text.Encoding.ASCII.GetString(dbBase64Text)
                           + System.Configuration.ConfigurationSettings.AppSettings["AzureDevConnection"];

                var conJson = JObject.Parse(File.ReadAllText(filepath));
                var constring = conJson["azure"].ToString();
                return constring;
            }
            return System.Configuration.ConfigurationSettings.AppSettings["AzureProdConnection"];
        }
    }
}
