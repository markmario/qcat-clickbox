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

    using ClickBox.Web.Models;
    using ClickBox.Web.TableStorage;

    using Microsoft.WindowsAzure.Storage.Table;

    using Odes.Licence.Model;

    using Product = ClickBox.Web.Models.Product;

    [RequireHttps(Order = 1)]
    public class KodingController : ClickBoxApiController
    {
        #region Constructors and Destructors

        public KodingController()
        {
        }

        public KodingController(CloudTableClient client)
        {
            this.Client = client;
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

                var data = this.Client.GetEntityByPartitionAndRowKey<Product>("QCAT-Odes");
                var account =
                    this.Client.GetEntityByPropertyFilterAsync<UserAccount>("UserName", codedDoc.UserName).Result;

                var persistedDoc = Mapper.Map<PersistedDocumentCoded>(codedDoc);
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

                // var doc =
                // await this.Client.GetEntityByPropertyFilterAsync<PersistendDocumentCoded>("Id", persistedDoc.Id);
                var doc =
                    await
                    this.Client.GetEntityByPartitionAndRowKeyAsync<PersistedDocumentCoded>(
                        persistedDoc.Id, 
                        persistedDoc.ProjectId.ToString(), 
                        true);

                if (doc == null)
                {
                    //this is the weird line of code
                    codedDoc.DateCreated = new DateTimeOffset(DateTime.Now); 
                    await this.Client.InsertStorageEntityAsync(persistedDoc);
                    var monthlyDoc = new MonthlyCodedDocument()
                                         {
                                             RowKey = persistedDoc.DocumentId.ToString(), 
                                             ProjectId = persistedDoc.ProjectId
                                         };
                    await this.Client.InsertStorageEntityAsync(monthlyDoc);
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