// --------------------------------------------------------------------------------------------------
//  <copyright file="Global.asax.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web
{
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;

    using ClickBox.Web.App_Start;

    using Raven.Client;
    using Raven.Client.Document;

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

        public static IDocumentStore DocumentStore { get; set; }

        #endregion

        #region Methods

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var ds = new DocumentStore() { ConnectionStringName = "RavenDB" };
            DocumentStore = ds.Initialize();

            Seed.It();
        }

        #endregion
    }
}