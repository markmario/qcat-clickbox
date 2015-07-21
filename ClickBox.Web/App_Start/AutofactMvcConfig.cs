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

    using Raven.Client;

    public static class AutofactMvcConfig
    {
        #region Public Methods and Operators

        public static bool IsControllerActionParameterInjectionEnabled()
        {
            bool injectParameters = false;
            bool.TryParse(
                WebConfigurationManager.AppSettings["EnableControllerActionParameterInjection"], 
                out injectParameters);
            return injectParameters;
        }

        public static void Start()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly(), typeof(LicenseController).Assembly);

            // Register the Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly(), typeof(LicenseController).Assembly);

            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());

            builder.RegisterModelBinderProvider();
            builder.RegisterModule(new AutofacWebTypesModule());

            builder.RegisterType<ExtensibleActionInvoker>()
                .As<IActionInvoker>()
                .WithParameter(
                    new NamedParameter("injectActionMethodParameters", IsControllerActionParameterInjectionEnabled()))
                .InstancePerHttpRequest();

            // builder.Register(
            // c => new FlexMembershipProvider(c.Resolve<IFlexUserStore>(), c.Resolve<IApplicationEnvironment>()))
            // .As<IFlexMembershipProvider>().As<IFlexOAuthProvider>().InstancePerHttpRequest();

            // builder.Register(c => new FlexRoleProvider(c.Resolve<IFlexRoleStore>()))
            // .As<IFlexRoleProvider>()
            // .InstancePerHttpRequest().PropertiesAutowired();
            // builder.Register(c => new FlexMembershipUserStore<User, Role>(c.Resolve<IDocumentSession>())).As<IFlexUserStore>().As<IFlexRoleStore>().PropertiesAutowired();

            // builder.Register(c => new AspnetEnvironment()).As<IApplicationEnvironment>().SingleInstance();
            // builder.Register(c => new DefaultSecurityEncoder()).As<ISecurityEncoder>().SingleInstance();
            builder.Register(c => MvcApplication.DocumentStore.OpenSession())
                .As<IDocumentSession>()
                .InstancePerHttpRequest();
            builder.Register(c => MvcApplication.DocumentStore).As<IDocumentStore>().SingleInstance();

            // builder.Register(c => MvcApplication.Bus).As<IBus>().SingleInstance();
            builder.RegisterFilterProvider();
            IContainer container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            var resolver = new AutofacWebApiDependencyResolver(container);

            GlobalConfiguration.Configuration.DependencyResolver = resolver;
        }

        #endregion
    }
}