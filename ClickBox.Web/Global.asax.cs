// --------------------------------------------------------------------------------------------------
//  <copyright file="Global.asax.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web
{
    using System;
    using System.Diagnostics;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;

    using AutoMapper;

    using ClickBox.Web.Models;
    using ClickBox.Web.TableStorage;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;

    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : HttpApplication
    {
        #region Public Properties
        
        public static CloudStorageAccount TableStore { get; private set; }

        #endregion

        #region Methods

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            TableStore = GetStorageAccount();
            SetAutoMappings();
            PrimeTableStorage();
        }

        private async static void PrimeTableStorage()
        {
            await TableStorageUtil.PrimeTable<PersistedUserAccount>();
            await TableStorageUtil.PrimeTable<Product>();
            await TableStorageUtil.PrimeTable<ClientIssuedLicense>();
            await TableStorageUtil.PrimeTable<WebLicenseRequest>();
        }

        private static void SetAutoMappings()
        {
            Mapper.CreateMap<PersistedUserAccount, UserAccount>();
            Mapper.CreateMap<UserAccount, PersistedUserAccount>();
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