// --------------------------------------------------------------------------------------------------
//  <copyright file="PersistedIsolatedBatch.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    using ClickBox.Web.TableStorage;

    using Microsoft.WindowsAzure.Storage.Table;

    using Odes.Licence.Model;

    [Bind(Exclude = "Timestamp, TableName, RowKey, PartitionKey, ETag")]
    public class PersistedIsolatedBatch : TableEntity, IContainTableReference, IBatchIsolated
    {
        private string id;

        private Guid projectId;

        public Guid ProjectId
        {
            get
            {
                return this.projectId;
            }
            set
            {
                this.projectId = value;
                this.PartitionKey = TableStorageUtil.GetPartitionPrefix() + value;
            }
        }
        public Guid BatchId { get; set; }
        public Guid RequestId { get; set; }
        public int DocumentsCreated { get; set; }
        public string MachineName { get; set; }
        public string UserName { get; set; }
        public string SId { get; set; }
        public DateTimeOffset DateCreated { get; set; }

        public override string ToString()
        {
            return this.RowKey + "\t\t" + this.Id + "]";
        }

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

        List<OldDocmentCount> IBatchIsolated.OldBatchValues { get; set; }

        public string OldBatchValues { get; set; }

        public string TableName
        {
            get
            {
                return "BatchesIsolated";
            }
            
        }
    }
}