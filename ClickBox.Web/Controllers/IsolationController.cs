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

                string rowKey;
                string partitionKey;

                if (account != null)
                {
                    rowKey = persistedIsolatedBatch.BatchId.ToString();
                    partitionKey = account.CompanyName + ":" + persistedIsolatedBatch.ProjectId;
                    persistedIsolatedBatch.RowKey = rowKey;
                    persistedIsolatedBatch.PartitionKey = partitionKey;
                    accountFound = true;
                }
                else
                {
                    // change this to forbidden when we want to stop clients from connecting
                    rowKey = persistedIsolatedBatch.BatchId.ToString();
                    partitionKey = persistedIsolatedBatch.UserName + ":" + persistedIsolatedBatch.ProjectId;
                    persistedIsolatedBatch.RowKey = rowKey;
                    persistedIsolatedBatch.PartitionKey = partitionKey;
                }

                var doc =
                    await
                    this.Client.GetEntityByPartitionAndRowKeyAsync<PersistedIsolatedBatch>(
                        partitionKey,
                        rowKey, true);

                if (doc == null)
                {
                    persistedIsolatedBatch.DateCreated = new DateTimeOffset(DateTime.Now);
                    await this.Client.InsertStorageEntityAsync(persistedIsolatedBatch);
                    var monthlyIsolatedBatch = new MonthlyIsolatedBatch()
                    {
                        RowKey = persistedIsolatedBatch.BatchId.ToString(),
                        ProjectId = persistedIsolatedBatch.ProjectId
                    };
                    await this.Client.InsertStorageEntityAsync(monthlyIsolatedBatch);
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
                    await this.Client.UpdateEntityAsync(doc);
                }
                
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