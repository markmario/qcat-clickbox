// --------------------------------------------------------------------------------------------------
//  <copyright file="LicenseController.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Mvc;

    using ClickBox.Web.Models;
    using ClickBox.Web.TableStorage;

    using Microsoft.WindowsAzure.Storage.Table;

    using Odes.Licence.Model;

    using Rhino.Licensing;

    using Product = ClickBox.Web.Models.Product;

    /// <summary>
    /// The license controller.
    /// </summary>
    [RequireHttps(Order = 1)]
    public class LicenseController : ApiController
    {
        #region Fields

        /// <summary>
        /// The _attributes.
        /// </summary>
        private Dictionary<string, string> attributes;

        private readonly CloudTableClient client;

        #endregion

        #region Constructors and Destructors

        public LicenseController(CloudTableClient client) {
            this.client = client;
        }

        #endregion

        #region Public Methods and Operators
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetProductDetail(string productName)
        {
            var prod = await client.GetEntityByPartitionAndRowKeyAsync<Product>(productName);
            if (prod != null)
            {
                return this.Request.CreateResponse(
                    HttpStatusCode.OK,
                    new Odes.Licence.Model.Product { Id = prod.Id });
            }

            return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, new HttpError("No Product found by name " + productName));
        }

        /// <summary>
        /// The post license request.
        /// </summary>
        /// <param name="licx">
        /// The license.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<HttpResponseMessage> PostLicenseRequest(WebLicenseRequest licx)
        {
            try
            {
                if (licx == null)
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, new HttpError("Invalid Request. No license request provided."));
                }

                var data = await client.GetEntityByPropertyFilterAsync<Product>("Id", licx.ProductId.ToString());

                var filters = (TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, new UserAccount().PartitionKey),
                        TableOperators.And,
                        TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("UserName", QueryComparisons.Equal, licx.Email),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("Password", QueryComparisons.Equal, licx.Password))));

                var accounts = await client.GetEntityListByPropertyFilterListAsync<UserAccount>(filters);

                UserAccount account;
                var enumerable = accounts as UserAccount[] ?? accounts.ToArray();
                if (enumerable.Count() == 1)
                {
                    account = enumerable[0];
                }
                else
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.Forbidden, new HttpError("Invaid Account Details"));
                }

                filters = (TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, new ClientIssuedLicense().PartitionKey),
                        TableOperators.And,
                        TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("MachineName", QueryComparisons.Equal, licx.SystemMachineName),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("UserAccountId", QueryComparisons.Equal, account.Id))));

                var oldRequests = await client.GetEntityListByPropertyFilterListAsync<ClientIssuedLicense>(filters);

                var clientIssuedLicenses = oldRequests as ClientIssuedLicense[] ?? oldRequests.ToArray();
                if (clientIssuedLicenses.Any())
                {
                    if (account.SupportEndDate != clientIssuedLicenses[0].ExpiryDate)
                    {
                        account.IssuedLicenses--;
                        await client.DeleteEntityAsync(clientIssuedLicenses[0]);
                    }
                    else
                    {
                        return this.Request.CreateResponse(HttpStatusCode.OK, clientIssuedLicenses[0].LicenseText);
                    }
                }

                if (account.IssuedLicenses >= account.AllocatedSeats)
                {
                    return this.Request.CreateErrorResponse(
                        HttpStatusCode.BadRequest,
                        new HttpError("Too many activations please contact QCAT"));
                }

                var generator = new LicenseGenerator(data.PrivateKey);

                this.attributes = GetAttributesForLicense(licx, account, data);

                var key = generator.Generate(
                    account.ContactName,
                    licx.RequestId,
                    account.SupportEndDate,
                    this.attributes,
                    LicenseType.Subscription);

                var lic = new ClientIssuedLicense
                {
                    ClickCount = licx.ClicksReqeusted,
                    DateCreated = DateTimeOffset.UtcNow,
                    ExpiryDate = account.SupportEndDate,
                    LicenseText = key,
                    ProductId = data.Id,
                    RequestId = licx.RequestId,
                    MachineName = licx.SystemMachineName,
                    UserAccountId = account.Id
                };

                account.IssuedLicenses = account.IssuedLicenses + 1;

                await client.InsertStorageEntityAsync(lic);
                await client.InsertStorageEntityAsync(licx);
                return this.Request.CreateResponse(HttpStatusCode.Created, key);
            }
            catch (Exception ex)
            {
                return this.Request.CreateErrorResponse(
                    HttpStatusCode.InternalServerError,
                    "There was a problem licensing your product. Please contant QCAT.",
                    ex);
            }
        }

        private static Dictionary<string, string> GetAttributesForLicense(ILicenseRequest licx, UserAccount account, Product product)
        {
            if (account.PageMakerEnabled)
            {
                return new Dictionary<string, string>
                           {
                               { "SID", licx.SystemId },
                               { "MachineName", licx.SystemMachineName },
                               { "RequestId", licx.RequestId.ToString() },
                               { "AccountName", licx.Email },
                               { "RequestIp", string.IsNullOrEmpty(licx.PublicIp) ? "Unknown"  : licx.PublicIp },
                               { "CompanyName", account.CompanyName },
                               { "ContactName", account.ContactName },
                               { "ProductName", product.Name },
                               { "ProductId", product.Id }
                           };
            }
            else
            {
                return new Dictionary<string, string>
                           {
                               { "SID", licx.SystemId },
                               { "MachineName", licx.SystemMachineName },
                               { "RequestId", licx.RequestId.ToString() },
                               { "ServiceQueue",  string.IsNullOrEmpty(licx.ServiceQueue)  ? "Undefined" : licx.ServiceQueue },
                               { "ClicksRequested", licx.ClicksReqeusted.ToString(CultureInfo.InvariantCulture) },
                               { "AccountEmail", licx.Email },
                               { "RequestIp", string.IsNullOrEmpty(licx.PublicIp) ? "Unknown" : licx.PublicIp },
                               { "CompanyName", account.CompanyName },
                               { "ContactName", account.ContactName },
                               { "MaxVersion", account.MaxVersionNumber },
                               { "IsEnterprise", account.IsEnterprise.ToString() }
                           };
            }
        }

        #endregion
    }
}