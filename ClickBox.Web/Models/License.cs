// --------------------------------------------------------------------------------------------------
//  <copyright file="License.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Models
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;

    public class ClientIssuedLicense : TableEntity, IContainTableReference
    {
        #region Public Properties

        public int ClickCount { get; set; }

        public DateTimeOffset DateCreated { get; set; }

        public DateTimeOffset ExpiryDate { get; set; }

        public string Id { get; set; }

        public string LicenseText { get; set; }

        public string MachineName { get; set; }

        public string ProductId { get; set; }

        public Guid RequestId { get; set; }

        public string Type { get; set; }

        public string UserAccountId { get; set; }

        #endregion

        public string TableName
        {
            get { return "Licences"; }
        }
    }
}