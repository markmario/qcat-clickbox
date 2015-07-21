// --------------------------------------------------------------------------------------------------
//  <copyright file="HomeController.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Controllers
{
    using System.Web.Mvc;

    using ClickBox.Web.Infrastructure;

    [RequireHttps(Order = 1)]
    [RequireLocalHostActionFilter()]
    public class HomeController : Controller
    {
        // GET: /Home/
        #region Public Methods and Operators

        public ActionResult Index()
        {
            //return this.View();
            return this.RedirectToAction("Index", "UserAccount");
        }

        #endregion
    }
}