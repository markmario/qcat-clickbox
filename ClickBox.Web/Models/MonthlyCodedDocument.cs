// --------------------------------------------------------------------------------------------------
//  <copyright file="MonthlyCodedDocument.cs" company="QCAT Pty Ltd.">
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
    public class MonthlyCodedDocument : TableEntity, IContainTableReference
    {
        public MonthlyCodedDocument()
        {
            var monthAndYear = DateTime.Now.Month + DateTime.Now.Year.ToString();
            this.PartitionKey = monthAndYear;
        }

        public string TableName
        {
            get
            {
                return "MonthlyDocuments";
            }
        }

        public Guid ProjectId { get; set; }
        public string UserName { get; set; }

        public string CompanyName { get; set; }
        public string AccountId { get; set; }
    }
}