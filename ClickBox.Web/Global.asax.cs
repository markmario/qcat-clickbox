// --------------------------------------------------------------------------------------------------
//  <copyright file="Global.asax.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;

    using AutoMapper;

    using Microsoft.Azure;

    using Models;
    using TableStorage;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;

    using Odes.Licence.Model;

    using Product = ClickBox.Web.Models.Product;

    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : HttpApplication
    {
        #region Public Properties
        
        public static CloudStorageAccount TableStore { get; private set; }

        #endregion

        #region Methods

        protected async void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            TableStore = GetStorageAccount();

            SetAutoMappings();
            await this.PrimeTableStorage();
        }

        private async Task PrimeTableStorage()
        {
            var client = TableStore.CreateCloudTableClient();
            await client.PrimeTable<PersistedUserAccount>();
            await client.PrimeTable<Product>();
            await client.PrimeTable<ClientIssuedLicense>();
            await client.PrimeTable<WebLicenseRequest>();
            await client.PrimeTable<PersistedIsolatedBatch>();
            await client.PrimeTable<MonthlyCodedDocument>();
            await client.PrimeTable<MonthlyIsolatedBatch>();
        }

        private static void SetAutoMappings()
        {
            Mapper.CreateMap<PersistedUserAccount, UserAccount>();
            Mapper.CreateMap<UserAccount, PersistedUserAccount>();
            Mapper.CreateMap<DocumentCoded, PersistendDocumentCoded>();
            Mapper.CreateMap<PersistendDocumentCoded, DocumentCoded>();
            Mapper.CreateMap<BatchIsolated, PersistedIsolatedBatch>();
            Mapper.CreateMap<PersistedIsolatedBatch, BatchIsolated>();
        }

        private static CloudStorageAccount GetStorageAccount()
        {
            var debug = CloudConfigurationManager.GetSetting("Runtime");
            if (debug == "debug")
            {
                return CloudStorageAccount.DevelopmentStorageAccount;
            }
            return CreateStorageAccountFromConnectionString(CloudConfigurationManager.GetSetting("StorageConnectionString"));
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