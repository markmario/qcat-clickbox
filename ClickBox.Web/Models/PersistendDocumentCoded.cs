// --------------------------------------------------------------------------------------------------
//  <copyright file="DocumentCoded.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Models
{
    using System;
    using System.Web.Mvc;
    using ClickBox.Web.TableStorage;
    using Microsoft.WindowsAzure.Storage.Table;
    using Odes.Licence.Model;

    [Bind(Exclude = "Timestamp, TableName, RowKey, PartitionKey, ETag")]
    public class PersistendDocumentCoded : TableEntity, IDocumentCoded, IContainTableReference
    {
        public PersistendDocumentCoded()
        {
            var monthAndYear = DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString();
            this.PartitionKey = TableStorageUtil.GetPartitionPrefix() + monthAndYear;
        }
        public string Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid DocumentId { get; set; }
        public Guid RequestId { get; set; }
        public string MachineName { get; set; }
        public string UserName { get; set; }
        public string SId { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public string TableName { get { return "DocumentsCoded"; } }
    }
}