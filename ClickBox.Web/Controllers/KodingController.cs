// --------------------------------------------------------------------------------------------------
//  <copyright file="KodingController.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Controllers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using AutoMapper;

    using Models;
    using TableStorage;

    using Microsoft.WindowsAzure.Storage.Table;

    using Odes.Licence.Model;

    using Product = ClickBox.Web.Models.Product;

    [RequireHttps(Order = 1)]
    public class KodingController : ClickBoxApiController
    {
        private const string UnknownAccount = "Unknown account (possibly unlicensed)";

        #region Constructors and Destructors

        public KodingController()
        {
        }

        public KodingController(CloudTableClient client)
        {
            Client = client;
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

                var data = this.Client.GetEntityByPartitionAndRowKey<Product>("ODES");
                var account =
                    this.Client.GetEntityByPropertyFilterAsync<UserAccount>("UserName", codedDoc.UserName).Result;

                var persistedDoc = Mapper.Map<PersistedDocumentCoded>(codedDoc);

                var monthlyStatUserName = string.IsNullOrEmpty(persistedDoc.UserName) 
                                             ? "Unknown koding user" : persistedDoc.UserName;

                if (account != null)
                {
                    persistedDoc.Id = account.Id + ":" + persistedDoc.ProjectId + ":" + persistedDoc.DocumentId;
                    accountFound = true;
                }
                else
                {
                    persistedDoc.Id = persistedDoc.UserName + ":" + persistedDoc.ProjectId + ":"
                                      + persistedDoc.DocumentId;
                }

                var doc =
                    await
                    this.Client.GetEntityByPartitionAndRowKeyAsync<PersistedDocumentCoded>(
                        persistedDoc.Id, 
                        persistedDoc.ProjectId.ToString());

                //if its being reviewed (i.e not null) then don't record in storage
                if (doc == null)
                {
                    //this is the weird line of code
                    codedDoc.DateCreated = new DateTimeOffset(DateTime.Now); 
                    await this.Client.InsertStorageEntityAsync(persistedDoc);
                    var monthlyDoc = new MonthlyCodedDocument()
                                         {
                                             RowKey = persistedDoc.DocumentId.ToString(), 
                                             ProjectId = persistedDoc.ProjectId,
                                             UserName = monthlyStatUserName,
                                             CompanyName = account != null ? account.CompanyName : UnknownAccount,
                                             AccountId = account != null ? account.Id : UnknownAccount,
                    };
                    await this.Client.InsertStorageEntityAsync(monthlyDoc);
                }

                return accountFound ? this.Request.CreateResponse(HttpStatusCode.Created) 
                                    : this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Account Details");

                // change this to forbidden when we want to stop clients from connecting
            }
            catch (Exception ex)
            {
                //log error to table storage
                return this.Request.CreateErrorResponse(
                    HttpStatusCode.InternalServerError, 
                    ex.Message, 
                    ex);
            }
        }

        #endregion
    }
}