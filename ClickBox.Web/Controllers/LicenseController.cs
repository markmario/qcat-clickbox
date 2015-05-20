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
    using System.Security;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Mvc;

    using ClickBox.Web.Models;

    using Odes.Licence.Model;

    using Raven.Client;

    using Rhino.Licensing;

    /// <summary>
    /// The license controller.
    /// </summary>
    [RequireHttps(Order = 1)]
    public class LicenseController : RavenDbApiController
    {
        #region Fields

        /// <summary>
        /// The _attributes.
        /// </summary>
        private Dictionary<string, string> attributes;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseController"/> class.
        /// </summary>
        /// <param name="store">
        /// The store.
        /// </param>
        public LicenseController(IDocumentStore store)
        {
            this.Store = store;
        }

        #endregion

        #region Public Methods and Operators

        public async Task<HttpResponseMessage> GetProductDetail([FromUri] string productName)
        {
            var prod = await this.Session.Query<Product>().Where(p => p.Name == productName).FirstOrDefaultAsync();
            if (prod != null)
            {
                return this.Request.CreateResponse(HttpStatusCode.OK, new { Id = prod.Id, Key = prod.PublicKey });
            }
            return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Product Found");
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
        public async Task<HttpResponseMessage> PostLicenseRequest(LicenseRequest licx)
        {
            try
            {
                if (licx == null)
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Request");
                }

                if (licx.ProductId == Guid.Empty)
                {
                    licx.ProductId = new Guid("6068d2a8-9685-4cdc-a6b0-9fb17004469b");
                }

                var data = await this.Session.LoadAsync<Product>(licx.ProductId);
                IList<UserAccount> accounts =
                    await
                    this.Session.Query<UserAccount>()
                        .Where(u => u.UserName == licx.Email && u.Password == licx.Password)
                        .ToListAsync();

                UserAccount account;
                if (accounts.Count == 1)
                {
                    account = accounts[0];
                }
                else
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Invaid Account Details");
                }

                IList<ClientIssuedLicense> oldRequests =
                    await
                    this.Session.Query<ClientIssuedLicense>()
                        .Where(u => u.MachineName == licx.SystemMachineName && u.UserAccountId == account.Id)
                        .ToListAsync();

                if (oldRequests.Count > 0)
                {
                    if (account.SupportEndDate != oldRequests[0].ExpiryDate)
                    {
                        this.Session.Delete(oldRequests[0]);
                        await this.Session.SaveChangesAsync();
                    }
                    else
                    {
                        return this.Request.CreateResponse(HttpStatusCode.OK, oldRequests[0].LicenseText);
                    }
                }

                var generator = new LicenseGenerator(data.PrivateKey);
                this.attributes = new Dictionary<string, string>
                                      {
                                          { "SID", licx.SystemId }, 
                                          { "MachineName", licx.SystemMachineName }, 
                                          { "RequestId", licx.RequestId.ToString() }, 
                                          {
                                              "ServiceQueue", 
                                              string.IsNullOrEmpty(licx.ServiceQueue)
                                                  ? "Undefined"
                                                  : licx.ServiceQueue
                                          }, 
                                          {
                                              "ClicksRequested", 
                                              licx.ClicksReqeusted.ToString(
                                                  CultureInfo.InvariantCulture)
                                          }, 
                                          { "AccountEmail", licx.Email }, 
                                          {
                                              "RequestIp", 
                                              string.IsNullOrEmpty(licx.PublicIp)
                                                  ? "Unknown"
                                                  : licx.PublicIp
                                          }, 
                                          { "CompanyName", account.CompanyName }, 
                                          { "ContactName", account.ContactName }, 
                                          { "MaxVersion", account.MaxVersionNumber }, 
                                          { "IsEnterprise", account.IsEnterprise.ToString() }
                                      };

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
                await this.Session.StoreAsync(lic);
                await this.Session.StoreAsync(licx);
                return this.Request.CreateResponse(HttpStatusCode.Created, key);
            }
            catch (Exception ex)
            {
                return this.Request.CreateErrorResponse(
                    HttpStatusCode.InternalServerError, 
                    "Some bad things happened", 
                    ex);
            }
        }

        #endregion
    }
}