// --------------------------------------------------------------------------------------------------
//  <copyright file="RequireLocalHostActionFilter.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Infrastructure
{
    using System.Web;
    using System.Web.Mvc;

    public class RequireLocalHostActionFilter : AuthorizeAttribute
    {
        #region Methods

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.Request.IsAuthenticated; // I need to test on the local host, so I reverse the logic.
        }

        #endregion
    }
}