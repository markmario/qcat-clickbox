// --------------------------------------------------------------------------------------------------
//  <copyright file="RavenDbApiController.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Controllers
{
    using System.Web.Http;
    using System.Web.Mvc;

    using Microsoft.WindowsAzure.Storage.Table;

    [RequireHttps(Order = 1)]
    public abstract class ClickBoxApiController : ApiController
    {
        #region Public Properties

        protected CloudTableClient Client { get; set; }
        
        #endregion
    }
}