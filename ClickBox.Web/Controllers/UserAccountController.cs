// --------------------------------------------------------------------------------------------------
//  <copyright file="UserAccountController.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using AutoMapper;

    using ClickBox.Web.Infrastructure;
    using ClickBox.Web.Models;
    using ClickBox.Web.TableStorage;

    using Kendo.Mvc.Extensions;
    using Kendo.Mvc.UI;

    using Microsoft.WindowsAzure.Storage.Table;

    using Rhino.Licensing;

    [RequireHttps(Order = 1)]
    [RequireLocalHostActionFilter]
    public class UserAccountController : Controller
    {
        #region Fields

        private readonly CloudTableClient client;

        #endregion

        #region Constructors and Destructors

        public UserAccountController(CloudTableClient client)
        {
            this.client = client;
        }

        #endregion

        #region Public Methods and Operators

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Create([DataSourceRequest] DataSourceRequest request, UserAccount account)
        {
            if (account != null && this.ModelState.IsValid)
            {
                account.InitModelBinderVersion(account);
                var licType = Enum.GetName(typeof(LicenseType), account.AccountType);
                var persisted = Mapper.Map<PersistedUserAccount>(account);
                persisted.AccountType = licType;
                await this.client.InsertStorageEntityAsync(persisted);
            }

            return this.Json(new[] { account }.ToDataSourceResult(request, this.ModelState));
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public async Task<ActionResult> Get([DataSourceRequest] DataSourceRequest request)
        {
            var toRet = await this.client.GetEntitiesAsync<PersistedUserAccount>();
            var persistedUserAccounts = toRet as PersistedUserAccount[] ?? toRet.ToArray();
            var viewModels = new List<UserAccount>();
            foreach (var pers in persistedUserAccounts)
            {
                var licType = (LicenseType)Enum.Parse(typeof(LicenseType), pers.AccountType);
                var viewModel = Mapper.Map<UserAccount>(pers);
                viewModel.AccountType = licType;
                viewModels.Add(viewModel);
            }

            return this.Json(viewModels.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return this.View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Update([DataSourceRequest] DataSourceRequest request, UserAccount account)
        {
            if (account != null && this.ModelState.IsValid)
            {
                var target = await this.client.GetEntityByPartitionAndRowKeyAsync<PersistedUserAccount>(account.Id);
                if (target != null)
                {
                    var licType = Enum.GetName(typeof(LicenseType), account.AccountType);
                    target.AccountType = licType;
                    target.Active = account.Active;
                    target.AllocatedSeats = account.AllocatedSeats;
                    target.CompanyName = account.CompanyName;
                    target.ContactName = account.ContactName;
                    target.IsolationEnabled = account.IsolationEnabled;
                    target.KoderEnabled = account.KoderEnabled;
                    target.Password = account.Password;
                    target.SupportEndDate = account.SupportEndDate;
                    target.UserName = account.UserName;
                    target.MaxVersionNumber = account.MaxVersionNumber;
                    await this.client.UpdateEntityAsync(target);
                }
            }

            return this.Json(this.ModelState.ToDataSourceResult());
        }

        #endregion
    }
}