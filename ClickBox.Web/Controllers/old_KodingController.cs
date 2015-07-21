// --------------------------------------------------------------------------------------------------
//  <copyright file="KodingController.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using ClickBox.Web.Models;

    using Odes.Licence.Model;

    using Raven.Client;

    using Product = ClickBox.Web.Models.Product;

    [RequireHttps(Order = 1)]
    public class old_KodingController : RavenDbApiController
    {
        #region Constructors and Destructors

        public old_KodingController(IDocumentStore store)
        {
            this.Store = store;
        }

        #endregion

        #region Public Methods and Operators

        [HttpPost]
        public async Task<HttpResponseMessage> PostCodedDocument(DocumentCoded codedDoc)
        {
            var accountFound = false;
            try
            {
                if (codedDoc == null)
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Request");
                }

                var data = await this.Session.LoadAsync<Product>("6068d2a8-9685-4cdc-a6b0-9fb17004469b");
                var accounts =
                    await this.Session.Query<UserAccount>().Where(u => u.UserName == codedDoc.UserName).ToListAsync();

                // var licx = await Session.Query<ClientIssuedLicense>().Where(u => u.RequestId == codedDoc.RequestId).ToListAsync();
                UserAccount account;
                if (accounts.Count == 1)
                {
                    account = accounts[0];
                    codedDoc.Id = account.Id + "/" + codedDoc.ProjectId + "/" + codedDoc.DocumentId;
                    accountFound = true;
                }
                else
                {
                    codedDoc.Id = codedDoc.UserName + "/" + codedDoc.ProjectId + "/" + codedDoc.DocumentId;
                }

                var doc = await this.Session.LoadAsync<DocumentCoded>(codedDoc.Id);
                if (doc == null)
                {
                    await this.Session.StoreAsync(codedDoc);
                }

                if (accountFound)
                {
                    return this.Request.CreateResponse(HttpStatusCode.Created);
                }
                else
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Account Details");
                        
                        // change this to forbidden when we want to stop clients from connecting
                }
            }
            catch (Exception ex)
            {
                return this.Request.CreateErrorResponse(
                    HttpStatusCode.InternalServerError, 
                    "Some bad shit happened", 
                    ex);
            }
        }

        #endregion
    }
}