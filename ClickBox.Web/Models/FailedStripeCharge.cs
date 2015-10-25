using Microsoft.WindowsAzure.Storage.Table;
using Stripe;
using System;
using System.Net;

namespace ClickBox.Web.Models
{
    public class FailedStripeCharge<T> : TableEntity, IContainTableReference where T : Exception
    {
        public FailedStripeCharge() { }

        public FailedStripeCharge(string tokenId)
        {
            PartitionKey = DateTime.Now.Year.ToString();
            RowKey = tokenId;
        }

        public string Error { get; set; }
        public string Code { get; set; }
        internal T Exception { get; set; }
        public string HttpStatusCode { get; set; }

        public bool IsStripeException
        {
            get
            {
                return typeof(T) == typeof(StripeException);
            }
        }

        public string ErrorMessage { get; set; }
        public string ExceptionMessage { get; set; }
        public Guid SupportId { get; set; }
        public string ErrorCode { get; set; }

        public static string NotApplicableDescriptor { get { return "NA"; } }

        public string TableName
        {
            get
            {
                return "FailedStripeCharges";
            }
        }
    }
}