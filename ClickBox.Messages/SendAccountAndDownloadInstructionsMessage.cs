using ClickBox.Mail;

namespace ClickBox.Messages
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "ArrangeThisQualifier")]
    public class SendAccountAndDownloadInstructionsMessage : IHaveDataForMail, IAzureQueueMessage
    {
        public string ContactName
        {
            get; set;
        }
        public string DowloadLink
        {
            get; set;
        }
        public string From
        {
            get; set;
        }
        public string FromName
        {
            get; set;
        }
        public string Instructions
        {
            get; set;
        }
        public string LicenseName { get; set; }
        public string MessageBody
        {
            get; set;
        }
        public string Password { get; set; }
        public string ProductName { get; set; }
        public bool PaymentReceived { get; set; }
        public string To
        {
            get; set;
        }
        public override string ToString()
        {
            return "{From:" + From + ", FromName:" + FromName + "," +
                    "To:" + To + ", ContactName:" + ContactName + ", " +
                    "MessageBody:" + MessageBody + ", DowloadLink:" + DowloadLink + "}";
        }

        public string QueueName => "account-created";
    }
}
