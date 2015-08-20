// --------------------------------------------------------------------------------------------------
//  <copyright file="RouteConfig.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RouteConfig
    {
        #region Public Methods and Operators

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default", 
                url: "{controller}/{action}/{id}", 
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional });
        }

        #endregion
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
}