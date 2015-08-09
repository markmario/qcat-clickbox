// --------------------------------------------------------------------------------------------------
//  <copyright file="IsolationController.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AutoMapper;
    using ClickBox.Web.Models;
    using ClickBox.Web.TableStorage;
    using Microsoft.WindowsAzure.Storage.Table;

    using Newtonsoft.Json;

    using Odes.Licence.Model;
    using Product = ClickBox.Web.Models.Product;

    [RequireHttps(Order = 1)]
    public class IsolationController : ClickBoxApiController
    {
        #region Constructors and Destructors

        public IsolationController(CloudTableClient client)
        {
            this.Client = client;
        }

        #endregion

        #region Public Methods and Operators

        [HttpPost]
        public async Task<HttpResponseMessage> PostIsoaltionBatch(BatchIsolated isolatedBatch)
        {
            //return  this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError,  "Incomplete");
            var accountFound = false;
            try
            {
                if (isolatedBatch == null)
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Request");
                }

                var data = await this.Client.GetEntityByPartitionAndRowKeyAsync<Product>("QCAT-Odes");
                var account = this.Client.GetEntityByPropertyFilterAsync<UserAccount>("UserName", isolatedBatch.UserName).Result;

                var persistedIsolatedBatch = Mapper.Map<PersistedIsolatedBatch>(isolatedBatch);
                if (account != null)
                {
                    //account = accounts[0];
                    persistedIsolatedBatch.Id = account.Id + ":" + persistedIsolatedBatch.ProjectId + ":" + persistedIsolatedBatch.BatchId;
                    accountFound = true;
                }
                else
                {
                    // change this to forbidden when we want to stop clients from connecting
                    persistedIsolatedBatch.Id = persistedIsolatedBatch.UserName + ":" + persistedIsolatedBatch.ProjectId + ":"
                                       + persistedIsolatedBatch.BatchId;
                }

                //var doc = await this.Session.LoadAsync<BatchIsolated>(isolatedBatch.Id);
                var doc =
                    await
                    this.Client.GetEntityByPartitionAndRowKeyAsync<PersistedIsolatedBatch>(
                        persistedIsolatedBatch.Id,
                        persistedIsolatedBatch.ProjectId.ToString(), true);

                if (doc == null)
                {
                    await this.Client.InsertStorageEntityAsync(persistedIsolatedBatch);
                }
                else
                {
                    var oldbatchValues = new List<OldDocmentCount>();;

                    if (doc.OldBatchValues == null)
                    {
                        oldbatchValues =  new List<OldDocmentCount>();
                    }

                    if (doc.DocumentsCreated > isolatedBatch.DocumentsCreated)
                    {
                        // Notify that this happened.
                    }

                    //if (doc.OldBatchValues.Count >= 3)
                    //{
                    //    // Notify that this happened.
                    //}

                    if (doc.OldBatchValues != null)
                    {
                        oldbatchValues = JsonConvert.DeserializeObject<List<OldDocmentCount>>(doc.OldBatchValues);
                    }
                    
                    oldbatchValues.Add(new OldDocmentCount(doc.DocumentsCreated, doc.DateCreated));
                    doc.OldBatchValues = JsonConvert.SerializeObject(oldbatchValues);
                    doc.DocumentsCreated = isolatedBatch.DocumentsCreated;
                    doc.DateCreated = isolatedBatch.DateCreated;
                }

                //await this.Session.SaveChangesAsync();
                await this.Client.UpdateEntityAsync(doc);
                return accountFound ? this.Request.CreateResponse(HttpStatusCode.Created) : 
                                      this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Account Details");
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