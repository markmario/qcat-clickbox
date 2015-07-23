namespace ClickBox.Web.Models
{
    using System;
    using System.Web.Mvc;

    using ClickBox.Web.TableStorage;

    using Microsoft.WindowsAzure.Storage.Table;
    using Odes.Licence.Model;

    [Bind(Exclude = "Timestamp, TableName, RowKey, PartitionKey, ETag")]
    public class WebLicenseRequest : TableEntity, ILicenseRequest, IContainTableReference
    {
        private string id;

        public WebLicenseRequest()
        {
            this.Id = Guid.NewGuid().ToString();
            this.PartitionKey = TableStorageUtil.GetPartitionPrefix() + 4;
        }

        public LicenceTypes LicenceType { get; set; }

        public int ClicksReqeusted { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ServiceQueue { get; set; }

        public string SystemId { get; set; }

        public Guid ProductId { get; set; }

        public DateTimeOffset SystemDateTimeStamp { get; set; }

        public string SystemNetworkCredential { get; set; }

        public string SystemMachineName { get; set; }

        public Guid RequestId { get; set; }

        public string PublicIp { get; set; }

        public string ProductName { get; set; }

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

        public string TableName
        {
            get { return "LicenseRequests"; }
        }
    }
}