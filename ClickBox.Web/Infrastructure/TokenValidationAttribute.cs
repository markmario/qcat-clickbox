// --------------------------------------------------------------------------------------------------
//  <copyright file="TokenValidationAttribute.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Infrastructure
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    public class TokenValidationAttribute : ActionFilterAttribute
    {
        #region Public Methods and Operators

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            string token;

            try
            {
                token = actionContext.Request.Headers.GetValues("Authorization-Token").First();
            }
            catch (Exception)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                                             {
                                                 Content =
                                                     new StringContent
                                                     (
                                                     "Missing Authorization-Token")
                                             };
                return;
            }

            try
            {
                if ("MagicAppV1" == RsaClass.Decrypt(token))
                {
                }
                else
                {
                    actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                                                 {
                                                     Content =
                                                         new StringContent
                                                         (
                                                         "Unauthorized User Account")
                                                 };
                }

                base.OnActionExecuting(actionContext);
            }
            catch (Exception ex)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                                             {
                                                 Content =
                                                     new StringContent
                                                     (
                                                     "Unauthorized User")
                                             };
                return;
            }
        }

        #endregion
    }
}