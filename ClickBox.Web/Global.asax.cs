// --------------------------------------------------------------------------------------------------
//  <copyright file="Global.asax.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Profile;
    using System.Web.Routing;

    using AutoMapper;

    using Microsoft.Azure;

    using Models;
    using TableStorage;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;

    using Newtonsoft.Json.Linq;

    using Odes.Licence.Model;

    using Product = ClickBox.Web.Models.Product;

    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : HttpApplication
    {
        #region Public Properties
        
        public static CloudStorageAccount AzureStorageAccount { get; private set; }
        public static string TableStoreConnectionString { get; private set; }
        public static string StripePurchaseString { get; private set; }

        #endregion

        #region Methods

        protected async void Application_Start()
        {
            //Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey =
            //    Environment.GetEnvironmentVariable("AzureInsightsKey");

            SetStorageAccountConnectionString();
            ApplicationUserManager.StartupAsync();
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            AzureStorageAccount = GetStorageAccount();
            SetStripeKeyForPurchaseRequests();
 
            SetAutoMappings();
            await this.PrimeTableStorage();
        }

        private async Task PrimeTableStorage()
        {
            var client = AzureStorageAccount.CreateCloudTableClient();
            await client.PrimeTable<PersistedUserAccount>();
            await client.PrimeTable<Product>();
            await client.PrimeTable<ClientIssuedLicense>();
            await client.PrimeTable<WebLicenseRequest>();
            await client.PrimeTable<PersistedIsolatedBatch>();
            await client.PrimeTable<PersistedDocumentCoded>();
            await client.PrimeTable<MonthlyCodedDocument>();
            await client.PrimeTable<MonthlyIsolatedBatch>();
            await client.PrimeTable<PostLicenseRequestFailure>();
            await client.PrimeTable<SuccessfulStripeCharge>();
            await client.PrimeTable<ChargeRequest>();
            await client.PrimeTable<FailedStripeCharge<Exception>>();
        }

        private static void SetAutoMappings()
        {
            Mapper.CreateMap<PersistedUserAccount, UserAccount>();
            Mapper.CreateMap<UserAccount, PersistedUserAccount>();
            Mapper.CreateMap<DocumentCoded, PersistedDocumentCoded>();
            Mapper.CreateMap<PersistedDocumentCoded, DocumentCoded>();
            Mapper.CreateMap<BatchIsolated, PersistedIsolatedBatch>();
            Mapper.CreateMap<PersistedIsolatedBatch, BatchIsolated>();
        }

        private static CloudStorageAccount GetStorageAccount()
        {
            return CreateStorageAccountFromConnectionString(SetStorageAccountConnectionString());
        }

        private static string SetStorageAccountConnectionString()
        {
            var runtime = System.Configuration.ConfigurationManager.AppSettings["Runtime"];
            if (runtime == "debug")
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var dbPath = Path.Combine(appDataPath, CloudConfigurationManager.GetSetting("DropBoxDb"));
                var lines = File.ReadAllLines(dbPath);
                var dbBase64Text = Convert.FromBase64String(lines[1]);

                string filepath;
                filepath = System.Text.Encoding.ASCII.GetString(dbBase64Text)
                           + CloudConfigurationManager.GetSetting("AzureDevConnection");

                var conJson = JObject.Parse(File.ReadAllText(filepath));
                var constring = conJson["azure"].ToString();
                TableStoreConnectionString = constring;
                return constring;
            }
            TableStoreConnectionString =
                System.Configuration.ConfigurationManager.ConnectionStrings["AzureProdConnection"].ToString();
            return TableStoreConnectionString;
        }

        private static void SetStripeKeyForPurchaseRequests()
        {
            var runtime = System.Configuration.ConfigurationManager.AppSettings["Runtime"];
            if (runtime == "debug")
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var dbPath = Path.Combine(appDataPath, CloudConfigurationManager.GetSetting("DropBoxDb"));
                var lines = File.ReadAllLines(dbPath);
                var dbBase64Text = Convert.FromBase64String(lines[1]);

                string filepath;
                filepath = System.Text.Encoding.ASCII.GetString(dbBase64Text)
                           + CloudConfigurationManager.GetSetting("AzureStripeKey");

                var conJson = JObject.Parse(File.ReadAllText(filepath));
                var constring = conJson["AzureStripeKey"].ToString();
                StripePurchaseString = constring;
            }
            else
            {
                StripePurchaseString = System.Configuration
                                             .ConfigurationManager
                                             .ConnectionStrings["AzureStripeKey"]
                                             .ToString();
            }
        }

        private static CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                Trace.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
                throw;
            }
            catch (ArgumentException)
            {
                Trace.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                throw;

            }

            return storageAccount;
        }
        #endregion
    }
}