// --------------------------------------------------------------------------------------------------
//  <copyright file="UserAccountController.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Controllers
{
    using System.Web.Mvc;

    using ClickBox.Web.Infrastructure;
    using ClickBox.Web.Models;

    using Kendo.Mvc.Extensions;
    using Kendo.Mvc.UI;

    using Raven.Client;

    [RequireHttps(Order = 1)]
    [RequireLocalHostActionFilter()]
    public class UserAccountController : Controller
    {
        #region Fields

        private readonly IDocumentSession _session;

        #endregion

        // GET: /UserAccount/
        #region Constructors and Destructors

        public UserAccountController(IDocumentSession session)
        {
            this._session = session;
        }

        #endregion

        #region Public Methods and Operators

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, UserAccount account)
        {
            if (account != null && this.ModelState.IsValid)
            {
                this._session.Store(account);
                this._session.SaveChanges();
            }

            return this.Json(new[] { account }.ToDataSourceResult(request, this.ModelState));
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Get([DataSourceRequest] DataSourceRequest request)
        {
            // var toRet = _ctx.Applications.OrderBy(o => o.Id).ToDataSourceResult(request);
            var toRet = this._session.Query<UserAccount>().ToDataSourceResult(request);
            return this.Json(toRet, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return this.View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, UserAccount account)
        {
            if (account != null && this.ModelState.IsValid)
            {
                var target = this._session.Load<UserAccount>(account.Id);
                if (target != null)
                {
                    target.AccountType = account.AccountType;
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

                    // _session.Store(account);
                    this._session.SaveChanges();
                }
            }

            return this.Json(this.ModelState.ToDataSourceResult());
        }

        #endregion
    }
}