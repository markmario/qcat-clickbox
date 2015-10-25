using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ClickBox.Web.Models
{
    public class ChargeRequest : TableEntity, IContainTableReference
    {
        public ChargeRequest() { }

        public ChargeRequest(string token)
        {
            PartitionKey = DateTime.Now.Year.ToString();
            RowKey = token;
        }

        public int? Amount { get; set; }
        public string Currency { get; set; }
        public string Email { get; set; }
        public string Organisation { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public Guid SupportId { get; set; }
        public string Token {get { return RowKey; }}

        public string TableName
        {
            get
            {
                return "ChargeRequests";
            }
        }

        public DateTimeOffset DateTimeOfRequest { get; set; }
    }
}