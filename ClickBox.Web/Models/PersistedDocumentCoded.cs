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
    public class PersistedDocumentCoded : TableEntity, IDocumentCoded, IContainTableReference
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
                this.PartitionKey = value.ToString();
            }
        }
        public Guid DocumentId { get; set; }
        public Guid RequestId { get; set; }
        public string MachineName { get; set; }
        public string UserName { get; set; }
        public string SId { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public string TableName { get { return "DocumentsCoded"; } }
        public string AccountId { get; set; }
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
    }
}