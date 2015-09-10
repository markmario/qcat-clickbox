using ClickBox.CreateAccounts.Mail;

namespace ClickBox.CreateAccounts.Messages
{
    public class SendAccountAndDownloadInstructionsMessage : IHaveDataForMail
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

        public string MessageBody
        {
            get; set;
        }

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
    }
}
