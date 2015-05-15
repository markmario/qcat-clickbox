// --------------------------------------------------------------------------------------------------
//  <copyright file="CustomHttpsAttribute.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Infrastructure
{
    using System;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    public class CustomHttpsAttribute : ActionFilterAttribute
    {
        #region Public Methods and Operators

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!string.Equals(actionContext.Request.RequestUri.Scheme, "https", StringComparison.OrdinalIgnoreCase))
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                                             {
                                                 Content =
                                                     new StringContent
                                                     (
                                                     "HTTPS Required")
                                             };
                return;
            }
        }

        #endregion
    }
}