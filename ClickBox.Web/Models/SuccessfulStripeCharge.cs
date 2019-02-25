using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ClickBox.Web.Models
{
    public class SuccessfulStripeCharge : TableEntity, IContainTableReference
    {
        public SuccessfulStripeCharge() { }

        public SuccessfulStripeCharge(string tokenId)
        {
            PartitionKey = DateTime.Now.Year.ToString();
            RowKey = tokenId;
        }
        

        public DateTime DateTimeCreated { get; set; }
        public string Id { get; set; }
        public Guid SupportId { get; set; }

        public string TableName
        {
            get
            {
                return "SuccessfulStripeCharges";
            }
        }

        public string TokenId { get; private set; }
    }
}