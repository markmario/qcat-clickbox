// --------------------------------------------------------------------------------------------------
//  <copyright file="UserAccount.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web.ModelBinding;
    using System.Web.Mvc;


    using ClickBox.Web.TableStorage;

    using Microsoft.WindowsAzure.Storage.Table;

    using Rhino.Licensing;
    using System.Collections.Generic;
    using Util;

    [Bind(Exclude = "Timestamp, TableName, RowKey, PartitionKey, ETag")]
    public class UserAccount : TableEntity, IContainTableReference
    {
        private string id;

        #region Public Properties

        [UIHint("ProductTypeEditor")]
        public string Product { get; set; }

        [UIHint("LicenseTypeEditor")]
        public LicenseType AccountType { get; set; }

        public bool Active { get; set; }

        [UIHint("Integer")]
        public int AllocatedSeats { get; set; }

        public string CompanyName { get; set; }

        public string ContactName { get; set; }

        [ScaffoldColumn(false)]
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

        public UserAccount()
        {
            this.Id = Guid.NewGuid().ToString();
            this.PartitionKey = 2.ToString();
        }

        public void InitModelBinderVersion(UserAccount binderAccount)
        {
            binderAccount.Id = Guid.NewGuid().ToString();
            binderAccount.PartitionKey = 2.ToString();
        }

        public override string ToString()
        {
            return this.UserName + "\t\t" + this.RowKey + "]";
        }

        [ScaffoldColumn(false)]
        public string TableName
        {
            get { return "UserAccounts"; }
        }

        [ScaffoldColumn(false)]
        public new string RowKey
        {
            get
            {
                return base.RowKey;
            }
            set
            {
                base.RowKey = value;
            }
        }

        [ScaffoldColumn(false)]
        public new string PartitionKey
        {
            get
            {
                return base.PartitionKey;
            }
            set
            {
                base.PartitionKey = value;
            }
        }

        [BindNever]
        [ScaffoldColumn(false)]
        public new DateTimeOffset Timestamp
        {
            get
            {
                return base.Timestamp;
            }
            set
            {
                base.Timestamp = value;
            }
        }

        [ScaffoldColumn(false)]
        public new string ETag
        {
            get
            {
                return base.ETag;
            }
            set
            {
                base.ETag = value;
            }
        }
    }
}