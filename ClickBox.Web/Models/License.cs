// --------------------------------------------------------------------------------------------------
//  <copyright file="License.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Models
{
    using System;
    using System.Web.Mvc;

    using ClickBox.Web.TableStorage;

    using Microsoft.WindowsAzure.Storage.Table;

    [Bind(Exclude = "Timestamp, TableName, RowKey, PartitionKey, ETag")]
    public class ClientIssuedLicense : TableEntity, IContainTableReference
    {
        private string id;

        public ClientIssuedLicense()
        {
            this.Id = Guid.NewGuid().ToString();
            this.PartitionKey = TableStorageUtil.GetPartitionPrefix() + 3;
        }

        #region Public Properties

        public int ClickCount { get; set; }

        public DateTimeOffset DateCreated { get; set; }

        public DateTimeOffset ExpiryDate { get; set; }

        public string Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
                this.RowKey = value;
            }
        }

        public string LicenseText { get; set; }

        public string MachineName { get; set; }

        public string ProductId { get; set; }

        public Guid RequestId { get; set; }

        public string Type { get; set; }

        public string UserAccountId { get; set; }

        #endregion

        public string TableName
        {
            get { return "ClientIssuedLicenses"; }
        }
    }
}