// --------------------------------------------------------------------------------------------------
//  <copyright file="MonthlyIsolatedBatch.cs" company="QCAT Pty Ltd.">
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
    public class MonthlyIsolatedBatch : TableEntity, IContainTableReference
    {
        public MonthlyIsolatedBatch()
        {
            var monthAndYear = DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString();
            this.PartitionKey = TableStorageUtil.GetPartitionPrefix() + monthAndYear;
        }

        public string TableName
        {
            get
            {
                return "MonthlyBatches";
            }
        }
    }
}