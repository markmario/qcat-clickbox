using System;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using Odes.Licence.Model;
using Odes.Licence.Request.Properties;
using QCat;

namespace Odes.Licence.Request
{
    class Program
    {
        private static readonly LicenseRequest Licence = new LicenseRequest();
        private static bool _hasEnded;

        static void Main(string[] args)
        {
            Console.WriteLine(Resources.Program_Main_Welcome_to_the_QCAT_ODES_Licence_request_program);
            Console.WriteLine(Resources.Program_Main_);
            Console.WriteLine();

            //Manual();
            Automatic();
            GenerateLicenceFile();
        }

        private static void Automatic()
        {
            Licence.LicenceType = LicenceTypes.Client;
            Licence.Email = "peter.cryan@nulegal.com.au";
            Licence.ServiceQueue = "TSTSYDSQL02";
            Licence.Password = "nul3g@l";
            Licence.SystemMachineName = Environment.MachineName;
            Licence.SystemId = new SecurityIdentifier((byte[])new DirectoryEntry(string.Format("WinNT://{0},Computer", Environment.MachineName)).Children.Cast<DirectoryEntry>().First().InvokeGet("objectSID"), 0).AccountDomainSid.ToString();
            Licence.UserName = Environment.UserName + "@" + Environment.UserDomainName;
           // Licence.SystemId = new SecurityIdentifier((byte[])new DirectoryEntry(string.Format("WinNT://{0},Computer", Environment.MachineName)).Children.Cast<DirectoryEntry>().First().InvokeGet("objectSID"), 0).AccountDomainSid.ToString();
            //Licence.UserName = Environment.UserName + "@" + Environment.UserDomainName;

        }

        private static void Manual()
        {
            Licence.SystemId = new SecurityIdentifier((byte[])new DirectoryEntry(string.Format("WinNT://{0},Computer", Environment.MachineName)).Children.Cast<DirectoryEntry>().First().InvokeGet("objectSID"), 0).AccountDomainSid.ToString();
            Licence.UserName = Environment.UserName + "@" + Environment.UserDomainName;
            var proceed = WhatLicenceTypeDoYouWant();

            if (StopOrContinue(proceed)) return;

            proceed = HowManyClicksDoYouWant();

            if (StopOrContinue(proceed)) return;

            proceed = WhatIsYourEmailAddress();

            if (StopOrContinue(proceed)) return;

            proceed = WhatIsYourPassword();

            if (StopOrContinue(proceed)) return;

            proceed = WhatIsTheNameOfYourServiceQueue();

            if (StopOrContinue(proceed)) return;
        }

        private static void GenerateLicenceFile()
        {
            Console.WriteLine();
            Console.WriteLine(Resources.genlicxfile);

            Console.WriteLine(Resources.wait);
            
            Licence.GetPublicIp();
            try
            {

#if ! DEBUG
                var client = new HttpClient { BaseAddress = new Uri("https://clickbox.qcat.com.au/") };
#else
            var client = new HttpClient { BaseAddress = new Uri("https://localhost:44302/") };
#endif

                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

               client.DefaultRequestHeaders.Add("Authorization-Token", Odes.Licence.Request.Properties.Resources.appid);


                var response = client.PostAsJsonAsync("api/License/", Licence).Result; // Blocking call!
                if (response.IsSuccessStatusCode)
                {
                    var rep = response.Content.ReadAsAsync<string>().Result;
                    var licxpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\License.xml";
                    SaveToDb(Guid.NewGuid(), rep);

                    File.WriteAllText(licxpath,
                                      rep, Encoding.UTF8);
                    Console.WriteLine(Resources.savelicx, licxpath);
                    
                }
                else
                {
                    Console.WriteLine(Resources.Program_GenerateLicenceFile__0____1__, (int)response.StatusCode, response.ReasonPhrase);
                    File.WriteAllText(Path.GetTempFileName() + ".qlic.elog", response.ToString(), Encoding.UTF8);
                }
                Console.WriteLine(Resources.End);
                _hasEnded = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(Resources.error);
                File.WriteAllText(Path.GetTempFileName()+".qlic.elog", ex.ToString(),Encoding.UTF8);
                Console.WriteLine(Resources.End);
                _hasEnded = true;
            }

            while (!_hasEnded) { }

            Console.ReadLine();
        }

        private static string SaveToDb(Guid id,string detail)
        {

            //const string getter =
            //    "SELECT TOP 1 [Id] ,[Name] ,[Detail] FROM [dbo].[Licx] WHERE [Name] = '{0}' OR NAME = '{1}'";
            const string lines = "INSERT INTO [dbo].[Licx] ( [Id] ,[Name] ,[Detail]) VALUES ('{0}' ,'{1}' ,'{2}')";

            using (var connection = new SqlConnection(TopCatDbConfiguration.Instance.ConnectionString))
            {
                connection.Open();
                try
                {
                    var batchcmd = new SqlCommand(
                        string.Format(lines, id, Environment.MachineName,detail), connection);
                    batchcmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    return null;
                }
                return null;
            }
        }

        private static bool WhatIsTheNameOfYourServiceQueue()
        {
            var answered = false;
            string queueName;

            do
            {
                Console.WriteLine();
                Console.WriteLine(Resources.servicequeue, Licence.LicenceTypeText());

                queueName = Console.ReadLine();

                if (string.IsNullOrEmpty(queueName) == false)
                {
                    answered = true;
                    Licence.ServiceQueue = queueName;
                }

            } while (!answered);

            return queueName != "Q";
        }

        private static bool WhatIsYourPassword()
        {
            var answered = false;
            string pwd;

            do
            {
                Console.WriteLine();
                Console.WriteLine(Resources.password);

                pwd = Console.ReadLine();

                if (string.IsNullOrEmpty(pwd) == false)
                {
                    answered = true;
                    Licence.Password = pwd;
                }

            } while (!answered);

            return pwd != "Q";
        }

        private static bool WhatIsYourEmailAddress()
        {
            var answered = false;
            string emailAddress;

            do
            {
                Console.WriteLine();
                Console.WriteLine(Resources.email);

                emailAddress = Console.ReadLine();

               // var emailMatch = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

                if (string.IsNullOrWhiteSpace(emailAddress) == false)// && emailMatch.IsMatch(emailAddress))
                {
                    answered = true;
                    Licence.Email = emailAddress;
                }

            } while (!answered);

            return emailAddress != "Q";
        }

        private static bool StopOrContinue(bool proceed)
        {
            if (!proceed)
            {
                Exiting();
                return true;
            }
            return false;
        }

        private static bool HowManyClicksDoYouWant()
        {
            bool answered;
            string numberOfClicks;
            if (Licence.LicenceType == LicenceTypes.Client)
                return true;
            do
            {
                Console.WriteLine();
                Console.WriteLine(Resources.clicks);

                foreach (var battery in BatterySizes.Batteries)
                {
                    Console.Write(battery.Option + Resources.Program_HowManyClicksDoYouWant_ + battery.Name + Resources.tab);
                }

                Console.WriteLine();

// ReSharper disable PossibleNullReferenceException
                numberOfClicks = Console.ReadLine().ToUpper();
// ReSharper restore PossibleNullReferenceException

                answered = BatterySizes.IsValidOptionSupplied(numberOfClicks);
                if (answered) Licence.ClicksReqeusted = BatterySizes.GetClickCountFromOption(numberOfClicks);

            } while (!answered);

            return numberOfClicks != "Q";
        }

        private static bool WhatLicenceTypeDoYouWant()
        {
            var answered = false;

            string typeofLicence;

            do
            {
                Console.WriteLine();
                Console.WriteLine(Resources.licxtypequestion + Environment.NewLine +
                    Resources.isolatorq + Environment.NewLine +
                    Resources.koderq + Environment.NewLine +
                    Resources.clientq);

// ReSharper disable PossibleNullReferenceException
                typeofLicence = Console.ReadLine().ToUpper();
// ReSharper restore PossibleNullReferenceException

                if (typeofLicence == "I" ||
                    typeofLicence == "K" ||
                    typeofLicence == "C" ||
                    typeofLicence == "Q")
                {
                    answered = true;
                    switch (typeofLicence)
                    {
                        case "I":
                            Licence.LicenceType = LicenceTypes.Isolator;
                            break;
                        case "K":
                            Licence.LicenceType = LicenceTypes.Koder;
                            break;
                        case "C":
                            Licence.LicenceType = LicenceTypes.Client;
                            break;
                    }
                }

            } while (!answered);

            return "I" == typeofLicence || "K" == typeofLicence || "C" == typeofLicence;
        }

        private static void Exiting()
        {
            Console.WriteLine(Resources.exitmagic);
            Console.ReadLine();
        }
    }
}