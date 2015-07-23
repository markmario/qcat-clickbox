namespace ClickBox.Web.Models
{
    using System;
    using System.Web.Mvc;

    using Microsoft.WindowsAzure.Storage.Table;
    using Odes.Licence.Model;

    [Bind(Exclude = "Timestamp, TableName, RowKey, PartitionKey, ETag")]
    public class WebLicenseRequest : TableEntity, ILicenseRequest, IContainTableReference
    {
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

        public string TableName
        {
            get { return "LicenseRequests"; }
        }
    }
}