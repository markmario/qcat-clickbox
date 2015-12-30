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

    using Microsoft.ApplicationInsights;

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
            var telemetry = new TelemetryClient();

            var accountFound = false;
            try
            {
                if (isolatedBatch == null)
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Request");
                }

                //get the product and then account that the user post the isolated batch is associated to
                var data = await this.Client.GetEntityByPartitionAndRowKeyAsync<Product>("QCAT-Odes");
                var filters =
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("UserName", QueryComparisons.Equal, isolatedBatch.UserName),
                            TableOperators.And,
                            TableQuery.GenerateFilterCondition("Product", QueryComparisons.Equal, "QCAT-Odes"));


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
                        DocumentCount = persistedIsolatedBatch.DocumentsCreated,
                        CompanyName = account != null ? account.CompanyName : UnknownAccount,
                        AccountId = account != null ? account.Id : UnknownAccount,
                    };
                    await this.Client.InsertStorageEntityAsync(monthlyIsolatedBatch);
                }
                else
                {
                    var oldbatchValues = new List<OldDocmentCount>(); ;

                    if (existingBatch.OldBatchValues == null)
                    {
                        oldbatchValues = new List<OldDocmentCount>();
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

                if (accountFound==false)
                {
                    telemetry.TrackTrace("Invalid Account Usage", new Dictionary<string, string>
                    {
                        {"Posted Account User Name", string.IsNullOrEmpty(isolatedBatch.UserName) ? "Unknown Account" : isolatedBatch.UserName}
                    });
                }

                return accountFound ? this.Request.CreateResponse(HttpStatusCode.Created) :
                                      this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Account Details");
            }
            catch (Exception ex)
            {
                Dictionary<string, string> properties;

                if (isolatedBatch == null)
                {
                    properties = new Dictionary<string, string>
                                     {
                                         {
                                             "OccuredAt",
                                             new DateTimeOffset(DateTime.Now).ToString()
                                         },
                                         {
                                             "EmptyPostDataType", typeof(BatchIsolated).FullName
                                         }
                                     };
                     telemetry.TrackException(ex, properties);
                }
                else
                {
                    properties = new Dictionary<string, string>
                                     {
                                         { "OccuredAt", isolatedBatch.DateCreated.ToString()},
                                         { "User Name", isolatedBatch.UserName }
                                     };
                    var measurements = new Dictionary<string, double> { { "DocumentsInFailedCountedBatch", isolatedBatch.DocumentsCreated} };
                    telemetry.TrackException(ex, properties, measurements);
                }
                
                return this.Request.CreateErrorResponse(
                    HttpStatusCode.InternalServerError,
                    ex.Message,
                    ex);
            }
        }

        #endregion
    }
}