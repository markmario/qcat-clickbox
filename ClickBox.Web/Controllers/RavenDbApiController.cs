// --------------------------------------------------------------------------------------------------
//  <copyright file="RavenDbApiController.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Controllers
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Mvc;

    using Raven.Client;

    [RequireHttps(Order = 1)]
    public abstract class RavenDbApiController : ApiController
    {
        #region Public Properties

        public IAsyncDocumentSession Session { get; set; }

        public IDocumentStore Store { get; set; }

        #endregion

        #region Public Methods and Operators

        public override async Task<HttpResponseMessage> ExecuteAsync(
            HttpControllerContext controllerContext, 
            CancellationToken cancellationToken)
        {
            using (this.Session = this.Store.OpenAsyncSession())
            {
                var result = await base.ExecuteAsync(controllerContext, cancellationToken);
                await this.Session.SaveChangesAsync();

                return result;
            }
        }

        #endregion
    }
}