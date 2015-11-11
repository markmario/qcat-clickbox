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
    using Models;
    using TableStorage;
    using Microsoft.WindowsAzure.Storage.Table;

    using Newtonsoft.Json;

    using Odes.Licence.Model;
    using Product = Models.Product;

    [RequireHttps(Order = 1)]
    public class IsolationController : ClickBoxApiController
    {
        private const string UnknownAccount = "Unknown account (possibly unlicensed)";

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

                //get the product and then account that the user post the isolated batch is associated to
                var data = await this.Client.GetEntityByPartitionAndRowKeyAsync<Product>("ODES");
                var filters =
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("UserName", QueryComparisons.Equal, isolatedBatch.UserName),
                            TableOperators.And,
                            TableQuery.GenerateFilterCondition("Product", QueryComparisons.Equal, "ODES"));

                
                var account = await this.Client.GetEntityByPropertyFilterListAsync<UserAccount>(filters);

                //map to storage type
                var persistedIsolatedBatch = Mapper.Map<PersistedIsolatedBatch>(isolatedBatch);
                persistedIsolatedBatch.AccountId = account == null ? "Unknown account" : account.Id;

                string rowKey;
                string partitionKey;

                if (account != null)
                {
                    rowKey = persistedIsolatedBatch.BatchId.ToString();
                    partitionKey = persistedIsolatedBatch.ProjectId.ToString();
                    persistedIsolatedBatch.RowKey = rowKey;
                    persistedIsolatedBatch.PartitionKey = partitionKey;
                    accountFound = true;
                }
                else
                {
                    // change this to forbidden when we want to stop clients from connecting
                    // this is unlicensed---------------------------------------------------
                    // need to check again why we are allowing this for now - so it can be documented
                    rowKey = persistedIsolatedBatch.BatchId.ToString();
                    partitionKey = persistedIsolatedBatch.ProjectId.ToString();
                    persistedIsolatedBatch.RowKey = rowKey;
                    persistedIsolatedBatch.PartitionKey = partitionKey;
                }

                var existingBatch =
                    await
                    this.Client.GetEntityByPartitionAndRowKeyAsync<PersistedIsolatedBatch>(
                        rowKey, partitionKey);

                if (existingBatch == null)
                {
                    persistedIsolatedBatch.DateCreated = new DateTimeOffset(DateTime.Now);
                    await this.Client.InsertStorageEntityAsync(persistedIsolatedBatch);
                    var monthlyIsolatedBatch = new MonthlyIsolatedBatch()
                    {
                        RowKey = persistedIsolatedBatch.BatchId.ToString(),
                        ProjectId = persistedIsolatedBatch.ProjectId,
                        UserName = persistedIsolatedBatch.UserName,
                        CompanyName = account != null ? account.CompanyName : UnknownAccount,
                        AccountId = account != null ? account.Id : UnknownAccount,
                    };
                    await this.Client.InsertStorageEntityAsync(monthlyIsolatedBatch);
                }
                else
                {
                    var oldbatchValues = new List<OldDocmentCount>();;

                    if (existingBatch.OldBatchValues == null)
                    {
                        oldbatchValues =  new List<OldDocmentCount>();
                    }

                    if (existingBatch.DocumentsCreated > isolatedBatch.DocumentsCreated)
                    {
                        // Notify that this happened.
                        // this batch would now have less documents to re code
                        // and should be charged less??? do we care?
                    }

                    //if (doc.OldBatchValues.Count >= 3)
                    //{
                    //    // Notify that this happened.
                    //}

                    if (existingBatch.OldBatchValues != null)
                    {
                        oldbatchValues = JsonConvert.DeserializeObject<List<OldDocmentCount>>(existingBatch.OldBatchValues);
                    }
                    
                    //record old data for batches that re isolated
                    oldbatchValues.Add(new OldDocmentCount(existingBatch.DocumentsCreated, existingBatch.DateCreated));
                    existingBatch.OldBatchValues = JsonConvert.SerializeObject(oldbatchValues);
                    existingBatch.DocumentsCreated = isolatedBatch.DocumentsCreated;
                    existingBatch.DateCreated = isolatedBatch.DateCreated;
                    await this.Client.UpdateEntityAsync(existingBatch);
                }
                
                return accountFound ? this.Request.CreateResponse(HttpStatusCode.Created) : 
                                      this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Account Details");
            }
            catch (Exception ex)
            {
                //log to table storage
                return this.Request.CreateErrorResponse(
                    HttpStatusCode.InternalServerError,
                    ex.Message,
                    ex);
            }
        }

        #endregion
    }
}