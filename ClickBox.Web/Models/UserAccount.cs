// --------------------------------------------------------------------------------------------------
//  <copyright file="UserAccount.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Rhino.Licensing;

    public class UserAccount
    {
        #region Public Properties

        [UIHint("LicenseTypeEditor")]
        public LicenseType AccountType { get; set; }

        public bool Active { get; set; }

        [UIHint("Integer")]
        public int AllocatedSeats { get; set; }

        public string CompanyName { get; set; }

        public string ContactName { get; set; }

        [ScaffoldColumn(false)]
        public string Id { get; set; }

        public bool IsEnterprise { get; set; }

        public bool IsolationEnabled { get; set; }

        public bool PageMakerEnabled { get; set; }

        [ScaffoldColumn(false)]
        [UIHint("Integer")]
        public int IssuedLicenses { get; set; }

        public bool KoderEnabled { get; set; }

        public string MaxVersionNumber { get; set; }

        public string Password { get; set; }

        [ScaffoldColumn(false)]
        public string Salt { get; set; }

        [UIHint("Date")]
        public DateTime SupportEndDate { get; set; }

        public string UserName { get; set; }

        #endregion
    }
}