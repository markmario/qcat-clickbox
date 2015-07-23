// --------------------------------------------------------------------------------------------------
//  <copyright file="Global.asax.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web
{
    using System;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Raven.Client;
    using Raven.Client.Document;
    using Microsoft.WindowsAzure.Storage;
    using System.Diagnostics;

    using ClickBox.Web.Models;

    using Microsoft.WindowsAzure;




    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        #region Constructors and Destructors

        public MvcApplication()
        {
            this.BeginRequest +=
                (sender, args) =>
                    {
                        HttpContext.Current.Items["CurrentRequestRavenSession"] =
                            DependencyResolver.Current.GetService<IDocumentSession>();
                    };
            this.EndRequest += (sender, args) =>
                {
                    using (var session = (IDocumentSession)HttpContext.Current.Items["CurrentRequestRavenSession"])
                    {
                        if (session == null)
                        {
                            return;
                        }

                        if (this.Server.GetLastError() != null)
                        {
                            return;
                        }

                        session.SaveChanges();
                    }

                    // TaskExecutor.StartExecuting();

                    // MiniProfiler.Stop();
                };
        }

        #endregion

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
            this.SetAutoMappings();
        }

        private void SetAutoMappings()
        {
            AutoMapper.Mapper.CreateMap<PersistedUserAccount, UserAccount>();
            AutoMapper.Mapper.CreateMap<UserAccount, PersistedUserAccount>();
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