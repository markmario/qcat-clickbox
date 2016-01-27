// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="QCAT ">
//   QCAT
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Odes.License.Updater
{
    #region

    using System;
    using System.Data.SqlClient;
    using System.DirectoryServices;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Net.Sockets;
    using System.Security.Principal;
    using System.Text;

    using Odes.Licence.Model;
    using Odes.License.Updater.Properties;

    using QCat;

    #endregion

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        #region Static Fields

        private static readonly LicenseRequest Licence = new LicenseRequest();

        private static bool _hasEnded;

        #endregion

        #region Methods

        private static void Automatic()
        {
            Licence.LicenceType = LicenceTypes.Client;

            //Licence.SystemMachineName = "LINUIX01";// Environment.MachineName;
            //Licence.SystemId = "S-1-5-21-3115597392-398023741-1760809799";// new SecurityIdentifier((byte[])new DirectoryEntry(string.Format("WinNT://{0},Computer", Environment.MachineName)).Children.Cast<DirectoryEntry>().First().InvokeGet("objectSID"), 0).AccountDomainSid.ToString();
            //Licence.UserName = Environment.UserName + "@" + Environment.UserDomainName;

            /// *####NULEGAL
            //// Licence.Email = "peter.cryan@nulegal.com.au";
            //// Licence.Password = "nul3g@l";
            // */

            /*
             *EFILE
            
            Licence.Email = "cholmes@e-file.com.au";
            Licence.Password = "3fil3";
            Licence.ServiceQueue = "BRIEFCASE";
             */
            /*
            *LEXDATA
                        Licence.Email = "Ajnesh.Ram@lexdata.com.au";
            Licence.Password = "l3xd@ta";
            Licence.ServiceQueue = "APP01PNSWLD";
             **/
            /*ROyAL Commission DEV */

            //Licence.Email = "training";
            //Licence.Password = "training";

            /*ROyAL Commission DEV

            ////Licence.Email = "rcircsa";
            ////Licence.Password = "rc1rcs@";
            ////Licence.ServiceQueue = "srvwsapp600";

            ////Licence.SystemMachineName = "srvwsdbs600";
            ////Licence.SystemId = new SecurityIdentifier((byte[])new DirectoryEntry(string.Format("WinNT://{0},Computer", Environment.MachineName)).Children.Cast<DirectoryEntry>().First().InvokeGet("objectSID"), 0).AccountDomainSid.ToString();
            ////Licence.UserName = "offline.gen";
            */

            /*ROyAL Commission PROD */
            //Licence.Email = "rcircsa";
            //Licence.Password = "rc1rcs@";
            //Licence.ServiceQueue = "srvwsapp004";

            // Licence.SystemMachineName = "srvwsdbs001";
            // Licence.SystemId = new SecurityIdentifier((byte[])new DirectoryEntry(string.Format("WinNT://{0},Computer", Environment.MachineName)).Children.Cast<DirectoryEntry>().First().InvokeGet("objectSID"), 0).AccountDomainSid.ToString();
            // Licence.UserName = "offline.gen";

            /*ASIC*/
            //Licence.ServiceQueue = "SYDSQL4";
            //Licence.Email = "asic";
            //Licence.Password = "as1c";

            //Licence.SystemMachineName = "SYDSQL4"; ;
            //Licence.SystemId = new SecurityIdentifier((byte[])new DirectoryEntry(string.Format("WinNT://{0},Computer", Environment.MachineName)).Children.Cast<DirectoryEntry>().First().InvokeGet("objectSID"), 0).AccountDomainSid.ToString();
            //Licence.UserName = @"a1\Svc.qcat";
            //Licence.SystemNetworkCredential = Licence.UserName;
            /* */

            /*KORDA MENTHA*/
            //Licence.Email = "tobymasterson";
            //Licence.Password = "lawimagetest01";
            //Licence.UserName = "tobymasterson";

            /*SIMON*/
            //Licence.ServiceQueue = "SNOODLEBUG";
            //Licence.Email = "simon@qcat.com.au";
            //Licence.Password = "jy4agzeipxwuhd4x7xrw3h4h64";

            //Licence.SystemMachineName = "SNOODLEBUG"; ;
            //Licence.SystemId = new SecurityIdentifier((byte[])new DirectoryEntry(string.Format("WinNT://{0},Computer", Environment.MachineName)).Children.Cast<DirectoryEntry>().First().InvokeGet("objectSID"), 0).AccountDomainSid.ToString();
            //Licence.UserName = @"simon";
            //Licence.SystemNetworkCredential = Licence.UserName;
            /* */

            //iCourts was licensed with THIS!!!!!
            Licence.ServiceQueue = "ICDSRV01"; //"ICDSVR01"; //ICDSVR01\ICDSQLSRV
            Licence.Email = "m.lan@icourts.com.au";
            Licence.Password = "6bmtgdcr2c3uhh63doisdhfosq";
            //"ICDSVR01\\ICDSQLSRV"
            Licence.SystemMachineName = @"ICDSRV01\ICDSQLSRV";// "ICDSQLSRV"; //use the fully qualified instance name ICDSVR01/etc
            Licence.SystemId = new SecurityIdentifier((byte[])new DirectoryEntry(string.Format("WinNT://{0},Computer", Environment.MachineName)).Children.Cast<DirectoryEntry>().First().InvokeGet("objectSID"), 0).AccountDomainSid.ToString();
            Licence.UserName = @"m.lan@icourts.com.au";
            Licence.SystemNetworkCredential = Licence.UserName;
            GenerateLicenseFileForODESV2(Licence);
        }

        private static void GenerateLicenseFileForODESV2(LicenseRequest request, string productName = "QCAT-Odes")
        {
            var licensor = new ProductLicenser();
            request.ProductName = productName;
            var rep  =licensor.GeneratedLisenseV2(request, Resources.appid);

            var licxpath = "License.xml";

            // SaveToDb(Guid.NewGuid(), rep);
            File.WriteAllText(licxpath, rep.LicenseText, Encoding.UTF8);
            Console.WriteLine(Resources.savelicx, licxpath);
        }

        private static void GenerateLicenceFile(string productName = "ODES")
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
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Add("Authorization-Token", Odes.License.Updater.Properties.Resources.appid);
                var result = client.GetAsync(string.Format("api/License/GetProductDetail?productName={0}", productName)).Result;
                if (result.IsSuccessStatusCode)
                {
                    var re = result.Content.ReadAsAsync<Product>().Result;
                    Licence.ProductId = new Guid(re.Id);
                }
                var response = client.PostAsJsonAsync("api/License/", Licence).Result; // Blocking call!
                if (response.IsSuccessStatusCode)
                {
                    var rep = response.Content.ReadAsAsync<string>().Result;

                    // var licxpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\License.xml";
                    var licxpath = "License.xml";

                    // SaveToDb(Guid.NewGuid(), rep);
                    File.WriteAllText(licxpath, rep, Encoding.UTF8);
                    Console.WriteLine(Resources.savelicx, licxpath);
                }
                else
                {
                    Console.WriteLine(
                        Resources.Program_GenerateLicenceFile__0____1__, 
                        (int)response.StatusCode, 
                        response.ReasonPhrase);
                    File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\error_licx.log", response.ToString(), Encoding.UTF8);
                }

                Console.WriteLine(Resources.End);
                _hasEnded = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(Resources.error);
                File.WriteAllText(Path.GetTempFileName() + ".qlic.elog", ex.ToString(), Encoding.UTF8);
                Console.WriteLine(Resources.End);
                _hasEnded = true;
            }

            while (!_hasEnded)
            {
            }

            Console.ReadLine();
        }

        private static void Main(string[] args)
        {
            Automatic();
            //GenerateLicenseFileForODESV2
            //GenerateLicenceFile();

            // Console.ReadLine();
        }

        private static string SaveToDb(Guid id, string detail)
        {
            // const string getter =
            // "SELECT TOP 1 [Id] ,[Name] ,[Detail] FROM [dbo].[Licx] WHERE [Name] = '{0}' OR NAME = '{1}'";
            const string lines = "INSERT INTO [dbo].[Licx] ( [Id] ,[Name] ,[Detail]) VALUES ('{0}' ,'{1}' ,'{2}')";

            using (var connection = new SqlConnection(TopCatDbConfiguration.Instance.ConnectionString))
            {
                connection.Open();
                try
                {
                    var batchcmd = new SqlCommand(string.Format(lines, id, Environment.MachineName, detail), connection);
                    batchcmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return null;
                }

                return null;
            }
        }

        #endregion
    }
}