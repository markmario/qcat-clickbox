// --------------------------------------------------------------------------------------------------
//  <copyright file="AutofactMvcConfig.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------

using ClickBox.Web;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(AutofactMvcConfig), "Start")]

namespace ClickBox.Web
{
    using System.Reflection;
    using System.Web.Configuration;
    using System.Web.Http;
    using System.Web.Mvc;

    using Autofac;
    using Autofac.Integration.Mvc;
    using Autofac.Integration.WebApi;

    using ClickBox.Web.Controllers;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    public static class AutofactMvcConfig
    {
        #region Public Methods and Operators

        public static bool IsControllerActionParameterInjectionEnabled()
        {
            bool injectParameters;
            bool.TryParse(
                WebConfigurationManager.AppSettings["EnableControllerActionParameterInjection"], 
                out injectParameters);
            return injectParameters;
        }

        public static void Start()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly(), typeof(LicenseController).Assembly);

            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
            builder.RegisterModelBinderProvider();

            // OPTIONAL: Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            // OPTIONAL: Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());

            // OPTIONAL: Enable property injection into action filters.
            builder.RegisterFilterProvider();

            builder.RegisterType<ExtensibleActionInvoker>()
                .As<IActionInvoker>()
                .WithParameter(
                    new NamedParameter("injectActionMethodParameters", IsControllerActionParameterInjectionEnabled()))
                .InstancePerRequest();

            // builder.Register(c => MvcApplication.DocumentStore.OpenSession())
            // .As<IDocumentSession>()
            // .InstancePerRequest();
            builder.Register(c => MvcApplication.TableStore).As<CloudStorageAccount>().SingleInstance();

            builder.Register(c => MvcApplication.TableStore.CreateCloudTableClient())
                .As<CloudTableClient>()
                .InstancePerRequest();

            // Register the Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly(), typeof(LicenseController).Assembly);

            var config = GlobalConfiguration.Configuration;
            builder.RegisterWebApiFilterProvider(config);

            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            var resolver = new AutofacWebApiDependencyResolver(container);

            GlobalConfiguration.Configuration.DependencyResolver = resolver;
        }

        #endregion
    }
}