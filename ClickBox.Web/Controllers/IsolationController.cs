// --------------------------------------------------------------------------------------------------
//  <copyright file="IsolationController.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Controllers
{
    using System;
    using System.Collections.Generic;
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
    public class IsolationController : RavenDbApiController
    {
        #region Constructors and Destructors

        public IsolationController(IDocumentStore store)
        {
            this.Store = store;
        }

        #endregion

        #region Public Methods and Operators

        [HttpPost]
        public async Task<HttpResponseMessage> PostIsoaltionBatch(BatchIsolated isolatedBatch)
        {
            var accountFound = false;
            try
            {
                if (isolatedBatch == null)
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Request");
                }

                var data = await this.Session.LoadAsync<Product>("6068d2a8-9685-4cdc-a6b0-9fb17004469b");
                var accounts =
                    await
                    this.Session.Query<UserAccount>().Where(u => u.UserName == isolatedBatch.UserName).ToListAsync();

                // var licx = await Session.Query<ClientIssuedLicense>().Where(u => u.RequestId == codedDoc.RequestId).ToListAsync();
                UserAccount account;
                if (accounts.Count == 1)
                {
                    account = accounts[0];
                    isolatedBatch.Id = account.Id + "/" + isolatedBatch.ProjectId + "/" + isolatedBatch.BatchId;
                    accountFound = true;
                }
                else
                {
                    // change this to forbidden when we want to stop clients from connecting
                    isolatedBatch.Id = isolatedBatch.UserName + "/" + isolatedBatch.ProjectId + "/"
                                       + isolatedBatch.BatchId;
                }

                var doc = await this.Session.LoadAsync<BatchIsolated>(isolatedBatch.Id);

                if (doc == null)
                {
                    await this.Session.StoreAsync(isolatedBatch);
                }
                else
                {
                    if (doc.OldBatchValues == null)
                    {
                        doc.OldBatchValues = new List<OldDocmentCount>();
                    }

                    if (doc.DocumentsCreated > isolatedBatch.DocumentsCreated)
                    {
                        // Notify that this happened.
                    }

                    if (doc.OldBatchValues.Count >= 3)
                    {
                        // Notify that this happened.
                    }

                    doc.OldBatchValues.Add(new OldDocmentCount(doc.DocumentsCreated, doc.DateCreated));
                    doc.DocumentsCreated = isolatedBatch.DocumentsCreated;
                    doc.DateCreated = isolatedBatch.DateCreated;
                }

                await this.Session.SaveChangesAsync();
                if (accountFound)
                {
                    return this.Request.CreateResponse(HttpStatusCode.Created);
                }
                else
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Account Details");
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